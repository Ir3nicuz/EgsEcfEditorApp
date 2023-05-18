namespace GenericDialogs
{
    partial class ErrorListingDialog
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
            this.AbortButton = new System.Windows.Forms.Button();
            this.NoButton = new System.Windows.Forms.Button();
            this.YesButton = new System.Windows.Forms.Button();
            this.MessageLabel = new System.Windows.Forms.Label();
            this.MessageIcon = new System.Windows.Forms.PictureBox();
            this.MessagePanel = new System.Windows.Forms.TableLayoutPanel();
            this.ErrorRichTextBox = new System.Windows.Forms.RichTextBox();
            this.ErrorPanel = new System.Windows.Forms.Panel();
            this.OkButton = new System.Windows.Forms.Button();
            this.ButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MessageIcon)).BeginInit();
            this.MessagePanel.SuspendLayout();
            this.ErrorPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.AutoSize = true;
            this.ButtonPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonPanel.Controls.Add(this.AbortButton);
            this.ButtonPanel.Controls.Add(this.NoButton);
            this.ButtonPanel.Controls.Add(this.YesButton);
            this.ButtonPanel.Controls.Add(this.OkButton);
            this.ButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.ButtonPanel.Location = new System.Drawing.Point(0, 332);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(584, 29);
            this.ButtonPanel.TabIndex = 0;
            // 
            // AbortButton
            // 
            this.AbortButton.AutoSize = true;
            this.AbortButton.Location = new System.Drawing.Point(506, 3);
            this.AbortButton.Name = "AbortButton";
            this.AbortButton.Size = new System.Drawing.Size(75, 23);
            this.AbortButton.TabIndex = 0;
            this.AbortButton.Text = "abort";
            this.AbortButton.UseVisualStyleBackColor = true;
            this.AbortButton.Click += new System.EventHandler(this.AbortButton_Click);
            // 
            // NoButton
            // 
            this.NoButton.AutoSize = true;
            this.NoButton.Location = new System.Drawing.Point(425, 3);
            this.NoButton.Name = "NoButton";
            this.NoButton.Size = new System.Drawing.Size(75, 23);
            this.NoButton.TabIndex = 1;
            this.NoButton.Text = "no";
            this.NoButton.UseVisualStyleBackColor = true;
            this.NoButton.Click += new System.EventHandler(this.NoButton_Click);
            // 
            // YesButton
            // 
            this.YesButton.AutoSize = true;
            this.YesButton.Location = new System.Drawing.Point(344, 3);
            this.YesButton.Name = "YesButton";
            this.YesButton.Size = new System.Drawing.Size(75, 23);
            this.YesButton.TabIndex = 2;
            this.YesButton.Text = "yes";
            this.YesButton.UseVisualStyleBackColor = true;
            this.YesButton.Click += new System.EventHandler(this.YesButton_Click);
            // 
            // MessageLabel
            // 
            this.MessageLabel.AutoSize = true;
            this.MessageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MessageLabel.Location = new System.Drawing.Point(109, 0);
            this.MessageLabel.Name = "MessageLabel";
            this.MessageLabel.Padding = new System.Windows.Forms.Padding(3);
            this.MessageLabel.Size = new System.Drawing.Size(472, 56);
            this.MessageLabel.TabIndex = 1;
            this.MessageLabel.Text = "message";
            // 
            // MessageIcon
            // 
            this.MessageIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MessageIcon.Location = new System.Drawing.Point(3, 3);
            this.MessageIcon.Name = "MessageIcon";
            this.MessageIcon.Padding = new System.Windows.Forms.Padding(3);
            this.MessageIcon.Size = new System.Drawing.Size(100, 50);
            this.MessageIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.MessageIcon.TabIndex = 4;
            this.MessageIcon.TabStop = false;
            // 
            // MessagePanel
            // 
            this.MessagePanel.AutoSize = true;
            this.MessagePanel.ColumnCount = 2;
            this.MessagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MessagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MessagePanel.Controls.Add(this.MessageIcon, 0, 0);
            this.MessagePanel.Controls.Add(this.MessageLabel, 1, 0);
            this.MessagePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.MessagePanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.MessagePanel.Location = new System.Drawing.Point(0, 0);
            this.MessagePanel.Name = "MessagePanel";
            this.MessagePanel.RowCount = 1;
            this.MessagePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MessagePanel.Size = new System.Drawing.Size(584, 56);
            this.MessagePanel.TabIndex = 5;
            // 
            // ErrorRichTextBox
            // 
            this.ErrorRichTextBox.BulletIndent = 10;
            this.ErrorRichTextBox.DetectUrls = false;
            this.ErrorRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ErrorRichTextBox.Location = new System.Drawing.Point(3, 0);
            this.ErrorRichTextBox.Name = "ErrorRichTextBox";
            this.ErrorRichTextBox.ReadOnly = true;
            this.ErrorRichTextBox.Size = new System.Drawing.Size(578, 276);
            this.ErrorRichTextBox.TabIndex = 6;
            this.ErrorRichTextBox.Text = "";
            this.ErrorRichTextBox.WordWrap = false;
            // 
            // ErrorPanel
            // 
            this.ErrorPanel.AutoSize = true;
            this.ErrorPanel.Controls.Add(this.ErrorRichTextBox);
            this.ErrorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ErrorPanel.Location = new System.Drawing.Point(0, 56);
            this.ErrorPanel.Name = "ErrorPanel";
            this.ErrorPanel.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.ErrorPanel.Size = new System.Drawing.Size(584, 276);
            this.ErrorPanel.TabIndex = 7;
            // 
            // OkButton
            // 
            this.OkButton.AutoSize = true;
            this.OkButton.Location = new System.Drawing.Point(263, 3);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 3;
            this.OkButton.Text = "ok";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // ErrorListingDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.ErrorPanel);
            this.Controls.Add(this.MessagePanel);
            this.Controls.Add(this.ButtonPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorListingDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ErrorListingDialog";
            this.Activated += new System.EventHandler(this.ErrorListingDialog_Activated);
            this.ResizeEnd += new System.EventHandler(this.ErrorListingDialog_ResizeEnd);
            this.ButtonPanel.ResumeLayout(false);
            this.ButtonPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MessageIcon)).EndInit();
            this.MessagePanel.ResumeLayout(false);
            this.MessagePanel.PerformLayout();
            this.ErrorPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel ButtonPanel;
        private System.Windows.Forms.Button AbortButton;
        private System.Windows.Forms.Button NoButton;
        private System.Windows.Forms.Button YesButton;
        private System.Windows.Forms.Label MessageLabel;
        private System.Windows.Forms.PictureBox MessageIcon;
        private System.Windows.Forms.TableLayoutPanel MessagePanel;
        private System.Windows.Forms.RichTextBox ErrorRichTextBox;
        private System.Windows.Forms.Panel ErrorPanel;
        private System.Windows.Forms.Button OkButton;
    }
}