using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Enums;

namespace GMechanics.Core.Classes.Interfaces
{
    public interface IInteractive
    {
        //This functions will fire by feature
        void AddInteractiveRecipient(InteractiveEventType eventType, GameObjectFeature recipient);
        void RemoveInteractiveRecipient(InteractiveEventType eventType, GameObjectFeature recipient);

        //This functions will be called by different members of a game object

        // - query
        object QueryInteractiveRecipients(InteractiveEventType eventType, GameObject owner,
            object member, object oldValue, object value, out bool cancelled);
        object QueryInteractiveRecipients(InteractiveEventType eventType, GameObject owner,
            object member, object value, out bool cancelled);
        object QueryInteractiveRecipients(InteractiveEventType eventType, GameObject owner,
            object member, out bool cancelled);

        // - notify
        void NotifyInteractiveRecipients(InteractiveEventType eventType, GameObject owner,
            object member, object value);
        void NotifyInteractiveRecipients(InteractiveEventType eventType, GameObject owner,
            object member);
    }
}
