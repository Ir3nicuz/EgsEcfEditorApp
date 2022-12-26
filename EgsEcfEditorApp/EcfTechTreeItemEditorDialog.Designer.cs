
namespace EgsEcfEditorApp
{
    partial class EcfTechTreeItemEditorDialog
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
            this.TechTreeEditorPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ElementNameTextBox = new System.Windows.Forms.TextBox();
            this.UnlockLevelNumUpDown = new System.Windows.Forms.NumericUpDown();
            this.UnlockCostNumUpDown = new System.Windows.Forms.NumericUpDown();
            this.ElementNameLabel = new System.Windows.Forms.Label();
            this.UnlockLevelLabel = new System.Windows.Forms.Label();
            this.UnlockCostLabel = new System.Windows.Forms.Label();
            this.ButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.AbortButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.TechTreeEditorPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UnlockLevelNumUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UnlockCostNumUpDown)).BeginInit();
            this.ButtonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // TechTreeEditorPanel
            // 
            this.TechTreeEditorPanel.ColumnCount = 2;
            this.TechTreeEditorPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.TechTreeEditorPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.TechTreeEditorPanel.Controls.Add(this.ElementNameTextBox, 1, 0);
            this.TechTreeEditorPanel.Controls.Add(this.UnlockLevelNumUpDown, 1, 1);
            this.TechTreeEditorPanel.Controls.Add(this.UnlockCostNumUpDown, 1, 2);
            this.TechTreeEditorPanel.Controls.Add(this.ElementNameLabel, 0, 0);
            this.TechTreeEditorPanel.Controls.Add(this.UnlockLevelLabel, 0, 1);
            this.TechTreeEditorPanel.Controls.Add(this.UnlockCostLabel, 0, 2);
            this.TechTreeEditorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TechTreeEditorPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.TechTreeEditorPanel.Location = new System.Drawing.Point(0, 0);
            this.TechTreeEditorPanel.Name = "TechTreeEditorPanel";
            this.TechTreeEditorPanel.RowCount = 3;
            this.TechTreeEditorPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TechTreeEditorPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TechTreeEditorPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TechTreeEditorPanel.Size = new System.Drawing.Size(484, 82);
            this.TechTreeEditorPanel.TabIndex = 0;
            // 
            // ElementNameTextBox
            // 
            this.ElementNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ElementNameTextBox.Location = new System.Drawing.Point(148, 3);
            this.ElementNameTextBox.Name = "ElementNameTextBox";
            this.ElementNameTextBox.ReadOnly = true;
            this.ElementNameTextBox.Size = new System.Drawing.Size(333, 20);
            this.ElementNameTextBox.TabIndex = 0;
            this.ElementNameTextBox.Click += new System.EventHandler(this.ElementNameTextBox_Click);
            // 
            // UnlockLevelNumUpDown
            // 
            this.UnlockLevelNumUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UnlockLevelNumUpDown.Location = new System.Drawing.Point(148, 30);
            this.UnlockLevelNumUpDown.Name = "UnlockLevelNumUpDown";
            this.UnlockLevelNumUpDown.Size = new System.Drawing.Size(333, 20);
            this.UnlockLevelNumUpDown.TabIndex = 1;
            this.UnlockLevelNumUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // UnlockCostNumUpDown
            // 
            this.UnlockCostNumUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UnlockCostNumUpDown.Location = new System.Drawing.Point(148, 57);
            this.UnlockCostNumUpDown.Name = "UnlockCostNumUpDown";
            this.UnlockCostNumUpDown.Size = new System.Drawing.Size(333, 20);
            this.UnlockCostNumUpDown.TabIndex = 2;
            // 
            // ElementNameLabel
            // 
            this.ElementNameLabel.AutoSize = true;
            this.ElementNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ElementNameLabel.Location = new System.Drawing.Point(3, 0);
            this.ElementNameLabel.Name = "ElementNameLabel";
            this.ElementNameLabel.Size = new System.Drawing.Size(139, 27);
            this.ElementNameLabel.TabIndex = 3;
            this.ElementNameLabel.Text = "name";
            this.ElementNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // UnlockLevelLabel
            // 
            this.UnlockLevelLabel.AutoSize = true;
            this.UnlockLevelLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UnlockLevelLabel.Location = new System.Drawing.Point(3, 27);
            this.UnlockLevelLabel.Name = "UnlockLevelLabel";
            this.UnlockLevelLabel.Size = new System.Drawing.Size(139, 27);
            this.UnlockLevelLabel.TabIndex = 4;
            this.UnlockLevelLabel.Text = "unlock level";
            this.UnlockLevelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // UnlockCostLabel
            // 
            this.UnlockCostLabel.AutoSize = true;
            this.UnlockCostLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UnlockCostLabel.Location = new System.Drawing.Point(3, 54);
            this.UnlockCostLabel.Name = "UnlockCostLabel";
            this.UnlockCostLabel.Size = new System.Drawing.Size(139, 28);
            this.UnlockCostLabel.TabIndex = 5;
            this.UnlockCostLabel.Text = "unlock cost";
            this.UnlockCostLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.AutoSize = true;
            this.ButtonPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonPanel.Controls.Add(this.AbortButton);
            this.ButtonPanel.Controls.Add(this.OkButton);
            this.ButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.ButtonPanel.Location = new System.Drawing.Point(0, 82);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(484, 29);
            this.ButtonPanel.TabIndex = 1;
            // 
            // AbortButton
            // 
            this.AbortButton.Location = new System.Drawing.Point(406, 3);
            this.AbortButton.Name = "AbortButton";
            this.AbortButton.Size = new System.Drawing.Size(75, 23);
            this.AbortButton.TabIndex = 0;
            this.AbortButton.Text = "abort";
            this.AbortButton.UseVisualStyleBackColor = true;
            this.AbortButton.Click += new System.EventHandler(this.AbortButton_Click);
            // 
            // OkButton
            // 
            this.OkButton.Location = new System.Drawing.Point(325, 3);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 1;
            this.OkButton.Text = "ok";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // EcfTechTreeItemEditorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 111);
            this.Controls.Add(this.TechTreeEditorPanel);
            this.Controls.Add(this.ButtonPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EcfTechTreeItemEditorDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EcfTechTreeItemEditorDialog";
            this.TechTreeEditorPanel.ResumeLayout(false);
            this.TechTreeEditorPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UnlockLevelNumUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UnlockCostNumUpDown)).EndInit();
            this.ButtonPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TechTreeEditorPanel;
        private System.Windows.Forms.FlowLayoutPanel ButtonPanel;
        private System.Windows.Forms.Button AbortButton;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.TextBox ElementNameTextBox;
        private System.Windows.Forms.NumericUpDown UnlockLevelNumUpDown;
        private System.Windows.Forms.NumericUpDown UnlockCostNumUpDown;
        private System.Windows.Forms.Label ElementNameLabel;
        private System.Windows.Forms.Label UnlockLevelLabel;
        private System.Windows.Forms.Label UnlockCostLabel;
    }
}