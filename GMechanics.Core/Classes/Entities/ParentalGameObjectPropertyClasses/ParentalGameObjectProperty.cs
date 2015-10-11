using System;
using System.ComponentModel;
using System.Xml.Serialization;
using GMechanics.Core.Classes.Entities.GameObjectPropertyClasses;

namespace GMechanics.Core.Classes.Entities.ParentalGameObjectPropertyClasses
{
    [Serializable]
    public sealed class ParentalGameObjectProperty : Atom
    {

#region Private members

        [NonSerialized]
        private object _propertyClass;

#endregion

#region Properties

        [DisplayName("\tMinimal value"), Category("\tProperty settings")]
        public float MinValue { get; set; }

        [Browsable(false)]
        public string PropertyClassName
        {
            get
            {
                if (_propertyClass != null)
                {
                    return (_propertyClass is string)
                               ? (string) _propertyClass
                               : ((GameObjectPropertyClass) _propertyClass).Name;
                }
                return null;
            }
            set
            {
                _propertyClass = value;
            }
        }

        [XmlIgnore, Browsable(false)]
        public GameObjectPropertyClass PropertyClass
        {
            get { return (GameObjectPropertyClass) _propertyClass; }
            set { _propertyClass = value; }
        }

#endregion

#region Class functions

        public ParentalGameObjectProperty() { }

        public ParentalGameObjectProperty(string name, 
            string transcription,  string description, 
            float minValue, GameObjectPropertyClass propertyClass)
        {
            Name = name;
            Transcription = transcription;
            Description = description;
            PropertyClass = propertyClass;
            MinValue = minValue;
        }

#endregion

#region ShouldSerialize

        private bool ShouldSerializeMinValue()
        {
            return false;
        }

#endregion

    }
}