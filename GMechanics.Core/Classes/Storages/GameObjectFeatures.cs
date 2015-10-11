using System;
using System.Runtime.Serialization;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Storages.BaseStorages;

namespace GMechanics.Core.Classes.Storages
{
    [Serializable]
    public sealed class GameObjectFeatures : BaseContainer<GameObjectFeature, GameObjectFeatures>
    {
        public GameObjectFeatures() { }

        public GameObjectFeatures(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public bool Add(string name, string transcription, string description,
                        GameObjectFeatureClass featureClass)
        {
            return Add(new GameObjectFeature(name, transcription, description, featureClass));
        }
    }
}