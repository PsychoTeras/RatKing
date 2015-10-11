using System;
using System.Runtime.Serialization;
using GMechanics.Core.Classes.Entities.GameObjectPropertyClasses;
using GMechanics.Core.Classes.Storages.BaseStorages;

namespace GMechanics.Core.Classes.Storages
{
    [Serializable]
    public sealed class GameObjectPropertiesClasses : BaseContainer<GameObjectPropertyClass, 
        GameObjectPropertiesClasses>
    {
        public GameObjectPropertiesClasses() { }

        public GameObjectPropertiesClasses(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public bool Add(string name, string transcription, string description)
        {
            return Add(new GameObjectPropertyClass(name, transcription, description));
        }
    }
}