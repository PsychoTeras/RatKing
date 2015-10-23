#define UNSAFE_ARRAY

using System.Threading;
using System.Threading.Tasks;
using RK.Common.Classes.Map;
using RK.Common.Proto;
using RK.Common.Proto.Packets;
#if UNSAFE_ARRAY
using System.Runtime.InteropServices;
#endif
using RK.Common.Win32;

namespace RK.Console
{
    unsafe class Program
    {
        const ushort MapWidth = 1000, MapHeight = 1000;
        private static Tile EmptyTile = new Tile();

        internal void TestGameMapPerf()
        {
            int count = MapWidth * MapHeight;
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

            using (GameMap map = new GameMap(MapWidth, MapHeight, 0))
            {
                timer = HRTimer.CreateAndStart();

                for (ushort y = 0; y < MapHeight; y++)
                {
                    for (ushort x = 0; x < MapWidth; x++)
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
                UserName = "psychoteras",
                Password = "password"
            };
            p.Setup();
            byte[] ps = p.Serialize();
            BasePacket.Deserialize(ps, ps.Length, 0, out psize);

            HRTimer timer = HRTimer.CreateAndStart();

            //Parallel.For(0, 1000000, i =>
            for (int i = 0; i < 1000000; i++)
            {
                p = new PUserLogin
                {
                    UserName = "psychoteras",
                    Password = "password"
                };
                p.Setup();
                ps = p.Serialize();
                p = (PUserLogin)BasePacket.Deserialize(ps, ps.Length, 0, out psize);
            }
//            });

            System.Console.WriteLine(timer.StopWatch());
        }

        internal static void TestMapWindowgetPerf()
        {
            using (GameMap map = new GameMap(MapWidth, MapHeight, 0))
            {
                Parallel.For(0, MapHeight, y => Parallel.For(0, MapWidth, x =>
                {
                    Tile* tile = map[(ushort)x, (ushort)y];
                    (*tile).Flags = x;
                    (*tile).RTFlags = y;
                }));
                
                HRTimer timer = HRTimer.CreateAndStart();
                int tileSize = EmptyTile.SizeOf(), dataSize = MapWidth * MapHeight * tileSize;
                
                byte[] tilesData = new byte[sizeof(int) + dataSize];
                fixed (byte* bTileData = tilesData)
                {
                    int pos = 0;
                    byte* bData = bTileData;
                    Serializer.Write(bData, dataSize, ref pos);
                    Parallel.For(0, MapHeight, y => Parallel.For(0, MapWidth, x =>
                    {
                        pos = sizeof(int) + y*MapWidth + x;
                        Tile* tile = map[(ushort)x, (ushort)y];
                        (*tile).Serialize(&bData[pos], ref pos);
                    }));
                }

                System.Console.WriteLine(timer.StopWatch());
            }
        }

        static void Main(string[] args)
        {
            TestMapWindowgetPerf();
            System.Console.ReadKey();
        }
    }
}
