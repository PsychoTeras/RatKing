using System.Collections.ObjectModel;
using GMechanics.Core.GameScript.Compiler;

namespace GMechanics.Core.GameScript.Runtime
{
    /// <summary>
    /// Represents a complete host funciton implementation module intended
    /// for bulk registration of host functions, possibly provided by a
    /// third party.
    /// </summary>
    public interface IHostModule : IHostFunctionHandler
    {
        #region Public properties

        /// <summary>
        /// Host function prototypes defined and implemented by the module.
        /// </summary>
        ReadOnlyCollection<HostFunctionPrototype> HostFunctionPrototypes { get; }

        #endregion
    }
}
