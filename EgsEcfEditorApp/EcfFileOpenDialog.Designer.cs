
namespace EcfFileViews
{
    partial class EcfFileOpenDialog
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
            this.FilePathAndNameLabel = new System.Windows.Forms.Label();
            this.FilePathAndNameTextBox = new System.Windows.Forms.TextBox();
            this.FormatDefinitionLabel = new System.Windows.Forms.Label();
            this.FormatDefinitionComboBox = new System.Windows.Forms.ComboBox();
            this.EncodingLabel = new System.Windows.Forms.Label();
            this.EncodingComboBox = new System.Windows.Forms.ComboBox();
            this.NewLineSymbolLabel = new System.Windows.Forms.Label();
            this.NewLineSymbolComboBox = new System.Windows.Forms.ComboBox();
            this.ButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.AbortButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.SettingsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.Tip = new System.Windows.Forms.ToolTip(this.components);
            this.ButtonPanel.SuspendLayout();
            this.SettingsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // FilePathAndNameLabel
            // 
            this.FilePathAndNameLabel.AutoSize = true;
            this.FilePathAndNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilePathAndNameLabel.Location = new System.Drawing.Point(3, 0);
            this.FilePathAndNameLabel.Name = "FilePathAndNameLabel";
            this.FilePathAndNameLabel.Size = new System.Drawing.Size(150, 26);
            this.FilePathAndNameLabel.TabIndex = 1;
            this.FilePathAndNameLabel.Text = "path";
            this.FilePathAndNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FilePathAndNameTextBox
            // 
            this.FilePathAndNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilePathAndNameTextBox.Location = new System.Drawing.Point(159, 3);
            this.FilePathAndNameTextBox.Name = "FilePathAndNameTextBox";
            this.FilePathAndNameTextBox.ReadOnly = true;
            this.FilePathAndNameTextBox.Size = new System.Drawing.Size(622, 20);
            this.FilePathAndNameTextBox.TabIndex = 1;
            // 
            // FormatDefinitionLabel
            // 
            this.FormatDefinitionLabel.AutoSize = true;
            this.FormatDefinitionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FormatDefinitionLabel.Location = new System.Drawing.Point(3, 26);
            this.FormatDefinitionLabel.Name = "FormatDefinitionLabel";
            this.FormatDefinitionLabel.Size = new System.Drawing.Size(150, 26);
            this.FormatDefinitionLabel.TabIndex = 2;
            this.FormatDefinitionLabel.Text = "type";
            this.FormatDefinitionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormatDefinitionComboBox
            // 
            this.FormatDefinitionComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FormatDefinitionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FormatDefinitionComboBox.FormattingEnabled = true;
            this.FormatDefinitionComboBox.Location = new System.Drawing.Point(159, 29);
            this.FormatDefinitionComboBox.MaxDropDownItems = 10;
            this.FormatDefinitionComboBox.Name = "FormatDefinitionComboBox";
            this.FormatDefinitionComboBox.Size = new System.Drawing.Size(622, 21);
            this.FormatDefinitionComboBox.Sorted = true;
            this.FormatDefinitionComboBox.TabIndex = 2;
            this.FormatDefinitionComboBox.SelectionChangeCommitted += new System.EventHandler(this.FormatDefinitionComboBox_SelectionChangeCommitted);
            // 
            // EncodingLabel
            // 
            this.EncodingLabel.AutoSize = true;
            this.EncodingLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EncodingLabel.Location = new System.Drawing.Point(3, 52);
            this.EncodingLabel.Name = "EncodingLabel";
            this.EncodingLabel.Size = new System.Drawing.Size(150, 26);
            this.EncodingLabel.TabIndex = 3;
            this.EncodingLabel.Text = "encoding";
            this.EncodingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // EncodingComboBox
            // 
            this.EncodingComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EncodingComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EncodingComboBox.FormattingEnabled = true;
            this.EncodingComboBox.Location = new System.Drawing.Point(159, 55);
            this.EncodingComboBox.MaxDropDownItems = 10;
            this.EncodingComboBox.Name = "EncodingComboBox";
            this.EncodingComboBox.Size = new System.Drawing.Size(622, 21);
            this.EncodingComboBox.Sorted = true;
            this.EncodingComboBox.TabIndex = 3;
            this.EncodingComboBox.SelectedIndexChanged += new System.EventHandler(this.EncodingComboBox_SelectedIndexChanged);
            // 
            // NewLineSymbolLabel
            // 
            this.NewLineSymbolLabel.AutoSize = true;
            this.NewLineSymbolLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NewLineSymbolLabel.Location = new System.Drawing.Point(3, 78);
            this.NewLineSymbolLabel.Name = "NewLineSymbolLabel";
            this.NewLineSymbolLabel.Size = new System.Drawing.Size(150, 29);
            this.NewLineSymbolLabel.TabIndex = 3;
            this.NewLineSymbolLabel.Text = "newline";
            this.NewLineSymbolLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // NewLineSymbolComboBox
            // 
            this.NewLineSymbolComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NewLineSymbolComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NewLineSymbolComboBox.FormattingEnabled = true;
            this.NewLineSymbolComboBox.Location = new System.Drawing.Point(159, 81);
            this.NewLineSymbolComboBox.Name = "NewLineSymbolComboBox";
            this.NewLineSymbolComboBox.Size = new System.Drawing.Size(622, 21);
            this.NewLineSymbolComboBox.Sorted = true;
            this.NewLineSymbolComboBox.TabIndex = 4;
            this.NewLineSymbolComboBox.SelectedIndexChanged += new System.EventHandler(this.NewLineSymbolComboBox_SelectedIndexChanged);
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.AutoSize = true;
            this.ButtonPanel.Controls.Add(this.AbortButton);
            this.ButtonPanel.Controls.Add(this.OkButton);
            this.ButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.ButtonPanel.Location = new System.Drawing.Point(0, 107);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(784, 29);
            this.ButtonPanel.TabIndex = 4;
            // 
            // AbortButton
            // 
            this.AbortButton.AutoSize = true;
            this.AbortButton.Location = new System.Drawing.Point(706, 3);
            this.AbortButton.Name = "AbortButton";
            this.AbortButton.Size = new System.Drawing.Size(75, 23);
            this.AbortButton.TabIndex = 6;
            this.AbortButton.Text = "abort";
            this.AbortButton.UseVisualStyleBackColor = true;
            this.AbortButton.Click += new System.EventHandler(this.AbortButton_Click);
            // 
            // OkButton
            // 
            this.OkButton.AutoSize = true;
            this.OkButton.Location = new System.Drawing.Point(625, 3);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 5;
            this.OkButton.Text = "ok";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // SettingsPanel
            // 
            this.SettingsPanel.AutoSize = true;
            this.SettingsPanel.ColumnCount = 2;
            this.SettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.SettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.SettingsPanel.Controls.Add(this.NewLineSymbolComboBox, 1, 3);
            this.SettingsPanel.Controls.Add(this.EncodingComboBox, 1, 2);
            this.SettingsPanel.Controls.Add(this.FormatDefinitionComboBox, 1, 1);
            this.SettingsPanel.Controls.Add(this.FilePathAndNameTextBox, 1, 0);
            this.SettingsPanel.Controls.Add(this.NewLineSymbolLabel, 0, 3);
            this.SettingsPanel.Controls.Add(this.EncodingLabel, 0, 2);
            this.SettingsPanel.Controls.Add(this.FormatDefinitionLabel, 0, 1);
            this.SettingsPanel.Controls.Add(this.FilePathAndNameLabel, 0, 0);
            this.SettingsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SettingsPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.SettingsPanel.Location = new System.Drawing.Point(0, 0);
            this.SettingsPanel.Name = "SettingsPanel";
            this.SettingsPanel.RowCount = 4;
            this.SettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.SettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.SettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.SettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.SettingsPanel.Size = new System.Drawing.Size(784, 107);
            this.SettingsPanel.TabIndex = 5;
            // 
            // EcfFileOpenDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 136);
            this.Controls.Add(this.SettingsPanel);
            this.Controls.Add(this.ButtonPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EcfFileOpenDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EcfFilePropertiesDialog";
            this.Activated += new System.EventHandler(this.EcfFilePropertiesDialog_Activated);
            this.ButtonPanel.ResumeLayout(false);
            this.ButtonPanel.PerformLayout();
            this.SettingsPanel.ResumeLayout(false);
            this.SettingsPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label FilePathAndNameLabel;
        private System.Windows.Forms.TextBox FilePathAndNameTextBox;
        private System.Windows.Forms.ComboBox FormatDefinitionComboBox;
        private System.Windows.Forms.ComboBox EncodingComboBox;
        private System.Windows.Forms.ComboBox NewLineSymbolComboBox;
        private System.Windows.Forms.Label FormatDefinitionLabel;
        private System.Windows.Forms.Label EncodingLabel;
        private System.Windows.Forms.Label NewLineSymbolLabel;
        private System.Windows.Forms.FlowLayoutPanel ButtonPanel;
        private System.Windows.Forms.Button AbortButton;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.TableLayoutPanel SettingsPanel;
        private System.Windows.Forms.ToolTip Tip;
    }
}