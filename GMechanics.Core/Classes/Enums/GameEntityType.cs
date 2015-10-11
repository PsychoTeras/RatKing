using System;
using System.Collections.Generic;
using GMechanics.Core.Classes.Entities.GameObjectAttributeClasses;
using GMechanics.Core.Classes.Entities.GameObjectEventClasses;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Entities.GameObjectPropertyClasses;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Entities.ParentalGameObjectAttributeClasses;
using GMechanics.Core.Classes.Entities.ParentalGameObjectPropertyClasses;
using GMechanics.Core.Classes.Types;
using GMechanics.Core.GameScript.Runtime;

namespace GMechanics.Core.Classes.Enums
{
    public enum GameEntityType : byte
    {
        //Unknown
        Unknown,

        //Basic types
        AssociativeArray = 1,
        String,
        Float,
        Integer,

        //Game objects
        ElementaryGameObject = 10,
        GameObject,
        GameObjectGroup,

        GameObjectAttribute = 20,
        GameObjectAttributeValue,
        ParentalGameObjectAttribute,
        ParentalGameObjectAttributeValue,

        GameObjectProperty,
        GameObjectPropertyClass,
        ParentalGameObjectProperty,

        GameObjectFeature,
        GameObjectFeatureClass,

        GameObjectEvent,

        //Game classes
        Size3D = 100,
        Location3D
    }

    public static class GameEntityTypesTable
    {
        private static Dictionary<Type, GameEntityType> Types { get; set; }

        public static GameEntityType TypeOf(object obj)
        {
            if (obj == null)
            {
                return GameEntityType.Unknown;
            }
            Type type = obj.GetType();
            return Types.ContainsKey(type) ? Types[type] : GameEntityType.Unknown;
        }

        static GameEntityTypesTable()
        {
            Types = new Dictionary<Type, GameEntityType>
                    {
                        //Basic types
                        {typeof (AssociativeArray), GameEntityType.AssociativeArray},
                        {typeof (string), GameEntityType.String},
                        {typeof (float), GameEntityType.Float},
                        {typeof (int), GameEntityType.Integer},

                        //Game objects
                        {typeof (ElementaryGameObject), GameEntityType.ElementaryGameObject},
                        {typeof (GameObject), GameEntityType.GameObject},
                        {typeof (GameObjectGroup), GameEntityType.GameObjectGroup},

                        {typeof (GameObjectAttribute), GameEntityType.GameObjectAttribute},
                        {typeof (GameObjectAttributeValue), GameEntityType.GameObjectAttributeValue},
                        {typeof (ParentalGameObjectAttribute), GameEntityType.ParentalGameObjectAttribute},
                        {typeof (ParentalGameObjectAttributeValue), GameEntityType.ParentalGameObjectAttributeValue},

                        {typeof (GameObjectProperty), GameEntityType.GameObjectProperty},
                        {typeof (GameObjectPropertyClass), GameEntityType.GameObjectPropertyClass},
                        {typeof (ParentalGameObjectProperty), GameEntityType.ParentalGameObjectProperty},

                        {typeof (GameObjectFeature), GameEntityType.GameObjectFeature},
                        {typeof (GameObjectFeatureClass), GameEntityType.GameObjectFeatureClass},

                        {typeof (GameObjectEvent), GameEntityType.GameObjectEvent},

                        // Game classes
                        {typeof (Size3D), GameEntityType.Size3D},
                        {typeof (Location3D), GameEntityType.Location3D}
                    };
        }
    };
}
