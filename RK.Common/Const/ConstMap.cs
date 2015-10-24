using RK.Common.Classes.Map;

namespace RK.Common.Const
{
    public static class ConstMap
    {
        public const int PIXEL_SIZE = 15;
        public const int PIXEL_SIZE_SQR = PIXEL_SIZE * PIXEL_SIZE;

        internal static int TILE_SIZE_OF = new Tile().SizeOf();
    }
}
