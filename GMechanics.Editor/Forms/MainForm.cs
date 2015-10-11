using System;
using System.IO;
using System.Windows.Forms;
using GMechanics.Core;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.GameObjectEventClasses;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Entities.GameObjectPropertyClasses;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Entities.ParentalGameObjectAttributeClasses;
using GMechanics.Core.Classes.Storages;
using GMechanics.Editor.Data;
using GMechanics.Editor.Forms.ApplicationAbout;
using GMechanics.Editor.Forms.GameObjectManage;
using GMechanics.Editor.Forms.InteractiveManage;
using GMechanics.Editor.Forms.ScriptEditor;
using GMechanics.Editor.Helpers;
using GMechanics.Editor.UIControls.TreeViewPathEdit;
using GMechanics.Editor.UIControls.TreeViews;
using GMechanics.FlowchartControl;
using Ionic.Zip;

namespace GMechanics.Editor.Forms
{
    public partial class MainForm : Form
    {
        private readonly GlobalStorage _globalStorage = GlobalStorage.Instance;
        private readonly LocalStorage _localStorage = LocalStorage.Instance;

        void Demo()
        {
//            GameObject go = new GameObject("Man", "", "");
//            GameObjectProperty prop = go.AddProperty("Health");
//            prop.Value = 50;
//            go.AddFeature("Death");
        }

        public MainForm()
        {
            InitializeComponent();

            GameObjectEvent.ScriptChanged += tvFeatures.OnScriptChanged;
            GameObjectEvent.GameObjectEventEditor = typeof(ScriptEditorForm);
            InteractiveRecipientsList.InteractiveRecipientsListEditor = typeof(InteractiveRecipientsManageForm);
            ParentalGameObjectAttributeValuesList.AttributeValuesListEditor = typeof(GameObjectAttributeManageForm);

            ReloadData(false);
            Helper.BringWindowToFront(Handle);

            Demo();
        }

        private void ShowMessage(string message)
        {
            notifyIcon.ShowBalloonTip(1500, "GMechanics Editor", message, ToolTipIcon.Info);
        }

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            string appPath = Path.GetDirectoryName(Application.ExecutablePath);
            string[] excludes =
            {
                appPath + "\\PropertyGridEx.dll",
                appPath + "\\ApplicationAbout.dll"
            };
            ApplicationAboutForm.Show(ModulesToShow.Both, excludes);
            Cursor = Cursors.Default;
        }

        private string GetBackupFileName()
        {
            return DateTime.Now.ToString().
                Replace(":", "x").
                Replace(".", "_").
                Replace("/", "_") + ".bak";
        }

        private void CreateBackup(bool showMessage)
        {
            if (!Directory.Exists(GlobalVariables.DataFolderPath))
            {
                Directory.CreateDirectory(GlobalVariables.DataFolderPath);
            }

            if (Directory.GetFiles(GlobalVariables.DataFolderPath).Length > 0)
            {
                Cursor = Cursors.WaitCursor;

                string backupFilePath = Path.Combine(GlobalVariables.BackupsFolderPath,
                                                     GetBackupFileName());
                if (!Directory.Exists(GlobalVariables.BackupsFolderPath))
                {
                    Directory.CreateDirectory(GlobalVariables.BackupsFolderPath);
                }
                if (File.Exists(backupFilePath))
                {
                    File.Delete(backupFilePath);
                }

                GlobalStorage.Instance.CloseSQLiteConnections();
                LocalStorage.Instance.CloseSQLiteConnections();

                using (ZipFile zip = new ZipFile())
                {
                    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                    zip.AddDirectory(GlobalVariables.DataFolderPath, string.Empty);
                    zip.Save(backupFilePath);
                }

                GlobalStorage.Instance.RestoreSQLiteConnections();
                LocalStorage.Instance.RestoreSQLiteConnections();

                Cursor = Cursors.Default;

                if (showMessage)
                {
                    ShowMessage("Backup was created");
                }
            }
        }

        private void RestoreBackup(string fileName)
        {
            Cursor = Cursors.WaitCursor;
            string dataFolderPath = GlobalVariables.DataFolderPath;

            GlobalStorage.Instance.CloseSQLiteConnections();
            LocalStorage.Instance.CloseSQLiteConnections();

            if (!Directory.Exists(dataFolderPath))
            {
                Directory.CreateDirectory(dataFolderPath);
            }
            Array.ForEach(Directory.GetFiles(dataFolderPath), File.Delete);

            using (ZipFile zip = new ZipFile(fileName))
            {
                zip.ExtractAll(dataFolderPath);
            }

            ReloadData(false);
            Cursor = Cursors.Default;

            ShowMessage("Backup was restored");
        }

        private void ReloadData(bool showMessage)
        {
            _globalStorage.Load();
            _localStorage.Load();

            tvProperties.ReloadTreeView();
            tvFeatures.ReloadTreeView();
            tvAttributes.ReloadTreeView();
            tvGameObjectsEntities.ReloadTreeView();
            tvGameObjects.ReloadTreeView();
            tvGameObjectsEntities.ReloadTreeView();

            if (showMessage)
            {
                ShowMessage("Reloading complete");
            }
        }

