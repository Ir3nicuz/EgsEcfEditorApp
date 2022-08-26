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
            Icon = IconRecources.Icon_AppBranding;
            Text = TitleRecources.DeprecatedDefinitionsDialog_Header;
            EmptyDataText = TextRecources.DeprecatedDefinitionsDialog_EmptyDataText;

            OkButton.Text = TitleRecources.Generic_Ok;

            Grid.Paint += DefinitionsGrid_Paint;

            NameColumn.HeaderText = TitleRecources.Generic_Name;
            InfoColumn.HeaderText = TitleRecources.Generic_Info;
            IsOptionalColumn.HeaderText = TitleRecources.Generic_IsOptional;
            HasValueColumn.HeaderText = TitleRecources.DeprecatedDefinitionsDialog_ColumnHeaderHasValue;
            AllowBlankColumn.HeaderText = TitleRecources.DeprecatedDefinitionsDialog_ColumnHeaderAllowBlank;
            IsForceEscapedColumn.HeaderText = TitleRecources.DeprecatedDefinitionsDialog_ColumnHeaderIsForceEscaped;
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

        // publics
        public void ShowDialog(IWin32Window parent, EgsEcfFile file)
        {
            DefinitionFileLabel.Text = string.Format("{0}: {1}", TitleRecources.DeprecatedDefinitionsDialog_DefinitionFile, Path.GetFileName(file.Definition.FilePathAndName));
            CompareFileLabel.Text = string.Format("{0}: {1}", TitleRecources.DeprecatedDefinitionsDialog_CompareFile, file.FileName);

            UpdateGrid(file);

            OkButton.Focus();
            ShowDialog(parent);
        }

        // privates
        private void UpdateGrid(EgsEcfFile file)
        {
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
            Grid.AutoResizeColumns();
            Grid.ClearSelection();
            Grid.ResumeLayout();
        }
    }
}
