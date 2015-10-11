using System;
using System.IO;
using System.Reflection;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Entities.ParentalGameObjectAttributeClasses;
using GMechanics.Core.Classes.Entities.ParentalGameObjectPropertyClasses;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Helpers;

namespace GMechanics.Core.Classes.Storages
{
    public sealed class GlobalStorage : MarshalByRefObject
    {
        private static readonly GlobalStorage StaticInstance = new GlobalStorage();

#region Accessors

        public GameObjectPropertiesClasses GameObjectPropertiesClasses
        {
            get { return GameObjectPropertiesClasses.Instance; }
        }

        public GameObjectProperties GameObjectProperties
        {
            get { return GameObjectProperties.Instance; }
        }

        public GameObjectFeaturesClasses GameObjectFeaturesClasses
        {
            get { return GameObjectFeaturesClasses.Instance; }
        }

        public GameObjectFeatures GameObjectFeatures
        {
            get { return GameObjectFeatures.Instance; }
        }

        public GameObjectAttributes GameObjectAttributes
        {
            get { return GameObjectAttributes.Instance; }
        }

        public ElementaryGameObjects ElementaryGameObjects
        {
            get { return ElementaryGameObjects.Instance; }
        }

        public GameObjects GameObjects
        {
            get { return GameObjects.Instance; }
        }

        public ElementaryGameObjectGroups ElementaryGameObjectGroups
        {
            get { return ElementaryGameObjectGroups.Instance; }
        }

        public GameObjectGroups GameObjectGroups
        {
            get { return GameObjectGroups.Instance; }
        }

        public static GlobalStorage Instance
        {
            get { return StaticInstance; }
        }

#endregion Accessors

        private GlobalStorage() { }

        public void RemoveDestroyedItemsForAll()
        {
            Helper.RemoveDestroyedItemsFromListsOwner(this);
        }

        public void RemoveDestroyedItemsForGameObjects()
        {
            Helper.RemoveDestroyedItemsFromList(GameObjects);
        }

        private void EstablishInteractiveRecipientsLinks(Atom atom)
        {
            byte[] data = (byte[]) atom.InteractiveRecipientsData;
            if (data != null)
            {
                int dataSize = data.Length;
                atom.InitializeInteractiveRecipientsList();
                using (MemoryStream ms = new MemoryStream(data))
                {
                    BinaryReader br = new BinaryReader(ms);
                    while (ms.Position < dataSize)
                    {
                        InteractiveEventType eventType = (InteractiveEventType) br.ReadByte();
                        ushort recipientCount = br.ReadUInt16();
                        for (int i = 0; i < recipientCount; i++)
                        {
                            string recipientName = br.ReadString();
                            GameObjectFeature recipient = GameObjectFeatures[recipientName];
                            if (recipient != null)
                            {
                                atom.AddInteractiveRecipient(eventType, recipient);
                            }
                        }
                    }
                }
            }
        }

        private void EstablishInteractiveRecipientsLinks(ParentalGameObjectAttributeValuesList values)
        {
            foreach (ParentalGameObjectAttributeValue value in values)
            {
                EstablishInteractiveRecipientsLinks(value);
                EstablishInteractiveRecipientsLinks(value.Values);
            }
        }

        private void EstablishGlobalObjectsLinks()
        {
            foreach (ParentalGameObjectAttribute obj in GameObjectAttributes.Values)
            {
                //Establish interactive recipients links
                EstablishInteractiveRecipientsLinks(obj);

                //Establish interactive recipients links for values
                ParentalGameObjectAttributeValuesList values = obj.Values;
                if (values != null)
                {
                    EstablishInteractiveRecipientsLinks(values);
                }
            }

            foreach (ParentalGameObjectProperty obj in GameObjectProperties.Values)
            {
                //Assign game object properties to own groups
                string className = obj.PropertyClassName;
                obj.PropertyClass = GameObjectPropertiesClasses[className];

                //Establish interactive recipients links
                EstablishInteractiveRecipientsLinks(obj);
            }

            foreach (GameObjectFeature obj in GameObjectFeatures.Values)
            {
                //Assign game object features to own groups
                string className = obj.FeatureClassName;
                obj.FeatureClass = GameObjectFeaturesClasses[className];

                //Establish interactive recipients links
                EstablishInteractiveRecipientsLinks(obj);
            }
        }

