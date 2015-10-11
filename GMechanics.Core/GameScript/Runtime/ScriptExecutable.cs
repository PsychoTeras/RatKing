using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using GMechanics.Core.GameScript.Classes;
using GMechanics.Core.GameScript.Compiler;

namespace GMechanics.Core.GameScript.Runtime
{
    /// <summary>
    /// Represents the compiled execuable form of a script.
    /// </summary>
    public class ScriptExecutable
    {
        private readonly Script _script;
        private List<ScriptInstruction> _scriptInstructions;
        private Dictionary<string, ScriptFunction> _scriptFunctions;
        private VariableDictionary _scriptDictionary;

        /// <summary>
        /// <see cref="Script"/> associated with the executable.
        /// </summary>
        public Script Script
        {
            get { return _script; }
        }

        /// <summary>
        /// Instruction stream comprising the executable form.
        /// </summary>
        public ReadOnlyCollection<ScriptInstruction> Instructions
        {
            get { return _scriptInstructions.AsReadOnly(); }
        }

        /// <summary>
        /// <see cref="ScriptFunction"/> map indexed by function name.
        /// </summary>
        public Dictionary<string, ScriptFunction> Functions
        {
            get { return _scriptFunctions; }
        }

        /// <summary>
        /// Returns the 'main' <see cref="ScriptFunction"/> if defined
        /// or throws an exception otherwise.
        /// </summary>
        public ScriptFunction MainFunction
        {
            get
            {
                if (!_scriptFunctions.ContainsKey("main"))
                    throw new ParserException(
                        "The script does not contain a main(...) function.");
                return _scriptFunctions["main"];
            }
        }

        /// <summary>
        /// The variable dictionary with a script scope.
        /// </summary>
        public VariableDictionary Dictionary
        {
            get { return _scriptDictionary; }
        }

        private void ShiftAddressRef(Operand operand)
        {
            // ignore null operand
            if (operand == null) return;
            // ignore if any operand other than instruction ref
            if (operand.Type != OperandType.InstructionRef) return;
            ScriptInstruction scriptInstructionRef = operand.InstructionRef;

            // ignore if refererenced instruction is not NOP or DBG
            if (scriptInstructionRef.Opcode != Opcode.NOP
                && scriptInstructionRef.Opcode != Opcode.DBG) return;
            
            // shift references to next non-NOP/DBG instruction
            ScriptInstruction scriptInstructionNext = scriptInstructionRef;
            while (scriptInstructionNext.Opcode == Opcode.NOP
                || scriptInstructionNext.Opcode == Opcode.DBG)
                scriptInstructionNext = _scriptInstructions[
                    (int)scriptInstructionNext.Address + 1];
            operand.InstructionRef = scriptInstructionNext;
        }

        internal void EliminateNullOpcodes()
        {
            RenumberInstructions();

            // process script instructions
            foreach (ScriptInstruction scriptInstruction in _scriptInstructions)
            {
                ShiftAddressRef(scriptInstruction.Operand0);
                ShiftAddressRef(scriptInstruction.Operand1);
            }

            // process entry points
            foreach (ScriptFunction scriptFunction in _scriptFunctions.Values)
            {
                ScriptInstruction scriptInstruction = scriptFunction.EntryPoint;

                ScriptInstruction scriptInstructionNext = scriptInstruction;
                while (scriptInstructionNext.Opcode == Opcode.NOP
                    || scriptInstructionNext.Opcode == Opcode.DBG)
                    scriptInstructionNext = _scriptInstructions[
                        (int)scriptInstructionNext.Address + 1];
                scriptFunction.EntryPoint = scriptInstructionNext;
            }

            // remove NOPs
            for (int i = _scriptInstructions.Count - 1; i >= 0; i--)
                if (_scriptInstructions[i].Opcode == Opcode.NOP)
                    _scriptInstructions.RemoveAt(i);

            RenumberInstructions();
        }

        internal void RenumberInstructions()
        {
            for (int i = 0; i < _scriptInstructions.Count; i++)
                _scriptInstructions[i].Address = (uint)i;
        }

