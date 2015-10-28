using System;
using System.Drawing;
using RK.Common.Classes.Common;

namespace RK.Common.Classes.Units
{
    public class PlayerData
    {
        internal bool IsMoving;

        public Player Player;

        public Point MovingStartedPoint;
        public DateTime MovingStartTime;
        public float MovingDistanceRest;

        public PlayerData(Player player)
        {
            Player = player;
        }

        public void StartMoving(Point playerPos, Direction direction)
        {
            Player.Direction = direction;
            Player.Position = MovingStartedPoint = playerPos;
            MovingStartTime = DateTime.Now;
            IsMoving = true;
        }

        public void StopMoving(Point playerPos)
        {
            Player.Direction = Direction.None;
            Player.Position = playerPos;
            IsMoving = false;
        }

        public static implicit operator Player(PlayerData playerData)
        {
            return playerData == null ? null : playerData.Player;
        }
    }
}
