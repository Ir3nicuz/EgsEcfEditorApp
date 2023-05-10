
namespace EcfFileViews
{
    partial class EcfItemEditingDialog
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
            this.OkButton = new System.Windows.Forms.Button();
            this.ResetButton = new System.Windows.Forms.Button();
            this.ViewPanel = new System.Windows.Forms.TabControl();
            this.CommentItemView = new System.Windows.Forms.TabPage();
            this.ParameterItemView = new System.Windows.Forms.TabPage();
            this.BlockItemView = new System.Windows.Forms.TabPage();
            this.BlockItemTabPanel = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ParameterMatrixView = new System.Windows.Forms.TabPage();
            this.ButtonPanel.SuspendLayout();
            this.ViewPanel.SuspendLayout();
            this.BlockItemView.SuspendLayout();
            this.BlockItemTabPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.AutoSize = true;
            this.ButtonPanel.Controls.Add(this.AbortButton);
            this.ButtonPanel.Controls.Add(this.OkButton);
            this.ButtonPanel.Controls.Add(this.ResetButton);
            this.ButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.ButtonPanel.Location = new System.Drawing.Point(0, 632);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(1184, 29);
            this.ButtonPanel.TabIndex = 0;
            // 
            // AbortButton
            // 
            this.AbortButton.AutoSize = true;
            this.AbortButton.Location = new System.Drawing.Point(1106, 3);
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
            this.OkButton.Location = new System.Drawing.Point(1025, 3);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 1;
            this.OkButton.Text = "ok";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // ResetButton
            // 
            this.ResetButton.AutoSize = true;
            this.ResetButton.Location = new System.Drawing.Point(944, 3);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(75, 23);
            this.ResetButton.TabIndex = 3;
            this.ResetButton.Text = "reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // ViewPanel
            // 
            this.ViewPanel.Controls.Add(this.CommentItemView);
            this.ViewPanel.Controls.Add(this.ParameterItemView);
            this.ViewPanel.Controls.Add(this.BlockItemView);
            this.ViewPanel.Controls.Add(this.ParameterMatrixView);
            this.ViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ViewPanel.ItemSize = new System.Drawing.Size(50, 20);
            this.ViewPanel.Location = new System.Drawing.Point(0, 0);
            this.ViewPanel.Name = "ViewPanel";
            this.ViewPanel.SelectedIndex = 0;
            this.ViewPanel.Size = new System.Drawing.Size(1184, 632);
            this.ViewPanel.TabIndex = 1;
            // 
            // CommentItemView
            // 
            this.CommentItemView.Location = new System.Drawing.Point(4, 24);
            this.CommentItemView.Name = "CommentItemView";
            this.CommentItemView.Size = new System.Drawing.Size(1176, 604);
            this.CommentItemView.TabIndex = 1;
            this.CommentItemView.Text = "CommentItemView";
            this.CommentItemView.UseVisualStyleBackColor = true;
            // 
            // ParameterItemView
            // 
            this.ParameterItemView.Location = new System.Drawing.Point(4, 24);
            this.ParameterItemView.Name = "ParameterItemView";
            this.ParameterItemView.Size = new System.Drawing.Size(1176, 604);
            this.ParameterItemView.TabIndex = 2;
            this.ParameterItemView.Text = "ParameterItemView";
            this.ParameterItemView.UseVisualStyleBackColor = true;
            // 
            // BlockItemView
            // 
            this.BlockItemView.Controls.Add(this.BlockItemTabPanel);
            this.BlockItemView.Location = new System.Drawing.Point(4, 24);
            this.BlockItemView.Name = "BlockItemView";
            this.BlockItemView.Size = new System.Drawing.Size(1176, 604);
            this.BlockItemView.TabIndex = 3;
            this.BlockItemView.Text = "BlockItemView";
            this.BlockItemView.UseVisualStyleBackColor = true;
            // 
            // BlockItemTabPanel
            // 
            this.BlockItemTabPanel.Controls.Add(this.tabPage1);
            this.BlockItemTabPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BlockItemTabPanel.Location = new System.Drawing.Point(0, 0);
            this.BlockItemTabPanel.Name = "BlockItemTabPanel";
            this.BlockItemTabPanel.SelectedIndex = 0;
            this.BlockItemTabPanel.Size = new System.Drawing.Size(1176, 604);
            this.BlockItemTabPanel.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1168, 578);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "rootBlock";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // ParameterMatrixView
            // 
            this.ParameterMatrixView.Location = new System.Drawing.Point(4, 24);
            this.ParameterMatrixView.Name = "ParameterMatrixView";
            this.ParameterMatrixView.Size = new System.Drawing.Size(1176, 604);
            this.ParameterMatrixView.TabIndex = 4;
            this.ParameterMatrixView.Text = "ParameterMatrix";
            this.ParameterMatrixView.UseVisualStyleBackColor = true;
            // 
            // EcfItemEditingDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this.ViewPanel);
            this.Controls.Add(this.ButtonPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EcfItemEditingDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EcfItemEditingDialog";
            this.Activated += new System.EventHandler(this.EcfItemEditingDialog_Activated);
            this.ButtonPanel.ResumeLayout(false);
            this.ButtonPanel.PerformLayout();
            this.ViewPanel.ResumeLayout(false);
            this.BlockItemView.ResumeLayout(false);
            this.BlockItemTabPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel ButtonPanel;
        private System.Windows.Forms.Button AbortButton;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.TabControl ViewPanel;
        private System.Windows.Forms.TabPage CommentItemView;
        private System.Windows.Forms.TabPage ParameterItemView;
        private System.Windows.Forms.TabPage BlockItemView;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.TabPage ParameterMatrixView;
        private System.Windows.Forms.TabControl BlockItemTabPanel;
        private System.Windows.Forms.TabPage tabPage1;
    }
}