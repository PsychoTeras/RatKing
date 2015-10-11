using System;
using System.ComponentModel;
using System.Xml.Serialization;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Entities.ParentalGameObjectPropertyClasses;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Interfaces;

namespace GMechanics.Core.Classes.Entities.GameObjectPropertyClasses
{
    [Serializable]
    public sealed class GameObjectProperty : IClassAsAtom
    {

#region Private members

        [NonSerialized]
        private float _maxValue;

        [NonSerialized]
        private float _value;

        [NonSerialized]
        private readonly GameObject _owner;

#endregion

#region Properties

        [XmlIgnore, Browsable(false)]
        public ParentalGameObjectProperty Parental { get; set; }

        [XmlIgnore]
        public string Name { get { return Parental.Name; } }

        [XmlIgnore]
        public string Transcription { get { return Parental.Transcription; } }

        [XmlIgnore]
        public string Description { get { return Parental.Description; } }

        [XmlIgnore]
        [DisplayName("Minimal value"), Category("Property settings")]
        public float MinValue
        {
            get { return Parental.MinValue; }
        }

        [DisplayName("Maximal value"), Category("Property settings")]
        public float MaxValue
        {
            get { return _maxValue; }
            set
            {
                bool cancelled = false;
                float maxValue = Parental != null
                        ? (float)Parental.QueryInteractiveRecipients(
                            InteractiveEventType.Changing, _owner, this, 
                            _maxValue, value, out cancelled)
                        : value;
                if (!cancelled)
                {
                    _maxValue = maxValue;
                    if (Parental != null)
                    {
                        Parental.NotifyInteractiveRecipients(InteractiveEventType.Changed,
                            _owner, this, _maxValue);
                    }
                }
            }
        }

        [Category("Property settings")]
        public float Value
        {
            get { return _value; }
            set
            {
                bool cancelled = false;
                float curValue = Parental != null 
                        ? (float)Parental.QueryInteractiveRecipients(
                            InteractiveEventType.Changing, _owner, this, 
                            _value, value, out cancelled)
                        : value;
                if (!cancelled)
                {
                    _value = Parental == null
                        ? curValue
                        : curValue < MinValue 
                            ? MinValue 
                            : curValue > MaxValue 
                                ? MaxValue 
                                : curValue;
                    if (Parental != null)
                    {
                        Parental.NotifyInteractiveRecipients(InteractiveEventType.Changed, 
                            _owner, this, _value);
                    }
                }
            }
        }

#endregion

#region Class functions

        public GameObjectProperty() { }

        public GameObjectProperty(ParentalGameObjectProperty parental, 
            GameObject owner)
        {
            _owner = owner;
            Parental = parental;
            _value = parental != null ? MinValue : 0.0f;
            _maxValue = Math.Max(_value, 100);
        }

        public GameObjectProperty(ParentalGameObjectProperty parental, 
            GameObject owner, float maxValue, float value)
        {
            _owner = owner;
            _value = value;
            _maxValue = maxValue;
            Parental = parental;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Parental, Value);
        }

#endregion

#region IClassAsAtom Members

        public void Destroy() { }

        [XmlIgnore, Browsable(false)]
        public Atom ClassAsAtom
        {
            get { return Parental; }
        }

#endregion

    }
}