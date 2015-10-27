using System;
using System.Drawing;
using RK.Common.Classes.Common;
using RK.Common.Classes.Units;
using RK.Common.Const;
using RK.Common.Map;
using Direction = RK.Common.Classes.Common.Direction;

namespace RK.Common.Algo
{
    public unsafe static class Engine
    {
        public static bool ValidateNewPlayerPosition(PlayerData playerData, IBaseMap map)
        {
            if (playerData.IsMoving)
            {
                int traveled = CalculatePlayerTraveledDistance(playerData);
                Point newPos = CalculateNewPlayerPos(playerData.Player, traveled);
//                PlayerPosProcessCollision(playerData.Player, map, ref newPos);
                if (playerData.Player.Position != newPos)
                {
                    playerData.Player.Position = newPos;
                    return true;
                }
            }
            return false;
        }

        private static int CalculatePlayerTraveledDistance(PlayerData playerData)
        {
            double moveTimeElapsed = DateTime.Now.Subtract(playerData.MovingStartTime).
                TotalMilliseconds;
            float fDistance = ((float)moveTimeElapsed / playerData.Player.Speed) + 
                              playerData.MovingDistanceRest;
            int traveled = (int)Math.Floor(fDistance);
            if (traveled > 0)
            {
                playerData.MovingDistanceRest = fDistance - traveled;
                playerData.MovingStartTime = DateTime.Now;
            }
            return traveled;
        }

        private static Point CalculateNewPlayerPos(Player player, int traveled)
        {
            Point newPos;
            Point startPos = player.Position;
            switch (player.Direction)
            {
                case Direction.N:
                    newPos = new Point(startPos.X, startPos.Y - traveled);
                    break;
                case Direction.NW:
                    newPos = new Point(startPos.X - traveled, startPos.Y - traveled);
                    break;
                case Direction.NE:
                    newPos = new Point(startPos.X + traveled, startPos.Y - traveled);
                    break;
                case Direction.S:
                    newPos = new Point(startPos.X, startPos.Y + traveled);
                    break;
                case Direction.SW:
                    newPos = new Point(startPos.X - traveled, startPos.Y + traveled);
                    break;
                case Direction.SE:
                    newPos = new Point(startPos.X + traveled, startPos.Y + traveled);
                    break;
                case Direction.W:
                    newPos = new Point(startPos.X - traveled, startPos.Y);
                    break;
                case Direction.E:
                    newPos = new Point(startPos.X + traveled, startPos.Y);
                    break;
                default:
                    newPos = default(Point);
                    break;
            }
            return newPos;
        }

