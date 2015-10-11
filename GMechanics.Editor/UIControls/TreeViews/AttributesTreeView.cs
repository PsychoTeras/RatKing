using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.ParentalGameObjectAttributeClasses;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Storages;
using GMechanics.Editor.Forms.GameObjectManage;
using GMechanics.Editor.Forms.InteractiveManage;
using GMechanics.Editor.Helpers;
using GMechanics.Editor.UIControls.Common;

namespace GMechanics.Editor.UIControls.TreeViews
{
    public sealed partial class AttributesTreeView : BaseTreeView
    {
        private readonly GameObjectAttributes _goaList;

        public AttributesTreeView()
        {
            InitializeComponent();
            tvAttributes.TreeViewNodeSorter = new TvNodeSorter();
            _goaList = GlobalStorage.Instance.GameObjectAttributes;
        }

        public override TreeViewEx TreeView
        {
            get { return tvAttributes; }
        }

        public override object SelectedObject
        {
            get
            {
                TreeNode node = SelectedGameObjectAttributeNode;
                return node != null ? node.Tag : null;
            }
        }

        private TreeNode SelectedGameObjectAttributeNode
        {
            get
            {
                TreeNode node = tvAttributes.SelectedNode;
                return node != null && node.Level > 0 ? node : null;
            }
        }

        private void TvAttributesAfterSelect(object sender, TreeViewEventArgs e)
        {
            btnDeleteAttribute.Enabled = btnEditAttribute.Enabled =
                btnManageInteractiveRecipients.Enabled = 
                SelectedGameObjectAttributeNode != null;
            InvokeNodeSelect(SelectedGameObjectAttributeNode);
        }

        public void SetObjectTreeNodeIcon(Atom atom, TreeNode rootNode, TreeNode node)
        {
            if (atom is ParentalGameObjectAttributeValue)
            {
                ParentalGameObjectAttributeValue obj = (ParentalGameObjectAttributeValue)atom;
                node.ImageIndex = node.SelectedImageIndex = obj.NestingLevel + 1;
            }
            else
            {
                node.ImageIndex = node.SelectedImageIndex = 1;
            }
        }

        internal static void AddNewIndexedIcon(int idx)
        {
            if (imageListAttributesTv.Images.Count - 2 < idx)
            {
                string strIdx = idx.ToString();
                Font font = new Font("Arial", 7, FontStyle.Bold);

                Image image = imageListAttributesTv.Images[1];
                Bitmap bmp = new Bitmap(image);
                
                Graphics graphic = Graphics.FromImage(bmp);
                graphic.SmoothingMode = SmoothingMode.AntiAlias;

                Color circleColor = Color.FromArgb(250, Color.RoyalBlue);
                graphic.FillEllipse(new SolidBrush(circleColor), new RectangleF(5, 5, 10, 10));
                graphic.DrawString(strIdx, font, new SolidBrush(Color.White), new PointF(6, 5));

                imageListAttributesTv.Images.Add(bmp);
            }
        }

        private void AddSubitems(TreeNode node, ParentalGameObjectAttributeValuesList values)
        {
            if (values != null)
            {
                foreach (ParentalGameObjectAttributeValue value in values)
                {
                    AddNewIndexedIcon(value.NestingLevel);
                    TreeNode childNode = Helper.AddGameEntitiesTreeViewNode(value, node,
                        SetObjectTreeNodeIcon);
                    AddSubitems(childNode, value.Values);
                }
            }
        }

        public void ReloadTreeView()
        {
            tvAttributes.BeginUpdate();

            sbMain.CancelSearch();

            TreeNode rootNode = tvAttributes.Nodes[0];
            rootNode.Nodes.Clear();

            foreach (ParentalGameObjectAttribute obj in _goaList.Values)
            {
                TreeNode node = Helper.AddGameEntitiesTreeViewNode(obj, rootNode, 
                    SetObjectTreeNodeIcon);
                AddSubitems(node, obj.Values);
            }

            tvAttributes.SelectedNode = rootNode;
            tvAttributes.SelectedNode.Expand();

            tvAttributes.EndUpdate();
            TvAttributesAfterSelect(tvAttributes, null);
        }

