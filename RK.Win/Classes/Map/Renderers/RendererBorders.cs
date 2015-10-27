using System;
using System.Drawing;
using RK.Common.Const;
using RK.Common.Map;
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
            ClientMap map = mapCtrl.Map;
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
                    if (tile != null && (*tile).Type == TileType.Wall)
                    {
                        float x1 = x*pixelSize - mapCtrl.PosX;
                        float y1 = y*pixelSize - mapCtrl.PosY;

                        int borders = map.Borders(x, y);
                        if (borders == 0)
                        {
                            borders = TileBorders.ScanAndSetBorders(x, y, TileType.Wall, map);
                        }

                        if (borders < 2)
                        {
                            continue;
                        }

                        float x2, y2;
                        if ((borders | 2) == borders)
                        {
                            y2 = y1 + pixelSize - 0;
                            buffer.DrawLine(_borderPen, x1, y1, x1, y2);
                        }
                        if ((borders | 4) == borders)
                        {
                            x2 = x1 + pixelSize - 0;
                            y2 = y1 + pixelSize - 0;
                            buffer.DrawLine(_borderPen, x2, y1, x2, y2);
                        }
                        if ((borders | 8) == borders)
                        {
                            x2 = x1 + pixelSize - 0;
                            buffer.DrawLine(_borderPen, x1, y1, x2, y1);
                        }
                        if ((borders | 16) == borders)
                        {
                            x2 = x1 + pixelSize - 0;
                            y2 = y1 + pixelSize - 0;
                            buffer.DrawLine(_borderPen, x1, y2, x2, y2);
                        }
                    }
                }
            }
        }

        public void ChangeScaleFactor(MapControl mapCtrl, float scaleFactor) { }

        public void ChangeMap(MapControl mapCtrl) { }

        public void UpdateTile(MapControl mapCtrl, ushort x, ushort y, Tile tile)
        {
            ushort minX = (ushort) Math.Max(x - 1, 0);
            ushort maxX = (ushort) Math.Min(x + 1, mapCtrl.Map.Width - 1);
            ushort minY = (ushort) Math.Max(y - 1, 0);
            ushort maxY = (ushort) Math.Min(y + 1, mapCtrl.Map.Height - 1);
            for (y = minY; y <= maxY; y++)
            {
                for (x = minX; x <= maxX; x++)
                {
                    mapCtrl.Map.FlagClearBorders(x, y);    
                }
            }
        }

#endregion

    }
}
