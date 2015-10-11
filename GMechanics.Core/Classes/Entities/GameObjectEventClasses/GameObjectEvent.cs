using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.GameScript;

namespace GMechanics.Core.Classes.Entities.GameObjectEventClasses
{
    public delegate void OnScriptChanged();

    [Serializable]
    [TypeConverter(typeof(GameObjectEventConverter))]
    [Editor(typeof (GameObjectEventEditor), typeof (UITypeEditor))]
    public sealed class GameObjectEvent
    {

#region Static members

        public static Type GameObjectEventEditor;
        public static OnScriptChanged ScriptChanged;

#endregion

#region Internal members and properties

        [NonSerialized]
        internal ScriptExecuter OnAssigningExecuter;
        public byte[] OnAssigningByteCode
        {
            get { return OnAssigningExecuter == null ? null : OnAssigningExecuter.ByteCode; }
            set { OnAssigningExecuter = value != null ? new ScriptExecuter(value) : null; }
        }

        [NonSerialized]
        internal ScriptExecuter OnAssignedExecuter;
        public byte[] OnAssignedByteCode
        {
            get { return OnAssignedExecuter == null ? null : OnAssignedExecuter.ByteCode; }
            set { OnAssignedExecuter = value != null ? new ScriptExecuter(value) : null; }
        }

        [NonSerialized]
        internal ScriptExecuter OnRemovingExecuter;
        public byte[] OnRemovingByteCode
        {
            get { return OnRemovingExecuter == null ? null : OnRemovingExecuter.ByteCode; }
            set { OnRemovingExecuter = value != null ? new ScriptExecuter(value) : null; }
        }

        [NonSerialized]
        internal ScriptExecuter OnRemovedExecuter;
        public byte[] OnRemovedByteCode
        {
            get { return OnRemovedExecuter == null ? null : OnRemovedExecuter.ByteCode; }
            set { OnRemovedExecuter = value != null ? new ScriptExecuter(value) : null; }
        }

        [NonSerialized]
        internal ScriptExecuter OnChangingExecuter;
        public byte[] OnChangingByteCode
        {
            get { return OnChangingExecuter == null ? null : OnChangingExecuter.ByteCode; }
            set { OnChangingExecuter = value != null ? new ScriptExecuter(value) : null; }
        }

        [NonSerialized]
        internal ScriptExecuter OnChangedExecuter;
        public byte[] OnChangedByteCode
        {
            get { return OnChangedExecuter == null ? null : OnChangedExecuter.ByteCode; }
            set { OnChangedExecuter = value != null ? new ScriptExecuter(value) : null; }
        }

        [NonSerialized]
        internal ScriptExecuter OnInteractExecuter;
        public byte[] OnInteractByteCode
        {
            get { return OnInteractExecuter == null ? null : OnInteractExecuter.ByteCode; }
            set { OnInteractExecuter = value != null ? new ScriptExecuter(value) : null; }
        }

        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return OnAssigningExecuter == null && OnAssignedExecuter == null &&
                       OnRemovingExecuter == null && OnRemovedExecuter == null &&
                       OnChangingExecuter == null && OnChangedExecuter == null &&
                       OnInteractExecuter == null;
            }
        }

#endregion

#region Class functions

        public bool IsEventEmpty(InteractiveEventType eventType)
        {
            switch (eventType)
            {
                case InteractiveEventType.Assigning:
                    return OnAssigningExecuter == null;
                case InteractiveEventType.Assigned:
                    return OnAssignedExecuter == null;
                case InteractiveEventType.Removing:
                    return OnRemovingExecuter == null;
                case InteractiveEventType.Removed:
                    return OnRemovedExecuter == null;
                case InteractiveEventType.Changing:
                    return OnChangingExecuter == null;
                case InteractiveEventType.Changed:
                    return OnChangedExecuter == null;
                case InteractiveEventType.Interact:
                    return OnInteractExecuter == null;
            }
            return true;
        }

#endregion

    }

#region GameObjectEventEditor

    internal class GameObjectEventEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider,
                                         object value)
        {
            IWindowsFormsEditorService service = provider.GetService(
                typeof (IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            GameObjectFeature gof = (GameObjectFeature)context.Instance;
            if (service != null && gof != null)
            {
                gof.Event = gof.Event ?? new GameObjectEvent();
                Form form = (Form) Activator.CreateInstance(GameObjectEvent.GameObjectEventEditor,
                    new object[] { gof, gof.Event });
                service.ShowDialog(form);
                if (gof.Event.IsEmpty)
                {
                    gof.Event = null;
                }
                value = gof.Event;
                form.Dispose();
                if (GameObjectEvent.ScriptChanged != null)
                {
                    GameObjectEvent.ScriptChanged();
                }
            }
            return value;
        }
    }

    #endregion

#region GameObjectEventConverter

    internal class GameObjectEventConverter : StringConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
                                         object value, Type destType)
        {
            if (destType == typeof(string))
            {
                string result = string.Empty;
                GameObjectFeature gof = (GameObjectFeature)context.Instance;
                if (gof.Event != null)
                {
                    Array types = Enum.GetValues(typeof(InteractiveEventType));
                    foreach (InteractiveEventType type in types)
                    {
                        if (!gof.Event.IsEventEmpty(type))
                        {
                            result = string.Format("{0}{1}{2}", result, string.IsNullOrEmpty(result) ?
                                string.Empty : ", ", type);
                        }
                    }
                }
                return string.Format("[{0}]", result);
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }

    #endregion AttributeValuesListConverter

}
