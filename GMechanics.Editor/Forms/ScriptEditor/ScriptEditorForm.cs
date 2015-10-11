using System;
using System.Drawing;
using System.Windows.Forms;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.GameObjectEventClasses;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.GameScript;
using GMechanics.Core.Helpers;
using GMechanics.Editor.Data;
using GMechanics.Editor.Helpers;
using GMechanics.Editor.UIControls.Common;
using GMechanics.Editor.UIControls.TreeViewPathEdit;
using GMechanics.ScriptTextBox;
using Helper = GMechanics.Editor.Helpers.Helper;

namespace GMechanics.Editor.Forms.ScriptEditor
{
    public partial class ScriptEditorForm : Form
    {
        public static GameObject DebugObject;
        public static GameObject DebugSubject;

        private readonly Atom _atom;
        private readonly GameObjectEvent _gameObjectEvent;
        private readonly ScriptProcessor _scriptProcessor = new ScriptProcessor();

        private InteractiveEventType SelectedEventType
        {
            get { return (InteractiveEventType) cbEventType.SelectedIndex; }
        }

        public ScriptEditorForm(Atom atom, GameObjectEvent gameObjectEvent)
        {
            InitializeComponent();

            _atom = atom;
            _gameObjectEvent = gameObjectEvent;
            _scriptProcessor.OnLogEvent += Print;

            //Fill EventType combobox by Game Object Event Types
            int curIdx = -1, idx = 0;
            Array values = Enum.GetValues(typeof(InteractiveEventType));
            foreach (InteractiveEventType value in values)
            {
                cbEventType.Items.Add(string.Format("Event type: {0}", value));
                if (!gameObjectEvent.IsEventEmpty(value))
                {
                    curIdx = idx;
                }
                idx++;
            }
            cbEventType.SelectedIndex = curIdx == -1 ? 0 : curIdx;

            //Reload object treeview
            tvObjects.ReloadTreeView();
        }

