using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using AccidentalNoise;
using RK.Common.Classes.Common;
using RK.Common.Host;
using RK.Common.Map;
using RK.Common.Win32;

namespace RK.Win.Forms
{
    public partial class frmMain : Form
    {
        private GameHost _host;
        private Point? _mousePos;
        private byte[] _keys = new byte[256];

        [DllImport("user32.dll")]
        private static extern int GetKeyboardState(byte[] keystate);

        private object[] GetEnumValues(Type e)
        {
            return Enum.GetValues(e).OfType<object>().ToArray();
        }

        public frmMain()
        {
            InitializeComponent();

            if (Environment.CommandLine.Trim().Split(new[] {' '}).Length == 2)
            {
                _host = new GameHost();
            }

            cbLabyrinthType.Items.AddRange(GetEnumValues(typeof(FractalType)));
            cbLabyrinthType.SelectedItem = FractalType.FBM;

            cbLabyrinthBasis.Items.AddRange(GetEnumValues(typeof(BasisTypes)));
            cbLabyrinthBasis.SelectedItem = BasisTypes.GRADVAL;

            cbLabyrinthInterp.Items.AddRange(GetEnumValues(typeof(InterpTypes)));
            cbLabyrinthInterp.SelectedItem = InterpTypes.LINEAR;

            cbLabyrinthACCombine.Items.AddRange(GetEnumValues(typeof(CombinerTypes)));
            cbLabyrinthACCombine.SelectedItem = CombinerTypes.MULT;
        }

        private bool KeyPressed(Keys keys)
        {
            GetKeyboardState(_keys);
            return (_keys[(int) keys] & 128) == 128;
        }

        private Direction GetPlayerMoveDirection()
        {
            Direction direction = Direction.None;
            if (KeyPressed(Keys.W))
            {
                direction = KeyPressed(Keys.D)
                    ? Direction.NE
                    : KeyPressed(Keys.A)
                        ? Direction.NW
                        : Direction.N;
            }
            if (KeyPressed(Keys.S))
            {
                direction = KeyPressed(Keys.D)
                    ? Direction.SE
                    : KeyPressed(Keys.A)
                        ? Direction.SW
                        : Direction.S;
            }
            if (KeyPressed(Keys.A))
            {
                direction = KeyPressed(Keys.W)
                    ? Direction.NW
                    : KeyPressed(Keys.S)
                        ? Direction.SW
                        : Direction.W;
            }
            if (KeyPressed(Keys.D))
            {
                direction = KeyPressed(Keys.W)
                    ? Direction.NE
                    : KeyPressed(Keys.S)
                        ? Direction.SE
                        : Direction.E;
            }
            return direction;
        }

