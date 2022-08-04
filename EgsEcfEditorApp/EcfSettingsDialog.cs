using EgsEcfEditorApp.Properties;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static EcfFileViewTools.EcfSorter;
using static EgsEcfParser.EcfDefinitionHandling;

namespace EgsEcfEditorApp
{
    public partial class EcfSettingsDialog : Form
    {
        public bool HasUnsavedData { get; private set; } = false;
        
        public EcfSettingsDialog()
        {
            InitializeComponent();
            InitForm();
        }

        // events
        private void InitForm()
        {
            Icon = IconRecources.Icon_App;
            Text = TitleRecources.EcfSettingsDialog_Header;

            // Hack to hide tabs
            SettingPanelsTabControl.SizeMode = TabSizeMode.Fixed;
            SettingPanelsTabControl.ItemSize = new Size(0, 1);

            SaveButton.Text = TitleRecources.Generic_Save;
            ResetButton.Text = TitleRecources.Generic_Reset;
            AbortButton.Text = TitleRecources.Generic_Abort;

            InitPanels();

            ChapterSelectorTreeView.SelectedNode = ChapterSelectorTreeView.Nodes[0];
            SwitchPanelw(0);
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
            SwitchPanelw(evt.Node.Index);
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
                Convert.ToString(GameVersionFolderComboBox.SelectedItem);
            HasUnsavedData = true;
        }
        private void WriteOnlyValidItemsCheckBox_Click(object sender, EventArgs evt)
        {
            UserSettings.Default.EgsEcfEditorApp_WriteOnlyValidItems = WriteOnlyValidItemsCheckBox.Checked;
            UpdateInvalidateParentOnErrorCheckBox();
            HasUnsavedData = true;
        }
        private void InvalidateParentsOnErrorCheckBox_Click(object sender, EventArgs evt)
        {
            UserSettings.Default.EgsEcfEditorApp_InvalidateParentsOnError = InvalidateParentsOnErrorCheckBox.Checked;
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

        // privates
        private void InitPanels()
        {
            InitGeneralPanel();
            InitCreationPanel();
            InitFilterPanel();
            InitSorterPanel();
        }
        private void InitGeneralPanel()
        {
            ChapterSelectorTreeView.Nodes[0].Text = TitleRecources.EcfSettingsDialog_GeneralSettingsChapterHeader;
            GameVersionFolderLabel.Text = TitleRecources.EcfSettingsDialog_GameVersionFolder;
            new ToolTip().SetToolTip(GameVersionFolderComboBox, TextRecources.EcfSettingsDialog_ToolTip_GameVersionFolder);
        }
        private void InitCreationPanel()
        {
            ChapterSelectorTreeView.Nodes[1].Text = TitleRecources.EcfSettingsDialog_CreationSettingsChapterHeader;
            WriteOnlyValidItemsLabel.Text = TitleRecources.EcfSettingsDialog_WriteOnlyValidItems;
            InvalidateParentsOnErrorLabel.Text = TitleRecources.EcfSettingsDialog_InvalidateParentOnError;
            new ToolTip().SetToolTip(WriteOnlyValidItemsCheckBox, TextRecources.EcfSettingsDialog_ToolTip_WriteOnlyValidItems);
            new ToolTip().SetToolTip(InvalidateParentsOnErrorCheckBox, TextRecources.EcfSettingsDialog_ToolTip_InvalidateParentsOnError);
        }
        private void InitFilterPanel()
        {
            ChapterSelectorTreeView.Nodes[2].Text = TitleRecources.EcfSettingsDialog_FilterSettingsChapterHeader;
            TreeViewFilterCommentsInitActiveLabel.Text = TitleRecources.EcfSettingsDialog_TreeViewFilterCommentsInitActive;
            TreeViewFilterParametersInitActiveLabel.Text = TitleRecources.EcfSettingsDialog_TreeViewFilterParametersInitActive;
            TreeViewFilterDataBlocksInitActiveLabel.Text = TitleRecources.EcfSettingsDialog_TreeViewFilterDataBlocksInitActive;
            new ToolTip().SetToolTip(TreeViewFilterCommentsInitActiveCheckBox, TextRecources.EcfSettingsDialog_ToolTip_TreeViewFilterCommentsInitActive);
            new ToolTip().SetToolTip(TreeViewFilterParametersInitActiveCheckBox, TextRecources.EcfSettingsDialog_ToolTip_TreeViewFilterParametersInitActive);
            new ToolTip().SetToolTip(TreeViewFilterDataBlocksInitActiveCheckBox, TextRecources.EcfSettingsDialog_ToolTip_TreeViewFilterDataBlocksInitActive);
        }
        private void InitSorterPanel()
        {
            ChapterSelectorTreeView.Nodes[3].Text = TitleRecources.EcfSettingsDialog_SorterSettingsChapterHeader;
            TreeViewSorterInitCountLabel.Text = TitleRecources.EcfSettingsDialog_TreeViewSorterInitCount;
            ParameterViewSorterInitCountLabel.Text = TitleRecources.EcfSettingsDialog_ParameterViewSorterInitCount;
            ErrorViewSorterInitCountLabel.Text = TitleRecources.EcfSettingsDialog_ErrorViewSorterInitCount;
            new ToolTip().SetToolTip(TreeViewSorterInitCountComboBox, TextRecources.EcfSettingsDialog_ToolTip_TreeViewSorterInitCount);
            new ToolTip().SetToolTip(ParameterViewSorterInitCountComboBox, TextRecources.EcfSettingsDialog_ToolTip_ParameterViewSorterInitCount);
            new ToolTip().SetToolTip(ErrorViewSorterInitCountComboBox, TextRecources.EcfSettingsDialog_ToolTip_ErrorViewSorterInitCount);
        }

        private void SwitchPanelw(int index)
        {
            // hack to prevent tab switch with tab key
            SettingPanelsTabControl.TabPages.Clear();
            switch (index)
            {
                case 0: SettingPanelsTabControl.TabPages.Add(GeneralTabPage); break;
                case 1: SettingPanelsTabControl.TabPages.Add(CreationTabPage); break;
                case 2: SettingPanelsTabControl.TabPages.Add(FilterTabPage); break;
                case 3: SettingPanelsTabControl.TabPages.Add(SorterTabPage); break;
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
            GameVersionFolderComboBox.BeginUpdate();
            GameVersionFolderComboBox.Items.Clear();
            GameVersionFolderComboBox.Items.AddRange(GetGameModes().ToArray());
            GameVersionFolderComboBox.EndUpdate();
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
        }
        private void PresetGeneralPanel()
        {
            GameVersionFolderComboBox.SelectedItem = UserSettings.Default.EgsEcfEditorApp_ActiveGameVersion;
            if (GameVersionFolderComboBox.SelectedIndex < 0)
            {
                GameVersionFolderComboBox.SelectedIndex = 0;
            }
        }
        private void PresetCreationPanel()
        {
            WriteOnlyValidItemsCheckBox.Checked = UserSettings.Default.EgsEcfEditorApp_WriteOnlyValidItems;
            InvalidateParentsOnErrorCheckBox.Checked = UserSettings.Default.EgsEcfEditorApp_InvalidateParentsOnError;
            UpdateInvalidateParentOnErrorCheckBox();
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

        private void UpdateInvalidateParentOnErrorCheckBox()
        {
            InvalidateParentsOnErrorCheckBox.Enabled = WriteOnlyValidItemsCheckBox.Checked;
        }
    }
}
