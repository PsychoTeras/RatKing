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

        protected virtual int SizeOf
        {
            get { return BASE_SIZE; }
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
            T t = new T();
            t.Id = id;
            t.Type = type;
            t.ErrorCode = errorCode;
            return t;
        }

#endregion

#region Serialize

        protected virtual void SerializeToMemory(byte* bData, int pos) { }

        public byte[] Serialize()
        {
            int packetSize = SizeOf;
            byte[] data = new byte[packetSize];
            fixed (byte* bData = data)
            {
                (*(int*) bData) = packetSize;
                (*(PacketType*) &bData[4]) = Type;
                (*(long*) &bData[6]) = Id;
                (*(short*) &bData[14]) = ErrorCode;
                SerializeToMemory(bData, BASE_SIZE);
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

        protected virtual void DeserializeFromMemory(byte* bData, int pos) { }

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

                response.Type = *(PacketType*)&bData[4];
                response.Id = *(long*)&bData[6];
                response.ErrorCode = *(short*)&bData[14];

                response.DeserializeFromMemory(bData, BASE_SIZE);

                return response;
            }
        }

#endregion

    }
}
