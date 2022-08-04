using EcfFileViewTools;
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
        private EcfDataGridView Grid = new EcfDataGridView();

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

            Grid.AllowUserToOrderColumns = true;
            Grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            Grid.Dock = DockStyle.Fill;
            Grid.ReadOnly = true;

            Grid.Paint += DefinitionsGrid_Paint;

            Grid.Columns.Add(new DataGridViewTextBoxColumn() 
            { 
                HeaderText = TitleRecources.Generic_Name, 
                SortMode = DataGridViewColumnSortMode.Automatic 
            });
            Grid.Columns.Add(new DataGridViewTextBoxColumn() 
            { 
                HeaderText = TitleRecources.Generic_Info, 
                SortMode = DataGridViewColumnSortMode.Automatic 
            });
            Grid.Columns.Add(new DataGridViewCheckBoxColumn()
            { 
                HeaderText = TitleRecources.Generic_IsOptional, 
                SortMode = DataGridViewColumnSortMode.Automatic 
            });
            Grid.Columns.Add(new DataGridViewCheckBoxColumn()
            { 
                HeaderText = TitleRecources.DeprecatedDefinitionsDialog_ColumnHeaderHasValue, 
                SortMode = DataGridViewColumnSortMode.Automatic 
            });
            Grid.Columns.Add(new DataGridViewCheckBoxColumn()
            {
                HeaderText = TitleRecources.DeprecatedDefinitionsDialog_ColumnHeaderAllowBlank,
                SortMode = DataGridViewColumnSortMode.Automatic
            });
            Grid.Columns.Add(new DataGridViewCheckBoxColumn() 
            { 
                HeaderText = TitleRecources.DeprecatedDefinitionsDialog_ColumnHeaderIsForceEscaped, 
                SortMode = DataGridViewColumnSortMode.Automatic 
            });

            Controls.Add(Grid);
        }
        private void OkButton_Click(object sender, EventArgs evt)
        {
            Close();
        }
        private void DefinitionsGrid_Paint(object sender, PaintEventArgs evt)
        {
            if (Grid.Rows.Count == 0)
            {
                TextRenderer.DrawText(evt.Graphics, EmptyDataText,
                    Grid.Font, Grid.ClientRectangle, Grid.ForeColor, Grid.BackgroundColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        public void ShowDialog(IWin32Window parent, EgsEcfFile file)
        {
            DefinitionFileLabel.Text = string.Format("{0}: {1}", TitleRecources.DeprecatedDefinitionsDialog_DefinitionFile, Path.GetFileName(file.Definition.FilePathAndName));
            CompareFileLabel.Text = string.Format("{0}: {1}", TitleRecources.DeprecatedDefinitionsDialog_CompareFile, file.FileName);

            Grid.SuspendLayout();
            Grid.Rows.Clear();
            FindDeprecatedItemDefinitions(file).ForEach(definition =>
            {
                DataGridViewRow row = new DataGridViewRow();
                row.Cells.Add(new DataGridViewTextBoxCell() { Value = definition.Name });
                row.Cells.Add(new DataGridViewTextBoxCell() { Value = definition.Info });
                row.Cells.Add(new DataGridViewCheckBoxCell() { Value = definition.IsOptional });
                row.Cells.Add(new DataGridViewCheckBoxCell() { Value = definition.HasValue });
                row.Cells.Add(new DataGridViewCheckBoxCell() { Value = definition.AllowBlank });
                row.Cells.Add(new DataGridViewCheckBoxCell() { Value = definition.IsForceEscaped });
                Grid.Rows.Add(row);
            });
            Grid.ClearSelection();
            Grid.AutoResizeColumns();
            Grid.ResumeLayout();

            OkButton.Focus();
            ShowDialog(parent);
        }
    }
}
