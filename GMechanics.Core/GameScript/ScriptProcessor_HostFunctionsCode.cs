using System;
using System.Collections.Generic;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.GameObjectAttributeClasses;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Entities.ParentalGameObjectAttributeClasses;
using GMechanics.Core.Classes.Storages;

namespace GMechanics.Core.GameScript
{
    // ReSharper disable InconsistentNaming
    public partial class ScriptProcessor
    {
        private static readonly GlobalStorage GlobalContainer = GlobalStorage.Instance;
        private static readonly Random _random = new Random();

#region Excluded objects

        private static object HostFunctionCall_ExcludeObject(
            GameObject @object, GameObject subject, List<object> param)
        {
            Atom atom = (Atom)param[0];
            return @object.ExcludeObject(atom);
        }

        private static object HostFunctionCall_IncludeObject(
            GameObject @object, GameObject subject, List<object> param)
        {
            Atom atom = (Atom)param[0];
            return @object.IncludeObject(atom);
        }

#endregion

#region Attributes
        
        private static object HostFunctionCall_RemoveAttribute(
            GameObject @object, GameObject subject, List<object> param)
        {
            string attributeName = (string)param[0];
            return @object.RemoveAttribute(attributeName, false);
        }

        private static object HostFunctionCall_AddAttribute(
            GameObject @object, GameObject subject, List<object> param)
        {
            string attributeName = (string)param[0];
            ParentalGameObjectAttribute pgoa = GlobalContainer.GameObjectAttributes[attributeName];
            if (pgoa != null)
            {
                GameObjectAttribute goa = new GameObjectAttribute(pgoa, null);
                return @object.AddAttribute(goa);
            }
            return false;
        }

        private static object HostFunctionCall_GetAllAttributes(
            GameObject @object, GameObject subject, List<object> param)
        {
            return @object.GetAllAttributes(true);
        }

        private static object HostFunctionCall_GetAttribute(
            GameObject @object, GameObject subject, List<object> param)
        {
            string attributeName = (string)param[0];
            bool findInParent = (bool)param[1];
            return @object.GetAttribute(attributeName, findInParent);
        }

        private static object HostFunctionCall_GetAttributeByIndex(
            GameObject @object, GameObject subject, List<object> param)
        {
            int index = (int)param[0];
            return @object.GetAttribute(index);
        }

        private static object HostFunctionCall_GetAttributesCount(
            GameObject @object, GameObject subject, List<object> param)
        {
            bool findInParent = (bool)param[0];
            return @object.GetAttributesCount(findInParent);
        }

        private static object HostFunctionCall_GetAttributeOwner(
            GameObject @object, GameObject subject, List<object> param)
        {
            string attributeName = (string) param[0];
            return @object.GetAttributeOwner(attributeName);
        }

        private static object HostFunctionCall_AttributeExists(
            GameObject @object, GameObject subject, List<object> param)
        {
            string attributeName = (string) param[0];
            bool findInParent = (bool)param[1];
            return @object.AttributeExists(attributeName, findInParent);
        }

#endregion

#region Attribute values
        
        private static object HostFunctionCall_AttributeValueExists(
            GameObject @object, GameObject subject, List<object> param)
        {
            string attributeName = (string) param[0];
            string attributeValueName = (string)param[1];
            bool findInParent = (bool)param[2];
            return @object.AttributeValueExists(attributeName, attributeValueName, findInParent);
        }

        private static object HostFunctionCall_GetAttributeValueOwner(
            GameObject @object, GameObject subject, List<object> param)
        {
            string attributeName = (string) param[0];
            string attributeValueName = (string)param[1];
            return @object.GetAttributeValueOwner(attributeName, attributeValueName);
        }

        private static object HostFunctionCall_GetAttributeValue(
            GameObject @object, GameObject subject, List<object> param)
        {
            string attributeName = (string)param[0];
            string attributeValueName = (string)param[1];
            bool findInParent = (bool)param[2];
            return @object.GetAttributeValue(attributeName, attributeValueName, findInParent);
        }

        private static object HostFunctionCall_GetAttributeValueByIndex(
            GameObject @object, GameObject subject, List<object> param)
        {
            string attributeName = (string)param[0];
            int index = (int)param[1];
            return @object.GetAttributeValue(attributeName, index);
        }

        private static object HostFunctionCall_GetAttributeValuesCount(
            GameObject @object, GameObject subject, List<object> param)
        {
            string attributeName = (string)param[0];
            bool findInParent = (bool)param[1];
            return @object.GetAttributeValuesCount(attributeName, findInParent);
        }

        private static object HostFunctionCall_AddAttributeValue(
            GameObject @object, GameObject subject, List<object> param)
        {
            string attributeName = (string)param[0];
            string attributeValueName = (string)param[1];
            return @object.AddAttributeValue(attributeName, attributeValueName);
        }

        private static object HostFunctionCall_RemoveAttributeValue(
            GameObject @object, GameObject subject, List<object> param)
        {
            string attributeName = (string)param[0];
            string attributeValueName = (string)param[1];
            return @object.RemoveAttributeValue(attributeName, attributeValueName, false);
        }
        
#endregion

#region Properties

        private static object HostFunctionCall_GetPropertyClass(
            GameObject @object, GameObject subject, List<object> param)
        {
            string propertyName = (string)param[0];
            return @object.GetPropertyClass(propertyName);
        }

        private static object HostFunctionCall_RemoveProperty(
            GameObject @object, GameObject subject, List<object> param)
        {
            string propertyName = (string)param[0];
            return @object.RemoveProperty(propertyName, false);
        }

