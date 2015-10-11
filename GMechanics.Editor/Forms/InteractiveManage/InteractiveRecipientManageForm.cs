using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Storages;

namespace GMechanics.Editor.Forms.InteractiveManage
{
    public sealed partial class InteractiveRecipientManageForm : Form
    {

        private readonly bool _editing;
        private readonly GameObjectFeature _gof;
        private readonly InteractiveEventType _type;
        private readonly GameEntityType _gameEntityType;
        private readonly InteractiveRecipientsList _list;

        public GameObjectFeature SelectedFeature
        {
            get { return (GameObjectFeature)cbFeature.SelectedItem; }
        }

        public InteractiveEventType SelectedEventType
        {
            get { return (InteractiveEventType)cbEventType.SelectedItem; }
        }

        private void Reload()
        {
            cbFeature.BeginUpdate();
            cbFeature.Items.Clear();
            ICollection features = GlobalStorage.Instance.GameObjectFeatures.Values;
            foreach (GameObjectFeature gof in features)
            {
                cbFeature.Items.Add(gof);
            }
            cbFeature.Sorted = true;
            cbFeature.EndUpdate();

            cbEventType.BeginUpdate();
            Array types = Enum.GetValues(typeof (InteractiveEventType));
            foreach (InteractiveEventType type in types)
            {
                if (_gameEntityType != GameEntityType.GameObjectFeature ||
                    (type != InteractiveEventType.Changing &&
                     type != InteractiveEventType.Changed))
                {
                    cbEventType.Items.Add(type);
                }
                cbEventType.EndUpdate();
            }
        }

        public InteractiveRecipientManageForm(InteractiveRecipientsList list, 
            GameEntityType gameEntityType, bool editing, GameObjectFeature gof, 
            InteractiveEventType type)
        {
            InitializeComponent();

            _gof = gof;
            _type = type;
            _list = list;
            _editing = editing;
            _gameEntityType = gameEntityType;

            string[] caption = new[] { "Add recipient",
                                       "Edit recipient" };
            Text = caption[editing ? 1 : 0];

            if (editing)
            {
                cbFeature.Text = gof.Name;
                cbEventType.SelectedIndex = cbEventType.Items.IndexOf(type);
            }

            Reload();
        }

        private void InteractiveRecipientManageFormKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    break;
                case Keys.Enter:
                    BtnOkClick(sender, e);
                    break;
            }
        }

        private void BtnOkClick(object sender, EventArgs e)
        {
            if (SelectedFeature == null)
            {
                MessageBox.Show("Feature must be selected", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbFeature.Focus();
                return;
            }

            if (cbEventType.SelectedItem == null)
            {
                MessageBox.Show("Event type must be selected", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbEventType.Focus();
                return;
            }

            if (_list.ContainsKey(SelectedEventType))
            {
                if (_editing && _gof == SelectedFeature && _type == SelectedEventType)
                {
                    Close();
                    return;
                }

                List<GameObjectFeature> features = _list[SelectedEventType];
                if (features.Contains(SelectedFeature))
                {
                    MessageBox.Show("This combination of feature and event type already exist", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cbFeature.Focus();
                    return;
                }
            }

            DialogResult = DialogResult.OK;
        }

        private void CbEventTypeEnter(object sender, EventArgs e)
        {
            cbEventType.DroppedDown = true;
        }
    }
}
