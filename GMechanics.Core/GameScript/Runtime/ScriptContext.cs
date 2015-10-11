using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.GameObjectAttributeClasses;
using GMechanics.Core.Classes.Entities.GameObjectPropertyClasses;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Interfaces;
using GMechanics.Core.Classes.Types;
using GMechanics.Core.GameScript.Compiler;

namespace GMechanics.Core.GameScript.Runtime
{
    public class ScriptContext
    {
        private class FunctionFrame
        {
            public ScriptFunction ScriptFunction;
            public VariableDictionary VariableDictionary;
            public int NextInstruction;
        }

        private IHostFunctionHandler _scriptHandler;
        private readonly ScriptFunction _scriptFunction;
        private readonly Script _script;
        private readonly ScriptExecutable _scriptExecutable;
        private readonly Stack<FunctionFrame> _stackFunctionFrames;
        private readonly Stack<object> _stackParameters;
        private readonly Dictionary<object, ScriptInstruction> _dictLocks;
        private readonly List<ScriptContext> _listThreads;
        private ScriptInstruction _scriptInstruction;
        private VariableDictionary _dictionaryLocal;
        private bool _interruptOnHostfunctionCall;
        private bool _interruped;
        private bool _terminated;

        private readonly ScriptExecutionLinkedData _executionLinkedData;
        private GameObject _currentObject;
        private GameObject _currentSubject;

        #region Private methods

        private object ResolveOperand(Operand operand)
        {
            object objectSource;
            switch (operand.Type)
            {
                case OperandType.Literal:
                    {
                        return operand.Value;
                    }

                case OperandType.Variable:
                    {
                        return _dictionaryLocal[(string) operand.Value];
                    }

                case OperandType.LiteralIndexedVariable:
                    {
                        objectSource = _dictionaryLocal[(string) operand.Value];
                        string propertyName = operand.IndexLiteral as string;

                        switch (GameEntityTypesTable.TypeOf(objectSource))
                        {
                            case GameEntityType.AssociativeArray:
                                {
                                    return ((AssociativeArray) objectSource)[operand.IndexLiteral];
                                }
                            case GameEntityType.String:
                                {
                                    string strSource = (string) objectSource;
                                    object objectIndex = operand.IndexLiteral;
                                    if (objectIndex is string && ((string) objectIndex) == "Length")
                                    {
                                        return strSource.Length;
                                    }
                                    if (objectIndex is int)
                                    {
                                        return strSource[(int) objectIndex] + "";
                                    }
                                    throw new ExecutionException("Only integers are allowed for string indexing.");
                                }
                            case GameEntityType.Float:
                            case GameEntityType.Integer:
                                {
                                    return objectSource;
                                }
                            case GameEntityType.GameObject:
                                {
                                    GameObject obj = (GameObject) objectSource;
                                    switch (propertyName)
                                    {
                                        case "Size":
                                            return obj.Size;
                                        case "Weight":
                                            return obj.Weight;
                                        case "Speed":
                                            return obj.Speed;
                                        case "Direction":
                                            return obj.Direction;
                                    }
                                    break;
                                }
                            case GameEntityType.GameObjectAttributeValue:
                                {
                                    GameObjectAttributeValue goav = (GameObjectAttributeValue) objectSource;
                                    switch (propertyName)
                                    {
                                        case "MinValue":
                                            return goav.MinValue;
                                        case "MaxValue":
                                            return goav.MaxValue;
                                        case "Value":
                                            return goav.Value;
                                        case "NestingLevel":
                                            return goav.NestingLevel;
                                    }
                                    break;
                                }
                            case GameEntityType.GameObjectProperty:
                                {
                                    GameObjectProperty gop = (GameObjectProperty) objectSource;
                                    switch (propertyName)
                                    {
                                        case "Value":
                                            return gop.Value;
                                        case "MinValue":
                                            return gop.MinValue;
                                        case "MaxValue":
                                            return gop.MaxValue;
                                    }
                                    break;
                                }
                            case GameEntityType.Size3D:
                                {
                                    Size3D size3D = (Size3D) objectSource;
                                    switch (propertyName)
                                    {
                                        case "X":
                                            return size3D.X;
                                        case "Y":
                                            return size3D.Y;
                                        case "Z":
                                            return size3D.Z;
                                    }
                                    break;
                                }
                        }

                        IClassAsAtom classAsAtom = objectSource as IClassAsAtom;
                        if (classAsAtom != null)
                        {
                            Atom atom = classAsAtom.ClassAsAtom;
                            switch (propertyName)
                            {
                                case "Name":
                                    return atom.Name;
                                case "Description":
                                    return atom.Description;
                                case "Transcription":
                                    return atom.Transcription;
                            }
                            throw new ExecutionException("Unknown atom property '" + propertyName + "'.");
                        }

                        if (objectSource is IList)
                        {
                            IList list = objectSource as IList;
                            return list.Count;
                        }

                        throw new ExecutionException("Only associative arrays, strings and lists can be indexed.");
                    }

                case OperandType.VariableIndexedVariable:
                    {
                        objectSource = _dictionaryLocal[(string) operand.Value];
                        switch (GameEntityTypesTable.TypeOf(objectSource))
                        {
                            case GameEntityType.AssociativeArray:
                                {
                                    object objectIndex = _dictionaryLocal[operand.IndexIdentifier];
                                    return ((AssociativeArray) objectSource)[objectIndex];
                                }
                            case GameEntityType.String:
                                {
                                    object objectIndex = _dictionaryLocal[operand.IndexIdentifier];
                                    if (!(objectIndex is int))
                                    {
                                        throw new ExecutionException(
                                            "Only integers are allowed for string indexing.");
                                    }
                                    return ((string) objectSource)[(int) objectIndex] + "";
                                }
                        }

                        if (objectSource is IList)
                        {
                            IList list = objectSource as IList;
                            object key = _dictionaryLocal[operand.IndexIdentifier];
                            if (key is int)
                            {
                                return list[(int) key];
                            }
                            return key;
                        }

                        throw new ExecutionException("Only associative arrays, strings and lists can be indexed.");
                    }

                default:
                    throw new ExecutionException("Cannot resolve operand type '" + operand.Type + "'.");
            }
        }

