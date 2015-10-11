using System.Collections.Generic;
using System.IO;
using GMechanics.Core.GameScript.Classes;
using GMechanics.Core.GameScript.Compiler;
using GMechanics.Core.GameScript.Helpers;

namespace GMechanics.Core.GameScript.Runtime
{
    /// <summary>
    /// Byte code operand representation.
    /// </summary>
    public class Operand
    {
        private object _objectIndex;

        /// <summary>
        /// The operand's <see cref="OperandType"/>.
        /// </summary>
        public OperandType Type { get; set; }

        /// <summary>
        /// Value interpretation of the operand.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Index literal interpretation of the operand.
        /// </summary>
        public object IndexLiteral
        {
            get
            {
                if (Type != OperandType.LiteralIndexedVariable)
                    throw new ExecutionException(
                        "Index identifier can only be accessed for literal-indexed variables.");
                return _objectIndex;
            }
            set
            {
                if (Type != OperandType.LiteralIndexedVariable)
                    throw new ExecutionException(
                        "Index identifier can only be accessed for literal-indexed variables.");
                _objectIndex = value;
            }
        }

        /// <summary>
        /// Index variable interpretation of the operand.
        /// </summary>
        public string IndexIdentifier
        {
            get
            {
                if (Type != OperandType.VariableIndexedVariable)
                    throw new ExecutionException(
                        "Index identifier can only be accessed for variable-indexed variables.");
                return (string)_objectIndex;
            }
            set
            {
                if (Type != OperandType.VariableIndexedVariable)
                    throw new ExecutionException(
                        "Index identifier can only be accessed for variable-indexed variables.");
                _objectIndex = value;
            }
        }

        /// <summary>
        /// <see cref="ScriptInstruction"/> interpretation of the operand.
        /// </summary>
        public ScriptInstruction InstructionRef
        {
            get
            {
                if (Type != OperandType.InstructionRef)
                    throw new ParserException(
                        "Operand is not an instruction reference.");

                return (ScriptInstruction)Value;
            }
            set
            {
                if (Type != OperandType.InstructionRef)
                    throw new ParserException(
                        "Operand is not an instruction reference.");

                Value = value;
            }
        }

        /// <summary>
        /// <see cref="ScriptFunction"/> interpretation of the operand.
        /// </summary>
        public ScriptFunction ScriptFunctionRef
        {
            get
            {
                if (Type != OperandType.ScriptFunctionRef)
                    throw new ParserException(
                        "Operand is not a script function reference.");

                return (ScriptFunction)Value;
            }
            set
            {
                if (Type != OperandType.ScriptFunctionRef)
                    throw new ParserException(
                        "Operand is not a script function reference.");

                Value = value;
            }
        }

        /// <summary>
        /// <see cref="HostFunctionPrototype"/> interpretation of the
        /// operand.
        /// </summary>
        public HostFunctionPrototype HostFunctionRef
        {
            get
            {
                if (Type != OperandType.HostFunctionRef)
                    throw new ParserException(
                        "Operand is not a host function reference.");

                return (HostFunctionPrototype)Value;
            }
            set
            {
                if (Type != OperandType.HostFunctionRef)
                    throw new ParserException(
                        "Operand is not a host function reference.");

                Value = value;
            }
        }

        private Operand(OperandType type, object objectValue, object objectIndex)
        {
            Type = type;
            Value = objectValue;
            _objectIndex = objectIndex;
        }

        private string ToString(object objectValue)
        {
            return objectValue is string
                       ? string.Format("\"{0}\"", objectValue)
                       : objectValue.ToString();
        }

        /// <summary>
        /// Creates a literal operand using the given literal value.
        /// </summary>
        /// <param name="objectValue">Literal value.</param>
        /// <returns>Literal operand.</returns>
        public static Operand CreateLiteral(object objectValue)
        {
            return new Operand(OperandType.Literal, objectValue, null);
        }

