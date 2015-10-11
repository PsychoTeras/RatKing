using System;
using System.Windows.Forms;

namespace GMechanics.Editor.Forms.GameObjectManage
{
    public sealed partial class GameObjectManageForm : Form
    {

#region Delegates

        public delegate bool IsGameObjectNameExistsCheckHandler(string groupName);

#endregion

        private readonly bool _editing;
        private readonly string _oldName;

        public IsGameObjectNameExistsCheckHandler IsGameObjectNameExists;
        private string _prewGameObjectNameText;

        public GameObjectManageForm(string gameObjectType, bool editing,
                                    string oldName, string oldTranscription,
                                    IsGameObjectNameExistsCheckHandler handler)
        {
            InitializeComponent();

            _editing = editing;
            _oldName = oldName;
            IsGameObjectNameExists = handler;
            _prewGameObjectNameText = string.Empty;

            if (_editing)
            {
                tbGameObjectName.Text = _oldName = _prewGameObjectNameText = oldName;
                tbTranscription.Text = oldTranscription;
            }

            string[] caption = new[]
                              {
                                  string.Format("Add new {0}", gameObjectType.ToLower()),
                                  string.Format("Edit {0}", gameObjectType.ToLower())
                              };
            Text = caption[_editing ? 1 : 0];
        }

        public string GameObjectName
        {
            get { return tbGameObjectName.Text.Trim(); }
        }

        public string Transcription
        {
            get { return tbTranscription.Text.Trim(); }
        }

        private void PropertyClassManageFormLoad(object sender, EventArgs e)
        {
            tbGameObjectName.Select();
            tbGameObjectName.SelectAll();
        }

        private void BtnOkClick(object sender, EventArgs e)
        {
            string className = tbGameObjectName.Text.Trim();

            if (string.IsNullOrEmpty(className))
            {
                MessageBox.Show("Game object name cannot be empty", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbGameObjectName.Focus();
                return;
            }

            if (IsGameObjectNameExists != null && IsGameObjectNameExists(className) &&
                (!_editing || _oldName != className))
            {
                MessageBox.Show("Game object with same name currently exist", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbGameObjectName.Focus();
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
            if (tbTranscription.Text == _prewGameObjectNameText)
            {
                _prewGameObjectNameText = tbTranscription.Text = tbGameObjectName.Text;
            }
        }
    }
}