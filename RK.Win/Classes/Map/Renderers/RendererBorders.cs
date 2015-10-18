using System;
using System.Drawing;
using RK.Common.Classes.Map;
using RK.Common.Const;
using RK.Win.Controls;

namespace RK.Win.Classes.Map.Renderers
{
    public unsafe class RendererBorders : IMapRenderer
    {

#region Private fields

        private Pen _borderPen = new Pen(Color.DimGray);

#endregion

#region IMapRenderer

        public void Render(MapControl mapCtrl, Graphics buffer, Rectangle area)
        {
            GameMap map = mapCtrl.Map;
            if (map == null)
            {
                return;
            }
            
            float scale = mapCtrl.ScaleFactor;
            float pixelSize = ConstMap.PIXEL_SIZE * scale;

            ushort mapX1 = (ushort)Math.Max(Math.Floor(mapCtrl.PosX / pixelSize - 1), 0);
            ushort mapY1 = (ushort)Math.Max(Math.Floor(mapCtrl.PosY / pixelSize - 1), 0);
            ushort mapX2 = (ushort)Math.Min(Math.Ceiling(mapCtrl.Width / scale / pixelSize + 2) + mapX1, map.Width);
            ushort mapY2 = (ushort)Math.Min(Math.Ceiling(mapCtrl.Height / scale / pixelSize + 2) + mapY1, map.Height);
            
            for (ushort y = mapY1; y < mapY2; y++)
            {
                for (ushort x = mapX1; x < mapX2; x++)
                {
                    Tile* tile = map[x, y];
                    if ((*tile).Type == TileType.Wall)
                    {
                        float x1 = x*pixelSize - mapCtrl.PosX;
                        float y1 = y*pixelSize - mapCtrl.PosY;

                        float x2, y2;
                        int borders = (*tile).Borders;
                        if ((borders | 1) == borders)
                        {
                            y2 = y1 + pixelSize - 1;
                            buffer.DrawLine(_borderPen, x1, y1, x1, y2);
                        }
                        if ((borders | 2) == borders)
                        {
                            x2 = x1 + pixelSize - 1;
                            y2 = y1 + pixelSize - 1;
                            buffer.DrawLine(_borderPen, x2, y1, x2, y2);
                        }
                        if ((borders | 4) == borders)
                        {
                            x2 = x1 + pixelSize - 1;
                            buffer.DrawLine(_borderPen, x1, y1, x2, y1);
                        }
                        if ((borders | 8) == borders)
                        {
                            x2 = x1 + pixelSize - 1;
                            y2 = y1 + pixelSize - 1;
                            buffer.DrawLine(_borderPen, x1, y2, x2, y2);
                        }
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