        internal bool SaveStorageToFile(string filePath, object container)
        {
            try
            {
                Type cType = container.GetType();
                MethodInfo mSerialize = cType.GetMethod("Serialize");
                byte[] data = (byte[]) mSerialize.Invoke(container, null);
                File.WriteAllBytes(filePath, data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool LoadStorageFromFile(string filePath, object container)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    byte[] data = File.ReadAllBytes(filePath);
                    Type cType = container.GetType();
                    MethodInfo mDeserialize = cType.GetMethod("Deserialize");
                    mDeserialize.Invoke(container, new object[] {data});
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        internal bool LoadSQLiteStorageFromFile(string filePath, object container)
        {
            try
            {
                Type cType = container.GetType();
                MethodInfo mLoad = cType.GetMethod("Load");
                mLoad.Invoke(container, new object[] {filePath});
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Save()
        {
            if (!Directory.Exists(GlobalVariables.DataFolderPath))
            {
                Directory.CreateDirectory(GlobalVariables.DataFolderPath);
            }

            SaveStorageToFile(GlobalVariables.GameObjectPropertiesClassesFilePath,
                              GameObjectPropertiesClasses);
            SaveStorageToFile(GlobalVariables.GameObjectPropertiesFilePath,
                              GameObjectProperties);
            SaveStorageToFile(GlobalVariables.GameObjectFeaturesClassesFilePath,
                              GameObjectFeaturesClasses);
            SaveStorageToFile(GlobalVariables.GameObjectFeaturesFilePath,
                              GameObjectFeatures);
            SaveStorageToFile(GlobalVariables.GameObjectAttributesFilePath,
                              GameObjectAttributes);

            ElementaryGameObjectGroups.Save();
            GameObjectGroups.Save();
            ElementaryGameObjects.Save();
            GameObjects.Save();
        }

        public void Load()
        {
            CloseSQLiteConnections();

            LoadStorageFromFile(GlobalVariables.GameObjectPropertiesClassesFilePath,
                                GameObjectPropertiesClasses);
            LoadStorageFromFile(GlobalVariables.GameObjectPropertiesFilePath,
                                GameObjectProperties);
            LoadStorageFromFile(GlobalVariables.GameObjectFeaturesClassesFilePath,
                                GameObjectFeaturesClasses);
            LoadStorageFromFile(GlobalVariables.GameObjectFeaturesFilePath,
                                GameObjectFeatures);
            LoadStorageFromFile(GlobalVariables.GameObjectAttributesFilePath,
                                GameObjectAttributes);

            LoadSQLiteStorageFromFile(GlobalVariables.ElementaryGameObjectsFilePath,
                                      ElementaryGameObjects);
            LoadSQLiteStorageFromFile(GlobalVariables.GameObjectsFilePath,
                                      GameObjects);

            LoadSQLiteStorageFromFile(GlobalVariables.ElementaryGameObjectGroupsFilePath,
                          ElementaryGameObjectGroups);
            LoadSQLiteStorageFromFile(GlobalVariables.GameObjectGroupsFilePath,
                                      GameObjectGroups);

            EstablishGlobalObjectsLinks();
        }

        public void CloseSQLiteConnections()
        {
            ElementaryGameObjectGroups.CloseConnection();
            GameObjectGroups.CloseConnection();
            ElementaryGameObjects.CloseConnection();
            GameObjects.CloseConnection();
        }

        public void RestoreSQLiteConnections()
        {
            ElementaryGameObjectGroups.RestorePreviousConnection();
            GameObjectGroups.RestorePreviousConnection();
            ElementaryGameObjects.RestorePreviousConnection();
            GameObjects.RestorePreviousConnection();
        }
    }
}