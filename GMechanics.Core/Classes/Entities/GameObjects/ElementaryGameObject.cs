using System.Collections.Generic;
using GMechanics.Core.Classes.Entities.GameObjectAttributeClasses;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Entities.GameObjectPropertyClasses;
using GMechanics.Core.Classes.Entities.ParentalGameObjectAttributeClasses;
using GMechanics.Core.Classes.Entities.ParentalGameObjectPropertyClasses;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Storages;

namespace GMechanics.Core.Classes.Entities.GameObjects
{
    public class ElementaryGameObject : Atom
    {

#region Static members

        protected static GameObjectAttributes GOA = GlobalStorage.Instance.GameObjectAttributes;
        protected static GameObjectPropertiesClasses GOPC = GlobalStorage.Instance.GameObjectPropertiesClasses;
        protected static GameObjectProperties GOP = GlobalStorage.Instance.GameObjectProperties;
        protected static GameObjectFeaturesClasses GOFC = GlobalStorage.Instance.GameObjectFeaturesClasses;
        protected static GameObjectFeatures GOF = GlobalStorage.Instance.GameObjectFeatures;

#endregion

#region Properties

        public GameObjectParentsList Parents { get; private set; }
        protected HashSet<long> ExcludedObjects { get; set; }

        internal GameObjectAttributesList Attributes { get; private set; }
        internal GameObjectPropertiesList Properties { get; private set; }
        internal GameObjectFeaturesList Features { get; private set; }
        internal GameObjectsList Inventory { get; private set; }

        public bool HasParents
        {
            get { return Parents != null && Parents.Count > 0; }
        }

#endregion

#region Class functions

        public ElementaryGameObject()
        {
            Inventory = new GameObjectsList();
            Parents = new GameObjectParentsList(this);
        }

        public ElementaryGameObject(ElementaryGameObject parent) : this()
        {
            AddParent(parent);
        }

        public ElementaryGameObject(string name, string transcription, string description) 
            : this(null)
        {
            Initialize(name, transcription, description);
        }

        public ElementaryGameObject(string name, string transcription, string description,
            ElementaryGameObject parent) : this(name, transcription, description)
        {
            AddParent(parent);
        }

        private void Initialize(string name, string transcription, string description)
        {
            Name = name;
            Transcription = transcription;
            Description = description;
        }

#endregion

#region Parents

        public void AddParent(ElementaryGameObject parent)
        {
            Parents.Add(parent);
        }

        public void RemoveParent(ElementaryGameObject parent)
        {
            Parents.Remove(parent);
        }

#endregion

#region Excluded objects

        public bool ExcludeObject(Atom obj)
        {
            if (obj != null && (ExcludedObjects == null || !ExcludedObjects.Contains(obj.ObjectId)))
            {
                if (ExcludedObjects == null)
                {
                    ExcludedObjects = new HashSet<long>();
                }
                ExcludedObjects.Add(obj.ObjectId);
                return true;
            }
            return false;
        }

        public bool IncludeObject(Atom obj)
        {
            if (obj != null && IsObjectExcluded(obj))
            {
                ExcludedObjects.Remove(obj.ObjectId);
                if (ExcludedObjects.Count == 0)
                {
                    ExcludedObjects = null;
                }
                return true;
            }
            return false;
        }

        public bool IsObjectExcluded(Atom atom)
        {
            return ExcludedObjects != null && ExcludedObjects.Contains(atom.ObjectId);
        }

#endregion

#region Attribues

        public int GetAttributesCount(bool findInParent)
        {
            int count = 0;
            HashSet<ParentalGameObjectAttribute> hashSet = 
                new HashSet<ParentalGameObjectAttribute>();

            //Find all attributes
            foreach (ElementaryGameObject currentClass in Parents.Get(findInParent))
            {
                if (currentClass.Attributes != null)
                {
                    foreach (GameObjectAttribute attribute in currentClass.Attributes)
                    {
                        if (!hashSet.Contains(attribute.Parental))
                        {
                            hashSet.Add(attribute.Parental);
                            count++;
                        }
                    }
                }
            } 

            //Return attributes count
            return count;
        }

