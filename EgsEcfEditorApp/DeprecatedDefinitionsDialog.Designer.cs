
namespace EcfFileViews
{
    partial class DeprecatedDefinitionsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.OkButton = new System.Windows.Forms.Button();
            this.CompareFileLabel = new System.Windows.Forms.Label();
            this.DefinitionFileLabel = new System.Windows.Forms.Label();
            this.Grid = new EcfWinFormControls.EcfDataGridView();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InfoColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsOptionalColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.HasValueColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.AllowBlankColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.IsForceEscapedColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.GridPanel = new System.Windows.Forms.Panel();
            this.ButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Grid)).BeginInit();
            this.GridPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.AutoSize = true;
            this.ButtonPanel.Controls.Add(this.OkButton);
            this.ButtonPanel.Controls.Add(this.CompareFileLabel);
            this.ButtonPanel.Controls.Add(this.DefinitionFileLabel);
            this.ButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.ButtonPanel.Location = new System.Drawing.Point(0, 332);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(784, 29);
            this.ButtonPanel.TabIndex = 0;
            // 
            // OkButton
            // 
            this.OkButton.AutoSize = true;
            this.OkButton.Location = new System.Drawing.Point(706, 3);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 0;
            this.OkButton.Text = "ok";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // CompareFileLabel
            // 
            this.CompareFileLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.CompareFileLabel.AutoSize = true;
            this.CompareFileLabel.Location = new System.Drawing.Point(665, 0);
            this.CompareFileLabel.Name = "CompareFileLabel";
            this.CompareFileLabel.Size = new System.Drawing.Size(35, 29);
            this.CompareFileLabel.TabIndex = 1;
            this.CompareFileLabel.Text = "label1";
            this.CompareFileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DefinitionFileLabel
            // 
            this.DefinitionFileLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.DefinitionFileLabel.AutoSize = true;
            this.DefinitionFileLabel.Location = new System.Drawing.Point(624, 0);
            this.DefinitionFileLabel.Name = "DefinitionFileLabel";
            this.DefinitionFileLabel.Size = new System.Drawing.Size(35, 29);
            this.DefinitionFileLabel.TabIndex = 2;
            this.DefinitionFileLabel.Text = "label1";
            this.DefinitionFileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Grid
            // 
            this.Grid.AllowUserToAddRows = false;
            this.Grid.AllowUserToDeleteRows = false;
            this.Grid.AllowUserToOrderColumns = true;
            this.Grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.Grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColumn,
            this.InfoColumn,
            this.IsOptionalColumn,
            this.HasValueColumn,
            this.AllowBlankColumn,
            this.IsForceEscapedColumn});
            this.Grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Grid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.Grid.Location = new System.Drawing.Point(3, 3);
            this.Grid.Name = "Grid";
            this.Grid.ReadOnly = true;
            this.Grid.ShowEditingIcon = false;
            this.Grid.Size = new System.Drawing.Size(778, 326);
            this.Grid.TabIndex = 1;
            // 
            // NameColumn
            // 
            this.NameColumn.HeaderText = "name";
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.ReadOnly = true;
            this.NameColumn.Width = 58;
            // 
            // InfoColumn
            // 
            this.InfoColumn.HeaderText = "info";
            this.InfoColumn.Name = "InfoColumn";
            this.InfoColumn.ReadOnly = true;
            this.InfoColumn.Width = 49;
            // 
            // IsOptionalColumn
            // 
            this.IsOptionalColumn.HeaderText = "isOpt";
            this.IsOptionalColumn.Name = "IsOptionalColumn";
            this.IsOptionalColumn.ReadOnly = true;
            this.IsOptionalColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.IsOptionalColumn.Width = 56;
            // 
            // HasValueColumn
            // 
            this.HasValueColumn.HeaderText = "hasVal";
            this.HasValueColumn.Name = "HasValueColumn";
            this.HasValueColumn.ReadOnly = true;
            this.HasValueColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.HasValueColumn.Width = 64;
            // 
            // AllowBlankColumn
            // 
            this.AllowBlankColumn.HeaderText = "allowBlank";
            this.AllowBlankColumn.Name = "AllowBlankColumn";
            this.AllowBlankColumn.ReadOnly = true;
            this.AllowBlankColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.AllowBlankColumn.Width = 83;
            // 
            // IsForceEscapedColumn
            // 
            this.IsForceEscapedColumn.HeaderText = "forced";
            this.IsForceEscapedColumn.Name = "IsForceEscapedColumn";
            this.IsForceEscapedColumn.ReadOnly = true;
            this.IsForceEscapedColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.IsForceEscapedColumn.Width = 62;
            // 
            // GridPanel
            // 
            this.GridPanel.AutoSize = true;
            this.GridPanel.Controls.Add(this.Grid);
            this.GridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridPanel.Location = new System.Drawing.Point(0, 0);
            this.GridPanel.Name = "GridPanel";
            this.GridPanel.Padding = new System.Windows.Forms.Padding(3);
            this.GridPanel.Size = new System.Drawing.Size(784, 332);
            this.GridPanel.TabIndex = 2;
            // 
            // DeprecatedDefinitionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 361);
            this.Controls.Add(this.GridPanel);
            this.Controls.Add(this.ButtonPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeprecatedDefinitionsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DeprecatedFormatDialog";
            this.ButtonPanel.ResumeLayout(false);
            this.ButtonPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Grid)).EndInit();
            this.GridPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel ButtonPanel;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Label CompareFileLabel;
        private System.Windows.Forms.Label DefinitionFileLabel;
        private EcfWinFormControls.EcfDataGridView Grid;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn InfoColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsOptionalColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn HasValueColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn AllowBlankColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsForceEscapedColumn;
        private System.Windows.Forms.Panel GridPanel;
    }
}