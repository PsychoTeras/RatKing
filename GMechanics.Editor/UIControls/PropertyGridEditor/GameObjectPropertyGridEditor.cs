using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GMechanics.Core.Classes.Entities.GameObjectAttributeClasses;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Entities.GameObjectPropertyClasses;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Types;
using GMechanics.Editor.Helpers;
using PropertyGridEx;

namespace GMechanics.Editor.UIControls.PropertyGridEditor
{
    public partial class GameObjectPropertyGridEditor : PropertyGridEx.PropertyGridEx
    {

#region Constants

        private const string GNCommonSettings = "\t\tCommon settings";
        private const string GNObjectSettings = "\t\tGame object settings";
        private const string GNAttributesSettings = "\tAttributes";
        private const string GNPropertiesSettings = "\tProperties";
        private const string GNFeaturesSettings = "Features";

        private const string PNName = "\tName";
        private const string PNTranscription = "\tTranscription";
        private const string PNDescription = "Description";

        private const string PNSize = "Size";
        private const string PNWeight = "Weight";
        private const string PNDirection = "Direction";
        private const string PNLocation = "Location";

#endregion

#region Private members

        private bool _editMode;
        private object _selectedObject;

#endregion

#region Properties

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

        public new object SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                if (_selectedObject != value)
                {
                    _selectedObject = value;
                    UpdateSelectedObject();
                }
            }
        }

#endregion

