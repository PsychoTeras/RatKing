using System;
using System.Collections.Generic;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.GameObjectAttributeClasses;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Entities.GameObjectPropertyClasses;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Entities.ParentalGameObjectAttributeClasses;
using GMechanics.Core.GameScript.Compiler;

namespace GMechanics.Core.GameScript
{
    public partial class ScriptProcessor
    {
        private delegate object EHostFunctionCall(GameObject @object, GameObject subject, 
                                                  List<object> param);

        // Host functions types list
        private static readonly Type[] HostFunctionsTypes =
                                                 {
#region Excluded objects
             typeof(bool)
            ,typeof(bool)
#endregion

#region Attributes
            ,typeof(bool)
            ,typeof(GameObject)
            ,typeof(GameObjectAttribute)
            ,typeof(GameObjectAttribute)
            ,typeof(int)
            ,typeof(List<GameObjectAttribute>)
            ,typeof(bool)
            ,typeof(bool)
#endregion

#region Attribute values
            ,typeof(bool)
            ,typeof(GameObject)
            ,typeof(ParentalGameObjectAttributeValue)
            ,typeof(ParentalGameObjectAttributeValue)
            ,typeof(int)
            ,typeof(ParentalGameObjectAttributeValue)
            ,typeof(bool)
#endregion

#region Properties
            ,typeof(bool)
            ,typeof(GameObject)
            ,typeof(GameObjectProperty)
            ,typeof(GameObjectProperty)
            ,typeof(int)
            ,typeof(List<GameObjectProperty>)
            ,typeof(bool)
            ,typeof(bool)
            ,typeof(GameObjectPropertyClass)
#endregion

#region Features
            ,typeof(bool)
            ,typeof(GameObject)
            ,typeof(GameObjectFeature)
            ,typeof(GameObjectFeature)
            ,typeof(int)
            ,typeof(List<GameObjectFeature>)
            ,typeof(bool)
            ,typeof(bool)
            ,typeof(GameObjectFeatureClass)
#endregion
                                                 };

        // Host functions input parameters types list
        private static readonly List<Type>[] HostFunctionsParametersTypes =
                                                                 {

#region Excluded objects
             new List<Type> {typeof(Atom)}
            ,new List<Type> {typeof(Atom)}
#endregion

#region Attributes
            ,new List<Type> {typeof(string), typeof(bool)}
            ,new List<Type> {typeof(string)}
            ,new List<Type> {typeof(string), typeof(bool)}
            ,new List<Type> {typeof(int)}
            ,new List<Type> {typeof(bool)}
            ,new List<Type>()
            ,new List<Type> {typeof(string)}
            ,new List<Type> {typeof(string)}
#endregion

#region Attribute values
            ,new List<Type> {typeof(string), typeof(string), typeof(bool)}
            ,new List<Type> {typeof(string), typeof(string)}
            ,new List<Type> {typeof(string), typeof(string), typeof(bool)}
            ,new List<Type> {typeof(string), typeof(int)}
            ,new List<Type> {typeof(string), typeof(bool)}
            ,new List<Type> {typeof(string), typeof(string)}
            ,new List<Type> {typeof(string), typeof(string)}
#endregion

#region Properties
            ,new List<Type> {typeof(string), typeof(bool)}
            ,new List<Type> {typeof(string)}
            ,new List<Type> {typeof(string), typeof(bool)}
            ,new List<Type> {typeof(int)}
            ,new List<Type> {typeof(bool)}
            ,new List<Type> {typeof(bool)}
            ,new List<Type> {typeof(string)}
            ,new List<Type> {typeof(string)}
            ,new List<Type> {typeof(string)}
#endregion

#region Features
            ,new List<Type> {typeof(string), typeof(bool)}
            ,new List<Type> {typeof(string)}
            ,new List<Type> {typeof(string), typeof(bool)}
            ,new List<Type> {typeof(int)}
            ,new List<Type> {typeof(bool)}
            ,new List<Type> {typeof(bool)}
            ,new List<Type> {typeof(string)}
            ,new List<Type> {typeof(string)}
            ,new List<Type> {typeof(string)}
#endregion
                                                                 };