        private void AssignVariable(Operand operandDest, object objectValue)
        {
            string strIdentifierDest = (string)operandDest.Value;

            switch (operandDest.Type)
            {
                case OperandType.Variable:
                    {
                        _dictionaryLocal[strIdentifierDest] = objectValue;
                        break;
                    }

                case OperandType.LiteralIndexedVariable:
                case OperandType.VariableIndexedVariable:
                    {
                        object objectDest = _dictionaryLocal.IsDeclared(strIdentifierDest)
                                                ? _dictionaryLocal[strIdentifierDest]
                                                : NullReference.Instance;
                        string propertyName = operandDest.IndexLiteral as string;

                        switch (GameEntityTypesTable.TypeOf(objectDest))
                        {
                            case GameEntityType.AssociativeArray:
                                {
                                    AssociativeArray associativeArray = (AssociativeArray) objectDest;
                                    if (operandDest.Type == OperandType.LiteralIndexedVariable)
                                    {
                                        associativeArray[operandDest.IndexLiteral] = objectValue;
                                    }
                                    else
                                    {
                                        object objectIndex = _dictionaryLocal[operandDest.IndexIdentifier];
                                        associativeArray[objectIndex] = objectValue;
                                    }
                                    break;
                                }
                            case GameEntityType.GameObject:
                                {
                                    GameObject obj = (GameObject) objectDest;
                                    switch (propertyName)
                                    {
                                        case "Size":
                                            obj.Size = (Size3D) objectValue;
                                            break;
                                        case "Weight":
                                            obj.Weight = (float) objectValue;
                                            break;
                                        case "Speed":
                                            obj.Speed = (float) objectValue;
                                            break;
                                        case "Direction":
                                            obj.Direction = (float) objectValue;
                                            break;
                                    }
                                    break;
                                }
                            case GameEntityType.GameObjectAttributeValue:
                                {
                                    GameObjectAttributeValue goav = (GameObjectAttributeValue) objectDest;
                                    switch (propertyName)
                                    {
                                        case "Value":
                                            goav.Value = (float) objectValue;
                                            break;
                                    }
                                    break;
                                }
                            case GameEntityType.GameObjectProperty:
                                {
                                    GameObjectProperty gop = (GameObjectProperty) objectDest;
                                    switch (propertyName)
                                    {
                                        case "Value":
                                            gop.Value = (float) objectValue;
                                            break;
                                        case "MaxValue":
                                            gop.MaxValue = (float) objectValue;
                                            break;
                                    }
                                    break;
                                }
                            case GameEntityType.Size3D:
                                {
                                    Size3D size3D = (Size3D) objectDest;
                                    switch (propertyName)
                                    {
                                        case "X":
                                            size3D.X = (float) objectValue;
                                            break;
                                        case "Y":
                                            size3D.Y = (float) objectValue;
                                            break;
                                        case "Z":
                                            size3D.Z = (float) objectValue;
                                            break;
                                    }
                                    break;
                                }
                        }
                        break;
                    }

                case OperandType.Literal:
                    throw new ExecutionException("MOV destination operand cannot be a literal.");
            }
        }

        private void ProcessArithmeticInstruction()
        {
            object objectValueDest = ResolveOperand(_scriptInstruction.Operand0);
            object objectValueSource = ResolveOperand(_scriptInstruction.Operand1);

            Type typeDest = objectValueDest.GetType();
            Type typeSource = objectValueSource.GetType();

            // handle arrays and string concatenation
            if (_scriptInstruction.Opcode == Opcode.ADD)
            {
                if (typeDest == typeof(string))
                {
                    AssignVariable(_scriptInstruction.Operand0, objectValueDest + 
                                   objectValueSource.ToString());
                    return;
                }

                if (typeDest == typeof(AssociativeArray))
                {
                    ((AssociativeArray)objectValueDest).Add(objectValueSource);
                    return;
                }
            }

            // handle array and string subtraction
            if (_scriptInstruction.Opcode == Opcode.SUB)
            {
                if (typeDest == typeof(string))
                {
                    AssignVariable(_scriptInstruction.Operand0,
                        objectValueDest.ToString().Replace(objectValueSource.ToString(), ""));
                    return;
                }
                if (typeDest == typeof(AssociativeArray))
                {
                    ((AssociativeArray)objectValueDest).Subtract(objectValueSource);
                    return;
                }
            }

            float fValueDest;
            float fValueSource;
            float fResult;

            if (typeDest == typeof(int))
                fValueDest = (int)objectValueDest;
            else if (typeDest == typeof(float))
                fValueDest = (float)objectValueDest;
            else
                throw new ExecutionException(
                    "Values of type '" + typeDest.Name
                    + "' cannot be used in arithmetic instructions.");

            if (typeSource == typeof(int))
                fValueSource = (int)objectValueSource;
            else if (typeSource == typeof(float))
                fValueSource = (float)objectValueSource;
            else
                throw new ExecutionException(
                    "Values of type '" + typeSource.Name
                    + "' cannot be used in arithmetic instructions.");

            switch (_scriptInstruction.Opcode)
            {
                case Opcode.ADD: fResult = fValueDest + fValueSource; break;
                case Opcode.SUB: fResult = fValueDest - fValueSource; break;
                case Opcode.MUL: fResult = fValueDest * fValueSource; break;
                case Opcode.DIV: fResult = fValueDest / fValueSource; break;
                case Opcode.POW: fResult = (float)Math.Pow(fValueDest, fValueSource); break;
                case Opcode.MOD: fResult = fValueDest % fValueSource; break;
                default:
                    throw new ExecutionException(
                        "Invalid arithmetic instruction '"
                        + _scriptInstruction.Opcode + "'.");
            }

            if (typeDest == typeof(int) && typeSource == typeof(int))
                AssignVariable(_scriptInstruction.Operand0, (int)fResult);
            else
                AssignVariable(_scriptInstruction.Operand0, fResult);
        }

