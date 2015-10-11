using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.GameObjectAttributeClasses;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Entities.GameObjectPropertyClasses;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Interfaces;
using GMechanics.Editor.Data;
using GMechanics.Editor.Helpers;
using GMechanics.FlowchartControl;
using GMechanics.FlowchartControl.ItemElements;

namespace GMechanics.Editor.UIControls.GraphicEditor
{
    public partial class GraphicEditor
    {
        private bool IsGameObjectHasEntityInstance(ElementaryGameObject gameObject, 
            IClassAsAtom atom)
        {
            GameEntityType type = GameEntityTypesTable.TypeOf(atom);
            switch (type)
            {
                case GameEntityType.GameObjectAttribute:
                case GameEntityType.ParentalGameObjectAttribute:
                    return gameObject.GetAttribute(atom.ClassAsAtom.Name, false) != null;
                case GameEntityType.GameObjectAttributeValue:
                case GameEntityType.ParentalGameObjectAttributeValue:
                    string attributeName = Helper.GetAttributeValueParent(atom.ClassAsAtom).Name;
                    return gameObject.GetAttributeValue(attributeName, atom.ClassAsAtom.Name, false) != null;
                case GameEntityType.GameObjectProperty:
                case GameEntityType.ParentalGameObjectProperty:
                    return gameObject.GetProperty(atom.ClassAsAtom.Name, false) != null;
                case GameEntityType.GameObjectFeature:
                    return gameObject.GetFeature(atom.ClassAsAtom.Name, false) != null;
            }
            return false;
        }

        private ElementaryGameObject GetGameObjectEntityOwner(ElementaryGameObject gameObject,
            IClassAsAtom atom)
        {
            GameEntityType type = GameEntityTypesTable.TypeOf(atom);
            switch (type)
            {
                case GameEntityType.GameObjectAttribute:
                case GameEntityType.ParentalGameObjectAttribute:
                    return gameObject.GetAttributeOwner(atom.ClassAsAtom.Name);
                case GameEntityType.GameObjectAttributeValue:
                case GameEntityType.ParentalGameObjectAttributeValue:
                    string attributeName = Helper.GetAttributeValueParent(atom.ClassAsAtom).Name;
                    return gameObject.GetAttributeValueOwner(attributeName, atom.ClassAsAtom.Name);
                case GameEntityType.GameObjectProperty:
                case GameEntityType.ParentalGameObjectProperty:
                    return gameObject.GetPropertyOwner(atom.ClassAsAtom.Name);
                case GameEntityType.GameObjectFeature:
                    return gameObject.GetFeatureOwner(atom.ClassAsAtom.Name);
            }
            return null;
        }

#region Create/recreate flowchart items

        private void RecreateFlowchartItemControls(FlowchartItem item,
            ElementaryGameObject gameObject, bool oneItemOnly)
        {
            if (oneItemOnly)
            {
                item.BeginUpdate();
            }

            item.Groups.ClearGroups();

            List<GameObjectAttribute> attributes = gameObject.GetAllAttributes(true);
            foreach (GameObjectAttribute attribute in attributes)
            {
                if (!IsGameObjectHasEntityInstance(gameObject, attribute))
                {
                    ElementaryGameObject objectParent = GetGameObjectEntityOwner(
                        gameObject, attribute);
                    AddFlowchartItemObject(item, attribute, objectParent);
                }
                else
                {
                    AddFlowchartItemObject(item, attribute);
                }
                List<GameObjectAttributeValue> values = gameObject.GetAllAttributeValues(
                    attribute.Name, true);
                foreach (GameObjectAttributeValue value in values)
                {
                    if (!IsGameObjectHasEntityInstance(gameObject, value))
                    {
                        ElementaryGameObject objectParent = GetGameObjectEntityOwner(
                            gameObject, value);
                        AddFlowchartItemObject(item, value, objectParent);
                    }
                    else
                    {
                        AddFlowchartItemObject(item, value);
                    }
                }
            }

            List<GameObjectProperty> properties = gameObject.GetAllProperties(true);
            foreach (GameObjectProperty property in properties)
            {
                if (!IsGameObjectHasEntityInstance(gameObject, property))
                {
                    ElementaryGameObject objectParent = GetGameObjectEntityOwner(
                        gameObject, property);
                    AddFlowchartItemObject(item, property, objectParent);
                }
                else
                {
                    AddFlowchartItemObject(item, property);
                }
            }

            List<GameObjectFeature> features = gameObject.GetAllFeatures(true);
            foreach (GameObjectFeature feature in features)
            {
                if (!IsGameObjectHasEntityInstance(gameObject, feature))
                {
                    ElementaryGameObject objectParent = GetGameObjectEntityOwner(
                        gameObject, feature);
                    AddFlowchartItemObject(item, feature, objectParent);
                }
                else
                {
                    AddFlowchartItemObject(item, feature);
                }
            }

            if (oneItemOnly)
            {
                item.EndUpdate();
            }
        }

