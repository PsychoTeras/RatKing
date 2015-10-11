using System;
using System.Runtime.Serialization;
using GMechanics.Core.Classes.Entities.GameObjectPropertyClasses;
using GMechanics.Core.Classes.Entities.ParentalGameObjectPropertyClasses;
using GMechanics.Core.Classes.Storages.BaseStorages;

namespace GMechanics.Core.Classes.Storages
{
    [Serializable]
    public sealed class GameObjectProperties : BaseContainer<ParentalGameObjectProperty, 
        GameObjectProperties>
    {
        public GameObjectProperties() { }

        public GameObjectProperties(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public bool Add(string name, string transcription, string description, float minValue,
                        GameObjectPropertyClass propertyClass)
        {
            return Add(new ParentalGameObjectProperty(name, transcription, description, minValue,
                                                      propertyClass));
        }
    }
}