        private void ProcessRelationalInstruction()
        {
            object objectValueDest = ResolveOperand(_scriptInstruction.Operand0);
            object objectValueSource = ResolveOperand(_scriptInstruction.Operand1);

            Type typeDest = objectValueDest.GetType();
            Type typeSource = objectValueSource.GetType();

            bool bResult = false;

            // handle null comparisons
            if (typeDest == typeof(NullReference) || typeSource == typeof(NullReference))
            {
                switch (_scriptInstruction.Opcode)
                {
                    case Opcode.CEQ: bResult = objectValueDest == objectValueSource; break;
                    case Opcode.CNE: bResult = objectValueDest != objectValueSource; break;
                    default:
                        throw new ExecutionException(
                            "Only CEQ, CNE and CNL instructions may reference NULL values.");
                }
                AssignVariable(_scriptInstruction.Operand0, bResult);
                return;
            }

            // handle string comparisons
            if (typeDest == typeof(string) && typeSource == typeof(string))
            {
                string strDest = (string) objectValueDest;
                string strSource = (string) objectValueSource;
                switch (_scriptInstruction.Opcode)
                {
                    case Opcode.CEQ: bResult = strDest == strSource; break;
                    case Opcode.CNE: bResult = strDest != strSource; break;
                    case Opcode.CG: bResult = strDest.CompareTo(strSource) > 0; break;
                    case Opcode.CGE: bResult = strDest.CompareTo(strSource) >= 0; break;
                    case Opcode.CL: bResult = strDest.CompareTo(strSource) < 0; break;
                    case Opcode.CLE: bResult = strDest.CompareTo(strSource) <= 0; break;
                }
                AssignVariable(_scriptInstruction.Operand0, bResult);
                return;
            }

            float fValueDest;
            float fValueSource;

            if (typeDest == typeof(int))
                fValueDest = (int)objectValueDest;
            else if (typeDest == typeof(float))
                fValueDest = (float)objectValueDest;
            else
                throw new ExecutionException(
                    "Values of type '" + typeDest.Name
                    + "' cannot be used in relational instructions.");

            if (typeSource == typeof(int))
                fValueSource = (int)objectValueSource;
            else if (typeSource == typeof(float))
                fValueSource = (float)objectValueSource;
            else
                throw new ExecutionException(
                    "Values of type '" + typeSource.Name
                    + "' cannot be used in relational instructions.");

            switch (_scriptInstruction.Opcode)
            {
                case Opcode.CEQ: bResult = fValueDest.Equals(fValueSource); break;
                case Opcode.CNE: bResult = !fValueDest.Equals(fValueSource); break;
                case Opcode.CG: bResult = fValueDest > fValueSource; break;
                case Opcode.CGE: bResult = fValueDest >= fValueSource; break;
                case Opcode.CL: bResult = fValueDest < fValueSource; break;
                case Opcode.CLE: bResult = fValueDest <= fValueSource; break;
                default:
                    throw new ExecutionException(
                        "Invalid relational instruction '"
                        + _scriptInstruction.Opcode + "'.");
            }

            AssignVariable(_scriptInstruction.Operand0, bResult);
        }

        private void ProcessLogicalInstruction()
        {
            object objectValueDest = ResolveOperand(_scriptInstruction.Operand0);
            object objectValueSource = ResolveOperand(_scriptInstruction.Operand1);

            Type typeDest = objectValueDest.GetType();
            Type typeSource = objectValueSource.GetType();

            if (typeDest != typeof(bool))
                throw new ExecutionException(
                    "Values of type '" + typeDest.Name
                    + "' cannot be used in logical expressions.");

            if (typeSource != typeof(bool))
                throw new ExecutionException(
                    "Values of type '" + typeDest.Name
                    + "' cannot be used in logical expressions.");

            bool bResult;
            bool bValueDest = (bool)objectValueDest;
            bool bValueSource = (bool)objectValueSource;

            switch (_scriptInstruction.Opcode)
            {
                case Opcode.AND: bResult = bValueDest && bValueSource; break;
                case Opcode.OR: bResult = bValueDest || bValueSource; break;
                default:
                    throw new ExecutionException(
                        "Invalid relational instruction '"
                        + _scriptInstruction.Opcode + "'.");
            }

            AssignVariable(_scriptInstruction.Operand0, bResult);
        }

