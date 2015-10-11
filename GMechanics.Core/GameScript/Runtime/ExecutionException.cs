using System;
using GMechanics.Core.GameScript.Classes;

namespace GMechanics.Core.GameScript.Runtime
{
    /// <summary>
    /// Exception thrown when runtime errors occur.
    /// </summary>
    public class ExecutionException
        : GameScriptException
    {
        /// <summary>
        /// Constructs a parameter-less exception.
        /// </summary>
        public ExecutionException() {}

        /// <summary>
        /// Constructs an exception with the given message.
        /// </summary>
        /// <param name="strMessage"></param>
        public ExecutionException(string strMessage) : base(strMessage) {}

        /// <summary>
        /// Constructs an exception with the given message
        /// and inner exception reference.
        /// </summary>
        /// <param name="strMessage">Exception message.</param>
        /// <param name="exceptionInner">Inner exception reference.</param>
        public ExecutionException(string strMessage, Exception exceptionInner)
            : base(strMessage, exceptionInner) {}
    }
}
