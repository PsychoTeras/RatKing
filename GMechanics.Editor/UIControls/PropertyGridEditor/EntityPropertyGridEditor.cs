using System;
using System.ComponentModel;
using System.Windows.Forms;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Types;
using GMechanics.Editor.UIControls.TreeViews;

namespace GMechanics.Editor.UIControls.PropertyGridEditor
{
    public sealed partial class EntityPropertyGridEditor : PropertyGridEx.PropertyGridEx
    {
#region Delegates

        public delegate void PropertyNameExistsCheckHandler(object s, Atom atom, 
            string newName, out bool isExists);

#endregion

        private BaseTreeView _treeView;

        public EntityPropertyGridEditor()
        {
            InitializeComponent();
        }

        [Browsable(true)]
        public BaseTreeView TreeView
        {
            get { return _treeView; }
            set
            {
                _treeView = value;
                SelectedObject = _treeView != null ? _treeView.SelectedObject : null;
            }
        }

        [Category("Action")]
        public event PropertyNameExistsCheckHandler PropertyNameExistsCheck;

        private void OnPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (_treeView != null)
            {
                GridItem gridItem = e.ChangedItem;
                switch ((gridItem.Label ?? "").Trim())
                {
                    case "Name":
                        {
                            string newName = gridItem.Value.ToString().Trim();
                            if (string.IsNullOrEmpty(newName))
                            {
                                MessageBox.Show("Name cannot be empty", "Error", MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                                return;
                            }

                            bool isExists = false;
                            Atom atom = (Atom) _treeView.SelectedObject;
                            if (PropertyNameExistsCheck != null)
                            {
                                PropertyNameExistsCheck(this, atom, newName, out isExists);
                            }

                            if (isExists)
                            {
                                MessageBox.Show("Record with same name currently exist", "Error",
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                                atom.Name = e.OldValue.ToString();
                                SelectedGridItem = gridItem;
                            }
                            else
                            {
                                _treeView.ChangeObjectName(e.OldValue.ToString());
                                _treeView.AfterUpdateObjectNameOrTranscription(atom);
                            }
                            break;
                        }
                    case "Transcription":
                        {
                            Atom atom = (Atom)_treeView.SelectedObject;
                            _treeView.AfterUpdateObjectNameOrTranscription(atom);
                            break;
                        }
                }
            }
        }

        protected override void OnSelectedGridItemChanged(SelectedGridItemChangedEventArgs e)
        {
            if (e.NewSelection != null && e.NewSelection.PropertyDescriptor != null &&
                ActiveControl != null)
            {
                bool readOnly = false;
                foreach (Attribute a in e.NewSelection.PropertyDescriptor.Attributes)
                {
                    if (a is PreventingEditingAttribute)
                    {
                        readOnly = true;
                        break;
                    }
                }
                foreach (Control control in ActiveControl.Controls)
                {
                    if (control.GetType().ToString().Contains("GridViewEdit"))
                    {
                        TextBox tb = control as TextBox;
                        if (tb != null)
                        {
                            tb.ReadOnly = readOnly;
                            break;
                        }
                    }
                }
            }
            base.OnSelectedGridItemChanged(e);
        }
    }
}