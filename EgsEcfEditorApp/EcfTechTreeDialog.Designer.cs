
namespace EgsEcfEditorApp
{
    partial class EcfTechTreeDialog
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
            this.TechTreePageContainer = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ToolContainer = new EcfToolBarControls.EcfToolContainer();
            this.TechTreePageContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // TechTreePageContainer
            // 
            this.TechTreePageContainer.Controls.Add(this.tabPage1);
            this.TechTreePageContainer.Controls.Add(this.tabPage2);
            this.TechTreePageContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TechTreePageContainer.ItemSize = new System.Drawing.Size(1, 32);
            this.TechTreePageContainer.Location = new System.Drawing.Point(0, 0);
            this.TechTreePageContainer.Name = "TechTreePageContainer";
            this.TechTreePageContainer.SelectedIndex = 0;
            this.TechTreePageContainer.Size = new System.Drawing.Size(800, 450);
            this.TechTreePageContainer.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 36);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 410);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 36);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(786, 298);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ToolContainer
            // 
            this.ToolContainer.AutoSize = true;
            this.ToolContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ToolContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.ToolContainer.Location = new System.Drawing.Point(0, 0);
            this.ToolContainer.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.ToolContainer.Name = "ToolContainer";
            this.ToolContainer.Size = new System.Drawing.Size(800, 0);
            this.ToolContainer.TabIndex = 1;
            // 
            // EcfTechTreeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.TechTreePageContainer);
            this.Controls.Add(this.ToolContainer);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EcfTechTreeDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EcfTechTreeDialog";
            this.TechTreePageContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl TechTreePageContainer;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private EcfToolBarControls.EcfToolContainer ToolContainer;
    }
}