        private Rectangle GetFlowchartItemLocation(FlowcharItemData data, 
            bool dragDropped)
        {
            if (data != null)
            {
                return data.Location;
            }
            if (dragDropped)
            {
                Point mousePos = PointToClient(MousePosition);
                return new Rectangle(
                    mousePos.X - DefaultItemSize.Width / 2,
                    mousePos.Y - DefaultItemSize.Height / 2,
                    DefaultItemSize.Width,
                    DefaultItemSize.Height);
            }
            return new Rectangle(
                32,
                32,
                DefaultItemSize.Width,
                DefaultItemSize.Height);
        }

        private string GetFlowchartItemSkinName(FlowcharItemData data,
            ElementaryGameObject gameObject)
        {
            if (data != null)
            {
                return data.SkinName;
            }
            return gameObject is GameObject
                       ? "Orange gradient"
                       : "White gradient";
        }

        private FlowchartItem AddFlowchartItem(ElementaryGameObject gameObject,
            bool dragDropped, bool oneItemOnly)
        {
            FlowcharItemData data = _localStorage.FlowcharItems.GetFlowchartItemData(
                gameObject.ObjectId, _gameObject.ObjectId);

            string skinName = GetFlowchartItemSkinName(data, gameObject);
            Rectangle location = GetFlowchartItemLocation(data, dragDropped);

            GameEntityType type = GameEntityTypesTable.TypeOf(gameObject);
            FlowchartItem item = Flowchart.AddItem(gameObject.Name, skinName, 
                                                   location, gameObject);
            if (dragDropped)
            {
                item.LeftLinkPointVisible = false;
            }

            item.BeginUpdate();

            int iconIdx = type == GameEntityType.ElementaryGameObject ? 3 : 4;
            item.Icon = ilEntities.Images[iconIdx];

            RecreateFlowchartItemControls(item, gameObject, false);

            if (oneItemOnly)
            {
                item.EndUpdate();
            }

            return item;
        }

        private void RecreateFlowcharItems()
        {
            Flowchart.BeginUpdate();

            Flowchart.ClearItems();
            
            if (_gameObject != null)
            {
                FlowchartItem selectedFlowchartItem = null;
                Dictionary<ElementaryGameObject, ElementaryGameObject> itemsRel =
                    new Dictionary<ElementaryGameObject, ElementaryGameObject>();

                List<ElementaryGameObject> objects = new List<ElementaryGameObject>();
                objects.Add(_gameObject);
                objects.AddRange(_gameObject.Parents.OwnParents);

                foreach (ElementaryGameObject ego in objects)
                {
                    bool isMainFlowchartItem = _gameObject.Parents.IsOwner(ego);
                    FlowchartItem flowchartItem = AddFlowchartItem(ego, false, false);
                    flowchartItem.LeftLinkPointVisible = isMainFlowchartItem;
                    flowchartItem.RightLinkPointVisible = !isMainFlowchartItem;

                    foreach (ElementaryGameObject parentEgo in ego.Parents.OwnParents)
                    {
                        itemsRel.Add(parentEgo, ego);
                    }

                    if (isMainFlowchartItem)
                    {
                        selectedFlowchartItem = flowchartItem;
                    }
                }

                foreach (KeyValuePair<ElementaryGameObject, ElementaryGameObject> pair in itemsRel)
                {
                    FlowchartItem srcItem = Flowchart.GetItemWithUserObject(pair.Key);
                    FlowchartItem destItem = Flowchart.GetItemWithUserObject(pair.Value);
                    Flowchart.LinkItems(srcItem, destItem);
                }

                Flowchart.SelectedItem = selectedFlowchartItem;
            }

            Flowchart.EndUpdate();

            lblNoGameObject.Visible = _gameObject == null;
        }

#endregion

#region Add game object entity instance

