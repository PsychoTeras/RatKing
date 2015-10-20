using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RK.Common.Common;
using RK.Common.Win32;

namespace RK.Common.Classes.Map
{
    public unsafe sealed class GameMap : DbObject, IDisposable
    {

#region Constants

        private const short MAGICNUM = 0x4D47; //GM
        private const float VERSION = 0.1f;

#endregion

#region Private types

        enum SectionType : byte
        {
            Header,
            Common,
            Tiles
        }

#endregion

#region Private fields

        private ushort _width;
        private ushort _height;
        private short _level;

        private Tile* _tiles;
        private MapAreas _wallAreas;
        private MapAreas _spaceAreas;

        private Dictionary<SectionType, SectionBase> _sections;

        private float _version = VERSION;

#endregion

#region Properties

        public ushort Width
        {
            get { return _width; }
        }

        public ushort Height
        {
            get { return _height; }
        }

        public Tile* this[ushort x, ushort y]
        {
            get { return &_tiles[y * _width + x]; }
        }

        public MapAreas WallAreas
        {
            get { return _wallAreas; }
        }

        public MapAreas SpaceAreas
        {
            get { return _spaceAreas; }
        }

#endregion

#region Ctor

        private GameMap()
        {
            InitializeGeneric();
            InitializeSections();
        }

        public GameMap(ushort width, ushort height, short level)
            : this(width, height, level, null) { }

        public GameMap(ushort width, ushort height, short level, byte* pRoughMap) 
            : this()
        {
            SetNewId();

            _level = level;

            ReinitializeTilesArray(width, height);

            if (pRoughMap != null)
            {
                Parallel.For(0, height, y => Parallel.For(0, width, x =>
                {
                    int i = y*_width + x;
                    _tiles[i].Type = (TileType) (pRoughMap[i] + 1);
                    _tiles[i].TypeIndex = 0;
                }));

                DetectAreas();
            }
        }

#endregion

#region Class methods

        private void InitializeGeneric()
        {
            _wallAreas = new MapAreas(this, TileType.Wall);
            _spaceAreas = new MapAreas(this, TileType.Nothing);
        }

        private void ReinitializeTilesArray(ushort width, ushort height)
        {
            if (_tiles != null)
            {
                Memory.HeapFree(_tiles);
            }
            _tiles = (Tile*)Memory.HeapAlloc((_width = width) * (_height = height) * Tile.SizeOf);
        }

        private void DetectAreas()
        {
            _wallAreas.Detect();
            TileBorders.ScanAndSetBorders(_wallAreas, TileType.Wall, this);

            _spaceAreas.Detect();
        }

#endregion

#region Save/load

        #region Section classes

        abstract class SectionBase
        {
            protected GameMap Map;

            protected SectionBase(GameMap map)
            {
                Map = map;
            }

            public abstract bool WriteSection(BinaryWriter bw);

            public abstract bool ReadSection(BinaryReader br, int sectionSize);
        }

        class HeaderSection : SectionBase
        {
            public HeaderSection(GameMap map) : base(map) { }

            public override bool WriteSection(BinaryWriter bw)
            {
                bw.Write(Map._version);
                return true;
            }

            public override bool ReadSection(BinaryReader br, int sectionSize)
            {
                Map._version = br.ReadSingle();
                return true;
            }
        }

        class MapSection : SectionBase
        {
            public MapSection(GameMap map) : base(map) { }

            public override bool WriteSection(BinaryWriter bw)
            {
                bw.Write(Map._width);
                bw.Write(Map._height);
                bw.Write(Map._level);
                return true;
            }

            public override bool ReadSection(BinaryReader br, int sectionSize)
            {
                ushort width = br.ReadUInt16();
                ushort height = br.ReadUInt16();
                Map.ReinitializeTilesArray(width, height);

                Map._level = br.ReadInt16();
                return true;
            }
        }

        class TilesSection : SectionBase
        {
            public TilesSection(GameMap map) : base(map) { }

