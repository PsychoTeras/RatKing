using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GMechanics.Editor.Helpers;
using GMechanics.Editor.UIControls.TreeViews;

namespace GMechanics.Editor.UIControls.TreeViewPathEdit
{
    public partial class TreeViewPathEdit : UserControl
    {

 #region Delegates

        public delegate void ButtonClickHandler(object sender, ButtonClickEventArgs e);

#endregion

        private readonly List<TreeViewPathButton> _buttons;

        private BaseTreeView _baseTreeView;
        private Color _prevControlColor;
        private bool _showNodesIcons = true;
        private TextBox _textPathInput;
        private TreeView _treeView;

        public TreeViewPathEdit()
        {
            InitializeComponent();
            _buttons = new List<TreeViewPathButton>();
            RefreshItems();
        }

        [DefaultValue(true), Browsable(true)]
        public bool ShowNodesIcons
        {
            get { return _showNodesIcons; }
            set
            {
                _showNodesIcons = value;
                if (_textPathInput == null)
                {
                    RefreshItems();
                }
            }
        }

        [DefaultValue(null), Browsable(true)]
        public BaseTreeView TreeView
        {
            get { return _baseTreeView; }
            set
            {
                _baseTreeView = value;
                _treeView = _baseTreeView != null ? _baseTreeView.TreeView : null;
                RefreshItems();
            }
        }

        [Category("Action")]
        public event ButtonClickHandler ButtonClick;

        private void RealignItems()
        {
            int width = -1;
            foreach (TreeViewPathButton button in _buttons)
            {
                button.Parent = this;
                button.Location = new Point(width, -1);
                if (button.Icon == null)
                {
                    button.IconVisible = false;
                }
                button.Size = new Size(button.Width, Height);
                button.Click += OnButtonClick;
                width += button.Width;
            }
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            if (ButtonClick != null)
            {
                ButtonClick(sender, new ButtonClickEventArgs
                                        {TreeNode = (TreeNode) ((TreeViewPathButton) sender).Tag});
            }
        }

        private void OnMenuItemClick(object sender, EventArgs e)
        {
            if (ButtonClick != null)
            {
                ToolStripItem item = (ToolStripItem) sender;
                ContextMenuStrip menu = (ContextMenuStrip) item.Owner;
                menu.Closed -= OnArrowButtonClick;
                menu.Tag = null;
                ButtonClick(sender, new ButtonClickEventArgs {TreeNode = (TreeNode) ((ToolStripItem) sender).Tag});
            }
        }

        private void OnArrowButtonClick(object sender, EventArgs e)
        {
            TreeViewPathButton button = (TreeViewPathButton) sender;
            TreeNode node = (TreeNode) button.Tag;

            ContextMenuStrip menu = new ContextMenuStrip {Tag = button};
            menu.Closed += OnMenuClosed;
            foreach (TreeNode child in node.Nodes)
            {
                Image image = _treeView.ImageList != null && child.ImageIndex != -1
                                  ? _treeView.ImageList.Images[child.ImageIndex]
                                  : null;
                ToolStripItem item = menu.Items.Add(child.Text, image);
                item.Click += OnMenuItemClick;
                item.Tag = child;
            }

            if (menu.Items.Count > 0)
            {
                button.HoldUpdate();
                menu.Show(MousePosition);
            }
        }

        private static void OnMenuClosed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            TreeViewPathButton button = (TreeViewPathButton) ((ContextMenuStrip) sender).Tag;
            if (button != null)
            {
                button.UnholdUpdate();
            }
        }

