
namespace EcfFileViews
{
    partial class ItemSelectorDialog
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
            this.components = new System.ComponentModel.Container();
            this.ButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.AbortButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.ItemSelectorComboBox = new System.Windows.Forms.ComboBox();
            this.SelectionPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SearchTextBox = new System.Windows.Forms.TextBox();
            this.SearchLabel = new System.Windows.Forms.Label();
            this.Tip = new System.Windows.Forms.ToolTip(this.components);
            this.ButtonPanel.SuspendLayout();
            this.SelectionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.AutoSize = true;
            this.ButtonPanel.Controls.Add(this.AbortButton);
            this.ButtonPanel.Controls.Add(this.OkButton);
            this.ButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.ButtonPanel.Location = new System.Drawing.Point(0, 82);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(434, 29);
            this.ButtonPanel.TabIndex = 0;
            // 
            // AbortButton
            // 
            this.AbortButton.AutoSize = true;
            this.AbortButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.AbortButton.Location = new System.Drawing.Point(356, 3);
            this.AbortButton.Name = "AbortButton";
            this.AbortButton.Size = new System.Drawing.Size(75, 23);
            this.AbortButton.TabIndex = 0;
            this.AbortButton.Text = "abort";
            this.AbortButton.UseVisualStyleBackColor = true;
            this.AbortButton.Click += new System.EventHandler(this.AbortButton_Click);
            // 
            // OkButton
            // 
            this.OkButton.AutoSize = true;
            this.OkButton.Location = new System.Drawing.Point(275, 3);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 1;
            this.OkButton.Text = "ok";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // ItemSelectorComboBox
            // 
            this.ItemSelectorComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ItemSelectorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ItemSelectorComboBox.FormattingEnabled = true;
            this.ItemSelectorComboBox.Location = new System.Drawing.Point(3, 57);
            this.ItemSelectorComboBox.Name = "ItemSelectorComboBox";
            this.ItemSelectorComboBox.Size = new System.Drawing.Size(428, 21);
            this.ItemSelectorComboBox.Sorted = true;
            this.ItemSelectorComboBox.TabIndex = 1;
            this.ItemSelectorComboBox.SelectedIndexChanged += new System.EventHandler(this.ItemSelectorComboBox_SelectedIndexChanged);
            // 
            // SelectionPanel
            // 
            this.SelectionPanel.AutoSize = true;
            this.SelectionPanel.ColumnCount = 1;
            this.SelectionPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SelectionPanel.Controls.Add(this.ItemSelectorComboBox, 0, 2);
            this.SelectionPanel.Controls.Add(this.SearchTextBox, 0, 1);
            this.SelectionPanel.Controls.Add(this.SearchLabel, 0, 0);
            this.SelectionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectionPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.SelectionPanel.Location = new System.Drawing.Point(0, 0);
            this.SelectionPanel.Name = "SelectionPanel";
            this.SelectionPanel.RowCount = 3;
            this.SelectionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.SelectionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.SelectionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.SelectionPanel.Size = new System.Drawing.Size(434, 82);
            this.SelectionPanel.TabIndex = 2;
            // 
            // SearchTextBox
            // 
            this.SearchTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchTextBox.Location = new System.Drawing.Point(3, 30);
            this.SearchTextBox.Name = "SearchTextBox";
            this.SearchTextBox.Size = new System.Drawing.Size(428, 20);
            this.SearchTextBox.TabIndex = 2;
            this.SearchTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SearchTextBox_KeyPress);
            this.SearchTextBox.MouseHover += new System.EventHandler(this.SearchTextBox_MouseHover);
            // 
            // SearchLabel
            // 
            this.SearchLabel.AutoSize = true;
            this.SearchLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchLabel.Location = new System.Drawing.Point(3, 0);
            this.SearchLabel.Name = "SearchLabel";
            this.SearchLabel.Size = new System.Drawing.Size(428, 27);
            this.SearchLabel.TabIndex = 3;
            this.SearchLabel.Text = "search";
            this.SearchLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // EcfItemSelectorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 111);
            this.Controls.Add(this.SelectionPanel);
            this.Controls.Add(this.ButtonPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EcfItemSelectorDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EcfItemSelectorDialog";
            this.Activated += new System.EventHandler(this.EcfItemSelectorDialog_Activated);
            this.ButtonPanel.ResumeLayout(false);
            this.ButtonPanel.PerformLayout();
            this.SelectionPanel.ResumeLayout(false);
            this.SelectionPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel ButtonPanel;
        private System.Windows.Forms.Button AbortButton;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.ComboBox ItemSelectorComboBox;
        private System.Windows.Forms.TableLayoutPanel SelectionPanel;
        private System.Windows.Forms.TextBox SearchTextBox;
        private System.Windows.Forms.Label SearchLabel;
        private System.Windows.Forms.ToolTip Tip;
    }
}