        private void MapMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && ModifierKeys == Keys.Control)
            {
                mapCtrl.ToggleWallUnderCursor(e.Location);
            }
            if (e.Button == MouseButtons.Left && ModifierKeys == (Keys.Control | Keys.Alt))
            {
                mapCtrl.ExplodeWallsUnderCursor(e.Location, 10);
            }
            if (e.Button == MouseButtons.Right)
            {
                mapCtrl.Cursor = Cursors.SizeAll;
                _mousePos = e.Location;
            }
        }

        private void MapMouseMove(object sender, MouseEventArgs e)
        {
            if (_mousePos != null)
            {
                mapCtrl.ShiftPosition(_mousePos.Value.X - e.X, _mousePos.Value.Y - e.Y);
                _mousePos = e.Location;
            }
            ShortPoint? tilePos = mapCtrl.CursorToTilePos(e.Location);
            lblLabyrinthTilePos.Text = tilePos.HasValue
                ? string.Format("Tile: {0} x {1}", tilePos.Value.X, tilePos.Value.Y)
                : string.Format("No tile under cursor");
        }

        private void MapMouseUp(object sender, MouseEventArgs e)
        {
            if (_mousePos != null)
            {
                mapCtrl.Cursor = Cursors.Default;
                _mousePos = null;
            }
        }

        private void MapMouseWheel(object sender, int delta)
        {
            mapCtrl.ScaleFactor += 0.07f * delta;
        }

        private uint? _labOldSeed;

        private unsafe void BtnGenerateLabyrinthClick(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            FractalType type = (FractalType) cbLabyrinthType.SelectedItem;
            BasisTypes basis = (BasisTypes) cbLabyrinthBasis.SelectedItem;
            InterpTypes interp = (InterpTypes) cbLabyrinthInterp.SelectedItem;
            int? octaves = !string.IsNullOrEmpty(tbLabyrinthOctaves.Text)
                ? int.Parse(tbLabyrinthOctaves.Text)
                : (int?) null;
            double? frequency = !string.IsNullOrEmpty(tbLabyrinthFrequency.Text)
                ? double.Parse(tbLabyrinthFrequency.Text)
                : (double?) null;
            double? angle = !string.IsNullOrEmpty(tbLabyrinthAngle.Text)
                ? double.Parse(tbLabyrinthAngle.Text)
                : (double?) null;
            double? lacunarity = !string.IsNullOrEmpty(tbLabyrinthLacunarity.Text)
                ? double.Parse(tbLabyrinthLacunarity.Text)
                : (double?) null;
            _labOldSeed = cbLabyrinthRnd.Checked ? (uint?) Environment.TickCount : _labOldSeed;

            ModuleBase moduleBase = new Fractal(type, basis, interp, octaves, frequency, _labOldSeed,
                angle, lacunarity);

            if (chkLabyrinthAC.Checked)
            {
                double acLow = double.Parse(tbLabyrinthACLow.Text);
                double acHigh = double.Parse(tbLabyrinthACHigh.Text);
                CombinerTypes combType = (CombinerTypes) cbLabyrinthACCombine.SelectedItem;
                AutoCorrect correct = new AutoCorrect(moduleBase, acLow, acHigh);
                moduleBase = new Combiner(combType, correct, moduleBase);
            }

//            Bias bias = new Bias(moduleBase, 0.01);
//            Gradient gradient = new Gradient(0, 0, 50, 100);
//            moduleBase = new TranslatedDomain(moduleBase, gradient, bias);

            if (chkLabyrinthSel.Checked)
            {
                double selLow = double.Parse(tbLabyrinthSelLow.Text);
                double selHigh = double.Parse(tbLabyrinthSelHigh.Text);
                double selThreshold = double.Parse(tbLabyrinthSelThreshold.Text);
                double? selFalloff = !string.IsNullOrEmpty(tbLabyrinthSelFalloff.Text)
                    ? double.Parse(tbLabyrinthSelFalloff.Text)
                    : (double?) null;
                moduleBase = new Select(moduleBase, selLow, selHigh, selThreshold, selFalloff);
            }

            if (pbLabyrinth.Image != null)
            {
                pbLabyrinth.Image.Dispose();
            }

            ushort width = 4000, height = 4000;
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, bitmap.PixelFormat);

            byte* pRoughMap = (byte*)Memory.HeapAlloc(width * height);
            Parallel.For(0, height, y =>
            {
                int* row = (int*) data.Scan0 + (y*data.Stride)/4;
                Parallel.For(0, width, x =>
                {
                    double p = (double) x/width;
                    double q = (double) y/height;
                    double val = moduleBase.Get(p, q);
                    pRoughMap[y * width + x] = (byte)Math.Abs(val - 1);
                    Color color = Color.Black.Lerp(Color.White, val);
                    row[x] = color.ToArgb();
                });
            });

            using (ServerMap map = new ServerMap(width, height, 0, pRoughMap))
            {
                map.SaveToFile("RK.save");
            }
            Memory.HeapFree(pRoughMap);

            bitmap.UnlockBits(data);
            pbLabyrinth.Image = bitmap;

            Cursor = DefaultCursor;
        }

        private void PbLabyrinthClick(object sender, EventArgs e)
        {
            tcMain.Focus();
        }

        private void FrmMainFormClosing(object sender, FormClosingEventArgs e)
        {
            mapCtrl.Dispose();
            miniMapCtrl.Dispose();
            if (_host != null)
            {
                _host.Dispose();
            }
        }

        private void EventsProviderKeyDown(object sender, KeyEventArgs e)
        {
            Direction direction = Direction.None;

            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.S:
                case Keys.A:
                case Keys.D:
                    if (tcMain.SelectedTab == tpMap)
                        direction = GetPlayerMoveDirection();
                    break;
                case Keys.C:
                    if (tcMain.SelectedTab == tpMap)
                        mapCtrl.CenterPlayer();
                    break;
                case Keys.Space:
                    {
                        if (tcMain.SelectedTab == tpLabyrinthGenerator)
                        {
                            BtnGenerateLabyrinthClick(null, null);
                        }
                        if (tcMain.SelectedTab == tpMap)
                        {
                            BtnLoadLabyrinthClick(this, null);
                        }
                        break;
                    }
            }

            if (tcMain.SelectedTab == tpMap)
            {
                mapCtrl.PlayerMove(direction);
            }
        }

        private void EventsProviderKeyUp(object sender, KeyEventArgs e)
        {
            Direction direction = Direction.None;

            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.S:
                case Keys.A:
                case Keys.D:
                    if (tcMain.SelectedTab == tpMap)
                        direction = GetPlayerMoveDirection();
                    break;
            }

            if (tcMain.SelectedTab == tpMap)
            {
                mapCtrl.PlayerMove(direction);
            }
        }

        private void BtnLoadLabyrinthClick(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            mapCtrl.DisconnectFromHost();
            mapCtrl.ConnectToHost();
            Cursor = DefaultCursor;
        }
    }

    static class ExtensionMethods
    {
        public static double Lerp(this double start, double end, double amount)
        {
            double difference = end - start;
            double adjusted = difference * amount;
            return start + adjusted;
        }

        public static Color Lerp(this Color color, Color to, double amount)
        {
            double sr = color.R, sg = color.G, sb = color.B;
            double er = to.R, eg = to.G, eb = to.B;
            byte r = (byte)sr.Lerp(er, amount),
                 g = (byte)sg.Lerp(eg, amount),
                 b = (byte)sb.Lerp(eb, amount);
            return Color.FromArgb(r, g, b);
        }
    }
}
