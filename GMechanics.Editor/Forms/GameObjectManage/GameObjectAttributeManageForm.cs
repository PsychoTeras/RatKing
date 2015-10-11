using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using GMechanics.Core.Classes.Entities.ParentalGameObjectAttributeClasses;
using GMechanics.Core.Classes.Interfaces;
using GMechanics.Core.Helpers;
using GMechanics.Editor.Helpers;
using Helper = GMechanics.Core.Helpers.Helper;

namespace GMechanics.Editor.Forms.GameObjectManage
{
    public sealed partial class GameObjectAttributeManageForm : Form, ICustomEditor
    {
        private static Size _formSize;

        private readonly bool _editing;
        private readonly string _oldAttributeName;

        private readonly ParentalGameObjectAttributeValuesList _values;
        private readonly AtributeValuesListMatchingMap _valuesMatchingMap;
        private readonly bool _valuesOnly;

        public Helper.IsAttributeNameExistsCheckHandler IsAttributeNameExists;
        private string _prewAttributeNameText;

        public GameObjectAttributeManageForm(bool editing, bool valuesOnly, string oldAttributeName, 
                                             string oldTranscription, ParentalGameObjectAttributeValuesList values,
                                             int nestingLevel, Helper.IsAttributeNameExistsCheckHandler handler)
        {
            InitializeComponent();

            if (!_formSize.IsEmpty)
            {
                Size = _formSize;
            }
            dgValues.AutoGenerateColumns = false;

            _editing = editing;
            _valuesOnly = valuesOnly;
            IsAttributeNameExists = handler;
            _prewAttributeNameText = string.Empty;

            _values = (values != null ? values.Clone() : new ParentalGameObjectAttributeValuesList());
            _valuesMatchingMap = Helpers.Helper.CreateMatchingMap(values, _values);
            _values.NestingLevel = nestingLevel;

            BindListToDataGrid();
            OnGridSelectionChanged();

            if (_editing)
            {
                tbAttributeName.Text = _oldAttributeName = _prewAttributeNameText = oldAttributeName;
                tbTranscription.Text = oldTranscription;
            }

            string[] caption = new[]
                              {
                                  "Add new attribute",
                                  "Edit attribute",
                                  "Edit attribute values"
                              };
            Text = caption[valuesOnly ? 2 : _editing ? 1 : 0];

            if (_valuesOnly)
            {
                HideAttributeSettings();
            }
        }

        public string AttributeName
        {
            get { return tbAttributeName.Text.Trim(); }
        }

        public string Transcription
        {
            get { return tbTranscription.Text.Trim(); }
        }

        public ParentalGameObjectAttributeValuesList Values
        {
            get { return _values; }
        }

        public object Result
        {
            get { return _values; }
        }

        public AtributeValuesListMatchingMap ValuesMatchingMap
        {
            get { return _valuesMatchingMap; }
        }

        private void HideAttributeSettings()
        {
            pAttributeSettings.Hide();
            lblLoV.Top -= pAttributeSettings.Height;
            pValues.Top -= pAttributeSettings.Height;
            pValues.Height += pAttributeSettings.Height;
            Height -= pAttributeSettings.Height;
        }

        private void GameObjectAttributeManageFormLoad(object sender, EventArgs e)
        {
            tbAttributeName.Select();
            tbAttributeName.SelectAll();
        }

        private void BindListToDataGrid()
        {
            if (dgValues.CurrentCell != null && dgValues.CurrentCell.IsInEditMode)
            {
                dgValues.EndEdit();
            }
            BindingSource source = new BindingSource {DataSource = _values};
            dgValues.DataSource = source;
        }

        private void SelectDataGrid()
        {
            dgValues.Focus();
            if (dgValues.CurrentCell != null)
            {
                dgValues.CurrentCell.Selected = true;
            }
        }

        private void OnGridSelectionChanged()
        {
            btnEdit.Enabled = btnDelete.Enabled = dgValues.SelectedCells.Count > 0;
        }

