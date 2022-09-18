
namespace EgsEcfEditorApp
{
    partial class EcfFileLoaderDialog
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
            this.AbortButton = new System.Windows.Forms.Button();
            this.FilePathAndNameTextBox = new System.Windows.Forms.TextBox();
            this.ProgressPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ProgressIndicator = new EcfWinFormControls.EcfProgressBar();
            this.ProgressPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // AbortButton
            // 
            this.AbortButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AbortButton.AutoSize = true;
            this.ProgressPanel.SetColumnSpan(this.AbortButton, 2);
            this.AbortButton.Location = new System.Drawing.Point(354, 85);
            this.AbortButton.Name = "AbortButton";
            this.AbortButton.Size = new System.Drawing.Size(75, 23);
            this.AbortButton.TabIndex = 1;
            this.AbortButton.Text = "abort";
            this.AbortButton.UseVisualStyleBackColor = true;
            this.AbortButton.Click += new System.EventHandler(this.AbortButton_Click);
            // 
            // FilePathAndNameTextBox
            // 
            this.ProgressPanel.SetColumnSpan(this.FilePathAndNameTextBox, 2);
            this.FilePathAndNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilePathAndNameTextBox.Location = new System.Drawing.Point(3, 3);
            this.FilePathAndNameTextBox.Name = "FilePathAndNameTextBox";
            this.FilePathAndNameTextBox.ReadOnly = true;
            this.FilePathAndNameTextBox.Size = new System.Drawing.Size(778, 20);
            this.FilePathAndNameTextBox.TabIndex = 2;
            // 
            // ProgressPanel
            // 
            this.ProgressPanel.AutoSize = true;
            this.ProgressPanel.ColumnCount = 2;
            this.ProgressPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ProgressPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ProgressPanel.Controls.Add(this.FilePathAndNameTextBox, 0, 0);
            this.ProgressPanel.Controls.Add(this.AbortButton, 0, 2);
            this.ProgressPanel.Controls.Add(this.ProgressIndicator, 0, 1);
            this.ProgressPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProgressPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.ProgressPanel.Location = new System.Drawing.Point(0, 0);
            this.ProgressPanel.Name = "ProgressPanel";
            this.ProgressPanel.RowCount = 3;
            this.ProgressPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ProgressPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ProgressPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ProgressPanel.Size = new System.Drawing.Size(784, 111);
            this.ProgressPanel.TabIndex = 3;
            // 
            // ProgressIndicator
            // 
            this.ProgressIndicator.BarText = "";
            this.ProgressPanel.SetColumnSpan(this.ProgressIndicator, 2);
            this.ProgressIndicator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProgressIndicator.Location = new System.Drawing.Point(3, 30);
            this.ProgressIndicator.Name = "ProgressIndicator";
            this.ProgressIndicator.Size = new System.Drawing.Size(778, 49);
            this.ProgressIndicator.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressIndicator.TabIndex = 3;
            // 
            // EcfFileLoaderDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 111);
            this.Controls.Add(this.ProgressPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EcfFileLoaderDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ProgressDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProgressDialog_FormClosing);
            this.ProgressPanel.ResumeLayout(false);
            this.ProgressPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button AbortButton;
        private System.Windows.Forms.TableLayoutPanel ProgressPanel;
        private System.Windows.Forms.TextBox FilePathAndNameTextBox;
        private EcfWinFormControls.EcfProgressBar ProgressIndicator;
    }
}