        public GameObjectAttribute AddAttribute(string attributeName)
        {
            GameObjectAttribute goa = null;
            ParentalGameObjectAttribute pgoa = GOA[attributeName];
            if (pgoa != null)
            {
                goa = new GameObjectAttribute(pgoa, null);
            }
            return AddAttribute(goa) ? goa : null;
        }

        public bool AddAttribute(GameObjectAttribute attribute)
        {
            if (attribute != null)
            {
                //Fire Assigning event
                bool cancelled;
                attribute.Parental.QueryInteractiveRecipients(InteractiveEventType.Assigning,
                    this as GameObject, attribute, out cancelled);

                //If can assign this attribute
                if (!cancelled)
                {
                    //Need remove old attribute with the same name
                    bool needRemoveOld = Attributes != null && Attributes[attribute.Parental.Name] != null;

                    //Remove old attribute with the same name
                    bool removed = !needRemoveOld || RemoveAttribute(attribute.Parental.Name, false);

                    //If removing was successful or it is not needed
                    if (!needRemoveOld || removed)
                    {
                        //Create attributes list
                        if (Attributes == null)
                        {
                            Attributes = new GameObjectAttributesList();
                        }

                        //Add attribute
                        Attributes.Add(attribute);

                        //Fire Assigned event
                        attribute.Parental.NotifyInteractiveRecipients(InteractiveEventType.Assigned, 
                            this as GameObject, attribute);
                        //Done
                        return true;
                    }
                }
            }
            return false;
        }

        private GameObjectAttribute GetAttribute(string attributeName, 
            bool findInParent, out object ownerClass)
        {
            ownerClass = null;
            GameObjectAttribute attribute = null;
            foreach (ElementaryGameObject currentClass in Parents.Get(findInParent))
            {
                attribute = currentClass.Attributes != null
                                ? currentClass.Attributes[attributeName]
                                : null;
                if (attribute != null)
                {
                    ownerClass = currentClass;
                    break;
                }
            } 
            return attribute;
        }

        public GameObjectAttribute GetAttribute(string attributeName, bool findInParent)
        {
            object ownerClass;
            return GetAttribute(attributeName, findInParent, out ownerClass);
        }

        public GameObjectAttribute GetAttribute(int index)
        {
            if (index >= 0)
            {
                int curIdx = 0;
                HashSet<ParentalGameObjectAttribute> hashSet = 
                    new HashSet<ParentalGameObjectAttribute>();
                foreach (ElementaryGameObject currentClass in Parents.Get())
                {
                    if (currentClass.Attributes != null)
                    {
                        foreach (GameObjectAttribute attribute in currentClass.Attributes)
                        {
                            if (!hashSet.Contains(attribute.Parental))
                            {
                                if (curIdx == index)
                                {
                                    return attribute;
                                }
                                hashSet.Add(attribute.Parental);
                                curIdx++;
                            }
                        }
                    }
                } 
            }
            return null;
        }

        public ElementaryGameObject GetAttributeOwner(string attributeName)
        {
            object ownerClass;
            GameObjectAttribute attribute = GetAttribute(attributeName, true, out ownerClass);
            return attribute != null ? (ElementaryGameObject)ownerClass : null;
        }

        public bool AttributeExists(string attributeName, bool findInParent)
        {
            return GetAttribute(attributeName, findInParent) != null;
        }

        public List<GameObjectAttribute> GetAllAttributes(bool findInParent)
        {
            List<GameObjectAttribute> result = new List<GameObjectAttribute>();
            HashSet<ParentalGameObjectAttribute> hashSet = 
                new HashSet<ParentalGameObjectAttribute>();
            foreach (ElementaryGameObject currentClass in Parents.Get(findInParent))
            {
                if (currentClass.Attributes != null)
                {
                    foreach (GameObjectAttribute attribute in currentClass.Attributes)
                    {
                        if (!hashSet.Contains(attribute.Parental))
                        {
                            result.Add(attribute);
                            hashSet.Add(attribute.Parental);
                        }
                    }
                }
            } 
            return result;
        }

