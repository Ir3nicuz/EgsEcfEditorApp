
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
            this.FirstFileActionContainer = new EcfToolBarControls.EcfToolContainer();
            this.SecondFileActionContainer = new EcfToolBarControls.EcfToolContainer();
            this.SecondFileSelectionContainer = new EcfToolBarControls.EcfToolContainer();
            this.FirstFileSelectionContainer = new EcfToolBarControls.EcfToolContainer();
            this.FirstFileDetailsView = new System.Windows.Forms.RichTextBox();
            this.SecondFileDetailsView = new System.Windows.Forms.RichTextBox();
            this.ButtonPanel.SuspendLayout();
            this.CompareAndMergePanel.SuspendLayout();
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
            // CompareAndMergePanel
            // 
            this.CompareAndMergePanel.AutoSize = true;
            this.CompareAndMergePanel.ColumnCount = 4;
            this.CompareAndMergePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.CompareAndMergePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.CompareAndMergePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.CompareAndMergePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.CompareAndMergePanel.Controls.Add(this.FirstFileComboBox, 0, 1);
            this.CompareAndMergePanel.Controls.Add(this.SecondFileComboBox, 2, 1);
            this.CompareAndMergePanel.Controls.Add(this.FirstFileTreeView, 0, 2);
            this.CompareAndMergePanel.Controls.Add(this.SecondFileTreeView, 2, 2);
            this.CompareAndMergePanel.Controls.Add(this.FirstFileActionContainer, 1, 0);
            this.CompareAndMergePanel.Controls.Add(this.SecondFileActionContainer, 2, 0);
            this.CompareAndMergePanel.Controls.Add(this.SecondFileSelectionContainer, 3, 0);
            this.CompareAndMergePanel.Controls.Add(this.FirstFileSelectionContainer, 0, 0);
            this.CompareAndMergePanel.Controls.Add(this.FirstFileDetailsView, 0, 3);
            this.CompareAndMergePanel.Controls.Add(this.SecondFileDetailsView, 2, 3);
            this.CompareAndMergePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CompareAndMergePanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.CompareAndMergePanel.Location = new System.Drawing.Point(0, 0);
            this.CompareAndMergePanel.Name = "CompareAndMergePanel";
            this.CompareAndMergePanel.RowCount = 4;
            this.CompareAndMergePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.CompareAndMergePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.CompareAndMergePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 76.92308F));
            this.CompareAndMergePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.07692F));
            this.CompareAndMergePanel.Size = new System.Drawing.Size(1184, 532);
            this.CompareAndMergePanel.TabIndex = 1;
            // 
            // FirstFileComboBox
            // 
            this.CompareAndMergePanel.SetColumnSpan(this.FirstFileComboBox, 2);
            this.FirstFileComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FirstFileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FirstFileComboBox.FormattingEnabled = true;
            this.FirstFileComboBox.Location = new System.Drawing.Point(3, 3);
            this.FirstFileComboBox.Name = "FirstFileComboBox";
            this.FirstFileComboBox.Size = new System.Drawing.Size(586, 21);
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
            this.SecondFileComboBox.Location = new System.Drawing.Point(595, 3);
            this.SecondFileComboBox.Name = "SecondFileComboBox";
            this.SecondFileComboBox.Size = new System.Drawing.Size(586, 21);
            this.SecondFileComboBox.Sorted = true;
            this.SecondFileComboBox.TabIndex = 1;
            this.SecondFileComboBox.SelectionChangeCommitted += new System.EventHandler(this.SecondFileComboBox_SelectionChangeCommitted);
            // 
            // FirstFileTreeView
            // 
            this.FirstFileTreeView.CheckBoxes = true;
            this.CompareAndMergePanel.SetColumnSpan(this.FirstFileTreeView, 2);
            this.FirstFileTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FirstFileTreeView.Location = new System.Drawing.Point(3, 30);
            this.FirstFileTreeView.Name = "FirstFileTreeView";
            this.FirstFileTreeView.ShowNodeToolTips = true;
            this.FirstFileTreeView.Size = new System.Drawing.Size(586, 382);
            this.FirstFileTreeView.TabIndex = 2;
            // 
            // SecondFileTreeView
            // 
            this.SecondFileTreeView.CheckBoxes = true;
            this.CompareAndMergePanel.SetColumnSpan(this.SecondFileTreeView, 2);
            this.SecondFileTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondFileTreeView.Location = new System.Drawing.Point(595, 30);
            this.SecondFileTreeView.Name = "SecondFileTreeView";
            this.SecondFileTreeView.ShowNodeToolTips = true;
            this.SecondFileTreeView.Size = new System.Drawing.Size(586, 382);
            this.SecondFileTreeView.TabIndex = 3;
            // 
            // FirstFileActionContainer
            // 
            this.FirstFileActionContainer.AutoSize = true;
            this.FirstFileActionContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FirstFileActionContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FirstFileActionContainer.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.FirstFileActionContainer.Location = new System.Drawing.Point(299, 0);
            this.FirstFileActionContainer.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.FirstFileActionContainer.Name = "FirstFileActionContainer";
            this.FirstFileActionContainer.Size = new System.Drawing.Size(290, 1);
            this.FirstFileActionContainer.TabIndex = 5;
            // 
            // SecondFileActionContainer
            // 
            this.SecondFileActionContainer.AutoSize = true;
            this.SecondFileActionContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SecondFileActionContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondFileActionContainer.Location = new System.Drawing.Point(595, 0);
            this.SecondFileActionContainer.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.SecondFileActionContainer.Name = "SecondFileActionContainer";
            this.SecondFileActionContainer.Size = new System.Drawing.Size(290, 1);
            this.SecondFileActionContainer.TabIndex = 6;
            // 
            // SecondFileSelectionContainer
            // 
            this.SecondFileSelectionContainer.AutoSize = true;
            this.SecondFileSelectionContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SecondFileSelectionContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondFileSelectionContainer.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.SecondFileSelectionContainer.Location = new System.Drawing.Point(891, 0);
            this.SecondFileSelectionContainer.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.SecondFileSelectionContainer.Name = "SecondFileSelectionContainer";
            this.SecondFileSelectionContainer.Size = new System.Drawing.Size(290, 1);
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
            this.FirstFileSelectionContainer.Size = new System.Drawing.Size(290, 1);
            this.FirstFileSelectionContainer.TabIndex = 8;
            // 
            // FirstFileDetailsView
            // 
            this.CompareAndMergePanel.SetColumnSpan(this.FirstFileDetailsView, 2);
            this.FirstFileDetailsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FirstFileDetailsView.Location = new System.Drawing.Point(3, 418);
            this.FirstFileDetailsView.Name = "FirstFileDetailsView";
            this.FirstFileDetailsView.ReadOnly = true;
            this.FirstFileDetailsView.Size = new System.Drawing.Size(586, 111);
            this.FirstFileDetailsView.TabIndex = 9;
            this.FirstFileDetailsView.Text = "";
            this.FirstFileDetailsView.WordWrap = false;
            // 
            // SecondFileDetailsView
            // 
            this.CompareAndMergePanel.SetColumnSpan(this.SecondFileDetailsView, 2);
            this.SecondFileDetailsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondFileDetailsView.Location = new System.Drawing.Point(595, 418);
            this.SecondFileDetailsView.Name = "SecondFileDetailsView";
            this.SecondFileDetailsView.ReadOnly = true;
            this.SecondFileDetailsView.Size = new System.Drawing.Size(586, 111);
            this.SecondFileDetailsView.TabIndex = 10;
            this.SecondFileDetailsView.Text = "";
            this.SecondFileDetailsView.WordWrap = false;
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
        private EcfToolBarControls.EcfToolContainer FirstFileActionContainer;
        private EcfToolBarControls.EcfToolContainer SecondFileActionContainer;
        private EcfToolBarControls.EcfToolContainer SecondFileSelectionContainer;
        private EcfToolBarControls.EcfToolContainer FirstFileSelectionContainer;
        private System.Windows.Forms.RichTextBox FirstFileDetailsView;
        private System.Windows.Forms.RichTextBox SecondFileDetailsView;
    }
}