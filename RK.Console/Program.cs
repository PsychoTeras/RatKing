#define UNSAFE_ARRAY
#define MEM_TILES_COMPRESSION_

using System.Collections;
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
            PUserLogin p = new PUserLogin
            {
                UserName = "psychoteras",
                Password = "password"
            };
            p.Setup();
            byte[] ps = p.Serialize();
            BasePacket.Deserialize(ps, ps.Length, 0, out psize);

            HRTimer timer = HRTimer.CreateAndStart();

            Parallel.For(0, 1000000, i =>
            {
                p = new PUserLogin
                {
                    UserName = "psychoteras",
                    Password = "password"
                };
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

        static void Main(string[] args)
        {
            TestMapWindowgetPerf();
            System.Console.ReadKey();
        }
    }
}
