using System.Collections.Generic;
using GMechanics.Core.GameScript.Compiler;
using GMechanics.Core.GameScript.Runtime;

namespace GMechanics.Core.GameScript
{
    public partial class ScriptProcessor : IHostFunctionHandler
    {
        private ScriptManager _scriptManager;

        public delegate void LogEvent(string message);
        public LogEvent OnLogEvent;

        public delegate void Event(List<object> listParameters, bool interrupted);
        public Event OnEvent;

        public ScriptProcessor()
        {
            InitializeScriptEngine();
        }

        private void InitializeScriptEngine()
        {
            _scriptManager = new ScriptManager();
            _scriptManager.DebugMode = false;
            _scriptManager.OptimiseCode = true;
            RegisterScriptHostFunctions(_scriptManager, this);
        }

        public byte[] Compile(string scriptSource)
        {
            Script script = new Script(_scriptManager);
            script.LoadSourceCodeFromString(scriptSource);
            return script.Executable.Serialize();
        }
    }
}
