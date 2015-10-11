﻿using System;
using System.Collections.Generic;
using RK.Common.Classes.Common;

namespace RK.Common.Classes.Map
{
    public class MapArea : List<ShortPoint>
    {

#region Public fields

        public int CellsCount;

#endregion

#region Ctor

        internal MapArea(List<TraceCell> closedTrace, TileType areaType, GameMap map)
            : base(closedTrace.Count)
        {
            TraceCell prevTraceCell = null;
            HashSet<int> unique = new HashSet<int>();
            foreach (TraceCell traceCell in closedTrace)
            {
                ParseArea(traceCell, prevTraceCell, areaType, map, unique);
                prevTraceCell = traceCell;
            }
            CalculateCellsCount();
        }

#endregion
        
#region Area parsing

        private void ParseArea(TraceCell traceCell, TraceCell prevTraceCell, TileType areaType, 
            GameMap map, HashSet<int> unique)
        {
            bool invalid;
            ShortPoint borderPoint;
            if (prevTraceCell != null && prevTraceCell.Direction == Direction.S && traceCell.Direction == Direction.W)
            {
                borderPoint = GetBorders(traceCell.X, traceCell.Y, Direction.S, areaType, map, out invalid);
                AcceptBorders(borderPoint, invalid, map, unique);
            }
            borderPoint = GetBorders(traceCell.X, traceCell.Y, traceCell.Direction, areaType, map, out invalid);
            AcceptBorders(borderPoint, invalid, map, unique);
        }

        private void AcceptBorders(ShortPoint borderPoint, bool invalid, GameMap map, HashSet<int> unique)
        {
            if (!invalid && !unique.Contains(borderPoint.Mark))
            {
                Add(borderPoint);
                unique.Add(borderPoint.Mark);
            }
        }

        private ShortPoint GetBorders(int x, int y, Direction direction, TileType areaType, 
            GameMap map, out bool invalid)
        {
            invalid = true;

            ushort w = map.Width, h = map.Height;

            if (TileBorders.IsNotValidTile(x, y, w, h, areaType, map))
            {
                switch (direction)
                {
                    case Direction.W:
                        y += 1;
                        break;
                    case Direction.E:
                    case Direction.S:
                        x += 1;
                        break;
                }
            }

            if (x < 0 || x >= w || y < 0 || y >= h)
            {
                return ShortPoint.Empty;
            }

            invalid = TileBorders.GetBorders(x, y, w, h, areaType, map) == 0;
            return new ShortPoint(x, y);
        }

#endregion

#region Cells counting

        private void AcceptCornerValue(Dictionary<ushort, List<ushort>> dict,
            ushort coord, ushort value)
        {
            List<ushort> list;
            if (!dict.TryGetValue(coord, out list))
            {
                list = new List<ushort>();
                dict.Add(coord, list);
            }
            if (!list.Contains(value))
            {
                list.Add(value);
            }
        }

        private int CalculateCornerValues(Dictionary<ushort, List<ushort>> scrDict,
            Dictionary<ushort, List<ushort>> pairDict)
        {
            int totalCount = 0;
            foreach (ushort coord in scrDict.Keys)
            {
                List<ushort> list = scrDict[coord];
                list.Sort();
                int crnCount = 0, max = list.Count - 1;
                for (int i = max == 0 ? 0 : 1; i <= max; i++)
                {
                    ushort val = list[i];
                    if (i == max || val - list[i - 1] > 1)
                    {
                        List<ushort> pairList;
                        if (pairDict != null && pairDict.TryGetValue(val, out pairList) &&
                            pairList.Contains(coord))
                        {
                            continue;
                        }
                        crnCount++;
                        if (max > 0)
                        {
                            list[crnCount] = val;
                        }
                    }
                }
                if (pairDict == null && max > 0)
                {
                    list.RemoveRange(crnCount + 1, list.Count - crnCount - 1);
                }
                totalCount += crnCount;
            }
            return totalCount;
        }

