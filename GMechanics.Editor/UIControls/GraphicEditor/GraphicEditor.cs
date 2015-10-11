using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Interfaces;
using GMechanics.Editor.Data;
using GMechanics.Editor.Helpers;
using GMechanics.FlowchartControl;
using GMechanics.FlowchartControl.ItemElements;

namespace GMechanics.Editor.UIControls.GraphicEditor
{
    public partial class GraphicEditor : UserControl
    {

#region Events and delegates

        [Category("Action")]
        public new event EventHandler DoubleClick;

        [Category("Action")]
        public event Flowchart.OnItemSelected ItemSelected;

#endregion

#region Private members

        private readonly LocalStorage _localStorage = LocalStorage.Instance;

        private ElementaryGameObject _gameObject;
        private static readonly Size DefaultItemSize = new Size(230, 200);

#endregion

#region Properties

        public Flowchart Flowchart { get; private set; }

        public ElementaryGameObject GameObject
        {
            get { return _gameObject; }
            set
            {
                if (_gameObject != value)
                {
                    _gameObject = value;
                    RecreateFlowcharItems();
                }
            }
        }

        public bool ReadOnly
        {
            get { return Flowchart.ReadOnly; }
            set { Flowchart.ReadOnly = value; }
        }

#endregion

#region Class functions

        public GraphicEditor()
        {
            InitializeComponent();
            Flowchart = new Flowchart { Parent = this, Dock = DockStyle.Fill };
            Flowchart.ReadOnly = true;
        }

        private void GraphicEditorLoad(object sender, EventArgs e)
        {
            Flowchart.DragEnter += FlowchartDragEnter;
            Flowchart.DragDrop += FlowchartDragDrop;
            Flowchart.ItemDragEnter += FlowchartItemDragEnter;
            Flowchart.ItemDragDrop += FlowchartItemDragDrop;
            Flowchart.MouseDown += FlowchartMouseDown;
            Flowchart.ItemResized += FlowchartItemLocationChanged;
            Flowchart.ItemMoved += FlowchartItemLocationChanged;
            Flowchart.ItemLinking += FlowchartItemLinking;
            Flowchart.ItemLinked += FlowchartItemLinked;
            Flowchart.ItemUnlinked += FlowchartItemUnlinked;
            Flowchart.ItemElementMouseUp += FlowChartItemElementMouseUp;
            Flowchart.ItemMouseUp += FlowchartItemMouseUp;
            Flowchart.ItemSelected += FlowchartItemSelected;
        }

        private void FlowchartItemSelected(FlowchartItem item)
        {
            if (ItemSelected != null)
            {
                ItemSelected(item);
            }
        }

        private void FlowchartItemMouseUp(FlowchartItem item, MouseEventArgs e)
        {
            FlowChartItemElementMouseUp(item, null, e);
        }

        private void FlowChartItemElementMouseUp(FlowchartItem item, 
            IItemElement itemElement, MouseEventArgs e)
        {
            if (!ReadOnly && e.Button == MouseButtons.Right)
            {
                Point pos = item.PointToScreen(e.Location);
                menuFlowchartItemElement.Show(pos);
            }
        }

        private void MenuFlowchartItemElementOpening(object sender, CancelEventArgs e)
        {
            FlowchartItem item = Flowchart.SelectedItem;
            IItemElement element = item.SelectedItemElement as ItemGroupElement;
            ElementaryGameObject gameObject = (ElementaryGameObject)item.UserObject;

            bool showMenu = menuItemRemoveItem.Visible = gameObject != GameObject;
            if (showMenu)
            {
                if (item.UserObject is ElementaryGameObject)
                {
                    menuItemRemoveItem.Image = ilMenu.Images[3];
                }
                else
                {
                    menuItemRemoveItem.Image = ilMenu.Images[4];
                }
                menuItemRemoveItem.Text = string.Format("Remove '{0}' game object",
                                                        gameObject.Name);
            }

            menuItemRemoveElement.Visible = toolStripMenuItem1.Visible = false;
            if (element != null)
            {
                string objectType = "element";
                IClassAsAtom addedObject = (IClassAsAtom)element.UserObject;
                GameEntityType type = GameEntityTypesTable.TypeOf(addedObject);

                bool canRemoveElement = false;

                switch (type)
                {
                    case GameEntityType.GameObjectAttribute:
                        {
                            canRemoveElement = gameObject.GetAttribute(addedObject.ClassAsAtom.Name,
                                false) != null;
                            menuItemRemoveElement.Image = ilMenu.Images[0];
                            objectType = "attribute";
                            break;
                        }
                    case GameEntityType.GameObjectAttributeValue:
                        {
                            string attributeName = Helper.GetAttributeValueParent(
                                addedObject.ClassAsAtom).Name;
                            canRemoveElement = gameObject.GetAttributeValue(attributeName,
                                addedObject.ClassAsAtom.Name, false) != null;
                            menuItemRemoveElement.Image = ilMenu.Images[0];
                            objectType = "attribute value";
                            break;
                        }
                    case GameEntityType.GameObjectProperty:
                        {
                            canRemoveElement = gameObject.GetProperty(addedObject.ClassAsAtom.Name,
                                false) != null;
                            menuItemRemoveElement.Image = ilMenu.Images[1];
                            objectType = "property";
                            break;
                        }
                    case GameEntityType.GameObjectFeature:
                        {
                            canRemoveElement = gameObject.GetFeature(addedObject.ClassAsAtom.Name,
                                false) != null;
                            menuItemRemoveElement.Image = ilMenu.Images[2];
                            objectType = "feature";
                            break;
                        }
                }
                menuItemRemoveElement.Text = string.Format("Remove '{0}' {1}",
                    addedObject.ClassAsAtom.Name, objectType);
                showMenu |= menuItemRemoveElement.Visible = canRemoveElement;
                toolStripMenuItem1.Visible = canRemoveElement && gameObject != GameObject;
            }

            e.Cancel = !showMenu;
        }

