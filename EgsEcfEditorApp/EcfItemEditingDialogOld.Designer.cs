
namespace EcfFileViews
{
    partial class EcfItemEditingDialogOld
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
            this.CommentItemRichTextBox = new System.Windows.Forms.RichTextBox();
            this.ParameterItemView = new System.Windows.Forms.TabPage();
            this.ParameterItemViewPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ParameterItemKeyPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ParameterItemKeyLabel = new System.Windows.Forms.Label();
            this.ParameterItemIsOptionalCheckBox = new System.Windows.Forms.CheckBox();
            this.ParameterItemKeyComboBox = new System.Windows.Forms.ComboBox();
            this.ParameterItemInfoPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ParameterItemCommentLabel = new System.Windows.Forms.Label();
            this.ParameterItemCommentTextBox = new System.Windows.Forms.TextBox();
            this.ParameterItemInfoLabel = new System.Windows.Forms.Label();
            this.ParameterItemInfoTextBox = new System.Windows.Forms.TextBox();
            this.ParameterItemParentLabel = new System.Windows.Forms.Label();
            this.ParameterItemParentTextBox = new System.Windows.Forms.TextBox();
            this.BlockItemView = new System.Windows.Forms.TabPage();
            this.BlockItemViewPanel = new System.Windows.Forms.TableLayoutPanel();
            this.BlockItemTypePanel = new System.Windows.Forms.TableLayoutPanel();
            this.BlockItemDataTypeLabel = new System.Windows.Forms.Label();
            this.BlockItemDataTypeComboBox = new System.Windows.Forms.ComboBox();
            this.BlockItemPreMarkLabel = new System.Windows.Forms.Label();
            this.BlockItemPreMarkComboBox = new System.Windows.Forms.ComboBox();
            this.BlockItemPostMarkLabel = new System.Windows.Forms.Label();
            this.BlockItemPostMarkComboBox = new System.Windows.Forms.ComboBox();
            this.BlockItemAddDataPanel = new System.Windows.Forms.TableLayoutPanel();
            this.BlockItemInheritorLabel = new System.Windows.Forms.Label();
            this.BlockItemInheritorTextBox = new System.Windows.Forms.TextBox();
            this.BlockItemCommentLabel = new System.Windows.Forms.Label();
            this.BlockItemCommentTextBox = new System.Windows.Forms.TextBox();
            this.ParameterMatrixView = new System.Windows.Forms.TabPage();
            this.MessagePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.ButtonPanel.SuspendLayout();
            this.ViewPanel.SuspendLayout();
            this.CommentItemView.SuspendLayout();
            this.ParameterItemView.SuspendLayout();
            this.ParameterItemViewPanel.SuspendLayout();
            this.ParameterItemKeyPanel.SuspendLayout();
            this.ParameterItemInfoPanel.SuspendLayout();
            this.BlockItemView.SuspendLayout();
            this.BlockItemViewPanel.SuspendLayout();
            this.BlockItemTypePanel.SuspendLayout();
            this.BlockItemAddDataPanel.SuspendLayout();
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
            this.CommentItemView.Controls.Add(this.CommentItemRichTextBox);
            this.CommentItemView.Location = new System.Drawing.Point(4, 24);
            this.CommentItemView.Name = "CommentItemView";
            this.CommentItemView.Size = new System.Drawing.Size(1176, 604);
            this.CommentItemView.TabIndex = 1;
            this.CommentItemView.Text = "CommentItemView";
            this.CommentItemView.UseVisualStyleBackColor = true;
            // 
            // CommentItemRichTextBox
            // 
            this.CommentItemRichTextBox.AcceptsTab = true;
            this.CommentItemRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CommentItemRichTextBox.Location = new System.Drawing.Point(0, 0);
            this.CommentItemRichTextBox.Name = "CommentItemRichTextBox";
            this.CommentItemRichTextBox.Size = new System.Drawing.Size(1176, 604);
            this.CommentItemRichTextBox.TabIndex = 0;
            this.CommentItemRichTextBox.Text = "";
            // 
            // ParameterItemView
            // 
            this.ParameterItemView.Controls.Add(this.ParameterItemViewPanel);
            this.ParameterItemView.Location = new System.Drawing.Point(4, 24);
            this.ParameterItemView.Name = "ParameterItemView";
            this.ParameterItemView.Size = new System.Drawing.Size(1176, 604);
            this.ParameterItemView.TabIndex = 2;
            this.ParameterItemView.Text = "ParameterItemView";
            this.ParameterItemView.UseVisualStyleBackColor = true;
            // 
            // ParameterItemViewPanel
            // 
            this.ParameterItemViewPanel.AutoSize = true;
            this.ParameterItemViewPanel.ColumnCount = 2;
            this.ParameterItemViewPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.ParameterItemViewPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.ParameterItemViewPanel.Controls.Add(this.ParameterItemKeyPanel, 0, 0);
            this.ParameterItemViewPanel.Controls.Add(this.ParameterItemInfoPanel, 1, 0);
            this.ParameterItemViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ParameterItemViewPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.ParameterItemViewPanel.Location = new System.Drawing.Point(0, 0);
            this.ParameterItemViewPanel.Name = "ParameterItemViewPanel";
            this.ParameterItemViewPanel.RowCount = 3;
            this.ParameterItemViewPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.ParameterItemViewPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.ParameterItemViewPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ParameterItemViewPanel.Size = new System.Drawing.Size(1176, 604);
            this.ParameterItemViewPanel.TabIndex = 0;
            // 
            // ParameterItemKeyPanel
            // 
            this.ParameterItemKeyPanel.AutoSize = true;
            this.ParameterItemKeyPanel.ColumnCount = 2;
            this.ParameterItemKeyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ParameterItemKeyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ParameterItemKeyPanel.Controls.Add(this.ParameterItemKeyLabel, 0, 0);
            this.ParameterItemKeyPanel.Controls.Add(this.ParameterItemIsOptionalCheckBox, 1, 0);
            this.ParameterItemKeyPanel.Controls.Add(this.ParameterItemKeyComboBox, 0, 1);
            this.ParameterItemKeyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ParameterItemKeyPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.ParameterItemKeyPanel.Location = new System.Drawing.Point(3, 3);
            this.ParameterItemKeyPanel.Name = "ParameterItemKeyPanel";
            this.ParameterItemKeyPanel.RowCount = 3;
            this.ParameterItemKeyPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.ParameterItemKeyPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.ParameterItemKeyPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.ParameterItemKeyPanel.Size = new System.Drawing.Size(346, 84);
            this.ParameterItemKeyPanel.TabIndex = 0;
            // 
            // ParameterItemKeyLabel
            // 
            this.ParameterItemKeyLabel.AutoSize = true;
            this.ParameterItemKeyLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ParameterItemKeyLabel.Location = new System.Drawing.Point(3, 0);
            this.ParameterItemKeyLabel.Name = "ParameterItemKeyLabel";
            this.ParameterItemKeyLabel.Size = new System.Drawing.Size(167, 28);
            this.ParameterItemKeyLabel.TabIndex = 1;
            this.ParameterItemKeyLabel.Text = "key";
            this.ParameterItemKeyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ParameterItemIsOptionalCheckBox
            // 
            this.ParameterItemIsOptionalCheckBox.AutoSize = true;
            this.ParameterItemIsOptionalCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ParameterItemIsOptionalCheckBox.Enabled = false;
            this.ParameterItemIsOptionalCheckBox.Location = new System.Drawing.Point(176, 3);
            this.ParameterItemIsOptionalCheckBox.Name = "ParameterItemIsOptionalCheckBox";
            this.ParameterItemIsOptionalCheckBox.Size = new System.Drawing.Size(167, 22);
            this.ParameterItemIsOptionalCheckBox.TabIndex = 2;
            this.ParameterItemIsOptionalCheckBox.Text = "isOpt";
            this.ParameterItemIsOptionalCheckBox.UseVisualStyleBackColor = true;
            // 
            // ParameterItemKeyComboBox
            // 
            this.ParameterItemKeyPanel.SetColumnSpan(this.ParameterItemKeyComboBox, 2);
            this.ParameterItemKeyComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ParameterItemKeyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ParameterItemKeyComboBox.FormattingEnabled = true;
            this.ParameterItemKeyComboBox.Location = new System.Drawing.Point(3, 31);
            this.ParameterItemKeyComboBox.Name = "ParameterItemKeyComboBox";
            this.ParameterItemKeyComboBox.Size = new System.Drawing.Size(340, 21);
            this.ParameterItemKeyComboBox.Sorted = true;
            this.ParameterItemKeyComboBox.TabIndex = 3;
            this.ParameterItemKeyComboBox.SelectionChangeCommitted += new System.EventHandler(this.ParameterItemKeyComboBox_SelectionChangeCommitted);
            // 
            // ParameterItemInfoPanel
            // 
            this.ParameterItemInfoPanel.AutoSize = true;
            this.ParameterItemInfoPanel.ColumnCount = 2;
            this.ParameterItemInfoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.ParameterItemInfoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.ParameterItemInfoPanel.Controls.Add(this.ParameterItemCommentLabel, 0, 2);
            this.ParameterItemInfoPanel.Controls.Add(this.ParameterItemCommentTextBox, 1, 2);
            this.ParameterItemInfoPanel.Controls.Add(this.ParameterItemInfoLabel, 0, 1);
            this.ParameterItemInfoPanel.Controls.Add(this.ParameterItemInfoTextBox, 1, 1);
            this.ParameterItemInfoPanel.Controls.Add(this.ParameterItemParentLabel, 0, 0);
            this.ParameterItemInfoPanel.Controls.Add(this.ParameterItemParentTextBox, 1, 0);
            this.ParameterItemInfoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ParameterItemInfoPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.ParameterItemInfoPanel.Location = new System.Drawing.Point(355, 3);
            this.ParameterItemInfoPanel.Name = "ParameterItemInfoPanel";
            this.ParameterItemInfoPanel.RowCount = 3;
            this.ParameterItemInfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.ParameterItemInfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.ParameterItemInfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.ParameterItemInfoPanel.Size = new System.Drawing.Size(818, 84);
            this.ParameterItemInfoPanel.TabIndex = 1;
            // 
            // ParameterItemCommentLabel
            // 
            this.ParameterItemCommentLabel.AutoSize = true;
            this.ParameterItemCommentLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ParameterItemCommentLabel.Location = new System.Drawing.Point(3, 56);
            this.ParameterItemCommentLabel.Name = "ParameterItemCommentLabel";
            this.ParameterItemCommentLabel.Size = new System.Drawing.Size(157, 28);
            this.ParameterItemCommentLabel.TabIndex = 1;
            this.ParameterItemCommentLabel.Text = "comment";
            this.ParameterItemCommentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ParameterItemCommentTextBox
            // 
            this.ParameterItemCommentTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ParameterItemCommentTextBox.Location = new System.Drawing.Point(166, 59);
            this.ParameterItemCommentTextBox.Name = "ParameterItemCommentTextBox";
            this.ParameterItemCommentTextBox.Size = new System.Drawing.Size(649, 20);
            this.ParameterItemCommentTextBox.TabIndex = 3;
            // 
            // ParameterItemInfoLabel
            // 
            this.ParameterItemInfoLabel.AutoSize = true;
            this.ParameterItemInfoLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ParameterItemInfoLabel.Location = new System.Drawing.Point(3, 28);
            this.ParameterItemInfoLabel.Name = "ParameterItemInfoLabel";
            this.ParameterItemInfoLabel.Size = new System.Drawing.Size(157, 28);
            this.ParameterItemInfoLabel.TabIndex = 0;
            this.ParameterItemInfoLabel.Text = "info";
            this.ParameterItemInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ParameterItemInfoTextBox
            // 
            this.ParameterItemInfoTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ParameterItemInfoTextBox.Location = new System.Drawing.Point(166, 31);
            this.ParameterItemInfoTextBox.Name = "ParameterItemInfoTextBox";
            this.ParameterItemInfoTextBox.ReadOnly = true;
            this.ParameterItemInfoTextBox.Size = new System.Drawing.Size(649, 20);
            this.ParameterItemInfoTextBox.TabIndex = 2;
            // 
            // ParameterItemParentLabel
            // 
            this.ParameterItemParentLabel.AutoSize = true;
            this.ParameterItemParentLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ParameterItemParentLabel.Location = new System.Drawing.Point(3, 0);
            this.ParameterItemParentLabel.Name = "ParameterItemParentLabel";
            this.ParameterItemParentLabel.Size = new System.Drawing.Size(157, 28);
            this.ParameterItemParentLabel.TabIndex = 4;
            this.ParameterItemParentLabel.Text = "parent";
            this.ParameterItemParentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ParameterItemParentTextBox
            // 
            this.ParameterItemParentTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ParameterItemParentTextBox.Location = new System.Drawing.Point(166, 3);
            this.ParameterItemParentTextBox.Name = "ParameterItemParentTextBox";
            this.ParameterItemParentTextBox.ReadOnly = true;
            this.ParameterItemParentTextBox.Size = new System.Drawing.Size(649, 20);
            this.ParameterItemParentTextBox.TabIndex = 5;
            // 
            // BlockItemView
            // 
            this.BlockItemView.Controls.Add(this.BlockItemViewPanel);
            this.BlockItemView.Location = new System.Drawing.Point(4, 24);
            this.BlockItemView.Name = "BlockItemView";
            this.BlockItemView.Size = new System.Drawing.Size(1176, 604);
            this.BlockItemView.TabIndex = 3;
            this.BlockItemView.Text = "BlockItemView";
            this.BlockItemView.UseVisualStyleBackColor = true;
            // 
            // BlockItemViewPanel
            // 
            this.BlockItemViewPanel.AutoSize = true;
            this.BlockItemViewPanel.ColumnCount = 2;
            this.BlockItemViewPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.BlockItemViewPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.BlockItemViewPanel.Controls.Add(this.BlockItemTypePanel, 0, 0);
            this.BlockItemViewPanel.Controls.Add(this.BlockItemAddDataPanel, 1, 0);
            this.BlockItemViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BlockItemViewPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.BlockItemViewPanel.Location = new System.Drawing.Point(0, 0);
            this.BlockItemViewPanel.Name = "BlockItemViewPanel";
            this.BlockItemViewPanel.RowCount = 3;
            this.BlockItemViewPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.BlockItemViewPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.BlockItemViewPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.BlockItemViewPanel.Size = new System.Drawing.Size(1176, 604);
            this.BlockItemViewPanel.TabIndex = 0;
            // 
            // BlockItemTypePanel
            // 
            this.BlockItemTypePanel.AutoSize = true;
            this.BlockItemTypePanel.ColumnCount = 2;
            this.BlockItemTypePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.BlockItemTypePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.BlockItemTypePanel.Controls.Add(this.BlockItemDataTypeLabel, 0, 1);
            this.BlockItemTypePanel.Controls.Add(this.BlockItemDataTypeComboBox, 1, 1);
            this.BlockItemTypePanel.Controls.Add(this.BlockItemPreMarkLabel, 0, 0);
            this.BlockItemTypePanel.Controls.Add(this.BlockItemPreMarkComboBox, 1, 0);
            this.BlockItemTypePanel.Controls.Add(this.BlockItemPostMarkLabel, 0, 2);
            this.BlockItemTypePanel.Controls.Add(this.BlockItemPostMarkComboBox, 1, 2);
            this.BlockItemTypePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BlockItemTypePanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.BlockItemTypePanel.Location = new System.Drawing.Point(3, 3);
            this.BlockItemTypePanel.Name = "BlockItemTypePanel";
            this.BlockItemTypePanel.RowCount = 3;
            this.BlockItemTypePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.BlockItemTypePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.BlockItemTypePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.BlockItemTypePanel.Size = new System.Drawing.Size(346, 84);
            this.BlockItemTypePanel.TabIndex = 2;
            // 
            // BlockItemDataTypeLabel
            // 
            this.BlockItemDataTypeLabel.AutoSize = true;
            this.BlockItemDataTypeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BlockItemDataTypeLabel.Location = new System.Drawing.Point(3, 28);
            this.BlockItemDataTypeLabel.Name = "BlockItemDataTypeLabel";
            this.BlockItemDataTypeLabel.Size = new System.Drawing.Size(132, 28);
            this.BlockItemDataTypeLabel.TabIndex = 0;
            this.BlockItemDataTypeLabel.Text = "datatype";
            this.BlockItemDataTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BlockItemDataTypeComboBox
            // 
            this.BlockItemDataTypeComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BlockItemDataTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BlockItemDataTypeComboBox.FormattingEnabled = true;
            this.BlockItemDataTypeComboBox.Location = new System.Drawing.Point(141, 31);
            this.BlockItemDataTypeComboBox.Name = "BlockItemDataTypeComboBox";
            this.BlockItemDataTypeComboBox.Size = new System.Drawing.Size(202, 21);
            this.BlockItemDataTypeComboBox.TabIndex = 2;
            // 
            // BlockItemPreMarkLabel
            // 
            this.BlockItemPreMarkLabel.AutoSize = true;
            this.BlockItemPreMarkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BlockItemPreMarkLabel.Location = new System.Drawing.Point(3, 0);
            this.BlockItemPreMarkLabel.Name = "BlockItemPreMarkLabel";
            this.BlockItemPreMarkLabel.Size = new System.Drawing.Size(132, 28);
            this.BlockItemPreMarkLabel.TabIndex = 0;
            this.BlockItemPreMarkLabel.Text = "premark";
            this.BlockItemPreMarkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BlockItemPreMarkComboBox
            // 
            this.BlockItemPreMarkComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BlockItemPreMarkComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BlockItemPreMarkComboBox.FormattingEnabled = true;
            this.BlockItemPreMarkComboBox.Location = new System.Drawing.Point(141, 3);
            this.BlockItemPreMarkComboBox.Name = "BlockItemPreMarkComboBox";
            this.BlockItemPreMarkComboBox.Size = new System.Drawing.Size(202, 21);
            this.BlockItemPreMarkComboBox.TabIndex = 1;
            // 
            // BlockItemPostMarkLabel
            // 
            this.BlockItemPostMarkLabel.AutoSize = true;
            this.BlockItemPostMarkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BlockItemPostMarkLabel.Location = new System.Drawing.Point(3, 56);
            this.BlockItemPostMarkLabel.Name = "BlockItemPostMarkLabel";
            this.BlockItemPostMarkLabel.Size = new System.Drawing.Size(132, 28);
            this.BlockItemPostMarkLabel.TabIndex = 3;
            this.BlockItemPostMarkLabel.Text = "postmark";
            this.BlockItemPostMarkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BlockItemPostMarkComboBox
            // 
            this.BlockItemPostMarkComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BlockItemPostMarkComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BlockItemPostMarkComboBox.FormattingEnabled = true;
            this.BlockItemPostMarkComboBox.Location = new System.Drawing.Point(141, 59);
            this.BlockItemPostMarkComboBox.Name = "BlockItemPostMarkComboBox";
            this.BlockItemPostMarkComboBox.Size = new System.Drawing.Size(202, 21);
            this.BlockItemPostMarkComboBox.TabIndex = 4;
            // 
            // BlockItemAddDataPanel
            // 
            this.BlockItemAddDataPanel.AutoSize = true;
            this.BlockItemAddDataPanel.ColumnCount = 2;
            this.BlockItemAddDataPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.BlockItemAddDataPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.BlockItemAddDataPanel.Controls.Add(this.BlockItemInheritorLabel, 0, 0);
            this.BlockItemAddDataPanel.Controls.Add(this.BlockItemInheritorTextBox, 1, 0);
            this.BlockItemAddDataPanel.Controls.Add(this.BlockItemCommentLabel, 0, 2);
            this.BlockItemAddDataPanel.Controls.Add(this.BlockItemCommentTextBox, 1, 2);
            this.BlockItemAddDataPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BlockItemAddDataPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.BlockItemAddDataPanel.Location = new System.Drawing.Point(355, 3);
            this.BlockItemAddDataPanel.Name = "BlockItemAddDataPanel";
            this.BlockItemAddDataPanel.RowCount = 3;
            this.BlockItemAddDataPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.BlockItemAddDataPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.BlockItemAddDataPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.BlockItemAddDataPanel.Size = new System.Drawing.Size(818, 84);
            this.BlockItemAddDataPanel.TabIndex = 3;
            // 
            // BlockItemInheritorLabel
            // 
            this.BlockItemInheritorLabel.AutoSize = true;
            this.BlockItemInheritorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BlockItemInheritorLabel.Location = new System.Drawing.Point(3, 0);
            this.BlockItemInheritorLabel.Name = "BlockItemInheritorLabel";
            this.BlockItemInheritorLabel.Size = new System.Drawing.Size(157, 28);
            this.BlockItemInheritorLabel.TabIndex = 4;
            this.BlockItemInheritorLabel.Text = "inher";
            this.BlockItemInheritorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BlockItemInheritorTextBox
            // 
            this.BlockItemInheritorTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BlockItemInheritorTextBox.Location = new System.Drawing.Point(166, 3);
            this.BlockItemInheritorTextBox.Name = "BlockItemInheritorTextBox";
            this.BlockItemInheritorTextBox.ReadOnly = true;
            this.BlockItemInheritorTextBox.Size = new System.Drawing.Size(649, 20);
            this.BlockItemInheritorTextBox.TabIndex = 5;
            // 
            // BlockItemCommentLabel
            // 
            this.BlockItemCommentLabel.AutoSize = true;
            this.BlockItemCommentLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BlockItemCommentLabel.Location = new System.Drawing.Point(3, 56);
            this.BlockItemCommentLabel.Name = "BlockItemCommentLabel";
            this.BlockItemCommentLabel.Size = new System.Drawing.Size(157, 28);
            this.BlockItemCommentLabel.TabIndex = 1;
            this.BlockItemCommentLabel.Text = "comment";
            this.BlockItemCommentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BlockItemCommentTextBox
            // 
            this.BlockItemCommentTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BlockItemCommentTextBox.Location = new System.Drawing.Point(166, 59);
            this.BlockItemCommentTextBox.Name = "BlockItemCommentTextBox";
            this.BlockItemCommentTextBox.Size = new System.Drawing.Size(649, 20);
            this.BlockItemCommentTextBox.TabIndex = 3;
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
            // MessagePanel
            // 
            this.MessagePanel.AutoSize = true;
            this.MessagePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.MessagePanel.Location = new System.Drawing.Point(0, 0);
            this.MessagePanel.Name = "MessagePanel";
            this.MessagePanel.Size = new System.Drawing.Size(1184, 0);
            this.MessagePanel.TabIndex = 1;
            // 
            // EcfItemEditingDialogNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this.ViewPanel);
            this.Controls.Add(this.MessagePanel);
            this.Controls.Add(this.ButtonPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EcfItemEditingDialogNew";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EcfItemEditingDialog";
            this.Activated += new System.EventHandler(this.EcfItemEditingDialog_Activated);
            this.ButtonPanel.ResumeLayout(false);
            this.ButtonPanel.PerformLayout();
            this.ViewPanel.ResumeLayout(false);
            this.CommentItemView.ResumeLayout(false);
            this.ParameterItemView.ResumeLayout(false);
            this.ParameterItemView.PerformLayout();
            this.ParameterItemViewPanel.ResumeLayout(false);
            this.ParameterItemViewPanel.PerformLayout();
            this.ParameterItemKeyPanel.ResumeLayout(false);
            this.ParameterItemKeyPanel.PerformLayout();
            this.ParameterItemInfoPanel.ResumeLayout(false);
            this.ParameterItemInfoPanel.PerformLayout();
            this.BlockItemView.ResumeLayout(false);
            this.BlockItemView.PerformLayout();
            this.BlockItemViewPanel.ResumeLayout(false);
            this.BlockItemViewPanel.PerformLayout();
            this.BlockItemTypePanel.ResumeLayout(false);
            this.BlockItemTypePanel.PerformLayout();
            this.BlockItemAddDataPanel.ResumeLayout(false);
            this.BlockItemAddDataPanel.PerformLayout();
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
        private System.Windows.Forms.FlowLayoutPanel MessagePanel;
        private System.Windows.Forms.TableLayoutPanel ParameterItemViewPanel;
        private System.Windows.Forms.TableLayoutPanel ParameterItemKeyPanel;
        private System.Windows.Forms.Label ParameterItemKeyLabel;
        private System.Windows.Forms.CheckBox ParameterItemIsOptionalCheckBox;
        private System.Windows.Forms.ComboBox ParameterItemKeyComboBox;
        private System.Windows.Forms.TableLayoutPanel ParameterItemInfoPanel;
        private System.Windows.Forms.Label ParameterItemInfoLabel;
        private System.Windows.Forms.Label ParameterItemCommentLabel;
        private System.Windows.Forms.TextBox ParameterItemInfoTextBox;
        private System.Windows.Forms.TextBox ParameterItemCommentTextBox;
        private System.Windows.Forms.TableLayoutPanel BlockItemViewPanel;
        private System.Windows.Forms.TableLayoutPanel BlockItemTypePanel;
        private System.Windows.Forms.TableLayoutPanel BlockItemAddDataPanel;
        private System.Windows.Forms.Label BlockItemDataTypeLabel;
        private System.Windows.Forms.Label BlockItemCommentLabel;
        private System.Windows.Forms.ComboBox BlockItemDataTypeComboBox;
        private System.Windows.Forms.TextBox BlockItemCommentTextBox;
        private System.Windows.Forms.Label BlockItemPreMarkLabel;
        private System.Windows.Forms.ComboBox BlockItemPreMarkComboBox;
        private System.Windows.Forms.Label BlockItemInheritorLabel;
        private System.Windows.Forms.TextBox BlockItemInheritorTextBox;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.Label BlockItemPostMarkLabel;
        private System.Windows.Forms.ComboBox BlockItemPostMarkComboBox;
        private System.Windows.Forms.Label ParameterItemParentLabel;
        private System.Windows.Forms.TextBox ParameterItemParentTextBox;
        private System.Windows.Forms.RichTextBox CommentItemRichTextBox;
        private System.Windows.Forms.TabPage ParameterMatrixView;
    }
}