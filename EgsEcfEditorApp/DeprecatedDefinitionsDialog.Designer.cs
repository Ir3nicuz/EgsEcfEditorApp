
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
            this.ButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.OkButton = new System.Windows.Forms.Button();
            this.CompareFileLabel = new System.Windows.Forms.Label();
            this.DefinitionFileLabel = new System.Windows.Forms.Label();
            this.ButtonPanel.SuspendLayout();
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
            // DeprecatedDefinitionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 361);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel ButtonPanel;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Label CompareFileLabel;
        private System.Windows.Forms.Label DefinitionFileLabel;
    }
}