using System.Drawing;
using RK.Common.Classes.Common;
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
                Serializer.StringLength(Name) + //Name 

                sizeof (TinySize) + //Size
                sizeof (ushort) + //Health
                sizeof (float) + //Speed

                sizeof (float) + //Angle
                sizeof (Point) + //Position
                sizeof (Direction); //Direction
        }

        // ReSharper disable once RedundantAssignment
        public int Serialize(byte* bData, int pos)
        {
            int newPos = pos;

            int sLen = Serializer.WriteString(bData, Name, newPos);

            (*(TinySize*)&bData[newPos += sLen]) = Size;
            (*(ushort*)&bData[newPos += sizeof(TinySize)]) = Health;
            (*(float*)&bData[newPos += sizeof(ushort)]) = Speed;

            (*(float*)&bData[newPos += sizeof(float)]) = Angle;
            (*(Point*)&bData[newPos += sizeof(float)]) = Position;
            (*(Direction*)&bData[newPos += sizeof(Point)]) = Direction;

            newPos += sizeof(Direction);

            return newPos - pos;
        }

        public int Deserialize(byte* bData, int pos)
        {
            int newPos = pos;

            int sLen = Serializer.ReadString(bData, out Name, newPos);

            Size = (*(TinySize*) &bData[newPos += sLen]);
            Health = (*(ushort*)&bData[newPos += sizeof(TinySize)]);
            Speed = (*(float*)&bData[newPos += sizeof(ushort)]);

            Angle = (*(float*)&bData[newPos += sizeof(float)]);
            Position = (*(Point*)&bData[newPos += sizeof(float)]);
            Direction = (*(Direction*)&bData[newPos += sizeof(Point)]);

            newPos += sizeof(Direction);

            return newPos - pos;
        }

#endregion

    }
}