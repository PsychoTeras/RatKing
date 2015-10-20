using System.Drawing;
using RK.Common.Common;
using RK.Common.Win32;

namespace RK.Common.Classes.Units
{
    public unsafe class Player : DbObject, ISerializable
    {

#region Public fields

        //Private
        public string Name;

        //Attributes
        public TinySize Size;
        public ushort Health;
        public float Speed;
        
        //Location
        public float Angle;
        public Point Position;
        public Direction Direction;

        //System
        public int MapId;

#endregion

#region Ctor

        public Player() { }

        public static Player Create(string name)
        {
            Player player = new Player
            {
                Name = name,
                Size = new TinySize(36, 36),
                Health = 100,
                Speed = 10f
            };
            player.SetNewId();
            return player;
        }

#endregion

#region Serializable

        public int SizeOf()
        {
            return
                Serializer.Length(Name) + //Name 

                sizeof (TinySize) + //Size
                sizeof (ushort) + //Health
                sizeof (float) + //Speed

                sizeof (float) + //Angle
                sizeof (Point) + //Position
                sizeof (Direction); //Direction
        }

        // ReSharper disable once RedundantAssignment
        public void Serialize(byte* bData, ref int pos)
        {
            Serializer.Write(bData, Name, ref pos);

            Serializer.Write(bData, Size, ref pos);
            Serializer.Write(bData, Health, ref pos);
            Serializer.Write(bData, Speed, ref pos);

            Serializer.Write(bData, Angle, ref pos);
            Serializer.Write(bData, Position, ref pos);
            Serializer.Write(bData, Direction, ref pos);
        }

        public void Deserialize(byte* bData, ref int pos)
        {
            Serializer.Read(bData, out Name, ref pos);

            Serializer.Read(bData, out Size, ref pos);
            Serializer.Read(bData, out Health, ref pos);
            Serializer.Read(bData, out Speed, ref pos);

            Serializer.Read(bData, out Angle, ref pos);
            Serializer.Read(bData, out Position, ref pos);
            Serializer.Read(bData, out Direction, ref pos);
        }

#endregion

    }
}