        private IClassAsAtom AddGameObjectEntityInstance(ElementaryGameObject gameObject,
            Atom atom)
        {
            IClassAsAtom addedObject = null;
            GameEntityType type = GameEntityTypesTable.TypeOf(atom);
            switch (type)
            {
                case GameEntityType.ParentalGameObjectAttribute:
                    {
                        addedObject = gameObject.AddAttribute(atom.Name);
                        break;
                    }
                case GameEntityType.ParentalGameObjectAttributeValue:
                    {
                        string attributeName = Helper.GetAttributeValueParent(atom).Name;
                        addedObject = gameObject.AddAttributeValue(attributeName, atom.Name);
                        break;
                    }
                case GameEntityType.ParentalGameObjectProperty:
                    {
                        addedObject = gameObject.AddProperty(atom.Name);
                        break;
                    }
                case GameEntityType.GameObjectFeature:
                    {
                        addedObject = gameObject.AddFeature(atom.Name);
                        break;
                    }
            }
            return addedObject;
        }

#endregion

#region Remove game object entity instance

        private void RemoveGameObjectEntityInstance(ElementaryGameObject gameObject,
            Atom atom)
        {
            GameEntityType type = GameEntityTypesTable.TypeOf(atom);
            switch (type)
            {
                case GameEntityType.ParentalGameObjectAttribute:
                    {
                        gameObject.RemoveAttribute(atom.Name, false);
                        break;
                    }
                case GameEntityType.ParentalGameObjectAttributeValue:
                    {
                        string attributeName = Helper.GetAttributeValueParent(atom).Name;
                        gameObject.RemoveAttributeValue(attributeName, atom.Name, false);
                        break;
                    }
                case GameEntityType.ParentalGameObjectProperty:
                    {
                        gameObject.RemoveProperty(atom.Name, false);
                        break;
                    }
                case GameEntityType.GameObjectFeature:
                    {
                        gameObject.RemoveFeature(atom.Name, false);
                        break;
                    }
            }
        }

#endregion

#region Add flowchart item object

        private void AddFlowchartItemObjectForAllChildren(FlowchartItem item,
            IClassAsAtom addedObject)
        {
            ElementaryGameObject prewEgo = _gameObject;
            ElementaryGameObject destEgo = (ElementaryGameObject) item.UserObject;
            foreach (ElementaryGameObject ego in _gameObject.Parents.Get())
            {
                if (destEgo == prewEgo || 
                    (prewEgo.Parents.HasOwnParent(destEgo) && prewEgo != _gameObject))
                {
                    break;
                }
                prewEgo = ego;
                if (ego.Parents.IsOwner(destEgo))
                {
                    continue;
                }
                FlowchartItem childItem = Flowchart.GetItemWithUserObject(ego);
                if (childItem != null && !IsGameObjectHasEntityInstance(ego, addedObject))
                {
                    ElementaryGameObject objectParent = GetGameObjectEntityOwner(ego, addedObject);
                    AddFlowchartItemObject(childItem, addedObject, objectParent);
                }
            }
            AddFlowchartItemObject(item, addedObject);
        }