        private void CalculateCellsCount()
        {
            var cvDict = new Dictionary<ushort, List<ushort>>[2];
            cvDict[0] = new Dictionary<ushort, List<ushort>>();
            cvDict[1] = new Dictionary<ushort, List<ushort>>();

            int max = Count - 1, sum = 0;
            for (int i = 0; i < max; i++)
            {
                sum += (this[i].X + this[i + 1].X)*(this[i + 1].Y - this[i].Y);
                AcceptCornerValue(cvDict[0], this[i].Y, this[i].X);
                AcceptCornerValue(cvDict[1], this[i].X, this[i].Y);
            }
            sum += (this[max].X + this[0].X)*(this[0].Y - this[max].Y);
            AcceptCornerValue(cvDict[0], this[max].Y, this[max].X);
            AcceptCornerValue(cvDict[1], this[max].X, this[max].Y);

            int cornerValues = CalculateCornerValues(cvDict[0], null) +
                               CalculateCornerValues(cvDict[1], cvDict[0]);

            CellsCount = Math.Max(Math.Abs(sum)/2 + cornerValues, Count);
        }

#endregion

    }

    public unsafe class MapAreas : List<MapArea>
    {

#region Private fields

        private GameMap _map;
        private TileType _areaType;

#endregion

#region Ctor

        public MapAreas(GameMap map, TileType areaType)
        {
            _map = map;
            _areaType = areaType;
        }

#endregion

#region Class methods

        public void Detect()
        {
            Clear();

            HashSet<int> visitedTiles = new HashSet<int>();
            
            ushort w = _map.Width, h = _map.Height;
            for (ushort y = 0; y < h; y++)
            {
                for (ushort x = 0; x < w; x++)
                {
                    if (visitedTiles.Contains(y << 16 | x))
                    {
                        continue;
                    }

                    if ((*_map[x, y]).Type == _areaType)
                    {
                        //Optimization to protect from extra work and unify identification results
                        while (x + 1 < w && (*_map[(ushort)(x + 1), y]).Type == _areaType)
                        {
                            x++;
                        }
                        if (visitedTiles.Contains(y << 16 | x))
                        {
                            continue;
                        }

                        Detect(x, y, visitedTiles);
                    }
                }
            }
        }

        private void Detect(ushort startX, ushort startY, HashSet<int> visitedTiles)
        {
            int initialValue = TraceCell.DirectionValue(startX, startY, _areaType, _map);
            if (initialValue == 0 || initialValue == 15)
            {
                return;
            }

            List<TraceCell> closedTrace = new List<TraceCell>();

            TraceCell prevCell = null;
            int x = startX, y = startY;
            do
            {
                TraceCell cell = null;
                int directionValue = TraceCell.DirectionValue(x, y, _areaType, _map);
                switch (directionValue)
                {
                    case 1:
                        cell = new TraceCell(x, y, Direction.N);
                        break;
                    case 2:
                        cell = new TraceCell(x, y, Direction.E);
                        break;
                    case 3:
                        cell = new TraceCell(x, y, Direction.E);
                        break;
                    case 4:
                        cell = new TraceCell(x, y, Direction.W);
                        break;
                    case 5:
                        cell = new TraceCell(x, y, Direction.N);
                        break;
                    case 6:
                        cell = prevCell != null && prevCell.Direction == Direction.N
                            ? new TraceCell(x, y, Direction.W)
                            : new TraceCell(x, y, Direction.E);
                        break;
                    case 7:
                        cell = new TraceCell(x, y, Direction.E);
                        break;
                    case 8:
                        cell = new TraceCell(x, y, Direction.S);
                        break;
                    case 9:
                        cell = prevCell != null && prevCell.Direction == Direction.E
                            ? new TraceCell(x, y, Direction.N)
                            : new TraceCell(x, y, Direction.S);
                        break;
                    case 10:
                        cell = new TraceCell(x, y, Direction.S);
                        break;
                    case 11:
                        cell = new TraceCell(x, y, Direction.S);
                        break;
                    case 12:
                        cell = new TraceCell(x, y, Direction.W);
                        break;
                    case 13:
                        cell = new TraceCell(x, y, Direction.N);
                        break;
                    case 14:
                        cell = new TraceCell(x, y, Direction.W);
                        break;
                }

                if (cell != null)
                {
                    closedTrace.Add(cell);
                    visitedTiles.Add(y << 16 | x);

                    x += cell.ScreenX;
                    y += cell.ScreenY;

                    prevCell = cell;
                }

            } while (x != startX || y != startY);

            Add(new MapArea(closedTrace, _areaType, _map));
        }

