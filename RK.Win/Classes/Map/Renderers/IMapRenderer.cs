using System.Drawing;
using RK.Common.Map;
using RK.Win.Controls;

namespace RK.Win.Classes.Map.Renderers
{
    public interface IMapRenderer
    {
        void Render(MapControl mapCtrl, Graphics buffer, Rectangle area);
        void ChangeScaleFactor(MapControl mapCtrl, float scaleFactor);
        void ChangeMap(MapControl mapCtrl);
        void UpdateTile(MapControl mapCtrl, ushort x, ushort y, Tile tile);
    }
}
