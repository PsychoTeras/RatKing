using System.ComponentModel;
using System.Windows.Forms;
using GMechanics.Core.Classes.Entities;
using GMechanics.Editor.UIControls.Common;

namespace GMechanics.Editor.UIControls.TreeViews
{
    public partial class BaseTreeView : UserControl
    {

#region Delegates

        public delegate void NodeSelectHandler(object s, TreeNode node);

#endregion

        protected BaseTreeView()
        {
            InitializeComponent();
        }

        public virtual TreeViewEx TreeView { get; set; }
        public virtual object SelectedObject { get; set; }

        [Category("Action")]
        public event NodeSelectHandler NodeSelect;

        protected void InvokeNodeSelect(TreeNode node)
        {
            NodeSelectHandler handler = NodeSelect;
            if (handler != null)
            {
                handler(this, node);
            }
        }

        public virtual void ChangeObjectName(string oldName) { }

        public virtual void AfterUpdateObjectNameOrTranscription(Atom atom)
        {
            TreeView.SelectedNode.Text = atom.ShortDisplayName;           
        }
    }
}