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
        public long MapId;
        public Point Position;
        public short Angle;

        //States
        public bool IsMoving;

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
                Speed = 1
            };
            player.SetNewId();
            return player;
        }

#endregion

    }
}