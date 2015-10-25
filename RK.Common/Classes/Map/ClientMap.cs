using System;
using System.Collections.Generic;
using RK.Common.Common;
using RK.Common.Win32;

namespace RK.Common.Classes.Map
{
    public unsafe sealed class ClientMap : IDisposable
    {

#region Constants

        private const int TILES_LIST_DEF_CAPACITY = 20;
        private const float TILES_LIST_CAPACITY_INC = 1.5f;

#endregion

#region Private fields

        private ushort _top;
        private ushort _left;
        private ushort _height;
        private ushort _width;

        private Tile* _tiles;
        private int _tilesListCount;
        private int _tilesListCapacity;

        private Dictionary<int, int> _tilesSet;
        private Dictionary<int, int> _map;
        private Dictionary<int, int> _rtFlags;

        private bool _onceLoaded;

#endregion

#region Properties

        public ushort Top
        {
            get { return _top; }
        }

        public ushort Left
        {
            get { return _left; }
        }

        public ushort Height
        {
            get { return _height; }
        }

        public ushort Width
        {
            get { return _width; }
        }

        public Tile* this[ushort x, ushort y]
        {
            get
            {
                int tileIdx;
                int tileCoordMark = y << 16 | x;
                return _map.TryGetValue(tileCoordMark, out tileIdx)
                    ? &_tiles[tileIdx]
                    : null;
            }
        }

#endregion

#region Ctor

        public ClientMap()
        {
            _tilesSet = new Dictionary<int, int>();
            _map = new Dictionary<int, int>();
            _rtFlags = new Dictionary<int, int>();
        }

#endregion

#region RTFlags operations

        //6 bits for wall borders. 0 - undefined. 1 - clear. 2 - left. 3 - right. 4. top. 5. bottom.
        public int Borders(ushort x, ushort y)
        {
            int rtFlags;
            int tileCoordMark = y << 16 | x;
            if (_rtFlags.TryGetValue(tileCoordMark, out rtFlags))
            {
                return rtFlags & 0x000000FF;
            }
            return -1;
        }

        public void FlagClearBorders(ushort x, ushort y)
        {
            int rtFlags;
            int tileCoordMark = y << 16 | x;
            if (_rtFlags.TryGetValue(tileCoordMark, out rtFlags))
            {
                _rtFlags[tileCoordMark] = (int)(rtFlags & 0xFFFFFF00);
            }
        }

        public void FlagSetBorders(ushort x, ushort y, byte borders)
        {
            int rtFlags;
            int tileCoordMark = y << 16 | x;
            if (_rtFlags.TryGetValue(tileCoordMark, out rtFlags))
            {
                _rtFlags[tileCoordMark] = (int)(rtFlags & 0xFFFFFF00 | borders);
            }
        }

#endregion

#region Class methods

        private void CheckTilesListCapacity()
        {
            if (_tilesListCount == _tilesListCapacity)
            {
                if (_tiles == null)
                {
                    _tilesListCapacity = TILES_LIST_DEF_CAPACITY;
                    _tiles = (Tile*) Memory.HeapAlloc(_tilesListCapacity*sizeof (Tile), false);
                }
                else
                {
                    _tilesListCapacity = (int) (_tilesListCapacity*TILES_LIST_CAPACITY_INC);
                    _tiles = (Tile*) Memory.HeapReAlloc(_tiles, _tilesListCapacity*sizeof (Tile), false);
                }
            }
        }

        private int AppendTile(ref Tile tile)
        {
            int tileIdx;
            int tileMark = tile.GetHashCode();
            if (!_tilesSet.TryGetValue(tileMark, out tileIdx))
            {
                CheckTilesListCapacity();
                tileIdx = _tilesListCount++;
                (*&_tiles[tileIdx]) = tile;
                _tilesSet.Add(tileMark, tileIdx);
            }
            return tileIdx;
        }

        public void AppendMapData(byte[] data, ShortRect window)
        {
            if (data == null)
            {
                return;
            }

            int startX = window.X, startY = window.Y;

            fixed (byte* bData = data)
            {
                int pos = 0, tilesCnt, curIdx = 0;
                Serializer.Read(bData, out tilesCnt, ref pos);
                for (int i = 0; i < tilesCnt; i++)
                {
                    byte rleMark;
                    ushort seriesLength;
                    Serializer.Read(bData, out rleMark, ref pos);
                    if ((rleMark | 0x80) == rleMark)
                    {
                        byte rleMark2;
                        Serializer.Read(bData, out rleMark2, ref pos);
                        seriesLength = (ushort)((rleMark & ~0x80) << 8 | rleMark2);
                    }
                    else
                    {
                        seriesLength = rleMark;
                    }

                    Tile tile = new Tile();
                    tile.Deserialize(bData, ref pos);
                    int tileIdx = AppendTile(ref tile);
                    for (int j = 0; j < seriesLength; j++)
                    {
                        ushort x = (ushort)(startX + (curIdx % window.Width));
                        ushort y = (ushort)(startY + (curIdx / window.Width));
                        int tileCoordMark = y << 16 | x;
                        if (_map.ContainsKey(tileCoordMark))
                        {
                            _map[tileCoordMark] = tileIdx;
                            FlagClearBorders(x, y);
                        }
                        else
                        {
                            _map.Add(tileCoordMark, tileIdx);
                            _rtFlags.Add(tileCoordMark, 0);
                        }
                        curIdx++;
                    }
                }
            }

            _left = _onceLoaded ? Math.Min(window.X, _left) : window.X;
            _top = _onceLoaded ? Math.Min(window.Y, _top) : window.Y;
            _height = (ushort) (_onceLoaded ? Math.Max(window.Height + window.Y, _height) : window.Height + window.Y);
            _width = (ushort) (_onceLoaded ? Math.Max(window.Width + window.X, _width) : window.Width + window.X);

            _onceLoaded = true;
        }

#endregion

#region IDisposable

        ~ClientMap()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                Memory.HeapFree(_tiles);
                _tiles = null;
            }
        }

#endregion

    }
}