            public override bool WriteSection(BinaryWriter bw)
            {
                int tilesCnt = Map._width * Map._height;
                for (int i = 0; i < tilesCnt; i++)
                {
                    //Read tile
                    Tile tile = Map._tiles[i];

                    //Find all similar tiles in a row
                    ushort similarTilesCnt = 1;
                    while (i < tilesCnt && similarTilesCnt < short.MaxValue - 1 &&
                           Map._tiles[i + 1] == tile)
                    {
                        similarTilesCnt++;
                        i++;
                    }

                    //Write RLE storage type (1 high bit) and similar tiles count (max 127-32,767)
                    //Convert to big-endian format for future simple reading
                    if (similarTilesCnt <= byte.MaxValue / 2)
                    {
                        bw.Write((byte)similarTilesCnt);
                    }
                    else
                    {
                        ushort hi = (ushort)(similarTilesCnt & 0x00FF);
                        byte lo = (byte)(similarTilesCnt >> 8 | 0x80);
                        similarTilesCnt = (ushort)(hi << 8 | lo);
                        bw.Write(similarTilesCnt);
                    }

                    //Store tile data
                    bw.Write((byte)tile.Type);
                    bw.Write(tile.TypeIndex);
                    bw.Write(tile.Flags);
                }
                return true;
            }

            public override bool ReadSection(BinaryReader br, int sectionSize)
            {
                int curTilePos = 0;
                long startPos = br.BaseStream.Position;
                while (br.BaseStream.Position - startPos < sectionSize)
                {
                    ushort tilesCnt;
                    byte rleMark = br.ReadByte();
                    if ((rleMark | 0x80) == rleMark)
                    {
                        byte rleMark2 = br.ReadByte();
                        tilesCnt = (ushort)((rleMark & ~0x80) << 8 | rleMark2);
                    }
                    else
                    {
                        tilesCnt = rleMark;
                    }

                    Tile tile = Tile.Empty;
                    tile.Type = (TileType)br.ReadByte();
                    tile.TypeIndex = br.ReadByte();
                    tile.Flags = br.ReadInt32();

                    for (int i = 0; i < tilesCnt; i++)
                    {
                        Memory.HeapCopy(&tile, &Map._tiles[curTilePos++], Tile.SizeOf);
                    }
                }
                return true;
            }
        }

        #endregion

        private void InitializeSections()
        {
            _sections = new Dictionary<SectionType, SectionBase>
            {
                {SectionType.Header, new HeaderSection(this)},
                {SectionType.Common, new MapSection(this)},
                {SectionType.Tiles, new TilesSection(this)},
            };
        }

        private bool WriteSection(SectionType type, SectionBase section, BinaryWriter bw)
        {
            //Write section type
            bw.Write((byte)type);

            //Reserve space for section length
            long sectLenPos = bw.BaseStream.Position;
            bw.Write(int.MinValue);

            //Write the section
            bool opCompleted = section.WriteSection(bw);

            //Check if something was wrong
            int sectionSize = (int) (bw.BaseStream.Position - sectLenPos - sizeof (int));
            if (!opCompleted || sectionSize == 0)
            {
                return false;
            }

            //Update section size
            bw.BaseStream.Seek(sectLenPos, SeekOrigin.Begin);
            bw.Write(sectionSize);
            bw.BaseStream.Seek(0, SeekOrigin.End);

            return true;
        }

        public byte[] SaveToMemory()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(MAGICNUM);
                    foreach (SectionType type in _sections.Keys)
                    {
                        if (!WriteSection(type, _sections[type], bw))
                        {
                            return null;
                        }
                    }
                }
                return ms.ToArray();
            }
        }

        public bool SaveToFile(string fileName)
        {
            byte[] data = SaveToMemory();
            if (data != null)
            {
                File.WriteAllBytes(fileName, data);
                return true;
            }
            return false;
        }

        private static bool ReadSection(SectionBase sectionBase, int sectionSize, BinaryReader br)
        {
            return sectionBase.ReadSection(br, sectionSize);
        }

        public static GameMap LoadFromMemory(byte[] data)
        {
            GameMap map = new GameMap();
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    short magicNum = br.ReadInt16();
                    if (magicNum != MAGICNUM)
                    {
                        return null;
                    }

                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        SectionType type = (SectionType) br.ReadByte();
                        int sectionSize = br.ReadInt32();
                        if (map._sections.ContainsKey(type))
                        {
                            if (!ReadSection(map._sections[type], sectionSize, br))
                            {
                                return null;
                            }
                        }
                        else
                        {
                            br.BaseStream.Seek(sectionSize, SeekOrigin.Current);
                        }
                    }
                }
            }

            map.DetectAreas();

            return map;
        }

        public static GameMap LoadFromFile(string fileName)
        {
            byte[] data = File.ReadAllBytes(fileName);
            return LoadFromMemory(data);
        }

#endregion

#region IDisposable

        ~GameMap()
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