        private ItemGroup GetOrCreateFlowchartItemGroup(FlowchartItem item, string groupName,
            ImageList imageList, int iconIndex)
        {
            return item.Groups.GetGroup(groupName) ??
                   item.Groups.AddGroup(groupName, imageList, iconIndex, true);
        }

        private void AddFlowchartItemObject(FlowchartItem item, IClassAsAtom addedObject)
        {
            AddFlowchartItemObject(item, addedObject, null);
        }

        private void AddFlowchartItemObject(FlowchartItem item, IClassAsAtom addedObject, 
            ElementaryGameObject objectParent)
        {
            string objectName = addedObject.ClassAsAtom.Name;
            GameEntityType type = GameEntityTypesTable.TypeOf(addedObject);

            ItemGroup group = null;
            switch (type)
            {
                case GameEntityType.GameObjectAttribute:
                case GameEntityType.GameObjectAttributeValue:
                    {
                        group = GetOrCreateFlowchartItemGroup(item, "Attributes", ilEntities, 0);
                        break;
                    }
                case GameEntityType.GameObjectProperty:
                    {
                        group = GetOrCreateFlowchartItemGroup(item, "Properties", ilEntities, 1);
                        break;
                    }
                case GameEntityType.GameObjectFeature:
                    {
                        group = GetOrCreateFlowchartItemGroup(item, "Features", ilEntities, 2);
                        break;
                    }
            }


            if (group != null)
            {
                ItemGroupElementsList elementsList;
                ItemGroupElement existsElement;
                switch (type)
                {
                    case GameEntityType.GameObjectAttributeValue:
                        {
                            string attributeName = Helper.GetAttributeValueParent(addedObject.ClassAsAtom).Name;
                            ItemGroupElement attributeElement = group.Elements[attributeName];
                            if (attributeElement == null)
                            {
                                attributeElement = group.Elements.Add(attributeName, addedObject);
                                if (objectParent != null)
                                {
                                    attributeElement.Icon = ilEntities.Images[5];
                                    attributeElement.IconHint = string.Format("Inherited from '{0}'", 
                                        objectParent.Name);
                                    attributeElement.IconCursor = Cursors.Help;
                                }
                                attributeElement.IconVisible = objectParent != null;
                            }
                            elementsList = attributeElement.Elements;
                            existsElement = attributeElement.Elements[objectName];
                            break;
                        }
                    default:
                        {
                            elementsList = group.Elements;
                            existsElement = group.Elements[objectName];
                        }
                        break;
                }

                if (existsElement == null)
                {
                    existsElement = elementsList.Add(objectName, addedObject);
                    existsElement.IconCursor = Cursors.Help;
                    existsElement.Icon = ilEntities.Images[5];
                    existsElement.IconVisible = objectParent != null;
                }
                else
                {
                    ElementaryGameObject destEgo = (ElementaryGameObject)item.UserObject;
                    switch (type)
                    {
                        case GameEntityType.GameObjectAttribute:
                            {
                                existsElement.IconVisible = !destEgo.AttributeExists(objectName, false);
                                break;
                            }
                        case GameEntityType.GameObjectAttributeValue:
                            {
                                string attributeName = Helper.GetAttributeValueParent(addedObject.ClassAsAtom).Name;
                                existsElement.IconVisible = !destEgo.AttributeValueExists(attributeName, objectName, false);
                                break;
                            }
                        case GameEntityType.GameObjectProperty:
                            {
                                existsElement.IconVisible = !destEgo.PropertyExists(objectName, false);
                                break;
                            }
                        case GameEntityType.GameObjectFeature:
                            {
                                existsElement.IconVisible = !destEgo.FeatureExists(objectName, false);
                                break;
                            }
                    }
                }

                if (objectParent != null)
                {
                    existsElement.IconHint = string.Format("Inherited from '{0}'", objectParent.Name);
                }

            }
        }

#endregion

#region Remove flowchart item object

