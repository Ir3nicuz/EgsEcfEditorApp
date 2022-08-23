
namespace EgsEcfEditorApp
{
    partial class EcfFileCAMDialog
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
            this.CloseButton = new System.Windows.Forms.Button();
            this.ComparePanel = new System.Windows.Forms.TableLayoutPanel();
            this.FirstFileComboBox = new System.Windows.Forms.ComboBox();
            this.SecondFileComboBox = new System.Windows.Forms.ComboBox();
            this.FirstFileTreeView = new EcfWinFormControls.EcfTreeView();
            this.SecondFileTreeView = new EcfWinFormControls.EcfTreeView();
            this.ButtonPanel.SuspendLayout();
            this.ComparePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.AutoSize = true;
            this.ButtonPanel.Controls.Add(this.CloseButton);
            this.ButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.ButtonPanel.Location = new System.Drawing.Point(0, 532);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(1184, 29);
            this.ButtonPanel.TabIndex = 0;
            // 
            // CloseButton
            // 
            this.CloseButton.AutoSize = true;
            this.CloseButton.Location = new System.Drawing.Point(1106, 3);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 0;
            this.CloseButton.Text = "close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // ComparePanel
            // 
            this.ComparePanel.AutoSize = true;
            this.ComparePanel.ColumnCount = 4;
            this.ComparePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ComparePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ComparePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ComparePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ComparePanel.Controls.Add(this.FirstFileComboBox, 0, 1);
            this.ComparePanel.Controls.Add(this.SecondFileComboBox, 2, 1);
            this.ComparePanel.Controls.Add(this.FirstFileTreeView, 0, 2);
            this.ComparePanel.Controls.Add(this.SecondFileTreeView, 2, 2);
            this.ComparePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ComparePanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.ComparePanel.Location = new System.Drawing.Point(0, 0);
            this.ComparePanel.Name = "ComparePanel";
            this.ComparePanel.RowCount = 3;
            this.ComparePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ComparePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ComparePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.00001F));
            this.ComparePanel.Size = new System.Drawing.Size(1184, 532);
            this.ComparePanel.TabIndex = 1;
            // 
            // FirstFileComboBox
            // 
            this.ComparePanel.SetColumnSpan(this.FirstFileComboBox, 2);
            this.FirstFileComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FirstFileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FirstFileComboBox.FormattingEnabled = true;
            this.FirstFileComboBox.Location = new System.Drawing.Point(3, 255);
            this.FirstFileComboBox.Name = "FirstFileComboBox";
            this.FirstFileComboBox.Size = new System.Drawing.Size(586, 21);
            this.FirstFileComboBox.Sorted = true;
            this.FirstFileComboBox.TabIndex = 0;
            // 
            // SecondFileComboBox
            // 
            this.ComparePanel.SetColumnSpan(this.SecondFileComboBox, 2);
            this.SecondFileComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondFileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SecondFileComboBox.FormattingEnabled = true;
            this.SecondFileComboBox.Location = new System.Drawing.Point(595, 255);
            this.SecondFileComboBox.Name = "SecondFileComboBox";
            this.SecondFileComboBox.Size = new System.Drawing.Size(586, 21);
            this.SecondFileComboBox.Sorted = true;
            this.SecondFileComboBox.TabIndex = 1;
            // 
            // FirstFileTreeView
            // 
            this.FirstFileTreeView.CheckBoxes = true;
            this.ComparePanel.SetColumnSpan(this.FirstFileTreeView, 2);
            this.FirstFileTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FirstFileTreeView.Location = new System.Drawing.Point(3, 282);
            this.FirstFileTreeView.Name = "FirstFileTreeView";
            this.FirstFileTreeView.Size = new System.Drawing.Size(586, 247);
            this.FirstFileTreeView.TabIndex = 2;
            // 
            // SecondFileTreeView
            // 
            this.SecondFileTreeView.CheckBoxes = true;
            this.ComparePanel.SetColumnSpan(this.SecondFileTreeView, 2);
            this.SecondFileTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondFileTreeView.Location = new System.Drawing.Point(595, 282);
            this.SecondFileTreeView.Name = "SecondFileTreeView";
            this.SecondFileTreeView.Size = new System.Drawing.Size(586, 247);
            this.SecondFileTreeView.TabIndex = 3;
            // 
            // EcfFileCAMDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 561);
            this.Controls.Add(this.ComparePanel);
            this.Controls.Add(this.ButtonPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EcfFileCAMDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EcfFileCompareAndMergeDialog";
            this.ButtonPanel.ResumeLayout(false);
            this.ButtonPanel.PerformLayout();
            this.ComparePanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel ButtonPanel;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.TableLayoutPanel ComparePanel;
        private System.Windows.Forms.ComboBox FirstFileComboBox;
        private System.Windows.Forms.ComboBox SecondFileComboBox;
        private EcfWinFormControls.EcfTreeView FirstFileTreeView;
        private EcfWinFormControls.EcfTreeView SecondFileTreeView;
    }
}