        public bool RemoveAttribute(string attributeName, bool findInParent)
        {
            bool removed = false, onRemovedFired = false;
            foreach (ElementaryGameObject currentClass in Parents.Get(findInParent))
            {
                GameObjectAttribute attribute = currentClass.Attributes != null
                                                    ? currentClass.Attributes[attributeName]
                                                    : null;

                if (attribute != null && !onRemovedFired)
                {
                    //Fire Removing event
                    bool cancelled;
                    attribute.Parental.QueryInteractiveRecipients(InteractiveEventType.Removing,
                        this as GameObject, attribute, out cancelled);

                    //We cannot remove this attribute now
                    if (cancelled)
                    {
                        return false;
                    }

                    //Otherwise, remove attribute
                    removed |= currentClass.Attributes.Remove(attribute);

                    //and fire Removed event
                    attribute.Parental.NotifyInteractiveRecipients(InteractiveEventType.Removed, 
                        this as GameObject, attribute);
                    onRemovedFired = true;
                }
                else
                {
                    //Remove attribute
                    if (attribute != null)
                    {
                        removed |= currentClass.Attributes.Remove(attribute);
                    }
                }
            } 
            return removed;
        }

#endregion

#region Attribute values

        public GameObjectAttributeValue AddAttributeValue(string attributeName, string valueName)
        {
            bool needToAddAttribute = false;

            //Get parent attibute or create new
            GameObjectAttribute attribute = Attributes != null
                                                ? Attributes[attributeName]
                                                : null;
            if (attribute == null)
            {
                ParentalGameObjectAttribute pgoa = GOA[attributeName];
                if (pgoa != null)
                {
                    attribute = new GameObjectAttribute(pgoa, null);
                    needToAddAttribute = true;
                }
            }

            //If attrbute exists
            if (attribute != null)
            {
                //and has contains value, do
                ParentalGameObjectAttributeValue pgoav = attribute.Parental.Values.GetValue(valueName, true);
                if (pgoav != null)
                {
                    GameObjectAttributeValue goav = new GameObjectAttributeValue(pgoav, this as GameObject);

                    //Fire Assigning event
                    bool cancelled;
                    pgoav.QueryInteractiveRecipients(InteractiveEventType.Assigning,
                        this as GameObject, goav, out cancelled);

                    //If can assign this attribute value
                    if (!cancelled)
                    {
                        //We cannot add this attribute now
                        if (needToAddAttribute && !AddAttribute(attribute))
                        {
                            return null;
                        }

                        //Otherwise include attribute and his value (if needed)
                        IncludeObject(pgoav);
                        IncludeObject(attribute.ClassAsAtom);

                        //Add attribute value
                        if (attribute.Values == null)
                        {
                            attribute.Values = new GameObjectAttributeValuesList { goav };
                        }
                        else if (!attribute.Values.Contains(goav))
                        {
                            attribute.Values.Add(goav);
                        }

                        //Fire Assigned event
                        pgoav.NotifyInteractiveRecipients(InteractiveEventType.Assigned, 
                            this as GameObject, goav);

                        //Return attribute value
                        return goav;
                    }
                }
            }
            return null;
        }

        private GameObjectAttributeValue GetAttributeValue(string attributeName, string valueName,
                                         bool findInParent, out GameObjectAttribute ownerAttribute,
                                         out object ownerClass)
        {
            ownerClass = null;
            ownerAttribute = null;
            GameObjectAttributeValue value = null;

//            Atom baseObject = null;
//            ParentalGameObjectAttribute pgoa = GOA[attributeName];
//            if (pgoa != null && pgoa.Values != null)
//            {
//                baseObject = pgoa.Values.GetValue(valueName, true);
//            }

            foreach (ElementaryGameObject currentClass in Parents.Get(findInParent))
            {
//                if (baseObject != null && currentClass.IsObjectExcluded(baseObject))
//                {
//                    break;
//                }

                GameObjectAttribute attribute = currentClass.Attributes != null
                                                    ? currentClass.Attributes[attributeName]
                                                    : null;
                if (attribute != null)
                {
                    value = attribute[valueName];
                    if (value != null)
                    {
                        ownerAttribute = attribute;
                        ownerClass = currentClass;
                        break;
                    }
                }
            } 
            return value;
        }

