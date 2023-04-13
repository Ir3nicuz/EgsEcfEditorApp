
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
            this.ButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.CloseButton = new System.Windows.Forms.Button();
            this.SearchValueHeaderLabel = new System.Windows.Forms.Label();
            this.ItemListingGrid = new EcfWinFormControls.EcfDataGridView();
            this.ItemListingGridPanel = new System.Windows.Forms.Panel();
            this.SearchValueTextBox = new System.Windows.Forms.TextBox();
            this.SearchValuePanel = new System.Windows.Forms.TableLayoutPanel();
            this.ButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ItemListingGrid)).BeginInit();
            this.ItemListingGridPanel.SuspendLayout();
            this.SearchValuePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.AutoSize = true;
            this.ButtonPanel.Controls.Add(this.CloseButton);
            this.ButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.ButtonPanel.Location = new System.Drawing.Point(0, 421);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(800, 29);
            this.ButtonPanel.TabIndex = 0;
            // 
            // CloseButton
            // 
            this.CloseButton.AutoSize = true;
            this.CloseButton.Location = new System.Drawing.Point(722, 3);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 0;
            this.CloseButton.Text = "close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // SearchValueHeaderLabel
            // 
            this.SearchValueHeaderLabel.AutoSize = true;
            this.SearchValueHeaderLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchValueHeaderLabel.Location = new System.Drawing.Point(3, 0);
            this.SearchValueHeaderLabel.Name = "SearchValueHeaderLabel";
            this.SearchValueHeaderLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SearchValueHeaderLabel.Size = new System.Drawing.Size(40, 26);
            this.SearchValueHeaderLabel.TabIndex = 0;
            this.SearchValueHeaderLabel.Text = "header";
            this.SearchValueHeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ItemListingGrid
            // 
            this.ItemListingGrid.AllowUserToAddRows = false;
            this.ItemListingGrid.AllowUserToDeleteRows = false;
            this.ItemListingGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.ItemListingGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ItemListingGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ItemListingGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.ItemListingGrid.Location = new System.Drawing.Point(3, 3);
            this.ItemListingGrid.Name = "ItemListingGrid";
            this.ItemListingGrid.ShowEditingIcon = false;
            this.ItemListingGrid.Size = new System.Drawing.Size(794, 415);
            this.ItemListingGrid.TabIndex = 2;
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
            // SearchValueTextBox
            // 
            this.SearchValueTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchValueTextBox.Location = new System.Drawing.Point(49, 3);
            this.SearchValueTextBox.Name = "SearchValueTextBox";
            this.SearchValueTextBox.ReadOnly = true;
            this.SearchValueTextBox.Size = new System.Drawing.Size(748, 20);
            this.SearchValueTextBox.TabIndex = 2;
            this.SearchValueTextBox.Text = "searchValue";
            // 
            // SearchValuePanel
            // 
            this.SearchValuePanel.AutoSize = true;
            this.SearchValuePanel.ColumnCount = 2;
            this.SearchValuePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.SearchValuePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SearchValuePanel.Controls.Add(this.SearchValueHeaderLabel, 0, 0);
            this.SearchValuePanel.Controls.Add(this.SearchValueTextBox, 1, 0);
            this.SearchValuePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.SearchValuePanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.SearchValuePanel.Location = new System.Drawing.Point(0, 0);
            this.SearchValuePanel.Name = "SearchValuePanel";
            this.SearchValuePanel.RowCount = 1;
            this.SearchValuePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SearchValuePanel.Size = new System.Drawing.Size(800, 26);
            this.SearchValuePanel.TabIndex = 4;
            // 
            // EcfItemListingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.SearchValuePanel);
            this.Controls.Add(this.ItemListingGridPanel);
            this.Controls.Add(this.ButtonPanel);
            this.Name = "EcfItemListingView";
            this.Text = "EcfItemListingDialog";
            this.ButtonPanel.ResumeLayout(false);
            this.ButtonPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ItemListingGrid)).EndInit();
            this.ItemListingGridPanel.ResumeLayout(false);
            this.SearchValuePanel.ResumeLayout(false);
            this.SearchValuePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel ButtonPanel;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Label SearchValueHeaderLabel;
        private EcfWinFormControls.EcfDataGridView ItemListingGrid;
        private System.Windows.Forms.Panel ItemListingGridPanel;
        private System.Windows.Forms.TextBox SearchValueTextBox;
        private System.Windows.Forms.TableLayoutPanel SearchValuePanel;
    }
}