        internal List<ScriptInstruction> InstructionsInternal
        {
            get { return _scriptInstructions; }
        }

        /// <summary>
        /// Constructs an executable form for the given <see cref="Script"/>.
        /// </summary>
        /// <param name="script"><see cref="Script"/> associated with the
        /// executable.</param>
        public ScriptExecutable(Script script)
        {
            _script = script;
            _scriptInstructions = new List<ScriptInstruction>();
            _scriptFunctions = new Dictionary<string, ScriptFunction>();
            if (script.Manager != null)
            {
                _scriptDictionary = VariableDictionary.CreateScriptDictionary(
                    script.Manager.GlobalDictionary);
            }
        }

        /// <summary>
        /// Checks if the executable has a 'main' function defined.
        /// </summary>
        /// <returns>True if 'main' function defined, false otherwise.
        /// </returns>
        public bool HasMainFunction()
        {
            return _scriptFunctions.ContainsKey("main");
        }

        private void PrepareObjectsDictionary(ObjectsDictionaryHashSet dictionary)
        {
            //Prepare dictionary for Script Instructions
            foreach (ScriptInstruction si in _scriptInstructions)
            {
                si.PrepareObjectsDictionary(dictionary);
            }

            //Prepare dictionary for Script Functions
            foreach (KeyValuePair<string, ScriptFunction> pair in _scriptFunctions)
            {
                pair.Value.PrepareObjectsDictionary(dictionary);
            }
        }

        private void Serialize(ObjectsDictionaryHashSet dictionary,
                               ObjectsDictionarySortedSet sortedSet, 
                               BinaryWriter bw)
        {
            //Write dictionary
            dictionary.Serialize(bw);

            //Serialize Script Functions list
            bw.Write((ushort)_scriptFunctions.Count);
            foreach (KeyValuePair<string, ScriptFunction> pair in _scriptFunctions)
            {
                pair.Value.Serialize(sortedSet, bw);
            }

            //Serialize Script Instructions list
            bw.Write((ushort)_scriptInstructions.Count);
            foreach (ScriptInstruction si in _scriptInstructions)
            {
                si.Serialize(sortedSet, bw);
            }
        }

        public byte[] Serialize()
        {
            byte[] result;

            //Prepare objects dictionary
            ObjectsDictionaryHashSet dictionary = new ObjectsDictionaryHashSet();
            PrepareObjectsDictionary(dictionary);
            
            //Serialize script
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    ObjectsDictionarySortedSet sortedSet = new ObjectsDictionarySortedSet(dictionary);
                    Serialize(dictionary, sortedSet, bw);
                }
                result = ms.ToArray();
            }

            //Return serialized result
            return result;
        }

        private void Deserialize(BinaryReader br, ObjectsDictionary dictionary)
        {
            //Deserialize Script Functions list
            int count = br.ReadUInt16();
            _scriptFunctions = new Dictionary<string, ScriptFunction>(count);
            for (int i = 0; i < count; i++)
            {
                ScriptFunction sf = ScriptFunction.Deserialize(br, dictionary, this);
                _scriptFunctions.Add(sf.Name, sf);
            }

            //Deserialize Script Instructions list
            count = br.ReadUInt16();
            _scriptInstructions = new List<ScriptInstruction>(count);
            for (int i = 0; i < count; i++)
            {
                ScriptInstruction si = ScriptInstruction.Deserialize(br, 
                    dictionary, _scriptFunctions, _script.Manager.HostFunctions);
                _scriptInstructions.Add(si);
            }

            //Correct script functions entry point
            foreach (ScriptFunction sf in _scriptFunctions.Values)
            {
                sf.CorrectEntryPoint(_scriptInstructions);
            }

            //Reinitialize Variable Dictionary Script
            _scriptDictionary = VariableDictionary.CreateScriptDictionary(
                                            _script.Manager.GlobalDictionary);

        }

        public void Deserialize(Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream))
            {
                //Load dictionary
                ObjectsDictionary dictionary = new ObjectsDictionary(br);

                //Deserialize
                Deserialize(br, dictionary);
            }
        }
    }
}