        private void MenuItemRemoveElementClick(object sender, EventArgs e)
        {
            FlowchartItem item = Flowchart.SelectedItem;
            if (item != null)
            {
                IItemElement element = item.SelectedItemElement as ItemGroupElement;
                if (element != null)
                {
                    IClassAsAtom addedObject = (IClassAsAtom)element.UserObject;
                    ElementaryGameObject gameObject = (ElementaryGameObject)item.UserObject;
                    RemoveGameObjectEntityInstance(gameObject, addedObject.ClassAsAtom);
                    RemoveFlowchartItemObjectForAllChildren(item, addedObject);
                }
            }
        }

        private void MenuItemRemoveItemClick(object sender, EventArgs e)
        {
            FlowchartItem item = Flowchart.SelectedItem;
            if (item != null)
            {

            }
        }

        private void FlowchartItemLinking(FlowchartItem srcItem, FlowchartItem destItem, 
            out bool cancelled)
        {
            ElementaryGameObject parentObject = (ElementaryGameObject)srcItem.UserObject;
            GameEntityType parentObjectType = GameEntityTypesTable.TypeOf(parentObject);
            ElementaryGameObject childObject = (ElementaryGameObject)destItem.UserObject;
            GameEntityType childObjectType = GameEntityTypesTable.TypeOf(childObject);
            cancelled = parentObjectType == GameEntityType.GameObject &&
                        childObjectType == GameEntityType.ElementaryGameObject;
        }

        private void FlowchartItemLinked(FlowchartItem srcItem, FlowchartItem destItem,
            bool manual)
        {
            if (manual)
            {
                ElementaryGameObject parentObject = (ElementaryGameObject) srcItem.UserObject;
                ElementaryGameObject childObject = (ElementaryGameObject) destItem.UserObject;
                if (parentObject != null && childObject != null)
                {
                    childObject.Parents.Add(parentObject);
                    RecreateFlowchartItemControls(destItem, childObject, true);
                }
            }
        }

        private void FlowchartItemUnlinked(FlowchartItem srcItem, FlowchartItem destItem,
            bool manual)
        {
            if (manual)
            {
                ElementaryGameObject parentObject = (ElementaryGameObject)srcItem.UserObject;
                ElementaryGameObject childObject = (ElementaryGameObject) destItem.UserObject;
                if (parentObject != null && childObject != null)
                {
                    childObject.Parents.Remove(parentObject);
                    RecreateFlowchartItemControls(destItem, childObject, true);
                }
            }
        }

        private void FlowchartDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
            string[] formats = e.Data.GetFormats();
            if (formats.Length == 1)
            {
                ElementaryGameObject gameObject = e.Data.GetData(formats[0]) as ElementaryGameObject;
                if (gameObject != null)
                {
                    GameEntityType type = GameEntityTypesTable.TypeOf(gameObject);
                    e.Effect = type == GameEntityType.ElementaryGameObject &&
                               !Flowchart.ContainsItemWithUserObject(gameObject)
                                   ? DragDropEffects.Copy
                                   : DragDropEffects.None;
                }
            }
        }

        private void FlowchartDragDrop(object sender, DragEventArgs e)
        {
            string[] formats = e.Data.GetFormats();
            ElementaryGameObject gameObject = e.Data.GetData(formats[0]) as ElementaryGameObject;
            if (gameObject != null)
            {
                FlowchartItem item = AddFlowchartItem(gameObject, true, true);
                item.LeftLinkPointVisible = !gameObject.HasParents;
                item.Selected = true;
            }
        }

        private void FlowchartItemDragEnter(FlowchartItem item, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
            string[] formats = e.Data.GetFormats();
            if (formats.Length == 1)
            {
                Atom atom = e.Data.GetData(formats[0]) as Atom;
                if (atom != null)
                {
                    ElementaryGameObject gameObject = (ElementaryGameObject)item.UserObject;
                    GameEntityType type = GameEntityTypesTable.TypeOf(atom);
                    e.Effect = type >= GameEntityType.GameObjectAttribute &&
                               !IsGameObjectHasEntityInstance(gameObject, atom)
                                   ? DragDropEffects.Copy
                                   : DragDropEffects.None;
                }
            }
        }

        private void FlowchartItemDragDrop(FlowchartItem item, DragEventArgs e)
        {
            string[] formats = e.Data.GetFormats();
            Atom atom = e.Data.GetData(formats[0]) as Atom;
            if (atom != null)
            {
                ElementaryGameObject gameObject = (ElementaryGameObject) item.UserObject;
                IClassAsAtom addedObject = AddGameObjectEntityInstance(gameObject, atom);
                AddFlowchartItemObjectForAllChildren(item, addedObject);
                item.Selected = true;
            }
        }

        private void FlowchartItemLocationChanged(FlowchartItem item)
        {
            _localStorage.FlowcharItems.StoreFlowchartItemData(
                item, ((ElementaryGameObject)item.UserObject).ObjectId,
                GameObject.ObjectId);
        }

        private void FlowchartMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks > 1 && DoubleClick != null)
            {
                DoubleClick(this, e);
            }
        }

#endregion

    }
}