        private void RemoveFlowchartItemObjectForAllChildren(FlowchartItem item,
            IClassAsAtom addedObject)
        {
            ElementaryGameObject prewEgo = _gameObject;
            ElementaryGameObject destEgo = (ElementaryGameObject)item.UserObject;
            foreach (ElementaryGameObject ego in _gameObject.Parents.Get())
            {
                if (destEgo == prewEgo ||
                    (prewEgo.Parents.HasOwnParent(destEgo) && prewEgo != _gameObject))
                {
                    break;
                }
                prewEgo = ego;
                if (ego.Parents.IsOwner(destEgo))
                {
                    continue;
                }
                FlowchartItem childItem = Flowchart.GetItemWithUserObject(ego);
                if (childItem != null && !IsGameObjectHasEntityInstance(ego, addedObject))
                {
                    RemoveFlowchartItemObject(childItem, addedObject);
                }
            }
            RemoveFlowchartItemObject(item, addedObject);
        }

        private void RemoveFlowchartItemObject(FlowchartItem item, IClassAsAtom addedObject)
        {
            ItemGroup group = null;
            GameEntityType type = GameEntityTypesTable.TypeOf(addedObject);
            ElementaryGameObject gameObject = (ElementaryGameObject)item.UserObject;

            ItemGroupElement inheritedElement = null;
            ElementaryGameObject inheritedElementOwner = null;

            switch (type)
            {
                case GameEntityType.GameObjectAttribute:
                    {
                        group = item.Groups.GetGroup("Attributes");
                        inheritedElementOwner = gameObject.GetAttributeOwner(addedObject.ClassAsAtom.Name);
                        if (inheritedElementOwner != null)
                        {
                            inheritedElement = group.Elements[addedObject.ClassAsAtom.Name];
                        }
                        break;
                    }
                case GameEntityType.GameObjectAttributeValue:
                    {
                        group = item.Groups.GetGroup("Attributes");
                        string attributeName = Helper.GetAttributeValueParent(addedObject.ClassAsAtom).Name;
                        inheritedElementOwner = gameObject.GetAttributeValueOwner(attributeName, 
                            addedObject.ClassAsAtom.Name);
                        if (inheritedElementOwner != null)
                        {
                            inheritedElement = group.Elements[attributeName].Elements[addedObject.ClassAsAtom.Name];
                        }
                        else
                        {
                            group.Elements[attributeName].Elements.Remove(addedObject.ClassAsAtom.Name);
                        }
                        break;
                    }
                case GameEntityType.GameObjectProperty:
                    {
                        group = item.Groups.GetGroup("Properties");
                        inheritedElementOwner = gameObject.GetPropertyOwner(addedObject.ClassAsAtom.Name);
                        if (inheritedElementOwner != null)
                        {
                            inheritedElement = group.Elements[addedObject.ClassAsAtom.Name];
                        }
                        break;
                    }
                case GameEntityType.GameObjectFeature:
                    {
                        group = item.Groups.GetGroup("Features");
                        inheritedElementOwner = gameObject.GetFeatureOwner(addedObject.ClassAsAtom.Name);
                        if (inheritedElementOwner != null)
                        {
                            inheritedElement = group.Elements[addedObject.ClassAsAtom.Name];
                        }
                        break;
                    }
            }

            if (inheritedElementOwner == null)
            {
                if (group != null)
                {
                    if (type != GameEntityType.GameObjectAttributeValue)
                    {
                        group.Elements.Remove(addedObject.ClassAsAtom.Name);
                    }
                    if (group.Elements.Count == 0)
                    {
                        item.Groups.RemoveGroup(group);
                    }
                }
            }
            else
            {
                inheritedElement.IconHint = string.Format("Inherited from '{0}'", inheritedElementOwner.Name);
                inheritedElement.IconVisible = true;
            }
        }

#endregion

    }
}
