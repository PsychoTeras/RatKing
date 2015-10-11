using System;
using GMechanics.Core.GameScript.Compiler;
using GMechanics.Core.GameScript.Runtime;

namespace GMechanics.Core.GameScript
{
    [Serializable]
    public class ScriptExecuter
    {

#region Static instance of ScriptManager

        static readonly ScriptManager ScriptManager = new ScriptManager();

        static ScriptExecuter()
        {
            ScriptManager.DebugMode = false;
            ScriptManager.OptimiseCode = true;
            ScriptProcessor.RegisterScriptHostFunctions(ScriptManager, new ScriptProcessor());
        }

#endregion

#region Private members

        [NonSerialized]
        private readonly Script _script;

        [NonSerialized]
        private readonly byte[] _byteCode;

#endregion

#region Properties

        public byte[] ByteCode
        {
            get { return _byteCode; }
        }

#endregion

#region Class fucntions

        public ScriptExecuter(byte[] byteCode)
        {
            _script = new Script(ScriptManager);
            _script.LoadByteCodeFromMemory(byteCode);
            _byteCode = byteCode;
        }

        internal void Execute(ScriptExecutionLinkedData data)
        {
            lock (data.Object)
            {
                _script.Executable.Dictionary[ScriptKeywords.Object] = data.Object;
                _script.Executable.Dictionary[ScriptKeywords.Subject] = data.Subject;
                _script.Executable.Dictionary[ScriptKeywords.Member] = data.Member;
                if (data.OldValue != null)
                {
                    _script.Executable.Dictionary[ScriptKeywords.OldValue] = data.OldValue;
                }
                if (data.Value != null)
                {
                    _script.Executable.Dictionary[ScriptKeywords.Value] = data.Value;
                }
                _script.Executable.Dictionary[ScriptKeywords.CallerThread] = data.CallerThread;
                _script.Executable.Dictionary[ScriptKeywords.Cancelled] = data.Cancelled;

                new ScriptContext(_script, data).Execute();

                if (data.Value != null)
                {
                    data.Value = _script.Executable.Dictionary[ScriptKeywords.Value];
                }
                data.Cancelled = (bool) _script.Executable.Dictionary[ScriptKeywords.Cancelled];

                _script.Executable.Dictionary.Clear();
            }
        }

#endregion

    }
}
