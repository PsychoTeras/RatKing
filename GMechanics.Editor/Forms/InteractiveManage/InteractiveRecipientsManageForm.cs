using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Interfaces;

namespace GMechanics.Editor.Forms.InteractiveManage
{
    public partial class InteractiveRecipientsManageForm : Form, ICustomEditor
    {
        private static Size _formSize;
        private static int _selectedColumn;

        private InteractiveRecipientsList _list;
        private readonly GameEntityType _gameEntityType;
        private readonly InteractiveRecipientsList _clonedList;
        private readonly Dictionary<long, ListViewGroup> _groupsList =
            new Dictionary<long, ListViewGroup>();

        public object Result
        {
            get { return _list; }
        }

        public InteractiveRecipientsManageForm(InteractiveRecipientsList list, 
                                               GameEntityType gameEntityType)
        {
            InitializeComponent();
            if (!_formSize.IsEmpty)
            {
                Size = _formSize;
            }
            LvRecipientsSelectedIndexChanged(this, null);

            _gameEntityType = gameEntityType;
            _clonedList = (_list = list).Clone();

            ReloadList();
        }

        private void LvRecipientsSelectedIndexChanged(object sender, System.EventArgs e)
        {
            btnEdit.Enabled = btnDelete.Enabled = lvRecipients.SelectedItems.Count > 0;
        }

        private void LvRecipientsColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (_selectedColumn != e.Column)
            {
                _selectedColumn = e.Column;
                ReloadList();
            }
        }

        private void ClearAll()
        {
            lvRecipients.BeginUpdate();
            lvRecipients.Items.Clear();
            lvRecipients.Groups.Clear();
            _groupsList.Clear();
        }

        private void FillGroupsList()
        {
            foreach (KeyValuePair<InteractiveEventType, List<GameObjectFeature>> pair in _clonedList)
            {
                InteractiveEventType type = pair.Key;
                foreach (GameObjectFeature gof in pair.Value)
                {
                    switch (_selectedColumn)
                    {
                        case 0:
                            {
                                long id = gof.ObjectId;
                                if (!_groupsList.ContainsKey(id))
                                {
                                    string header = string.Format("Feature: {0}", gof);
                                    _groupsList.Add(id, new ListViewGroup(header));
                                }
                                break;
                            }
                        case 1:
                            {
                                long id = (long)type;
                                if (!_groupsList.ContainsKey(id))
                                {
                                    string header = string.Format("Event: {0}", type);
                                    _groupsList.Add(id, new ListViewGroup(header));
                                }
                                break;
                            }
                    }
                }
            }
            foreach (ListViewGroup group in _groupsList.Values)
            {
                lvRecipients.Groups.Add(group);
            }
        }

        private ListViewGroup GetRecipientRecordGroup(InteractiveEventType type, GameObjectFeature gof)
        {
            switch (_selectedColumn)
            {
                case 0:
                    {
                        long id = gof.ObjectId;
                        if (!_groupsList.ContainsKey(id))
                        {
                            string header = string.Format("Feature: {0}", gof);
                            ListViewGroup group = new ListViewGroup(header);
                            lvRecipients.Groups.Add(group);
                            _groupsList.Add(id, group);
                        }
                        return _groupsList[gof.ObjectId];
                    }
                case 1:
                    {
                        long id = (long)type;
                        if (!_groupsList.ContainsKey(id))
                        {
                            string header = string.Format("Event: {0}", type);
                            ListViewGroup group = new ListViewGroup(header);
                            lvRecipients.Groups.Add(group);
                            _groupsList.Add(id, group);
                        }
                        return _groupsList[(long) type];
                    }
            }
            return null;
        }

        private ListViewItem AddRecord(InteractiveEventType type, GameObjectFeature gof)
        {
            ListViewGroup group = GetRecipientRecordGroup(type, gof);
            ListViewItem item = new ListViewItem(gof.ToString(), group);
            item.Tag = new KeyValuePair<InteractiveEventType, GameObjectFeature>(type, gof);
            item.SubItems.Add(type.ToString());
            lvRecipients.Items.Add(item);
            return item;
        }

        private void FillRecipientsList()
        {
            foreach (KeyValuePair<InteractiveEventType, List<GameObjectFeature>> pair in _clonedList)
            {
                InteractiveEventType type = pair.Key;
                foreach (GameObjectFeature gof in pair.Value)
                {
                    AddRecord(type, gof);
                }
            }
        }

