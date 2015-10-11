#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Interfaces;

#endregion

namespace GMechanics.Core.Classes.Entities
{
    [TypeConverter(typeof(InteractiveRecipientsListConverter))]
    [Editor(typeof(InteractiveRecipientsListEditor), typeof(UITypeEditor))]
    public class InteractiveRecipientsList : Dictionary<InteractiveEventType, List<GameObjectFeature>>
    {

#region Static members

        public static Type InteractiveRecipientsListEditor;

#endregion

#region Class functions

        public InteractiveRecipientsList() : base(0) {}

        public new List<GameObjectFeature> this[InteractiveEventType eventType]
        {
            get { return ContainsKey(eventType) ? base[eventType] : null; }
        }

        public void Add(InteractiveEventType eventType, GameObjectFeature recipient)
        {
            if (!ContainsKey(eventType))
            {
                Add(eventType, new List<GameObjectFeature> {recipient});
            }
            else
            {
                List<GameObjectFeature>  list = this[eventType];
                if (!list.Contains(recipient))
                {
                    list.Add(recipient);
                    list.Sort();
                }
            }
        }

        public void Remove(InteractiveEventType eventType, GameObjectFeature recipient)
        {
            if (ContainsKey(eventType))
            {
                List<GameObjectFeature> list = this[eventType];
                list.Remove(recipient);
                if (list.Count == 0)
                {
                    Remove(eventType);
                }
            }
        }

        public InteractiveRecipientsList Clone()
        {
            InteractiveRecipientsList clone = new InteractiveRecipientsList();
            foreach (KeyValuePair<InteractiveEventType, List<GameObjectFeature>> pair in this)
            {
                InteractiveEventType type = pair.Key;
                foreach (GameObjectFeature gof in pair.Value)
                {
                    clone.Add(type, gof);
                }
            }
            return clone;
        }

#endregion

    }

#region InteractiveRecipientsListEditor

    internal class InteractiveRecipientsListEditor : UITypeEditor
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
            Atom atom = (Atom)context.Instance;
            if (service != null && atom != null)
            {
                InteractiveRecipientsList list = atom.InteractiveRecipients ?? new InteractiveRecipientsList();
                Form form = (Form)Activator.CreateInstance(InteractiveRecipientsList.InteractiveRecipientsListEditor,
                    new object[] { list, GameEntityTypesTable.TypeOf(atom) });
                service.ShowDialog(form);
                list = (InteractiveRecipientsList) ((ICustomEditor) form).Result;
                value = list.Count == 0 ? null : list;
                form.Dispose();
            }
            return value;
        }
        // ReSharper restore AssignNullToNotNullAttribute
    }

#endregion

#region InteractiveRecipientsListConverter

    internal class InteractiveRecipientsListConverter : StringConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
                                         object value, Type destType)
        {
            if (destType == typeof(string))
            {
                string result = string.Empty;
                InteractiveRecipientsList list = ((Atom)context.Instance).InteractiveRecipients;
                if (list != null)
                {
                    foreach (KeyValuePair<InteractiveEventType, List<GameObjectFeature>> pair in list)
                    {
                        InteractiveEventType type = pair.Key;
                        foreach (GameObjectFeature gof in pair.Value)
                        {
                            bool lastValue = result.Length > 200;
                            string txt = string.Format("{0}: {1}", gof, type);
                            result = string.Format("{0}{1}{2}", result, string.IsNullOrEmpty(result)
                                ? string.Empty : ", ", lastValue ? "..." : txt);
                            if (lastValue)
                            {
                                break;
                            }
                        }
                    }
                }
                return string.Format("[{0}]", result);
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }

#endregion

}