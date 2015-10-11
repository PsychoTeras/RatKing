using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;
using GMechanics.Core.Classes.Entities.ParentalGameObjectAttributeClasses;
using GMechanics.Core.Classes.Interfaces;

namespace GMechanics.Core.Classes.Entities.GameObjectAttributeClasses
{
    [TypeConverter(typeof(GameObjectAttributeConverter))]
    public sealed class GameObjectAttribute : IClassAsAtom,
        ICustomTypeDescriptor
    {

#region Private members

        [NonSerialized]
        private GameObjectAttributeValuesList _values;

#endregion

#region Properties

        [XmlIgnore]
        public ParentalGameObjectAttribute Parental { get; private set; }

        [XmlIgnore]
        public string Name { get { return Parental.Name; } }

        [XmlIgnore]
        public string Transcription { get { return Parental.Transcription; } }

        [XmlIgnore]
        public string Description { get { return Parental.Description; } }

        [XmlIgnore]
        public GameObjectAttributeValuesList Values
        {
            get { return _values; }
            set { _values = value != null && value.Count > 0 ? value : null; }
        }

        [XmlIgnore]
        public GameObjectAttributeValue this[string name]
        {
            get { return Values != null ? Values[name] : null; }
        }

#endregion

#region Class functions

        public GameObjectAttribute(ParentalGameObjectAttribute parental,
            GameObjectAttributeValuesList values)
        {
            Parental = parental;
            Values = values;
        }

        public override string ToString()
        {
            return string.Format("{0}", Parental);
        }

#endregion

#region IClassAsAtom Members

        public void Destroy() {}

        [XmlIgnore, Browsable(false)]
        public Atom ClassAsAtom
        {
            get { return Parental; }
        }

#endregion

#region ICustomTypeDescriptor

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);
            if (Values != null)
            {
                int cnt = Values.Count;
                for (int i = 0; i < cnt; i++)
                {
                    pds.Add(new GameObjectAttributePropertyDescriptor(this, i));
                }
            }
            return pds;
        }

#endregion

    }

#region GameObjectAttributeConverter

    internal class GameObjectAttributeConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, 
            CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof (string))
            {
                return string.Empty;
            }
            return base.ConvertTo(context, culture, value, destType);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

#endregion AttributeValuesListConverter

#region GameObjectAttributePropertyDescriptor

    public class GameObjectAttributePropertyDescriptor : PropertyDescriptor
    {
        private readonly int _index;
        private readonly GameObjectAttribute _goa;

        public GameObjectAttributePropertyDescriptor(GameObjectAttribute goa, int index) : 
            base(index.ToString(), null)
        {
            _goa = goa;
            _index = index;
        }

        public override Type ComponentType
        {
            get { return _goa.GetType(); }
        }

        public override string DisplayName
        {
            get
            {
                string prefix = new string('\t', _goa.Values.Count - _index - 1);
                return string.Format("{0}{1}", prefix, _goa.Values[_index].Name);
            }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return _goa.Values[_index].GetType(); }
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override object GetValue(object component)
        {
            return _goa.Values[_index];
        }

        public override void SetValue(object component, object value)
        {
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }

#endregion


}