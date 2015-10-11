using System.Collections.Generic;
using System.Globalization;

namespace GMechanics.Core.GameScript.Compiler
{
    internal class ScriptLexer
    {
        #region Private Enumerated Types

        private enum LexState
        {
            Space,
            CommentOrDivideOrAssignDivide,
            LineComment,
            BlockCommentStart,
            BlockCommentEnd,
            AssignOrEqual,
            PlusOrIncrementOrAssignPlus,
            MinusOrDecrementOrAssignMinus,
            MultiplyOrAssignMultiply,
            PowerOrAssignPower,
            ModuloOrAssignModulo,
            And,
            Or,
            NotOrNotEqual,
            GreaterOrGreaterEqual,
            LessOrLessEqual,
            IdentifierOrKeyword,
            String,
            StringEscape,
            IntegerOrFloat,
            Float,
            Directive
        }

        #endregion

        #region Private variables

        private readonly List<string> _listSourceLines;
        private int _sourceLine;
        private int _sourceChar;
        private LexState _lexState;

        #endregion

        #region Private methods

        private void ThrowInvalidCharacterException(char ch)
        {
            throw new LexerException("Unexpected character '" + ch + "'.",
                _sourceLine, _sourceChar, _listSourceLines[_sourceLine]);
        }

        private bool EndOfSource
        {
            get { return _sourceLine >= _listSourceLines.Count; }
        }

        private char ReadChar()
        {
            if (EndOfSource)
                throw new LexerException("End of source reached.");

            char ch = _listSourceLines[_sourceLine][_sourceChar++];

            if (_sourceChar >= _listSourceLines[_sourceLine].Length)
            {
                _sourceChar = 0;
                ++_sourceLine;
            }

            return ch;
        }

        private void UndoChar()
        {
            if (_sourceLine == 0 && _sourceChar == 0)
                throw new LexerException(
                    "Cannot undo char beyond start of source.");
            --_sourceChar;
            if (_sourceChar < 0)
            {
                --_sourceLine;
                _sourceChar = _listSourceLines[_sourceLine].Length - 1;
            }
        }

        #endregion

        #region Public methods

        public ScriptLexer(List<string> listSourceLines)
        {
            _listSourceLines = new List<string>();
            foreach (string strSourceLine in listSourceLines)
                _listSourceLines.Add(strSourceLine + "\r\n");
           
            _sourceLine = 0;
            _sourceChar = 0;
            _lexState = LexState.Space;
        }

