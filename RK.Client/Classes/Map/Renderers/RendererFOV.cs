using System.Drawing;
using System.Drawing.Drawing2D;
using RK.Client.Controls;
using RK.Common.Classes.Units;
using RK.Common.Map;

namespace RK.Client.Classes.Map.Renderers
{
    public class RendererFOV : IMapRenderer
    {

#region Constants

        public const int PAD = 180;

#endregion

#region Private fields

        private Bitmap _fovBitmap;
        private Bitmap _fovRotatedBitmap;
        private Graphics _fovRotated;
        private RectangleF _fovRect;
        private float _prewPlayerAngle;

        private SolidBrush _fillBrush;

#endregion

#region Ctor

        public RendererFOV()
        {
            _fillBrush = new SolidBrush(Color.Black);
            _fovBitmap = (Bitmap) Image.FromFile(@"Resources\fov.png");
            _fovRect = new RectangleF(PAD, PAD, _fovBitmap.Width - PAD * 2,
                                      _fovBitmap.Height - PAD * 2);
            _fovRotatedBitmap = new Bitmap(_fovBitmap.Width, _fovBitmap.Height);
            _fovRotated = Graphics.FromImage(_fovRotatedBitmap);
            _fovRotated.InterpolationMode = InterpolationMode.Low;
            _fovRotated.SmoothingMode = SmoothingMode.HighSpeed;
            _prewPlayerAngle = 1;
        }

#endregion

        public void Render(MapControl mapCtrl, Graphics buffer, Rectangle area)
        {
            ClientMap map = mapCtrl.Map;
            Player player = mapCtrl.PlayerData;
            if (map == null || player == null)
            {
                return;
            }

            int posX = (int)(player.Position.X + player.Size.Width / 2 - area.X - _fovRect.Width / 2);
            int posY = (int)(player.Position.Y + player.Size.Height / 2 - area.Y - _fovRect.Height / 2);

            float pSizeW = _fovBitmap.Width, pSizeHw = (float)_fovBitmap.Width / 2;
            float pSizeH = _fovBitmap.Height, pSizeHh = (float)_fovBitmap.Height / 2;

            if (player.Angle != _prewPlayerAngle)
            {
                _fovRotated.Clear(Color.FromArgb(0, 0, 0, 0));
                _fovRotated.TranslateTransform(pSizeHw, pSizeHh);
                _fovRotated.RotateTransform(player.Angle - _prewPlayerAngle);
                _fovRotated.TranslateTransform(-pSizeHw, -pSizeHh);
                _fovRotated.DrawImage(_fovBitmap, 0, 0);
                _prewPlayerAngle = player.Angle;
            }

            buffer.DrawImage(_fovRotatedBitmap, posX, posY, _fovRect, GraphicsUnit.Pixel);

            if (posX > 0)
            {
                buffer.FillRectangle(_fillBrush, new Rectangle(0, 0, posX, area.Height));
            }
            if (posY > 0)
            {
                buffer.FillRectangle(_fillBrush, new Rectangle(posX, 0, area.Width - posX, posY));
            }
            if (posX + _fovRect.Width < area.X + area.Width)
            {
                buffer.FillRectangle(_fillBrush, new Rectangle((int)(posX + _fovRect.Width), posY,
                    area.Width - posX, area.Height - posY));
            }
            if (posY + _fovRect.Height < area.Y + area.Height)
            {
                buffer.FillRectangle(_fillBrush, new Rectangle(posX, (int)(posY + _fovRect.Height),
                    area.Width - posX, area.Height - posY));
            }
        }

        public void ChangeScaleFactor(MapControl mapCtrl, float scaleFactor) { }

        public void ChangeMap(MapControl mapCtrl) {}

        public void UpdateTile(MapControl mapCtrl, ushort x, ushort y, Tile tile) { }
    }
}
