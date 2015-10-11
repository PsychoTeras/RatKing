using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.GameObjectEventClasses;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Storages;
using GMechanics.Editor.Forms.GameObjectManage;
using GMechanics.Editor.Forms.InteractiveManage;
using GMechanics.Editor.Forms.ScriptEditor;
using GMechanics.Editor.Helpers;
using GMechanics.Editor.UIControls.Common;

namespace GMechanics.Editor.UIControls.TreeViews
{
    public sealed partial class FeaturesTreeView : BaseTreeView
    {
        private readonly GameObjectFeatures _gofList;
        private readonly GameObjectFeaturesClasses _gofcList;

        public FeaturesTreeView()
        {
            InitializeComponent();
            tvFeatures.TreeViewNodeSorter = new TvNodeSorter();
            _gofList = GlobalStorage.Instance.GameObjectFeatures;
            _gofcList = GlobalStorage.Instance.GameObjectFeaturesClasses;
        }

        private void SetObjectTreeNodeIcon(Atom atom, TreeNode rootNode, TreeNode node)
        {
            GameObjectFeature gof = atom as GameObjectFeature;
            node.ImageIndex = node.SelectedImageIndex = (gof == null ? 1 : 2) +
                (gof != null && gof.Event != null && !gof.Event.IsEmpty ? 1 : 0);
        }

        public override TreeViewEx TreeView
        {
            get { return tvFeatures; }
        }

        public override object SelectedObject
        {
            get
            {
                TreeNode node = SelectedGameObjectFeatureNode ?? 
                    SelectedGameObjectFeatureClassNode;
                return node != null ? node.Tag : null;
            }
        }

        private TreeNode SelectedGameObjectFeatureNode
        {
            get
            {
                TreeNode node = tvFeatures.SelectedNode;
                return node != null && node.Level == 2 ? node : null;
            }
        }

        private TreeNode SelectedGameObjectFeatureClassNode
        {
            get
            {
                TreeNode node = tvFeatures.SelectedNode;
                if (node != null)
                {
                    return node.Level == 1 ? node : node.Parent;
                }
                return null;
            }
        }

        private string SelectedGameObjectFeatureClassName
        {
            get
            {
                TreeNode node = SelectedGameObjectFeatureClassNode;
                return node != null ? ((Atom) node.Tag).Name : string.Empty;
            }
        }

        private TreeNode GetGameObjectFeatureClassNode(string className)
        {
            GameObjectFeatureClass obj = _gofcList[className];
            if (obj != null)
            {
                TreeNode node = tvFeatures.Nodes[0].Nodes.Cast<TreeNode>().
                    FirstOrDefault(n => n.Tag == obj);
                return node;
            }
            return null;
        }

        public void ReloadTreeView()
        {
            tvFeatures.BeginUpdate();

            //Clear filter
            sbMain.CancelSearch();

            //Clear all nodes
            tvFeatures.Nodes[0].Nodes.Clear();

            //Create all features classes nodes
            Hashtable gepcNodes = new Hashtable();
            foreach (GameObjectFeatureClass obj in _gofcList.Values)
            {
                TreeNode rootNode = tvFeatures.Nodes[0];
                TreeNode node = Helper.AddGameEntitiesTreeViewNode(obj, rootNode);
                gepcNodes.Add(obj.Name, node);
            }

            //For each of features class node create assigned feature node
            foreach (GameObjectFeature obj in _gofList.Values)
            {
                TreeNode parentNode = (TreeNode) gepcNodes[obj.FeatureClass.Name];
                if (parentNode != null)
                {
                    Helper.AddGameEntitiesTreeViewNode(obj, parentNode, SetObjectTreeNodeIcon);
                }
            }

            tvFeatures.SelectedNode = tvFeatures.Nodes[0];
            tvFeatures.SelectedNode.Expand();

            tvFeatures.EndUpdate();
            TvFeaturesAfterSelect(tvFeatures, null);
        }

        private void TvFeaturesAfterSelect(object sender, TreeViewEventArgs e)
        {
            btnDeleteFeature.Enabled = btnAddFeature.Enabled =
                SelectedGameObjectFeatureClassNode != null;
            btnEditScript.Enabled = btnManageInteractiveRecipients.Enabled = 
                SelectedGameObjectFeatureNode != null; 
            InvokeNodeSelect(SelectedGameObjectFeatureClassNode);
        }

