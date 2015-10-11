using System;
using System.ComponentModel;
using GMechanics.Core.Classes.Types;

namespace GMechanics.Core.Classes.Entities.ParentalGameObjectAttributeClasses
{
    [Serializable]
    public sealed class ParentalGameObjectAttribute : Atom
    {

#region Private members

        [NonSerialized]
        private ParentalGameObjectAttributeValuesList _values;

#endregion

#region Properties

        [PreventingEditing]
        [DisplayName("Values"), Category("Attribute settings")]
        public ParentalGameObjectAttributeValuesList Values
        {
            get { return _values; }
            set { _values = value ?? new ParentalGameObjectAttributeValuesList(); }
        }

#endregion

#region Class members

        public ParentalGameObjectAttribute()
        {
            _values = new ParentalGameObjectAttributeValuesList();
        }

        public ParentalGameObjectAttribute(string name, string transcription,
            string description, ParentalGameObjectAttributeValuesList values)
        {
            Name = name;
            Transcription = transcription;
            Description = description;
            Values = values;
        }

        public override void Destroy()
        {
            Values.Destroy();
            base.Destroy();
        }

#endregion

#region ShouldSerialize

        private bool ShouldSerializeValues()
        {
            return false;
        }

#endregion

    }
}