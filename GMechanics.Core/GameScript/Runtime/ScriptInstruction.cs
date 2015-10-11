using System.Collections.Generic;
using System.IO;
using System.Text;
using GMechanics.Core.GameScript.Classes;
using GMechanics.Core.GameScript.Compiler;

namespace GMechanics.Core.GameScript.Runtime
{
    /// <summary>
    /// Represents a single script instruction consisting of an operator code
    /// and optionally one or more operands.
    /// </summary>
    public class ScriptInstruction
    {
        /// <summary>
        /// Numeric address of the script instruction.
        /// </summary>
        public uint Address { get; set; }

        /// <summary>
        /// Instruction opcode.
        /// </summary>
        public Opcode Opcode { get; set; }

        /// <summary>
        /// Optional first operand.
        /// </summary>
        public Operand Operand0 { get; set; }

        /// <summary>
        /// Optional second operand.
        /// </summary>
        public Operand Operand1 { get; set; }

        /// <summary>
        /// Constructs a double-operand instruction with the given opcode and
        /// two operands.
        /// </summary>
        /// <param name="opcode">Instruction opcode.</param>
        /// <param name="operand0">First instruction operand.</param>
        /// <param name="operand1">Second instruction operand.</param>
        public ScriptInstruction(Opcode opcode, Operand operand0, Operand operand1)
        {
            Address = 0;
            Opcode = opcode;
            Operand0 = operand0;
            Operand1 = operand1;
        }

        /// <summary>
        /// Constructs a single-operand instruction with the given opcode and
        /// operand.
        /// </summary>
        /// <param name="opcode">Instruction opcode.</param>
        /// <param name="operand0">Instruction operand.</param>
        public ScriptInstruction(Opcode opcode, Operand operand0) : this(opcode, operand0, null) {}

        /// <summary>
        /// Constructs a zero-operand instruction with the given opcode.
        /// </summary>
        /// <param name="opcode">Instruction opcode.</param>
        public ScriptInstruction(Opcode opcode) : this(opcode, null, null) {}

        /// <summary>
        /// Returns a string representation of the instruction.
        /// </summary>
        /// <returns>A string representation of the instruction.</returns>
        public override string ToString()
        {
            if (Opcode == Opcode.DBG)
            {
                int iLine = (int)Operand0.Value;
                return "Ln: " + iLine.ToString("000000") + " " + Operand1.Value;
            }

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(Address.ToString("[00000000]"));
            stringBuilder.Append("    ");

            stringBuilder.Append(Opcode);
            int iOpcodeLength = Opcode.ToString().Length;
            if (iOpcodeLength == 2)
                stringBuilder.Append("  ");
            if (iOpcodeLength == 3)
                stringBuilder.Append(" ");

            if (Operand0 != null)
            {
                stringBuilder.Append(" ");
                stringBuilder.Append(Operand0);
            }

            if (Operand1 != null)
            {
                stringBuilder.Append(", ");
                stringBuilder.Append(Operand1);
            }

            return stringBuilder.ToString();
        }

        internal void PrepareObjectsDictionary(ObjectsDictionaryHashSet dictionary)
        {
            if (Operand0 != null)
            {
                Operand0.PrepareObjectsDictionary(dictionary);
            }
            if (Operand1 != null)
            {
                Operand1.PrepareObjectsDictionary(dictionary);
            }            
        }

        internal void Serialize(ObjectsDictionarySortedSet sortedSet, BinaryWriter bw)
        {
            bw.Write(Address);
            bw.Write((byte) Opcode);

            bw.Write(Operand0 != null);
            if (Operand0 != null)
            {
                Operand0.Serialize(sortedSet, bw);
            }

            bw.Write(Operand1 != null);
            if (Operand1 != null)
            {
                Operand1.Serialize(sortedSet, bw);
            }
        }

        internal static ScriptInstruction Deserialize(BinaryReader br, 
            ObjectsDictionary dictionary,
            Dictionary<string, ScriptFunction> scriptFunctions,
            Dictionary<string, HostFunctionPrototype> hostsFunctions)
        {
            ScriptInstruction si = new ScriptInstruction(Opcode.DBG);
            si.Address = br.ReadUInt32();
            si.Opcode = (Opcode) br.ReadByte();

            if (br.ReadBoolean())
            {
                si.Operand0 = Operand.Deserialize(br, dictionary, scriptFunctions,
                                                  hostsFunctions);
            }

            if (br.ReadBoolean())
            {
                si.Operand1 = Operand.Deserialize(br, dictionary, scriptFunctions,
                                                  hostsFunctions);
            }

            return si;
        }
    }
}
