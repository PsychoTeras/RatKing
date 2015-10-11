using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GMechanics.Core.Classes.Entities.GameObjectFeatureClasses
{
    [Serializable]
    public sealed class GameObjectFeatureClass : Atom
    {

#region Private members

        [NonSerialized]
        private object _parentFeatureClass;

#endregion

#region Properties

        [Browsable(false)]
        public string ParentFeatureClassName
        {
            get
            {
                if (_parentFeatureClass != null)
                {
                    return (_parentFeatureClass is string)
                               ? (string)_parentFeatureClass
                               : ((GameObjectFeatureClass)_parentFeatureClass).Name;
                }
                return null;
            }
            set
            {
                _parentFeatureClass = value;
            }
        }

        [XmlIgnore, Browsable(false)]
        public GameObjectFeatureClass ParentFeatureClass
        {
            get { return (GameObjectFeatureClass)_parentFeatureClass; }
            set { _parentFeatureClass = value; }
        }

        [Browsable(false)]
        public override InteractiveRecipientsList InteractiveRecipients { get; set; }

#endregion

#region Class members

        public GameObjectFeatureClass() { }

        public GameObjectFeatureClass(string name, string transcription, 
            string description)
        {
            Name = name;
            Transcription = transcription;
            Description = description;
        }

#endregion

    }
}