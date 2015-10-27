using System;
using System.Collections.Generic;
using RK.Common.Classes.Common;
using RK.Common.Win32;

namespace RK.Common.Map
{
    public unsafe sealed class ClientMap : IBaseMap, IDisposable
    {

#region Constants

        private const int TILES_LIST_DEF_CAPACITY = 20;
        private const float TILES_LIST_CAPACITY_INC = 1.5f;

#endregion

#region Private fields

        private ushort _height;
        private ushort _width;

        private Tile* _tiles;
        private int _tilesListCount;
        private int _tilesListCapacity;

        private byte* _miniMapData;
        private ShortSize _miniMapSize;

        private Dictionary<int, int> _tilesSet;
        private Dictionary<int, int> _map;
        private Dictionary<int, int> _rtFlags;

#endregion

#region Properties

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

        public ShortSize MiniMapSize
        {
            get { return _miniMapSize; }
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

#region Map

        public void Setup(ShortSize mapSize)
        {
            _width = mapSize.Width;
            _height = mapSize.Height;
        }

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
                _tiles[tileIdx] = tile;
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
                    int maxIdx = curIdx + seriesLength;
                    for (; curIdx < maxIdx; curIdx++)
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
                    }
                }
            }
        }

#endregion

#region Minimap

        public byte GetMiniMapPixel(ushort x, ushort y)
        {
            int idx = y*_miniMapSize.Width + x;
            byte pxl = _miniMapData[idx/2];
            return (byte) (idx%2 == 0 ? pxl >> 4 : pxl & 0x0F);
        }

        public void AppendMiniMapData(byte[] data, ShortSize size)
        {
            if (_miniMapData != null)
            {
                Memory.HeapFree(_miniMapData);
            }

            _miniMapSize = size;
            int miniMapArea = size.Width*size.Height;
            _miniMapData = (byte*) Memory.HeapAlloc(miniMapArea/2);

            fixed (byte* bData = data)
            {
                int pos = 0, pxlCnt, curIdx = 0;
                Serializer.Read(bData, out pxlCnt, ref pos);
                for (int i = 0; i < pxlCnt; i++)
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

                    byte pxl;
                    Serializer.Read(bData, out pxl, ref pos);
                    int maxIdx = curIdx + seriesLength;
                    for (; curIdx < maxIdx; curIdx++)
                    {
                        _miniMapData[curIdx] = pxl;
                    }
                }
            }
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