        /// <summary>
        /// Creates a variable operand using the given variable identifier.
        /// </summary>
        /// <param name="strIdentifier">Variable identifier.</param>
        /// <returns>Simple variable operand</returns>
        public static Operand CreateVariable(string strIdentifier)
        {
            return new Operand(OperandType.Variable, strIdentifier, null);
        }

        /// <summary>
        /// Creates a variable reference indexed by a literal value.
        /// </summary>
        /// <param name="strIdentifier">Identifier for the indexed variable.</param>
        /// <param name="objectIndex">Literal index.</param>
        /// <returns>Literal-indexed variable operand.</returns>
        public static Operand CreateLiteralIndexedVariable(
            string strIdentifier, object objectIndex)
        {
            return new Operand(OperandType.LiteralIndexedVariable, strIdentifier, objectIndex);
        }

        /// <summary>
        /// Creates a variable reference indexed by another variable.
        /// </summary>
        /// <param name="strIdentifier">Identifier for the indexed variable.
        /// </param>
        /// <param name="strIndexIdentifier">Identifier for the index.</param>
        /// <returns>Variable-indexed variable operand.</returns>
        public static Operand CreateVariableIndexedVariable(
            string strIdentifier, string strIndexIdentifier)
        {
            return new Operand(OperandType.VariableIndexedVariable, strIdentifier, strIndexIdentifier);
        }

        /// <summary>
        /// Creates a <see cref="ScriptInstruction"/> reference.
        /// </summary>
        /// <param name="scriptInstruction">Script instruction referred by
        /// the operand.</param>
        /// <returns>Script instruction reference operand.</returns>
        public static Operand CreateInstructionRef(
            ScriptInstruction scriptInstruction)
        {
            return new Operand(OperandType.InstructionRef, scriptInstruction, null);
        }

        /// <summary>
        /// Creates a <see cref="ScriptFunction"/> reference.
        /// </summary>
        /// <param name="scriptFunction">Script function referred by
        /// the operand.</param>
        /// <returns>Script function reference operand.</returns>
        public static Operand CreateScriptFunctionRef(
            ScriptFunction scriptFunction)
        {
            return new Operand(OperandType.ScriptFunctionRef, scriptFunction, null);
        }

        /// <summary>
        /// Creates a <see cref="HostFunctionPrototype"/> reference.
        /// </summary>
        /// <param name="hostFunctionPrototype">Host function referred by
        /// the operand.</param>
        /// <returns>Host function reference operand.</returns>
        public static Operand CreateHostFunctionRef(
            HostFunctionPrototype hostFunctionPrototype)
        {
            return new Operand(OperandType.HostFunctionRef, hostFunctionPrototype, null);
        }

        /// <summary>
        /// Returns a string representation of the operand.
        /// </summary>
        /// <returns>string representation of the operand.</returns>
        public override string ToString()
        {
            switch (Type)
            {
                case OperandType.Literal:
                    return ToString(Value);
                case OperandType.Variable:
                    return Value.ToString();
                case OperandType.LiteralIndexedVariable:
                    return Value + "[" + ToString(_objectIndex) + "]";
                case OperandType.VariableIndexedVariable:
                    return Value + "[" + _objectIndex + "]";
                case OperandType.InstructionRef:
                    return "[" + ((ScriptInstruction) Value).Address.ToString("00000000") + "]";
                case OperandType.ScriptFunctionRef:
                    {
                        ScriptFunction scriptFunction = (ScriptFunction)Value;
                        return "[" + scriptFunction.EntryPoint.Address.ToString("00000000") + "] : " +
                               scriptFunction.Name + "(...)";
                    }
                case OperandType.HostFunctionRef:
                    return Value.ToString();
                default:
                    return Type.ToString();
            }
        }