        private void BtnAddGameObjectAttributeClick(object sender, EventArgs e)
        {
            sbMain.CancelSearch();

            Cursor = Cursors.WaitCursor;
            using (GameObjectAttributeManageForm form = new GameObjectAttributeManageForm(
                false, false, string.Empty, string.Empty, null, 1, Helper.IsGameObjectAttributeNameExists))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ParentalGameObjectAttribute obj = new ParentalGameObjectAttribute(form.AttributeName,
                        form.Transcription, string.Empty, form.Values);
                    _goaList.Add(obj);

                    tvAttributes.BeginUpdate();
                    TreeNode rootNode = tvAttributes.Nodes[0];
                    TreeNode node = Helper.AddGameEntitiesTreeViewNode(obj, rootNode, SetObjectTreeNodeIcon);
                    AddSubitems(node, obj.Values);
                    tvAttributes.Sort();
                    tvAttributes.SelectedNode = node;
                    node.Expand();
                    tvAttributes.EndUpdate();

                    tvAttributes.SelectedNode = node;
                }
            }
            Cursor = Cursors.Default;
        }

        private void BtnEditAttributeClick(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            ParentalGameObjectAttribute attribute = SelectedObject as ParentalGameObjectAttribute;
            if (attribute != null)
            {
                using (GameObjectAttributeManageForm form = new GameObjectAttributeManageForm(
                    true, false, attribute.Name, attribute.Transcription, attribute.Values, 1, 
                    Helper.IsGameObjectAttributeNameExists))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        string oldName = attribute.Name;
                        ParentalGameObjectAttributeValuesList values = Helper.ApplyMatchingMap(
                            form.ValuesMatchingMap, form.Values);
                        attribute = new ParentalGameObjectAttribute(form.AttributeName, form.Transcription,
                            attribute.Description, values);

                        _goaList[oldName].Assign(attribute);
                        ChangeObjectName(oldName);
                        GlobalStorage.Instance.RemoveDestroyedItemsForGameObjects();

                        tvAttributes.BeginUpdate();
                        tvAttributes.SelectedNode.Nodes.Clear();
                        AddSubitems(tvAttributes.SelectedNode, values);
                        tvAttributes.SelectedNode.Tag = attribute;
                        tvAttributes.SelectedNode.Text = attribute.ShortDisplayName;
                        tvAttributes.EndUpdate();

                        TvAttributesAfterSelect(tvAttributes, null);
                    }
                }
            }

            ParentalGameObjectAttributeValue attributeValue = SelectedObject as ParentalGameObjectAttributeValue;
            if (attributeValue != null)
            {
                using (GameObjectAttributeManageForm form = new GameObjectAttributeManageForm(
                    true, true, string.Empty, string.Empty, attributeValue.Values, 
                    attributeValue.NestingLevel + 1, Helper.IsGameObjectAttributeNameExists))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        ParentalGameObjectAttributeValuesList values = Helper.ApplyMatchingMap(
                            form.ValuesMatchingMap, form.Values);
                        attributeValue.Values = values;
                        
                        tvAttributes.BeginUpdate();
                        tvAttributes.SelectedNode.Nodes.Clear();
                        AddSubitems(tvAttributes.SelectedNode, values);
                        if (tvAttributes.SelectedNode.Nodes.Count > 0)
                        {
                            tvAttributes.SelectedNode.Expand();
                        }
                        tvAttributes.EndUpdate();

                        TvAttributesAfterSelect(tvAttributes, null);
                    }
                }                
            }

            Cursor = Cursors.Default;
        }

        private void BtnDeleteAttributeClick(object sender, EventArgs e)
        {
            if (SelectedObject != null)
            {
                Atom atom = (Atom) SelectedObject;
                if (MessageBox.Show(string.Format("Are you sure you want to delete attribute '{0}'?",
                    atom.Name), "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == 
                    DialogResult.Yes)
                {
                    _goaList.Remove(atom.Name);
                    tvAttributes.Nodes.Remove(tvAttributes.SelectedNode);
                    GlobalStorage.Instance.RemoveDestroyedItemsForGameObjects();
                }
            }
        }

        private void TvAttributesKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None)
            {
                switch (e.KeyCode)
                {
                    case Keys.F5:
                        {
                            BtnAddGameObjectAttributeClick(this, null);
                            break;
                        }
                    case Keys.F6:
                        {
                            BtnEditAttributeClick(this, null);
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
                            BtnDeleteAttributeClick(this, null);
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
            _goaList.ChangeName(oldName);
        }

        private void TvAttributesMouseDown(object sender, MouseEventArgs e)
        {
            TreeNode node = tvAttributes.GetNodeAt(e.X, e.Y);
            if (node != null)
            {
                tvAttributes.SelectedNode = node;
                if (e.Clicks > 1)
                {
                    if (node.IsExpanded)
                        node.Collapse();
                    else
                        node.Expand();
                }
            }
        }

        public override void AfterUpdateObjectNameOrTranscription(Atom atom)
        {
            base.AfterUpdateObjectNameOrTranscription(atom);
            TvAttributesAfterSelect(tvAttributes, null);
        }

        private void TvAttributesNodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.X >= e.Node.Bounds.Left - 18)
            {
                BtnEditAttributeClick(sender, null);
            }
            else
            {
                if (e.Node.IsExpanded)
                    e.Node.Collapse();
                else
                    e.Node.Expand();
            }
        }

        private void BtnManageInteractiveRecipientsClick(object sender, EventArgs e)
        {
            Atom atom = (Atom)SelectedObject;
            if (atom != null)
            {
                InteractiveRecipientsList list = atom.InteractiveRecipients ?? new InteractiveRecipientsList();
                using (InteractiveRecipientsManageForm form = new InteractiveRecipientsManageForm(list, 
                    GameEntityTypesTable.TypeOf(atom)))
                {
                    form.ShowDialog();
                    list = (InteractiveRecipientsList) form.Result;
                    atom.InteractiveRecipients = list.Count == 0 ? null : list;
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