        //private BorderCell AddNewBorderCell(ushort x, ushort y)
        //{
        //    Dictionary<int, BorderCell> dy;
        //    if (!_borderCells.TryGetValue(x, out dy))
        //    {
        //        dy = new Dictionary<int, BorderCell>();
        //        _borderCells.Add(x, dy);
        //    }
        //    if (!dy.ContainsKey(y))
        //    {
        //        BorderCell borderCell = new BorderCell(x, y);
        //        dy.Add(y, borderCell);
        //        return borderCell;
        //    }
        //    return null;
        //}

        //public void AddCell(ushort x, ushort y)
        //{
        //    AddNewBorderCell(x, y);
        //    RecreateBorders(x, y);
        //}

        //public void RemoveCell(ushort x, ushort y)
        //{
        //    Dictionary<int, BorderCell> dy;
        //    if (_borderCells.TryGetValue(x, out dy) && dy.ContainsKey(y))
        //    {
        //        dy.Remove(y);
        //    }
        //    RecreateBorders(x, y);
        //}

        //private void RemoveCell(BorderCell borderCell)
        //{
        //    Dictionary<int, BorderCell> dy;
        //    if (_borderCells.TryGetValue(borderCell.X, out dy) &&
        //        dy.ContainsKey(borderCell.Y))
        //    {
        //        dy.Remove(borderCell.Y);
        //    }
        //}

        //private void RecreateBorder(int x, int y, BorderCell borderCell)
        //{
        //    if (x >= 0 && x < _map.Width && y >= 0 && y < _map.Width)
        //    {
        //        if (borderCell == null)
        //        {
        //            Tile tile = *_map[(ushort)x, (ushort)y];
        //            if (tile.Type == TileType.Wall)
        //            {
        //                borderCell = AddNewBorderCell((ushort)x, (ushort)y);
        //            }
        //        }
        //        if (borderCell != null)
        //        {
        //            borderCell.UpdateBorders(_map);
        //            if (borderCell.Borders == 0)
        //            {
        //                RemoveCell(borderCell);
        //            }
        //        }
        //    }
        //}

        //private void RecreateBorders(ushort x, ushort y)
        //{
        //    RecreateBorder(x, y, this[x, y]);
        //    for (int i = -1; i <= 1; i += 2)
        //    {
        //        RecreateBorder(x + i, y, this[x + i, y]);
        //        RecreateBorder(x, y + i, this[x, y + i]);
        //    }
        //}

#endregion

    }

    unsafe class TraceCell
    {
        public int X;
        public int Y;
        public Direction Direction;

        public int ScreenX
        {
            get
            {
                switch (Direction)
                {
                    case Direction.E:
                        return 1;
                    case Direction.W:
                        return -1;
                    default:
                        return 0;
                }
            }
        }

        public int ScreenY
        {
            get
            {
                switch (Direction)
                {
                    case Direction.S:
                        return 1;
                    case Direction.N:
                        return -1;
                    default:
                        return 0;
                }
            }
        }

        public static int DirectionValue(int x, int y, TileType areaType, GameMap map)
        {
            int flag = 0;
            ushort w = map.Width, h = map.Height;
            if (IsValidTile(x, y, w, h, areaType, map)) flag |= 1;
            if (IsValidTile(x + 1, y, w, h, areaType, map)) flag |= 2;
            if (IsValidTile(x, y + 1, w, h, areaType, map)) flag |= 4;
            if (IsValidTile(x + 1, y + 1, w, h, areaType, map)) flag |= 8;
            return flag;
        }

        private static bool IsValidTile(int x, int y, ushort w, ushort h, TileType areaType, GameMap map)
        {
            return x >= 0 && x < w && y >= 0 && y < h && (*map[(ushort)x, (ushort)y]).Type == areaType;
        }

        public TraceCell(int x, int y, Direction direction)
        {
            X = x;
            Y = y;
            Direction = direction;
        }

        public override string ToString()
        {
            return string.Format("{0} x {1} -> {2}", X, Y, Direction);
        }
    }

    enum Direction : byte
    {
        S, N, W, E
    }
}