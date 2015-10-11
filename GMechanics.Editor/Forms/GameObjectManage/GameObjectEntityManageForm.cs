using System;
using System.Windows.Forms;

namespace GMechanics.Editor.Forms.GameObjectManage
{
    public sealed partial class GameObjectEntityManageForm : Form
    {

#region Delegates

        public delegate bool IsEntityNameExistsCheckHandler(string entityName);

#endregion

        private readonly bool _editing;
        private readonly string _entityType;
        private readonly string _oldEntityName;

        public IsEntityNameExistsCheckHandler IsEntityNameExists;
        private string _prewEntityNameText;

        public GameObjectEntityManageForm(string entityType, bool editing,
                                          string oldEntityName, string oldTranscription, string[] classes,
                                          string currentClass, IsEntityNameExistsCheckHandler handler)
        {
            InitializeComponent();

            _entityType = entityType;
            _editing = editing;
            IsEntityNameExists = handler;
            cbClass.DataSource = classes;
            _prewEntityNameText = string.Empty;
            cbClass.SelectedIndex = Array.IndexOf(classes, currentClass);

            if (_editing)
            {
                tbEntityName.Text = _oldEntityName = _prewEntityNameText = oldEntityName;
                tbTranscription.Text = oldTranscription;
            }

            string[] caption = new[]
                              {
                                  string.Format("Add new {0}", _entityType.ToLower()),
                                  string.Format("Edit {0}", _entityType.ToLower())
                              };
            Text = caption[_editing ? 1 : 0];
            lblName.Text = string.Format("{0} name:", _entityType);
            lblClass.Text = string.Format("{0} class:", _entityType);
        }

        public string EntityName
        {
            get { return tbEntityName.Text.Trim(); }
        }

        public string Transcription
        {
            get { return tbTranscription.Text.Trim(); }
        }

        public string ClassName
        {
            get { return cbClass.Text; }
        }

        private void PropertyManageFormLoad(object sender, EventArgs e)
        {
            tbEntityName.Select();
            tbEntityName.SelectAll();
        }

        private void BtnOkClick(object sender, EventArgs e)
        {
            string propertyName = tbEntityName.Text.Trim();

            if (string.IsNullOrEmpty(propertyName))
            {
                MessageBox.Show(string.Format("{0} name cannot be empty", _entityType),
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbEntityName.Focus();
                return;
            }

            if (IsEntityNameExists != null && IsEntityNameExists(propertyName) &&
                (!_editing || _oldEntityName != propertyName))
            {
                MessageBox.Show(string.Format("{0} with same name currently exist", _entityType),
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbEntityName.Focus();
                return;
            }

            if (cbClass.SelectedIndex == -1)
            {
                MessageBox.Show(string.Format("{0} class must be selected", _entityType),
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbClass.Focus();
                cbClass.DroppedDown = true;
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void BtnCancelClick(object sender, EventArgs e)
        {
            Close();
        }

        private void PropertyManageFormKeyDown(object sender, KeyEventArgs e)
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

        private void TbPropertyNameTextChanged(object sender, EventArgs e)
        {
            if (tbTranscription.Text == _prewEntityNameText)
            {
                _prewEntityNameText = tbTranscription.Text = tbEntityName.Text;
            }
        }
    }
}