using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using GMechanics.Core.Classes.Interfaces;
using GMechanics.Core.Classes.Types;
using GMechanics.Core.Helpers;

namespace GMechanics.Core.Classes.Entities.ParentalGameObjectAttributeClasses
{
    [Serializable]
    [TypeConverter(typeof (AttributeValuesListConverter))]
    [Editor(typeof (AttributeValuesListEditor), typeof (UITypeEditor))]
    public sealed class ParentalGameObjectAttributeValuesList :
        BaseList<ParentalGameObjectAttributeValue, ParentalGameObjectAttributeValuesList>,
        ICustomTypeDescriptor
    {

#region Static members

        public static Type AttributeValuesListEditor;

#endregion

#region Properties

        [Browsable(false)]
        public int NestingLevel { get; set; }

#endregion

#region Class functions

        private ParentalGameObjectAttributeValue GetValue(string valueName, 
            bool findInChilds, ParentalGameObjectAttributeValue value)
        {
            if (value.Name == valueName)
            {
                return value;
            }
            if (findInChilds)
            {
                return GetValue(valueName, value.Values);
            }
            return null;
        }

        private ParentalGameObjectAttributeValue GetValue(string valueName,
            ParentalGameObjectAttributeValuesList valuesList)
        {
            if (valuesList != null)
            {
                foreach (ParentalGameObjectAttributeValue goav in valuesList)
                {
                    ParentalGameObjectAttributeValue value = GetValue(valueName, true, goav);
                    if (value != null)
                    {
                        return value;
                    }
                }
            }
            return null;
        }

        public ParentalGameObjectAttributeValue GetValue(string valueName, bool findInChilds)
        {
            foreach (ParentalGameObjectAttributeValue v in this)
            {
                ParentalGameObjectAttributeValue value = GetValue(valueName, findInChilds, v);
                if (value != null)
                {
                    return value;
                }
            }
            return null;
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
            for (int i = 0; i < Count; i++)
            {
                pds.Add(new AttributeValuesListPropertyDescriptor(this, i));
            }
            return pds;
        }

#endregion

    }

#region AttributeValuesListEditor

    internal class AttributeValuesListEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        // ReSharper disable AssignNullToNotNullAttribute
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider,
                                         object value)
        {
            IWindowsFormsEditorService service = provider.GetService(
                typeof (IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            ParentalGameObjectAttributeValuesList values = value as ParentalGameObjectAttributeValuesList;
            if (service != null)
            {
                int nestingLevel = (context.Instance is ParentalGameObjectAttributeValue) 
                    ? ((ParentalGameObjectAttributeValue)context.Instance).NestingLevel + 1
                    : ((ParentalGameObjectAttribute)context.Instance).Values.NestingLevel;
                object form = Activator.CreateInstance(ParentalGameObjectAttributeValuesList.AttributeValuesListEditor,
                                                       new object[] { true, true, string.Empty, string.Empty, values, 
                                                       nestingLevel, (Helper.IsAttributeNameExistsCheckHandler)
                                                       Helper.IsGameObjectAttributeNameExists });
                if (service.ShowDialog((Form)form) == DialogResult.OK)
                {
                    return ((ICustomEditor)form).Result;
                }
            }
            return values;
        }
        // ReSharper restore AssignNullToNotNullAttribute

    }

#endregion

#region AttributeValuesListConverter

    internal class AttributeValuesListConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, 
            CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof (string))
            {
                return value != null ? value.ToString() : "[]";
            }
            return base.ConvertTo(context, culture, value, destType);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return context.Instance is ParentalGameObjectAttribute
                       ? ((ParentalGameObjectAttribute) context.Instance).Values.Count > 0
                       : ((ParentalGameObjectAttributeValue) context.Instance).Values.Count > 0;
        }
    }

#endregion AttributeValuesListConverter

#region AttributeValuesListPropertyDescriptor

    public class AttributeValuesListPropertyDescriptor : PropertyDescriptor
    {
        private readonly int _index;
        private readonly ParentalGameObjectAttributeValuesList _list;

        public AttributeValuesListPropertyDescriptor(ParentalGameObjectAttributeValuesList 
            list, int index) : base(index.ToString(), null)
        {
            _list = list;
            _index = index;
        }

        public override Type ComponentType
        {
            get { return _list.GetType(); }
        }

        public override string DisplayName
        {
            get
            {
                string prefix = new string('\t', _list.Count - _index - 1);
                return string.Format("{0}{1}", prefix, _list[_index].Name);
            }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return _list[_index].GetType(); }
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override object GetValue(object component)
        {
            return _list[_index];
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