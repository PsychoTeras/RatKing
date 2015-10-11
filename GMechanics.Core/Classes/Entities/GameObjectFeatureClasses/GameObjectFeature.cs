using System;
using System.ComponentModel;
using System.Xml.Serialization;
using GMechanics.Core.Classes.Entities.GameObjectEventClasses;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Types;
using GMechanics.Core.GameScript;

namespace GMechanics.Core.Classes.Entities.GameObjectFeatureClasses
{
    [Serializable]
    public sealed class GameObjectFeature : Atom, IComparable<GameObjectFeature>
    {

#region Private members

        [NonSerialized]
        private object _featureClass;

#endregion

#region Properties
        
        [Browsable(false)]
        public string FeatureClassName
        {
            get
            {
                if (_featureClass != null)
                {
                    return (_featureClass is string)
                               ? (string)_featureClass
                               : ((GameObjectFeatureClass)_featureClass).Name;
                }
                return null;
            }
            set
            {
                _featureClass = value;
            }
        }

        [XmlIgnore, Browsable(false)]
        public GameObjectFeatureClass FeatureClass
        {
            get { return (GameObjectFeatureClass)_featureClass; }
            set { _featureClass = value; }
        }

        [DisplayName("Execution priority"), Category("\tFeature settings")]
        public byte Priority { get; set; }

        [PreventingEditing]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DisplayName("Script editor"), Category("Interactive")]
        public GameObjectEvent Event { get; set; }

#endregion

#region Class functions

        public GameObjectFeature() {}
        public GameObjectFeature(string name, string transcription, 
            string description, GameObjectFeatureClass featureClass)
        {
            Name = name;
            Transcription = transcription;
            Description = description;
            FeatureClass = featureClass;
        }

#endregion

#region Interactive functions

        internal object DoInteractive(ScriptExecutionLinkedData data)
        {
            if (Event != null)
            {
                ScriptExecuter executer = null;
                switch (data.EventType)
                {
                    case InteractiveEventType.Assigning:
                        executer = Event.OnAssigningExecuter;
                        break;
                    case InteractiveEventType.Assigned:
                        executer = Event.OnAssignedExecuter;
                        break;
                    case InteractiveEventType.Removing:
                        executer = Event.OnRemovingExecuter;
                        break;
                    case InteractiveEventType.Removed:
                        executer = Event.OnRemovedExecuter;
                        break;
                    case InteractiveEventType.Changing:
                        executer = Event.OnChangingExecuter;
                        break;
                    case InteractiveEventType.Changed:
                        executer = Event.OnChangedExecuter;
                        break;
                    case InteractiveEventType.Interact:
                        executer = Event.OnInteractExecuter;
                        break;
                }
                if (executer != null)
                {
                    executer.Execute(data);
                    return data.Value;
                }
            }
            return null;
        }

#endregion

#region ShouldSerialize

        private bool ShouldSerializeFeatureClass()
        {
            return false;
        }

        private bool ShouldSerializeEvent()
        {
            return false;
        }

        private bool ShouldSerializePriority()
        {
            return false;
        }

#endregion

#region IComparable Members

        public int CompareTo(GameObjectFeature comparer)
        {
            return Priority.CompareTo(comparer.Priority);
        }

#endregion

    }
}