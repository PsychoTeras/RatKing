using System;
using System.Windows.Forms;

namespace GMechanics.Editor.Forms.GameObjectManage
{
    public sealed partial class GameObjectEntityClassManageForm : Form
    {

#region Delegates

        public delegate bool IsEntityClassNameExistsCheckHandler(string className);

#endregion

        private readonly bool _editing;
        private readonly string _oldClassName;

        public IsEntityClassNameExistsCheckHandler IsEntityClassNameExists;
        private string _prewClassNameText;

        public GameObjectEntityClassManageForm(string entityType, bool editing,
                                               string oldClassName, string oldTranscription,
                                               IsEntityClassNameExistsCheckHandler handler)
        {
            InitializeComponent();

            _editing = editing;
            _oldClassName = oldClassName;
            IsEntityClassNameExists = handler;
            _prewClassNameText = string.Empty;

            if (_editing)
            {
                tbClassName.Text = _oldClassName = _prewClassNameText = oldClassName;
                tbTranscription.Text = oldTranscription;
            }

            string[] caption = new[]
                              {
                                  string.Format("Add new {0} class", entityType.ToLower()),
                                  string.Format("Edit {0} class", entityType.ToLower())
                              };
            Text = caption[_editing ? 1 : 0];
        }

        public string ClassName
        {
            get { return tbClassName.Text.Trim(); }
        }

        public string Transcription
        {
            get { return tbTranscription.Text.Trim(); }
        }

        private void PropertyClassManageFormLoad(object sender, EventArgs e)
        {
            tbClassName.Select();
            tbClassName.SelectAll();
        }

        private void BtnOkClick(object sender, EventArgs e)
        {
            string className = tbClassName.Text.Trim();

            if (string.IsNullOrEmpty(className))
            {
                MessageBox.Show("Class name cannot be empty", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbClassName.Focus();
                return;
            }

            if (IsEntityClassNameExists != null && IsEntityClassNameExists(className) &&
                (!_editing || _oldClassName != className))
            {
                MessageBox.Show("Class with same name currently exist", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbClassName.Focus();
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
            if (tbTranscription.Text == _prewClassNameText)
            {
                _prewClassNameText = tbTranscription.Text = tbClassName.Text;
            }
        }
    }
}