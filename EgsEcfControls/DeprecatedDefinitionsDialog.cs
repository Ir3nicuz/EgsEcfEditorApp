using EgsEcfParser;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static EgsEcfParser.EcfFormatting;

namespace EgsEcfControls
{
    public partial class DeprecatedDefinitionsDialog : Form
    {
        private string EmptyDataText { get; set;  }

        public DeprecatedDefinitionsDialog(Icon appIcon)
        {
            InitializeComponent();
            InitForm(appIcon);
        }

        // events
        private void InitForm(Icon appIcon)
        {
            Icon = appIcon;
            Text = Properties.titles.DeprecatedDefinitionsDialog_Header;
            EmptyDataText = Properties.texts.DeprecatedDefinitionsDialog_EmptyDataText;

            OkButton.Text = Properties.titles.Generic_Ok;

            DefinitionsGrid.Paint += DefinitionsGrid_Paint;

            DefinitionsGrid.Columns.Add(new DataGridViewTextBoxColumn() { 
                HeaderText = Properties.titles.Generic_Name, SortMode = DataGridViewColumnSortMode.Automatic });
            DefinitionsGrid.Columns.Add(new DataGridViewTextBoxColumn() { 
                HeaderText = Properties.titles.Generic_Info, SortMode = DataGridViewColumnSortMode.Automatic });
            DefinitionsGrid.Columns.Add(new DataGridViewCheckBoxColumn() { 
                HeaderText = Properties.titles.Generic_IsOptional, SortMode = DataGridViewColumnSortMode.Automatic });
            DefinitionsGrid.Columns.Add(new DataGridViewCheckBoxColumn() { 
                HeaderText = Properties.titles.DeprecatedDefinitionsDialog_ColumnHeaderHasValue, SortMode = DataGridViewColumnSortMode.Automatic });
            DefinitionsGrid.Columns.Add(new DataGridViewCheckBoxColumn() { 
                HeaderText = Properties.titles.DeprecatedDefinitionsDialog_ColumnHeaderIsForceEscaped, SortMode = DataGridViewColumnSortMode.Automatic });
        }
        private void OkButton_Click(object sender, EventArgs evt)
        {
            Close();
        }
        private void DefinitionsGrid_Paint(object sender, PaintEventArgs evt)
        {
            if (DefinitionsGrid.Rows.Count == 0)
            {
                TextRenderer.DrawText(evt.Graphics, EmptyDataText,
                    DefinitionsGrid.Font, DefinitionsGrid.ClientRectangle, DefinitionsGrid.ForeColor, DefinitionsGrid.BackgroundColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        public void ShowDialog(IWin32Window parent, EgsEcfFile file)
        {
            DefinitionFileLabel.Text = string.Format("{0}: {1}", Properties.titles.DeprecatedDefinitionsDialog_DefinitionFile, Path.GetFileName(file.Definition.FilePathAndName));
            CompareFileLabel.Text = string.Format("{0}: {1}", Properties.titles.DeprecatedDefinitionsDialog_CompareFile, file.FileName);

            DefinitionsGrid.Rows.Clear();
            FindDeprecatedItemDefinitions(file).ForEach(definition =>
            {
                DataGridViewRow row = new DataGridViewRow();
                row.Cells.Add(new DataGridViewTextBoxCell() { Value = definition.Name });
                row.Cells.Add(new DataGridViewTextBoxCell() { Value = definition.Info });
                row.Cells.Add(new DataGridViewCheckBoxCell() { Value = definition.IsOptional });
                row.Cells.Add(new DataGridViewCheckBoxCell() { Value = definition.HasValue });
                row.Cells.Add(new DataGridViewCheckBoxCell() { Value = definition.IsForceEscaped });
                DefinitionsGrid.Rows.Add(row);
            });
            DefinitionsGrid.ClearSelection();

            OkButton.Focus();
            ShowDialog(parent);
        }
    }
}