        public GameObjectAttributeValue GetAttributeValue(string attributeName,
            string valueName, bool findInParent)
        {
            object ownerClass;
            GameObjectAttribute ownerAttribute;
            return GetAttributeValue(attributeName, valueName, findInParent, out ownerAttribute, 
                                     out ownerClass);
        }

        public GameObjectAttributeValue GetAttributeValue(string attributeName, int index)
        {
            if (index >= 0)
            {
                int curIdx = 0;
                foreach (ElementaryGameObject currentClass in Parents.Get())
                {
                    GameObjectAttribute attribute = currentClass.Attributes != null
                                                        ? currentClass.Attributes[attributeName]
                                                        : null;
                    if (attribute != null)
                    {
                        if (attribute.Values.Count + curIdx > index)
                        {
                            int idx = (attribute.Values.Count + curIdx - 1) - index;
                            return attribute.Values[idx];
                        }
                        curIdx += attribute.Values.Count;
                    }
                } 
            }
            return null;
        }

        public int GetAttributeValuesCount(string attributeName, bool findInParent)
        {
            int count = 0;

            //Find all values
            foreach (ElementaryGameObject currentClass in Parents.Get(findInParent))
            {
                GameObjectAttribute attribute = currentClass.Attributes != null
                                                    ? currentClass.Attributes[attributeName]
                                                    : null;
                if (attribute != null)
                {
                    count += attribute.Values.Count;
                }
            } 

            //Return values count
            return count;
        }

        public ElementaryGameObject GetAttributeValueOwner(string attributeName, string valueName)
        {
            object ownerClass;
            GameObjectAttribute ownerAttribute;
            GameObjectAttributeValue value = GetAttributeValue(attributeName, valueName, true,
                out ownerAttribute, out ownerClass);
            return value != null ? (ElementaryGameObject)ownerClass : null;
        }

        public bool AttributeValueExists(string attributeName, string valueName, bool findInParent)
        {
            return GetAttributeValue(attributeName, valueName, findInParent) != null;
        }

        public List<GameObjectAttributeValue> GetAllAttributeValues(string attributeName, bool findInParent)
        {
            List<GameObjectAttributeValue> result = new List<GameObjectAttributeValue>();
            foreach (ElementaryGameObject currentClass in Parents.Get(findInParent))
            {
                GameObjectAttribute attribute = currentClass.GetAttribute(attributeName, findInParent);
                if (attribute != null && attribute.Values != null)
                {
                    result.AddRange(attribute.Values);
                }
            } 
            return result;
        }

        public bool RemoveAttributeValue(string attributeName, string valueName, bool findInParent)
        {
            bool removed = false, onRemovedFired = false;
            foreach (ElementaryGameObject currentClass in Parents.Get(findInParent))
            {
                GameObjectAttribute attribute = currentClass.Attributes != null
                                                    ? currentClass.Attributes[attributeName]
                                                    : null;
                //If attribute exists
                if (attribute != null)
                {
                    //Get attribute value
                    GameObjectAttributeValue goav = attribute[valueName];
                    
                    if (goav != null && !onRemovedFired)
                    {
                        //Fire Removing event
                        bool cancelled;
                        goav.Parental.QueryInteractiveRecipients(InteractiveEventType.Removing,
                            this as GameObject, goav, out cancelled);

                        //We cannot remove this attribute value now
                        if (cancelled)
                        {
                            return false;
                        }

                        //Otherwise, remove attribute value
                        removed |= attribute.Values.Remove(goav);

                        //and fire Removed event
                        goav.Parental.NotifyInteractiveRecipients(InteractiveEventType.Removed, 
                            this as GameObject, goav);
                        onRemovedFired = true;
                    }
                    else
                    {
                        //Remove attribute value
                        removed |= attribute.Values.Remove(goav);
                    }

                    //If attribute values list is emty, set it to NULL
                    if (attribute.Values.Count == 0)
                    {
                        attribute.Values = null;
                    }
                }
            } 
            return removed;
        }

#endregion

#region Properties

