using System;
using System.Drawing;
using RK.Common.Classes.Map;
using RK.Win.Controls;

namespace RK.Win.Classes.Map.Renderers
{
    public class RendererBG : IMapRenderer, IDisposable
    {

#region Private fields

        private MapBG _map;
        private Brush _textBrush;
        private StringFormat _textFormat;

#endregion

#region Ctor

        public RendererBG(float scaleFactor)
        {
            _textFormat = new StringFormat();
            _textFormat.Alignment = StringAlignment.Center;
            _textFormat.LineAlignment = StringAlignment.Center;
            _map = new MapBG(scaleFactor, true, new Size(6, 6));
        }

#endregion

#region IMapRenderer

        public void Render(MapControl mapCtrl, Graphics buffer, Rectangle area)
        {
            if (_textBrush == null)
            {
                _textBrush = new SolidBrush(mapCtrl.ForeColor);
            }

            int startTileX, startTileY, tilesCntX, tilesCntY;
            _map.InvalidateArea(area, out startTileX, out startTileY, out tilesCntX, out tilesCntY);
            for (int x = startTileX; x < startTileX + tilesCntX; x++)
            {
                for (int y = startTileY; y < startTileY + tilesCntY; y++)
                {
                    Rectangle destRect = new Rectangle(x * _map.TileWidth - area.X,
                        y * _map.TileHeight - area.Y, _map.TileWidth, _map.TileHeight);
                    buffer.DrawImageUnscaledAndClipped(_map[x, y], destRect);
                    if (mapCtrl.ShowTileNumber)
                    {
                        buffer.DrawString(string.Format("{0}:{1}", x, y), mapCtrl.Font, _textBrush,
                            destRect, _textFormat);
                    }
                }
            }
        }

        public void ChangeScaleFactor(MapControl mapCtrl, float scaleFactor)
        {
            _map.Initialize(scaleFactor, true);
        }

        public void ChangeMap(MapControl mapCtrl) { }

        public void UpdateTile(MapControl mapCtrl, ushort x, ushort y, Tile tile) { }

        public void Dispose()
        {
            _map.Dispose();
            _textFormat.Dispose();
            if (_textBrush != null)
            {
                _textBrush.Dispose();
            }
        }

#endregion

    }
}
