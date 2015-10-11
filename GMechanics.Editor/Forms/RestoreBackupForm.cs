using System;
using System.IO;
using System.Windows.Forms;
using GMechanics.Core;

namespace GMechanics.Editor.Forms
{
    public partial class RestoreBackupForm : Form
    {
        public RestoreBackupForm()
        {
            InitializeComponent();
        }

        public string BackupFileName
        {
            get
            {
                return (string) (lvBackups.SelectedItems.Count == 1
                                     ? lvBackups.SelectedItems[0].Tag
                                     : null);
            }
        }

        private string DateTimeFromFileName(string fileName)
        {
            string name = (Path.GetFileNameWithoutExtension(fileName) ?? "").
                Replace("x", ":").Replace("_", ".");
            return name;
        }

        private ListViewItem NewRecordFromFileName(string fileName)
        {
            ListViewItem item = new ListViewItem {Tag = fileName};
            string name = Path.GetFileName(fileName);
            item.Text = DateTimeFromFileName(name);
            return item;
        }

        private void RefreshBackupsList()
        {
            Cursor = Cursors.WaitCursor;
            lvBackups.BeginUpdate();
            lvBackups.Items.Clear();

            if (Directory.Exists(GlobalVariables.BackupsFolderPath))
            {
                int cnt = 0;
                string[] backupFiles = Directory.GetFiles(GlobalVariables.BackupsFolderPath, 
                                            "*.bak", SearchOption.TopDirectoryOnly);
                Array.Sort(backupFiles, (s1, s2) => s2.CompareTo(s1));
                foreach (string backupFile in backupFiles)
                {
                    ListViewItem item = NewRecordFromFileName(backupFile);
                    lvBackups.Items.Add(item);
                    if (chkLastTen.Checked && ++cnt == 10)
                    {
                        break;
                    }
                }
            }

            lvBackups.EndUpdate();
            Cursor = Cursors.Default;
            LvBackupsSelectedIndexChanged(this, null);
        }

        private void Restore()
        {
            if (btnRestore.Enabled)
            {
                bool restore = MessageBox.Show(string.Format("Restore the backup dated {0}?",
                    lvBackups.SelectedItems[0].Text), "Confirmation", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Information) == DialogResult.Yes;
                if (restore)
                {
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void RestoreFormLoad(object sender, EventArgs e)
        {
            RefreshBackupsList();
        }

        private void LvBackupsSelectedIndexChanged(object sender, EventArgs e)
        {
            btnRestore.Enabled = lvBackups.SelectedItems.Count == 1;
        }

        private void LvBackupsDoubleClick(object sender, EventArgs e)
        {
            Restore();
        }

        private void RestoreBackupFormKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    break;
                case Keys.Enter:
                    Restore();
                    break;
            }
        }
    }
}