        private void ProcessIterator(AssociativeArray associativeArray)
        {
            if (associativeArray.Count == 0) return;

            object objectIterator = ResolveOperand(_scriptInstruction.Operand0);

            bool bFoundKey = false;
            object objectNextKey = null;
            foreach (object objectKey in associativeArray.Keys)
            {
                if (bFoundKey)
                {
                    objectNextKey = objectKey;
                    break;
                }

                if (objectKey == objectIterator)
                    bFoundKey = true;
            }

            if (!bFoundKey)
            {
                // if no matching iterator found, set it to first
                Dictionary<object, object>.KeyCollection.Enumerator enumKeys
                    = associativeArray.Keys.GetEnumerator();
                enumKeys.MoveNext();
                objectNextKey = enumKeys.Current;
            }

            if (objectNextKey == null)
                objectNextKey = NullReference.Instance;

            _dictionaryLocal[_scriptInstruction.Operand0.Value.ToString()]
                = objectNextKey;
        }

        private void ProcessIterator(string strValue)
        {
            if (strValue.Length == 0) return;

            object objectIterator = ResolveOperand(_scriptInstruction.Operand0);

            if (objectIterator.GetType() != typeof(int))
            {
                // if type not int, treat as mismatch and set to first
                _dictionaryLocal[_scriptInstruction.Operand0.Value.ToString()] = 0;
                return;
            }

            int iIterator = (int)objectIterator;

            if (iIterator < strValue.Length - 1)
                _dictionaryLocal[_scriptInstruction.Operand0.Value.ToString()]
                    = iIterator + 1;
            else
                _dictionaryLocal[_scriptInstruction.Operand0.Value.ToString()]
                    = NullReference.Instance;
        }

        private void ProcessIterator(IEnumerable enumerable)
        {
            object objectIterator = ResolveOperand(_scriptInstruction.Operand0);

            bool foundObj = false;
            object nextObj = null;
            enumerable = enumerable as object[] ?? enumerable.Cast<object>().ToArray();
            foreach (object obj in enumerable)
            {
                if (foundObj)
                {
                    nextObj = obj;
                    break;
                }
                foundObj |= obj == objectIterator;
            }

            if (!foundObj)
            {
                IEnumerator enumerator = enumerable.GetEnumerator();
                enumerator.MoveNext();
                nextObj = enumerator.Current;
            }

            if (nextObj == null)
            {
                nextObj = NullReference.Instance;
            }

            _dictionaryLocal[_scriptInstruction.Operand0.Value.ToString()] = nextObj;
        }

        private void ProcessEVNT(bool interruption)
        {
            int paramCnt = (int) _scriptInstruction.Operand0.Value;

            // pop values from stack into list
            List<object> listParameters = new List<object>();
            for (int i = 0; i < paramCnt; i++)
            {
                listParameters.Insert(0, _stackParameters.Pop());
            }

            if (_scriptHandler != null)
            {
                string name = interruption ? "Interrupt" : "Event";
                _scriptHandler.OnHostFunctionCall(name, listParameters, null, null);
            }

            //Set interrupted flag
            _interruped = _interruptOnHostfunctionCall || interruption;
        }

        private void ProcessDCG()
        {
            // should not run in functions
            throw new ExecutionException(
                "DCG opcodes cannot be executed within a function frame.");
        }

        private void ProcessDCL()
        {
            string strIdentifier = (string) _scriptInstruction.Operand0.Value;
            _dictionaryLocal[strIdentifier] = NullReference.Instance;
        }

        private void ProcessINT()
        {
            _interruped = true;
        }

        private void ProcessLOCK()
        {
            object objectValue
                = ResolveOperand(_scriptInstruction.Operand0);

            if (objectValue is NullReference)
                throw new ExecutionException("Lock key must be a literal value.");

            if (_script.Manager.Locks.ContainsKey(objectValue))
            {
                ScriptContext scriptContextLocks
                    = _script.Manager.Locks[objectValue];
                if (scriptContextLocks == this && _dictLocks[objectValue] != _scriptInstruction)
                    throw new ExecutionException(
                        "Nested locks cannot share the same locking key.");

                // repeat instruction
                FunctionFrame functionFrame = _stackFunctionFrames.Peek();
                --functionFrame.NextInstruction;
                

                // interrupt
                _interruped = true;
            }
            else
            {
                _script.Manager.Locks[objectValue] = this;
                _dictLocks[objectValue] = _scriptInstruction;
            }
        }

        private void ProcessULCK()
        {
            object objectValue
                = ResolveOperand(_scriptInstruction.Operand0);

            if (objectValue is NullReference)
                throw new ExecutionException("Lock key must be a literal value.");


            if (!_script.Manager.Locks.ContainsKey(objectValue))
                throw new ExecutionException("Lock '" + objectValue + "' is already unlocked.");

            _dictLocks.Remove(objectValue);
            _script.Manager.Locks.Remove(objectValue);
        }

        private void ProcessMOV()
        {
            object objectValue = ResolveOperand(_scriptInstruction.Operand1);
            AssignVariable(_scriptInstruction.Operand0, objectValue);
        }