        internal void PrepareObjectsDictionary(ObjectsDictionaryHashSet dictionary)
        {
            bool isNotNull = _objectIndex != null;
            if (isNotNull)
            {
                dictionary.Idxs.Add((string)_objectIndex);
            }

            isNotNull = Value != null && Value != NullReference.Instance;
            if (isNotNull)
            {
                switch (Type)
                {
                    case OperandType.Literal:
                        {
                            dictionary.Lits.Add(Value.ToString());
                            break;
                        }
                    case OperandType.Variable:
                    case OperandType.LiteralIndexedVariable:
                    case OperandType.VariableIndexedVariable:
                        {
                            dictionary.Vars.Add((string) Value);
                            break;
                        }
                    case OperandType.InstructionRef:
                        {
                            ((ScriptInstruction) Value).PrepareObjectsDictionary(dictionary);
                            break;
                        }
                    case OperandType.ScriptFunctionRef:
                        {
                            dictionary.Fncs.Add(((ScriptFunction) Value).Name);
                            break;
                        }
                    case OperandType.HostFunctionRef:
                        {
                            dictionary.Fncs.Add(((HostFunctionPrototype) Value).Name);
                            break;
                        }
                }
            }
        }

        internal void Serialize(ObjectsDictionarySortedSet sortedSet, BinaryWriter bw)
        {
            Helper.WriteBinaryIndex(bw, _objectIndex, sortedSet.Idxs);

            bw.Write((byte) Type);
            switch (Type)
            {
                    //Null, NullReference, String, Int, Boolean...
                case OperandType.Literal:
                    {
                        Helper.WriteTypedBinaryIndex(bw, Value, sortedSet.Lits);
                        break;
                    }
                    //Only not nullable strings
                case OperandType.Variable:
                case OperandType.LiteralIndexedVariable:
                case OperandType.VariableIndexedVariable:
                    {
                        Helper.WriteBinaryIndex(bw, Value, sortedSet.Vars);
                        break;
                    }
                case OperandType.InstructionRef:
                    {
                        ((ScriptInstruction)Value).Serialize(sortedSet, bw);
                        break;
                    }
                case OperandType.ScriptFunctionRef:
                    {
                        Helper.WriteBinaryIndex(bw, ((ScriptFunction)Value).Name, sortedSet.Fncs);
                        break;
                    }
                case OperandType.HostFunctionRef:
                    {
                        Helper.WriteBinaryIndex(bw, ((HostFunctionPrototype)Value).Name, sortedSet.Fncs);
                        break;
                    }
            }
        }

        internal static Operand Deserialize(BinaryReader br, 
            ObjectsDictionary dictionary,
            Dictionary<string, ScriptFunction> scriptFunctions,
            Dictionary<string, HostFunctionPrototype> hostsFunctions)
        {
            Operand op = new Operand(OperandType.Literal, null, null);
            op._objectIndex = Helper.ReadBinaryString(br, dictionary.Idxs);

            op.Type = (OperandType) br.ReadByte();
            switch (op.Type)
            {
                case OperandType.Literal:
                    {
                        op.Value = Helper.ReadTypedBinaryObject(br, dictionary.Lits);
                        break;
                    }
                case OperandType.Variable:
                case OperandType.LiteralIndexedVariable:
                case OperandType.VariableIndexedVariable:
                    {
                        op.Value = Helper.ReadBinaryString(br, dictionary.Vars);
                        break;
                    }
                case OperandType.InstructionRef:
                    {
                        op.Value = ScriptInstruction.Deserialize(br, dictionary, 
                            scriptFunctions, hostsFunctions);
                        break;
                    }
                case OperandType.ScriptFunctionRef:
                    {
                        op.Value = scriptFunctions[Helper.ReadBinaryString(br, 
                            dictionary.Fncs)];
                        break;
                    }
                case OperandType.HostFunctionRef:
                    {
                        op.Value = hostsFunctions[Helper.ReadBinaryString(br, 
                            dictionary.Fncs)];
                        break;
                    }
            }

            return op;
        }
    }
}
