using System;
using System.Collections.Generic;
using RK.Common.Classes.Common;
using RK.Common.Classes.Units;
using RK.Common.Const;
using RK.Common.Win32;
using D = RK.Common.Classes.Common.Direction;

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
        private ushort _tilesListCount;
        private ushort _tilesListCapacity;
        private Dictionary<int, ushort> _tilesSet;

        private byte* _miniMapData;
        private ShortSize _miniMapSize;

        private Dictionary<int, int> _map;
        private Dictionary<int, int> _rtFlags;

        private List<ShortRect> _windows;

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
            _tilesSet = new Dictionary<int, ushort>();
            _map = new Dictionary<int, int>();
            _rtFlags = new Dictionary<int, int>();
            _windows = new List<ShortRect>();
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
                _rtFlags[tileCoordMark] = (int) (rtFlags & 0xFFFFFF00);
            }
        }

        public void FlagSetBorders(ushort x, ushort y, byte borders)
        {
            int rtFlags;
            int tileCoordMark = y << 16 | x;
            if (_rtFlags.TryGetValue(tileCoordMark, out rtFlags))
            {
                _rtFlags[tileCoordMark] = (int) (rtFlags & 0xFFFFFF00 | borders);
            }
        }

#endregion

#region Map

        private bool IsCloseToBorder(ShortRect window, ShortPoint playerPos, Player player,
            float borderAreaSpacePart)
        {
            int horzSpace = (int) (window.Width/borderAreaSpacePart);
            int vertSpace = (int) (window.Height/borderAreaSpacePart);
            return (playerPos.X <= window.X + horzSpace && playerPos.X - horzSpace >= 0 &&
                    (player.Direction == D.W || player.Direction == D.NW || player.Direction == D.SW)) ||
                   (playerPos.X >= window.X + window.Width - horzSpace && playerPos.X + horzSpace < _width &&
                    (player.Direction == D.E || player.Direction == D.NE || player.Direction == D.SE)) ||
                   (playerPos.Y <= window.Y + vertSpace && playerPos.Y - vertSpace >= 0 &&
                    (player.Direction == D.N || player.Direction == D.NE || player.Direction == D.NW)) ||
                   (playerPos.Y >= window.Y + window.Height - vertSpace && playerPos.Y + vertSpace < _height &&
                    (player.Direction == D.S || player.Direction == D.SE || player.Direction == D.SW));
        }

        public bool NeedsToLoadMapWindow(Player player, float borderAreaSpacePart)
        {
            ShortPoint playerPos = player.Position.ToShortPoint(ConstMap.PIXEL_SIZE);
            foreach (ShortRect window in _windows)
            {
                if (!window.Contains(playerPos)) continue;
                if (!IsCloseToBorder(window, playerPos, player, borderAreaSpacePart)) return false;
            }
            return true;
        }

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
                    _tilesListCapacity = (ushort) (_tilesListCapacity*TILES_LIST_CAPACITY_INC);
                    _tiles = (Tile*) Memory.HeapReAlloc(_tiles, _tilesListCapacity*sizeof (Tile), false);
                }
            }
        }

        private ushort AppendTile(ref Tile tile)
        {
            ushort tileIdx;
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
            Dictionary<ushort, Tile> tilesSet = new Dictionary<ushort, Tile>();

            fixed (byte* bData = data)
            {
                int pos = 0;

                //Read tiles set
                ushort tilesInSet;
                Serializer.Read(bData, out tilesInSet, ref pos);
                for (ushort i = 0; i < tilesInSet; i++)
                {
                    Tile tile = new Tile();
                    tile.Deserialize(bData, ref pos);
                    tilesSet.Add(i, tile);
                }

                //Read tiles list
                int tilesCnt, curIdx = 0;
                Serializer.Read(bData, out tilesCnt, ref pos);
                for (int i = 0; i < tilesCnt; i++)
                {
                    //Read tile series length
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

                    //Match tile
                    ushort tileIdx;
                    Serializer.Read(bData, out tileIdx, ref pos);
                    Tile tile = tilesSet[tileIdx];
                    tileIdx = AppendTile(ref tile);

                    //Fill tiles list
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

            _windows.Add(window);
        }

        public Tile* SetTileType(ushort x, ushort y, TileType type)
        {
            ushort tileIdx;
            Tile tile = *this[x, y];
            int tileMark = tile.GetHashCode();
            if (_tilesSet.TryGetValue(tileMark, out tileIdx))
            {
                tileMark = tile.GetHashCode(type);
                if (_tilesSet.TryGetValue(tileMark, out tileIdx))
                {
                    int tileCoordMark = y << 16 | x;
                    _map[tileCoordMark] = tileIdx;
                    return &_tiles[tileIdx];
                }
            }
            return null;
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
