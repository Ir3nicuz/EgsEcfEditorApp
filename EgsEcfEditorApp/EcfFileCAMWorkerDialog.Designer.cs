
namespace EgsEcfEditorApp
{
    partial class EcfFileCAMWorkerDialog
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
            this.ProgressPanel = new System.Windows.Forms.TableLayoutPanel();
            this.AbortButton = new System.Windows.Forms.Button();
            this.ProgressIndicator = new System.Windows.Forms.ProgressBar();
            this.CAMActionLabel = new System.Windows.Forms.Label();
            this.ProgressPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProgressPanel
            // 
            this.ProgressPanel.AutoSize = true;
            this.ProgressPanel.ColumnCount = 2;
            this.ProgressPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ProgressPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ProgressPanel.Controls.Add(this.AbortButton, 0, 2);
            this.ProgressPanel.Controls.Add(this.ProgressIndicator, 0, 1);
            this.ProgressPanel.Controls.Add(this.CAMActionLabel, 0, 0);
            this.ProgressPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProgressPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.ProgressPanel.Location = new System.Drawing.Point(0, 0);
            this.ProgressPanel.Name = "ProgressPanel";
            this.ProgressPanel.RowCount = 3;
            this.ProgressPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.ProgressPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.ProgressPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.ProgressPanel.Size = new System.Drawing.Size(441, 147);
            this.ProgressPanel.TabIndex = 0;
            // 
            // AbortButton
            // 
            this.AbortButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AbortButton.AutoSize = true;
            this.ProgressPanel.SetColumnSpan(this.AbortButton, 2);
            this.AbortButton.Location = new System.Drawing.Point(183, 120);
            this.AbortButton.Name = "AbortButton";
            this.AbortButton.Size = new System.Drawing.Size(75, 23);
            this.AbortButton.TabIndex = 0;
            this.AbortButton.Text = "abort";
            this.AbortButton.UseVisualStyleBackColor = true;
            this.AbortButton.Click += new System.EventHandler(this.AbortButton_Click);
            // 
            // ProgressIndicator
            // 
            this.ProgressPanel.SetColumnSpan(this.ProgressIndicator, 2);
            this.ProgressIndicator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProgressIndicator.Location = new System.Drawing.Point(3, 61);
            this.ProgressIndicator.MarqueeAnimationSpeed = 25;
            this.ProgressIndicator.Name = "ProgressIndicator";
            this.ProgressIndicator.Size = new System.Drawing.Size(435, 52);
            this.ProgressIndicator.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.ProgressIndicator.TabIndex = 1;
            // 
            // CAMActionLabel
            // 
            this.CAMActionLabel.AutoSize = true;
            this.ProgressPanel.SetColumnSpan(this.CAMActionLabel, 2);
            this.CAMActionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CAMActionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAMActionLabel.Location = new System.Drawing.Point(3, 0);
            this.CAMActionLabel.Name = "CAMActionLabel";
            this.CAMActionLabel.Size = new System.Drawing.Size(435, 58);
            this.CAMActionLabel.TabIndex = 2;
            this.CAMActionLabel.Text = "label1";
            this.CAMActionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // EcfFileCAMWorkerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 147);
            this.Controls.Add(this.ProgressPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EcfFileCAMWorkerDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EcfFileCAMWorkerDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EcfFileCAMWorkerDialog_FormClosing);
            this.ProgressPanel.ResumeLayout(false);
            this.ProgressPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel ProgressPanel;
        private System.Windows.Forms.Button AbortButton;
        private System.Windows.Forms.ProgressBar ProgressIndicator;
        private System.Windows.Forms.Label CAMActionLabel;
    }
}