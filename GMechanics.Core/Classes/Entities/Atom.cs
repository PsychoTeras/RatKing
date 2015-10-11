using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Interfaces;
using GMechanics.Core.Classes.Pools;
using GMechanics.Core.Classes.Types;
using GMechanics.Core.GameScript;

namespace GMechanics.Core.Classes.Entities
{
    [Serializable, DefaultProperty("Name")]
    public class Atom : MarshalByRefObject, IClassAsAtom, IInteractive
    {

#region Static members

        private static readonly object ObjectIdCounterLock = new object();
        private static long _objectIdCounter = DateTime.UtcNow.ToFileTimeUtc(); //!!! volatile

#endregion

#region Private members

        [NonSerialized]
        private bool _destroyed;

        [NonSerialized]
        private object _interactiveRecipients;

#endregion

#region Properties

        [Category("\t\tCommon settings"), DisplayName("\t\t\t\t\tName")]
        public virtual string Name { get; set; }

        [Category("\t\tCommon settings"), DisplayName("\t\t\t\t\tTranscription")]
        public virtual string Transcription { get; set; }

        [Category("\t\tCommon settings"), DisplayName("\t\t\t\tDescription"),
         Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0," +
                "Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof (UITypeEditor))]
        public virtual string Description { get; set; }

        [XmlIgnore, PreventingEditing]
        [DisplayName("Recipients"), Category("Interactive")]
        public virtual InteractiveRecipientsList InteractiveRecipients
        {
            get { return (InteractiveRecipientsList) _interactiveRecipients; }
            set { _interactiveRecipients = value; }
        }

        [Browsable(false)]
        public object InteractiveRecipientsData
        {
            get
            {
                if (_interactiveRecipients == null || _interactiveRecipients is byte[])
                {
                    return _interactiveRecipients;
                }

                InteractiveRecipientsList recipientsDict = InteractiveRecipients;
                if (recipientsDict.Count == 0)
                {
                    return null;
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryWriter bw = new BinaryWriter(ms);
                    foreach (InteractiveEventType eventType in recipientsDict.Keys)
                    {
                        ushort count;
                        List<GameObjectFeature> list = recipientsDict[eventType];
                        if (list != null && (count = (ushort) list.Count) > 0)
                        {
                            bw.Write((byte) eventType);
                            bw.Write(count);
                            foreach (GameObjectFeature recipient in list)
                            {
                                bw.Write(recipient.Name);
                            }
                        }
                    }
                    return ms.ToArray();
                }
            }
            set { _interactiveRecipients = value; }
        }

        [XmlIgnore, Browsable(false)]
        public virtual string DisplayName
        {
            get { return string.Format("{0} [{1}]", Name, Transcription); }
        }

        [XmlIgnore, Browsable(false)]
        public virtual string ShortDisplayName
        {
            get { return Transcription != "" && Name != Transcription ? DisplayName : Name; }
        }

        [XmlIgnore, Browsable(false)]
        public bool IsDestroyed
        {
            get { return _destroyed; }
        }

        [Browsable(false)]
        public long ObjectId { get; internal set; }

#endregion

#region Static methods

        private static long GetObjectId()
        {
            lock (ObjectIdCounterLock)
            {
                return ++_objectIdCounter;
            }
        }

#endregion

#region Class functions

        protected Atom()
        {
            Initialize();
        }

        private void Initialize()
        {
            ObjectId = GetObjectId();
            Name = Transcription = Description = string.Empty;
        }

        internal void InitializeInteractiveRecipientsList()
        {
            _interactiveRecipients = new InteractiveRecipientsList();
        }

        public virtual Atom Assign(Atom src)
        {
            //Assign all fields
            const BindingFlags flags = BindingFlags.Instance | 
                                       BindingFlags.Public |
                                       BindingFlags.NonPublic;
            FieldInfo[] fields = GetType().GetFields(flags);
            foreach (FieldInfo fieldInfo in fields)
            {
                fieldInfo.SetValue(this, fieldInfo.GetValue(src));
            }

            //Assign all properties
            PropertyInfo[] properties = GetType().GetProperties(flags);
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (propertyInfo.CanRead && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(this, propertyInfo.GetValue(src, null), null);
                }
            }

            return src;
        }

