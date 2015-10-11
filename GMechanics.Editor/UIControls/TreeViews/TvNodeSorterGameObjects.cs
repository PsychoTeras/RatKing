using System;
using System.Collections;
using System.Windows.Forms;

namespace GMechanics.Editor.UIControls.TreeViews
{
    public class TvNodeSorterGameObjects : IComparer
    {

#region IComparer Members

        public int Compare(object x, object y)
        {
            TreeNode tx = x as TreeNode, ty = y as TreeNode;
            if (tx != null && ty != null)
            {
                if (tx.Tag != null && ty.Tag != null)
                {
                    Type type1 = tx.Tag.GetType();
                    Type type2 = ty.Tag.GetType();
                    if (type1 == type2)
                    {
                        return String.CompareOrdinal(tx.Text, ty.Text);
                    }
                    return type1.Name.Contains("Group") ? -1 : 1;
                }
                return tx.Index > ty.Index ? -1 : 1;
            }
            return 0;
        }

#endregion

    }
}
