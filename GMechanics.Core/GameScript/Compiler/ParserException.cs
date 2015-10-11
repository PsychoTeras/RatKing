using System;
using GMechanics.Core.GameScript.Classes;

namespace GMechanics.Core.GameScript.Compiler
{
    /// <summary>
    /// Exception for script parsing errors.
    /// </summary>
    public class ParserException
        : GameScriptException
    {
        #region Private variables

        #endregion

        #region Public methods

        /// <summary>
        /// Constructs an exception.
        /// </summary>
        public ParserException()
        {
        }

        /// <summary>
        /// Constructs an exception with the given message.
        /// </summary>
        /// <param name="strMessage">Exception message.</param>
        public ParserException(string strMessage)
            : base(strMessage)
        {
        }

        /// <summary>
        /// Constructs an exception with the given message
        /// and inner exception reference.
        /// </summary>
        /// <param name="strMessage">Exception message.</param>
        /// <param name="exceptionInner">Inner exception reference.</param>
        public ParserException(string strMessage, Exception exceptionInner)
            : base(strMessage, exceptionInner)
        {
        }

        /// <summary>
        /// Constructs an exception with the given message and
        /// parsing token.
        /// </summary>
        /// <param name="strMessage">Exception message.</param>
        /// <param name="token">Parsing token related to the
        /// exception.</param>
        public ParserException(string strMessage, Token token)
            : base(strMessage + " Line " + token.SourceLine
                + ", character "+token.SourceCharacter)
        {
        }

        #endregion
    }
}