        // Host functions call delegates
        private static readonly Dictionary<string, EHostFunctionCall> HostFunctions = 
            new Dictionary<string, EHostFunctionCall>
                                                                 {
#region Excluded objects
             {"ExcludeObject", HostFunctionCall_ExcludeObject}
            ,{"IncludeObject", HostFunctionCall_IncludeObject}
#endregion

#region Attributes
            ,{"AttributeExists", HostFunctionCall_AttributeExists}
            ,{"GetAttributeOwner", HostFunctionCall_GetAttributeOwner}
            ,{"GetAttribute", HostFunctionCall_GetAttribute}
            ,{"GetAttributeByIndex", HostFunctionCall_GetAttributeByIndex}
            ,{"GetAttributesCount", HostFunctionCall_GetAttributesCount}
            ,{"GetAllAttributes", HostFunctionCall_GetAllAttributes}
            ,{"AddAttribute", HostFunctionCall_AddAttribute}
            ,{"RemoveAttribute", HostFunctionCall_RemoveAttribute}
#endregion

#region Attribute values
            ,{"AttributeValueExists", HostFunctionCall_AttributeValueExists}
            ,{"GetAttributeValueOwner", HostFunctionCall_GetAttributeValueOwner}
            ,{"GetAttributeValue", HostFunctionCall_GetAttributeValue}
            ,{"GetAttributeValueByIndex", HostFunctionCall_GetAttributeValueByIndex}
            ,{"GetAttributeValuesCount", HostFunctionCall_GetAttributeValuesCount}
            ,{"AddAttributeValue", HostFunctionCall_AddAttributeValue}
            ,{"RemoveAttributeValue", HostFunctionCall_RemoveAttributeValue}
#endregion

#region Properties
            ,{"PropertyExists", HostFunctionCall_PropertyExists}
            ,{"GetPropertyOwner", HostFunctionCall_GetPropertyOwner}
            ,{"GetProperty", HostFunctionCall_GetProperty}
            ,{"GetPropertyByIndex", HostFunctionCall_GetPropertyByIndex}
            ,{"GetPropertiesCount", HostFunctionCall_GetPropertiesCount}
            ,{"GetAllProperties", HostFunctionCall_GetAllProperties}
            ,{"AddProperty", HostFunctionCall_AddProperty}
            ,{"RemoveProperty", HostFunctionCall_RemoveProperty}
            ,{"GetPropertyClass", HostFunctionCall_GetPropertyClass}
#endregion

#region Features
            ,{"FeatureExists", HostFunctionCall_FeatureExists}
            ,{"GetFeatureOwner", HostFunctionCall_GetFeatureOwner}
            ,{"GetFeature", HostFunctionCall_GetFeature}
            ,{"GetFeatureByIndex", HostFunctionCall_GetFeatureByIndex}
            ,{"GetFeaturesCount", HostFunctionCall_GetFeaturesCount}
            ,{"GetAllFeatures", HostFunctionCall_GetAllFeatures}
            ,{"AddFeature", HostFunctionCall_AddFeature}
            ,{"RemoveFeature", HostFunctionCall_RemoveFeature}
            ,{"GetFeatureClass", HostFunctionCall_GetFeatureClass}
#endregion
                                                                 };

        private static void RegisterCommonFunctions(ScriptManager scriptManager, 
            ScriptProcessor scriptProcessor)
        {
            // Log function
            HostFunctionPrototype function = new HostFunctionPrototype(null, "Log", (Type) null);
            scriptManager.RegisterHostFunction(function, scriptProcessor);

            // Random generator function (int)
            function = new HostFunctionPrototype(typeof(int), "Random", new List<Type> { typeof(int), typeof(int) });
            scriptManager.RegisterHostFunction(function, scriptProcessor);

            // Random generator function (float)
            function = new HostFunctionPrototype(typeof(float), "RandomF");
            scriptManager.RegisterHostFunction(function, scriptProcessor);

            // Event
            function = new HostFunctionPrototype(null, "Event", (Type)null);
            scriptManager.RegisterHostFunction(function, scriptProcessor);

            // Interruption event
            function = new HostFunctionPrototype(null, "Interrupt", (Type)null);
            scriptManager.RegisterHostFunction(function, scriptProcessor);
        }

        // Register script host functions
        internal static void RegisterScriptHostFunctions(ScriptManager scriptManager,
            ScriptProcessor scriptProcessor)
        {
            // Register common functions
            RegisterCommonFunctions(scriptManager, scriptProcessor);

            // Register game objects functions
            int idx = 0;
            foreach (string functionName in HostFunctions.Keys)
            {
                HostFunctionPrototype hfp = new HostFunctionPrototype(
                    HostFunctionsTypes[idx], functionName,
                    HostFunctionsParametersTypes[idx]);
                scriptManager.RegisterHostFunction(hfp, scriptProcessor);
                idx++;
            }
        }
    }
}
