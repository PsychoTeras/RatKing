using System.Drawing;
using RK.Common.Classes.Common;

namespace RK.Common.Classes.Units
{
    public class Player : DbObject
    {

#region Public fields

        //Private
        public string Name;

        //Attributes
        public TinySize Size;
        public ushort Health;
        public float Speed;
        
        //Map
        public int MapId;
        public float Angle;
        public Point Position;
        public Direction Direction;

#endregion

#region Ctor

        private Player() { }

        public static Player Create(string name)
        {
            Player player = new Player
            {
                Name = name,
                Size = new TinySize(48, 48),
                Health = 100,
                Speed = 10
            };
            player.SetNewId();
            return player;
        }

#endregion

    }
}