using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.GameObjectPropertyClasses;
using GMechanics.Core.Classes.Entities.ParentalGameObjectPropertyClasses;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Storages;
using GMechanics.Editor.Forms.GameObjectManage;
using GMechanics.Editor.Forms.InteractiveManage;
using GMechanics.Editor.Helpers;
using GMechanics.Editor.UIControls.Common;

namespace GMechanics.Editor.UIControls.TreeViews
{
    public sealed partial class PropertiesTreeView : BaseTreeView
    {
        private readonly GameObjectProperties _gopList;
        private readonly GameObjectPropertiesClasses _gopcList;

        public PropertiesTreeView()
        {
            InitializeComponent();
            tvProperties.TreeViewNodeSorter = new TvNodeSorter();
            _gopList = GlobalStorage.Instance.GameObjectProperties;
            _gopcList = GlobalStorage.Instance.GameObjectPropertiesClasses;
        }

        public override TreeViewEx TreeView
        {
            get { return tvProperties; }
        }

        public override object SelectedObject
        {
            get
            {
                TreeNode node = SelectedGameObjectPropertyNode ?? SelectedGameObjectPropertyClassNode;
                return node != null ? node.Tag : null;
            }
        }

        private TreeNode SelectedGameObjectPropertyNode
        {
            get
            {
                TreeNode node = tvProperties.SelectedNode;
                return node != null && node.Level == 2 ? node : null;
            }
        }

        private TreeNode SelectedGameObjectPropertyClassNode
        {
            get
            {
                TreeNode node = tvProperties.SelectedNode;
                if (node != null)
                {
                    return node.Level == 1 ? node : node.Parent;
                }
                return null;
            }
        }

        private string SelectedGameObjectPropertyClassName
        {
            get
            {
                TreeNode node = SelectedGameObjectPropertyClassNode;
                return node != null ? ((Atom) node.Tag).Name : string.Empty;
            }
        }

        private TreeNode GetGameObjectPropertyClassNode(string className)
        {
            GameObjectPropertyClass obj = _gopcList[className];
            if (obj != null)
            {
                TreeNode node = tvProperties.Nodes[0].Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Tag == obj);
                return node;
            }
            return null;
        }

        public void ReloadTreeView()
        {
            tvProperties.BeginUpdate();

            //Clear filter
            sbMain.CancelSearch();

            //Clear all nodes
            tvProperties.Nodes[0].Nodes.Clear();
 
            //Create all properties classes nodes
            Hashtable gepcNodes = new Hashtable();
            foreach (GameObjectPropertyClass obj in _gopcList.Values)
            {
                TreeNode rootNode = tvProperties.Nodes[0];
                TreeNode node = Helper.AddGameEntitiesTreeViewNode(obj, rootNode);
                gepcNodes.Add(obj.Name, node);
            }

            //For each of property class node create assigned property node
            foreach (ParentalGameObjectProperty obj in _gopList.Values)
            {
                TreeNode parentNode = (TreeNode) gepcNodes[obj.PropertyClass.Name];
                if (parentNode != null)
                {
                    Helper.AddGameEntitiesTreeViewNode(obj, parentNode);
                }
            }

            tvProperties.SelectedNode = tvProperties.Nodes[0];
            tvProperties.SelectedNode.Expand();

            tvProperties.EndUpdate();
            TvPropertiesAfterSelect(tvProperties, null);
        }

        private void TvPropertiesAfterSelect(object sender, TreeViewEventArgs e)
        {
            btnDeleteFeature.Enabled = btnAddProperty.Enabled =
                SelectedGameObjectPropertyClassNode != null;
            btnManageInteractiveRecipients.Enabled = SelectedGameObjectPropertyNode != null;
            InvokeNodeSelect(SelectedGameObjectPropertyClassNode);
        }

