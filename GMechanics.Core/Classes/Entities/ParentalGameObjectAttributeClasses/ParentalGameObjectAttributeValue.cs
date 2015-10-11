using System;
using System.ComponentModel;
using System.Xml.Serialization;
using GMechanics.Core.Classes.Types;

namespace GMechanics.Core.Classes.Entities.ParentalGameObjectAttributeClasses
{
    [Serializable]
    [TypeConverter(typeof (ExpandableObjectConverter))]
    public sealed class ParentalGameObjectAttributeValue : Atom
    {

#region Private members

        [NonSerialized]
        private string _name;

        [NonSerialized]
        private float _minValue;

        [NonSerialized]
        private float _maxValue;

        [NonSerialized]
        private ParentalGameObjectAttributeValuesList _values;

#endregion

#region Properties

        [Browsable(false), XmlIgnore]
        public Atom Parent { get; internal set; }

        [Browsable(false), XmlIgnore]
        public ParentalGameObjectAttribute ParentAttribute
        {
            get
            {
                Atom parent = Parent;
                while (parent != null && !(parent is ParentalGameObjectAttribute))
                {
                    parent = ((ParentalGameObjectAttributeValue) parent).Parent;
                }
                return (ParentalGameObjectAttribute) parent;
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        public override string Name
        {
            get { return _name; }
            set
            {
                bool setTranscription = (Transcription == _name);
                _name = value;
                if (setTranscription)
                {
                    Transcription = _name;
                }
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        public override string Transcription { get; set; }

        [Browsable(false)]
        public int NestingLevel { get; set; }

        [PreventingEditing]
        [DisplayName("Values"), Category("\tAttribute value settings")]
        public ParentalGameObjectAttributeValuesList Values
        {
            get { return _values; }
            set { _values = value ?? new ParentalGameObjectAttributeValuesList(); }
        }

        [DisplayName("\t\t\tMinimal value"), Category("\tAttribute value settings")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public float MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                _maxValue = _maxValue < _minValue ? _minValue : _maxValue;
            }
        }

        [DisplayName("\t\tMaximal value"), Category("\tAttribute value settings")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public float MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                _minValue = _minValue > _maxValue ? _maxValue : _minValue;
            }
        }

#endregion

#region Class functions

        public ParentalGameObjectAttributeValue()
        {
            Name = Transcription = string.Empty;
            _values = new ParentalGameObjectAttributeValuesList();
        }

        public ParentalGameObjectAttributeValue(int nestingLevel) : this()
        {
            NestingLevel = nestingLevel;
        }

        public override object Clone()
        {
            ParentalGameObjectAttributeValue result = new ParentalGameObjectAttributeValue();
            result.Name = Name;
            result.Transcription = Transcription;
            result.Description = Description;
            result.NestingLevel = NestingLevel;
            result.MinValue = MinValue;
            result.MaxValue = MaxValue;
            result.Values = Values.Clone();
            return result;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", 
                Name ?? string.Empty, 
                Transcription ?? string.Empty);
        }

        public override void Destroy()
        {
            Values.Destroy();
            base.Destroy();
        }

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

        private bool ShouldSerializeValues()
        {
            return false;
        }

        private bool ShouldSerializeNestingLevel()
        {
            return false;
        }

        private bool ShouldSerializeMinValue()
        {
            return false;
        }

        private bool ShouldSerializeMaxValue()
        {
            return false;
        }
       
#endregion

    }
}