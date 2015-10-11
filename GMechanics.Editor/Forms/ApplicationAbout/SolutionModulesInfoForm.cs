using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace GMechanics.Editor.Forms.ApplicationAbout
{
    partial class SolutionModulesInfoForm : Form
    {
        public SolutionModulesInfoForm()
        {
            InitializeComponent();
        }

        public void Init(string solutionDirectory, ModulesToShow modulesToShow,
                         string[] excludes)
        {
            string[] files = new string[0];
            string[] filesExe = Directory.GetFiles(solutionDirectory, "*.exe");
            Array.Sort(filesExe);
            string[] filesDll = Directory.GetFiles(solutionDirectory, "*.dll");
            Array.Sort(filesDll);
            switch (modulesToShow)
            {
                case ModulesToShow.Exe:
                    files = filesExe;
                    break;
                case ModulesToShow.Dll:
                    files = filesDll;
                    break;
                case ModulesToShow.Both:
                    files = new string[filesExe.Length + filesDll.Length];
                    filesExe.CopyTo(files, 0);
                    filesDll.CopyTo(files, filesExe.Length);
                    break;
            }
            IList<string> excludesList = excludes;
            foreach (string file in files)
            {
                if (excludes == null || !excludesList.Contains(file))
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(file);
                    ListViewItem lvi = new ListViewItem(Path.GetFileName(file));
                    lvi.SubItems.Add(fvi.FileVersion);
                    lvModules.Items.Add(lvi);
                }
            }
            ShowDialog();
        }

        private void BtnOkClick(object sender, EventArgs e)
        {
            Close();
        }

        private void ModulesInfoFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                BtnOkClick(sender, null);
            }
        }
    }

    public enum ModulesToShow
    {
        Exe,
        Dll,
        Both
    }
}
