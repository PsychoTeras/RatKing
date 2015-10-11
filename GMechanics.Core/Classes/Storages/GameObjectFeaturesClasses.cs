using System;
using System.Runtime.Serialization;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Storages.BaseStorages;

namespace GMechanics.Core.Classes.Storages
{
    [Serializable]
    public sealed class GameObjectFeaturesClasses : BaseContainer<GameObjectFeatureClass, 
        GameObjectFeaturesClasses>
    {
        public GameObjectFeaturesClasses() { }

        public GameObjectFeaturesClasses(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public bool Add(string name, string transcription, string description)
        {
            return Add(new GameObjectFeatureClass(name, transcription, description));
        }
    }
}