        public void RefreshItems()
        {
            Helper.LockUpdate(this);

            foreach (TreeViewPathButton button in _buttons)
            {
                button.Dispose();
            }
            _buttons.Clear();

            if (_treeView != null && _treeView.SelectedNode != null)
            {
                TreeNode node = _treeView.SelectedNode;
                do
                {
                    TreeViewPathButton button = new TreeViewPathButton();
                    button.ButtonText = node.Text;
                    button.Icon = _showNodesIcons && _treeView.ImageList != null && node.ImageIndex != -1
                                      ? _treeView.ImageList.Images[node.ImageIndex]
                                      : null;
                    button.Tag = node;
                    button.ArrowButtonVisible = node.Nodes.Count > 0;
                    button.ArrowButtonClick += OnArrowButtonClick;
                    _buttons.Insert(0, button);
                } while ((node = node.Parent) != null);

                RealignItems();
            }

            Helper.UnlockUpdate(this);
        }

        private TreeNode FindNodeInRoot(string text)
        {
            text = text.ToLower();
            foreach (TreeNode node in _treeView.Nodes)
            {
                if (node.Parent == null && node.Text.ToLower().Equals(text))
                {
                    return node;
                }
            }
            return null;
        }

        private bool DoNavigateByTextPath()
        {
            if (_treeView != null && _treeView.Nodes.Count > 0)
            {
                string path = _textPathInput.Text.Trim();
                string[] split = path.Split(new[] {_treeView.PathSeparator}, StringSplitOptions.RemoveEmptyEntries);

                List<TreeNode> nodes = new List<TreeNode>();
                TreeNode node = FindNodeInRoot(split[0]);

                if (node != null)
                {
                    nodes.Add(node);
                    int cnt = split.Length;
                    for (int i = 1; i < cnt; i++)
                    {
                        bool founded = false;
                        string nodeName = split[i].ToLower();
                        foreach (TreeNode child in node.Nodes)
                        {
                            if (child.Text.ToLower().Equals(nodeName))
                            {
                                nodes.Add(node = child);
                                founded = true;
                                break;
                            }
                        }
                        if (!founded)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }

                foreach (TreeNode lnode in nodes)
                {
                    (node = lnode).Expand();
                }
                _treeView.SelectedNode = node;

                return true;
            }
            return false;
        }

        public void ShowTextPathInput()
        {
            if (_textPathInput == null)
            {
                Helper.LockUpdate(this);

                string path = string.Empty;
                foreach (TreeViewPathButton button in _buttons)
                {
                    if (_treeView != null)
                    {
                        path += (!string.IsNullOrEmpty(path) ? _treeView.PathSeparator : string.Empty) +
                                button.ButtonText;
                    }
                    button.Dispose();
                }
                _buttons.Clear();

                _textPathInput = new TextBox
                                     {
                                         Parent = this,
                                         BorderStyle = BorderStyle.None,
                                         Dock = DockStyle.Fill,
                                         Text = path
                                     };
                _textPathInput.KeyDown += OnTextPathKeyDown;
                _textPathInput.Leave += OnTextPathLeave;
                _textPathInput.Show();

                _prevControlColor = BackColor;
                BackColor = _textPathInput.BackColor;

                Helper.UnlockUpdate(this);
                _textPathInput.Focus();
                _textPathInput.SelectAll();
            }
        }

        public void HideTextPathInput()
        {
            if (_textPathInput != null)
            {
                Helper.LockUpdate(this);
                _textPathInput.KeyDown -= OnTextPathKeyDown;
                _textPathInput.Leave -= OnTextPathLeave;
                _textPathInput.Hide();
                _textPathInput.Dispose();
                _textPathInput = null;
                BackColor = _prevControlColor;
                RefreshItems();
            }
        }

        private void OnTextPathLeave(object sender, EventArgs e)
        {
            HideTextPathInput();
        }

        private void OnTextPathKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    {
                        HideTextPathInput();
                        break;
                    }
                case Keys.Enter:
                    {
                        if (DoNavigateByTextPath())
                        {
                            HideTextPathInput();
                        }
                        else
                        {
                            MessageBox.Show(
                                string.Format("Path '{0}' incorrect.\r\nPlease check the spelling and try again.",
                                              _textPathInput.Text.Trim()), "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                        break;
                    }
            }
        }

        private void TreeViewPathEditMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowTextPathInput();
            }
        }
    }
}