#region Class functions

        public GameObjectPropertyGridEditor()
        {
            InitializeComponent();
            SelectedGridItemChanged += OnSelectedGridItemChanged;
            EndGameObjectEditing();
        }

        private void BeginGameObjectEditing()
        {
            _editMode = true;
            ViewBackColor = Color.FromKnownColor(KnownColor.Window);
        }

        private void EndGameObjectEditing()
        {
            ViewBackColor = Color.White;
            _editMode = false;
        }

        private void UpdateSelectedObject()
        {
            PropertyValueChanged -= OnPropertyValueChanged;

            Helper.LockUpdate(this);
            try
            {
                //Clear previous items
                base.SelectedObject = null;
                Item.Clear();

                //If no object selected, return
                if (_selectedObject == null)
                {
                    return;
                }

                //else, switch selected object type
                GameEntityType type = GameEntityTypesTable.TypeOf(_selectedObject);
                switch (type)
                {
                    //If selected object is game object, manually fill properties list
                    case GameEntityType.ElementaryGameObject:
                    case GameEntityType.GameObject:
                        {
                            ShowCustomProperties = true;
                            PopulateGameObjectCustomProperties(type);
                            break;
                        }
                    //For each other object, use base SelectedObject property
                    default:
                        {
                            ShowCustomProperties = false;
                            base.SelectedObject = _selectedObject;
                            break;
                        }
                }
            }
            finally
            {
                if (PropertySort != PropertySort.CategorizedAlphabetical)
                {
                    PropertySort = PropertySort.CategorizedAlphabetical;
                }
                Helper.UnlockUpdate(this);
                Refresh();
            }

            PropertyValueChanged += OnPropertyValueChanged;
        }

        private void PopulateGameObjectCustomProperties(GameEntityType gameObjectType)
        {
            //Update elementary game object properties
            PopulateElementaryGameObjectMembers();

            //If it`s an GameObject, update game object properties
            if (gameObjectType == GameEntityType.GameObject)
            {
                PopulateGameObjectMembers();
            }

            //Populate game object attributes
            PopulateGameObjectAttributes();

            //Populate game object properties
            PopulateGameObjectProperties();

            //Populate game object features
            PopulateGameObjectFeatures();
        }

        private void PopulateElementaryGameObjectMembers()
        {
            ElementaryGameObject gameObject = (ElementaryGameObject) _selectedObject;
            AddCustomProperty(GNCommonSettings, PNName, gameObject.Name, false);
            AddCustomProperty(GNCommonSettings, PNTranscription, gameObject.Transcription, false);
            AddCustomProperty(GNCommonSettings, PNDescription, gameObject.Description, false);
        }

        private void PopulateGameObjectMembers()
        {
            GameObject gameObject = (GameObject)_selectedObject;
            AddCustomProperty(GNObjectSettings, PNSize, gameObject.Size, false);
            AddCustomProperty(GNObjectSettings, PNWeight, gameObject.Weight, false);
            AddCustomProperty(GNObjectSettings, PNDirection, gameObject.Direction, false);
            AddCustomProperty(GNObjectSettings, PNLocation, gameObject.Location, false);
        }

        private void PopulateGameObjectAttributes()
        {
            ElementaryGameObject gameObject = (ElementaryGameObject)_selectedObject;
            List<GameObjectAttribute> attributes = gameObject.GetAllAttributes(true);
            foreach (GameObjectAttribute attribute in attributes)
            {
                bool readOnly = !gameObject.AttributeExists(attribute.Name, false);
                AddCustomProperty(GNAttributesSettings, attribute.Name, attribute,
                                  readOnly);
            }
        }

        private void PopulateGameObjectProperties()
        {
            ElementaryGameObject gameObject = (ElementaryGameObject)_selectedObject;
            List<GameObjectProperty> properties = gameObject.GetAllProperties(true);
            foreach (GameObjectProperty property in properties)
            {
                bool readOnly = !gameObject.PropertyExists(property.Name, false);
                AddCustomProperty(GNPropertiesSettings, property.Name, property,
                                  readOnly);
            }
        }

        private void PopulateGameObjectFeatures()
        {
            ElementaryGameObject gameObject = (ElementaryGameObject)_selectedObject;
            List<GameObjectFeature> features = gameObject.GetAllFeatures(true);
            foreach (GameObjectFeature feature in features)
            {
                bool readOnly = !gameObject.FeatureExists(feature.Name, false);
                AddCustomProperty(GNFeaturesSettings, feature.Name, feature,
                                  readOnly);
            }
        }

        private void AddCustomProperty(string groupName, string propertyName, 
            object value, bool readOnly)
        {
            Item.Add(propertyName, value, readOnly, groupName, "", true);
            CustomProperty property = Item[Item.Count - 1];
            property.DefaultValue = null;
        }

        private void OnPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.PropertyDescriptor != null &&
                e.ChangedItem.PropertyDescriptor.GetType().Name == "CustomPropertyDescriptor")
            {
                CustomProperty.CustomPropertyDescriptor cpd = (CustomProperty.CustomPropertyDescriptor)
                                                              e.ChangedItem.PropertyDescriptor;
                CustomProperty property = (CustomProperty) cpd.CustomProperty;
                switch (cpd.Category)
                {
                    case GNCommonSettings:
                        {
                            CommonSettingValueChanged(property);
                            break;
                        }
                    case GNObjectSettings:
                        {
                            ObjectSettingValueChanged(property);
                            break;
                        }
                    case GNAttributesSettings:
                        {
                            AttributeSettingValueChanged(cpd, property);
                            break;
                        }
                    case GNPropertiesSettings:
                        {
                            PropertySettingValueChanged(cpd, property);
                            break;
                        }
                }
                OnSelectedGridItemChanged(null, null);
            }
        }

        private void CommonSettingValueChanged(CustomProperty property)
        {
            ElementaryGameObject gameObject = (ElementaryGameObject)_selectedObject;
            switch (property.Name)
            {
                case PNName:
                    {
                        gameObject.Name = property.Value.ToString();
                        break;
                    }
                case PNTranscription:
                    {
                        gameObject.Transcription = property.Value.ToString();
                        break;
                    }
                case PNDescription:
                    {
                        gameObject.Description = property.Value.ToString();
                        break;
                    }
            }
        }

        private void ObjectSettingValueChanged(CustomProperty property)
        {
            GameObject gameObject = (GameObject)_selectedObject;
            switch (property.Name)
            {
                case PNSize:
                    {
                        gameObject.Size = Size3D.FromString(property.Value.ToString());
                        break;
                    }
                case PNWeight:
                    {
                        gameObject.Weight = (float)property.Value;
                        break;
                    }
                case PNDirection:
                    {
                        gameObject.Direction = (float)property.Value;
                        break;
                    }
                case PNLocation:
                    {
                        gameObject.Location = Location3D.FromString(property.Value.ToString());
                        break;
                    }
            }
        }

        private void AttributeSettingValueChanged(CustomProperty.CustomPropertyDescriptor cpd,
            CustomProperty property)
        {
        }

        private void PropertySettingValueChanged(CustomProperty.CustomPropertyDescriptor cpd,
            CustomProperty property)
        {
        }

        private void OnSelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
        }

#endregion

    }
}