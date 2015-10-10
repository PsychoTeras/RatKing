using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace RK.Win.Classes.Map
{
    public unsafe class MapBG : IDisposable
    {

#region Private fields

        private Bitmap[] _bgTiles;
        private Size _tilesCount;
        private bool _doNotSplitByTiles;

        private Dictionary<int, Dictionary<int, byte>> _map;

        private int _startTileX;
        private int _startTileY;
        private int _tilesCntX;
        private int _tilesCntY;

#endregion

#region Properties

        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        public int TilesTotal { get; private set; }

        public Bitmap this[int x, int y]
        {
            get { return _bgTiles[_map[x][y]]; }
        }

#endregion

#region Ctor

        public MapBG(float scaleFactor, bool doNotSplitByTiles, Size? tilesCount = null)
        {
            _doNotSplitByTiles = doNotSplitByTiles;
            Initialize(scaleFactor, false, tilesCount);
        } 

#endregion

#region Class methods

        public void Initialize(float scaleFactor, bool changeScaleFactor = false,
            Size? tilesCount = null)
        {
            if (!changeScaleFactor)
            {
                _map = new Dictionary<int, Dictionary<int, byte>>();
            }

            Dispose();

            string fileName = @"Resources\bg_tiles.png";
            using (Image image = Image.FromFile(fileName))
            {
                if (!changeScaleFactor)
                {
                    _tilesCount = _doNotSplitByTiles
                        ? new Size(1, 1)
                        : tilesCount ?? DetectTilesCount(image as Bitmap);
                    TilesTotal = _tilesCount.Width * _tilesCount.Height;
                }
                _bgTiles = new Bitmap[TilesTotal];

                float realTileWidth = (float)image.Width / _tilesCount.Width;
                float realTileHeight = (float)image.Height / _tilesCount.Height;
                TileWidth = (int) (realTileWidth*scaleFactor);
                TileHeight = (int) (realTileHeight*scaleFactor);
                for (int x = 0; x < _tilesCount.Width; x++)
                {
                    for (int y = 0; y < _tilesCount.Height; y++)
                    {
                        Bitmap tileBitmap = new Bitmap(TileWidth, TileHeight);
                        using (Graphics tileGraphics = Graphics.FromImage(tileBitmap))
                        {
                            RectangleF srcRect = new RectangleF(x*realTileWidth, y*realTileHeight,
                                realTileWidth, realTileHeight);
                            RectangleF destRect = new RectangleF(0, 0, TileWidth, TileHeight);
                            tileGraphics.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
                        }
                        _bgTiles[x * _tilesCount.Height + y] = tileBitmap;
                    }
                }
            }
        }

        public void InvalidateArea(Rectangle area, out int startTileX, out int startTileY,
                                   out int tilesCntX, out int tilesCntY)
        {
            Random rnd = new Random(Environment.TickCount);

            startTileX = (int) Math.Floor((double) area.X/TileWidth);
            startTileY = (int) Math.Floor((double) area.Y/TileHeight);
            tilesCntX = (int) Math.Ceiling((double) area.Width/TileWidth) + 1;
            tilesCntY = (int) Math.Ceiling((double) area.Height/TileHeight) + 1;

            if (_startTileX != startTileX || _startTileY != startTileY ||
                _tilesCntX != tilesCntX || _tilesCntY != tilesCntY)
            {
                for (int x = startTileX; x < startTileX + tilesCntX; x++)
                {
                    if (!_map.ContainsKey(x))
                    {
                        _map.Add(x, new Dictionary<int, byte>());
                    }
                    for (int y = startTileY; y < startTileY + tilesCntY; y++)
                    {
                        byte tileIndex = (byte) rnd.Next(TilesTotal);
                        if (!_map[x].ContainsKey(y))
                        {
                            _map[x].Add(y, tileIndex);
                        }
                    }
                }
                _startTileX = startTileX;
                _startTileY = startTileY;
                _tilesCntX = tilesCntX;
                _tilesCntY = tilesCntY;
            }
        }

        public void Dispose()
        {
            if (_bgTiles != null)
            {
                foreach (Bitmap bitmap in _bgTiles)
                {
                    bitmap.Dispose();
                }
            }
        }

#endregion

#region Helpers

        private byte GetBitmapComponentsCnt(Bitmap src)
        {
            return (byte) (Image.GetPixelFormatSize(src.PixelFormat) / 8);
        }

        private void GetGrayscaledImage(Bitmap src, byte* pgsData, int destSize)
        {
            byte componentsCnt = GetBitmapComponentsCnt(src);
            Rectangle srcRect = new Rectangle(0, 0, src.Width, src.Height);
            BitmapData srcData = src.LockBits(srcRect, ImageLockMode.ReadOnly, src.PixelFormat);
            Parallel.For(0, srcData.Height, h =>
            {
                byte* row = (byte*)srcData.Scan0 + (h * srcData.Stride);
                Parallel.For(0, srcData.Width, w =>
                {
                    int posSrc = w * componentsCnt;
                    int posDest = h * srcData.Width + w;
                    pgsData[posDest] = (byte)((row[posSrc] + row[posSrc + 1] + row[posSrc + 2]) / 3);
                });
            });
            src.UnlockBits(srcData);
        }

        private int GenerateGaussianKernel(int kernelSize, float sigma, byte* gaussianKernel)
        {
            float* kernel = stackalloc float[kernelSize * kernelSize];

            float d1 = 1/(2*(float) Math.PI*sigma*sigma);
            float d2 = 2*sigma*sigma;

            float min = float.MaxValue;
            int halfKernelSize = kernelSize / 2;
            Parallel.For(-halfKernelSize, halfKernelSize + 1, i => 
            Parallel.For(-halfKernelSize, halfKernelSize + 1, j =>
            {
                int pos = ((i + halfKernelSize)*kernelSize) + j + halfKernelSize;
                kernel[pos] = ((1/d1)*(float) Math.Exp(-(i*i + j*j)/d2));
                if (kernel[pos] < min)
                {
                    min = kernel[pos];
                }
            }));

            int sum = 0;
            int mult = (min > 0 && min < 1) ? (int)(1 / min) : 1;
            Parallel.For(-halfKernelSize, halfKernelSize + 1, i => 
            Parallel.For(-halfKernelSize, halfKernelSize + 1, j =>
            {
                int pos = ((i + halfKernelSize) * kernelSize) + j + halfKernelSize;
                kernel[pos] = (float)Math.Round(kernel[pos] * mult, 0);
                gaussianKernel[pos] = (byte)kernel[pos];
                sum += gaussianKernel[pos];
            }));

            return sum;
        }

        private void ApplyGaussianFilter(byte* data, int width, int height, int kernelSize, float sigma)
        {
            int halfKernelSize = kernelSize / 2;
            byte* kernel = stackalloc byte[kernelSize * kernelSize];
            int kernelWeight = GenerateGaussianKernel(kernelSize, sigma, kernel);

            for (int w = halfKernelSize; w < width - halfKernelSize; w++)
            {
                for (int h = halfKernelSize; h < height - halfKernelSize; h++)
                {
                    float sum = 0;
                    for (int k = -halfKernelSize; k <= halfKernelSize; k++)
                    {
                        for (int l = -halfKernelSize; l <= halfKernelSize; l++)
                        {
                            int srcPos = ((w + k)*width) + h + l;
                            int kPos = ((k + halfKernelSize)*(halfKernelSize*2 + 1)) + l + halfKernelSize;
                            sum += (float) data[srcPos]*kernel[kPos];
                        }
                    }
                    data[w*height + h] = (byte) Math.Round(sum/kernelWeight);
                }
            }
        }

        private void Differentiate(byte* data, int width, int height, int[,] filter, 
                                   short* derivative)
        {
            int fw = filter.GetLength(0), halfFw = fw/2;
            int fh = filter.GetLength(1), halfFh = fh/2;

            for (int w = halfFw; w < width - halfFw; w++)
            {
                for (int h = halfFh; h < height - halfFh; h++)
                {
                    short sum = 0;
                    for (int k = -halfFw; k <= halfFw; k++)
                    {
                        for (int l = -halfFh; l <= halfFh; l++)
                        {
                            int srcPos = ((w + k) * width) + h + l;
                            sum += (short)(data[srcPos] * filter[halfFw + k, halfFh + l]);
                        }
                    }
                    derivative[w*height + h] = sum;
                }
            }
        }

        private void SupressLocalNonMaximums(byte* data, short* derivativeX, short* derivativeY,
                                             int width, int height)
        {
            Parallel.For(1, width - 1, w =>
                Parallel.For(1, height - 1, h =>
                {
                    int pos = (w*(height - 2)) + h;
                    int posXb = (w*(height - 2)) + h - 1;
                    int posXa = (w*(height - 2)) + h + 1;
                    int posYb = ((w - 1)*(height - 2)) + h;
                    int posYa = ((w + 1)*(height - 2)) + h;

                    int posXbYb = ((w - 1)*(height - 2)) + h - 1;
                    int posXbYa = ((w + 1)*(height - 2)) + h - 1;
                    int posXaYb = ((w - 1)*(height - 2)) + h + 1;
                    int posXaYa = ((w + 1)*(height - 2)) + h + 1;

                    double tangent = derivativeX[pos] != 0
                        ? Math.Atan((float) derivativeY[pos]/derivativeX[pos])*180/Math.PI
                        : 90;

                    //Horizontal
                    if ((-22.5 < tangent && tangent <= 22.5) || (157.5 < tangent && tangent <= -157.5))
                    {
                        if (data[pos] < data[posYa] || data[pos] < data[posYb])
                            data[pos] = 0;
                    }

                    //Vertical
                    if ((-112.5 < tangent && tangent <= -67.5) || (67.5 < tangent && tangent <= 112.5))
                    {
                        if (data[pos] < data[posXa] || data[pos] < data[posXb])
                            data[pos] = 0;
                    }

                    //+45 Degree
                    if ((-67.5 < tangent && tangent <= -22.5) || (112.5 < tangent && tangent <= 157.5))
                    {
                        if (data[pos] < data[posXaYb] || data[pos] < data[posXbYa])
                            data[pos] = 0;
                    }

                    //-45 Degree
                    if ((-157.5 < tangent && tangent <= -112.5) || (67.5 < tangent && tangent <= 22.5))
                    {
                        if (data[pos] < data[posXaYa] || data[pos] < data[posXbYb])
                            data[pos] = 0;
                    }
                }));
        }

        private byte SelectStrongEdges(byte* data, short* derivativeX, short* derivativeY,
                                       int width, int height, int threshold)
        {
            int[] leadColors = {0, 0};
            Parallel.For(0, width, w => 
            Parallel.For(0, height, h =>
            {
                int pos = w*height + h;
                data[pos] = (byte) Math.Sqrt((derivativeX[pos]*derivativeX[pos]) +
                                             (derivativeY[pos]*derivativeY[pos]));
                data[pos] = (data[pos] >= threshold) ? byte.MaxValue : byte.MinValue;
                Interlocked.Increment(ref leadColors[data[pos]/byte.MaxValue]);
            }));
            return leadColors[0] < leadColors[1] ? byte.MinValue : byte.MaxValue;
        }

        private byte SelectEdges(byte* data, int width, int height, int threshold)
        {
            fixed (short* derivativeX = new short[width * height])
            {
                fixed (short* derivativeY = new short[width * height])
                {
                    int[,] dx =
                    {
                        {1, 0, -1},
                        {1, 0, -1},
                        {1, 0, -1}
                    };
                    Differentiate(data, width, height, dx, derivativeX);

                    int[,] dy =
                    {
                        {1, 1, 1},
                        {0, 0, 0},
                        {-1, -1, -1}
                    };
                    Differentiate(data, width, height, dy, derivativeY);

                    SupressLocalNonMaximums(data, derivativeX, derivativeY, width, height);

                    return SelectStrongEdges(data, derivativeX, derivativeY, width, height, threshold);
                }
            }
        }

        private byte* InterpolateEdges(byte* data, ref int width, ref int height, byte edgeColor)
        {
            int dataSize = (width / 3) * (height / 3);
            byte nonEdgeColor = (byte) Math.Abs(edgeColor - byte.MaxValue);
            fixed (byte* edgesMap = new byte[dataSize])
            {
                for (int h = 1; h < height - (height % 2); h += 3)
                {
                    for (int w = 1; w < width - (width % 2); w += 3)
                    {
                        int pos = (h*(width - 2)) + w;
                        int posXb = (h * (width - 2)) + w - 1;
                        int posXa = (h * (width - 2)) + w + 1;
                        int posYb = ((h - 1) * (width - 2)) + w;
                        int posYa = ((h + 1) * (width - 2)) + w;
                        int destPos = (((h - 1) / 3) * (width / 3 - 2)) + ((w - 1) / 3);
                        edgesMap[destPos] =
                            data[pos] == edgeColor ||
                            data[posXb] == edgeColor ||
                            data[posXa] == edgeColor ||
                            data[posYb] == edgeColor ||
                            data[posYa] == edgeColor
                                ? edgeColor
                                : nonEdgeColor;
                    }
                }
                width /= 3;
                height /= 3;
                return edgesMap;
            }
        }

        private void DetectEdge(byte* data, int i, int j, int side1, int side2, 
                                byte edgeColor, ref int edgeColors)
        {
            int pos = (i * (side2 - 2)) + j;
            int posB = ((j - 1) * (side1 - 2)) + i;
            int posA = ((j + 1) * (side1 - 2)) + i;
            if (i < side1 && j < side2 &&
                (data[pos] == edgeColor || data[posB] == edgeColor || data[posA] == edgeColor))
            {
                edgeColors++;
            }
        }

        private void StoreEdge(int edgeColors, int i, int side, float threshold, List<int> edges)
        {
            if ((float) edgeColors/side*100 >= threshold &&
                (edges.Count == 0 || edges.Last() < i - Edges.SPACE_BETWEEN_EDGES))
            {
                edges.Add(i);
            }
        }

        private Edges DetectEdges(byte* data, int width, int height, byte edgeColor, float threshold)
        {
            data = InterpolateEdges(data, ref width, ref height, edgeColor);

            //byte[] gsData = GrayscaledDataToArray(data, width*height);
            //File.WriteAllBytes("d:\\g.dat", gsData);

            Edges edges = new Edges(width, height);

            int maxSide = Math.Max(width, height) - 1;
            for (int i = 1; i < maxSide; i++)
            {
                int edgeColorsHorz = 0, edgeColorsVert = 0;
                for (int j = 1; j < maxSide; j++)
                {
                    DetectEdge(data, i, j, width, height, edgeColor, ref edgeColorsHorz);
                    DetectEdge(data, j, i, height, width, edgeColor, ref edgeColorsVert);
                }
                StoreEdge(edgeColorsHorz, i, width, threshold, edges.Horz);
                StoreEdge(edgeColorsVert, i, height, threshold, edges.Vert);
            }

            edges.CompleteBorderEdges();

            return edges;
        }

        protected byte[] GrayscaledDataToArray(byte* pgsData, int dataSize)
        {
            byte[] gsData = new byte[dataSize];
            Marshal.Copy(new IntPtr(pgsData), gsData, 0, dataSize);
            return gsData;
        }

        private Size DetectTilesCount(Bitmap src)
        {
            int destSize = src.Width * src.Height;
            byte* pgsData = stackalloc byte[destSize];
            GetGrayscaledImage(src, pgsData, destSize);
            ApplyGaussianFilter(pgsData, src.Width, src.Height, 7, 1.5f);
            byte edgeColor = SelectEdges(pgsData, src.Width, src.Height, 60);
            Edges edges = DetectEdges(pgsData, src.Width, src.Height, edgeColor, 70);
            return new Size(edges.TilesCols, edges.TilesRows);
        }

        class Edges
        {
            public const int SPACE_BETWEEN_EDGES = 20;

            private int _width;
            private int _height;

            public List<int> Horz { get; private set; }
            public List<int> Vert { get; private set; }

            public int TilesCols
            {
                get { return (Horz.Count - 1); }
            }

            public int TilesRows
            {
                get { return (Vert.Count - 1); }
            }

            public Edges(int width, int height)
            {
                _width = width;
                _height = height;
                Horz = new List<int>();
                Vert = new List<int>();
            }

            private void CompleteBorderEdges(List<int> edges, int maxSize)
            {
                if (edges.Count == 0 || edges.First() > SPACE_BETWEEN_EDGES)
                {
                    edges.Insert(0, 0);
                }
                if (edges.Count == 1 || maxSize - edges.Last() > SPACE_BETWEEN_EDGES)
                {
                    edges.Add(maxSize);
                }
            }

            public void CompleteBorderEdges()
            {
                CompleteBorderEdges(Horz, _width);
                CompleteBorderEdges(Vert, _height);
            }
        }

#endregion

    }
}
