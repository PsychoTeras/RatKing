using System;
using System.Text;
using RK.Common.Common;
using RK.Common.Proto.ErrorCodes;
using RK.Common.Proto.Responses;
using RK.Common.Win32;

namespace RK.Common.Proto
{
    public abstract unsafe class BaseResponse : ITransferable
    {

#region Constants

        private const int BASE_SIZE = 16;

#endregion

#region Public fields

        public long Id;
        public short ErrorCode;

#endregion

#region Properties

        public abstract PacketType Type { get; }

        protected virtual int SizeOf
        {
            get { return 0; }
        }

        protected virtual bool Compressed
        {
            get { return false; }
        }

        public virtual bool Private
        {
            get { return false; }
        }

        public bool HasError
        {
            get { return ErrorCode != 0; }
        }

#endregion

#region Class methods

        protected BaseResponse() { }

        protected BaseResponse(BasePacket packet)
        {
            Id = packet.Id;
        }

        public T As<T>() where T : BaseResponse
        {
            Assert();
            return this as T;
        }

        public void Assert()
        {
            if (HasError)
            {
                throw new Exception(ToString());
            }
        }

        public override string ToString()
        {
            if (HasError)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Error code: {0}", ErrorCode);
                return sb.ToString();
            }
            return "No errors";
        }

#endregion

#region Class static methods

        public static Exception Throw(string message, int errorCode)
        {
            Exception ex = new Exception(message);
            ex.Data.Add("ErrorCode", errorCode);
            throw ex;
        }

        public static BaseResponse FromException(BasePacket packet, Exception ex)
        {
            return FromException(packet.Id, packet.Type, ex);
        }

        public static BaseResponse FromException(long id, PacketType type, Exception ex) 
        {
            short errorCode = ex.Data.Contains("ErrorCode")
                                ? (short)ex.Data["ErrorCode"]
                                : ECGeneral.UnknownError;
            BaseResponse response = AllocNew(type);
            response.Id = id;
            response.ErrorCode = errorCode;
            return response;
        }

#endregion

#region Serialize

        protected virtual void SerializeToMemory(byte* bData, int pos) { }

        public byte[] Serialize()
        {
            int packetSize = BASE_SIZE + SizeOf;
            byte[] data = new byte[packetSize];
            fixed (byte* bData = data)
            {
                (*(short*)bData) = (short)packetSize;
                (*(PacketType*)&bData[4]) = Type;
                (*(long*)&bData[6]) = Id;
                (*(short*)&bData[14]) = ErrorCode;
                SerializeToMemory(bData, BASE_SIZE);

                if (Compressed)
                {
                    byte[] compressed = Compression.Compress(data, BASE_SIZE);
                    int compressedLength = compressed.Length;
                    packetSize = BASE_SIZE + compressedLength;
                    Array.Resize(ref data, packetSize);
                    Array.Copy(compressed, 0, data, BASE_SIZE, compressedLength);
                    fixed (byte* bNewData = data)
                    {
                        (*(short*)bNewData) = (short)packetSize;
                    }
                }
            }
            return data;
        }

#endregion

#region Deserialize

        private static BaseResponse AllocNew(PacketType packetType)
        {
            switch (packetType)
            {
                    //User
                case PacketType.UserLogin:
                    return new RUserLogin();
                case PacketType.UserEnter:
                    return new RUserEnter();

                    //Player
                case PacketType.PlayerEnter:
                    return new RPlayerEnter();
                case PacketType.PlayerExit:
                    return new RPlayerExit();
                case PacketType.PlayerMove:
                    return new RPlayerMove();
                case PacketType.PlayerRotate:
                    return new RPlayerRotate();

                    //Map
                case PacketType.MapBuffer:
                    return new RMapBuffer();

                    //Base
                default:
                    return null;
            }
        }

        protected virtual void DeserializeFromMemory(byte* bData, int pos) { }

        public static BaseResponse Deserialize(byte[] data, int dataSize, int pos, 
            out int responseSize)
        {
            dataSize -= pos;
            if (dataSize < BASE_SIZE)
            {
                responseSize = -1;
                return null;
            }

            BaseResponse response;

            fixed (byte* bData = &data[pos])
            {
                responseSize = *((int*)bData);
                if (dataSize < responseSize)
                {
                    return null;
                }

                PacketType responseType = (PacketType)(*(short*)&bData[4]);
                response = AllocNew(responseType);

                response.Id = *(long*)&bData[6];
                response.ErrorCode = *(short*)&bData[14];

                if (!response.Compressed)
                {
                    response.DeserializeFromMemory(bData, BASE_SIZE);
                    return response;
                }
            }

            if (response.Compressed)
            {
                byte[] decompressed = Compression.Decompress(data, pos + BASE_SIZE);
                fixed (byte* pDecompressed = decompressed)
                {
                    response.DeserializeFromMemory(pDecompressed, 0);
                }
            }

            return response;
        }

#endregion

    }
}
