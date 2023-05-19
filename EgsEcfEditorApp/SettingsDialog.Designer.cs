
namespace EgsEcfEditorApp
{
    partial class SettingsDialog
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
            this.ResetButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.ChapterSelectorTreeView = new System.Windows.Forms.TreeView();
            this.SettingsBasePanel = new System.Windows.Forms.Panel();
            this.Tip = new System.Windows.Forms.ToolTip(this.components);
            this.ButtonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.AutoSize = true;
            this.ButtonPanel.Controls.Add(this.AbortButton);
            this.ButtonPanel.Controls.Add(this.ResetButton);
            this.ButtonPanel.Controls.Add(this.SaveButton);
            this.ButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.ButtonPanel.Location = new System.Drawing.Point(0, 364);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(712, 29);
            this.ButtonPanel.TabIndex = 0;
            // 
            // AbortButton
            // 
            this.AbortButton.AutoSize = true;
            this.AbortButton.Location = new System.Drawing.Point(634, 3);
            this.AbortButton.Name = "AbortButton";
            this.AbortButton.Size = new System.Drawing.Size(75, 23);
            this.AbortButton.TabIndex = 0;
            this.AbortButton.Text = "abort";
            this.AbortButton.UseVisualStyleBackColor = true;
            this.AbortButton.Click += new System.EventHandler(this.AbortButton_Click);
            // 
            // ResetButton
            // 
            this.ResetButton.AutoSize = true;
            this.ResetButton.Location = new System.Drawing.Point(553, 3);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(75, 23);
            this.ResetButton.TabIndex = 1;
            this.ResetButton.Text = "reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.AutoSize = true;
            this.SaveButton.Location = new System.Drawing.Point(472, 3);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 2;
            this.SaveButton.Text = "save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // ChapterSelectorTreeView
            // 
            this.ChapterSelectorTreeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.ChapterSelectorTreeView.FullRowSelect = true;
            this.ChapterSelectorTreeView.HideSelection = false;
            this.ChapterSelectorTreeView.Location = new System.Drawing.Point(0, 0);
            this.ChapterSelectorTreeView.Name = "ChapterSelectorTreeView";
            this.ChapterSelectorTreeView.Size = new System.Drawing.Size(150, 364);
            this.ChapterSelectorTreeView.TabIndex = 1;
            this.ChapterSelectorTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ChapterSelectorTreeView_AfterSelect);
            // 
            // SettingsBasePanel
            // 
            this.SettingsBasePanel.AutoSize = true;
            this.SettingsBasePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SettingsBasePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SettingsBasePanel.Location = new System.Drawing.Point(150, 0);
            this.SettingsBasePanel.Name = "SettingsBasePanel";
            this.SettingsBasePanel.Size = new System.Drawing.Size(562, 364);
            this.SettingsBasePanel.TabIndex = 3;
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 393);
            this.Controls.Add(this.SettingsBasePanel);
            this.Controls.Add(this.ChapterSelectorTreeView);
            this.Controls.Add(this.ButtonPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EcfSettingsDialog";
            this.Activated += new System.EventHandler(this.EcfSettingsDialog_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EcfSettingsDialog_FormClosing);
            this.ButtonPanel.ResumeLayout(false);
            this.ButtonPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel ButtonPanel;
        private System.Windows.Forms.Button AbortButton;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.TreeView ChapterSelectorTreeView;
        private System.Windows.Forms.Panel SettingsBasePanel;
        private System.Windows.Forms.ToolTip Tip;
    }
}