        private static object HostFunctionCall_AddProperty(
            GameObject @object, GameObject subject, List<object> param)
        {
            string propertyName = (string)param[0];
            return @object.AddProperty(propertyName);
        }

        private static object HostFunctionCall_GetAllProperties(
            GameObject @object, GameObject subject, List<object> param)
        {
            bool findInParent = (bool)param[0];
            return @object.GetAllProperties(findInParent);
        }

        private static object HostFunctionCall_GetProperty(
            GameObject @object, GameObject subject, List<object> param)
        {
            string propertyName = (string)param[0];
            bool findInParent = (bool)param[1];
            return @object.GetProperty(propertyName, findInParent);
        }

        private static object HostFunctionCall_GetPropertyByIndex(
            GameObject @object, GameObject subject, List<object> param)
        {
            int index = (int)param[0];
            return @object.GetProperty(index);
        }

        private static object HostFunctionCall_GetPropertiesCount(
            GameObject @object, GameObject subject, List<object> param)
        {
            bool findInParent = (bool)param[0];
            return @object.GetPropertiesCount(findInParent);
        }

        private static object HostFunctionCall_GetPropertyOwner(
            GameObject @object, GameObject subject, List<object> param)
        {
            string propertyName = (string)param[0];
            return @object.GetPropertyOwner(propertyName);
        }

        private static object HostFunctionCall_PropertyExists(
            GameObject @object, GameObject subject, List<object> param)
        {
            string propertyName = (string)param[0];
            bool findInParent = (bool)param[1];
            return @object.PropertyExists(propertyName, findInParent);
        }

#endregion

#region Features

        private static object HostFunctionCall_GetFeatureClass(
            GameObject @object, GameObject subject, List<object> param)
        {
            string featureName = (string)param[0];
            return @object.GetFeatureClass(featureName);
        }

        private static object HostFunctionCall_RemoveFeature(
            GameObject @object, GameObject subject, List<object> param)
        {
            string featureName = (string)param[0];
            return @object.RemoveFeature(featureName, false);
        }

        private static object HostFunctionCall_AddFeature(
            GameObject @object, GameObject subject, List<object> param)
        {
            string featureName = (string)param[0];
            GameObjectFeature gof = GlobalContainer.GameObjectFeatures[featureName];
            if (gof != null)
            {
                return @object.AddFeature(gof);
            }
            return false;
        }

        private static object HostFunctionCall_GetAllFeatures(
            GameObject @object, GameObject subject, List<object> param)
        {
            bool findInParent = (bool)param[0];
            return @object.GetAllFeatures(findInParent);
        }

        private static object HostFunctionCall_GetFeature(
            GameObject @object, GameObject subject, List<object> param)
        {
            string featureName = (string)param[0];
            bool findInParent = (bool)param[1];
            return @object.GetFeature(featureName, findInParent);
        }

        private static object HostFunctionCall_GetFeatureByIndex(
            GameObject @object, GameObject subject, List<object> param)
        {
            int index = (int)param[0];
            return @object.GetFeature(index);
        }

        private static object HostFunctionCall_GetFeaturesCount(
            GameObject @object, GameObject subject, List<object> param)
        {
            bool findInParent = (bool)param[0];
            return @object.GetFeaturesCount(findInParent);
        }

        private static object HostFunctionCall_GetFeatureOwner(
            GameObject @object, GameObject subject, List<object> param)
        {
            string featureName = (string)param[0];
            return @object.GetFeatureOwner(featureName);
        }

        private static object HostFunctionCall_FeatureExists(
            GameObject @object, GameObject subject, List<object> param)
        {
            string featureName = (string)param[0];
            bool findInParent = (bool)param[1];
            return @object.FeatureExists(featureName, findInParent);
        }

#endregion

#region Common functions

        private static object Common_Event(Event onEvent, List<object> param, 
                                           bool interuption)
        {
            if (onEvent != null)
            {
                onEvent(param, interuption);
            }
            return null;
        }

        private static object Common_Log(LogEvent onLogEvent, List<object> param)
        {
            if (onLogEvent != null)
            {
                onLogEvent(param[0].ToString());
            }
            return null;
        }

        private static object Common_Random(List<object> param)
        {
            int minValue = (int) param[0];
            int maxValue = (int) param[1];
            return _random.Next(minValue, maxValue);
        }

        private static object Common_RandomF()
        {
            return (float)_random.NextDouble();
        }  

#endregion

#region Host functions

        private object OnCommonFunctionCall(string strFunctionName, List<object> param)
        {
            switch (strFunctionName)
            {
                case "Log":
                    return Common_Log(OnLogEvent, param);
                case "Random":
                    return Common_Random(param);
                case "RandomF":
                    return Common_RandomF();
                case "Event":
                    return Common_Event(OnEvent, param, false);
                case "Interrupt":
                    return Common_Event(OnEvent, param, true);
            }
            return null;
        }

        public object OnHostFunctionCall(string functionName, List<object> param,
                                         object userData1, object userData2)
        {
            // Call game objects function
            if (HostFunctions.ContainsKey(functionName))
            {
                EHostFunctionCall function = HostFunctions[functionName];
                return function((GameObject)userData1, (GameObject)userData2, param);
            }

            // Or call one of the common functions
            return OnCommonFunctionCall(functionName, param);
        }

#endregion

    }
    // ReSharper restore InconsistentNaming
}
