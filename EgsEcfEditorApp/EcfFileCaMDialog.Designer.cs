
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
            this.CompareAndMergePanel = new System.Windows.Forms.TableLayoutPanel();
            this.FirstFileComboBox = new System.Windows.Forms.ComboBox();
            this.SecondFileComboBox = new System.Windows.Forms.ComboBox();
            this.FirstFileTreeView = new EcfWinFormControls.EcfTreeView();
            this.SecondFileTreeView = new EcfWinFormControls.EcfTreeView();
            this.SecondFileSelectionContainer = new EcfToolBarControls.EcfToolContainer();
            this.FirstFileSelectionContainer = new EcfToolBarControls.EcfToolContainer();
            this.FirstFileDetailsView = new System.Windows.Forms.RichTextBox();
            this.SecondFileDetailsView = new System.Windows.Forms.RichTextBox();
            this.ActionContainer = new EcfToolBarControls.EcfToolContainer();
            this.NavigationContainer = new EcfToolBarControls.EcfToolContainer();
            this.PageIndicator = new System.Windows.Forms.TextBox();
            this.FirstFileDetailsBorderPanel = new System.Windows.Forms.Panel();
            this.SecondFileDetailsBorderPanel = new System.Windows.Forms.Panel();
            this.ButtonPanel.SuspendLayout();
            this.CompareAndMergePanel.SuspendLayout();
            this.FirstFileDetailsBorderPanel.SuspendLayout();
            this.SecondFileDetailsBorderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.AutoSize = true;
            this.ButtonPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
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
            // CompareAndMergePanel
            // 
            this.CompareAndMergePanel.AutoSize = true;
            this.CompareAndMergePanel.ColumnCount = 5;
            this.CompareAndMergePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.CompareAndMergePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.CompareAndMergePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.CompareAndMergePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.CompareAndMergePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.CompareAndMergePanel.Controls.Add(this.FirstFileComboBox, 0, 1);
            this.CompareAndMergePanel.Controls.Add(this.SecondFileComboBox, 3, 1);
            this.CompareAndMergePanel.Controls.Add(this.FirstFileTreeView, 0, 2);
            this.CompareAndMergePanel.Controls.Add(this.SecondFileTreeView, 3, 2);
            this.CompareAndMergePanel.Controls.Add(this.SecondFileSelectionContainer, 4, 0);
            this.CompareAndMergePanel.Controls.Add(this.FirstFileSelectionContainer, 0, 0);
            this.CompareAndMergePanel.Controls.Add(this.ActionContainer, 2, 3);
            this.CompareAndMergePanel.Controls.Add(this.NavigationContainer, 2, 2);
            this.CompareAndMergePanel.Controls.Add(this.PageIndicator, 1, 0);
            this.CompareAndMergePanel.Controls.Add(this.FirstFileDetailsBorderPanel, 0, 4);
            this.CompareAndMergePanel.Controls.Add(this.SecondFileDetailsBorderPanel, 3, 4);
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
            this.CompareAndMergePanel.Size = new System.Drawing.Size(1184, 532);
            this.CompareAndMergePanel.TabIndex = 1;
            // 
            // FirstFileComboBox
            // 
            this.CompareAndMergePanel.SetColumnSpan(this.FirstFileComboBox, 2);
            this.FirstFileComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FirstFileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FirstFileComboBox.FormattingEnabled = true;
            this.FirstFileComboBox.Location = new System.Drawing.Point(3, 29);
            this.FirstFileComboBox.Name = "FirstFileComboBox";
            this.FirstFileComboBox.Size = new System.Drawing.Size(582, 21);
            this.FirstFileComboBox.Sorted = true;
            this.FirstFileComboBox.TabIndex = 0;
            this.FirstFileComboBox.SelectionChangeCommitted += new System.EventHandler(this.FirstFileComboBox_SelectionChangeCommitted);
            // 
            // SecondFileComboBox
            // 
            this.CompareAndMergePanel.SetColumnSpan(this.SecondFileComboBox, 2);
            this.SecondFileComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondFileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SecondFileComboBox.FormattingEnabled = true;
            this.SecondFileComboBox.Location = new System.Drawing.Point(597, 29);
            this.SecondFileComboBox.Name = "SecondFileComboBox";
            this.SecondFileComboBox.Size = new System.Drawing.Size(584, 21);
            this.SecondFileComboBox.Sorted = true;
            this.SecondFileComboBox.TabIndex = 1;
            this.SecondFileComboBox.SelectionChangeCommitted += new System.EventHandler(this.SecondFileComboBox_SelectionChangeCommitted);
            // 
            // FirstFileTreeView
            // 
            this.FirstFileTreeView.CheckBoxes = true;
            this.CompareAndMergePanel.SetColumnSpan(this.FirstFileTreeView, 2);
            this.FirstFileTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FirstFileTreeView.Location = new System.Drawing.Point(3, 56);
            this.FirstFileTreeView.Name = "FirstFileTreeView";
            this.CompareAndMergePanel.SetRowSpan(this.FirstFileTreeView, 2);
            this.FirstFileTreeView.ShowNodeToolTips = true;
            this.FirstFileTreeView.Size = new System.Drawing.Size(582, 313);
            this.FirstFileTreeView.TabIndex = 2;
            // 
            // SecondFileTreeView
            // 
            this.SecondFileTreeView.CheckBoxes = true;
            this.CompareAndMergePanel.SetColumnSpan(this.SecondFileTreeView, 2);
            this.SecondFileTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondFileTreeView.Location = new System.Drawing.Point(597, 56);
            this.SecondFileTreeView.Name = "SecondFileTreeView";
            this.CompareAndMergePanel.SetRowSpan(this.SecondFileTreeView, 2);
            this.SecondFileTreeView.ShowNodeToolTips = true;
            this.SecondFileTreeView.Size = new System.Drawing.Size(584, 313);
            this.SecondFileTreeView.TabIndex = 3;
            // 
            // SecondFileSelectionContainer
            // 
            this.SecondFileSelectionContainer.AutoSize = true;
            this.SecondFileSelectionContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SecondFileSelectionContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondFileSelectionContainer.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.SecondFileSelectionContainer.Location = new System.Drawing.Point(655, 0);
            this.SecondFileSelectionContainer.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.SecondFileSelectionContainer.Name = "SecondFileSelectionContainer";
            this.SecondFileSelectionContainer.Size = new System.Drawing.Size(526, 26);
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
            this.FirstFileSelectionContainer.Size = new System.Drawing.Size(524, 26);
            this.FirstFileSelectionContainer.TabIndex = 8;
            // 
            // FirstFileDetailsView
            // 
            this.FirstFileDetailsView.BackColor = System.Drawing.SystemColors.Window;
            this.FirstFileDetailsView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FirstFileDetailsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FirstFileDetailsView.Location = new System.Drawing.Point(0, 0);
            this.FirstFileDetailsView.Name = "FirstFileDetailsView";
            this.FirstFileDetailsView.ReadOnly = true;
            this.FirstFileDetailsView.Size = new System.Drawing.Size(580, 152);
            this.FirstFileDetailsView.TabIndex = 9;
            this.FirstFileDetailsView.Text = "";
            this.FirstFileDetailsView.WordWrap = false;
            // 
            // SecondFileDetailsView
            // 
            this.SecondFileDetailsView.BackColor = System.Drawing.SystemColors.Window;
            this.SecondFileDetailsView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SecondFileDetailsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondFileDetailsView.Location = new System.Drawing.Point(0, 0);
            this.SecondFileDetailsView.Name = "SecondFileDetailsView";
            this.SecondFileDetailsView.ReadOnly = true;
            this.SecondFileDetailsView.Size = new System.Drawing.Size(582, 152);
            this.SecondFileDetailsView.TabIndex = 10;
            this.SecondFileDetailsView.Text = "";
            this.SecondFileDetailsView.WordWrap = false;
            // 
            // ActionContainer
            // 
            this.ActionContainer.AutoSize = true;
            this.ActionContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ActionContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ActionContainer.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.ActionContainer.Location = new System.Drawing.Point(591, 213);
            this.ActionContainer.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.ActionContainer.Name = "ActionContainer";
            this.ActionContainer.Size = new System.Drawing.Size(1, 159);
            this.ActionContainer.TabIndex = 5;
            // 
            // NavigationContainer
            // 
            this.NavigationContainer.AutoSize = true;
            this.NavigationContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.NavigationContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NavigationContainer.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.NavigationContainer.Location = new System.Drawing.Point(591, 53);
            this.NavigationContainer.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.NavigationContainer.Name = "NavigationContainer";
            this.NavigationContainer.Size = new System.Drawing.Size(1, 160);
            this.NavigationContainer.TabIndex = 11;
            // 
            // PageIndicator
            // 
            this.PageIndicator.BackColor = System.Drawing.SystemColors.Window;
            this.CompareAndMergePanel.SetColumnSpan(this.PageIndicator, 3);
            this.PageIndicator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PageIndicator.Location = new System.Drawing.Point(533, 3);
            this.PageIndicator.Name = "PageIndicator";
            this.PageIndicator.ReadOnly = true;
            this.PageIndicator.Size = new System.Drawing.Size(116, 20);
            this.PageIndicator.TabIndex = 12;
            this.PageIndicator.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FirstFileDetailsBorderPanel
            // 
            this.FirstFileDetailsBorderPanel.AutoSize = true;
            this.FirstFileDetailsBorderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CompareAndMergePanel.SetColumnSpan(this.FirstFileDetailsBorderPanel, 2);
            this.FirstFileDetailsBorderPanel.Controls.Add(this.FirstFileDetailsView);
            this.FirstFileDetailsBorderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FirstFileDetailsBorderPanel.Location = new System.Drawing.Point(3, 375);
            this.FirstFileDetailsBorderPanel.Name = "FirstFileDetailsBorderPanel";
            this.FirstFileDetailsBorderPanel.Size = new System.Drawing.Size(582, 154);
            this.FirstFileDetailsBorderPanel.TabIndex = 13;
            // 
            // SecondFileDetailsBorderPanel
            // 
            this.SecondFileDetailsBorderPanel.AutoSize = true;
            this.SecondFileDetailsBorderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CompareAndMergePanel.SetColumnSpan(this.SecondFileDetailsBorderPanel, 2);
            this.SecondFileDetailsBorderPanel.Controls.Add(this.SecondFileDetailsView);
            this.SecondFileDetailsBorderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondFileDetailsBorderPanel.Location = new System.Drawing.Point(597, 375);
            this.SecondFileDetailsBorderPanel.Name = "SecondFileDetailsBorderPanel";
            this.SecondFileDetailsBorderPanel.Size = new System.Drawing.Size(584, 154);
            this.SecondFileDetailsBorderPanel.TabIndex = 14;
            // 
            // EcfFileCAMDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 561);
            this.Controls.Add(this.CompareAndMergePanel);
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
            this.CompareAndMergePanel.ResumeLayout(false);
            this.CompareAndMergePanel.PerformLayout();
            this.FirstFileDetailsBorderPanel.ResumeLayout(false);
            this.SecondFileDetailsBorderPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel ButtonPanel;
        private System.Windows.Forms.Button CloseButton;
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
        private System.Windows.Forms.TextBox PageIndicator;
        private System.Windows.Forms.Panel FirstFileDetailsBorderPanel;
        private System.Windows.Forms.Panel SecondFileDetailsBorderPanel;
    }
}