using System.Drawing;

namespace GMechanics.Editor.Data
{
    public sealed class FlowcharItemData
    {
        public Rectangle Location;
        public string SkinName;

        public FlowcharItemData() {}

        public FlowcharItemData(Rectangle location, string skinName)
        {
            Location = location;
            SkinName = skinName;
        }
    }
}
