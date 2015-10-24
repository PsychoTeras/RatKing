using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using RK.Common.Classes.Map;
using RK.Common.Const;
using RK.Win.Controls;

namespace RK.Win.Classes.Map.Renderers
{
    public unsafe class RendererWalls : IMapRenderer
    {

#region Private fields

        private TextureBrush _fillBrush;

#endregion

#region Ctor

        public RendererWalls()
        {
//            _fillBrush = new SolidBrush(Color.Black);
            _fillBrush = new TextureBrush(Image.FromFile(@"Resources\rock.png"), WrapMode.Tile);
        }

#endregion

#region IMapRenderer

        public void Render(MapControl mapCtrl, Graphics buffer, Rectangle area)
        {
            if (mapCtrl.ClientMap == null)
            {
                return;
            }

            float scale = mapCtrl.ScaleFactor;
            float pixelSize = ConstMap.PIXEL_SIZE * scale;
            ClientMap map = mapCtrl.ClientMap;

            ushort mapX1 = (ushort)Math.Max(Math.Floor(mapCtrl.PosX / pixelSize - 1), 0);
            ushort mapY1 = (ushort)Math.Max(Math.Floor(mapCtrl.PosY / pixelSize - 1), 0);
            ushort mapX2 = (ushort)Math.Min(Math.Ceiling(mapCtrl.Width / scale / pixelSize + 2) + mapX1, map.Width);
            ushort mapY2 = (ushort)Math.Min(Math.Ceiling(mapCtrl.Height / scale / pixelSize + 2) + mapY1, map.Height);

            for (ushort y = mapY1; y < mapY2; y++)
            {
                for (ushort x = mapX1; x < mapX2; x++)
                {
                    Tile* tile = map[x, y];
                    if (tile != null && (*tile).Type == TileType.Wall)
                    {
                        float pixXToRealX = x * pixelSize - mapCtrl.PosX;
                        float pixXToRealY = y * pixelSize - mapCtrl.PosY;
                        RectangleF rect = new RectangleF(
                            pixXToRealX - -0,
                            pixXToRealY - -0,
                            pixelSize + -0,
                            pixelSize + -0);
                        buffer.FillRectangle(_fillBrush, rect);
                    }
                }
            }
        }

        public void ChangeScaleFactor(MapControl mapCtrl, float scaleFactor) { }

        public void ChangeMap(MapControl mapCtrl) { }

        public void UpdateTile(MapControl mapCtrl, ushort x, ushort y, Tile tile) { }

#endregion

    }
}
