using System.Collections;
using System.Windows.Forms;

namespace GMechanics.Editor.UIControls.TreeViews
{
    public class TvNodeSorter : IComparer
    {

#region IComparer Members

        public int Compare(object x, object y)
        {
            TreeNode tx = x as TreeNode, ty = y as TreeNode;
            return tx != null && ty != null ? string.CompareOrdinal(tx.Text, ty.Text) : 0;
        }

#endregion

    }
}