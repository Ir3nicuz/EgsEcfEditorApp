using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.IO;
using System.Windows.Forms;
using static EgsEcfParser.EcfDefinitionHandling;

namespace EcfFileViews
{
    public partial class DeprecatedDefinitionsDialog : Form
    {
        private string EmptyDataText { get; set;  }

        public DeprecatedDefinitionsDialog()
        {
            InitializeComponent();
            InitForm();
        }

        // events
        private void InitForm()
        {
            Icon = IconRecources.Icon_App;
            Text = TitleRecources.DeprecatedDefinitionsDialog_Header;
            EmptyDataText = TextRecources.DeprecatedDefinitionsDialog_EmptyDataText;

            OkButton.Text = TitleRecources.Generic_Ok;

            DefinitionsGrid.Paint += DefinitionsGrid_Paint;

            DefinitionsGrid.Columns.Add(new DataGridViewTextBoxColumn() 
            { 
                HeaderText = TitleRecources.Generic_Name, 
                SortMode = DataGridViewColumnSortMode.Automatic 
            });
            DefinitionsGrid.Columns.Add(new DataGridViewTextBoxColumn() 
            { 
                HeaderText = TitleRecources.Generic_Info, 
                SortMode = DataGridViewColumnSortMode.Automatic 
            });
            DefinitionsGrid.Columns.Add(new DataGridViewCheckBoxColumn()
            { 
                HeaderText = TitleRecources.Generic_IsOptional, 
                SortMode = DataGridViewColumnSortMode.Automatic 
            });
            DefinitionsGrid.Columns.Add(new DataGridViewCheckBoxColumn()
            { 
                HeaderText = TitleRecources.DeprecatedDefinitionsDialog_ColumnHeaderHasValue, 
                SortMode = DataGridViewColumnSortMode.Automatic 
            });
            DefinitionsGrid.Columns.Add(new DataGridViewCheckBoxColumn()
            {
                HeaderText = TitleRecources.DeprecatedDefinitionsDialog_ColumnHeaderAllowBlank,
                SortMode = DataGridViewColumnSortMode.Automatic
            });
            DefinitionsGrid.Columns.Add(new DataGridViewCheckBoxColumn() 
            { 
                HeaderText = TitleRecources.DeprecatedDefinitionsDialog_ColumnHeaderIsForceEscaped, 
                SortMode = DataGridViewColumnSortMode.Automatic 
            });
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
            DefinitionFileLabel.Text = string.Format("{0}: {1}", TitleRecources.DeprecatedDefinitionsDialog_DefinitionFile, Path.GetFileName(file.Definition.FilePathAndName));
            CompareFileLabel.Text = string.Format("{0}: {1}", TitleRecources.DeprecatedDefinitionsDialog_CompareFile, file.FileName);

            DefinitionsGrid.Rows.Clear();
            FindDeprecatedItemDefinitions(file).ForEach(definition =>
            {
                DataGridViewRow row = new DataGridViewRow();
                row.Cells.Add(new DataGridViewTextBoxCell() { Value = definition.Name });
                row.Cells.Add(new DataGridViewTextBoxCell() { Value = definition.Info });
                row.Cells.Add(new DataGridViewCheckBoxCell() { Value = definition.IsOptional });
                row.Cells.Add(new DataGridViewCheckBoxCell() { Value = definition.HasValue });
                row.Cells.Add(new DataGridViewCheckBoxCell() { Value = definition.AllowBlank });
                row.Cells.Add(new DataGridViewCheckBoxCell() { Value = definition.IsForceEscaped });
                DefinitionsGrid.Rows.Add(row);
            });
            DefinitionsGrid.ClearSelection();

            OkButton.Focus();
            ShowDialog(parent);
        }
    }
}
