using System;
using System.Text;
using RK.Common.Proto.ErrorCodes;

namespace RK.Common.Proto
{
    public class BaseResponse
    {

#region Private fields

        private string _errorMessage;

#endregion

#region Properties

        public PacketType Type;

        public int ErrorCode;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                HasError = !string.IsNullOrEmpty(_errorMessage);
            }
        }

        public bool HasError { get; private set; }

#endregion
        
#region Static methods

        public static BaseResponse Successful(BasePacket packet)
        {
            return Successful(packet.Type);
        }

        public static BaseResponse Successful(PacketType type)
        {
            return new BaseResponse(type);
        }

#endregion

#region Class methods

        public BaseResponse()
        {
            _errorMessage = string.Empty;
        }

        public BaseResponse(PacketType type) : this()
        {
            Type = type;
        }

        public BaseResponse(BasePacket packet) : this(packet.Type) { }

        public BaseResponse(PacketType type, string errorMessage, int errorCode = 0)
            : this(type)
        {
            Set(type, errorMessage, errorCode);
        }

        public BaseResponse(PacketType type, Exception ex, int errorCode = 0)
            : this(type)
        {
            Set(type, ex, errorCode);
        }

        public BaseResponse Set(PacketType type, string errorMessage, int errorCode)
        {
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            return this;
        }

        public BaseResponse Set(PacketType type, Exception ex, int errorCode)
        {
            ErrorMessage = ex.Message;
            ErrorCode = errorCode;
            return this;
        }

        public override string ToString()
        {
            if (HasError)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Error message: {0}\r\n", ErrorMessage);
                sb.AppendFormat("Error code: {0}", ErrorCode);
                return sb.ToString();
            }
            return "No errors";
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
            return FromException<T>(packet.Type, ex);
        }

        public static T FromException<T>(PacketType type, Exception ex) 
            where T : BaseResponse, new()
        {
            int errorCode = ex.Data.Contains("ErrorCode")
                                ? (int) ex.Data["ErrorCode"]
                                : ECGeneral.UnknownError;
            return FromException<T>(type, ex, errorCode);
        }

        public static T FromException<T>(BasePacket packet, Exception ex, int errorCode)
            where T : BaseResponse, new()
        {
            return FromException<T>(packet.Type, ex, errorCode);
        }

        public static T FromException<T>(PacketType type, Exception ex, int errorCode)
            where T : BaseResponse, new()
        {
            return (T)new T().Set(type, ex, errorCode);
        }

#endregion

    }
}
