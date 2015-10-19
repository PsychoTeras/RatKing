using System;
using System.Text;
using RK.Common.Proto.ErrorCodes;
using RK.Common.Proto.Responses;

namespace RK.Common.Proto
{
    public unsafe class BaseResponse
    {

#region Constants

        protected const int BASE_SIZE = 16;

#endregion

#region Public fields

        public long Id;
        public PacketType Type;
        public short ErrorCode;

#endregion

#region Properties

        public bool HasError
        {
            get { return ErrorCode != 0; }
        }

#endregion
        
#region Static methods

        public static BaseResponse Successful(BasePacket packet)
        {
            return new BaseResponse(packet);
        }

#endregion

#region Class methods

        public BaseResponse() { }

        public BaseResponse(PacketType type)
        {
            Type = type;
        }

        public BaseResponse(BasePacket packet)
        {
            Id = packet.Id;
            Type = packet.Type;
        }

        private BaseResponse Set(long id, PacketType type, short errorCode)
        {
            Id = id;
            Type = type;
            ErrorCode = errorCode;
            return this;
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

        protected virtual BaseResponse Set(BasePacket p)
        {
            Id = p.Id;
            Type = p.Type;
            return this;
        }

#endregion

#region Class static methods

        public static T FromPacket<T>(BasePacket p)
            where T: BaseResponse, new()
        {
            return (T) new T().Set(p);
        }

        public static Exception Throw(string message, int errorCode)
        {
            Exception ex = new Exception(message);
            ex.Data.Add("ErrorCode", errorCode);
            throw ex;
        }

        public static T FromException<T>(BasePacket packet, Exception ex)
            where T : BaseResponse, new()
        {
            return FromException<T>(packet.Id, packet.Type, ex);
        }

        public static T FromException<T>(long id, PacketType type, Exception ex) 
            where T : BaseResponse, new()
        {
            short errorCode = ex.Data.Contains("ErrorCode")
                                ? (short)ex.Data["ErrorCode"]
                                : ECGeneral.UnknownError;
            return FromException<T>(id, type, errorCode);
        }

        public static T FromException<T>(long id, PacketType type, short errorCode)
            where T : BaseResponse, new()
        {
            return (T)new T().Set(id, type, errorCode);
        }

#endregion

#region Serialize

        protected void SerializeHeader(byte* bData, int packetSize)
        {
            (*(int*)bData) = packetSize;
            (*(PacketType*)&bData[4]) = Type;
            (*(long*)&bData[6]) = Id;
            (*(short*)&bData[14]) = ErrorCode;
        }

        public virtual byte[] Serialize()
        {
            byte[] data = new byte[BASE_SIZE];
            fixed (byte* bData = data)
            {
                SerializeHeader(bData, BASE_SIZE);
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

                    //Player
                case PacketType.PlayerEnter:
                    return new RPlayerEnter();
                case PacketType.PlayerMove:
                    return new RPlayerMove();
                case PacketType.PlayerRotate:
                    return new RPlayerRotate();

                    //Base
                default:
                    return new BaseResponse();
            }
        }

        internal virtual void InitializeFromMemory(byte* bData)
        {
            Type = *(PacketType*)&bData[4];
            Id = *(long*)&bData[6];
            ErrorCode = *(short*)&bData[14];
        }

        public static BaseResponse Deserialize(byte[] data, out int responseSize)
        {
            int dataLength = data.Length;
            if (dataLength < BASE_SIZE)
            {
                responseSize = -1;
                return null;
            }

            fixed (byte* bData = data)
            {
                responseSize = *((int*)bData);
                if (responseSize < dataLength)
                {
                    return null;
                }

                PacketType responseType = (PacketType)(*(short*)&bData[4]);
                BaseResponse response = AllocNew(responseType);
                response.InitializeFromMemory(bData);
                return response;
            }
        }

#endregion

    }
}