        private static void PlayerPosProcessCollision(Player player, IBaseMap map, ref Point newPos)
        {
            Point pos = player.Position;
            TinySize size = player.Size;
            bool stop = false, xStop = false, yStop = false;

            int mapWidth = map.Width, mapHeight = map.Height;

            int cellTs = (int)Math.Floor((float)pos.Y / ConstMap.PIXEL_SIZE);
            int cellTe = (int)Math.Floor((float)newPos.Y / ConstMap.PIXEL_SIZE);
            int cellLs = (int)Math.Floor((float)pos.X / ConstMap.PIXEL_SIZE);
            int cellLe = (int)Math.Floor((float)newPos.X / ConstMap.PIXEL_SIZE);

            int cellBs = (int)Math.Floor((float)(pos.Y + size.Height - 1) / ConstMap.PIXEL_SIZE);
            int cellBe = (int)Math.Floor((float)(newPos.Y + size.Height - 1) / ConstMap.PIXEL_SIZE);
            int cellRs = (int)Math.Floor((float)(pos.X + size.Width - 1) / ConstMap.PIXEL_SIZE);
            int cellRe = (int)Math.Floor((float)(newPos.X + size.Width - 1) / ConstMap.PIXEL_SIZE);

            int cellT = (int)Math.Floor((float)pos.Y / ConstMap.PIXEL_SIZE);
            int cellB = (int)Math.Floor((float)(pos.Y + size.Height - 1) / ConstMap.PIXEL_SIZE);

            int cellL = (int)Math.Floor((float)pos.X / ConstMap.PIXEL_SIZE);
            int cellR = (int)Math.Floor((float)(pos.X + size.Width - 1) / ConstMap.PIXEL_SIZE);

            switch (player.Direction)
            {
                case Direction.N:
                    {
                        for (int y = cellTs; y >= cellTe; y--)
                        {
                            if (y < 0 || y >= mapHeight || stop) break;
                            for (int x = cellL; x <= cellR; x++)
                            {
                                if (x < 0 || x >= mapWidth) continue;
                                if ((*map[(ushort)x, (ushort)y]).Type != TileType.Nothing)
                                {
                                    newPos.Y = y * ConstMap.PIXEL_SIZE + ConstMap.PIXEL_SIZE;
                                    stop = true;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                case Direction.NW:
                    {
                        int tTotalDist = cellTs - cellTe;
                        int lTotalDist = cellLs - cellLe;
                        int maxTotalDist = Math.Max(tTotalDist, lTotalDist);

                        for (int i = 1; i <= maxTotalDist; i++)
                        {
                            if (xStop && yStop) break;

                            cellT--; cellB--; cellL--; cellR--;

                            for (int y = cellT; y <= cellB; y++)
                            {
                                if (y < 0 || y >= mapHeight) continue;

                                //Y
                                if (y == cellT && i <= tTotalDist)
                                {
                                    for (int x = cellL; x <= cellR; x++)
                                    {
                                        if (x < 0 || x >= mapWidth) continue;
                                        if (!yStop && (*map[(ushort)(x + 1), (ushort)cellT]).Type != TileType.Nothing)
                                        {
                                            newPos.Y = cellT * ConstMap.PIXEL_SIZE + ConstMap.PIXEL_SIZE;
                                            yStop = true;
                                            break;
                                        }
                                    }
                                }

                                //X
                                if (!xStop && i <= lTotalDist &&
                                    (*map[(ushort)cellL, (ushort)(y + 1)]).Type != TileType.Nothing)
                                {
                                    newPos.X = cellL * ConstMap.PIXEL_SIZE + ConstMap.PIXEL_SIZE;
                                    xStop = true;
                                    break;
                                }

                                if (xStop && yStop) break;
                            }
                        }
                        break;
                    }
                case Direction.NE:
                    {
                        int tTotalDist = cellTs - cellTe;
                        int rTotalDist = cellRe - cellRs;
                        int maxTotalDist = Math.Max(tTotalDist, rTotalDist);

                        for (int i = 1; i <= maxTotalDist; i++)
                        {
                            if (xStop && yStop) break;

                            cellT--; cellB--; cellL++; cellR++;

                            for (int y = cellT; y <= cellB; y++)
                            {
                                if (y < 0 || y >= mapHeight) continue;

                                //Y
                                if (y == cellT && i <= tTotalDist)
                                {
                                    for (int x = cellL; x <= cellR; x++)
                                    {
                                        if (x < 0 || x >= mapWidth) continue;
                                        if (!yStop && (*map[(ushort)(x - 1), (ushort)cellT]).Type != TileType.Nothing)
                                        {
                                            newPos.Y = cellT * ConstMap.PIXEL_SIZE + ConstMap.PIXEL_SIZE;
                                            yStop = true;
                                            break;
                                        }
                                    }
                                }

                                //X
                                if (!xStop && i <= rTotalDist &&
                                    (*map[(ushort)cellR, (ushort)(y + 1)]).Type != TileType.Nothing)
                                {
                                    newPos.X = cellR * ConstMap.PIXEL_SIZE - size.Width;
                                    xStop = true;
                                    break;
                                }

                                if (xStop && yStop) break;
                            }
                        }
                        break;
                    }
                case Direction.S:
                    {
                        for (int y = cellBs; y <= cellBe; y++)
                        {
                            if (y < 0 || y >= mapHeight || stop) break;
                            for (int x = cellL; x <= cellR; x++)
                            {
                                if (x < 0 || x >= mapWidth) continue;
                                if ((*map[(ushort)x, (ushort)y]).Type != TileType.Nothing)
                                {
                                    newPos.Y = y * ConstMap.PIXEL_SIZE - size.Height;
                                    stop = true;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                case Direction.SW:
                    {
                        int bTotalDist = cellBe - cellBs;
                        int lTotalDist = cellLs - cellLe;
                        int maxTotalDist = Math.Max(bTotalDist, lTotalDist);

                        for (int i = 1; i <= maxTotalDist; i++)
                        {
                            if (xStop && yStop) break;

                            cellT++; cellB++; cellL--; cellR--;

                            for (int y = cellT; y <= cellB; y++)
                            {
                                if (y < 0 || y >= mapHeight) continue;

                                //Y
                                if (y == cellT && i <= bTotalDist)
                                {
                                    for (int x = cellL; x <= cellR; x++)
                                    {
                                        if (x < 0 || x >= mapWidth) continue;
                                        if (!yStop && (*map[(ushort)(x + 1), (ushort)cellB]).Type != TileType.Nothing)
                                        {
                                            newPos.Y = cellB * ConstMap.PIXEL_SIZE - size.Height;
                                            yStop = true;
                                            break;
                                        }
                                    }
                                }

                                //X
                                if (!xStop && i <= lTotalDist &&
                                    (*map[(ushort)cellL, (ushort)(y - 1)]).Type != TileType.Nothing)
                                {
                                    newPos.X = cellL * ConstMap.PIXEL_SIZE + ConstMap.PIXEL_SIZE;
                                    xStop = true;
                                    break;
                                }

                                if (xStop && yStop) break;
                            }
                        }
                        break;
                    }
                case Direction.SE:
                    {
                        int bTotalDist = cellBe - cellBs;
                        int rTotalDist = cellRe - cellRs;
                        int maxTotalDist = Math.Max(bTotalDist, rTotalDist);

                        for (int i = 1; i <= maxTotalDist; i++)
                        {
                            if (xStop && yStop) break;

                            cellT++; cellB++; cellL++; cellR++;

                            for (int y = cellT; y <= cellB; y++)
                            {
                                if (y < 0 || y >= mapHeight) continue;

                                //Y
                                if (y == cellT && i <= bTotalDist)
                                {
                                    for (int x = cellL; x <= cellR; x++)
                                    {
                                        if (x < 0 || x >= mapWidth) continue;
                                        if (!yStop && (*map[(ushort)(x - 1), (ushort)cellB]).Type != TileType.Nothing)
                                        {
                                            newPos.Y = cellB * ConstMap.PIXEL_SIZE - size.Height;
                                            yStop = true;
                                            break;
                                        }
                                    }
                                }

                                //X
                                if (!xStop && i <= rTotalDist &&
                                    (*map[(ushort)cellR, (ushort)(y - 1)]).Type != TileType.Nothing)
                                {
                                    newPos.X = cellR * ConstMap.PIXEL_SIZE - size.Width;
                                    xStop = true;
                                    break;
                                }

                                if (xStop && yStop) break;
                            }
                        }
                        break;
                    }
                case Direction.W:
                    {
                        for (int x = cellLs; x >= cellLe; x--)
                        {
                            if (x < 0 || x >= mapWidth || stop) break;
                            for (int y = cellT; y <= cellB; y++)
                            {
                                if (y < 0 || y >= mapHeight) continue;
                                if ((*map[(ushort)x, (ushort)y]).Type != TileType.Nothing)
                                {
                                    newPos.X = x * ConstMap.PIXEL_SIZE + ConstMap.PIXEL_SIZE;
                                    stop = true;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                case Direction.E:
                    {
                        for (int x = cellRs; x <= cellRe; x++)
                        {
                            if (x < 0 || x >= mapWidth || stop) break;
                            for (int y = cellT; y <= cellB; y++)
                            {
                                if (y < 0 || y >= mapHeight) continue;
                                if ((*map[(ushort)x, (ushort)y]).Type != TileType.Nothing)
                                {
                                    newPos.X = x * ConstMap.PIXEL_SIZE - size.Width;
                                    stop = true;
                                    break;
                                }
                            }
                        }
                        break;
                    }
            }
        }

    }
}