        public List<Token> GetTokens(ref bool blockCommentStarted)
        {
            _sourceLine = 0;
            _sourceChar = 0;
            _lexState = LexState.Space;
            string strToken = null;

            TokenType tokenType = TokenType.Unassigned;
            List<Token> listTokens = new List<Token>();

            while (!EndOfSource)
            {
                string strSourceLine;

                if (blockCommentStarted)
                {
                    bool hasCodeAfterComment = false;
                    while (!EndOfSource)
                    {
                        strSourceLine = _listSourceLines[_sourceLine];
                        if ((_sourceChar = strSourceLine.IndexOf(@"*/", _sourceChar)) == -1)
                        {
                            _sourceLine++;
                            _sourceChar = 0;
                        }
                        else
                        {
                            _sourceChar += 2;
                            _lexState = LexState.Space;
                            hasCodeAfterComment = true;
                            break;
                        }
                    }
                    if (hasCodeAfterComment)
                    {
                        blockCommentStarted = false;
                    }
                    else
                    {
                        return listTokens;
                    }
                }
                
                strSourceLine = _listSourceLines[_sourceLine];
                char ch = ReadChar();

                switch (_lexState)
                {
                    case LexState.Space:
                        switch (ch)
                        {
                            case ' ':
                            case '\t':
                            case '\r':
                            case '\n':
                                break; // ignore whitespace
                            case '{':
                                listTokens.Add(new Token(TokenType.LeftBrace,
                                    "{", _sourceLine, _sourceChar, strSourceLine));
                                break;
                            case '}':
                                listTokens.Add(new Token(TokenType.RightBrace, "}",
                                    _sourceLine, _sourceChar, strSourceLine));
                                break;
                            case '(':
                                Token token = listTokens[listTokens.Count - 1];
                                if (token.Type == TokenType.GameObjectProperty)
                                {
                                    token.Type = TokenType.GameObjectFunction;
                                }
                                listTokens.Add(new Token(TokenType.LeftPar, "(",
                                    _sourceLine, _sourceChar, strSourceLine));
                                break;
                            case ')':
                                listTokens.Add(new Token(TokenType.RightPar, ")",
                                    _sourceLine, _sourceChar, strSourceLine));
                                break;
                            case '[':
                                listTokens.Add(new Token(TokenType.LeftBracket, "[",
                                    _sourceLine, _sourceChar, strSourceLine));
                                break;
                            case ']':
                                listTokens.Add(new Token(TokenType.RightBracket, "]",
                                    _sourceLine, _sourceChar, strSourceLine));
                                break;
                            case '.':
                                listTokens.Add(new Token(TokenType.Period, ".",
                                    _sourceLine, _sourceChar, strSourceLine));
                                break;
                            case ',':
                                listTokens.Add(new Token(TokenType.Comma, ",",
                                    _sourceLine, _sourceChar, strSourceLine));
                                break;
                            case ';':
                                listTokens.Add(new Token(TokenType.SemiColon, ";",
                                    _sourceLine, _sourceChar, strSourceLine));
                                break;
                            case '=':
                                _lexState = LexState.AssignOrEqual;
                                break;
                            case '+':
                                _lexState = LexState.PlusOrIncrementOrAssignPlus;
                                break;
                            case '-':
                                _lexState = LexState.MinusOrDecrementOrAssignMinus;
                                break;
                            case '*':
                                _lexState = LexState.MultiplyOrAssignMultiply;
                                break;
                            case '/':
                                _lexState = LexState.CommentOrDivideOrAssignDivide;
                                break;
                            case '^':
                                _lexState = LexState.PowerOrAssignPower;
                                break;
                            case '%':
                                _lexState = LexState.ModuloOrAssignModulo;
                                break;
                            case '&':
                                _lexState = LexState.And;
                                break;
                            case '|':
                                _lexState = LexState.Or;
                                break;
                            case '!':
                                _lexState = LexState.NotOrNotEqual;
                                break;
                            case '>':
                                _lexState = LexState.GreaterOrGreaterEqual;
                                break;
                            case '<':
                                _lexState = LexState.LessOrLessEqual;
                                break;
                            case '\"':
                                strToken = "";
                                _lexState = LexState.String;
                                break;
                            case ':':
                                listTokens.Add(new Token(TokenType.Colon, ":",
                                    _sourceLine, _sourceChar, strSourceLine));
                                break;
                            case '#':
                                _lexState = LexState.Directive;
                                break;
                            default:
                                if (ch == '_' || ch == '@' || char.IsLetter(ch))
                                {
                                    _lexState = LexState.IdentifierOrKeyword;
                                    strToken = "" + ch;
                                }
                                else if (char.IsDigit(ch))
                                {
                                    strToken = "" + ch;
                                    _lexState = LexState.IntegerOrFloat;
                                }
                                else
                                    ThrowInvalidCharacterException(ch);
                                break;
                        }
                        break;
                    case LexState.CommentOrDivideOrAssignDivide:
                        switch (ch)
                        {
                            case '/':
                                _lexState = LexState.LineComment;
                                break;
                            case '*':
                                _lexState = LexState.BlockCommentStart;
                                blockCommentStarted = true;
                                break;
                            case '=':
                                listTokens.Add(new Token(TokenType.AssignDivide, "/=",
                                    _sourceLine, _sourceChar, strSourceLine));
                                _lexState = LexState.Space;
                                break;
                            default:
                                listTokens.Add(new Token(TokenType.Divide, "/",
                                    _sourceLine, _sourceChar, strSourceLine));
                                UndoChar();
                                _lexState = LexState.Space;
                                break;
                        }
                        break;
                    case LexState.Directive:
                    case LexState.LineComment:
                        if (ch == '\n')
                            _lexState = LexState.Space;
                        break;
                    case LexState.BlockCommentStart:
                        if (ch == '*')
                            _lexState = LexState.BlockCommentEnd;
                        break;
                    case LexState.BlockCommentEnd:
                        _lexState = ch == '/' ? LexState.Space : LexState.BlockCommentStart;
                        break;
                    case LexState.AssignOrEqual:
                        if (ch == '=')
                        {
                            listTokens.Add(new Token(TokenType.Equal, "==",
                                _sourceLine, _sourceChar, strSourceLine));
                            _lexState = LexState.Space;
                        }
                        else
                        {
                            listTokens.Add(new Token(TokenType.Assign, "=",
                                _sourceLine, _sourceChar, strSourceLine));
                            UndoChar();
                            _lexState = LexState.Space;
                        }
                        break;
                    case LexState.PlusOrIncrementOrAssignPlus:
                        if (ch == '+')
                        {
                            listTokens.Add(new Token(TokenType.Increment, "++",
                                _sourceLine, _sourceChar, strSourceLine));
                            _lexState = LexState.Space;
                        }
                        else if (ch == '=')
                        {
                            listTokens.Add(new Token(TokenType.AssignPlus, "+=",
                                _sourceLine, _sourceChar, strSourceLine));
                            _lexState = LexState.Space;
                        }
                        else
                        {
                            listTokens.Add(new Token(TokenType.Plus, "+",
                                _sourceLine, _sourceChar, strSourceLine));
                            UndoChar();
                            _lexState = LexState.Space;
                        }
                        break;
                    case LexState.MinusOrDecrementOrAssignMinus:
                        if (ch == '-')
                        {
                            listTokens.Add(new Token(TokenType.Decrement, "--",
                                _sourceLine, _sourceChar, strSourceLine));
                            _lexState = LexState.Space;
                        }
                        else if (ch == '=')
                        {
                            listTokens.Add(new Token(TokenType.AssignMinus, "-=",
                                _sourceLine, _sourceChar, strSourceLine));
                            _lexState = LexState.Space;
                        }
                        else
                        {
                            listTokens.Add(new Token(TokenType.Minus, "-",
                                _sourceLine, _sourceChar, strSourceLine));
                            UndoChar();
                            _lexState = LexState.Space;
                        }
                        break;
                    case LexState.MultiplyOrAssignMultiply:
                        if (ch == '=')
                        {
                            listTokens.Add(new Token(TokenType.AssignMultiply, "*=",
                                _sourceLine, _sourceChar, strSourceLine));
                            _lexState = LexState.Space;
                        }
                        else
                        {
                            listTokens.Add(new Token(TokenType.Multiply, "*",
                                _sourceLine, _sourceChar, strSourceLine));
                            UndoChar();
                            _lexState = LexState.Space;
                        }
                        break;
                    case LexState.PowerOrAssignPower:
                        if (ch == '=')
                        {
                            listTokens.Add(new Token(TokenType.AssignPower, "^=",
                                _sourceLine, _sourceChar, strSourceLine));
                            _lexState = LexState.Space;
                        }
                        else
                        {
                            listTokens.Add(new Token(TokenType.Power, "^",
                                _sourceLine, _sourceChar, strSourceLine));
                            UndoChar();
                            _lexState = LexState.Space;
                        }
                        break;
                    case LexState.ModuloOrAssignModulo:
                        if (ch == '=')
                        {
                            listTokens.Add(new Token(TokenType.AssignModulo, "%=",
                                _sourceLine, _sourceChar, strSourceLine));
                            _lexState = LexState.Space;
                        }
                        else
                        {
                            listTokens.Add(new Token(TokenType.Modulo, "%",
                                _sourceLine, _sourceChar, strSourceLine));
                            UndoChar();
                            _lexState = LexState.Space;
                        }
                        break;
                    case LexState.And:
                        if (ch == '&')
                        {
                            listTokens.Add(new Token(TokenType.And, "&&",
                                _sourceLine, _sourceChar, strSourceLine));
                            _lexState = LexState.Space;
                        }
                        else
                            ThrowInvalidCharacterException(ch);
                            break;
                    case LexState.Or:
                        if (ch == '|')
                        {
                            listTokens.Add(new Token(TokenType.Or, "||",
                                _sourceLine, _sourceChar, strSourceLine));
                            _lexState = LexState.Space;
                        }
                        else
                            ThrowInvalidCharacterException(ch);
                        break;
                    case LexState.NotOrNotEqual:
                        if (ch == '=')
                        {
                            listTokens.Add(new Token(TokenType.NotEqual, "!=",
                                _sourceLine, _sourceChar, strSourceLine));
                            _lexState = LexState.Space;
                        }
                        else
                        {
                            listTokens.Add(new Token(TokenType.Not, "!",
                                _sourceLine, _sourceChar, strSourceLine));
                            UndoChar();
                            _lexState = LexState.Space;
                        }
                        break;
                    case LexState.GreaterOrGreaterEqual:
                        if (ch == '=')
                        {
                            listTokens.Add(new Token(TokenType.GreaterOrEqual, ">=",
                                _sourceLine, _sourceChar, strSourceLine));
                            _lexState = LexState.Space;
                        }
                        else
                        {
                            listTokens.Add(new Token(TokenType.Greater, ">",
                                _sourceLine, _sourceChar, strSourceLine));
                            UndoChar();
                            _lexState = LexState.Space;
                        }
                        break;
                    case LexState.LessOrLessEqual:
                        if (ch == '=')
                        {
                            listTokens.Add(new Token(TokenType.LessOrEqual, "<=",
                                _sourceLine, _sourceChar, strSourceLine));
                            _lexState = LexState.Space;
                        }
                        else
                        {
                            listTokens.Add(new Token(TokenType.Less, "<",
                                _sourceLine, _sourceChar, strSourceLine));
                            UndoChar();
                            _lexState = LexState.Space;
                        }
                        break;
                    case LexState.IdentifierOrKeyword:
                        if (ch == '_' || char.IsLetterOrDigit(ch))
                            strToken += ch;
                        else
                        {
                            if (strToken == "include")
                                tokenType = TokenType.Include;
                            else if (strToken == "global")
                                tokenType = TokenType.Global;
                            else if (strToken == "var")
                                tokenType = TokenType.Var;
                            else if (strToken == "yield")
                                tokenType = TokenType.Yield;
                            else if (strToken == "event")
                                tokenType = TokenType.Event;
                            else if (strToken == "interrupt")
                                tokenType = TokenType.Interrupt;
                            else if (strToken == "wait")
                                tokenType = TokenType.Wait;
                            else if (strToken == "notify")
                                tokenType = TokenType.Notify;
                            else if (strToken == "lock")
                                tokenType = TokenType.Lock;
                            else if (strToken == "if")
                                tokenType = TokenType.If;
                            else if (strToken == "else")
                                tokenType = TokenType.Else;
                            else if (strToken == "while")
                                tokenType = TokenType.While;
                            else if (strToken == "for")
                                tokenType = TokenType.For;
                            else if (strToken == "foreach")
                                tokenType = TokenType.Foreach;
                            else if (strToken == "in")
                                tokenType = TokenType.In;
                            else if (strToken == "switch")
                                tokenType = TokenType.Switch;
                            else if (strToken == "case")
                                tokenType = TokenType.Case;
                            else if (strToken == "default")
                                tokenType = TokenType.Default;
                            else if (strToken == "break")
                                tokenType = TokenType.Break;
                            else if (strToken == "continue")
                                tokenType = TokenType.Continue;
                            else if (strToken == "function")
                                tokenType = TokenType.Function;
                            else if (strToken == "return")
                                tokenType = TokenType.Return;
                            else if (strToken == "thread")
                                tokenType = TokenType.Thread;
                            else if (strToken == "null")
                                tokenType = TokenType.Null;
                            else if (strToken == "true" || strToken == "false")
                                tokenType = TokenType.Boolean;
                            else if (strToken == ScriptKeywords.Subject)
                                tokenType = TokenType.GameSubject;
                            else if (strToken == ScriptKeywords.Object)
                                tokenType = TokenType.GameObject;
                            else if (strToken == ScriptKeywords.Member)
                                tokenType = TokenType.GameObjectMember;
                            else if (strToken == ScriptKeywords.OldValue)
                                tokenType = TokenType.GameObjectMemberOldValue;
                            else if (strToken == ScriptKeywords.Value)
                                tokenType = TokenType.GameObjectMemberValue;
                            else if (strToken == ScriptKeywords.Cancelled)
                                tokenType = TokenType.FlagCancelled;
                            else
                            {
                                //!!! Should to add GOMember there
                                tokenType = tokenType == TokenType.GameObject || 
                                            tokenType == TokenType.GameSubject
                                                ? TokenType.GameObjectProperty
                                                : TokenType.Identifier;
                            }

                            if (tokenType == TokenType.Boolean)
                                listTokens.Add(new Token(tokenType, strToken == "true",
                                    _sourceLine, _sourceChar, strSourceLine));
                            else
                                listTokens.Add(new Token(tokenType, strToken,
                                    _sourceLine, _sourceChar, strSourceLine));

                            UndoChar();
                            _lexState = LexState.Space;
                        }
                        break;
                    case LexState.String:
                        if (ch == '\"')
                        {
                            listTokens.Add(new Token(TokenType.String, strToken,
                                _sourceLine, _sourceChar, strSourceLine));
                            _lexState = LexState.Space;
                        }
                        else if (ch == '\\')
                            _lexState = LexState.StringEscape;
                        else if (ch == '\r' || ch == '\n')
                            throw new LexerException("string literal cannot span multiple lines.",
                                _sourceLine, _sourceChar, _listSourceLines[_sourceLine]);
                        else
                            strToken += ch;
                        break;
                    case LexState.StringEscape:
                        if (ch == '\\' || ch == '\"')
                        {
                            strToken += ch;
                            _lexState = LexState.String;
                        }
                        else if (ch == 't')
                        {
                            strToken += '\t';
                            _lexState = LexState.String;
                        }
                        else if (ch == 'r')
                        {
                            strToken += '\r';
                            _lexState = LexState.String;
                        }
                        else if (ch == 'n')
                        {
                            strToken += '\n';
                            _lexState = LexState.String;
                        }
                        else
                            throw new LexerException(
                                "Invalid string escape sequence '\\" + ch + "'.",
                                _sourceLine, _sourceChar, _listSourceLines[_sourceLine]);
                        break;
                    case LexState.IntegerOrFloat:
                        if (char.IsDigit(ch))
                            strToken += ch;
                        else if (ch == '.')
                        {
                            strToken += ch;
                            _lexState = LexState.Float;
                        }
                        else
                        {
                            if (strToken != null)
                            {
                                int iValue = int.Parse(strToken);
                                listTokens.Add(new Token(TokenType.Integer, iValue,
                                               _sourceLine, _sourceChar, strSourceLine));
                            }
                            UndoChar();
                            _lexState = LexState.Space;
                        }
                        break;
                    case LexState.Float:
                        if (char.IsDigit(ch))
                            strToken += ch;
                        else
                        {
                            if (strToken != null)
                            {
                                float fValue = float.Parse(strToken, CultureInfo.InvariantCulture.NumberFormat);
                                Token token = new Token(TokenType.Float, fValue, _sourceLine, _sourceChar,
                                                        strSourceLine);
                                listTokens.Add(token);
                            }
                            UndoChar();
                            _lexState = LexState.Space;
                        }
                        break;
                    default:
                        throw new LexerException("Unhandled lexer state.");
                }
            }

            if (_lexState != LexState.Space && _lexState != LexState.BlockCommentStart)
            {
                throw new LexerException(
                    "Unexpected end of source reached.");
            }

            return listTokens;
        }

        #endregion
    }
}
