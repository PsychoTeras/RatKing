using System;

namespace GMechanics.Core.GameScript.Classes
{
    public class GameScriptException : Exception
    {
        #region Private variables

        private readonly string _message;
        private readonly Exception _innerException;

        #endregion

        #region Public methods

        public GameScriptException()
        {
            _message = "No details specified.";
            _innerException = null;
        }

        public GameScriptException(string message)
        {
            _message = message;
            _innerException = null;
        }

        public GameScriptException(string message, Exception innerException)
        {
            _message = message;
            _innerException = innerException;
        }

        public override string ToString()
        {
            return MessageTrace;
        }

        #endregion

        #region Public properties

        public new string Message
        {
            get { return _message; }
        }

        public string MessageTrace
        {
            get
            {
                if (_innerException != null)
                {
                    string strMessageTrace = _message + " ";
                    GameScriptException exception = _innerException as GameScriptException;
                    if (exception != null)
                    {
                        strMessageTrace += exception.MessageTrace;
                    }
                    else
                    {
                        strMessageTrace += _innerException.Message;
                    }
                    return strMessageTrace;
                }
                return _message;
            }
        }

        #endregion
    }
}
