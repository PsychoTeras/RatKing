#define UNSAFE_ARRAY
#define MEM_TILES_COMPRESSION_

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RK.Common.Classes;
using RK.Common.Classes.Map;
using RK.Common.Proto;
using RK.Common.Proto.Packets;
using RK.Common.Win32;

#if UNSAFE_ARRAY
using System.Runtime.InteropServices;
#endif

#if MEM_TILES_COMPRESSION
using System.IO;
using System.IO.Compression;
using RK.Common.Compress;
#endif

namespace RK.Console
{
    unsafe class Program
    {
        private const int THREADS_COUNT = 8;
        private const int ITERATIONS_COUNT = 100000;

        private static WaitHandle[] _tEvents;
        private static Tile _emptyTile = new Tile();

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

            using (GameMap map = new GameMap(mapWidth, mapHeight, 0))
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
            string text = File.ReadAllText(@"d:\DICOM files\DOT\DicomSender.txt");
            PUserLogin p = new PUserLogin
            {
                UserName = "PsychoTeras",
                Password = "password"
            };
            p.Setup();
            byte[] ps = p.Serialize();
            var dd = BasePacket.Deserialize(ps, ps.Length, 0, out psize);

            HRTimer timer = HRTimer.CreateAndStart();

            Parallel.For(0, 1000000, i =>
            {
                p.Setup();
                ps = p.Serialize();
                BasePacket.Deserialize(ps, ps.Length, 0, out psize);
            });

            System.Console.WriteLine(timer.StopWatch());
        }

        internal static void TestMapWindowgetPerf()
        {
            using (GameMap gameMap = GameMap.LoadFromFile("RK.save"))
            {
                GameMap map = gameMap;

                HRTimer timer = HRTimer.CreateAndStart();

                int mapWidth = map.Width;
                int startX = 0, startY = 0;
                int wWidth = 150, wHeight = 150;
                int endX = startX + wWidth, endY = startY + wHeight;

                int smallSimilarsCnt = 0;
                int smallSimilarCntLim = byte.MaxValue / 2;
                ArrayList tilesInfo = new ArrayList();
                for (int y = startY; y < endY; y++)
                {
                    for (int x = startX; x < endX; x++)
                    {
                        Tile tile = *map[y * mapWidth + x];

                        //Find all similar tiles in a row
                        ushort similarTilesCnt = 1;
                        while (similarTilesCnt < short.MaxValue - 1)
                        {
                            int xn = x + 1, yn = y;
                            if (xn == endX)
                            {
                                xn = startX;
                                yn++;
                            }
                            if (yn == endY || *map[yn * mapWidth + xn] != tile) break;

                            similarTilesCnt++;
                            x = xn;
                            y = yn;
                        }

                        if (similarTilesCnt <= smallSimilarCntLim)
                        {
                            smallSimilarsCnt++;
                        }

                        //Add tile info
                        tilesInfo.Add(new Pair<Tile, ushort>(tile, similarTilesCnt));
                    }
                }

                int tilesCount = tilesInfo.Count;
                int dataSize = sizeof (byte)*smallSimilarsCnt +
                               sizeof (ushort)*(tilesCount - smallSimilarsCnt) +
                               _emptyTile.SizeOf()*tilesCount;
                byte[] tilesData = new byte[dataSize];
                fixed (byte* bData = tilesData)
                {
                    int pos = 0;
                    foreach (Pair<Tile, ushort> tileInfo in tilesInfo)
                    {
                        if (tileInfo.Value <= smallSimilarCntLim)
                        {
                            Serializer.Write(bData, (byte) tileInfo.Value, ref pos);
                        }
                        else
                        {
                            Serializer.Write(bData, tileInfo.Value, ref pos);
                        }
                        tileInfo.Key.Serialize(bData, ref pos);
                    }
                }

#if MEM_TILES_COMPRESSION
//                QuickLZ c = new QuickLZ();
//                byte[] compressed = c.Compress(tilesData);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (DeflateStream compressor = new DeflateStream(ms, CompressionLevel.Fastest))
                    {
                        compressor.Write(tilesData, 0, dataSize);
                    }
                    tilesData = ms.ToArray();
                }
#endif

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
                object[] param = {i, dict, _tEvents[i]};
                ThreadPool.QueueUserWorkItem(DoTestMultithreadDictionary, param);
            }
            WaitHandle.WaitAll(_tEvents);

            System.Console.WriteLine(timer.StopWatch());
        }

        static void Main(string[] args)
        {
            TestProtoPacketsPerf();
            System.Console.ReadKey();
        }
    }
}
