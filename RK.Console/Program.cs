#define UNSAFE_ARRAY

using RK.Common.Classes.Map;
#if UNSAFE_ARRAY
using System.Runtime.InteropServices;
#endif
using RK.Common.Win32;

namespace RK.Console
{
    unsafe class Program
    {
        internal void TestGameMapPerf()
        {
            ushort width = 10000, height = 10000;
            int count = width * height;
            HRTimer timer = HRTimer.CreateAndStart();

#if UNSAFE_ARRAY
            int sizeOfTile = Marshal.SizeOf(typeof(Tile));
            Tile* pTiles = (Tile*)Memory.Alloc(count * sizeOfTile);
#else
            Tile[,] aTiles = new Tile[width, height];
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
            Memory.Free(pTiles);
#endif

            using (GameMap map = new GameMap(width, height, 0))
            {
                timer = HRTimer.CreateAndStart();

                for (ushort y = 0; y < height; y++)
                {
                    for (ushort x = 0; x < width; x++)
                    {
                        Tile* tile = map[x, y];
                        (*tile).Type = TileType.Undefined;
                        (*tile).TypeIndex = 1;
                    }
                }
                System.Console.WriteLine(timer.StopWatch());
            }
        }

        static void Main(string[] args)
        {


            System.Console.ReadKey();
        }
    }
}
