using System;
using System.Windows.Forms;

namespace GMechanics.Editor.Forms.GameObjectManage
{
    public sealed partial class GameObjectGroupManageForm : Form
    {

#region Delegates

        public delegate bool IsGameObjectGroupNameExistsCheckHandler(string groupName);

#endregion

        private readonly bool _editing;
        private readonly string _oldGroupName;

        public IsGameObjectGroupNameExistsCheckHandler IsGameObjectGroupNameExists;
        private string _prewGroupNameText;

        public GameObjectGroupManageForm(string gameObjectType, bool editing,
                                         string oldGroupName, string oldTranscription,
                                         IsGameObjectGroupNameExistsCheckHandler handler)
        {
            InitializeComponent();

            _editing = editing;
            _oldGroupName = oldGroupName;
            IsGameObjectGroupNameExists = handler;
            _prewGroupNameText = string.Empty;

            if (_editing)
            {
                tbGroupName.Text = _oldGroupName = _prewGroupNameText = oldGroupName;
                tbTranscription.Text = oldTranscription;
            }

            string[] caption = new[]
                              {
                                  string.Format("Add new {0} group", gameObjectType.ToLower()),
                                  string.Format("Edit {0} group", gameObjectType.ToLower())
                              };
            Text = caption[_editing ? 1 : 0];
        }

        public string GroupName
        {
            get { return tbGroupName.Text.Trim(); }
        }

        public string Transcription
        {
            get { return tbTranscription.Text.Trim(); }
        }

        private void PropertyClassManageFormLoad(object sender, EventArgs e)
        {
            tbGroupName.Select();
            tbGroupName.SelectAll();
        }

        private void BtnOkClick(object sender, EventArgs e)
        {
            string className = tbGroupName.Text.Trim();

            if (string.IsNullOrEmpty(className))
            {
                MessageBox.Show("Group name cannot be empty", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbGroupName.Focus();
                return;
            }

            if (IsGameObjectGroupNameExists != null && IsGameObjectGroupNameExists(className) &&
                (!_editing || _oldGroupName != className))
            {
                MessageBox.Show("Group with same name currently exist", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbGroupName.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void BtnCancelClick(object sender, EventArgs e)
        {
            Close();
        }

        private void PropertyClassManageFormKeyDown(object sender, KeyEventArgs e)
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

        private void TbClassNameTextChanged(object sender, EventArgs e)
        {
            if (tbTranscription.Text == _prewGroupNameText)
            {
                _prewGroupNameText = tbTranscription.Text = tbGroupName.Text;
            }
        }
    }
}