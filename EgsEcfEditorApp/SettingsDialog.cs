using EgsEcfEditorApp.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static EcfFileViewTools.EcfSorter;
using static EgsEcfParser.EcfDefinitionHandling;

namespace EgsEcfEditorApp
{
    public partial class SettingsDialog : Form
    {
        public bool HasUnsavedData { get; private set; } = false;

        public SettingsDialog()
        {
            InitializeComponent();
            InitForm();
        }

        // events
        private void InitForm()
        {
            Icon = IconRecources.Icon_AppBranding;
            Text = TitleRecources.EcfSettingsDialog_Header;

            // Hack to hide tabs
            SettingPanelsTabControl.SizeMode = TabSizeMode.Fixed;
            SettingPanelsTabControl.ItemSize = new Size(0, 1);

            SaveButton.Text = TitleRecources.Generic_Save;
            ResetButton.Text = TitleRecources.Generic_Reset;
            AbortButton.Text = TitleRecources.Generic_Abort;

            InitPanels();

            ChapterSelectorTreeView.SelectedNode = ChapterSelectorTreeView.Nodes[0];
            SwitchPanel(0);
        }
        private void EcfSettingsDialog_FormClosing(object sender, FormClosingEventArgs evt)
        {
            if (HasUnsavedData)
            {
                if (MessageBox.Show(this, TextRecources.EcfSettingsDialog_UnsavedReallyCloseQuestion, 
                    TitleRecources.Generic_Attention, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    UserSettings.Default.Reload();
                    HasUnsavedData = false;
                }
                else
                {
                    evt.Cancel = true;
                }
            }
        }
        private void AbortButton_Click(object sender, EventArgs evt)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }
        private void ResetButton_Click(object sender, EventArgs evt)
        {
            if (MessageBox.Show(this, TextRecources.EcfSettingsDialog_ReallyResetAllQuestion, 
                TitleRecources.Generic_Attention, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                UserSettings.Default.Reset();
                PresetPanels();
                HasUnsavedData = false;
            }
        }
        private void SaveButton_Click(object sender, EventArgs evt)
        {
            if (HasUnsavedData)
            {
                UserSettings.Default.Save();
                HasUnsavedData = false;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
        private void ChapterSelectorTreeView_AfterSelect(object sender, TreeViewEventArgs evt)
        {
            SwitchPanel(evt.Node.Index);
        }
        private void EcfSettingsDialog_Activated(object sender, EventArgs evt)
        {
            PreparePanels();
            PresetPanels();
            ChapterSelectorTreeView.Focus();
        }

        // Panel events
        private void GameVersionFolderComboBox_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            UserSettings.Default.EgsEcfEditorApp_ActiveGameVersion = 
                Convert.ToString(GameVersionComboBox.SelectedItem);
            HasUnsavedData = true;
        }
        private void WriteOnlyValidItemsCheckBox_Click(object sender, EventArgs evt)
        {
            UserSettings.Default.EgsEcfEditorApp_FileCreation_WriteOnlyValidItems = WriteOnlyValidItemsCheckBox.Checked;
            UpdateCreationPanel();
            HasUnsavedData = true;
        }
        private void InvalidateParentsOnErrorCheckBox_Click(object sender, EventArgs evt)
        {
            UserSettings.Default.EgsEcfEditorApp_FileCreation_InvalidateParentsOnError = InvalidateParentsOnErrorCheckBox.Checked;
            HasUnsavedData = true;
        }
        private void AllowFallbackToParsedDataCheckBox_Click(object sender, EventArgs evt)
        {
            UserSettings.Default.EgsEcfEditorApp_FileCreation_AllowFallbackToParsedData = AllowFallbackToParsedDataCheckBox.Checked;
            HasUnsavedData = true;
        }
        private void TreeViewFilterCommentsInitActiveCheckBox_Click(object sender, EventArgs evt)
        {
            UserSettings.Default.EgsEcfControls_TreeViewFilterCommentsInitActive = TreeViewFilterCommentsInitActiveCheckBox.Checked;
            HasUnsavedData = true;
        }
        private void TreeViewFilterParametersInitActiveCheckBox_Click(object sender, EventArgs evt)
        {
            UserSettings.Default.EgsEcfControls_TreeViewFilterParametersInitActive = TreeViewFilterParametersInitActiveCheckBox.Checked;
            HasUnsavedData = true;
        }
        private void TreeViewFilterDataBlocksInitActiveCheckBox_Click(object sender, EventArgs evt)
        {
            UserSettings.Default.EgsEcfControls_TreeViewFilterDataBlocksInitActive = TreeViewFilterDataBlocksInitActiveCheckBox.Checked;
            HasUnsavedData = true;
        }
        private void TreeViewSorterInitCountComboBox_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            UserSettings.Default.EgsEcfControls_TreeViewSorterInitCount =
                Convert.ToInt32(TreeViewSorterInitCountComboBox.SelectedItem);
            HasUnsavedData = true;
        }
        private void ParameterViewSorterInitCountComboBox_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            UserSettings.Default.EgsEcfControls_ParameterViewSorterInitCount =
                Convert.ToInt32(ParameterViewSorterInitCountComboBox.SelectedItem);
            HasUnsavedData = true;
        }
        private void ErrorViewSorterInitCountComboBox_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            UserSettings.Default.EgsEcfControls_ErrorViewSorterInitCount =
                Convert.ToInt32(ErrorViewSorterInitCountComboBox.SelectedItem);
            HasUnsavedData = true;
        }
        private void TechTreeParameterKeyReferenceNameTextBox_TextChanged(object sender, EventArgs evt)
        {
            UserSettings.Default.EcfTechTreeDialog_ParameterKey_ReferenceName = 
                TechTreeParameterKeyReferenceNameTextBox.Text;
            HasUnsavedData = true;
        }
        private void TechTreeParameterKeyTechTreeNamesTextBox_TextChanged(object sender, EventArgs evt)
        {
            UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeNames = 
                TechTreeParameterKeyTechTreeNamesTextBox.Text;
            HasUnsavedData = true;
        }
        private void TechTreeParameterKeyTechTreeParentNameTextBox_TextChanged(object sender, EventArgs evt)
        {
            UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeParentName = 
                TechTreeParameterKeyTechTreeParentNameTextBox.Text;
            HasUnsavedData = true;
        }
        private void TechTreeParameterKeyUnlockLevelTextBox_TextChanged(object sender, EventArgs evt)
        {
            UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockLevel = 
                TechTreeParameterKeyUnlockLevelTextBox.Text;
            HasUnsavedData = true;
        }
        private void TechTreeDefaultValueUnlockLevelNumericUpDown_ValueChanged(object sender, EventArgs evt)
        {
            UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockLevel = 
                Convert.ToInt32(TechTreeDefaultValueUnlockLevelNumericUpDown.Value);
            HasUnsavedData = true;
        }
        private void TechTreeParameterKeyUnlockCostTextBox_TextChanged(object sender, EventArgs evt)
        {
            UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockCost = 
                TechTreeParameterKeyUnlockCostTextBox.Text;
            HasUnsavedData = true;
        }
        private void TechTreeDefaultValueUnlockCostNumericUpDown_ValueChanged(object sender, EventArgs evt)
        {
            UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockCost = 
                Convert.ToInt32(TechTreeDefaultValueUnlockCostNumericUpDown.Value);
            HasUnsavedData = true;
        }
        private void LicenseDataLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt)
        {
            Process.Start(Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTrademarkAttribute), false).Cast<AssemblyTrademarkAttribute>().FirstOrDefault().Trademark);
        }
        private void ReadmeDataLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt)
        {
            Process.Start(Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).Cast<AssemblyDescriptionAttribute>().FirstOrDefault().Description);
        }

        // privates
        private void InitPanels()
        {
            InitGeneralPanel(0);
            InitCreationPanel(1);
            InitFilterPanel(2);
            InitSorterPanel(3);
            InitTechTreePanel(4);
            InitInfoPanel(5);
        }
        private void InitGeneralPanel(int index)
        {
            ChapterSelectorTreeView.Nodes[index].Text = TitleRecources.EcfSettingsDialog_GeneralPanel_Header;

            GameVersionLabel.Text = TitleRecources.EcfSettingsDialog_GeneralPanel_GameVersion;

            Tip.SetToolTip(GameVersionComboBox, TextRecources.EcfSettingsDialog_ToolTip_GameVersionFolder);
        }
        private void InitCreationPanel(int index)
        {
            ChapterSelectorTreeView.Nodes[index].Text = TitleRecources.EcfSettingsDialog_CreationPanel_Header;

            WriteOnlyValidItemsLabel.Text = TitleRecources.EcfSettingsDialog_CreationPanel_WriteOnlyValidItems;
            InvalidateParentsOnErrorLabel.Text = TitleRecources.EcfSettingsDialog_CreationPanel_InvalidateParentOnError;
            AllowFallbackToParsedDataLabel.Text = TitleRecources.EcfSettingsDialog_CreationPanel_AllowFallbackToParsedData;

            Tip.SetToolTip(WriteOnlyValidItemsCheckBox, TextRecources.EcfSettingsDialog_ToolTip_WriteOnlyValidItems);
            Tip.SetToolTip(InvalidateParentsOnErrorCheckBox, TextRecources.EcfSettingsDialog_ToolTip_InvalidateParentsOnError);
            Tip.SetToolTip(AllowFallbackToParsedDataCheckBox, TextRecources.EcfSettingsDialog_ToolTip_AllowFallbackToParsedData);
        }
        private void InitFilterPanel(int index)
        {
            ChapterSelectorTreeView.Nodes[index].Text = TitleRecources.EcfSettingsDialog_FilterPanel_Header;

            TreeViewFilterCommentsInitActiveLabel.Text = TitleRecources.EcfSettingsDialog_FilterPanel_TreeViewFilterCommentsInitActive;
            TreeViewFilterParametersInitActiveLabel.Text = TitleRecources.EcfSettingsDialog_FilterPanel_TreeViewFilterParametersInitActive;
            TreeViewFilterDataBlocksInitActiveLabel.Text = TitleRecources.EcfSettingsDialog_FilterPanel_TreeViewFilterDataBlocksInitActive;

            Tip.SetToolTip(TreeViewFilterCommentsInitActiveCheckBox, TextRecources.EcfSettingsDialog_ToolTip_TreeViewFilterCommentsInitActive);
            Tip.SetToolTip(TreeViewFilterParametersInitActiveCheckBox, TextRecources.EcfSettingsDialog_ToolTip_TreeViewFilterParametersInitActive);
            Tip.SetToolTip(TreeViewFilterDataBlocksInitActiveCheckBox, TextRecources.EcfSettingsDialog_ToolTip_TreeViewFilterDataBlocksInitActive);
        }
        private void InitSorterPanel(int index)
        {
            ChapterSelectorTreeView.Nodes[index].Text = TitleRecources.EcfSettingsDialog_SorterPanel_Header;

            TreeViewSorterInitCountLabel.Text = TitleRecources.EcfSettingsDialog_SorterPanel_TreeViewSorterInitCount;
            ParameterViewSorterInitCountLabel.Text = TitleRecources.EcfSettingsDialog_SorterPanel_ParameterViewSorterInitCount;
            ErrorViewSorterInitCountLabel.Text = TitleRecources.EcfSettingsDialog_SorterPanel_ErrorViewSorterInitCount;

            Tip.SetToolTip(TreeViewSorterInitCountComboBox, TextRecources.EcfSettingsDialog_ToolTip_TreeViewSorterInitCount);
            Tip.SetToolTip(ParameterViewSorterInitCountComboBox, TextRecources.EcfSettingsDialog_ToolTip_ParameterViewSorterInitCount);
            Tip.SetToolTip(ErrorViewSorterInitCountComboBox, TextRecources.EcfSettingsDialog_ToolTip_ErrorViewSorterInitCount);
        }
        private void InitTechTreePanel(int index)
        {
            ChapterSelectorTreeView.Nodes[index].Text = TitleRecources.EcfSettingsDialog_TechTreePanel_Header;

            TechTreeParameterKeyReferenceNameLabel.Text = TitleRecources.EcfSettingsDialog_TechTreePanel_ParameterKeyReferenceName;
            TechTreeParameterKeyTechTreeNamesLabel.Text = TitleRecources.EcfSettingsDialog_TechTreePanel_ParameterKeyTechTreeNames;
            TechTreeParameterKeyTechTreeParentNameLabel.Text = TitleRecources.EcfSettingsDialog_TechTreePanel_ParameterKeyTechTreeParentName;
            TechTreeParameterKeyUnlockLevelLabel.Text = TitleRecources.EcfSettingsDialog_TechTreePanel_ParameterKeyUnlockLevel;
            TechTreeDefaultValueUnlockLevelLabel.Text = TitleRecources.EcfSettingsDialog_TechTreePanel_DefaultValueUnlockLevel;
            TechTreeParameterKeyUnlockCostLabel.Text = TitleRecources.EcfSettingsDialog_TechTreePanel_ParameterKeyUnlockCost;
            TechTreeDefaultValueUnlockCostLabel.Text = TitleRecources.EcfSettingsDialog_TechTreePanel_DefaultValueUnlockCost;

            Tip.SetToolTip(TechTreeParameterKeyReferenceNameTextBox, TextRecources.EcfSettingsDialog_ToolTip_ParameterKeyReferenceName);
            Tip.SetToolTip(TechTreeParameterKeyTechTreeNamesTextBox, TextRecources.EcfSettingsDialog_ToolTip_ParameterKeyTechTreeNames);
            Tip.SetToolTip(TechTreeParameterKeyTechTreeParentNameTextBox, TextRecources.EcfSettingsDialog_ToolTip_ParameterKeyTechTreeParentName);
            Tip.SetToolTip(TechTreeParameterKeyUnlockLevelTextBox, TextRecources.EcfSettingsDialog_ToolTip_ParameterKeyUnlockLevel);
            Tip.SetToolTip(TechTreeDefaultValueUnlockLevelNumericUpDown, TextRecources.EcfSettingsDialog_ToolTip_DefaultValueUnlockLevel);
            Tip.SetToolTip(TechTreeParameterKeyUnlockCostTextBox, TextRecources.EcfSettingsDialog_ToolTip_ParameterKeyUnlockCost);
            Tip.SetToolTip(TechTreeDefaultValueUnlockCostNumericUpDown, TextRecources.EcfSettingsDialog_ToolTip_DefaultValueUnlockCost);
        }
        private void InitInfoPanel(int index)
        {
            ChapterSelectorTreeView.Nodes[index].Text = TitleRecources.EcfSettingsDialog_InfoPanel_Header;

            AuthorTitleLabel.Text = TitleRecources.Generic_Author;
            VersionTitleLabel.Text = TitleRecources.Generic_Version;
            LicenseTitleLabel.Text = TitleRecources.Generic_License;
            ReadmeTitleLabel.Text = TitleRecources.Generic_Manual;

            AppNameDataLabel.Text = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false).Cast<AssemblyTitleAttribute>().FirstOrDefault().Title;
            LogoPictureBox.Image = new Icon(IconRecources.Icon_AppBranding, 256, 256).ToBitmap();
            AuthorDataLabel.Text = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false).Cast<AssemblyCompanyAttribute>().FirstOrDefault().Company;
            VersionDataLabel.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            LicenseDataLinkLabel.Text = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false).Cast<AssemblyCopyrightAttribute>().FirstOrDefault().Copyright;
            ReadmeDataLinkLabel.Text = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false).Cast<AssemblyConfigurationAttribute>().FirstOrDefault().Configuration;
        }

        private void SwitchPanel(int index)
        {
            // hack to prevent tab switch with tab key
            SettingPanelsTabControl.TabPages.Clear();
            switch (index)
            {
                case 0: SettingPanelsTabControl.TabPages.Add(GeneralTabPage); break;
                case 1: SettingPanelsTabControl.TabPages.Add(CreationTabPage); break;
                case 2: SettingPanelsTabControl.TabPages.Add(FilterTabPage); break;
                case 3: SettingPanelsTabControl.TabPages.Add(SorterTabPage); break;
                case 4: SettingPanelsTabControl.TabPages.Add(TechTreeTabPage); break;
                case 5: SettingPanelsTabControl.TabPages.Add(InfoTabPage); break;
                default: break;
            }
        }
        
        private void PreparePanels()
        {
            PrepareGeneralPanel();
            PrepareSorterPanel();
        }
        private void PrepareGeneralPanel()
        {
            GameVersionComboBox.BeginUpdate();
            GameVersionComboBox.Items.Clear();
            GameVersionComboBox.Items.AddRange(GetGameModes().ToArray());
            GameVersionComboBox.EndUpdate();
        }
        private void PrepareSorterPanel()
        {
            TreeViewSorterInitCountComboBox.BeginUpdate();
            TreeViewSorterInitCountComboBox.Items.Clear();
            TreeViewSorterInitCountComboBox.Items.AddRange(Enum.GetValues(typeof(VisibleItemCount)).Cast<VisibleItemCount>().Select(value => Convert.ToInt32(value).ToString()).ToArray());
            TreeViewSorterInitCountComboBox.EndUpdate();

            ParameterViewSorterInitCountComboBox.BeginUpdate();
            ParameterViewSorterInitCountComboBox.Items.Clear();
            ParameterViewSorterInitCountComboBox.Items.AddRange(Enum.GetValues(typeof(VisibleItemCount)).Cast<VisibleItemCount>().Select(value => Convert.ToInt32(value).ToString()).ToArray());
            ParameterViewSorterInitCountComboBox.EndUpdate();

            ErrorViewSorterInitCountComboBox.BeginUpdate();
            ErrorViewSorterInitCountComboBox.Items.Clear();
            ErrorViewSorterInitCountComboBox.Items.AddRange(Enum.GetValues(typeof(VisibleItemCount)).Cast<VisibleItemCount>().Select(value => Convert.ToInt32(value).ToString()).ToArray());
            ErrorViewSorterInitCountComboBox.EndUpdate();
        }

        private void PresetPanels()
        {
            PresetGeneralPanel();
            PresetCreationPanel();
            PresetFilterPanel();
            PresetSorterPanel();
            PresetTechTreePanel();
        }
        private void PresetGeneralPanel()
        {
            GameVersionComboBox.SelectedItem = UserSettings.Default.EgsEcfEditorApp_ActiveGameVersion;
            if (GameVersionComboBox.SelectedIndex < 0)
            {
                GameVersionComboBox.SelectedIndex = 0;
            }
        }
        private void PresetCreationPanel()
        {
            WriteOnlyValidItemsCheckBox.Checked = UserSettings.Default.EgsEcfEditorApp_FileCreation_WriteOnlyValidItems;
            InvalidateParentsOnErrorCheckBox.Checked = UserSettings.Default.EgsEcfEditorApp_FileCreation_InvalidateParentsOnError;
            AllowFallbackToParsedDataCheckBox.Checked = UserSettings.Default.EgsEcfEditorApp_FileCreation_AllowFallbackToParsedData;
            UpdateCreationPanel();
        }
        private void PresetFilterPanel()
        {
            TreeViewFilterCommentsInitActiveCheckBox.Checked = UserSettings.Default.EgsEcfControls_TreeViewFilterCommentsInitActive;
            TreeViewFilterParametersInitActiveCheckBox.Checked = UserSettings.Default.EgsEcfControls_TreeViewFilterParametersInitActive;
            TreeViewFilterDataBlocksInitActiveCheckBox.Checked = UserSettings.Default.EgsEcfControls_TreeViewFilterDataBlocksInitActive;
        }
        private void PresetSorterPanel()
        {
            TreeViewSorterInitCountComboBox.SelectedItem = UserSettings.Default.EgsEcfControls_TreeViewSorterInitCount.ToString();
            if (TreeViewSorterInitCountComboBox.SelectedIndex < 0)
            {
                TreeViewSorterInitCountComboBox.SelectedIndex = 0;
            }
            
            ParameterViewSorterInitCountComboBox.SelectedItem = UserSettings.Default.EgsEcfControls_ParameterViewSorterInitCount.ToString();
            if (ParameterViewSorterInitCountComboBox.SelectedIndex < 0)
            {
                ParameterViewSorterInitCountComboBox.SelectedIndex = 0;
            }
            
            ErrorViewSorterInitCountComboBox.SelectedItem = UserSettings.Default.EgsEcfControls_ErrorViewSorterInitCount.ToString();
            if (ErrorViewSorterInitCountComboBox.SelectedIndex < 0)
            {
                ErrorViewSorterInitCountComboBox.SelectedIndex = 0;
            }
        }
        private void PresetTechTreePanel()
        {
            TechTreeParameterKeyReferenceNameTextBox.Text = UserSettings.Default.EcfTechTreeDialog_ParameterKey_ReferenceName;
            TechTreeParameterKeyTechTreeNamesTextBox.Text = UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeNames;
            TechTreeParameterKeyTechTreeParentNameTextBox.Text = UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeParentName;
            TechTreeParameterKeyUnlockLevelTextBox.Text = UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockLevel;
            TechTreeDefaultValueUnlockLevelNumericUpDown.Value = UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockLevel;
            TechTreeParameterKeyUnlockCostTextBox.Text = UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockCost;
            TechTreeDefaultValueUnlockCostNumericUpDown.Value = UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockCost;
        }

        private void UpdateCreationPanel()
        {
            InvalidateParentsOnErrorCheckBox.Enabled = WriteOnlyValidItemsCheckBox.Checked;
            AllowFallbackToParsedDataCheckBox.Enabled = WriteOnlyValidItemsCheckBox.Checked;
        }
    }
}
