using System;
using System.Collections.Generic;
using System.IO;
using GMechanics.Core.GameScript.Classes;
using GMechanics.Core.GameScript.Runtime;

namespace GMechanics.Core.GameScript.Compiler
{
    public class Script
    {
        public delegate List<string> GetIncludedScriptText(string includeName);

        private List<string> _sourceLines;
        private readonly ScriptManager _scriptManager;
        private ScriptExecutable _scriptExecutable;

        public void Compile()
        {
            try
            {
                bool blockCommentStarted = false;
                ScriptLexer scriptLexer = new ScriptLexer(_sourceLines);
                List<Token> listTokens = scriptLexer.GetTokens(ref blockCommentStarted);

                // parse/compile script
                ScriptParser scriptParser = new ScriptParser(this, listTokens);
                _scriptExecutable = scriptParser.Parse();

                // optimise
                if (_scriptManager.OptimiseCode)
                {
                    new ExecutionOptimiser(_scriptExecutable).Optimise();
                }
            }
            catch (Exception ex)
            {
                throw new GameScriptException(string.Format("Error while compiling script: {0}.", ex));
            }
        }

        private void LoadScript(GetIncludedScriptText getIncludedScriptTextFunc)
        {
            Dictionary<string, bool> incluedScripts = new Dictionary<string, bool>();

            bool blockCommentStarted = false;
            int count = _sourceLines.Count;
            for (int i = 0; i < count; i++)
            {
                string line = _sourceLines[i];
                List<string> sourceLinesSingle = new List<string>();
                sourceLinesSingle.Add(line);

                ScriptLexer scriptLexer = new ScriptLexer(sourceLinesSingle);
                List<Token> listTokens;
                try
                {
                    listTokens = scriptLexer.GetTokens(ref blockCommentStarted);
                }
                catch (Exception)
                {
                    // if unexpected end of stream, ignore line
                    continue;
                }

                // ignore if empty line
                if (listTokens.Count == 0) continue;

                // ignore if first token is not include
                if (listTokens[0].Type != TokenType.Include) continue;

                // expect more tokens after include
                if (listTokens.Count < 2)
                    throw new ParserException("Include path expected in include statement.");

                // expect string literal
                if (listTokens[1].Type != TokenType.String)
                    throw new ParserException("string literal expected after 'include' keyword.");

                // expect semicolon
                if (listTokens.Count < 3)
                    throw new ParserException("Semicolon ';' expected at the end of the include statement.");

                if (listTokens[2].Type != TokenType.SemiColon)
                    throw new ParserException("Semicolon ';' expected at the end of the include statement.");

                if (listTokens.Count > 3)
                    throw new ParserException("Nothing expected after semicolon ';' at the end of the include statement.");

                if (getIncludedScriptTextFunc == null)
                    throw new ParserException("Script include found, but GetIncludedScriptText delegate is not availabe.");

                // get include name
                string strScriptInclude = (string)listTokens[1].Lexeme;

                // remove include statement
                _sourceLines.RemoveAt(i);

                // do not include script more than once
                if (incluedScripts.ContainsKey(strScriptInclude))
                    continue;

                // load include script source
                List<string> listIncludeLines = getIncludedScriptTextFunc(strScriptInclude);

                // insert include source
                _sourceLines.InsertRange(i, listIncludeLines);

                // keep track of included scripts
                incluedScripts[strScriptInclude] = true;

                // reposition line index at newly inserted include
                --i;
            }
        }

        private void PrepareScript(bool compile, GetIncludedScriptText getIncludedScriptTextFunc)
        {
            _sourceLines.Add(" ");
            LoadScript(getIncludedScriptTextFunc);
            if (compile)
            {
                Compile();
            }
        }

        public void LoadSourceCodeFromFile(string textFile)
        {
            LoadSourceCodeFromFile(textFile, true, null);
        }

        public void LoadSourceCodeFromFile(string textFile, bool compile)
        {
            LoadSourceCodeFromFile(textFile, compile, null);
        }

        public void LoadSourceCodeFromFile(string textFile, bool compile, 
            GetIncludedScriptText getIncludedScriptTextFunc)
        {
            if (!File.Exists(textFile))
            {
                throw new GameScriptException(string.Format("Error while loading file {0}.", textFile));
            }
            _sourceLines = new List<string>(File.ReadAllLines(textFile));
            PrepareScript(compile, getIncludedScriptTextFunc);
        }

        public void LoadSourceCodeFromString(string text)
        {
            LoadSourceCodeFromString(text, true, null);
        }

        public void LoadSourceCodeFromString(string text, bool compile)
        {
            LoadSourceCodeFromString(text, compile, null);
        }

        public void LoadSourceCodeFromString(string text, bool compile,
            GetIncludedScriptText getIncludedScriptTextFunc)
        {
            text = text.Replace("\r\n", "\n");
            _sourceLines = new List<string>(text.Split(new[] { "\n" }, 
                StringSplitOptions.RemoveEmptyEntries));
            PrepareScript(compile, getIncludedScriptTextFunc);
        }

        public void LoadByteCodeFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new GameScriptException(string.Format("Error while loading file {0}.", fileName));
            }
            _scriptExecutable = new ScriptExecutable(this);
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                _scriptExecutable.Deserialize(fs);
            }
        }

        public void LoadByteCodeFromMemory(byte[] data)
        {
            if (data == null)
            {
                throw new GameScriptException("Bytecode data is null-referenced.");
            }
            _scriptExecutable = new ScriptExecutable(this);
            using (MemoryStream fs = new MemoryStream(data))
            {
                _scriptExecutable.Deserialize(fs);
            }
        }

        internal Script(ScriptManager scriptManager)
        {
            _scriptManager = scriptManager;
        }

        internal ScriptManager Manager
        {
            get { return _scriptManager; }
        }

        public ScriptExecutable Executable
        {
            get { return _scriptExecutable; }
        }
    }
}
