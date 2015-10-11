using System;
using System.Runtime.Serialization;
using GMechanics.Core.Classes.Entities.ParentalGameObjectAttributeClasses;
using GMechanics.Core.Classes.Storages.BaseStorages;

namespace GMechanics.Core.Classes.Storages
{
    [Serializable]
    public sealed class GameObjectAttributes : BaseContainer<ParentalGameObjectAttribute, 
        GameObjectAttributes>
    {
        public GameObjectAttributes() { }

        public GameObjectAttributes(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public bool Add(string name, string transcription, string description,
                        ParentalGameObjectAttributeValuesList values)
        {
            return Add(new ParentalGameObjectAttribute(name, transcription, 
                description, values));
        }
    }
}