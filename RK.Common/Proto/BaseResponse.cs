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

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                HasError = !string.IsNullOrEmpty(_errorMessage);
            }
        }
        public int ErrorCode { get; set; }
        public bool HasError { get; private set; }

#endregion
        
#region Static methods

        public static BaseResponse Successful
        {
            get { return new BaseResponse(); }
        }

#endregion

#region Class methods

        public BaseResponse()
        {
            _errorMessage = string.Empty;
        }

        public BaseResponse(string errorMessage, int errorCode = 0)
        {
            Set(errorMessage, errorCode);
        }

        public BaseResponse(Exception ex, int errorCode = 0)
        {
            Set(ex, errorCode);
        }

        public BaseResponse Set(string errorMessage, int errorCode)
        {
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            return this;
        }

        public BaseResponse Set(Exception ex, int errorCode)
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
                sb.AppendFormat("Error message:\t{0}\n", ErrorMessage);
                sb.AppendFormat("Error code:\t{0}", ErrorCode);
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

        public static T FromException<T>(Exception ex) 
            where T : BaseResponse, new()
        {
            int errorCode = ex.Data.Contains("ErrorCode")
                                ? (int) ex.Data["ErrorCode"]
                                : ECGeneral.UnknownError;
            return FromException<T>(ex, errorCode);
        }

        public static T FromException<T>(Exception ex, int errorCode)
            where T : BaseResponse, new()
        {
            return (T)new T().Set(ex, errorCode);
        }

#endregion

    }
}
