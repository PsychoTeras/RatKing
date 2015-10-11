using System.Threading;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Enums;

namespace GMechanics.Core.GameScript
{
    internal class ScriptExecutionLinkedData
    {
        public Thread CallerThread;
        public InteractiveEventType EventType;
        public GameObjectFeature Recipient;
        public GameObject Object;
        public GameObject Subject;
        public object Member;
        public object Value;
        public object OldValue;
        public bool Cancelled;

        public ScriptExecutionLinkedData NextElement;

        public ScriptExecutionLinkedData Init(InteractiveEventType eventType, 
            GameObjectFeature recipient, GameObject @object, GameObject subject,
            object member, object oldValue, object value)
        {
            EventType = eventType;
            Recipient = recipient;
            Object = @object;
            Subject = subject;
            Member = member;
            OldValue = oldValue;
            Value = value;
            NextElement = null;
            Cancelled = false;
            CallerThread = Thread.CurrentThread;
            return this;
        }

        public new long GetHashCode()
        {
            return Member.GetHashCode() << 32 | Recipient.GetHashCode();
        }
    }
}
