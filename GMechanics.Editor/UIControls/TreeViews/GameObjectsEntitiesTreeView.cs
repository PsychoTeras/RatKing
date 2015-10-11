using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Entities.GameObjectPropertyClasses;
using GMechanics.Core.Classes.Entities.ParentalGameObjectAttributeClasses;
using GMechanics.Core.Classes.Entities.ParentalGameObjectPropertyClasses;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Storages;
using GMechanics.Editor.Helpers;
using GMechanics.Editor.UIControls.Common;

namespace GMechanics.Editor.UIControls.TreeViews
{
    public partial class GameObjectsEntitiesTreeView : BaseTreeView
    {
        private readonly GameObjectAttributes _goaList;
        private readonly GameObjectProperties _gopList;
        private readonly GameObjectPropertiesClasses _gopcList;
        private readonly GameObjectFeatures _gofList;
        private readonly GameObjectFeaturesClasses _gofcList;

        private bool _editMode;

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
                }
            }
        }

        public GameObjectsEntitiesTreeView()
        {
            InitializeComponent();
            EndGameObjectEditing();
            tvObjectsEntities.TreeViewNodeSorter = new TvNodeSorter();
            _goaList = GlobalStorage.Instance.GameObjectAttributes;
            _gopList = GlobalStorage.Instance.GameObjectProperties;
            _gopcList = GlobalStorage.Instance.GameObjectPropertiesClasses;
            _gofList = GlobalStorage.Instance.GameObjectFeatures;
            _gofcList = GlobalStorage.Instance.GameObjectFeaturesClasses;
        }

        public override TreeViewEx TreeView
        {
            get { return tvObjectsEntities; }
        }

        public override object SelectedObject
        {
            get
            {
                TreeNode node = SelectedObjectNode;
                return node != null ? node.Tag : null;
            }
        }

        private TreeNode SelectedObjectNode
        {
            get
            {
                TreeNode node = tvObjectsEntities.SelectedNode;
                return node != null && node.Level >= 1 ? node : null;
            }
        }

        private void SetObjectTreeNodeIconFeature(Atom atom, TreeNode rootNode, TreeNode node)
        {
            GameObjectFeature gof = atom as GameObjectFeature;
            node.ImageIndex = node.SelectedImageIndex = (gof == null ? 1 : 3) +
                (gof != null && gof.Event != null && !gof.Event.IsEmpty ? 1 : 0);
        }

        private void SetObjectTreeNodeIconAttribute(Atom atom, TreeNode rootNode, TreeNode node)
        {
            if (atom is ParentalGameObjectAttributeValue)
            {
                ParentalGameObjectAttributeValue obj = (ParentalGameObjectAttributeValue)atom;
                node.ImageIndex = node.SelectedImageIndex = obj.NestingLevel + 5;
            }
            else
            {
                node.ImageIndex = node.SelectedImageIndex = 5;
            }
        }

        private void AddSubitemsAttribute(TreeNode node, ParentalGameObjectAttributeValuesList values)
        {
            if (values != null)
            {
                foreach (ParentalGameObjectAttributeValue value in values)
                {
                    AttributesTreeView.AddNewIndexedIcon(value.NestingLevel);
                    TreeNode childNode = Helper.AddGameEntitiesTreeViewNode(value, node,
                        SetObjectTreeNodeIconAttribute);
                    AddSubitemsAttribute(childNode, value.Values);
                }
            }
        }

        private void ReloadTreeViewAttributes()
        {
            foreach (ParentalGameObjectAttribute obj in _goaList.Values)
            {
                TreeNode node = Helper.AddGameEntitiesTreeViewNode(obj, tvObjectsEntities.Nodes[0],
                    SetObjectTreeNodeIconAttribute);
                AddSubitemsAttribute(node, obj.Values);
            }
        }

        private void ReloadTreeViewProperties()
        {
            //Create all properties classes nodes
            Hashtable gepcNodes = new Hashtable();
            foreach (GameObjectPropertyClass obj in _gopcList.Values)
            {
                TreeNode rootNode = tvObjectsEntities.Nodes[1];
                TreeNode node = Helper.AddGameEntitiesTreeViewNode(obj, rootNode);
                gepcNodes.Add(obj.Name, node);
            }

            //For each of property class node create assigned property node
            foreach (ParentalGameObjectProperty obj in _gopList.Values)
            {
                TreeNode parentNode = (TreeNode)gepcNodes[obj.PropertyClass.Name];
                if (parentNode != null)
                {
                    Helper.AddGameEntitiesTreeViewNode(obj, parentNode);
                }
            }
        }

        private void ReloadTreeViewFeatures()
        {
            //Create all features classes nodes
            Hashtable gepcNodes = new Hashtable();
            foreach (GameObjectFeatureClass obj in _gofcList.Values)
            {
                TreeNode rootNode = tvObjectsEntities.Nodes[2];
                TreeNode node = Helper.AddGameEntitiesTreeViewNode(obj, rootNode);
                gepcNodes.Add(obj.Name, node);
            }

            //For each of features class node create assigned feature node
            foreach (GameObjectFeature obj in _gofList.Values)
            {
                TreeNode parentNode = (TreeNode)gepcNodes[obj.FeatureClass.Name];
                if (parentNode != null)
                {
                    Helper.AddGameEntitiesTreeViewNode(obj, parentNode, SetObjectTreeNodeIconFeature);
                }
            }
        }

        private void BeginGameObjectEditing()
        {
            _editMode = true;
            tvObjectsEntities.BackColor = BackColor = Color.FromKnownColor(KnownColor.Window);
        }

        private void EndGameObjectEditing()
        {
            tvObjectsEntities.BackColor = BackColor = Color.White;
            _editMode = false;
        }

        private void SyncNodesImages()
        {
            ImageList.Images.Clear();
            foreach (Image img in PropertiesTreeView.imageListPropertiesTv.Images)
            {
                ImageList.Images.Add(img);
            }
            for (int i = 2; i < FeaturesTreeView.imageListFeaturesTv.Images.Count; i++)
            {
                Image img = FeaturesTreeView.imageListFeaturesTv.Images[i];
                ImageList.Images.Add(img);
            }
            for (int i = 1; i < AttributesTreeView.imageListAttributesTv.Images.Count; i++)
            {
                Image img = AttributesTreeView.imageListAttributesTv.Images[i];
                ImageList.Images.Add(img);
            }
        }

        public void ReloadTreeView()
        {
            tvObjectsEntities.BeginUpdate();

            sbMain.CancelSearch();

            SyncNodesImages();

            for (int i = 0; i < 3; i++)
            {
                TreeNode rootNode = tvObjectsEntities.Nodes[i];
                rootNode.Nodes.Clear();
            }

            ReloadTreeViewAttributes();
            ReloadTreeViewProperties();
            ReloadTreeViewFeatures();

            for (int i = 0; i < 3; i++)
            {
                tvObjectsEntities.Nodes[i].Expand();
            }
            tvObjectsEntities.SelectedNode = tvObjectsEntities.Nodes[0];

            tvObjectsEntities.EndUpdate();
        }

        private void TvObjectsAfterSelect(object sender, TreeViewEventArgs e)
        {
            InvokeNodeSelect(SelectedObjectNode);
        }

        private void ToolStripResize(object sender, EventArgs e)
        {
            sbMain.Left = toolStrip.Width - sbMain.Width - 2;
        }

        private void BtnRefreshClick(object sender, EventArgs e)
        {
            ReloadTreeView();
        }

        private void TvObjectsKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None)
            {
                switch (e.KeyCode)
                {
                    case Keys.F3:
                        {
                            BtnRefreshClick(this, null);
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

        private void TvObjectsMouseDown(object sender, MouseEventArgs e)
        {
            TreeNode node = tvObjectsEntities.GetNodeAt(e.X, e.Y);
            if (node != null)
            {
                tvObjectsEntities.SelectedNode = node;
            }
        }

        private void SbMainFilterNode(object s, TreeNode node, string filter, out bool canHide)
        {
            Atom atom = node.Tag as Atom;
            canHide = !string.IsNullOrEmpty(filter) &&
                node.Text.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) == -1 &&
                (atom == null || atom.Description.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) == -1);
        }

        private void TvObjectsItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode node = (TreeNode) e.Item;
            if (_editMode && e.Button == MouseButtons.Left && node.Tag != null)
            {
                Atom atom = (Atom) node.Tag;
                GameEntityType type = GameEntityTypesTable.TypeOf(atom);
                if (type == GameEntityType.ParentalGameObjectAttribute ||
                    type == GameEntityType.ParentalGameObjectAttributeValue ||
                    type == GameEntityType.ParentalGameObjectProperty ||
                    type == GameEntityType.GameObjectFeature)
                {
                    DoDragDrop(atom, DragDropEffects.Copy);
                }
            }
        }
    }
}