        private void RestorePrewSelectedRecord(ListViewItem prewSelected)
        {
            if (prewSelected != null)
            {
                KeyValuePair<InteractiveEventType, GameObjectFeature> pair =
                    (KeyValuePair<InteractiveEventType, GameObjectFeature>)prewSelected.Tag;
                string pairStr = pair.ToString();
                ListViewItem item = lvRecipients.Items.Cast<ListViewItem>().FirstOrDefault(
                    i => i.Tag.ToString() == pairStr);
                if (item != null)
                {
                    item.Selected = true;
                    item.EnsureVisible();
                }
            }
        }

        private void ReloadList()
        {
            ListViewItem prewSelected = null;
            if (lvRecipients.SelectedItems.Count > 0)
            {
                prewSelected = lvRecipients.SelectedItems[0];
            }

            ClearAll();
            FillGroupsList();
            FillRecipientsList();
            RestorePrewSelectedRecord(prewSelected);

            lvRecipients.EndUpdate();
            lvRecipients.Refresh();
        }

        private void AddRecipient()
        {
            using (var form = new InteractiveRecipientManageForm(_clonedList, 
                _gameEntityType, false, null, InteractiveEventType.Assigning))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    lvRecipients.BeginUpdate();

                    _clonedList.Add(form.SelectedEventType, form.SelectedFeature);
                    ListViewItem item = AddRecord(form.SelectedEventType, form.SelectedFeature);
                    item.Selected = true;

                    lvRecipients.EndUpdate();
                    lvRecipients.Refresh();
                    item.EnsureVisible();
                }
            }
        }

        private void EditSelectedRecipient()
        {
            if (lvRecipients.SelectedItems.Count > 0)
            {
                ListViewItem item = lvRecipients.SelectedItems[0];
                KeyValuePair<InteractiveEventType, GameObjectFeature> pair =
                    (KeyValuePair<InteractiveEventType, GameObjectFeature>)item.Tag;
                using (var form = new InteractiveRecipientManageForm(_clonedList, 
                    _gameEntityType, true, pair.Value, pair.Key))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        lvRecipients.BeginUpdate();

                        lvRecipients.Items.Remove(item);
                        _clonedList.Remove(pair.Key, pair.Value);
                        _clonedList.Add(form.SelectedEventType, form.SelectedFeature);

                        item = AddRecord(form.SelectedEventType, form.SelectedFeature);
                        item.Selected = true;

                        lvRecipients.EndUpdate();
                        lvRecipients.Refresh();
                        item.EnsureVisible();
                    }
                }
            }
        }

        private void DeleteSelectedRecipient()
        {
            if (lvRecipients.SelectedItems.Count > 0)
            {
                ListViewItem item = lvRecipients.SelectedItems[0];
                KeyValuePair<InteractiveEventType, GameObjectFeature> pair =
                    (KeyValuePair<InteractiveEventType, GameObjectFeature>) item.Tag;
                lvRecipients.BeginUpdate();
                _clonedList.Remove(pair.Key, pair.Value);
                lvRecipients.Items.Remove(item);
                lvRecipients.EndUpdate();
            }
        }

        private void BtnAddClick(object sender, System.EventArgs e)
        {
            AddRecipient();
        }

        private void BtnEditClick(object sender, System.EventArgs e)
        {
            EditSelectedRecipient();
        }

        private void LvRecipientsMouseDoubleClick(object sender, MouseEventArgs e)
        {
            EditSelectedRecipient();
        }

        private void BtnDeleteClick(object sender, System.EventArgs e)
        {
            DeleteSelectedRecipient();
        }

        private void InteractiveRecipientsManageFormKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    AddRecipient();
                    break;
                case Keys.F6:
                    EditSelectedRecipient();
                    break;
                case Keys.F8:
                case Keys.Delete:
                    DeleteSelectedRecipient();
                    break;
                case Keys.Escape:
                    Close();
                    break;
                case Keys.Enter:
                    BtnOkClick(sender, e);
                    break;
            }
        }

        private void BtnOkClick(object sender, System.EventArgs e)
        {
            _list = _clonedList;
            DialogResult = DialogResult.OK;
        }

        private void InteractiveRecipientsManageFormFormClosed(object sender, FormClosedEventArgs e)
        {
            _formSize = new Size(Width, Height);
        }
    }
}
