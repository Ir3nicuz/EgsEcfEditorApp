
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
            this.CompareAndMergePanel = new System.Windows.Forms.TableLayoutPanel();
            this.FirstFileComboBox = new System.Windows.Forms.ComboBox();
            this.SecondFileComboBox = new System.Windows.Forms.ComboBox();
            this.SecondFileSelectionContainer = new EcfToolBarControls.EcfToolContainer();
            this.FirstFileSelectionContainer = new EcfToolBarControls.EcfToolContainer();
            this.ActionContainer = new EcfToolBarControls.EcfToolContainer();
            this.NavigationContainer = new EcfToolBarControls.EcfToolContainer();
            this.FirstFileDetailsBorderPanel = new System.Windows.Forms.Panel();
            this.FirstFileDetailsView = new System.Windows.Forms.RichTextBox();
            this.SecondFileDetailsBorderPanel = new System.Windows.Forms.Panel();
            this.SecondFileDetailsView = new System.Windows.Forms.RichTextBox();
            this.FirstFileTreeBorderPanel = new System.Windows.Forms.GroupBox();
            this.FirstFileTreeView = new EcfWinFormControls.EcfTreeView();
            this.SecondFileTreeBorderPanel = new System.Windows.Forms.GroupBox();
            this.SecondFileTreeView = new EcfWinFormControls.EcfTreeView();
            this.CompareAndMergePanel.SuspendLayout();
            this.FirstFileDetailsBorderPanel.SuspendLayout();
            this.SecondFileDetailsBorderPanel.SuspendLayout();
            this.FirstFileTreeBorderPanel.SuspendLayout();
            this.SecondFileTreeBorderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // CompareAndMergePanel
            // 
            this.CompareAndMergePanel.AutoSize = true;
            this.CompareAndMergePanel.ColumnCount = 3;
            this.CompareAndMergePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.CompareAndMergePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.CompareAndMergePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.CompareAndMergePanel.Controls.Add(this.FirstFileComboBox, 0, 1);
            this.CompareAndMergePanel.Controls.Add(this.SecondFileComboBox, 2, 1);
            this.CompareAndMergePanel.Controls.Add(this.SecondFileSelectionContainer, 2, 0);
            this.CompareAndMergePanel.Controls.Add(this.FirstFileSelectionContainer, 0, 0);
            this.CompareAndMergePanel.Controls.Add(this.ActionContainer, 1, 3);
            this.CompareAndMergePanel.Controls.Add(this.NavigationContainer, 1, 2);
            this.CompareAndMergePanel.Controls.Add(this.FirstFileDetailsBorderPanel, 0, 4);
            this.CompareAndMergePanel.Controls.Add(this.SecondFileDetailsBorderPanel, 2, 4);
            this.CompareAndMergePanel.Controls.Add(this.FirstFileTreeBorderPanel, 0, 2);
            this.CompareAndMergePanel.Controls.Add(this.SecondFileTreeBorderPanel, 2, 2);
            this.CompareAndMergePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CompareAndMergePanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.CompareAndMergePanel.Location = new System.Drawing.Point(0, 0);
            this.CompareAndMergePanel.Name = "CompareAndMergePanel";
            this.CompareAndMergePanel.RowCount = 5;
            this.CompareAndMergePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.CompareAndMergePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.CompareAndMergePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.55704F));
            this.CompareAndMergePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.22148F));
            this.CompareAndMergePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.22148F));
            this.CompareAndMergePanel.Size = new System.Drawing.Size(1184, 561);
            this.CompareAndMergePanel.TabIndex = 1;
            // 
            // FirstFileComboBox
            // 
            this.FirstFileComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FirstFileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FirstFileComboBox.FormattingEnabled = true;
            this.FirstFileComboBox.Location = new System.Drawing.Point(3, 3);
            this.FirstFileComboBox.Name = "FirstFileComboBox";
            this.FirstFileComboBox.Size = new System.Drawing.Size(583, 21);
            this.FirstFileComboBox.Sorted = true;
            this.FirstFileComboBox.TabIndex = 0;
            this.FirstFileComboBox.SelectionChangeCommitted += new System.EventHandler(this.FirstFileComboBox_SelectionChangeCommitted);
            // 
            // SecondFileComboBox
            // 
            this.SecondFileComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondFileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SecondFileComboBox.FormattingEnabled = true;
            this.SecondFileComboBox.Location = new System.Drawing.Point(598, 3);
            this.SecondFileComboBox.Name = "SecondFileComboBox";
            this.SecondFileComboBox.Size = new System.Drawing.Size(583, 21);
            this.SecondFileComboBox.Sorted = true;
            this.SecondFileComboBox.TabIndex = 1;
            this.SecondFileComboBox.SelectionChangeCommitted += new System.EventHandler(this.SecondFileComboBox_SelectionChangeCommitted);
            // 
            // SecondFileSelectionContainer
            // 
            this.SecondFileSelectionContainer.AutoSize = true;
            this.SecondFileSelectionContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SecondFileSelectionContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondFileSelectionContainer.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.SecondFileSelectionContainer.Location = new System.Drawing.Point(598, 0);
            this.SecondFileSelectionContainer.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.SecondFileSelectionContainer.Name = "SecondFileSelectionContainer";
            this.SecondFileSelectionContainer.Size = new System.Drawing.Size(583, 1);
            this.SecondFileSelectionContainer.TabIndex = 7;
            // 
            // FirstFileSelectionContainer
            // 
            this.FirstFileSelectionContainer.AutoSize = true;
            this.FirstFileSelectionContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FirstFileSelectionContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FirstFileSelectionContainer.Location = new System.Drawing.Point(3, 0);
            this.FirstFileSelectionContainer.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.FirstFileSelectionContainer.Name = "FirstFileSelectionContainer";
            this.FirstFileSelectionContainer.Size = new System.Drawing.Size(583, 1);
            this.FirstFileSelectionContainer.TabIndex = 8;
            // 
            // ActionContainer
            // 
            this.ActionContainer.AutoSize = true;
            this.ActionContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ActionContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ActionContainer.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.ActionContainer.Location = new System.Drawing.Point(592, 206);
            this.ActionContainer.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.ActionContainer.Name = "ActionContainer";
            this.ActionContainer.Size = new System.Drawing.Size(1, 177);
            this.ActionContainer.TabIndex = 5;
            // 
            // NavigationContainer
            // 
            this.NavigationContainer.AutoSize = true;
            this.NavigationContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.NavigationContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NavigationContainer.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.NavigationContainer.Location = new System.Drawing.Point(592, 27);
            this.NavigationContainer.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.NavigationContainer.Name = "NavigationContainer";
            this.NavigationContainer.Size = new System.Drawing.Size(1, 179);
            this.NavigationContainer.TabIndex = 11;
            // 
            // FirstFileDetailsBorderPanel
            // 
            this.FirstFileDetailsBorderPanel.AutoSize = true;
            this.FirstFileDetailsBorderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FirstFileDetailsBorderPanel.Controls.Add(this.FirstFileDetailsView);
            this.FirstFileDetailsBorderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FirstFileDetailsBorderPanel.Location = new System.Drawing.Point(3, 386);
            this.FirstFileDetailsBorderPanel.Name = "FirstFileDetailsBorderPanel";
            this.FirstFileDetailsBorderPanel.Size = new System.Drawing.Size(583, 172);
            this.FirstFileDetailsBorderPanel.TabIndex = 13;
            // 
            // FirstFileDetailsView
            // 
            this.FirstFileDetailsView.BackColor = System.Drawing.SystemColors.Window;
            this.FirstFileDetailsView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FirstFileDetailsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FirstFileDetailsView.Location = new System.Drawing.Point(0, 0);
            this.FirstFileDetailsView.Name = "FirstFileDetailsView";
            this.FirstFileDetailsView.ReadOnly = true;
            this.FirstFileDetailsView.Size = new System.Drawing.Size(581, 170);
            this.FirstFileDetailsView.TabIndex = 9;
            this.FirstFileDetailsView.Text = "";
            this.FirstFileDetailsView.WordWrap = false;
            // 
            // SecondFileDetailsBorderPanel
            // 
            this.SecondFileDetailsBorderPanel.AutoSize = true;
            this.SecondFileDetailsBorderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SecondFileDetailsBorderPanel.Controls.Add(this.SecondFileDetailsView);
            this.SecondFileDetailsBorderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondFileDetailsBorderPanel.Location = new System.Drawing.Point(598, 386);
            this.SecondFileDetailsBorderPanel.Name = "SecondFileDetailsBorderPanel";
            this.SecondFileDetailsBorderPanel.Size = new System.Drawing.Size(583, 172);
            this.SecondFileDetailsBorderPanel.TabIndex = 14;
            // 
            // SecondFileDetailsView
            // 
            this.SecondFileDetailsView.BackColor = System.Drawing.SystemColors.Window;
            this.SecondFileDetailsView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SecondFileDetailsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondFileDetailsView.Location = new System.Drawing.Point(0, 0);
            this.SecondFileDetailsView.Name = "SecondFileDetailsView";
            this.SecondFileDetailsView.ReadOnly = true;
            this.SecondFileDetailsView.Size = new System.Drawing.Size(581, 170);
            this.SecondFileDetailsView.TabIndex = 10;
            this.SecondFileDetailsView.Text = "";
            this.SecondFileDetailsView.WordWrap = false;
            // 
            // FirstFileTreeBorderPanel
            // 
            this.FirstFileTreeBorderPanel.AutoSize = true;
            this.FirstFileTreeBorderPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FirstFileTreeBorderPanel.Controls.Add(this.FirstFileTreeView);
            this.FirstFileTreeBorderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FirstFileTreeBorderPanel.Location = new System.Drawing.Point(3, 30);
            this.FirstFileTreeBorderPanel.Name = "FirstFileTreeBorderPanel";
            this.FirstFileTreeBorderPanel.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.CompareAndMergePanel.SetRowSpan(this.FirstFileTreeBorderPanel, 2);
            this.FirstFileTreeBorderPanel.Size = new System.Drawing.Size(583, 350);
            this.FirstFileTreeBorderPanel.TabIndex = 15;
            this.FirstFileTreeBorderPanel.TabStop = false;
            this.FirstFileTreeBorderPanel.Text = "groupBox1";
            // 
            // FirstFileTreeView
            // 
            this.FirstFileTreeView.CheckBoxes = true;
            this.FirstFileTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FirstFileTreeView.Location = new System.Drawing.Point(0, 16);
            this.FirstFileTreeView.Name = "FirstFileTreeView";
            this.FirstFileTreeView.ShowNodeToolTips = true;
            this.FirstFileTreeView.Size = new System.Drawing.Size(583, 334);
            this.FirstFileTreeView.TabIndex = 2;
            // 
            // SecondFileTreeBorderPanel
            // 
            this.SecondFileTreeBorderPanel.AutoSize = true;
            this.SecondFileTreeBorderPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SecondFileTreeBorderPanel.Controls.Add(this.SecondFileTreeView);
            this.SecondFileTreeBorderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondFileTreeBorderPanel.Location = new System.Drawing.Point(598, 30);
            this.SecondFileTreeBorderPanel.Name = "SecondFileTreeBorderPanel";
            this.SecondFileTreeBorderPanel.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.CompareAndMergePanel.SetRowSpan(this.SecondFileTreeBorderPanel, 2);
            this.SecondFileTreeBorderPanel.Size = new System.Drawing.Size(583, 350);
            this.SecondFileTreeBorderPanel.TabIndex = 16;
            this.SecondFileTreeBorderPanel.TabStop = false;
            this.SecondFileTreeBorderPanel.Text = "groupBox1";
            // 
            // SecondFileTreeView
            // 
            this.SecondFileTreeView.CheckBoxes = true;
            this.SecondFileTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondFileTreeView.Location = new System.Drawing.Point(0, 16);
            this.SecondFileTreeView.Name = "SecondFileTreeView";
            this.SecondFileTreeView.ShowNodeToolTips = true;
            this.SecondFileTreeView.Size = new System.Drawing.Size(583, 334);
            this.SecondFileTreeView.TabIndex = 3;
            // 
            // EcfFileCAMDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 561);
            this.Controls.Add(this.CompareAndMergePanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EcfFileCAMDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EcfFileCompareAndMergeDialog";
            this.CompareAndMergePanel.ResumeLayout(false);
            this.CompareAndMergePanel.PerformLayout();
            this.FirstFileDetailsBorderPanel.ResumeLayout(false);
            this.SecondFileDetailsBorderPanel.ResumeLayout(false);
            this.FirstFileTreeBorderPanel.ResumeLayout(false);
            this.SecondFileTreeBorderPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel CompareAndMergePanel;
        private System.Windows.Forms.ComboBox FirstFileComboBox;
        private System.Windows.Forms.ComboBox SecondFileComboBox;
        private EcfWinFormControls.EcfTreeView FirstFileTreeView;
        private EcfWinFormControls.EcfTreeView SecondFileTreeView;
        private EcfToolBarControls.EcfToolContainer ActionContainer;
        private EcfToolBarControls.EcfToolContainer SecondFileSelectionContainer;
        private EcfToolBarControls.EcfToolContainer FirstFileSelectionContainer;
        private System.Windows.Forms.RichTextBox FirstFileDetailsView;
        private System.Windows.Forms.RichTextBox SecondFileDetailsView;
        private EcfToolBarControls.EcfToolContainer NavigationContainer;
        private System.Windows.Forms.Panel FirstFileDetailsBorderPanel;
        private System.Windows.Forms.Panel SecondFileDetailsBorderPanel;
        private System.Windows.Forms.GroupBox FirstFileTreeBorderPanel;
        private System.Windows.Forms.GroupBox SecondFileTreeBorderPanel;
    }
}