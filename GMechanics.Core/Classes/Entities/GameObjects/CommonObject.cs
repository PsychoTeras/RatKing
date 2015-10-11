#region Usings

using System;
using GMechanics.Core.Classes.Types;

#endregion

namespace GMechanics.Core.Classes.GameObjects
{
    [Serializable]
    public sealed class CommonObject : ElementaryObject
    {

#region Class members

        public Size3D Size { get; set; }
        public float Weight { get; set; }
        public float Speed { get; set; }
        public float Direction { get; set; }
        public Location3D Location { get; set; }

#endregion

#region Class functions

        public CommonObject()
        {
            Initialize();
        }

        public CommonObject(CommonObject parent) : base(parent)
        {
            Initialize();
        }

        public CommonObject(string name, string transcription, string description,
                            ElementaryObject parent)
            : base(name, transcription, description, parent)
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