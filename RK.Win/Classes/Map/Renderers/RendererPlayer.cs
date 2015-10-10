using System.Drawing;
using RK.Common.Classes.Map;
using RK.Win.Controls;

namespace RK.Win.Classes.Map.Renderers
{
    public class RendererPlayer : IMapRenderer
    {

#region Private fields


#endregion

#region Ctor

        public RendererPlayer()
        {
        }

#endregion

#region IMapRenderer

        public void Render(MapControl mapCtrl, Graphics buffer, Rectangle area) { }

        public void ChangeScaleFactor(MapControl mapCtrl, float scaleFactor) { }

        public void ChangeMap(MapControl mapCtrl) { }

        public void UpdateTile(MapControl mapCtrl, ushort x, ushort y, Tile tile) { }

#endregion

    }
}