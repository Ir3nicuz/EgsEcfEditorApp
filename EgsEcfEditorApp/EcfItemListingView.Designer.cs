
namespace EgsEcfEditorApp
{
    partial class EcfItemListingView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.CloseButton = new System.Windows.Forms.Button();
            this.ItemListingGridPanel = new System.Windows.Forms.Panel();
            this.ButtonInfoPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ItemListingGrid = new EcfWinFormControls.EcfDataGridView();
            this.ListingGridColumn_Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ListingGridColumn_File = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ListingGridColumn_Item = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ListingGridColumn_Parameter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SearchHitsLabel = new System.Windows.Forms.Label();
            this.ItemListingGridPanel.SuspendLayout();
            this.ButtonInfoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ItemListingGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseButton.AutoSize = true;
            this.CloseButton.Location = new System.Drawing.Point(722, 3);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 0;
            this.CloseButton.Text = "close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // ItemListingGridPanel
            // 
            this.ItemListingGridPanel.AutoSize = true;
            this.ItemListingGridPanel.Controls.Add(this.ItemListingGrid);
            this.ItemListingGridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ItemListingGridPanel.Location = new System.Drawing.Point(0, 0);
            this.ItemListingGridPanel.Name = "ItemListingGridPanel";
            this.ItemListingGridPanel.Padding = new System.Windows.Forms.Padding(3);
            this.ItemListingGridPanel.Size = new System.Drawing.Size(800, 421);
            this.ItemListingGridPanel.TabIndex = 3;
            // 
            // ButtonInfoPanel
            // 
            this.ButtonInfoPanel.AutoSize = true;
            this.ButtonInfoPanel.ColumnCount = 2;
            this.ButtonInfoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ButtonInfoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ButtonInfoPanel.Controls.Add(this.CloseButton, 1, 0);
            this.ButtonInfoPanel.Controls.Add(this.SearchHitsLabel, 0, 0);
            this.ButtonInfoPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonInfoPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.ButtonInfoPanel.Location = new System.Drawing.Point(0, 421);
            this.ButtonInfoPanel.Name = "ButtonInfoPanel";
            this.ButtonInfoPanel.RowCount = 1;
            this.ButtonInfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ButtonInfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.ButtonInfoPanel.Size = new System.Drawing.Size(800, 29);
            this.ButtonInfoPanel.TabIndex = 5;
            // 
            // ItemListingGrid
            // 
            this.ItemListingGrid.AllowUserToAddRows = false;
            this.ItemListingGrid.AllowUserToDeleteRows = false;
            this.ItemListingGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemListingGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.ItemListingGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ItemListingGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ListingGridColumn_Number,
            this.ListingGridColumn_File,
            this.ListingGridColumn_Item,
            this.ListingGridColumn_Parameter});
            this.ItemListingGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ItemListingGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.ItemListingGrid.Location = new System.Drawing.Point(3, 3);
            this.ItemListingGrid.Name = "ItemListingGrid";
            this.ItemListingGrid.ReadOnly = true;
            this.ItemListingGrid.ShowEditingIcon = false;
            this.ItemListingGrid.Size = new System.Drawing.Size(794, 415);
            this.ItemListingGrid.TabIndex = 2;
            this.ItemListingGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ItemListingGrid_CellDoubleClick);
            // 
            // ListingGridColumn_Number
            // 
            this.ListingGridColumn_Number.HeaderText = "nr";
            this.ListingGridColumn_Number.Name = "ListingGridColumn_Number";
            this.ListingGridColumn_Number.ReadOnly = true;
            this.ListingGridColumn_Number.Width = 41;
            // 
            // ListingGridColumn_File
            // 
            this.ListingGridColumn_File.HeaderText = "file";
            this.ListingGridColumn_File.Name = "ListingGridColumn_File";
            this.ListingGridColumn_File.ReadOnly = true;
            this.ListingGridColumn_File.Width = 45;
            // 
            // ListingGridColumn_Item
            // 
            this.ListingGridColumn_Item.HeaderText = "item";
            this.ListingGridColumn_Item.Name = "ListingGridColumn_Item";
            this.ListingGridColumn_Item.ReadOnly = true;
            this.ListingGridColumn_Item.Width = 51;
            // 
            // ListingGridColumn_Parameter
            // 
            this.ListingGridColumn_Parameter.HeaderText = "parameter";
            this.ListingGridColumn_Parameter.Name = "ListingGridColumn_Parameter";
            this.ListingGridColumn_Parameter.ReadOnly = true;
            this.ListingGridColumn_Parameter.Width = 79;
            // 
            // SearchHitsLabel
            // 
            this.SearchHitsLabel.AutoSize = true;
            this.SearchHitsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchHitsLabel.Location = new System.Drawing.Point(3, 0);
            this.SearchHitsLabel.Name = "SearchHitsLabel";
            this.SearchHitsLabel.Size = new System.Drawing.Size(394, 29);
            this.SearchHitsLabel.TabIndex = 1;
            this.SearchHitsLabel.Text = "??? hits";
            this.SearchHitsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // EcfItemListingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ItemListingGridPanel);
            this.Controls.Add(this.ButtonInfoPanel);
            this.Name = "EcfItemListingView";
            this.Text = "EcfItemListingDialog";
            this.ItemListingGridPanel.ResumeLayout(false);
            this.ButtonInfoPanel.ResumeLayout(false);
            this.ButtonInfoPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ItemListingGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button CloseButton;
        private EcfWinFormControls.EcfDataGridView ItemListingGrid;
        private System.Windows.Forms.Panel ItemListingGridPanel;
        private System.Windows.Forms.DataGridViewTextBoxColumn ListingGridColumn_Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn ListingGridColumn_File;
        private System.Windows.Forms.DataGridViewTextBoxColumn ListingGridColumn_Item;
        private System.Windows.Forms.DataGridViewTextBoxColumn ListingGridColumn_Parameter;
        private System.Windows.Forms.TableLayoutPanel ButtonInfoPanel;
        private System.Windows.Forms.Label SearchHitsLabel;
    }
}