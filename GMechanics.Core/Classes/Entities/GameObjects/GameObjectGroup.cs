using System.ComponentModel;

namespace GMechanics.Core.Classes.Entities.GameObjects
{
    public sealed class GameObjectGroup : Atom
    {

#region Properties

        [Browsable(false)]
        public GameObjectGroup Parent { get; internal set; }

        [Browsable(false)]
        public ElementaryGameObjectsList GameObjects { get; internal set; }

        [Browsable(false)]
        public string Path
        {
            get
            {
                string path = Name;
                GameObjectGroup parent = Parent;
                while (parent != null)
                {
                    path = string.Format(@"{0}\{1}", parent.Name, path);
                    parent = parent.Parent;
                }
                return path;
            }
        }

        [Browsable(false)]
        public int GameObjectsCount
        {
            get { return GameObjects.Count; }
        }

        [Browsable(false)]
        public override InteractiveRecipientsList InteractiveRecipients { get; set; }

#endregion

#region Class members

        public GameObjectGroup()
        {
            GameObjects = new ElementaryGameObjectsList();
        }

        public GameObjectGroup(string name, string transcription, 
            string description) : this()
        {
            Name = name;
            Transcription = transcription;
            Description = description;
        }

        public GameObjectGroup(string name, string transcription,
            string description, GameObjectGroup parent)
            : this(name, transcription, description)
        {
            Parent = parent;
        }

#endregion

#region Game objects

        public bool AddGameObject(ElementaryGameObject gameObject)
        {
            if (gameObject != null && !GameObjects.Contains(gameObject))
            {
                GameObjects.Add(gameObject);
                return true;
            }
            return false;
        }

        public ElementaryGameObject GetGameObject(int index)
        {
            return GameObjects[index];
        }

        public ElementaryGameObject GetGameObject(string gameObjectName)
        {
            return GameObjects[gameObjectName];
        }

        public bool GameObjectExists(string gameObjectName)
        {
            return GetGameObject(gameObjectName) != null;
        }

        public bool RemoveGameObject(ElementaryGameObject gameObject)
        {
            return GameObjects.Remove(gameObject);
        }

        public bool RemoveGameObject(string gameObjectName)
        {
            return RemoveGameObject(GetGameObject(gameObjectName));
        }

#endregion

    }
}