        private bool ValidateCurrentCell()
        {
            if (dgValues.CurrentCell != null && dgValues.CurrentCell.IsInEditMode)
            {
                string value = dgValues.EditingControl.Text.Trim();

                switch (dgValues.CurrentCell.ColumnIndex)
                {
                    case 0:
                        {
                            if (string.IsNullOrEmpty(value))
                            {
                                MessageBox.Show("Value cannot be empty", "Error", MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                                return false;
                            }

                            if (IsValueExistsInList(value, dgValues.CurrentCell.RowIndex))
                            {
                                MessageBox.Show("Same value currently exist", "Error", MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                                return false;
                            }
                            break;
                        }

                    case 2:
                    case 3:
                        {
                            float f;
                            bool res = float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture,
                                                      out f);
                            if (!res)
                            {
                                MessageBox.Show(string.Format("'{0}' is not a digit value.", value), "Error",
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }
                            break;
                        }
                }
            }
            return true;
        }

        private void AddNewRow()
        {
            if (ValidateCurrentCell())
            {
                _values.Add(new ParentalGameObjectAttributeValue(_values.NestingLevel));
                BindListToDataGrid();
                dgValues.Rows[dgValues.Rows.Count - 1].Cells[0].Selected = true;
                EditSelectedCell();
            }
        }

        private void EditSelectedCell()
        {
            if (dgValues.CurrentCell != null)
            {
                if (!dgValues.CurrentCell.IsInEditMode)
                {
                    dgValues.BeginEdit(false);
                }
                else
                {
                    if (ValidateCurrentCell())
                    {
                        dgValues.EndEdit();
                    }
                }
                SelectDataGrid();
            }
        }

        private void DeleteSelectedRow()
        {
            if (dgValues.CurrentCell != null)
            {
                int idx = dgValues.CurrentCell.RowIndex;
                _values.RemoveAt(idx);
                BindListToDataGrid();
                if (_values.Count > 0)
                {
                    if (idx == _values.Count)
                    {
                        idx--;
                    }
                    dgValues.Rows[idx].Cells[0].Selected = true;
                }
            }
        }

        private void BtnAddClick(object sender, EventArgs e)
        {
            AddNewRow();
        }

        private void BtnEditClick(object sender, EventArgs e)
        {
            EditSelectedCell();
        }

        private void BtnDeleteClick(object sender, EventArgs e)
        {
            DeleteSelectedRow();
        }

        private void DgValuesCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                BindListToDataGrid();
                dgValues.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
            }
        }

        private void DgValuesSelectionChanged(object sender, EventArgs e)
        {
            OnGridSelectionChanged();
        }

        private bool IsValueExistsInList(string value, int curIdx)
        {
            return _values.FirstOrDefault(item => item.Name.Equals(value, 
                StringComparison.OrdinalIgnoreCase) && _values.IndexOf(item) != curIdx) != null;
        }

        private void DgValuesCellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            e.Cancel = !ValidateCurrentCell();
        }

        private void DgValuesCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgValues.CurrentCell.Value != null && e.ColumnIndex == 0 && 
                string.IsNullOrEmpty(dgValues.CurrentCell.Value.ToString().Trim()))
            {
                DeleteSelectedRow();
            }
        }

        private void DgValuesKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Insert:
                    {
                        AddNewRow();
                        break;
                    }
                case Keys.Down:
                    {
                        if (dgValues.CurrentCell == null ||
                            dgValues.CurrentCell.RowIndex == dgValues.Rows.Count - 1)
                        {
                            AddNewRow();
                        }
                        break;
                    }
                case Keys.Delete:
                    {
                        DeleteSelectedRow();
                        break;
                    }
            }
        }

        private void GameObjectAttributeManageFormKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    {
                        BtnCancelClick(this, null);
                        break;
                    }
                case Keys.Enter:
                    {
                        BtnOkClick(this, null);
                        break;
                    }
            }
        }

        private void TbAttributeNameTextChanged(object sender, EventArgs e)
        {
            if (tbTranscription.Text == _prewAttributeNameText)
            {
                _prewAttributeNameText = tbTranscription.Text = tbAttributeName.Text;
            }
        }

        private void BtnCancelClick(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnOkClick(object sender, EventArgs e)
        {
            if (!_valuesOnly)
            {
                string attributeName = tbAttributeName.Text.Trim();

                if (string.IsNullOrEmpty(attributeName))
                {
                    MessageBox.Show("Attribute name cannot be empty", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbAttributeName.Focus();
                    return;
                }

                if (IsAttributeNameExists != null && IsAttributeNameExists(attributeName) &&
                    (!_editing || _oldAttributeName != attributeName))
                {
                    MessageBox.Show("Attribute with same name currently exist", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbAttributeName.Focus();
                    return;
                }

                if (_values.Count == 0)
                {
                    MessageBox.Show("Possible values list must contain one or more records",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dgValues.Focus();
                    return;
                }
            }

            DialogResult = DialogResult.OK;
        }

        private void GameObjectAttributeManageFormFormClosed(object sender, FormClosedEventArgs e)
        {
            _formSize = new Size(Width, Height + (_valuesOnly ? pAttributeSettings.Height : 0));
        }
    }
}