        private void ProcessINC()
        {
            // dest id
            string strIdentifierDest
                = (string)_scriptInstruction.Operand0.Value;
            // dest value
            object objectValueDest
                = ResolveOperand(_scriptInstruction.Operand0);

            Type typeDest = objectValueDest.GetType();

            if (typeDest == typeof(int))
            {
                int iValue = (int)objectValueDest;
                _dictionaryLocal[strIdentifierDest] = ++iValue;
            }
            else if (typeDest == typeof(float))
            {
                float fValue = (float)objectValueDest;
                _dictionaryLocal[strIdentifierDest] = ++fValue;
            }
            else
                throw new ExecutionException(
                    "Values of type '" + typeDest.Name
                    + "' cannot be used in arithmetic increment instruction.");
        }

        private void ProcessDEC()
        {
            // dest id
            string strIdentifierDest
                = (string)_scriptInstruction.Operand0.Value;
            // dest value
            object objectValueDest
                = ResolveOperand(_scriptInstruction.Operand0);

            Type typeDest = objectValueDest.GetType();

            if (typeDest == typeof(int))
            {
                int iValue = (int)objectValueDest;
                _dictionaryLocal[strIdentifierDest] = --iValue;
            }
            else if (typeDest == typeof(float))
            {
                float fValue = (float)objectValueDest;
                _dictionaryLocal[strIdentifierDest] = --fValue;
            }
            else
                throw new ExecutionException(
                    "Values of type '" + typeDest.Name
                    + "' cannot be used in arithmetic decrement instruction.");
        }

        private void ProcessNEG()
        {
            // dest id
            string strIdentifierDest
                = (string)_scriptInstruction.Operand0.Value;
            // dest value
            object objectValueDest
                = ResolveOperand(_scriptInstruction.Operand0);

            Type typeDest = objectValueDest.GetType();

            if (typeDest == typeof(int))
            {
                int iValue = (int)objectValueDest;
                _dictionaryLocal[strIdentifierDest] = -iValue;
            }
            else if (typeDest == typeof(float))
            {
                float fValue = (float)objectValueDest;
                _dictionaryLocal[strIdentifierDest] = -fValue;
            }
            else
                throw new ExecutionException(
                    "Values of type '" + typeDest.Name
                    + "' cannot be used in arithmetic negation instruction.");
        }

        private void ProcessADD()
        {
            ProcessArithmeticInstruction();
        }

        private void ProcessSUB()
        {
            ProcessArithmeticInstruction();
        }

        private void ProcessMUL()
        {
            ProcessArithmeticInstruction();
        }

        private void ProcessDIV()
        {
            ProcessArithmeticInstruction();
        }

        private void ProcessPOW()
        {
            ProcessArithmeticInstruction();
        }

        private void ProcessMOD()
        {
            ProcessArithmeticInstruction();
        }

        private void ProcessCNL()
        {
            // dest id
            string strIdentifierDest = (string)_scriptInstruction.Operand0.Value;
            // dest value
            object objectValueDest = ResolveOperand(_scriptInstruction.Operand0);
            _dictionaryLocal[strIdentifierDest] = objectValueDest == NullReference.Instance;
        }

        private void ProcessCEQ()
        {
            ProcessRelationalInstruction();
        }

        private void ProcessCNE()
        {
            ProcessRelationalInstruction();
        }

        private void ProcessCG()
        {
            ProcessRelationalInstruction();
        }

        private void ProcessCGE()
        {
            ProcessRelationalInstruction();
        }

        private void ProcessCL()
        {
            ProcessRelationalInstruction();
        }

        private void ProcessCLE()
        {
            ProcessRelationalInstruction();
        }

        private void ProcessOR()
        {
            ProcessLogicalInstruction();
        }

        private void ProcessAND()
        {
            ProcessLogicalInstruction();
        }

        private void ProcessNOT()
        {
            // dest id
            string strIdentifierDest = (string)_scriptInstruction.Operand0.Value;
            // dest value
            object objectValueDest = ResolveOperand(_scriptInstruction.Operand0);
 
            Type typeDest = objectValueDest.GetType();

            if (typeDest != typeof(bool))
                throw new ExecutionException(
                    "Values of type '" + typeDest.Name
                    + "' cannot be used in logical negation instruction.");

            _dictionaryLocal[strIdentifierDest] = !((bool)objectValueDest);
        }

        private void ProcessJMP()
        {
            ScriptInstruction scriptInstructionTarget = _scriptInstruction.Operand0.InstructionRef;
            FunctionFrame functionFrame = _stackFunctionFrames.Peek();
            functionFrame.NextInstruction = (int) scriptInstructionTarget.Address;
        }
        
        private void ProcessJT()
        {
            object objectCondition = ResolveOperand(_scriptInstruction.Operand0);
            bool bCondition = (bool)objectCondition;
            if (!bCondition) return;

            ScriptInstruction scriptInstructionTarget = _scriptInstruction.Operand1.InstructionRef;
            FunctionFrame functionFrame = _stackFunctionFrames.Peek();
            functionFrame.NextInstruction = (int)scriptInstructionTarget.Address;
        }

