using GMechanics.Core.GameScript.Compiler;

namespace GMechanics.Core.GameScript.Runtime
{
    /// <summary>
    /// Instruction operand type.
    /// </summary>
    public enum OperandType : byte
    {
        /// <summary>
        /// Literal value.
        /// </summary>
        Literal,

        /// <summary>
        /// Simple variable identifier.
        /// </summary>
        Variable,

        /// <summary>
        /// Variable indexed indirectly via another variable indentifier.
        /// </summary>
        VariableIndexedVariable,

        /// <summary>
        /// Variable indexed directly via a literal value.
        /// </summary>
        LiteralIndexedVariable,

        /// <summary>
        /// Reference to a <see cref="ScriptInstruction"/>.
        /// </summary>
        InstructionRef,

        /// <summary>
        /// Reference to a <see cref="ScriptFunction"/>.
        /// </summary>
        ScriptFunctionRef,

        /// <summary>
        /// Reference to a <see cref="HostFunctionPrototype"/>.
        /// </summary>
        HostFunctionRef
    }
}
