using System.Collections;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.GameScript;

namespace GMechanics.Core.Classes.Pools
{
    public class ScriptExecutionDataPool
    {
        public static ScriptExecutionDataPool Instance = new ScriptExecutionDataPool();

        private const float GrowFactor = 1.5f;
        private readonly Queue _queue;
        private int _queueCapacity = 10;

        private void ExpandQueueCapacity()
        {
            int count = (int) ((_queueCapacity*GrowFactor) - _queueCapacity);
            for (int i = 0; i < count; i++)
            {
                _queue.Enqueue(new ScriptExecutionLinkedData());
            }
            _queueCapacity += count;
        }

        public ScriptExecutionDataPool()
        {
            _queue = Queue.Synchronized(new Queue(_queueCapacity, GrowFactor));
        }

        internal ScriptExecutionLinkedData Pop(InteractiveEventType eventType,
            GameObjectFeature recipient, GameObject @object, GameObject subject,
            object member, object oldValue, object value)
        {
            lock (_queue)
            {
                if (_queue.Count == 0)
                {
                    ExpandQueueCapacity();
                }
                return ((ScriptExecutionLinkedData)_queue.Dequeue()).Init(eventType,
                    recipient, @object, subject, member, oldValue, value);
            }
        }

        internal void Push(ScriptExecutionLinkedData obj)
        {
            lock (_queue)
            {
                _queue.Enqueue(obj);
            }            
        }
    }
}
