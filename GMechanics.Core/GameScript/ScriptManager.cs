using System.Collections.Generic;
using GMechanics.Core.GameScript.Classes;
using GMechanics.Core.GameScript.Compiler;
using GMechanics.Core.GameScript.Runtime;

namespace GMechanics.Core.GameScript
{
    /// <summary>
    /// Represents a global script domain where scripts can be
    /// loaded and executed.
    /// </summary>
    internal class ScriptManager
    {
        #region Private variables

        private readonly VariableDictionary _variableDictionaryGlobal;
        private readonly Dictionary<string, HostFunctionPrototype> _dictHostFunctionPrototypes;
        private readonly Dictionary<object, ScriptContext> _dictLocks;

        #endregion

        #region Internal properties

        internal Dictionary<object, ScriptContext> Locks
        {
            get { return _dictLocks; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Constructsa script manager.
        /// </summary>
        public ScriptManager()
        {
            _variableDictionaryGlobal = VariableDictionary.CreateGlobalDictionary();
            _dictHostFunctionPrototypes = new Dictionary<string, HostFunctionPrototype>();
            _dictLocks = new Dictionary<object, ScriptContext>();
            DebugMode = true;
            OptimiseCode = true;
        }

        /// <summary>
        /// Checks if a host function prototype is registered with the given
        /// name.
        /// </summary>
        /// <param name="strName">Name of the host function.</param>
        /// <returns>True if host function registered, or false otherwise.
        /// </returns>
        public bool IsHostFunctionRegistered(string strName)
        {
            return _dictHostFunctionPrototypes.ContainsKey(strName);
        }

        /// <summary>
        /// Registers the given <see cref="IHostModule"/> with the script
        /// manager.
        /// </summary>
        /// <param name="hostModule">Host module to register.</param>
        public void RegisterHostModule(IHostModule hostModule)
        {
            foreach (HostFunctionPrototype hostFunctionPrototype
                in hostModule.HostFunctionPrototypes)
                RegisterHostFunction(hostFunctionPrototype, hostModule);
        }

        /// <summary>
        /// Registers the given <see cref="HostFunctionPrototype"/> with an
        /// accompanying <see cref="IHostFunctionHandler"/>. Handlers
        /// defined at <see cref="ScriptContext"/> level for this function
        /// are ignored.
        /// </summary>
        /// <param name="hostFunctionPrototype">Host function prototype to
        /// register.</param>
        /// <param name="hostFunctionHandler">Handler associated with the
        /// host function.</param>
        public void RegisterHostFunction(
            HostFunctionPrototype hostFunctionPrototype,
            IHostFunctionHandler hostFunctionHandler)
        {
            string strName = hostFunctionPrototype.Name;
            if (_dictHostFunctionPrototypes.ContainsKey(strName))
                throw new GameScriptException(
                    "Host function '" + strName + "' already registered.");

            hostFunctionPrototype.Handler = hostFunctionHandler;
            _dictHostFunctionPrototypes[strName] = hostFunctionPrototype;
        }

        /// <summary>
        /// Registers the given <see cref="HostFunctionPrototype"/> without
        /// a handler. If a <see cref="Script"/> uses the given host
        /// function, the handler must be bound at
        /// <see cref="ScriptContext"/> level.
        /// </summary>
        /// <param name="hostFunctionPrototype">Host function prototype to
        /// register.</param>
        public void RegisterHostFunction(
            HostFunctionPrototype hostFunctionPrototype)
        {
            RegisterHostFunction(hostFunctionPrototype, null);
        }

        /// <summary>
        /// Clears all the currently active locks.
        /// </summary>
        public void ClearActiveLocks()
        {
            _dictLocks.Clear();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The variable dictionary at global level.
        /// </summary>
        public VariableDictionary GlobalDictionary
        {
            get { return _variableDictionaryGlobal; }
        }

        /// <summary>
        /// Registered <see cref="HostFunctionPrototype"/>s indexed by name.
        /// </summary>
        public Dictionary<string, HostFunctionPrototype> HostFunctions
        {
            get { return _dictHostFunctionPrototypes; }
        }

        /// <summary>
        /// Controls generation of debug instructions for traceability
        /// purposes.
        /// </summary>
        public bool DebugMode { get; set; }

        /// <summary>
        /// Enables or disables peephole optimisation of the generated
        /// byte code.
        /// </summary>
        public bool OptimiseCode { get; set; }

        /// <summary>
        /// The currently active locks mapped to the
        /// owning <see cref="ScriptContext"/>s.
        /// </summary>
        public Dictionary<object, ScriptContext> ActiveLocks
        {
            get { return new Dictionary<object, ScriptContext>(_dictLocks); }
        }

        #endregion
    }
}