        public int GetPropertiesCount(bool findInParent)
        {
            int count = 0;
            HashSet<ParentalGameObjectProperty> hashSet = new HashSet<ParentalGameObjectProperty>();

            //Find all properties
            foreach (ElementaryGameObject currentClass in Parents.Get(findInParent))
            {
                if (currentClass.Properties != null)
                {
                    foreach (GameObjectProperty property in currentClass.Properties)
                    {
                        if (!hashSet.Contains(property.Parental))
                        {
                            hashSet.Add(property.Parental);
                            count++;
                        }
                    }
                }
            } 

            //Return properties count
            return count;
        }

        public GameObjectProperty AddProperty(string propertyName)
        {
            GameObjectProperty gop = null;
            ParentalGameObjectProperty pgop = GOP[propertyName];
            if (pgop != null)
            {
                gop = new GameObjectProperty(pgop, this as GameObject);
            }
            return AddProperty(gop) ? gop : null;
        }

        public bool AddProperty(GameObjectProperty property)
        {
            if (property != null)
            {
                //Fire Assigning event
                bool cancelled;
                property.Parental.QueryInteractiveRecipients(InteractiveEventType.Assigning,
                    this as GameObject, property, out cancelled);

                //If can assign this property
                if (!cancelled)
                {
                    //Need remove old property with the same name
                    bool needRemoveOld = Properties != null && Properties[property.Parental.Name] != null;

                    //Remove old property with the same name
                    bool removed = !needRemoveOld || RemoveProperty(property.Parental.Name, false);

                    //If removing was successful or it is not needed
                    if (!needRemoveOld || removed)
                    {
                        //Create properties list
                        if (Properties == null)
                        {
                            Properties = new GameObjectPropertiesList();
                        }

                        //Add property
                        Properties.Add(property);

                        //Fire Assigned event
                        property.Parental.NotifyInteractiveRecipients(InteractiveEventType.Assigned, 
                            this as GameObject, property);
                        //Done
                        return true;
                    }
                }
            }
            return false;
        }

        private GameObjectProperty GetProperty(string propertyName, bool findInParent,
                                               out object ownerClass)
        {
            ownerClass = null;
            GameObjectProperty property = null;
            foreach (ElementaryGameObject currentClass in Parents.Get(findInParent))
            {
                property = currentClass.Properties != null
                               ? currentClass.Properties[propertyName]
                               : null;
                if (property != null)
                {
                    ownerClass = currentClass;
                    break;
                }
            } 
            return property;
        }

        public GameObjectProperty GetProperty(string propertyName, bool findInParent)
        {
            object ownerClass;
            return GetProperty(propertyName, findInParent, out ownerClass);
        }

        public GameObjectProperty GetProperty(int index)
        {
            if (index >= 0)
            {
                int curIdx = 0;
                HashSet<ParentalGameObjectProperty> hashSet = new HashSet<ParentalGameObjectProperty>();
                foreach (ElementaryGameObject currentClass in Parents.Get())
                {
                    if (currentClass.Properties != null)
                    {
                        foreach (GameObjectProperty property in currentClass.Properties)
                        {
                            if (!hashSet.Contains(property.Parental))
                            {
                                if (curIdx == index)
                                {
                                    return property;
                                }
                                hashSet.Add(property.Parental);
                                curIdx++;
                            }
                        }
                    }
                } 
            }
            return null;
        }

