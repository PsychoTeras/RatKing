using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Storages;
using GMechanics.Editor.Forms.GameObjectManage;
using GMechanics.Editor.Helpers;
using GMechanics.Editor.UIControls.Common;

namespace GMechanics.Editor.UIControls.TreeViews
{
    public partial class GameObjectsTreeView : BaseTreeView
    {
        public delegate void EditObjectHandler(object s, EditObjectArgs e);

        private readonly GameObjectGroups _gogList;
        private readonly ElementaryGameObjectGroups _egogList;
        private readonly GameObjects _goList;
        private readonly ElementaryGameObjects _egoList;

        private bool _editMode;
        private TreeNode _editingObjectNode;

        [Category("Action")]
        public event EditObjectHandler EditObject;

        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                if (_editMode != value)
                {
                    if (value)
                    {
                        BeginGameObjectEditing();
                    }
                    else
                    {
                        EndGameObjectEditing();
                    }
                    TvGameObjectsAfterSelect(this, null);
                    if (EditObject != null)
                    {
                        EditObjectArgs args = new EditObjectArgs();
                        args.Editing = _editMode;
                        args.GameObject = SelectedObject;
                        args.GameObjectType = SelectedGameObjectNodeType;
                        EditObject(this, args);
                    }
                }
            }
        }

        public GameObjectsTreeView()
        {
            InitializeComponent();
            EndGameObjectEditing();
            tvGameObjects.TreeViewNodeSorter = new TvNodeSorterGameObjects();
            _gogList = GlobalStorage.Instance.GameObjectGroups;
            _egogList = GlobalStorage.Instance.ElementaryGameObjectGroups;
            _goList = GlobalStorage.Instance.GameObjects;
            _egoList = GlobalStorage.Instance.ElementaryGameObjects;
        }

        public override object SelectedObject
        {
            get
            {
                TreeNode node = tvGameObjects.SelectedNode;
                return node != null ? node.Tag : null;
            }
        }

        public override TreeViewEx TreeView
        {
            get { return tvGameObjects; }
        }

        private bool RootNodeSelected
        {
            get
            {
                return tvGameObjects.SelectedNode != null &&
                       tvGameObjects.SelectedNode.Level == 0;
            }
        }

        private bool ChildNodeSelected
        {
            get
            {
                return tvGameObjects.SelectedNode != null &&
                       tvGameObjects.SelectedNode.Level > 0;
            }
        }

        private TreeNode SelectedObjectNode
        {
            get { return ChildNodeSelected ? tvGameObjects.SelectedNode : null; }
        }

        public GameEntityType SelectedGameObjectNodeType
        {
            get
            {
                TreeNode node = tvGameObjects.SelectedNode;
                if (node != null)
                {
                    while (node.Parent != null)
                    {
                        node = node.Parent;
                    }
                    return node.Index == 0
                               ? GameEntityType.ElementaryGameObject
                               : GameEntityType.GameObject;
                }
                return GameEntityType.Unknown;
            }
        }

        public TreeNode SelectedGameObjectGroupNode
        {
            get
            {
                TreeNode node = tvGameObjects.SelectedNode;
                if (node != null)
                {
                    return node.Tag is GameObjectGroup || node.Parent == null
                               ? node
                               : node.Parent.Tag is GameObjectGroup
                                     ? node.Parent
                                     : null;
                }
                return null;
            }
        }

        public GameObjectGroup SelectedGameObjectGroup
        {
            get
            {
                TreeNode node = SelectedGameObjectGroupNode;
                return node != null ? node.Tag as GameObjectGroup : null;
            }
        }

        public ElementaryGameObject SelectedGameObject
        {
            get
            {
                TreeNode node = tvGameObjects.SelectedNode;
                if (node != null)
                {
                    return node.Tag as GameObject ?? node.Tag as ElementaryGameObject;
                }
                return null;
            }
        }

        private void SetObjectTreeNodeIconElementaryGroup(Atom atom, TreeNode rootNode, TreeNode node)
        {
            node.ImageIndex = node.SelectedImageIndex = 1;
        }

        private void SetObjectTreeNodeIconGroup(Atom atom, TreeNode rootNode, TreeNode node)
        {
            node.ImageIndex = node.SelectedImageIndex = 5;
        }

        private void SetObjectTreeNodeIconElementary(Atom atom, TreeNode rootNode, TreeNode node)
        {
            node.ImageIndex = node.SelectedImageIndex = 2;
        }

        private void SetObjectTreeNodeIcon(Atom atom, TreeNode rootNode, TreeNode node)
        {
            node.ImageIndex = node.SelectedImageIndex = 6;
        }

        private void AddElementarySubGroups(TreeNode node, GameObjectGroup obj)
        {
            IEnumerable e = _egogList.Values.Where(g => g.Parent == obj);
            foreach (GameObjectGroup childObj in e)
            {
                TreeNode childNode = Helper.AddGameEntitiesTreeViewNode(childObj, node,
                    SetObjectTreeNodeIconElementaryGroup);
                foreach (ElementaryGameObject gameObject in childObj.GameObjects)
                {
                    Helper.AddGameEntitiesTreeViewNode(gameObject, childNode, 
                        SetObjectTreeNodeIconElementary);
                }
                AddElementarySubGroups(childNode, childObj);
            }
        }

        private void AddSubGroups(TreeNode node, GameObjectGroup obj)
        {
            IEnumerable e = _gogList.Values.Where(g => g.Parent == obj);
            foreach (GameObjectGroup childObj in e)
            {
                TreeNode childNode = Helper.AddGameEntitiesTreeViewNode(childObj, node, 
                    SetObjectTreeNodeIconGroup);
                foreach (ElementaryGameObject o in childObj.GameObjects)
                {
                    GameObject gameObject = (GameObject) o;
                    Helper.AddGameEntitiesTreeViewNode(gameObject, childNode,
                        SetObjectTreeNodeIcon);
                }
                AddSubGroups(childNode, childObj);
            }
        }

        public void ReloadTreeView()
        {
            tvGameObjects.BeginUpdate();

            TreeNode rootNode = tvGameObjects.Nodes[0];
            rootNode.Nodes.Clear();
            IEnumerable e = _egogList.Values.Where(g => g.Parent == null);
            foreach (GameObjectGroup obj in e)
            {
                TreeNode node = Helper.AddGameEntitiesTreeViewNode(obj, rootNode,
                    SetObjectTreeNodeIconElementaryGroup);
                foreach (ElementaryGameObject gameObject in obj.GameObjects)
                {
                    Helper.AddGameEntitiesTreeViewNode(gameObject, node,
                        SetObjectTreeNodeIconElementary);
                }
                AddElementarySubGroups(node, obj);
            }
            rootNode.Expand();

            rootNode = tvGameObjects.Nodes[1];
            rootNode.Nodes.Clear();
            e = _gogList.Values.Where(g => g.Parent == null);
            foreach (GameObjectGroup obj in e)
            {
                TreeNode node = Helper.AddGameEntitiesTreeViewNode(obj, rootNode,
                    SetObjectTreeNodeIconGroup);
                foreach (ElementaryGameObject o in obj.GameObjects)
                {
                    GameObject gameObject = (GameObject) o;
                    Helper.AddGameEntitiesTreeViewNode(gameObject, node,
                        SetObjectTreeNodeIcon);
                }
                AddSubGroups(node, obj);
            }
            rootNode.Expand();

            tvGameObjects.EndUpdate();
            tvGameObjects.SelectedNode = tvGameObjects.Nodes[0];
        }

        private void ToolStripResize(object sender, EventArgs e)
        {
            sbMain.Left = toolStrip.Width - sbMain.Width - 2;
        }

        private void SbMainFilterNode(object s, TreeNode node, string filter, out bool canHide)
        {
            Atom atom = node.Tag as Atom;
            canHide = !string.IsNullOrEmpty(filter) &&
                node.Text.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) == -1 &&
                (atom == null || atom.Description.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) == -1);

        }

        private void TvGameObjectsAfterSelect(object sender, TreeViewEventArgs e)
        {
            btnAddGameObjectGroup.Enabled = (SelectedGameObjectGroup != null ||
                RootNodeSelected) && !_editMode;
            btnAddGameObject.Enabled = btnDelete.Enabled = ChildNodeSelected && 
                !_editMode;
            btnEditGameObject.Enabled = SelectedGameObject != null || _editMode;

            bool isEl = SelectedGameObjectNodeType == GameEntityType.ElementaryGameObject;
            string el = isEl ? " elementary" : string.Empty;

            btnAddGameObjectGroup.ToolTipText = string.Format("Add new{0} object group (F4)", el);
            btnAddGameObject.ToolTipText = string.Format("Add new{0} game object (F5)", el);
            btnEditGameObject.ToolTipText = string.Format("Edit{0} game object (F6)", el);

            btnAddGameObject.Image = imageListGameObjectsTv.Images[isEl ? 3 : 7];
            btnEditGameObject.Image = imageListGameObjectsTv.Images[_editMode ? 9 : isEl ? 4 : 8];

            if (e != null)
            {
                InvokeNodeSelect(SelectedObjectNode);
            }
        }

        public bool IsElementaryGameObjectGroupNameExists(string groupPath)
        {
            GameObjectGroup group = SelectedGameObjectGroup;
            string path = Path.Combine(group != null ? group.Path : "", groupPath);
            return Helper.IsElementaryGameObjectGroupNameExists(path);
        }

        public bool IsGameObjectGroupNameExists(string groupPath)
        {
            GameObjectGroup group = SelectedGameObjectGroup;
            string path = Path.Combine(group != null ? group.Path : "", groupPath);
            return Helper.IsGameObjectGroupNameExists(path);
        }

        private void BtnAddGameObjectGroupClick(object sender, EventArgs e)
        {
            if (_editMode)
            {
                return;
            }

            sbMain.CancelSearch();
            Cursor = Cursors.WaitCursor;

            GameEntityType groupType = SelectedGameObjectNodeType;
            string gameObjectType = groupType == GameEntityType.ElementaryGameObject
                                        ? "Elementary game object"
                                        : "Game object";

            GameObjectGroupManageForm.IsGameObjectGroupNameExistsCheckHandler handler =
                groupType == GameEntityType.ElementaryGameObject
                    ? (GameObjectGroupManageForm.IsGameObjectGroupNameExistsCheckHandler)
                      IsElementaryGameObjectGroupNameExists
                    : IsGameObjectGroupNameExists;

            using (GameObjectGroupManageForm form = new GameObjectGroupManageForm(
                gameObjectType, false, string.Empty, string.Empty, handler))
            {

                if (form.ShowDialog() == DialogResult.OK)
                {
                    GameObjectGroup obj = new GameObjectGroup(form.GroupName,
                        form.Transcription, string.Empty, SelectedGameObjectGroup);

                    Helper.SetObjectTreeNodeIconHandler iconHandler = null;

                    switch (groupType)
                    {
                        case GameEntityType.ElementaryGameObject:
                            {
                                _egogList.Add(obj);
                                iconHandler = SetObjectTreeNodeIconElementaryGroup;
                                break;
                            }
                        case GameEntityType.GameObject:
                            {
                                _gogList.Add(obj);
                                iconHandler = SetObjectTreeNodeIconGroup;
                                break;
                            }
                    }
                    
                    //Add group node
                    tvGameObjects.BeginUpdate();
                    TreeNode rootNode = SelectedGameObjectGroupNode;
                    TreeNode node = Helper.AddGameEntitiesTreeViewNode(obj, 
                        rootNode, iconHandler);
                    tvGameObjects.Sort();
                    tvGameObjects.SelectedNode = node;
                    tvGameObjects.EndUpdate();
                }
            }

            Cursor = Cursors.Default;
        }

        private void BtnAddGameObjectClick(object sender, EventArgs e)
        {
            if (_editMode)
            {
                return;
            }

            sbMain.CancelSearch();
            Cursor = Cursors.WaitCursor;

            GameEntityType groupType = SelectedGameObjectNodeType;
            string gameObjectType = groupType == GameEntityType.ElementaryGameObject
                                        ? "Elementary game object"
                                        : "Game object";

            GameObjectManageForm.IsGameObjectNameExistsCheckHandler handler =
                groupType == GameEntityType.ElementaryGameObject
                    ? (GameObjectManageForm.IsGameObjectNameExistsCheckHandler)
                      Helper.IsElementaryGameObjectNameExists
                    : Helper.IsGameObjectNameExists;

            using (GameObjectManageForm form = new GameObjectManageForm(
                gameObjectType, false, string.Empty, string.Empty, handler))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    object obj = null;
                    Helper.SetObjectTreeNodeIconHandler iconHandler = null;

                    switch (groupType)
                    {
                        case GameEntityType.ElementaryGameObject:
                            {
                                obj = new ElementaryGameObject(form.GameObjectName,
                                    form.Transcription, string.Empty);
                                _egoList.Add((ElementaryGameObject)obj);
                                iconHandler = SetObjectTreeNodeIconElementary;
                                break;
                            }
                        case GameEntityType.GameObject:
                            {
                                obj = new GameObject(form.GameObjectName, form.Transcription,
                                    string.Empty);
                                _goList.Add((GameObject)obj);
                                iconHandler = SetObjectTreeNodeIcon;
                                break;
                            }
                    }

                    SelectedGameObjectGroup.AddGameObject((ElementaryGameObject) obj);

                    //Add group item node
                    tvGameObjects.BeginUpdate();
                    TreeNode rootNode = SelectedGameObjectGroupNode;
                    TreeNode node = Helper.AddGameEntitiesTreeViewNode((Atom)obj,
                        rootNode, iconHandler);
                    tvGameObjects.Sort();
                    tvGameObjects.SelectedNode = node;
                    tvGameObjects.EndUpdate();

                    EditMode = true;
                }
            }

            Cursor = Cursors.Default;
        }

        private void BeginGameObjectEditing()
        {
            _editMode = true;
            _editingObjectNode = SelectedObjectNode;
            tvGameObjects.BackColor = BackColor = Color.FromKnownColor(KnownColor.Window);
        }

        private void EndGameObjectEditing()
        {
            sbMain.CancelSearch();
            tvGameObjects.SelectedNode = _editingObjectNode;
            _editingObjectNode = null;
            tvGameObjects.BackColor = BackColor = Color.White;
            _editMode = false;
        }

        private void BtnEditGameObjectClick(object sender, EventArgs e)
        {
            if (SelectedGameObject != null || _editMode)
            {
                EditMode = !EditMode;
            }
        }

        private void BtnDeleteClick(object sender, EventArgs e)
        {
            if (!_editMode)
            {
                bool removed = SelectedGameObjectGroup != null || SelectedGameObject != null;
                if (SelectedGameObjectGroup != null && SelectedGameObject != null)
                {
                    if (MessageBox.Show(string.Format("Are you sure you want to delete game object '{0}'?",
                        SelectedGameObject.Name), "Confirmation", MessageBoxButtons.YesNoCancel, 
                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        SelectedGameObjectGroup.RemoveGameObject(SelectedGameObject);
                    }
                }
                else if (SelectedGameObjectGroup != null)
                {
                    if (MessageBox.Show(string.Format("Are you sure you want to delete game object group '{0}'?",
                        SelectedGameObjectGroup.Name), "Confirmation", MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (_gogList.ContainsValue(SelectedGameObjectGroup))
                        {
                            _gogList.Remove(SelectedGameObjectGroup);
                        }
                        else
                        {
                            _egogList.Remove(SelectedGameObjectGroup);
                        }
                    }
                }
                if (removed)
                {
                    TreeNode parent = tvGameObjects.SelectedNode.Parent;
                    tvGameObjects.SelectedNode.Remove();
                    tvGameObjects.SelectedNode = parent;
                }
            }
        }

        private void TvGameObjectsDoubleClick(object sender, EventArgs e)
        {
            if (SelectedGameObject != null && !_editMode)
            {
                BtnEditGameObjectClick(this, null);
            }
        }

        private void TvGameObjectsKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None)
            {
                switch (e.KeyCode)
                {
                    case Keys.F4:
                        {
                            BtnAddGameObjectGroupClick(this, null);
                            break;
                        }
                    case Keys.F5:
                        {
                            BtnAddGameObjectClick(this, null);
                            break;
                        }
                    case Keys.F6:
                        {
                            BtnEditGameObjectClick(this, null);
                            break;
                        }
                    case Keys.F8:
                    case Keys.Delete:
                        {
                            BtnDeleteClick(this, null);
                            break;
                        }
                    case Keys.Escape:
                        {
                            if (sbMain.Searching)
                            {
                                sbMain.CancelSearch();
                            }
                            else if (_editMode)
                            {
                                EditMode = false;
                            }
                            break;
                        }
                }
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Q)
            {
                sbMain.Focus();
            }
        }

        private void TvGameObjectsMouseDown(object sender, MouseEventArgs e)
        {
            TreeNode node = tvGameObjects.GetNodeAt(e.X, e.Y);
            if (node != null)
            {
                tvGameObjects.SelectedNode = node;
            }
        }

        private void TvGameObjectsItemDrag(object sender, ItemDragEventArgs e)
        {
            if (_editMode)
            {
                if (e.Button == MouseButtons.Left && SelectedGameObject != null)
                {
                    DoDragDrop(SelectedGameObject, DragDropEffects.Copy);
                }
            }
        }
    }

    public class EditObjectArgs
    {
        public object GameObject;
        public GameEntityType GameObjectType;
        public bool Editing;
    }
}