        private void BtnAddGameObjectPropertyClassClick(object sender, EventArgs e)
        {
            sbMain.CancelSearch();
            Cursor = Cursors.WaitCursor;

            using (GameObjectEntityClassManageForm form = new GameObjectEntityClassManageForm(
                "Property", false, string.Empty, string.Empty, Helper.IsGameObjectPropertyClassNameExists))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    //Add new property class
                    GameObjectPropertyClass obj = new GameObjectPropertyClass(form.ClassName,
                                                          form.Transcription, string.Empty);
                    _gopcList.Add(obj);

                    //Add property class node
                    tvProperties.BeginUpdate();
                    TreeNode rootNode = tvProperties.Nodes[0];
                    TreeNode node = Helper.AddGameEntitiesTreeViewNode(obj, rootNode);
                    tvProperties.Sort();
                    tvProperties.SelectedNode = node;
                    tvProperties.EndUpdate();
                }
            }

            Cursor = Cursors.Default;
        }

        private void BtnAddGameObjectPropertyClick(object sender, EventArgs e)
        {
            sbMain.CancelSearch();
            Cursor = Cursors.WaitCursor;

            using (GameObjectEntityManageForm form = new GameObjectEntityManageForm(
                "Property", false, string.Empty, string.Empty, _gopcList.AtomsNamesList,
                SelectedGameObjectPropertyClassName, Helper.IsGameObjectPropertyNameExists))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    //Add new property
                    GameObjectPropertyClass ownerClass = _gopcList[form.ClassName];
                    ParentalGameObjectProperty obj = new ParentalGameObjectProperty(form.EntityName,
                        form.Transcription, string.Empty, 0, ownerClass);
                    _gopList.Add(obj);

                    //Add property node
                    TreeNode parentNode = GetGameObjectPropertyClassNode(form.ClassName);
                    if (parentNode != null)
                    {
                        tvProperties.BeginUpdate();
                        TreeNode node = Helper.AddGameEntitiesTreeViewNode(obj, parentNode);
                        tvProperties.Sort();
                        tvProperties.SelectedNode = node;
                        tvProperties.EndUpdate();
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
                bool isFeatureClass = SelectedObject is GameObjectPropertyClass;
                string objType = isFeatureClass ? "feature class" : "feature";
                if (MessageBox.Show(string.Format("Are you sure you want to delete {0} '{1}'?", objType,
                                                  atom.Name), "Confirmation", MessageBoxButtons.YesNoCancel,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (isFeatureClass)
                    {
                        List<ParentalGameObjectProperty> list = _gopList.Values.
                            Where(gof => gof.PropertyClass.Name == atom.Name).ToList();
                        for (int i = list.Count - 1; i >= 0; i--)
                        {
                            ParentalGameObjectProperty gop = list[i];
                            _gopList.Remove(gop.Name);
                        }
                        _gopcList.Remove(atom.Name);
                    }
                    else
                    {
                        _gopList.Remove(atom.Name);
                    }
                    tvProperties.Nodes.Remove(tvProperties.SelectedNode);
                }
            }
        }

        private void TvPropertiesKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None)
            {
                switch (e.KeyCode)
                {
                    case Keys.F4:
                        {
                            BtnAddGameObjectPropertyClassClick(this, null);
                            break;
                        }
                    case Keys.F5:
                        {
                            BtnAddGameObjectPropertyClick(this, null);
                            break;
                        }
                    case Keys.F6:
                        {
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
            if (atom is GameObjectPropertyClass)
            {
                _gopcList.ChangeName(oldName);
            }
            else
            {
                _gopList.ChangeName(oldName);
            }
        }

        public override void AfterUpdateObjectNameOrTranscription(Atom atom)
        {
            base.AfterUpdateObjectNameOrTranscription(atom);
            TvPropertiesAfterSelect(tvProperties, null);
        }

        private void BtnManageInteractiveRecipientsClick(object sender, EventArgs e)
        {
            if (SelectedGameObjectPropertyNode != null)
            {
                ParentalGameObjectProperty gop = (ParentalGameObjectProperty)SelectedObject;
                InteractiveRecipientsList list = gop.InteractiveRecipients ?? new InteractiveRecipientsList();
                using (InteractiveRecipientsManageForm form = new InteractiveRecipientsManageForm(
                    list, GameEntityType.ParentalGameObjectProperty))
                {
                    form.ShowDialog();
                    list = (InteractiveRecipientsList)form.Result;
                    gop.InteractiveRecipients = list.Count == 0 ? null : list;
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