        public ElementaryGameObject GetPropertyOwner(string propertyName)
        {
            object ownerClass;
            GameObjectProperty property = GetProperty(propertyName, true, out ownerClass);
            return property != null ? (ElementaryGameObject)ownerClass : null;
        }

        public GameObjectPropertyClass GetPropertyClass(string propertyName)
        {
            object ownerClass;
            GameObjectProperty property = GetProperty(propertyName, true, out ownerClass);
            return property != null ? property.Parental.PropertyClass : null;
        }

        public bool PropertyExists(string propertyName, bool findInParent)
        {
            return GetProperty(propertyName, findInParent) != null;
        }

        public List<GameObjectProperty> GetAllProperties(bool findInParent)
        {
            List<GameObjectProperty> result = new List<GameObjectProperty>();
            HashSet<ParentalGameObjectProperty> hashSet = new HashSet<ParentalGameObjectProperty>();
            foreach (ElementaryGameObject currentClass in Parents.Get(findInParent))
            {
                if (currentClass.Properties != null)
                {
                    foreach (GameObjectProperty property in currentClass.Properties)
                    {
                        if (!hashSet.Contains(property.Parental))
                        {
                            result.Add(property);
                            hashSet.Add(property.Parental);
                        }
                    }
                }
            }
            return result;
        }

        public bool RemoveProperty(string propertyName, bool findInParent)
        {
            bool removed = false, onRemovedFired = false;
            foreach (ElementaryGameObject currentClass in Parents.Get(findInParent))
            {
                GameObjectProperty property = currentClass.Properties != null
                                                  ? currentClass.Properties[propertyName]
                                                  : null;

                if (property != null && !onRemovedFired)
                {
                    //Fire Removing event
                    bool cancelled;
                    property.Parental.QueryInteractiveRecipients(InteractiveEventType.Removing,
                        this as GameObject, property, out cancelled);

                    //We cannot remove this property now
                    if (cancelled)
                    {
                        return false;
                    }

                    //Otherwise, remove property
                    removed |= currentClass.Properties.Remove(property);

                    //and fire Removed event
                    property.Parental.NotifyInteractiveRecipients(InteractiveEventType.Removed, 
                        this as GameObject, property);
                    onRemovedFired = true;
                }
                else
                {
                    //Remove property
                    if (property != null)
                    {
                        removed |= currentClass.Properties.Remove(property);
                    }
                }
            }
            return removed;
        }

#endregion

#region Features

        public int GetFeaturesCount(bool findInParent)
        {
            int count = 0;
            HashSet<GameObjectFeature> hashSet = new HashSet<GameObjectFeature>();

            //Find all features
            foreach (ElementaryGameObject currentClass in Parents.Get(findInParent))
            {
                if (currentClass.Features != null)
                {
                    foreach (GameObjectFeature feature in currentClass.Features)
                    {
                        if (!hashSet.Contains(feature))
                        {
                            hashSet.Add(feature);
                            count++;
                        }
                    }
                }
            } 

            //Return features count
            return count;
        }

        public GameObjectFeature AddFeature(string featureName)
        {
            GameObjectFeature gof = GOF[featureName];
            return AddFeature(gof) ? gof : null;
        }

        public bool AddFeature(GameObjectFeature feature)
        {
            //Get feature instance
            if (feature != null)
            {
                //Fire Assigning event
                bool cancelled;
                feature.QueryInteractiveRecipients(InteractiveEventType.Assigning,
                    this as GameObject, feature, out cancelled);

                //If can assign this feature
                if (!cancelled)
                {
                    //Need remove old feature with the same name
                    bool needRemoveOld = Features != null && Features[feature.Name] != null;

                    //Remove old feature with the same name
                    bool removed = !needRemoveOld || RemoveFeature(feature.Name, false);

                    //If removing was successful or it is not needed
                    if (!needRemoveOld || removed)
                    {
                        //Create features list
                        if (Features == null)
                        {
                            Features = new GameObjectFeaturesList();
                        }

                        //Add feature
                        Features.Add(feature);

                        //Fire Assigned event
                        feature.NotifyInteractiveRecipients(InteractiveEventType.Assigned, 
                            this as GameObject, feature);

                        //Return feature
                        return true;
                    }
                }
            }
            return false;
        }

