using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GMechanics.Core.Classes.Entities.GameObjectPropertyClasses
{
    [Serializable]
    public sealed class GameObjectPropertyClass : Atom
    {

#region Private members

        [NonSerialized]
        private object _parentPropertyClass;

#endregion

#region Properties

        [Browsable(false)]
        public string ParentPropertyClassName
        {
            get
            {
                if (_parentPropertyClass != null)
                {
                    return (_parentPropertyClass is string)
                               ? (string)_parentPropertyClass
                               : ((GameObjectPropertyClass)_parentPropertyClass).Name;
                }
                return null;
            }
            set
            {
                _parentPropertyClass = value;
            }
        }

        [XmlIgnore, Browsable(false)]
        public GameObjectPropertyClass ParentPropertyClass
        {
            get { return (GameObjectPropertyClass)_parentPropertyClass; }
            set { _parentPropertyClass = value; }
        }

        [Browsable(false)]
        public override InteractiveRecipientsList InteractiveRecipients { get; set; }

#endregion

#region Class functions

        public GameObjectPropertyClass() { }

        public GameObjectPropertyClass(string name, string transcription, 
            string description)
        {
            Name = name;
            Transcription = transcription;
            Description = description;
        }

#endregion

    }
}