        public virtual object Clone()
        {
            return new Atom().Assign(this);
        }

        public virtual void Destroy()
        {
            _destroyed = true;
        }

        public override string ToString()
        {
            return Name ?? string.Empty;
        }

#endregion

#region IInteractive Members

        public void AddInteractiveRecipient(InteractiveEventType eventType, GameObjectFeature recipient)
        {
            if (_interactiveRecipients == null)
            {
                _interactiveRecipients = new InteractiveRecipientsList(); 
            }
            InteractiveRecipients.Add(eventType, recipient);
        }

        public void RemoveInteractiveRecipient(InteractiveEventType eventType, GameObjectFeature recipient)
        {
            InteractiveRecipientsList recipientsDict = InteractiveRecipients;
            if (recipientsDict != null)
            {
                recipientsDict.Remove(eventType, recipient);
                if (recipientsDict.Count == 0)
                {
                    _interactiveRecipients = null;
                }
            }
        }

        public object QueryInteractiveRecipients(InteractiveEventType eventType, GameObject owner, 
            object member, object oldValue, object value, out bool cancelled)
        {
            cancelled = false;

            InteractiveRecipientsList recipientsDict = InteractiveRecipients;
            if (recipientsDict != null)
            {
                List<GameObjectFeature> list = recipientsDict[eventType];
                if (list != null)
                {
                    ScriptExecutionPool pool = ScriptExecutionPool.Instance;
                    ScriptExecutionLinkedData data = null, previous = null;
                    foreach (GameObjectFeature recipient in list)
                    {
                        data = pool.ScriptExecutionDataPool.Pop(eventType, recipient, owner,
                            null, member, oldValue, value);
                        if (previous != null)
                        {
                            previous.NextElement = data;
                        }
                        previous = data;
                    }
                    return pool.ExecuteSync(data, out cancelled);
                }
            }

            return value;
        }

        public object QueryInteractiveRecipients(InteractiveEventType eventType, GameObject owner, 
            object member, object value, out bool cancelled)
        {
            return QueryInteractiveRecipients(eventType, owner, member, null, value, out cancelled);
        }

        public object QueryInteractiveRecipients(InteractiveEventType eventType, GameObject owner,
            object member, out bool cancelled)
        {
            return QueryInteractiveRecipients(eventType, owner, member, null, null, out cancelled);
        }

        public void NotifyInteractiveRecipients(InteractiveEventType eventType, GameObject owner,
            object member, object value)
        {
            InteractiveRecipientsList recipientsDict = InteractiveRecipients;
            if (recipientsDict != null)
            {
                List<GameObjectFeature> list = recipientsDict[eventType];
                if (list != null)
                {
                    ScriptExecutionPool pool = ScriptExecutionPool.Instance;
                    ScriptExecutionLinkedData data = null, previous = null;
                    foreach (GameObjectFeature recipient in list)
                    {
                        data = pool.ScriptExecutionDataPool.Pop(eventType, recipient, owner,
                            null, member, null, value);
                        if (previous != null)
                        {
                            previous.NextElement = data;
                        }
                        previous = data;
                    }
                    pool.ExecuteAsync(data);
                }
            }
        }

        public void NotifyInteractiveRecipients(InteractiveEventType eventType, GameObject owner, 
            object member)
        {
            NotifyInteractiveRecipients(eventType, owner, member, null);
        }

#endregion

#region IClassAsAtom Members

        [XmlIgnore, Browsable(false)]
        public Atom ClassAsAtom { get { return this; } }

#endregion

#region ShouldSerialize

        private bool ShouldSerializeName()
        {
            return false;
        }

        private bool ShouldSerializeTranscription()
        {
            return false;
        }

        private bool ShouldSerializeDescription()
        {
            return false;
        }

        private bool ShouldSerializeInteractiveRecipients()
        {
            return false;
        }

#endregion

    }
}