        private void SaveData()
        {
            CreateBackup(false);
            _globalStorage.Save();
            _localStorage.Save();
            ShowMessage("Saving complete");
        }

        private void SetSelectedTreeNode(TreeNode node)
        {
            Helper.LockUpdate(tcMain.SelectedTab);
            node.TreeView.SelectedNode = node;
            Helper.UnlockUpdate(tcMain.SelectedTab);
        }

        private void PreLoadAction()
        {
            if (!graphicEditor.ReadOnly)
            {
                FlipGraphicEditorReadOnlyState();
            }
            graphicEditor.GameObject = null;
            pgeGameObject.SelectedObject = null;
            Application.DoEvents();
        }

        private void BtnSaveClick(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            SaveData();
            Cursor = Cursors.Default;
        }

        private void BtnReloadClick(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            PreLoadAction();
            ReloadData(true);
            Cursor = Cursors.Default;
        }

        private void TreeViewPathEditButtonClick(object sender, ButtonClickEventArgs e)
        {
            SetSelectedTreeNode(e.TreeNode);
        }

        private void TvPropertiesNodeSelect(object sender, TreeNode node)
        {
            pgeProperties.SelectedObject = tvProperties.SelectedObject;
            tvpeProperties.RefreshItems();
        }

        private void GePropertiesPropertyNameExistsCheck(object s, Atom atom, string newName,
                                                         out bool isExists)
        {
            isExists = atom as GameObjectPropertyClass != null
                           ? Helper.IsGameObjectPropertyClassNameExists(newName)
                           : Helper.IsGameObjectPropertyNameExists(newName);
        }

        private void TvFeaturesNodeSelect(object s, TreeNode node)
        {
            pgeFeatures.SelectedObject = tvFeatures.SelectedObject;
            tvpeFeatures.RefreshItems();
        }

        private void PgeFeaturesPropertyNameExistsCheck(object s, Atom atom, 
                                          string newName, out bool isExists)
        {
            isExists = atom as GameObjectFeatureClass != null
                           ? Helper.IsGameObjectFeatureClassNameExists(newName)
                           : Helper.IsGameObjectFeatureNameExists(newName);
        }

        private void TvAttributesNodeSelect(object s, TreeNode node)
        {
            pgeAttributes.SelectedObject = tvAttributes.SelectedObject;
            tvpeAttributes.RefreshItems();
        }

        private void PgeAttributesPropertyNameExistsCheck(object s, Atom atom, 
                                            string newName, out bool isExists)
        {
            isExists = Helper.IsGameObjectAttributeNameExists(newName);
        }

        private void TvGameObjectsNodeSelect(object s, TreeNode node)
        {
            if (!tvGameObjects.EditMode)
            {
                Helper.LockUpdate(graphicEditor);

                graphicEditor.GameObject = null;
                pgeGameObject.SelectedObject = null;
                if (tvGameObjects.SelectedGameObject != null)
                {
                    graphicEditor.GameObject = tvGameObjects.SelectedGameObject;
                    graphicEditor.ReadOnly = true;
                }
                else
                {
                    pgeGameObject.SelectedObject = tvGameObjects.SelectedObject;
                }

                Helper.UnlockUpdate(graphicEditor);
                tvpeGameObjects.RefreshItems();
            }
        }

        private void BtnBackupClick(object sender, EventArgs e)
        {
            CreateBackup(true);
        }

        private void BtnRestoreClick(object sender, EventArgs e)
        {
            using (RestoreBackupForm form = new RestoreBackupForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    RestoreBackup(form.BackupFileName);
                }
            }
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }

        private void SplitContainerSplitterMoved(object sender, SplitterEventArgs e)
        {
            SplitContainer sc = (SplitContainer)sender;
            if (sc != scAttributes)
            {
                scAttributes.SplitterDistance = sc.SplitterDistance;
            }
            if (sc != scProperties)
            {
                scProperties.SplitterDistance = sc.SplitterDistance;
            }
            if (sc != scFeatures)
            {
                scFeatures.SplitterDistance = sc.SplitterDistance;
            }
            if (sc != scGameObjects)
            {
                scGameObjects.SplitterDistance = sc.SplitterDistance;
            }
        }

        private void TvGameObjectsEditObject(object s, EditObjectArgs e)
        {
            tvGameObjectsEntities.EditMode = e.Editing;
            graphicEditor.ReadOnly = !e.Editing;
            pgeGameObject.EditMode = e.Editing;
        }

        private void FlipGraphicEditorReadOnlyState()
        {
            if (graphicEditor.GameObject != null)
            {
                graphicEditor.ReadOnly = !graphicEditor.ReadOnly;
                tvGameObjects.EditMode = !graphicEditor.ReadOnly;
                tvGameObjectsEntities.EditMode = !graphicEditor.ReadOnly;
                pgeGameObject.EditMode = !graphicEditor.ReadOnly;
            }
        }

        private void GraphicEditorDoubleClick(object sender, EventArgs args)
        {
            FlipGraphicEditorReadOnlyState();
        }

        private void GraphicEditorItemSelected(FlowchartItem item)
        {
            if (item != null)
            {
                ElementaryGameObject gameObject = item.UserObject as ElementaryGameObject;
                pgeGameObject.SelectedObject = gameObject;
            }
        }
    }
}