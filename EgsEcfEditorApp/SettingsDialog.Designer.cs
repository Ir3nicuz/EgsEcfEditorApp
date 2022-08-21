
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
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("general");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("creation");
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("filter");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("sorter");
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("info");
            this.ButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.AbortButton = new System.Windows.Forms.Button();
            this.ResetButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.ChapterSelectorTreeView = new System.Windows.Forms.TreeView();
            this.SettingPanelsTabControl = new System.Windows.Forms.TabControl();
            this.GeneralTabPage = new System.Windows.Forms.TabPage();
            this.GeneralSettingsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.GameVersionFolderLabel = new System.Windows.Forms.Label();
            this.GameVersionFolderComboBox = new System.Windows.Forms.ComboBox();
            this.CreationTabPage = new System.Windows.Forms.TabPage();
            this.CreationPanel = new System.Windows.Forms.TableLayoutPanel();
            this.WriteOnlyValidItemsCheckBox = new System.Windows.Forms.CheckBox();
            this.InvalidateParentsOnErrorCheckBox = new System.Windows.Forms.CheckBox();
            this.WriteOnlyValidItemsLabel = new System.Windows.Forms.Label();
            this.InvalidateParentsOnErrorLabel = new System.Windows.Forms.Label();
            this.AllowFallbackToParsedDataLabel = new System.Windows.Forms.Label();
            this.AllowFallbackToParsedDataCheckBox = new System.Windows.Forms.CheckBox();
            this.FilterTabPage = new System.Windows.Forms.TabPage();
            this.FilterPanel = new System.Windows.Forms.TableLayoutPanel();
            this.TreeViewFilterCommentsInitActiveLabel = new System.Windows.Forms.Label();
            this.TreeViewFilterParametersInitActiveLabel = new System.Windows.Forms.Label();
            this.TreeViewFilterDataBlocksInitActiveLabel = new System.Windows.Forms.Label();
            this.TreeViewFilterCommentsInitActiveCheckBox = new System.Windows.Forms.CheckBox();
            this.TreeViewFilterParametersInitActiveCheckBox = new System.Windows.Forms.CheckBox();
            this.TreeViewFilterDataBlocksInitActiveCheckBox = new System.Windows.Forms.CheckBox();
            this.SorterTabPage = new System.Windows.Forms.TabPage();
            this.SorterPanel = new System.Windows.Forms.TableLayoutPanel();
            this.TreeViewSorterInitCountLabel = new System.Windows.Forms.Label();
            this.ParameterViewSorterInitCountLabel = new System.Windows.Forms.Label();
            this.ErrorViewSorterInitCountLabel = new System.Windows.Forms.Label();
            this.TreeViewSorterInitCountComboBox = new System.Windows.Forms.ComboBox();
            this.ParameterViewSorterInitCountComboBox = new System.Windows.Forms.ComboBox();
            this.ErrorViewSorterInitCountComboBox = new System.Windows.Forms.ComboBox();
            this.InfoTabPage = new System.Windows.Forms.TabPage();
            this.InfoPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ReadmeTitleLabel = new System.Windows.Forms.Label();
            this.LicenseTitleLabel = new System.Windows.Forms.Label();
            this.LogoPictureBox = new System.Windows.Forms.PictureBox();
            this.AuthorTitleLabel = new System.Windows.Forms.Label();
            this.VersionTitleLabel = new System.Windows.Forms.Label();
            this.VersionDataLabel = new System.Windows.Forms.Label();
            this.AuthorDataLabel = new System.Windows.Forms.Label();
            this.AppNameDataLabel = new System.Windows.Forms.Label();
            this.LicenseDataLinkLabel = new System.Windows.Forms.LinkLabel();
            this.ReadmeDataLinkLabel = new System.Windows.Forms.LinkLabel();
            this.SettingsBorderPanel = new System.Windows.Forms.Panel();
            this.ButtonPanel.SuspendLayout();
            this.SettingPanelsTabControl.SuspendLayout();
            this.GeneralTabPage.SuspendLayout();
            this.GeneralSettingsPanel.SuspendLayout();
            this.CreationTabPage.SuspendLayout();
            this.CreationPanel.SuspendLayout();
            this.FilterTabPage.SuspendLayout();
            this.FilterPanel.SuspendLayout();
            this.SorterTabPage.SuspendLayout();
            this.SorterPanel.SuspendLayout();
            this.InfoTabPage.SuspendLayout();
            this.InfoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).BeginInit();
            this.SettingsBorderPanel.SuspendLayout();
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
            treeNode16.Name = "GeneralNode";
            treeNode16.Text = "general";
            treeNode17.Name = "CreationNode";
            treeNode17.Text = "creation";
            treeNode18.Name = "FilterNode";
            treeNode18.Text = "filter";
            treeNode19.Name = "SorterNode";
            treeNode19.Text = "sorter";
            treeNode20.Name = "InfoNode";
            treeNode20.Text = "info";
            this.ChapterSelectorTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode16,
            treeNode17,
            treeNode18,
            treeNode19,
            treeNode20});
            this.ChapterSelectorTreeView.Size = new System.Drawing.Size(150, 364);
            this.ChapterSelectorTreeView.TabIndex = 1;
            this.ChapterSelectorTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ChapterSelectorTreeView_AfterSelect);
            // 
            // SettingPanelsTabControl
            // 
            this.SettingPanelsTabControl.Controls.Add(this.GeneralTabPage);
            this.SettingPanelsTabControl.Controls.Add(this.CreationTabPage);
            this.SettingPanelsTabControl.Controls.Add(this.FilterTabPage);
            this.SettingPanelsTabControl.Controls.Add(this.SorterTabPage);
            this.SettingPanelsTabControl.Controls.Add(this.InfoTabPage);
            this.SettingPanelsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SettingPanelsTabControl.Location = new System.Drawing.Point(0, 0);
            this.SettingPanelsTabControl.Name = "SettingPanelsTabControl";
            this.SettingPanelsTabControl.SelectedIndex = 0;
            this.SettingPanelsTabControl.Size = new System.Drawing.Size(560, 362);
            this.SettingPanelsTabControl.TabIndex = 2;
            // 
            // GeneralTabPage
            // 
            this.GeneralTabPage.Controls.Add(this.GeneralSettingsPanel);
            this.GeneralTabPage.Location = new System.Drawing.Point(4, 22);
            this.GeneralTabPage.Name = "GeneralTabPage";
            this.GeneralTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.GeneralTabPage.Size = new System.Drawing.Size(552, 336);
            this.GeneralTabPage.TabIndex = 0;
            this.GeneralTabPage.Text = "general";
            this.GeneralTabPage.UseVisualStyleBackColor = true;
            // 
            // GeneralSettingsPanel
            // 
            this.GeneralSettingsPanel.AutoSize = true;
            this.GeneralSettingsPanel.ColumnCount = 2;
            this.GeneralSettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.GeneralSettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.GeneralSettingsPanel.Controls.Add(this.GameVersionFolderLabel, 0, 0);
            this.GeneralSettingsPanel.Controls.Add(this.GameVersionFolderComboBox, 1, 0);
            this.GeneralSettingsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GeneralSettingsPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.GeneralSettingsPanel.Location = new System.Drawing.Point(3, 3);
            this.GeneralSettingsPanel.Name = "GeneralSettingsPanel";
            this.GeneralSettingsPanel.RowCount = 2;
            this.GeneralSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.GeneralSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.GeneralSettingsPanel.Size = new System.Drawing.Size(546, 330);
            this.GeneralSettingsPanel.TabIndex = 0;
            // 
            // GameVersionFolderLabel
            // 
            this.GameVersionFolderLabel.AutoSize = true;
            this.GameVersionFolderLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GameVersionFolderLabel.Location = new System.Drawing.Point(3, 0);
            this.GameVersionFolderLabel.Name = "GameVersionFolderLabel";
            this.GameVersionFolderLabel.Size = new System.Drawing.Size(267, 27);
            this.GameVersionFolderLabel.TabIndex = 0;
            this.GameVersionFolderLabel.Text = "label1";
            this.GameVersionFolderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GameVersionFolderComboBox
            // 
            this.GameVersionFolderComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GameVersionFolderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GameVersionFolderComboBox.FormattingEnabled = true;
            this.GameVersionFolderComboBox.Location = new System.Drawing.Point(276, 3);
            this.GameVersionFolderComboBox.Name = "GameVersionFolderComboBox";
            this.GameVersionFolderComboBox.Size = new System.Drawing.Size(267, 21);
            this.GameVersionFolderComboBox.Sorted = true;
            this.GameVersionFolderComboBox.TabIndex = 1;
            this.GameVersionFolderComboBox.SelectionChangeCommitted += new System.EventHandler(this.GameVersionFolderComboBox_SelectionChangeCommitted);
            // 
            // CreationTabPage
            // 
            this.CreationTabPage.Controls.Add(this.CreationPanel);
            this.CreationTabPage.Location = new System.Drawing.Point(4, 22);
            this.CreationTabPage.Name = "CreationTabPage";
            this.CreationTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.CreationTabPage.Size = new System.Drawing.Size(552, 336);
            this.CreationTabPage.TabIndex = 1;
            this.CreationTabPage.Text = "creation";
            this.CreationTabPage.UseVisualStyleBackColor = true;
            // 
            // CreationPanel
            // 
            this.CreationPanel.AutoSize = true;
            this.CreationPanel.ColumnCount = 2;
            this.CreationPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.CreationPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.CreationPanel.Controls.Add(this.WriteOnlyValidItemsCheckBox, 1, 0);
            this.CreationPanel.Controls.Add(this.InvalidateParentsOnErrorCheckBox, 1, 1);
            this.CreationPanel.Controls.Add(this.WriteOnlyValidItemsLabel, 0, 0);
            this.CreationPanel.Controls.Add(this.InvalidateParentsOnErrorLabel, 0, 1);
            this.CreationPanel.Controls.Add(this.AllowFallbackToParsedDataLabel, 0, 2);
            this.CreationPanel.Controls.Add(this.AllowFallbackToParsedDataCheckBox, 1, 2);
            this.CreationPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CreationPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.CreationPanel.Location = new System.Drawing.Point(3, 3);
            this.CreationPanel.Name = "CreationPanel";
            this.CreationPanel.RowCount = 4;
            this.CreationPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.CreationPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.CreationPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.CreationPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.CreationPanel.Size = new System.Drawing.Size(546, 330);
            this.CreationPanel.TabIndex = 1;
            // 
            // WriteOnlyValidItemsCheckBox
            // 
            this.WriteOnlyValidItemsCheckBox.AutoSize = true;
            this.WriteOnlyValidItemsCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WriteOnlyValidItemsCheckBox.Location = new System.Drawing.Point(276, 3);
            this.WriteOnlyValidItemsCheckBox.Name = "WriteOnlyValidItemsCheckBox";
            this.WriteOnlyValidItemsCheckBox.Size = new System.Drawing.Size(267, 14);
            this.WriteOnlyValidItemsCheckBox.TabIndex = 0;
            this.WriteOnlyValidItemsCheckBox.UseVisualStyleBackColor = true;
            this.WriteOnlyValidItemsCheckBox.Click += new System.EventHandler(this.WriteOnlyValidItemsCheckBox_Click);
            // 
            // InvalidateParentsOnErrorCheckBox
            // 
            this.InvalidateParentsOnErrorCheckBox.AutoSize = true;
            this.InvalidateParentsOnErrorCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InvalidateParentsOnErrorCheckBox.Location = new System.Drawing.Point(276, 23);
            this.InvalidateParentsOnErrorCheckBox.Name = "InvalidateParentsOnErrorCheckBox";
            this.InvalidateParentsOnErrorCheckBox.Size = new System.Drawing.Size(267, 14);
            this.InvalidateParentsOnErrorCheckBox.TabIndex = 1;
            this.InvalidateParentsOnErrorCheckBox.UseVisualStyleBackColor = true;
            this.InvalidateParentsOnErrorCheckBox.Click += new System.EventHandler(this.InvalidateParentsOnErrorCheckBox_Click);
            // 
            // WriteOnlyValidItemsLabel
            // 
            this.WriteOnlyValidItemsLabel.AutoSize = true;
            this.WriteOnlyValidItemsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WriteOnlyValidItemsLabel.Location = new System.Drawing.Point(3, 0);
            this.WriteOnlyValidItemsLabel.Name = "WriteOnlyValidItemsLabel";
            this.WriteOnlyValidItemsLabel.Size = new System.Drawing.Size(267, 20);
            this.WriteOnlyValidItemsLabel.TabIndex = 2;
            this.WriteOnlyValidItemsLabel.Text = "writeonlyvalid";
            this.WriteOnlyValidItemsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // InvalidateParentsOnErrorLabel
            // 
            this.InvalidateParentsOnErrorLabel.AutoSize = true;
            this.InvalidateParentsOnErrorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InvalidateParentsOnErrorLabel.Location = new System.Drawing.Point(3, 20);
            this.InvalidateParentsOnErrorLabel.Name = "InvalidateParentsOnErrorLabel";
            this.InvalidateParentsOnErrorLabel.Size = new System.Drawing.Size(267, 20);
            this.InvalidateParentsOnErrorLabel.TabIndex = 3;
            this.InvalidateParentsOnErrorLabel.Text = "invalidate parent";
            this.InvalidateParentsOnErrorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AllowFallbackToParsedDataLabel
            // 
            this.AllowFallbackToParsedDataLabel.AutoSize = true;
            this.AllowFallbackToParsedDataLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AllowFallbackToParsedDataLabel.Location = new System.Drawing.Point(3, 40);
            this.AllowFallbackToParsedDataLabel.Name = "AllowFallbackToParsedDataLabel";
            this.AllowFallbackToParsedDataLabel.Size = new System.Drawing.Size(267, 20);
            this.AllowFallbackToParsedDataLabel.TabIndex = 4;
            this.AllowFallbackToParsedDataLabel.Text = "allow falback";
            this.AllowFallbackToParsedDataLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AllowFallbackToParsedDataCheckBox
            // 
            this.AllowFallbackToParsedDataCheckBox.AutoSize = true;
            this.AllowFallbackToParsedDataCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AllowFallbackToParsedDataCheckBox.Location = new System.Drawing.Point(276, 43);
            this.AllowFallbackToParsedDataCheckBox.Name = "AllowFallbackToParsedDataCheckBox";
            this.AllowFallbackToParsedDataCheckBox.Size = new System.Drawing.Size(267, 14);
            this.AllowFallbackToParsedDataCheckBox.TabIndex = 5;
            this.AllowFallbackToParsedDataCheckBox.UseVisualStyleBackColor = true;
            this.AllowFallbackToParsedDataCheckBox.Click += new System.EventHandler(this.AllowFallbackToParsedDataCheckBox_Click);
            // 
            // FilterTabPage
            // 
            this.FilterTabPage.Controls.Add(this.FilterPanel);
            this.FilterTabPage.Location = new System.Drawing.Point(4, 22);
            this.FilterTabPage.Name = "FilterTabPage";
            this.FilterTabPage.Size = new System.Drawing.Size(552, 336);
            this.FilterTabPage.TabIndex = 2;
            this.FilterTabPage.Text = "filter";
            this.FilterTabPage.UseVisualStyleBackColor = true;
            // 
            // FilterPanel
            // 
            this.FilterPanel.AutoSize = true;
            this.FilterPanel.ColumnCount = 2;
            this.FilterPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.FilterPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.FilterPanel.Controls.Add(this.TreeViewFilterCommentsInitActiveLabel, 0, 0);
            this.FilterPanel.Controls.Add(this.TreeViewFilterParametersInitActiveLabel, 0, 1);
            this.FilterPanel.Controls.Add(this.TreeViewFilterDataBlocksInitActiveLabel, 0, 2);
            this.FilterPanel.Controls.Add(this.TreeViewFilterCommentsInitActiveCheckBox, 1, 0);
            this.FilterPanel.Controls.Add(this.TreeViewFilterParametersInitActiveCheckBox, 1, 1);
            this.FilterPanel.Controls.Add(this.TreeViewFilterDataBlocksInitActiveCheckBox, 1, 2);
            this.FilterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilterPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.FilterPanel.Location = new System.Drawing.Point(0, 0);
            this.FilterPanel.Name = "FilterPanel";
            this.FilterPanel.RowCount = 4;
            this.FilterPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.FilterPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.FilterPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.FilterPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.FilterPanel.Size = new System.Drawing.Size(552, 336);
            this.FilterPanel.TabIndex = 0;
            // 
            // TreeViewFilterCommentsInitActiveLabel
            // 
            this.TreeViewFilterCommentsInitActiveLabel.AutoSize = true;
            this.TreeViewFilterCommentsInitActiveLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeViewFilterCommentsInitActiveLabel.Location = new System.Drawing.Point(3, 0);
            this.TreeViewFilterCommentsInitActiveLabel.Name = "TreeViewFilterCommentsInitActiveLabel";
            this.TreeViewFilterCommentsInitActiveLabel.Size = new System.Drawing.Size(270, 20);
            this.TreeViewFilterCommentsInitActiveLabel.TabIndex = 0;
            this.TreeViewFilterCommentsInitActiveLabel.Text = "comments filter active";
            this.TreeViewFilterCommentsInitActiveLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TreeViewFilterParametersInitActiveLabel
            // 
            this.TreeViewFilterParametersInitActiveLabel.AutoSize = true;
            this.TreeViewFilterParametersInitActiveLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeViewFilterParametersInitActiveLabel.Location = new System.Drawing.Point(3, 20);
            this.TreeViewFilterParametersInitActiveLabel.Name = "TreeViewFilterParametersInitActiveLabel";
            this.TreeViewFilterParametersInitActiveLabel.Size = new System.Drawing.Size(270, 20);
            this.TreeViewFilterParametersInitActiveLabel.TabIndex = 1;
            this.TreeViewFilterParametersInitActiveLabel.Text = "parameter filter active";
            this.TreeViewFilterParametersInitActiveLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TreeViewFilterDataBlocksInitActiveLabel
            // 
            this.TreeViewFilterDataBlocksInitActiveLabel.AutoSize = true;
            this.TreeViewFilterDataBlocksInitActiveLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeViewFilterDataBlocksInitActiveLabel.Location = new System.Drawing.Point(3, 40);
            this.TreeViewFilterDataBlocksInitActiveLabel.Name = "TreeViewFilterDataBlocksInitActiveLabel";
            this.TreeViewFilterDataBlocksInitActiveLabel.Size = new System.Drawing.Size(270, 20);
            this.TreeViewFilterDataBlocksInitActiveLabel.TabIndex = 2;
            this.TreeViewFilterDataBlocksInitActiveLabel.Text = "datablocks filter active";
            this.TreeViewFilterDataBlocksInitActiveLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TreeViewFilterCommentsInitActiveCheckBox
            // 
            this.TreeViewFilterCommentsInitActiveCheckBox.AutoSize = true;
            this.TreeViewFilterCommentsInitActiveCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeViewFilterCommentsInitActiveCheckBox.Location = new System.Drawing.Point(279, 3);
            this.TreeViewFilterCommentsInitActiveCheckBox.Name = "TreeViewFilterCommentsInitActiveCheckBox";
            this.TreeViewFilterCommentsInitActiveCheckBox.Size = new System.Drawing.Size(270, 14);
            this.TreeViewFilterCommentsInitActiveCheckBox.TabIndex = 3;
            this.TreeViewFilterCommentsInitActiveCheckBox.UseVisualStyleBackColor = true;
            this.TreeViewFilterCommentsInitActiveCheckBox.Click += new System.EventHandler(this.TreeViewFilterCommentsInitActiveCheckBox_Click);
            // 
            // TreeViewFilterParametersInitActiveCheckBox
            // 
            this.TreeViewFilterParametersInitActiveCheckBox.AutoSize = true;
            this.TreeViewFilterParametersInitActiveCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeViewFilterParametersInitActiveCheckBox.Location = new System.Drawing.Point(279, 23);
            this.TreeViewFilterParametersInitActiveCheckBox.Name = "TreeViewFilterParametersInitActiveCheckBox";
            this.TreeViewFilterParametersInitActiveCheckBox.Size = new System.Drawing.Size(270, 14);
            this.TreeViewFilterParametersInitActiveCheckBox.TabIndex = 4;
            this.TreeViewFilterParametersInitActiveCheckBox.UseVisualStyleBackColor = true;
            this.TreeViewFilterParametersInitActiveCheckBox.Click += new System.EventHandler(this.TreeViewFilterParametersInitActiveCheckBox_Click);
            // 
            // TreeViewFilterDataBlocksInitActiveCheckBox
            // 
            this.TreeViewFilterDataBlocksInitActiveCheckBox.AutoSize = true;
            this.TreeViewFilterDataBlocksInitActiveCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeViewFilterDataBlocksInitActiveCheckBox.Location = new System.Drawing.Point(279, 43);
            this.TreeViewFilterDataBlocksInitActiveCheckBox.Name = "TreeViewFilterDataBlocksInitActiveCheckBox";
            this.TreeViewFilterDataBlocksInitActiveCheckBox.Size = new System.Drawing.Size(270, 14);
            this.TreeViewFilterDataBlocksInitActiveCheckBox.TabIndex = 5;
            this.TreeViewFilterDataBlocksInitActiveCheckBox.UseVisualStyleBackColor = true;
            this.TreeViewFilterDataBlocksInitActiveCheckBox.Click += new System.EventHandler(this.TreeViewFilterDataBlocksInitActiveCheckBox_Click);
            // 
            // SorterTabPage
            // 
            this.SorterTabPage.Controls.Add(this.SorterPanel);
            this.SorterTabPage.Location = new System.Drawing.Point(4, 22);
            this.SorterTabPage.Name = "SorterTabPage";
            this.SorterTabPage.Size = new System.Drawing.Size(552, 336);
            this.SorterTabPage.TabIndex = 3;
            this.SorterTabPage.Text = "sorter";
            this.SorterTabPage.UseVisualStyleBackColor = true;
            // 
            // SorterPanel
            // 
            this.SorterPanel.AutoSize = true;
            this.SorterPanel.ColumnCount = 2;
            this.SorterPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SorterPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SorterPanel.Controls.Add(this.TreeViewSorterInitCountLabel, 0, 0);
            this.SorterPanel.Controls.Add(this.ParameterViewSorterInitCountLabel, 0, 1);
            this.SorterPanel.Controls.Add(this.ErrorViewSorterInitCountLabel, 0, 2);
            this.SorterPanel.Controls.Add(this.TreeViewSorterInitCountComboBox, 1, 0);
            this.SorterPanel.Controls.Add(this.ParameterViewSorterInitCountComboBox, 1, 1);
            this.SorterPanel.Controls.Add(this.ErrorViewSorterInitCountComboBox, 1, 2);
            this.SorterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SorterPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.SorterPanel.Location = new System.Drawing.Point(0, 0);
            this.SorterPanel.Name = "SorterPanel";
            this.SorterPanel.RowCount = 4;
            this.SorterPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SorterPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SorterPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SorterPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SorterPanel.Size = new System.Drawing.Size(552, 336);
            this.SorterPanel.TabIndex = 0;
            // 
            // TreeViewSorterInitCountLabel
            // 
            this.TreeViewSorterInitCountLabel.AutoSize = true;
            this.TreeViewSorterInitCountLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeViewSorterInitCountLabel.Location = new System.Drawing.Point(3, 0);
            this.TreeViewSorterInitCountLabel.Name = "TreeViewSorterInitCountLabel";
            this.TreeViewSorterInitCountLabel.Size = new System.Drawing.Size(270, 27);
            this.TreeViewSorterInitCountLabel.TabIndex = 0;
            this.TreeViewSorterInitCountLabel.Text = "tree sorter init count";
            this.TreeViewSorterInitCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ParameterViewSorterInitCountLabel
            // 
            this.ParameterViewSorterInitCountLabel.AutoSize = true;
            this.ParameterViewSorterInitCountLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ParameterViewSorterInitCountLabel.Location = new System.Drawing.Point(3, 27);
            this.ParameterViewSorterInitCountLabel.Name = "ParameterViewSorterInitCountLabel";
            this.ParameterViewSorterInitCountLabel.Size = new System.Drawing.Size(270, 27);
            this.ParameterViewSorterInitCountLabel.TabIndex = 1;
            this.ParameterViewSorterInitCountLabel.Text = "parameter sorter init count";
            this.ParameterViewSorterInitCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ErrorViewSorterInitCountLabel
            // 
            this.ErrorViewSorterInitCountLabel.AutoSize = true;
            this.ErrorViewSorterInitCountLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ErrorViewSorterInitCountLabel.Location = new System.Drawing.Point(3, 54);
            this.ErrorViewSorterInitCountLabel.Name = "ErrorViewSorterInitCountLabel";
            this.ErrorViewSorterInitCountLabel.Size = new System.Drawing.Size(270, 27);
            this.ErrorViewSorterInitCountLabel.TabIndex = 2;
            this.ErrorViewSorterInitCountLabel.Text = "error sorter init count";
            this.ErrorViewSorterInitCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TreeViewSorterInitCountComboBox
            // 
            this.TreeViewSorterInitCountComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeViewSorterInitCountComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TreeViewSorterInitCountComboBox.FormattingEnabled = true;
            this.TreeViewSorterInitCountComboBox.Location = new System.Drawing.Point(279, 3);
            this.TreeViewSorterInitCountComboBox.Name = "TreeViewSorterInitCountComboBox";
            this.TreeViewSorterInitCountComboBox.Size = new System.Drawing.Size(270, 21);
            this.TreeViewSorterInitCountComboBox.TabIndex = 3;
            this.TreeViewSorterInitCountComboBox.SelectionChangeCommitted += new System.EventHandler(this.TreeViewSorterInitCountComboBox_SelectionChangeCommitted);
            // 
            // ParameterViewSorterInitCountComboBox
            // 
            this.ParameterViewSorterInitCountComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ParameterViewSorterInitCountComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ParameterViewSorterInitCountComboBox.FormattingEnabled = true;
            this.ParameterViewSorterInitCountComboBox.Location = new System.Drawing.Point(279, 30);
            this.ParameterViewSorterInitCountComboBox.Name = "ParameterViewSorterInitCountComboBox";
            this.ParameterViewSorterInitCountComboBox.Size = new System.Drawing.Size(270, 21);
            this.ParameterViewSorterInitCountComboBox.TabIndex = 4;
            this.ParameterViewSorterInitCountComboBox.SelectionChangeCommitted += new System.EventHandler(this.ParameterViewSorterInitCountComboBox_SelectionChangeCommitted);
            // 
            // ErrorViewSorterInitCountComboBox
            // 
            this.ErrorViewSorterInitCountComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ErrorViewSorterInitCountComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ErrorViewSorterInitCountComboBox.FormattingEnabled = true;
            this.ErrorViewSorterInitCountComboBox.Location = new System.Drawing.Point(279, 57);
            this.ErrorViewSorterInitCountComboBox.Name = "ErrorViewSorterInitCountComboBox";
            this.ErrorViewSorterInitCountComboBox.Size = new System.Drawing.Size(270, 21);
            this.ErrorViewSorterInitCountComboBox.TabIndex = 5;
            this.ErrorViewSorterInitCountComboBox.SelectionChangeCommitted += new System.EventHandler(this.ErrorViewSorterInitCountComboBox_SelectionChangeCommitted);
            // 
            // InfoTabPage
            // 
            this.InfoTabPage.Controls.Add(this.InfoPanel);
            this.InfoTabPage.Location = new System.Drawing.Point(4, 22);
            this.InfoTabPage.Name = "InfoTabPage";
            this.InfoTabPage.Size = new System.Drawing.Size(552, 336);
            this.InfoTabPage.TabIndex = 4;
            this.InfoTabPage.Text = "info";
            this.InfoTabPage.UseVisualStyleBackColor = true;
            // 
            // InfoPanel
            // 
            this.InfoPanel.AutoSize = true;
            this.InfoPanel.ColumnCount = 2;
            this.InfoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.InfoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.InfoPanel.Controls.Add(this.ReadmeTitleLabel, 0, 5);
            this.InfoPanel.Controls.Add(this.LicenseTitleLabel, 0, 4);
            this.InfoPanel.Controls.Add(this.LogoPictureBox, 0, 1);
            this.InfoPanel.Controls.Add(this.AuthorTitleLabel, 0, 2);
            this.InfoPanel.Controls.Add(this.VersionTitleLabel, 0, 3);
            this.InfoPanel.Controls.Add(this.VersionDataLabel, 1, 3);
            this.InfoPanel.Controls.Add(this.AuthorDataLabel, 1, 2);
            this.InfoPanel.Controls.Add(this.AppNameDataLabel, 0, 0);
            this.InfoPanel.Controls.Add(this.LicenseDataLinkLabel, 1, 4);
            this.InfoPanel.Controls.Add(this.ReadmeDataLinkLabel, 1, 5);
            this.InfoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InfoPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.InfoPanel.Location = new System.Drawing.Point(0, 0);
            this.InfoPanel.Name = "InfoPanel";
            this.InfoPanel.RowCount = 6;
            this.InfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.InfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.InfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.InfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.InfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.InfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.InfoPanel.Size = new System.Drawing.Size(552, 336);
            this.InfoPanel.TabIndex = 1;
            // 
            // ReadmeTitleLabel
            // 
            this.ReadmeTitleLabel.AutoSize = true;
            this.ReadmeTitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReadmeTitleLabel.Location = new System.Drawing.Point(3, 317);
            this.ReadmeTitleLabel.Name = "ReadmeTitleLabel";
            this.ReadmeTitleLabel.Padding = new System.Windows.Forms.Padding(3);
            this.ReadmeTitleLabel.Size = new System.Drawing.Size(270, 19);
            this.ReadmeTitleLabel.TabIndex = 11;
            this.ReadmeTitleLabel.Text = "readme";
            this.ReadmeTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LicenseTitleLabel
            // 
            this.LicenseTitleLabel.AutoSize = true;
            this.LicenseTitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LicenseTitleLabel.Location = new System.Drawing.Point(3, 298);
            this.LicenseTitleLabel.Name = "LicenseTitleLabel";
            this.LicenseTitleLabel.Padding = new System.Windows.Forms.Padding(3);
            this.LicenseTitleLabel.Size = new System.Drawing.Size(270, 19);
            this.LicenseTitleLabel.TabIndex = 9;
            this.LicenseTitleLabel.Text = "license";
            this.LicenseTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LogoPictureBox
            // 
            this.InfoPanel.SetColumnSpan(this.LogoPictureBox, 2);
            this.LogoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogoPictureBox.Location = new System.Drawing.Point(3, 57);
            this.LogoPictureBox.Name = "LogoPictureBox";
            this.LogoPictureBox.Size = new System.Drawing.Size(546, 200);
            this.LogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.LogoPictureBox.TabIndex = 0;
            this.LogoPictureBox.TabStop = false;
            // 
            // AuthorTitleLabel
            // 
            this.AuthorTitleLabel.AutoSize = true;
            this.AuthorTitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AuthorTitleLabel.Location = new System.Drawing.Point(3, 260);
            this.AuthorTitleLabel.Name = "AuthorTitleLabel";
            this.AuthorTitleLabel.Padding = new System.Windows.Forms.Padding(3);
            this.AuthorTitleLabel.Size = new System.Drawing.Size(270, 19);
            this.AuthorTitleLabel.TabIndex = 4;
            this.AuthorTitleLabel.Text = "author";
            this.AuthorTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // VersionTitleLabel
            // 
            this.VersionTitleLabel.AutoSize = true;
            this.VersionTitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VersionTitleLabel.Location = new System.Drawing.Point(3, 279);
            this.VersionTitleLabel.Name = "VersionTitleLabel";
            this.VersionTitleLabel.Padding = new System.Windows.Forms.Padding(3);
            this.VersionTitleLabel.Size = new System.Drawing.Size(270, 19);
            this.VersionTitleLabel.TabIndex = 5;
            this.VersionTitleLabel.Text = "vers";
            this.VersionTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // VersionDataLabel
            // 
            this.VersionDataLabel.AutoSize = true;
            this.VersionDataLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VersionDataLabel.Location = new System.Drawing.Point(279, 279);
            this.VersionDataLabel.Name = "VersionDataLabel";
            this.VersionDataLabel.Padding = new System.Windows.Forms.Padding(3);
            this.VersionDataLabel.Size = new System.Drawing.Size(270, 19);
            this.VersionDataLabel.TabIndex = 6;
            this.VersionDataLabel.Text = "vers";
            this.VersionDataLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AuthorDataLabel
            // 
            this.AuthorDataLabel.AutoSize = true;
            this.AuthorDataLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AuthorDataLabel.Location = new System.Drawing.Point(279, 260);
            this.AuthorDataLabel.Name = "AuthorDataLabel";
            this.AuthorDataLabel.Padding = new System.Windows.Forms.Padding(3);
            this.AuthorDataLabel.Size = new System.Drawing.Size(270, 19);
            this.AuthorDataLabel.TabIndex = 7;
            this.AuthorDataLabel.Text = "auth";
            this.AuthorDataLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AppNameDataLabel
            // 
            this.AppNameDataLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AppNameDataLabel.AutoSize = true;
            this.InfoPanel.SetColumnSpan(this.AppNameDataLabel, 2);
            this.AppNameDataLabel.Font = new System.Drawing.Font("Viner Hand ITC", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AppNameDataLabel.Location = new System.Drawing.Point(3, 0);
            this.AppNameDataLabel.Name = "AppNameDataLabel";
            this.AppNameDataLabel.Padding = new System.Windows.Forms.Padding(5);
            this.AppNameDataLabel.Size = new System.Drawing.Size(546, 54);
            this.AppNameDataLabel.TabIndex = 8;
            this.AppNameDataLabel.Text = "name";
            this.AppNameDataLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LicenseDataLinkLabel
            // 
            this.LicenseDataLinkLabel.AutoSize = true;
            this.LicenseDataLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LicenseDataLinkLabel.Location = new System.Drawing.Point(279, 298);
            this.LicenseDataLinkLabel.Name = "LicenseDataLinkLabel";
            this.LicenseDataLinkLabel.Padding = new System.Windows.Forms.Padding(3);
            this.LicenseDataLinkLabel.Size = new System.Drawing.Size(270, 19);
            this.LicenseDataLinkLabel.TabIndex = 12;
            this.LicenseDataLinkLabel.TabStop = true;
            this.LicenseDataLinkLabel.Text = "license";
            this.LicenseDataLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LicenseDataLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LicenseDataLinkLabel_LinkClicked);
            // 
            // ReadmeDataLinkLabel
            // 
            this.ReadmeDataLinkLabel.AutoSize = true;
            this.ReadmeDataLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReadmeDataLinkLabel.Location = new System.Drawing.Point(279, 317);
            this.ReadmeDataLinkLabel.Name = "ReadmeDataLinkLabel";
            this.ReadmeDataLinkLabel.Padding = new System.Windows.Forms.Padding(3);
            this.ReadmeDataLinkLabel.Size = new System.Drawing.Size(270, 19);
            this.ReadmeDataLinkLabel.TabIndex = 13;
            this.ReadmeDataLinkLabel.TabStop = true;
            this.ReadmeDataLinkLabel.Text = "readme";
            this.ReadmeDataLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ReadmeDataLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ReadmeDataLinkLabel_LinkClicked);
            // 
            // SettingsBorderPanel
            // 
            this.SettingsBorderPanel.AutoSize = true;
            this.SettingsBorderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SettingsBorderPanel.Controls.Add(this.SettingPanelsTabControl);
            this.SettingsBorderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SettingsBorderPanel.Location = new System.Drawing.Point(150, 0);
            this.SettingsBorderPanel.Name = "SettingsBorderPanel";
            this.SettingsBorderPanel.Size = new System.Drawing.Size(562, 364);
            this.SettingsBorderPanel.TabIndex = 3;
            // 
            // EcfSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 393);
            this.Controls.Add(this.SettingsBorderPanel);
            this.Controls.Add(this.ChapterSelectorTreeView);
            this.Controls.Add(this.ButtonPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EcfSettingsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EcfSettingsDialog";
            this.Activated += new System.EventHandler(this.EcfSettingsDialog_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EcfSettingsDialog_FormClosing);
            this.ButtonPanel.ResumeLayout(false);
            this.ButtonPanel.PerformLayout();
            this.SettingPanelsTabControl.ResumeLayout(false);
            this.GeneralTabPage.ResumeLayout(false);
            this.GeneralTabPage.PerformLayout();
            this.GeneralSettingsPanel.ResumeLayout(false);
            this.GeneralSettingsPanel.PerformLayout();
            this.CreationTabPage.ResumeLayout(false);
            this.CreationTabPage.PerformLayout();
            this.CreationPanel.ResumeLayout(false);
            this.CreationPanel.PerformLayout();
            this.FilterTabPage.ResumeLayout(false);
            this.FilterTabPage.PerformLayout();
            this.FilterPanel.ResumeLayout(false);
            this.FilterPanel.PerformLayout();
            this.SorterTabPage.ResumeLayout(false);
            this.SorterTabPage.PerformLayout();
            this.SorterPanel.ResumeLayout(false);
            this.SorterPanel.PerformLayout();
            this.InfoTabPage.ResumeLayout(false);
            this.InfoTabPage.PerformLayout();
            this.InfoPanel.ResumeLayout(false);
            this.InfoPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).EndInit();
            this.SettingsBorderPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel ButtonPanel;
        private System.Windows.Forms.Button AbortButton;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.TreeView ChapterSelectorTreeView;
        private System.Windows.Forms.TabControl SettingPanelsTabControl;
        private System.Windows.Forms.TabPage GeneralTabPage;
        private System.Windows.Forms.TabPage CreationTabPage;
        private System.Windows.Forms.TabPage FilterTabPage;
        private System.Windows.Forms.TabPage SorterTabPage;
        private System.Windows.Forms.Panel SettingsBorderPanel;
        private System.Windows.Forms.TableLayoutPanel GeneralSettingsPanel;
        private System.Windows.Forms.Label GameVersionFolderLabel;
        private System.Windows.Forms.ComboBox GameVersionFolderComboBox;
        private System.Windows.Forms.TableLayoutPanel CreationPanel;
        private System.Windows.Forms.CheckBox WriteOnlyValidItemsCheckBox;
        private System.Windows.Forms.CheckBox InvalidateParentsOnErrorCheckBox;
        private System.Windows.Forms.Label WriteOnlyValidItemsLabel;
        private System.Windows.Forms.Label InvalidateParentsOnErrorLabel;
        private System.Windows.Forms.TableLayoutPanel FilterPanel;
        private System.Windows.Forms.Label TreeViewFilterCommentsInitActiveLabel;
        private System.Windows.Forms.Label TreeViewFilterParametersInitActiveLabel;
        private System.Windows.Forms.Label TreeViewFilterDataBlocksInitActiveLabel;
        private System.Windows.Forms.CheckBox TreeViewFilterCommentsInitActiveCheckBox;
        private System.Windows.Forms.CheckBox TreeViewFilterParametersInitActiveCheckBox;
        private System.Windows.Forms.CheckBox TreeViewFilterDataBlocksInitActiveCheckBox;
        private System.Windows.Forms.TableLayoutPanel SorterPanel;
        private System.Windows.Forms.Label TreeViewSorterInitCountLabel;
        private System.Windows.Forms.Label ParameterViewSorterInitCountLabel;
        private System.Windows.Forms.Label ErrorViewSorterInitCountLabel;
        private System.Windows.Forms.ComboBox TreeViewSorterInitCountComboBox;
        private System.Windows.Forms.ComboBox ParameterViewSorterInitCountComboBox;
        private System.Windows.Forms.ComboBox ErrorViewSorterInitCountComboBox;
        private System.Windows.Forms.Label AllowFallbackToParsedDataLabel;
        private System.Windows.Forms.CheckBox AllowFallbackToParsedDataCheckBox;
        private System.Windows.Forms.TabPage InfoTabPage;
        private System.Windows.Forms.PictureBox LogoPictureBox;
        private System.Windows.Forms.TableLayoutPanel InfoPanel;
        private System.Windows.Forms.Label AuthorTitleLabel;
        private System.Windows.Forms.Label VersionTitleLabel;
        private System.Windows.Forms.Label VersionDataLabel;
        private System.Windows.Forms.Label AuthorDataLabel;
        private System.Windows.Forms.Label AppNameDataLabel;
        private System.Windows.Forms.Label ReadmeTitleLabel;
        private System.Windows.Forms.Label LicenseTitleLabel;
        private System.Windows.Forms.LinkLabel LicenseDataLinkLabel;
        private System.Windows.Forms.LinkLabel ReadmeDataLinkLabel;
    }
}