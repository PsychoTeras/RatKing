using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.GameObjectAttributeClasses;
using GMechanics.Core.Classes.Interfaces;
using GMechanics.Core.GameScript.Classes;
using GMechanics.Core.GameScript.Runtime;

namespace GMechanics.Core.GameScript.Compiler
{
    public class HostFunctionPrototype
    {

#region Private variables

        private readonly string _name;
        private readonly Type _typeResult;
        private readonly List<Type> _parameterTypes;

#endregion

#region Private methods

        private void ValidateType(Type type)
        {
            if (type == null) return;

            if (type != typeof(int) && 
                type != typeof(float) && 
                type != typeof(bool) &&
                type != typeof(string) && 
                type != typeof(AssociativeArray) &&
                type != typeof(GameObjectAttribute) && 
                !typeof(IList).IsAssignableFrom(type) &&
                !typeof(IClassAsAtom).IsAssignableFrom(type))
            {
                throw new ExecutionException("Type '" + type.Name + "' not allowed in host function prototypes.");
            }
        }

        private string ToString(Type type)
        {
            if (type == null) return string.Empty;
            if (type == typeof(int)) return "int";
            if (type == typeof(float)) return "float";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(string)) return "string";
            if (type == typeof(AssociativeArray)) return "array";
            if (typeof(IClassAsAtom).IsAssignableFrom(type)) return "atom";
            
            throw new GameScriptException("Type '" + type.Name + 
                "' is not allowed in host function prototypes.");
        }

#endregion

#region Public methods

        public HostFunctionPrototype(Type typeResult, string name, List<Type> parameterTypes)
        {
            ValidateType(typeResult);
            foreach (Type typeParameter in parameterTypes)
            {
                ValidateType(typeParameter);
            }

            _name = name;
            _typeResult = typeResult;
            _parameterTypes = parameterTypes;

            Handler = null;
        }

        public HostFunctionPrototype(string strName) : 
            this(null, strName, new List<Type>())
        {
        }

        public HostFunctionPrototype(Type typeResult, string name) : 
            this(typeResult, name, new List<Type>())
        {
        }
        
        public HostFunctionPrototype(Type typeResult, string name, Type typeParameter)
            : this(typeResult, name, new List<Type>())
        {
            _parameterTypes.Add(typeParameter);
        }

        public HostFunctionPrototype(Type typeResult, string name, Type typeParameter0, 
            Type typeParameter1) : this(typeResult, name, new List<Type>())
        {
            _parameterTypes.Add(typeParameter0);
            _parameterTypes.Add(typeParameter1);
        }

        public HostFunctionPrototype(Type typeResult, string name, Type typeParameter0,
            Type typeParameter1, Type typeParameter2) : this(typeResult, name, new List<Type>())
        {
            _parameterTypes.Add(typeParameter0);
            _parameterTypes.Add(typeParameter1);
            _parameterTypes.Add(typeParameter2);
        }

        public void VerifyParameters(List<object> listParameters)
        {
            if (listParameters.Count != _parameterTypes.Count)
            {
                throw new ExecutionException("Host function parameter count mismatch.");
            }

            for (int index = 0; index < listParameters.Count; index++)
            {
                // ignore untyped parameter
                if (_parameterTypes[index] == null) continue;

                // ignore null parameter value
                if (listParameters[index] == null) continue;

                Type typeExpected = _parameterTypes[index];
                Type typeSpecified = listParameters[index].GetType();
                if (typeExpected != typeSpecified && typeExpected != typeof(Atom))
                    throw new ExecutionException (
                        "Parameter of type '" + typeSpecified.Name 
                        + "' specified instead of type '"
                        + typeExpected.Name + "' in host function '"
                        + _name+"'.");
            }
        }

        /// <summary>
        /// Verifies the given result object against the return type
        /// defined by the prototype. An exception is thrown if the
        /// return object fails verification.
        /// </summary>
        /// <param name="objectResult"></param>
        public void VerifyResult(object objectResult)
        {
            if (_typeResult == null || objectResult == null) return;

            if (objectResult.GetType() != _typeResult)
                throw new ExecutionException(
                    "Result of type '" + objectResult.GetType().Name 
                    + "' returned instead of type '"
                    + _typeResult.Name + "' from host function '"
                    + _name+"'.");
        }

        /// <summary>
        /// Returns a string representation of the host function
        /// prototype.
        /// </summary>
        /// <returns>A string representation of the host function
        /// prototype.</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(ToString(_typeResult));
            stringBuilder.Append(" host.");
            stringBuilder.Append(_name);
            stringBuilder.Append("(");
            for (int index = 0; index < _parameterTypes.Count; index++)
            {
                if (index > 0) stringBuilder.Append(", ");
                stringBuilder.Append(ToString(_parameterTypes[index]));
            }
            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Host function name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Parameter type list.
        /// </summary>
        public List<Type> ParameterTypes
        {
            get { return _parameterTypes; }
        }

        /// <summary>
        /// Function result type.
        /// </summary>
        public Type Result
        {
            get { return _typeResult; }
        }

        /// <summary>
        /// Host function handler. This property is set only when handlers
        /// are bound at <see cref="ScriptManager"/> level.
        /// </summary>
        public IHostFunctionHandler Handler { get; internal set; }

        #endregion
    }
}
