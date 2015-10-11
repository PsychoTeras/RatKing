using System.Collections.Generic;

namespace GMechanics.Core.GameScript.Runtime
{
    public interface IHostFunctionHandler
    {
        object OnHostFunctionCall(string functionName, List<object> listParameters,
                                  object userData1, object userData2);
    }
}
