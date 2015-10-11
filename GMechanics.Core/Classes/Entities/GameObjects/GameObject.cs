#region Usings

using GMechanics.Core.Classes.Types;

#endregion

namespace GMechanics.Core.Classes.Entities.GameObjects
{
    public sealed class GameObject : ElementaryGameObject
    {

#region Class members

        public Size3D Size { get; set; }
        public float Weight { get; set; }
        public float Speed { get; set; }
        public float Direction { get; set; }
        public Location3D Location { get; set; }

#endregion

#region Class functions

        public GameObject()
        {
            Initialize();
        }

        public GameObject(ElementaryGameObject parent)
            : base(parent)
        {
            Initialize();
        }

        public GameObject(string name, string transcription, string description)
            : base(name, transcription, description)
        {
            Initialize();
        }

        public GameObject(string name, string transcription, string description,
            ElementaryGameObject parent) : base(name, transcription, description, parent)
        {
            Initialize();
        }

        private void Initialize()
        {
            Speed = Weight = 1.0f;
            Size = new Size3D(1.0f, 1.0f, 1.0f);
            Location = new Location3D(0, 0, 0);
        }

#endregion

    }
}