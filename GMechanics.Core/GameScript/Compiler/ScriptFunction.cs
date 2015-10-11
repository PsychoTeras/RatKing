using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using GMechanics.Core.GameScript.Classes;
using GMechanics.Core.GameScript.Helpers;
using GMechanics.Core.GameScript.Runtime;

namespace GMechanics.Core.GameScript.Compiler
{
    /// <summary>
    /// Represents a compiled function within a script. The function defines
    /// the number of parameters accepted by the function and the associated
    /// entry point into the executable.
    /// </summary>
    public class ScriptFunction
    {
        private List<string> _listParameters;

        /// <summary>
        /// The <see cref="ScriptExecutable"/> associated with the
        /// <see cref="Script"/> that contains the function.
        /// </summary>
        public ScriptExecutable Executable { get; private set; }

        /// <summary>
        /// Name of the function.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Number of parameters accepted by the function.
        /// </summary>
        public uint ParameterCount
        {
            get { return (uint)_listParameters.Count; }
        }

        /// <summary>
        /// List of parameter names.
        /// </summary>
        public ReadOnlyCollection<string> Parameters
        {
            get { return _listParameters.AsReadOnly(); }
        }

        /// <summary>
        /// Entry point into the <see cref="ScriptExecutable"/> in
        /// <see cref="ScriptInstruction"/> form.
        /// </summary>
        private object _entryPoint;
        public ScriptInstruction EntryPoint
        {
            get { return _entryPoint as ScriptInstruction; }
            set { _entryPoint = value; }
        }

        /// <summary>
        /// Constructs a script function for the given
        /// <see cref="ScriptExecutable"/>, with the given name, parameter
        /// count and entry point into the executable.
        /// </summary>
        /// <param name="scriptExecutable">Executable form of the
        /// <see cref="Script"/>.</param>
        /// <param name="strName">Script function name.</param>
        /// <param name="listParameters">List of parameter names.</param>
        /// <param name="scriptInstructionEntryPoint">Entry point
        /// <see cref="ScriptInstruction"/> in the executable.</param>
        public ScriptFunction(ScriptExecutable scriptExecutable,
            string strName, IEnumerable<string> listParameters,
            ScriptInstruction scriptInstructionEntryPoint)
        {
            Executable = scriptExecutable;
            Name = strName;
            _listParameters = listParameters != null ? new List<string>(listParameters) : null;
            EntryPoint = scriptInstructionEntryPoint;
        }

        /// <summary>
        /// Returns a string representation of the function.
        /// </summary>
        /// <returns>string representation of the function.</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Name);
            stringBuilder.Append("(");
            for (int index = 0; index < _listParameters.Count; index++)
            {
                if (index > 0) stringBuilder.Append(", ");
                stringBuilder.Append(_listParameters[index]);
            }
            stringBuilder.Append(") [");
            stringBuilder.Append(ParameterCount.ToString("00000000"));
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        internal void PrepareObjectsDictionary(ObjectsDictionaryHashSet dictionary)
        {
            //Store dictionary name
            dictionary.Fncs.Add(Name);

            //Store parameters name
            foreach (string param in _listParameters)
            {
                dictionary.Vars.Add(param);
            }
        }

        internal void Serialize(ObjectsDictionarySortedSet sortedSet, BinaryWriter bw)
        {
            //Write name
            Helper.WriteBinaryIndex(bw, Name, sortedSet.Fncs);

            //Write parameters
            bw.Write((byte)_listParameters.Count);
            foreach (string param in _listParameters)
            {
                Helper.WriteBinaryIndex(bw, param, sortedSet.Vars);
            }

            //Write address of entry point
            bw.Write(EntryPoint.Address);
        }

        internal static ScriptFunction Deserialize(BinaryReader br, ObjectsDictionary dictionary,
            ScriptExecutable se)
        {
            ScriptFunction sf = new ScriptFunction(se, null, null, null);
            sf.Name = Helper.ReadBinaryString(br, dictionary.Fncs);

            int count = br.ReadByte();
            sf._listParameters = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                string param = Helper.ReadBinaryString(br, dictionary.Vars);
                sf._listParameters.Add(param);
            }

            sf._entryPoint = (int)br.ReadUInt32();

            return sf;
        }

        public void CorrectEntryPoint(List<ScriptInstruction> si)
        {
            if (si.Count > 0)
            {
                _entryPoint = si[(int) _entryPoint];
            }
        }
    }
}
