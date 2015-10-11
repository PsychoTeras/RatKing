using System;
using System.ComponentModel;
using System.Xml.Serialization;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Entities.ParentalGameObjectAttributeClasses;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Interfaces;

namespace GMechanics.Core.Classes.Entities.GameObjectAttributeClasses
{
    public sealed class GameObjectAttributeValue : IClassAsAtom
    {

#region Private members

        [NonSerialized]
        private float _value;

        [NonSerialized]
        private readonly GameObject _owner;

#endregion

#region Properties

        [XmlIgnore]
        public ParentalGameObjectAttributeValue Parental { get; private set; }

        [XmlIgnore]
        public string Name { get { return Parental.Name; } }

        [XmlIgnore]
        public string Transcription { get { return Parental.Transcription; } }

        [XmlIgnore]
        public string Description { get { return Parental.Description; } }

        [XmlIgnore]
        public int NestingLevel { get { return Parental.NestingLevel; } }

        [XmlIgnore, DisplayName("\tMinimal value")]
        public float MinValue { get { return Parental.MinValue; } }

        [XmlIgnore, DisplayName("Maximal value")]
        public float MaxValue { get { return Parental.MaxValue; } }

        [DisplayName("Value")]
        public float Value
        {
            get { return _value; }
            set
            {
                bool cancelled;
                float curValue = (float)Parental.QueryInteractiveRecipients(
                    InteractiveEventType.Changing, _owner, this, _value, value, 
                    out cancelled);
                if (!cancelled)
                {
                    _value = curValue < MinValue ? MinValue :
                        curValue > MaxValue ? MaxValue : curValue;
                    Parental.NotifyInteractiveRecipients(InteractiveEventType.Changed, 
                        _owner, this, _value);
                }
            }
        }

#endregion

#region Class functions

        public GameObjectAttributeValue(ParentalGameObjectAttributeValue parental, 
            GameObject owner)
        {
            Parental = parental;
            _owner = owner;
        }

        public GameObjectAttributeValue(ParentalGameObjectAttributeValue parental,
            GameObject owner, float value) : this(parental, owner)
        {
            _value = value;
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