        private GameObjectFeature GetFeature(string featureName, bool findInParent,
                                             out object ownerClass)
        {
            ownerClass = null;
            GameObjectFeature feature = null;
            foreach (ElementaryGameObject currentClass in Parents.Get(findInParent))
            {
                feature = currentClass.Features != null
                              ? currentClass.Features[featureName]
                              : null;
                if (feature != null)
                {
                    ownerClass = currentClass;
                    break;
                }
            } 
            return feature;
        }

        public GameObjectFeature GetFeature(string featureName, bool findInParent)
        {
            object ownerClass;
            return GetFeature(featureName, findInParent, out ownerClass);
        }

        public GameObjectFeature GetFeature(int index)
        {
            if (index >= 0)
            {
                int curIdx = 0;
                HashSet<GameObjectFeature> hashSet = new HashSet<GameObjectFeature>();
                foreach (ElementaryGameObject currentClass in Parents.Get())
                {
                    if (currentClass.Features != null)
                    {
                        foreach (GameObjectFeature feature in currentClass.Features)
                        {
                            if (!hashSet.Contains(feature))
                            {
                                if (curIdx == index)
                                {
                                    return feature;
                                }
                                hashSet.Add(feature);
                                curIdx++;
                            }
                        }
                    }
                } 
            }
            return null;
        }

        public ElementaryGameObject GetFeatureOwner(string featureName)
        {
            object ownerClass;
            GameObjectFeature feature = GetFeature(featureName, true, out ownerClass);
            return feature != null ? (ElementaryGameObject)ownerClass : null;
        }

        public GameObjectFeatureClass GetFeatureClass(string featureName)
        {
            object ownerClass;
            GameObjectFeature feature = GetFeature(featureName, true, out ownerClass);
            return feature != null ? feature.FeatureClass : null;
        }

        public bool FeatureExists(string featureName, bool findInParent)
        {
            return GetFeature(featureName, findInParent) != null;
        }

        public List<GameObjectFeature> GetAllFeatures(bool findInParent)
        {
            List<GameObjectFeature> result = new List<GameObjectFeature>();
            HashSet<GameObjectFeature> hashSet = new HashSet<GameObjectFeature>();
            foreach (ElementaryGameObject currentClass in Parents.Get(findInParent))
            {
                if (currentClass.Features != null)
                {
                    foreach (GameObjectFeature feature in currentClass.Features)
                    {
                        if (!hashSet.Contains(feature))
                        {
                            result.Add(feature);
                            hashSet.Add(feature);
                        }
                    }
                }
            } 
            return result;
        }

        public bool RemoveFeature(string featureName, bool findInParent)
        {

            bool removed = false, onRemovedFired = false;
            foreach (ElementaryGameObject currentClass in Parents.Get(findInParent))
            {
                GameObjectFeature feature = currentClass.Features != null
                                                ? currentClass.Features[featureName]
                                                : null;

                if (feature != null && !onRemovedFired)
                {
                    //Fire Removing event
                    bool cancelled;
                    feature.QueryInteractiveRecipients(InteractiveEventType.Removing,
                        this as GameObject, feature, out cancelled);

                    //We cannot remove this feature now
                    if (cancelled)
                    {
                        return false;
                    }

                    //Otherwise, remove feature
                    removed |= currentClass.Features.Remove(feature);

                    //and fire Removed event
                    feature.NotifyInteractiveRecipients(InteractiveEventType.Removed,
                        this as GameObject, feature);

                    onRemovedFired = true;
                }
                else
                {
                    //Remove feature
                    if (feature != null)
                    {
                        removed |= currentClass.Features.Remove(feature);
                    }
                }
            }
            return removed;
        }

#endregion

    }
}