        private void ProcessJF()
        {
            object objectCondition = ResolveOperand(_scriptInstruction.Operand0);
            bool bCondition = (bool)objectCondition;
            if (bCondition) return;

            ScriptInstruction scriptInstructionTarget = _scriptInstruction.Operand1.InstructionRef;
            FunctionFrame functionFrame = _stackFunctionFrames.Peek();
            functionFrame.NextInstruction = (int)scriptInstructionTarget.Address;
        }

        private void ProcessCLRA()
        {
            if (_scriptInstruction.Operand0.Type != OperandType.Variable)
                throw new ExecutionException(
                    "Operand in CLA instruction must be a simple variable.");
            AssociativeArray associativeArray
                = new AssociativeArray();
            string strIdentifierArray
                = _scriptInstruction.Operand0.Value.ToString();
            _dictionaryLocal[strIdentifierArray] = associativeArray;
        }

        private void ProcessNEXT()
        {
            if (_scriptInstruction.Operand0.Type != OperandType.Variable)
                throw new ExecutionException(
                    "Iterator operand in NEXT instruction must be a simple variable.");

            if (_scriptInstruction.Operand1.Type != OperandType.Variable)
                throw new ExecutionException(
                    "Array operand reference in NEXT instruction must be a simple variable.");

            object objectEnumerable = _dictionaryLocal[_scriptInstruction.Operand1.Value.ToString()];

            if (objectEnumerable is AssociativeArray)
                ProcessIterator((AssociativeArray)objectEnumerable);
            else if (objectEnumerable is string)
                ProcessIterator((string)objectEnumerable);
            else if (objectEnumerable is IEnumerable)
                ProcessIterator((IEnumerable)objectEnumerable);
            else
                throw new ExecutionException(
                    "Enumerable operand in NEXT instruction must be an associative array or a string.");
        }

        private void ProcessPUSH()
        {
            object objectValue = ResolveOperand(_scriptInstruction.Operand0);
            _stackParameters.Push(objectValue);
        }

        private void ProcessPOP()
        {
            string strIdentifier;
            object objectValue = _stackParameters.Pop();
            Operand operand = _scriptInstruction.Operand0;
            switch (operand.Type)
            {
                case OperandType.Variable:
                    strIdentifier = operand.Value.ToString();
                    _dictionaryLocal[strIdentifier] = objectValue;
                    break;
                case OperandType.LiteralIndexedVariable:
                case OperandType.VariableIndexedVariable:
                    strIdentifier = operand.Value.ToString();
                    object objectArray = _dictionaryLocal[strIdentifier];
                    if (objectArray.GetType() != typeof(AssociativeArray))
                        throw new ExecutionException(
                            "Associative array expected for POP instruction with indexed operand.");
                    AssociativeArray associativeArray = (AssociativeArray)objectArray;

                    if (operand.Type == OperandType.LiteralIndexedVariable)
                        // pop into literal-indexed array
                        associativeArray[operand.IndexLiteral] = objectValue;
                    else
                    {
                        // pop into variable-indexed array
                        object objectIndexValue = _dictionaryLocal[operand.IndexIdentifier];
                        associativeArray[objectIndexValue] = objectValue;
                    }
                    break;
                default:
                    throw new ExecutionException(
                        "Operand type '" + operand.Type + "' not supported by POP instruction.");

            }
        }

        private void ProcessCALL()
        {
            // get function
            ScriptFunction scriptFunction = _scriptInstruction.Operand0.ScriptFunctionRef;
            ScriptInstruction scriptInstructionTarget = scriptFunction.EntryPoint;

            FunctionFrame functionFrame = new FunctionFrame();
            functionFrame.ScriptFunction = scriptFunction;
            functionFrame.VariableDictionary = VariableDictionary.CreateLocalDictionary(
                                                    _scriptExecutable.Dictionary);
            _dictionaryLocal = functionFrame.VariableDictionary;
            functionFrame.NextInstruction = (int) scriptInstructionTarget.Address;

            _stackFunctionFrames.Push(functionFrame);
        }

        private void ProcessRET()
        {
            _stackFunctionFrames.Pop();

            if (_stackFunctionFrames.Count == 0)
            {
                _terminated = true;
                return;
            }

            _dictionaryLocal = _stackFunctionFrames.Peek().VariableDictionary;
        }

        private void ProcessHOST()
        {
            HostFunctionPrototype hostFunctionPrototype = _scriptInstruction.Operand0.HostFunctionRef;

            IHostFunctionHandler hostFunctionHandler = null;
            if (hostFunctionPrototype.Handler == null)
            {
                if (_scriptHandler != null)
                {
                    hostFunctionHandler = _scriptHandler;
                }
            }
            else
            {
                hostFunctionHandler = hostFunctionPrototype.Handler;
            }

            // pop values from stack into list
            List<object> listParameters = new List<object>();
            int cnt = hostFunctionPrototype.ParameterTypes.Count;
            for (int i = 0; i < cnt; i++)
            {
                listParameters.Insert(0, _stackParameters.Pop());
            }

            // verify param list against proto
            hostFunctionPrototype.VerifyParameters(listParameters);

            // delegate call to handler (if set)
            object objectResult = null;
            if (hostFunctionHandler != null)
            {
                objectResult = hostFunctionHandler.OnHostFunctionCall(hostFunctionPrototype.Name, 
                    listParameters, _currentObject, _currentSubject);

                // verify result against proto
                hostFunctionPrototype.VerifyResult(objectResult);
            }

            // push result into stack
            if (objectResult == null)
            {
                objectResult = NullReference.Instance;
            }
            _stackParameters.Push(objectResult);

            if (_interruptOnHostfunctionCall)
            {
                _interruped = true;
            }
        }

