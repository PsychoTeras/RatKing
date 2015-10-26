#define UNSAFE_ARRAY

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RK.Common.Classes.Units;
using RK.Common.Map;
using RK.Common.Proto;
using RK.Common.Proto.Packets;
using RK.Common.Win32;

#if UNSAFE_ARRAY
using System.Runtime.InteropServices;
#endif

namespace RK.Console
{
    unsafe class Program
    {
        private const int THREADS_COUNT = 8;
        private const int ITERATIONS_COUNT = 1000000;

        private static WaitHandle[] _tEvents;

        internal void TestGameMapPerf()
        {
            const ushort mapWidth = 1000, mapHeight = 1000;

            int count = mapWidth * mapHeight;
            HRTimer timer = HRTimer.CreateAndStart();

#if UNSAFE_ARRAY
            int sizeOfTile = Marshal.SizeOf(typeof(Tile));
            Tile* pTiles = (Tile*)Memory.HeapAlloc(count * sizeOfTile);
#else
            Tile[,] aTiles = new Tile[MapWidth, MapHeight];
#endif
            System.Console.WriteLine(timer.StopWatch());

            timer = HRTimer.CreateAndStart();
            for (int i = 0; i < count; i++)
            {
#if !UNSAFE_ARRAY
                fixed (Tile* pTiles = aTiles)
#endif
                {
                    pTiles[i].Type = TileType.Wall;
                    pTiles[i].TypeIndex = 100;
                }
            }
            System.Console.WriteLine(timer.StopWatch());
#if UNSAFE_ARRAY
            Memory.HeapFree(pTiles);
#endif

            using (ServerMap map = new ServerMap(mapWidth, mapHeight, 0))
            {
                timer = HRTimer.CreateAndStart();

                for (ushort y = 0; y < mapHeight; y++)
                {
                    for (ushort x = 0; x < mapWidth; x++)
                    {
                        Tile* tile = map[x, y];
                        (*tile).Type = TileType.Nothing;
                        (*tile).TypeIndex = 1;
                    }
                }
                System.Console.WriteLine(timer.StopWatch());
            }
        }

        internal static void TestProtoPacketsPerf()
        {
            short psize;
            PUserLogin p = new PUserLogin
            {
                UserName = "PsychoTeras",
                Password = "password"
            };
            p.Setup();
            byte[] ps = p.Serialize();
            BasePacket.Deserialize(ps, ps.Length, 0, out psize);

            HRTimer timer = HRTimer.CreateAndStart();

            Parallel.For(0, ITERATIONS_COUNT, i =>
            {
                p.Setup();
                ps = p.Serialize();
                BasePacket.Deserialize(ps, ps.Length, 0, out psize);
            });

            System.Console.WriteLine(timer.StopWatch());
        }

        internal static void TestMapWindowGetPerf()
        {
            using (ServerMap serverMap = ServerMap.LoadFromFile("RK.save"))
            {
                ServerMap map = serverMap;

                HRTimer timer = HRTimer.CreateAndStart();

                for (int i = 0; i < 100; i++)
                {
                    int startX = 0, startY = 0;
                    int wWidth = 150, wHeight = 150;
                    map.GetWindow(startX, startY, wWidth, wHeight);
                }
                System.Console.WriteLine(timer.StopWatch());
            }
        }

        private static void DoTestMultithreadDictionary(object obj)
        {
            int idx = (int)((object[])obj)[0];
            var dict = (Dictionary<long, int>) ((object[]) obj)[1];
            var ev = (EventWaitHandle)((object[])obj)[2];

            if (idx%2 == 0)
            {
                for (int i = 0; i < ITERATIONS_COUNT; i++)
                {
                    lock (dict)
                    {
                        dict.Add(idx*ITERATIONS_COUNT + i, i);
                    }
                }
            }
            else
            {
                for (int i = 0; i < ITERATIONS_COUNT; i++)
                {
                    int key = (idx - 1)*ITERATIONS_COUNT + i;
                    if (dict.ContainsKey(key))
                    {
                        int val;
                        dict.TryGetValue(key, out val);
                    }
                }
            }
            ev.Set();
        }

        internal static void TestMultithreadDictionaryPerf()
        {
            var dict = new Dictionary<long, int>();

            _tEvents = new WaitHandle[THREADS_COUNT];
            for (int i = 0; i < THREADS_COUNT; i++)
            {
                _tEvents[i] = new ManualResetEvent(false);
            }

            HRTimer timer = HRTimer.CreateAndStart();
            for (int i = 0; i < THREADS_COUNT; i++)
            {
                object[] param = { i, dict, _tEvents[i] };
                ThreadPool.QueueUserWorkItem(DoTestMultithreadDictionary, param);
            }
            WaitHandle.WaitAll(_tEvents);

            System.Console.WriteLine(timer.StopWatch());
        }

        private static void DoTestMultithreadList(object obj)
        {
            int idx = (int)((object[])obj)[0];
            var list = (List<Player>)((object[])obj)[1];
            var ev = (EventWaitHandle)((object[])obj)[2];
//            var sync = (ReaderWriterLockSlim)((object[])obj)[3];

            if (idx % 2 == 0)
            {
                for (int i = 0; i < ITERATIONS_COUNT; i++)
                {
//                    sync.EnterWriteLock();
                    lock (list)
                    {
                        list.Add(new Player {Id = idx*ITERATIONS_COUNT + i});
                    }
//                    sync.ExitWriteLock();
                }
            }
            else
            {
                for (int i = 0; i < ITERATIONS_COUNT; i++)
                {
//                    sync.EnterReadLock();
                    lock (list)
                    {
                        if (list.Count > i)
                        {
                            list[i].MapId++;
                        }
                    }
//                    sync.ExitReadLock();
                }
            }
            ev.Set();
        }

        internal static void TestMultithreadListPerf()
        {
            var list = new List<Player>();
            var sync = new ReaderWriterLockSlim();

            _tEvents = new WaitHandle[THREADS_COUNT];
            for (int i = 0; i < THREADS_COUNT; i++)
            {
                _tEvents[i] = new ManualResetEvent(false);
            }

            HRTimer timer = HRTimer.CreateAndStart();
            for (int i = 0; i < THREADS_COUNT; i++)
            {
                object[] param = { i, list, _tEvents[i], sync };
                ThreadPool.QueueUserWorkItem(DoTestMultithreadList, param);
            }
            WaitHandle.WaitAll(_tEvents);

            System.Console.WriteLine(timer.StopWatch());
        }

        internal static void TestCollectionsPerf()
        {
            var list = new List<Player>();

            HRTimer timer = HRTimer.CreateAndStart();
            for (int i = 0; i < ITERATIONS_COUNT; i++)
            {
                lock (list)
                {
                    list.Add(new Player {Id = i});
                }
            }
            System.Console.WriteLine(timer.StopWatch());

            timer = HRTimer.CreateAndStart();
            for (int i = 0; i < list.Count; i++)
            {
                Player player = list[i];
                if (player.Id != -1)
                {
                    player.Id++;
                }
            }
            System.Console.WriteLine(timer.StopWatch());

            timer = HRTimer.CreateAndStart();
            list.RemoveAll(p => p.Id > 0);
            System.Console.WriteLine(timer.StopWatch());
        }

        static void Main(string[] args)
        {
            TestCollectionsPerf();
            System.Console.ReadKey();
        }
    }
}