        private bool CheckOnScriptSourceChanged()
        {
            if (teScript.IsChanged)
            {
                return MessageBox.Show("Script source was changed. Discard all changes without saving?",
                                       "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) ==
                                       DialogResult.Yes;
            }
            return true;
        }

        private void LoadScript(InteractiveEventType eventType)
        {
            // Read script source code from database
            ScriptSourceData scriptSource = LocalStorage.Instance.
                ScriptsSources.GetScriptSource(_atom, eventType);

            // Load script text into the script editor window
            teScript.Text = scriptSource.Source;

            // Set ScriptActive checkbox state according to IsActive script flag
            cbScriptActive.Click -= CbScriptActiveClick;
            cbScriptActive.Checked = scriptSource.IsActive;
            cbScriptActive.Click += CbScriptActiveClick;

            // Collapse folding block for all functions, except main function, and for all regions
            for (int i = 0; i < teScript.Lines.Count; i++)
            {
                string line = teScript.Lines[i].Trim();
                bool isRegion = line.StartsWith("#region", StringComparison.OrdinalIgnoreCase);
                if ((line.StartsWith("function", StringComparison.OrdinalIgnoreCase) &&
                    !line.Contains("main()")) || isRegion)
                {
                    teScript.CollapseFoldingBlock(i + (line.Contains("{") || isRegion ? 0 : 1));
                }
            }

            // Prepare GUI controls
            UpdateEditorColorAccordingToScriptActive();
            teScript.IsChanged = false;
        }

        private void SaveScript()
        {
            string scriptSource = teScript.Text;
            LocalStorage.Instance.ScriptsSources.SaveScriptSource(_atom, SelectedEventType, 
                scriptSource, cbScriptActive.Checked);
        }

        private void SetByteCodeForSelectedEvent(byte[] byteCode)
        {
            switch (SelectedEventType)
            {
                case InteractiveEventType.Assigning:
                    _gameObjectEvent.OnAssigningByteCode = byteCode;
                    break;
                case InteractiveEventType.Assigned:
                    _gameObjectEvent.OnAssignedByteCode = byteCode;
                    break;
                case InteractiveEventType.Removing:
                    _gameObjectEvent.OnRemovingByteCode = byteCode;
                    break;
                case InteractiveEventType.Removed:
                    _gameObjectEvent.OnRemovedByteCode = byteCode;
                    break;
                case InteractiveEventType.Changing:
                    _gameObjectEvent.OnChangingByteCode = byteCode;
                    break;
                case InteractiveEventType.Changed:
                    _gameObjectEvent.OnChangedByteCode = byteCode;
                    break;
                case InteractiveEventType.Interact:
                    _gameObjectEvent.OnInteractByteCode = byteCode;
                    break;
            }
        }

        private void CompileScript()
        {
            // Prepare GUI controls
            tbScriptOut.Clear();
            btnCompileScript.Enabled = false;
            Application.DoEvents();

            // Compilation started
            tbScriptOut.Text += LogString("Compilation started.");

            try
            {
                // Start HR timer
                HRTimer hrTimer = HRTimer.CreateAndStart();

                // If script is active, compile source
                byte[] byteCode = null;
                if (cbScriptActive.Checked)
                {
                    byteCode = _scriptProcessor.Compile(teScript.Text);
                }

                // If compilation was done without any exceptions
                // 1. Set bytecode for selected event
                SetByteCodeForSelectedEvent(byteCode);

                // 2. Store script source into database
                SaveScript();

                // Stop HR timer and print compilation info
                string res = string.Format("Compilation succeeded by {0} ms.", hrTimer.StopWatch());
                tbScriptOut.Text += LogString(res);

                // Script was compiled and saved successfully, so turn IsChanged flag to false
                teScript.IsChanged = false;
            }
            catch (Exception ex)
            {
                tbScriptOut.Text += LogString(string.Format("{0}", ex).Replace("loading or ", ""));
            }
            
            // Write separation line into the log window and restore GUI controls
            tbScriptOut.Text += LogString("---------------------------------");
            btnCompileScript.Enabled = cbScriptActive.Checked;
        }
        
        private string LogString(string message)
        {
            return string.Format("{0}: {1}{2}", DateTime.Now, message, Environment.NewLine);
        }

        private void Print(string message)
        {
            tbScriptOut.Text += LogString(message);
        }

        private void TvpeMainButtonClick(object sender, ButtonClickEventArgs e)
        {
            Helper.LockUpdate(this);
            tvObjects.TreeView.SelectedNode = e.TreeNode;
            Helper.UnlockUpdate(this);
        }

        private void TvObjectsNodeSelect(object s, TreeNode node)
        {
            tvpeObjects.RefreshItems();
        }

        private void BtnCompileScriptClick(object sender, EventArgs e)
        {
            CompileScript();
        }

        private void ScriptEditorFormKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F4:
                    {
                        if (!e.Alt)
                        {
                            cbScriptActive.Checked = !cbScriptActive.Checked;
                            CbScriptActiveClick(cbScriptActive, null);
                        }
                        break;
                    }
                case Keys.F5:
                    {
                        if (cbScriptActive.Checked)
                            CompileScript();
                        break;
                    }
                case  Keys.Escape:
                    {
                        Close();
                        break;
                    }
            }
        }

        private void CbScriptActiveClick(object sender, EventArgs e)
        {
            UpdateEditorColorAccordingToScriptActive();
            if (cbScriptActive.Checked)
            {
                CompileScript();
            }
            else
            {
                tbScriptOut.Clear();
            }
        }

        private void UpdateEditorColorAccordingToScriptActive()
        {
            cbScriptActive.Text = cbScriptActive.Checked
                ? "Script is active"
                : "Script is inactive";
            cbScriptActive.Image = cbScriptActive.Checked
                ? Properties.Resources.bullet_ball_glass_green
                : Properties.Resources.bullet_ball_glass_red;
            teScript.BackColor = cbScriptActive.Checked ? Color.White : Color.WhiteSmoke;
            btnCompileScript.Enabled = cbScriptActive.Checked;
            teScript.ReadOnly = !cbScriptActive.Checked;
        }

        private void TeScriptTextChanged(object sender, TextChangedEventArgs e)
        {
            teScript.IsChanged = true;
        }

        private void ScriptEditorFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !CheckOnScriptSourceChanged();
        }

        private void cbEventType_DroppingDown(object s, ComboBoxEx.DroppingDownEventArgs e)
        {
            e.Cancel = !CheckOnScriptSourceChanged();
        }

        private void CbEventTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            LoadScript(SelectedEventType);
            teScript.Select();
            teScript.Focus();
        }
    }
}