        private void ProcessTHRD()
        {
            // get function
            ScriptFunction scriptFunction = _scriptInstruction.Operand0.ScriptFunctionRef;

            // pop values from stack into list
            List<object> listParameters = new List<object>();
            for (int index = 0; index < scriptFunction.ParameterCount; index++)
                listParameters.Insert(0, _stackParameters.Pop());

            // create new sub-context to act as a thread
            ScriptContext scriptContextThread
                = new ScriptContext(scriptFunction, listParameters);

            // assign host function handler from parent
            scriptContextThread.Handler = _scriptHandler;

            // add to thread list
            _listThreads.Add(scriptContextThread);
        }

        private void ProcessSWPGO()
        {
            if (_currentObject == _executionLinkedData.Object)
            {
                _currentObject = _executionLinkedData.Subject;
                _currentSubject = _executionLinkedData.Object;
            }
            else
            {
                _currentObject = _executionLinkedData.Object;
                _currentSubject = _executionLinkedData.Subject;
            }
        }

        private void ProcessMPUSH()
        {
            object objectValue = ResolveOperand(_scriptInstruction.Operand1);
            AssignVariable(_scriptInstruction.Operand0, objectValue);
            objectValue = ResolveOperand(_scriptInstruction.Operand0);
            _stackParameters.Push(objectValue);
        }

