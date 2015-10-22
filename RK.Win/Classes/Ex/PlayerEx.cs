using System;
using System.Drawing;
using RK.Common.Classes.Units;

namespace RK.Win.Classes.Ex
{
    public sealed class PlayerEx
    {
        public Point MovingStartedPoint;
        public DateTime MovingStartTime;
        public float MovingDistanceRest;

        public bool NeedUpdatePosition;

        public PlayerEx(Player player)
        {
            
        }
    }
}