        private void BtnAddGameObjectFeatureClassClick(object sender, EventArgs e)
        {
            sbMain.CancelSearch();
            Cursor = Cursors.WaitCursor;

            using (GameObjectEntityClassManageForm form = new GameObjectEntityClassManageForm(
                "Feature", false, string.Empty, string.Empty, Helper.IsGameObjectFeatureClassNameExists))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    //Add new property class
                    GameObjectFeatureClass obj = new GameObjectFeatureClass(form.ClassName,
                        form.Transcription, string.Empty);
                    _gofcList.Add(obj);

                    //Add property class node
                    tvFeatures.BeginUpdate();
                    TreeNode rootNode = tvFeatures.Nodes[0];
                    TreeNode node = Helper.AddGameEntitiesTreeViewNode(obj, rootNode);
                    tvFeatures.Sort();
                    tvFeatures.SelectedNode = node;
                    tvFeatures.EndUpdate();
                }
            }
            Cursor = Cursors.Default;
        }

        private void BtnAddGameObjectFeatureClick(object sender, EventArgs e)
        {
            sbMain.CancelSearch();
            Cursor = Cursors.WaitCursor;
            using (GameObjectEntityManageForm form = new GameObjectEntityManageForm(
                "Feature", false, string.Empty, string.Empty, _gofcList.AtomsNamesList,
                SelectedGameObjectFeatureClassName, Helper.IsGameObjectFeatureNameExists))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    //Add new property
                    GameObjectFeatureClass ownerClass = _gofcList[form.ClassName];
                    GameObjectFeature obj = new GameObjectFeature(form.EntityName,
                                                    form.Transcription, string.Empty, ownerClass);
                    _gofList.Add(obj);

                    //Add property node
                    TreeNode parentNode = GetGameObjectFeatureClassNode(form.ClassName);
                    if (parentNode != null)
                    {
                        tvFeatures.BeginUpdate();
                        TreeNode node = Helper.AddGameEntitiesTreeViewNode(obj, parentNode, 
                            SetObjectTreeNodeIcon);
                        tvFeatures.Sort();
                        tvFeatures.SelectedNode = node;
                        tvFeatures.EndUpdate();
                    }
                }
            }
            Cursor = Cursors.Default;
        }

        private void BtnDeleteFeatureClick(object sender, EventArgs e)
        {
            if (SelectedObject != null)
            {
                Atom atom = (Atom) SelectedObject;
                bool isFeatureClass = SelectedObject is GameObjectFeatureClass;
                string objType = isFeatureClass ? "feature class" : "feature";
                if (MessageBox.Show(string.Format("Are you sure you want to delete {0} '{1}'?", objType,
                                                  atom.Name), "Confirmation", MessageBoxButtons.YesNoCancel,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (isFeatureClass)
                    {
                        List<GameObjectFeature> list = _gofList.Values.Where(
                            gof => gof.FeatureClass.Name == atom.Name).ToList();
                        for (int i = list.Count - 1; i >= 0; i--)
                        {
                            Atom gof = list[i];
                            _gofList.Remove(gof.Name);
                        }
                        _gofcList.Remove(atom.Name);
                    }
                    else
                    {
                        _gofList.Remove(atom.Name);
                    }
                    tvFeatures.Nodes.Remove(tvFeatures.SelectedNode);
                }
            }
        }

        private void TvFeaturesKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None)
            {
                switch (e.KeyCode)
                {
                    case Keys.F4:
                        {
                            BtnAddGameObjectFeatureClassClick(this, null);
                            break;
                        }
                    case Keys.F5:
                        {
                            BtnAddGameObjectFeatureClick(this, null);
                            break;
                        }
                    case Keys.F6:
                        {
                            BtnEditScriptClick(this, null);
                            break;
                        }
                    case Keys.F7:
                        {
                            BtnManageInteractiveRecipientsClick(this, null);
                            break;
                        }
                    case Keys.F8:
                    case Keys.Delete:
                        {
                            BtnDeleteFeatureClick(this, null);
                            break;
                        }
                    case Keys.Escape:
                        {
                            sbMain.CancelSearch();
                            break;
                        }
                }
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Q)
            {
                sbMain.Focus();
            }
        }

        public override void ChangeObjectName(string oldName)
        {
            Atom atom = (Atom) SelectedObject;
            if (atom is GameObjectFeatureClass)
            {
                _gofcList.ChangeName(oldName);
            }
            else
            {
                _gofList.ChangeName(oldName);
            }
        }

        private void BtnEditScriptClick(object sender, EventArgs e)
        {
            GameObjectFeature gof = SelectedObject as GameObjectFeature;
            if (gof != null)
            {
                Cursor = Cursors.WaitCursor;
                gof.Event = gof.Event ?? new GameObjectEvent();
                using (ScriptEditorForm form = new ScriptEditorForm(gof, gof.Event))
                {
                    form.ShowDialog();
                    if (gof.Event.IsEmpty)
                    {
                        gof.Event = null;
                    }
                    OnScriptChanged();
                }
                TvFeaturesAfterSelect(tvFeatures, null);
                Cursor = Cursors.Default;
            }
        }

        public void OnScriptChanged()
        {
            SetObjectTreeNodeIcon((Atom) SelectedObject, SelectedGameObjectFeatureClassNode,
                                  SelectedGameObjectFeatureNode);
        }

        public override void AfterUpdateObjectNameOrTranscription(Atom atom)
        {
            base.AfterUpdateObjectNameOrTranscription(atom);
            TvFeaturesAfterSelect(tvFeatures, null);
        }

        private void TvFeaturesNodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.X >= e.Node.Bounds.Left - 18 && e.Node.Tag is GameObjectFeature)
            {
                BtnEditScriptClick(sender, e);
            }
        }

        private void BtnManageInteractiveRecipientsClick(object sender, EventArgs e)
        {
            if (SelectedGameObjectFeatureNode != null)
            {
                GameObjectFeature gof = (GameObjectFeature)SelectedObject;
                InteractiveRecipientsList list = gof.InteractiveRecipients ?? new InteractiveRecipientsList();
                using (InteractiveRecipientsManageForm form = new InteractiveRecipientsManageForm(
                    list, GameEntityType.GameObjectFeature))
                {
                    form.ShowDialog();
                    list = (InteractiveRecipientsList)form.Result;
                    gof.InteractiveRecipients = list.Count == 0 ? null : list;
                }
            }
        }

        private void ToolStripResize(object sender, EventArgs e)
        {
            sbMain.Left = toolStrip.Width - sbMain.Width - 2;
        }

        private void SbMainFilterNode(object s, TreeNode node, string filter, out bool canHide)
        {
            Atom atom = (Atom)node.Tag;
            canHide = !string.IsNullOrEmpty(filter) &&
                node.Text.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) == -1 &&
                atom.Description.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) == -1;
        }
    }
}