        public void Execute()
        {
            List<ScriptInstruction> instructions = _scriptExecutable.InstructionsInternal;
            var instructionsCount = instructions.Count;
            if (instructionsCount == 0)
            {
                return;
            }

            while (!_interruped)
            {
                FunctionFrame functionFrame = _stackFunctionFrames.Peek();
                _scriptInstruction = instructions[functionFrame.NextInstruction++];

                switch (_scriptInstruction.Opcode)
                {
                    case Opcode.EVNT:
                    case Opcode.IEVNT:
                        ProcessEVNT(_scriptInstruction.Opcode == Opcode.IEVNT); 
                        break;
                    case Opcode.DCG: ProcessDCG(); break;
                    case Opcode.DCL: ProcessDCL(); break;
                    case Opcode.INT: ProcessINT(); break;
                    case Opcode.LOCK: ProcessLOCK(); break;
                    case Opcode.ULCK: ProcessULCK(); break;
                    case Opcode.MOV: ProcessMOV(); break;
                    case Opcode.INC: ProcessINC(); break;
                    case Opcode.DEC: ProcessDEC(); break;
                    case Opcode.NEG: ProcessNEG(); break;
                    case Opcode.ADD: ProcessADD(); break;
                    case Opcode.SUB: ProcessSUB(); break;
                    case Opcode.MUL: ProcessMUL(); break;
                    case Opcode.DIV: ProcessDIV(); break;
                    case Opcode.POW: ProcessPOW(); break;
                    case Opcode.MOD: ProcessMOD(); break;
                    case Opcode.CNL: ProcessCNL(); break;
                    case Opcode.CEQ: ProcessCEQ(); break;
                    case Opcode.CNE: ProcessCNE(); break;
                    case Opcode.CG: ProcessCG(); break;
                    case Opcode.CGE: ProcessCGE(); break;
                    case Opcode.CL: ProcessCL(); break;
                    case Opcode.CLE: ProcessCLE(); break;
                    case Opcode.OR: ProcessOR(); break;
                    case Opcode.AND: ProcessAND(); break;
                    case Opcode.NOT: ProcessNOT(); break;
                    case Opcode.JMP: ProcessJMP(); break;
                    case Opcode.JT: ProcessJT(); break;
                    case Opcode.JF: ProcessJF(); break;
                    case Opcode.CLRA: ProcessCLRA(); break;
                    case Opcode.NEXT: ProcessNEXT(); break;
                    case Opcode.PUSH: ProcessPUSH(); break;
                    case Opcode.POP: ProcessPOP(); break;
                    case Opcode.CALL: ProcessCALL(); break;
                    case Opcode.RET: ProcessRET(); break;
                    case Opcode.HOST: ProcessHOST(); break;
                    case Opcode.THRD: ProcessTHRD(); break;
                    case Opcode.SWPGO: ProcessSWPGO(); break;
                    case Opcode.MPUSH: ProcessMPUSH(); break;
                }

                if (functionFrame.NextInstruction + 1 > instructionsCount)
                {
                    return;
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Constructs a script context with the given <see cref="ScriptFunction"/>
        /// entry point and function parameters.
        /// </summary>
        /// <param name="scriptFunction">Script function to execute.</param>
        /// <param name="listParameters">Script function parameters.</param>
        public ScriptContext(ScriptFunction scriptFunction, List<object> listParameters)
        {
            if (scriptFunction.ParameterCount > listParameters.Count)
                throw new ExecutionException("Missing function parameters.");
            if (scriptFunction.ParameterCount < listParameters.Count)
                throw new ExecutionException("Too many function parameters.");

            _scriptFunction = scriptFunction;
            _script = scriptFunction.Executable.Script;
            _scriptExecutable = _script.Executable;
            _stackFunctionFrames = new Stack<FunctionFrame>();
            _stackParameters = new Stack<object>();
            _dictLocks = new Dictionary<object, ScriptInstruction>();
            _listThreads = new List<ScriptContext>();
            _scriptHandler = null;

            _interruptOnHostfunctionCall = false;

            Reset();

            // push any passed parameters
            foreach (object objectParameter in listParameters)
            {
                if (objectParameter == null)
                    _stackParameters.Push(NullReference.Instance);
                else
                {
                    Type typeParameter = objectParameter.GetType();
                    if (typeParameter == typeof(NullReference))
                        _stackParameters.Push(NullReference.Instance);
                    else if (typeParameter == typeof(int)
                        || typeParameter == typeof(float)
                        || typeParameter == typeof(bool)
                        || typeParameter == typeof(string)
                        || typeParameter == typeof(AssociativeArray))
                        _stackParameters.Push(objectParameter);
                    else
                        throw new ExecutionException("Parameters of type '"
                            + typeParameter.Name + "' not allowed.");
                }
            }
        }

        /// <summary>
        /// Constructs a script context with the given <see cref="ScriptFunction"/>
        /// entry point. No parameters are assumed.
        /// </summary>
        /// <param name="scriptFunction">Script function to execute.</param>
        public ScriptContext(ScriptFunction scriptFunction)
            : this(scriptFunction, new List<object>())
        {
        }

        /// <summary>
        /// Constructs a script context for the given <see cref="Script"/> assuming
        /// a main() function with the given parameter values.
        /// </summary>
        /// <param name="script">Script to execute.</param>
        /// <param name="listParameters">Script function parameters.</param>
        public ScriptContext(Script script, List<object> listParameters)
            : this(script.Executable.MainFunction, listParameters)
        {
        }

        internal ScriptContext(Script script, ScriptExecutionLinkedData executionLinkedData) : 
            this(script.Executable.MainFunction, new List<object>())
        {
            _executionLinkedData = executionLinkedData;
            _currentObject = _executionLinkedData.Object;
            _currentSubject = _executionLinkedData.Subject;
        }

        /// <summary>
        /// Resets execution of the script context.
        /// </summary>
        public void Reset()
        {
            _stackFunctionFrames.Clear();
            FunctionFrame functionFrame = new FunctionFrame();
            functionFrame.ScriptFunction = _scriptFunction;
            functionFrame.VariableDictionary = VariableDictionary.CreateLocalDictionary(
                _scriptExecutable.Dictionary);
            functionFrame.NextInstruction = _scriptFunction.EntryPoint != null
                ? (int)_scriptFunction.EntryPoint.Address : 0;
            _stackFunctionFrames.Push(functionFrame);

            _stackParameters.Clear();

            _scriptInstruction = null;
            _dictionaryLocal = functionFrame.VariableDictionary;

            // release all locks held by this context
            foreach (object objectLock in _dictLocks.Keys)
                _script.Manager.Locks.Remove(objectLock);
            _dictLocks.Clear();

            _terminated = false;
            _interruped = false;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// <see cref="Script"/> associated with this context.
        /// </summary>
        public Script Script
        {
            get { return _script; }
        }

        /// <summary>
        /// Boolean flag to enable or disable interrupts whenever
        /// a host function is invoked.
        /// </summary>
        public bool InterruptOnHostfunctionCall
        {
            get { return _interruptOnHostfunctionCall; }
            set { _interruptOnHostfunctionCall = value; }
        }

        /// <summary>
        /// Child thread contexts generated by the script.
        /// </summary>
        public ReadOnlyCollection<ScriptContext> ChildThreads
        {
            get { return _listThreads.AsReadOnly(); }
        }

        /// <summary>
        /// Boolean flag indicating if an interrupt was generated
        /// after the last execution run. The flag is cleared in
        /// subsequent executions.
        /// </summary>
        public bool Interrupted
        {
            get { return _interruped; }
        }

        /// <summary>
        /// Boolean flag indicating if the initially specified
        /// function has completed execution or otherwise.
        /// </summary>
        public bool Terminated
        {
            get { return _terminated; }
        }

        /// <summary>
        /// Index to the next instruction to be executed.
        /// </summary>
        public int NextInstruction
        {
            get
            {
                if (_stackFunctionFrames.Count == 0) return -1;
                return _stackFunctionFrames.Peek().NextInstruction;
            }
        }

        /// <summary>
        /// Current execution function stack.
        /// </summary>
        public ReadOnlyCollection<ScriptFunction> FunctionStack
        {
            get
            {
                List<ScriptFunction> listFunctions = new List<ScriptFunction>();
                foreach (FunctionFrame functionFrame in _stackFunctionFrames)
                    listFunctions.Add(functionFrame.ScriptFunction);
                return new List<ScriptFunction>(listFunctions).AsReadOnly();
            }
        }

        /// <summary>
        /// Current parameter value stack.
        /// </summary>
        public ReadOnlyCollection<object> ParameterStack
        {
            get { return new List<object>(_stackParameters).AsReadOnly(); }
        }

        /// <summary>
        /// Variable dictionary defined at local scope.
        /// </summary>
        public VariableDictionary LocalDictionary
        {
            get { return _dictionaryLocal; }
        }

        /// <summary>
        /// Context-level host function handler. This handler
        /// is ignored by host functions with handlers defined
        /// at <see cref="ScriptManager"/> level.
        /// </summary>
        public IHostFunctionHandler Handler
        {
            get { return _scriptHandler; }
            set { _scriptHandler = value; }
        }

        #endregion
    }
}
