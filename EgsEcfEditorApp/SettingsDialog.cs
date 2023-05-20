using EgsEcfEditorApp.Properties;
using System;
using System.Collections.Generic;
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
        private List<SettingsPanel> SettingsPanels { get; } = new List<SettingsPanel>();
        
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

            InitControls();
        }
        private void EcfSettingsDialog_FormClosing(object sender, FormClosingEventArgs evt)
        {
            if (HasUnsavedChanges())
            {
                if (MessageBox.Show(this, TextRecources.EcfSettingsDialog_UnsavedReallyCloseQuestion, 
                    TitleRecources.Generic_Attention, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    UserSettings.Default.Reload();
                    ResetUnsavedChanges();
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
                UpdatePanels();
                ResetUnsavedChanges();
            }
        }
        private void SaveButton_Click(object sender, EventArgs evt)
        {
            if (HasUnsavedChanges())
            {
                UserSettings.Default.Save();
                ResetUnsavedChanges();
            }
            DialogResult = DialogResult.OK;
            Close();
        }
        private void ChapterSelectorTreeView_AfterSelect(object sender, TreeViewEventArgs evt)
        {
            SettingsBasePanel.Controls.Clear();
            SettingsBasePanel.Controls.Add(SettingsPanels.FirstOrDefault(panel => string.Equals(panel.IdTag, ChapterSelectorTreeView.SelectedNode.Text)));
        }
        private void EcfSettingsDialog_Activated(object sender, EventArgs evt)
        {
            PreparePanels();
            UpdatePanels();
            ChapterSelectorTreeView.Focus();
        }

        // privates
        private void InitControls()
        {
            SaveButton.Text = TitleRecources.Generic_Save;
            ResetButton.Text = TitleRecources.Generic_Reset;
            AbortButton.Text = TitleRecources.Generic_Abort;

            SettingsPanels.Add(new GeneralSettingsPanel(TitleRecources.EcfSettingsDialog_General_Header, Tip));
            SettingsPanels.Add(new CreationSettingsPanel(TitleRecources.EcfSettingsDialog_Creation_Header, Tip));
            SettingsPanels.Add(new TreeFilterSettingsPanel(TitleRecources.EcfSettingsDialog_Filter_Header, Tip));
            SettingsPanels.Add(new SorterSettingsPanel(TitleRecources.EcfSettingsDialog_Sorter_Header, Tip));
            SettingsPanels.Add(new TechTreeSettingsPanel(TitleRecources.EcfSettingsDialog_TechTree_Header, Tip));
            SettingsPanels.Add(new ItemHandlingSupportSettingsPanel(TitleRecources.EcfSettingsDialog_ItemHandlingSupport_Header, Tip));
            SettingsPanels.Add(new InfoPanel(TitleRecources.EcfSettingsDialog_Info_Header));

            ChapterSelectorTreeView.Nodes.Add(TitleRecources.EcfSettingsDialog_General_Header);
            ChapterSelectorTreeView.Nodes.Add(TitleRecources.EcfSettingsDialog_Creation_Header);
            ChapterSelectorTreeView.Nodes.Add(TitleRecources.EcfSettingsDialog_Filter_Header);
            ChapterSelectorTreeView.Nodes.Add(TitleRecources.EcfSettingsDialog_Sorter_Header);
            ChapterSelectorTreeView.Nodes.Add(TitleRecources.EcfSettingsDialog_TechTree_Header);
            ChapterSelectorTreeView.Nodes.Add(TitleRecources.EcfSettingsDialog_ItemHandlingSupport_Header);
            ChapterSelectorTreeView.Nodes.Add(TitleRecources.EcfSettingsDialog_Info_Header);

            ChapterSelectorTreeView.SelectedNode = ChapterSelectorTreeView.Nodes[0];
        }
        private bool HasUnsavedChanges()
        {
            return SettingsPanels.Any(panel => panel.HasUnsavedChanges);
        }
        private void ResetUnsavedChanges()
        {
            SettingsPanels.ForEach(panel => panel.HasUnsavedChanges = false);
        }
        private void PreparePanels()
        {
            SettingsPanels.ForEach(panel => panel.PresetItems());
        }
        private void UpdatePanels()
        {
            SettingsPanels.ForEach(panel => panel.UpdateItems());
        }

        // panel classes
        private abstract class SettingsPanel : TableLayoutPanel
        {
            public bool HasUnsavedChanges { get; set; } = false;
            public string IdTag { get; private set; } = null;

            private int ItemsCount { get; set; } = 0;
            private ToolTip ToolTipContainer { get; }

            public SettingsPanel(string idTag, ToolTip toolTipContainer)
            {
                IdTag = idTag;
                ToolTipContainer = toolTipContainer;
                InitControls();
            }

            // publics
            public abstract void PresetItems();
            public abstract void UpdateItems();

            // privates
            private void InitControls()
            {
                SuspendLayout();

                AutoSize = true;
                ColumnCount = 1;
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                Dock = DockStyle.Fill;
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize;

                ResumeLayout(false);
            }
            protected void AddSettingsItem(SettingsItem item, string title, string toolTip, EventHandler onSettingChanged)
            {
                item.Text = title;
                item.SetToolTip(ToolTipContainer, toolTip);
                item.SettingChanged += onSettingChanged;

                AddSettingsItem(item);
            }
            protected void AddSettingsItem(SettingsTableItem item)
            {
                Controls.Add(item, 0, ItemsCount);
                RowStyles.Add(new RowStyle());
                ItemsCount++;
                RowCount = ItemsCount;
            }
            protected void AddSpacer(SettingsTableItem item = null)
            {
                if (item != null)
                {
                    Controls.Add(item, 0, ItemsCount);
                    ItemsCount++;
                }
                else
                {
                    ItemsCount++;
                }
                RowCount = ItemsCount;
                RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            }
        }
        private class GeneralSettingsPanel : SettingsPanel
        {
            private ComboBoxSettingItem GameMode { get; } = new ComboBoxSettingItem(); 

            public GeneralSettingsPanel(string idTag, ToolTip toolTipContainer) : base(idTag, toolTipContainer)
            {
                InitControls();
            }

            // events
            private void GameMode_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EgsEcfEditorApp_ActiveGameMode = Convert.ToString(GameMode.Value);
                HasUnsavedChanges = true;
            }

            // public
            public override void PresetItems()
            {
                GameMode.PresetItem(GetGameModes().ToArray());
            }
            public override void UpdateItems()
            {
                GameMode.Value = UserSettings.Default.EgsEcfEditorApp_ActiveGameMode;
            }

            // privates
            private void InitControls()
            {
                SuspendLayout();

                AddSettingsItem(GameMode, TitleRecources.EcfSettingsDialog_General_GameMode,
                    TextRecources.EcfSettingsDialog_ToolTip_GameMode, GameMode_SettingChanged);
                AddSpacer();

                ResumeLayout(true);
            }
        }
        private class CreationSettingsPanel : SettingsPanel
        {
            private CheckBoxSettingItem WriteOnlyValidItems { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem InvalidateParentsOnError { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem AllowFallbackToParsedData { get; } = new CheckBoxSettingItem();

            public CreationSettingsPanel(string idTag, ToolTip toolTipContainer) : base(idTag, toolTipContainer)
            {
                InitControls();
            }

            // events
            private void WriteOnlyValidItems_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EgsEcfEditorApp_FileCreation_WriteOnlyValidItems = WriteOnlyValidItems.Value;
                UpdateAccessability();
                HasUnsavedChanges = true;
            }
            private void InvalidateParentsOnError_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EgsEcfEditorApp_FileCreation_InvalidateParentsOnError = InvalidateParentsOnError.Value;
                HasUnsavedChanges = true;
            }
            private void AllowFallbackToParsedData_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EgsEcfEditorApp_FileCreation_AllowFallbackToParsedData = AllowFallbackToParsedData.Value;
                HasUnsavedChanges = true;
            }

            // public
            public override void PresetItems()
            {
                
            }
            public override void UpdateItems()
            {
                WriteOnlyValidItems.Value = UserSettings.Default.EgsEcfEditorApp_FileCreation_WriteOnlyValidItems;
                InvalidateParentsOnError.Value = UserSettings.Default.EgsEcfEditorApp_FileCreation_InvalidateParentsOnError;
                AllowFallbackToParsedData.Value = UserSettings.Default.EgsEcfEditorApp_FileCreation_AllowFallbackToParsedData;
                UpdateAccessability();
            }

            // private
            private void InitControls()
            {
                SuspendLayout();

                AddSettingsItem(WriteOnlyValidItems, TitleRecources.EcfSettingsDialog_Creation_WriteOnlyValidItems,
                    TextRecources.EcfSettingsDialog_ToolTip_WriteOnlyValidItems, WriteOnlyValidItems_SettingChanged);
                AddSettingsItem(InvalidateParentsOnError, TitleRecources.EcfSettingsDialog_Creation_InvalidateParentOnError,
                    TextRecources.EcfSettingsDialog_ToolTip_InvalidateParentsOnError, InvalidateParentsOnError_SettingChanged);
                AddSettingsItem(AllowFallbackToParsedData, TitleRecources.EcfSettingsDialog_Creation_AllowFallbackToParsedData,
                    TextRecources.EcfSettingsDialog_ToolTip_AllowFallbackToParsedData, AllowFallbackToParsedData_SettingChanged);
                AddSpacer();

                ResumeLayout(true);
            }
            private void UpdateAccessability()
            {
                InvalidateParentsOnError.IsEnabled = WriteOnlyValidItems.Value;
                AllowFallbackToParsedData.IsEnabled = WriteOnlyValidItems.Value;
            }
        }
        private class TreeFilterSettingsPanel : SettingsPanel
        {
            private CheckBoxSettingItem CommentsInitActive { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem ParametersInitActive { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DataBlocksInitActive { get; } = new CheckBoxSettingItem();

            public TreeFilterSettingsPanel(string idTag, ToolTip toolTipContainer) : base(idTag, toolTipContainer)
            {
                InitControls();
            }

            // events
            private void CommentsInitActive_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EgsEcfEditorApp_FileCreation_WriteOnlyValidItems = CommentsInitActive.Value;
                HasUnsavedChanges = true;
            }
            private void ParametersInitActive_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EgsEcfEditorApp_FileCreation_InvalidateParentsOnError = ParametersInitActive.Value;
                HasUnsavedChanges = true;
            }
            private void DataBlocksInitActive_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EgsEcfEditorApp_FileCreation_AllowFallbackToParsedData = DataBlocksInitActive.Value;
                HasUnsavedChanges = true;
            }

            // public
            public override void PresetItems()
            {

            }
            public override void UpdateItems()
            {
                CommentsInitActive.Value = UserSettings.Default.EgsEcfControls_TreeViewFilterCommentsInitActive;
                ParametersInitActive.Value = UserSettings.Default.EgsEcfControls_TreeViewFilterParametersInitActive;
                DataBlocksInitActive.Value = UserSettings.Default.EgsEcfControls_TreeViewFilterDataBlocksInitActive;
            }

            // privates
            private void InitControls()
            {
                SuspendLayout();

                AddSettingsItem(CommentsInitActive, TitleRecources.EcfSettingsDialog_Filter_TreeViewFilterCommentsInitActive,
                    TextRecources.EcfSettingsDialog_ToolTip_TreeViewFilterCommentsInitActive, CommentsInitActive_SettingChanged);
                AddSettingsItem(ParametersInitActive, TitleRecources.EcfSettingsDialog_Filter_TreeViewFilterParametersInitActive,
                    TextRecources.EcfSettingsDialog_ToolTip_TreeViewFilterParametersInitActive, ParametersInitActive_SettingChanged);
                AddSettingsItem(DataBlocksInitActive, TitleRecources.EcfSettingsDialog_Filter_TreeViewFilterDataBlocksInitActive,
                    TextRecources.EcfSettingsDialog_ToolTip_TreeViewFilterDataBlocksInitActive, DataBlocksInitActive_SettingChanged);
                AddSpacer();

                ResumeLayout(true);
            }
        }
        private class SorterSettingsPanel : SettingsPanel
        {
            private ComboBoxSettingItem TreeViewSorterInitCount { get; } = new ComboBoxSettingItem();
            private ComboBoxSettingItem ParameterViewSorterInitCount { get; } = new ComboBoxSettingItem();
            private ComboBoxSettingItem ErrorViewSorterInitCount { get; } = new ComboBoxSettingItem();

            public SorterSettingsPanel(string idTag, ToolTip toolTipContainer) : base(idTag, toolTipContainer)
            {
                InitControls();
            }

            // events
            private void TreeViewSorterInitCount_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EgsEcfControls_TreeViewSorterInitCount = Convert.ToInt32(TreeViewSorterInitCount.Value);
                HasUnsavedChanges = true;
            }
            private void ParameterViewSorterInitCount_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EgsEcfControls_ParameterViewSorterInitCount = Convert.ToInt32(ParameterViewSorterInitCount.Value);
                HasUnsavedChanges = true;
            }
            private void ErrorViewSorterInitCount_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EgsEcfControls_ErrorViewSorterInitCount = Convert.ToInt32(ErrorViewSorterInitCount.Value);
                HasUnsavedChanges = true;
            }

            // public
            public override void PresetItems()
            {
                TreeViewSorterInitCount.PresetItem(Enum.GetValues(typeof(VisibleItemCount)).Cast<VisibleItemCount>().Select(value => 
                    Convert.ToInt32(value).ToString()).ToArray());
                ParameterViewSorterInitCount.PresetItem(Enum.GetValues(typeof(VisibleItemCount)).Cast<VisibleItemCount>().Select(value => 
                    Convert.ToInt32(value).ToString()).ToArray());
                ErrorViewSorterInitCount.PresetItem(Enum.GetValues(typeof(VisibleItemCount)).Cast<VisibleItemCount>().Select(value => 
                    Convert.ToInt32(value).ToString()).ToArray());
            }
            public override void UpdateItems()
            {
                TreeViewSorterInitCount.Value = UserSettings.Default.EgsEcfControls_TreeViewSorterInitCount.ToString();
                ParameterViewSorterInitCount.Value = UserSettings.Default.EgsEcfControls_ParameterViewSorterInitCount.ToString();
                ErrorViewSorterInitCount.Value = UserSettings.Default.EgsEcfControls_ErrorViewSorterInitCount.ToString();
            }

            // private 
            private void InitControls()
            {
                SuspendLayout();

                AddSettingsItem(TreeViewSorterInitCount, TitleRecources.EcfSettingsDialog_Sorter_TreeViewSorterInitCount,
                    TextRecources.EcfSettingsDialog_ToolTip_TreeViewSorterInitCount, TreeViewSorterInitCount_SettingChanged);
                AddSettingsItem(ParameterViewSorterInitCount, TitleRecources.EcfSettingsDialog_Sorter_ParameterViewSorterInitCount,
                    TextRecources.EcfSettingsDialog_ToolTip_ParameterViewSorterInitCount, ParameterViewSorterInitCount_SettingChanged);
                AddSettingsItem(ErrorViewSorterInitCount, TitleRecources.EcfSettingsDialog_Sorter_ErrorViewSorterInitCount,
                    TextRecources.EcfSettingsDialog_ToolTip_ErrorViewSorterInitCount, ErrorViewSorterInitCount_SettingChanged);
                AddSpacer();

                ResumeLayout(true);
            }
        }
        private class TechTreeSettingsPanel : SettingsPanel
        {
            private TextBoxSettingItem ParameterKeyTechTreeNames { get; } = new TextBoxSettingItem();
            private TextBoxSettingItem ParameterKeyTechTreeParentName { get; } = new TextBoxSettingItem();
            private TextBoxSettingItem ParameterKeyUnlockLevel { get; } = new TextBoxSettingItem();
            private NumericSettingItem DefaultValueUnlockLevel { get; } = new NumericSettingItem();
            private TextBoxSettingItem ParameterKeyUnlockCost { get; } = new TextBoxSettingItem();
            private NumericSettingItem DefaultValueUnlockCost { get; } = new NumericSettingItem();

            public TechTreeSettingsPanel(string idTag, ToolTip toolTipContainer) : base(idTag, toolTipContainer)
            {
                InitControls();
            }

            // events
            private void ParameterKeyTechTreeNames_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeNames = ParameterKeyTechTreeNames.Value;
                HasUnsavedChanges = true;
            }
            private void ParameterKeyTechTreeParentName_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeParentName = ParameterKeyTechTreeParentName.Value;
                HasUnsavedChanges = true;
            }
            private void ParameterKeyUnlockLevel_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockLevel = ParameterKeyUnlockLevel.Value;
                HasUnsavedChanges = true;
            }
            private void DefaultValueUnlockLevel_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockLevel = Convert.ToInt32(DefaultValueUnlockLevel.Value);
                HasUnsavedChanges = true;
            }
            private void ParameterKeyUnlockCost_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockCost = ParameterKeyUnlockCost.Value;
                HasUnsavedChanges = true;
            }
            private void DefaultValueUnlockCost_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockCost = Convert.ToInt32(DefaultValueUnlockCost.Value);
                HasUnsavedChanges = true;
            }

            // public
            public override void PresetItems()
            {
                
            }
            public override void UpdateItems()
            {
                DefaultValueUnlockLevel.Minimum = InternalSettings.Default.EgsEcfEditorApp_ParameterHandling_UnlockLevelMinValue;
                DefaultValueUnlockLevel.Maximum = InternalSettings.Default.EgsEcfEditorApp_ParameterHandling_UnlockLevelMaxValue;
                DefaultValueUnlockCost.Minimum = InternalSettings.Default.EgsEcfEditorApp_ParameterHandling_UnlockCostMinValue;
                DefaultValueUnlockCost.Maximum = InternalSettings.Default.EgsEcfEditorApp_ParameterHandling_UnlockCostMaxValue;

                ParameterKeyTechTreeNames.Value = UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeNames;
                ParameterKeyTechTreeParentName.Value = UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeParentName;
                ParameterKeyUnlockLevel.Value = UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockLevel;
                DefaultValueUnlockLevel.Value = UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockLevel;
                ParameterKeyUnlockCost.Value = UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockCost;
                DefaultValueUnlockCost.Value = UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockCost;
            }

            // private
            private void InitControls()
            {
                SuspendLayout();

                AddSettingsItem(ParameterKeyTechTreeNames, TitleRecources.EcfSettingsDialog_TechTree_ParameterKeyTechTreeNames,
                    TextRecources.EcfSettingsDialog_ToolTip_ParameterKeyTechTreeNames, ParameterKeyTechTreeNames_SettingChanged);
                AddSettingsItem(ParameterKeyTechTreeParentName, TitleRecources.EcfSettingsDialog_TechTree_ParameterKeyTechTreeParentName,
                    TextRecources.EcfSettingsDialog_ToolTip_ParameterKeyTechTreeParentName, ParameterKeyTechTreeParentName_SettingChanged);
                AddSettingsItem(ParameterKeyUnlockLevel, TitleRecources.EcfSettingsDialog_TechTree_ParameterKeyUnlockLevel,
                    TextRecources.EcfSettingsDialog_ToolTip_ParameterKeyUnlockLevel, ParameterKeyUnlockLevel_SettingChanged);
                AddSettingsItem(DefaultValueUnlockLevel, TitleRecources.EcfSettingsDialog_TechTree_DefaultValueUnlockLevel,
                    TextRecources.EcfSettingsDialog_ToolTip_DefaultValueUnlockLevel, DefaultValueUnlockLevel_SettingChanged);
                AddSettingsItem(ParameterKeyUnlockCost, TitleRecources.EcfSettingsDialog_TechTree_ParameterKeyUnlockCost,
                    TextRecources.EcfSettingsDialog_ToolTip_ParameterKeyUnlockCost, ParameterKeyUnlockCost_SettingChanged);
                AddSettingsItem(DefaultValueUnlockCost, TitleRecources.EcfSettingsDialog_TechTree_DefaultValueUnlockCost,
                    TextRecources.EcfSettingsDialog_ToolTip_DefaultValueUnlockCost, DefaultValueUnlockCost_SettingChanged);
                AddSpacer();

                ResumeLayout(true);
            }
        }
        private class ItemHandlingSupportSettingsPanel : SettingsPanel
        {
            private CheckBoxSettingItem InterFileChecksActive { get; } = new CheckBoxSettingItem();
            private TextBoxSettingItem ParameterKeyTemplateRoot { get; } = new TextBoxSettingItem();
            private TextBoxSettingItem ParameterKeyBlocks { get; } = new TextBoxSettingItem();
            private CheckBoxSettingItem DefaultDefinitionIsOptional { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefaultDefinitionHasValue { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefaultDefinitionIsAllowingBlank { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefaultDefinitionIsForceEscaped { get; } = new CheckBoxSettingItem();
            private TextBoxSettingItem DefaultDefinitionInfo { get; } = new TextBoxSettingItem();

            public ItemHandlingSupportSettingsPanel(string idTag, ToolTip toolTipContainer) : base(idTag, toolTipContainer)
            {
                InitControls();
            }

            // events
            private void InterFileChecksActive_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_InterFileChecksActive = InterFileChecksActive.Value;
                HasUnsavedChanges = true;
            }
            private void ParameterKeyTemplateRoot_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_ParameterKey_TemplateName = ParameterKeyTemplateRoot.Value;
                HasUnsavedChanges = true;
            }
            private void ParameterKeyBlocks_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_ParameterKey_Blocks = ParameterKeyBlocks.Value;
                HasUnsavedChanges = true;
            }
            private void DefaultDefinitionIsOptional_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefaultValue_DefinitionIsOptional = DefaultDefinitionIsOptional.Value;
                HasUnsavedChanges = true;
            }
            private void DefaultDefinitionHasValue_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefaultValue_DefinitionHasValue = DefaultDefinitionHasValue.Value;
                HasUnsavedChanges = true;
            }
            private void DefaultDefinitionIsAllowingBlank_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefaultValue_DefinitionIsAllowingBlank = DefaultDefinitionIsAllowingBlank.Value;
                HasUnsavedChanges = true;
            }
            private void DefaultDefinitionIsForceEscaped_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefaultValue_DefinitionIsForceEscaped = DefaultDefinitionIsForceEscaped.Value;
                HasUnsavedChanges = true;
            }
            private void DefaultDefinitionInfo_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefaultValue_DefinitionInfo = DefaultDefinitionInfo.Value;
                HasUnsavedChanges = true;
            }

            // public
            public override void PresetItems()
            {

            }
            public override void UpdateItems()
            {
                InterFileChecksActive.Value = UserSettings.Default.ItemHandlingSupport_InterFileChecksActive;
                ParameterKeyTemplateRoot.Value = UserSettings.Default.ItemHandlingSupport_ParameterKey_TemplateName;
                ParameterKeyBlocks.Value = UserSettings.Default.ItemHandlingSupport_ParameterKey_Blocks;
                DefaultDefinitionIsOptional.Value = UserSettings.Default.ItemHandlingSupport_DefaultValue_DefinitionIsOptional;
                DefaultDefinitionHasValue.Value = UserSettings.Default.ItemHandlingSupport_DefaultValue_DefinitionHasValue;
                DefaultDefinitionIsAllowingBlank.Value = UserSettings.Default.ItemHandlingSupport_DefaultValue_DefinitionIsAllowingBlank;
                DefaultDefinitionIsForceEscaped.Value = UserSettings.Default.ItemHandlingSupport_DefaultValue_DefinitionIsForceEscaped;
                DefaultDefinitionInfo.Value = UserSettings.Default.ItemHandlingSupport_DefaultValue_DefinitionInfo;
            }

            // privates
            private void InitControls()
            {
                SuspendLayout();

                AddSettingsItem(InterFileChecksActive, TitleRecources.EcfSettingsDialog_ItemHandlingSupport_InterFileChecksActive,
                    TextRecources.EcfSettingsDialog_ToolTip_InterFileChecksActive, InterFileChecksActive_SettingChanged);
                AddSettingsItem(ParameterKeyTemplateRoot, TitleRecources.EcfSettingsDialog_ItemHandlingSupport_ParameterKeyTemplateRoot, 
                    TextRecources.EcfSettingsDialog_ToolTip_ParameterKeyTemplateRoot, ParameterKeyTemplateRoot_SettingChanged);
                AddSettingsItem(ParameterKeyBlocks, TitleRecources.EcfSettingsDialog_ItemHandlingSupport_ParameterKeyBlocks,
                    TextRecources.EcfSettingsDialog_ToolTip_ParameterKeyBlocks, ParameterKeyBlocks_SettingChanged);
                AddSettingsItem(DefaultDefinitionIsOptional, TitleRecources.EcfSettingsDialog_ItemHandlingSupport_DefaultDefinitionIsOptional,
                    TextRecources.EcfSettingsDialog_ToolTip_DefaultDefinitionIsOptional, DefaultDefinitionIsOptional_SettingChanged);
                AddSettingsItem(DefaultDefinitionHasValue, TitleRecources.EcfSettingsDialog_ItemHandlingSupport_DefaultDefinitionHasValue,
                    TextRecources.EcfSettingsDialog_ToolTip_DefaultDefinitionHasValue, DefaultDefinitionIsAllowingBlank_SettingChanged);
                AddSettingsItem(DefaultDefinitionIsAllowingBlank, TitleRecources.EcfSettingsDialog_ItemHandlingSupport_DefaultDefinitionIaAllowingBlank,
                    TextRecources.EcfSettingsDialog_ToolTip_DefaultDefinitionIaAllowingBlank, DefaultDefinitionIsAllowingBlank_SettingChanged);
                AddSettingsItem(DefaultDefinitionIsForceEscaped, TitleRecources.EcfSettingsDialog_ItemHandlingSupport_DefaultDefinitionIsForceEscaped,
                    TextRecources.EcfSettingsDialog_ToolTip_DefaultDefinitionIsForceEscaped, DefaultDefinitionIsForceEscaped_SettingChanged);
                AddSettingsItem(DefaultDefinitionInfo, TitleRecources.EcfSettingsDialog_ItemHandlingSupport_DefaultDefinitionInfo,
                    TextRecources.EcfSettingsDialog_ToolTip_DefaultDefinitionInfo, DefaultDefinitionInfo_SettingChanged);
                AddSpacer();

                ResumeLayout(true);
            }
        }
        private class InfoPanel : SettingsPanel
        {
            private TitleSettingsItem AppNameItem { get; } = new TitleSettingsItem();
            private LogoSettingsItem LogoItem { get; } = new LogoSettingsItem();
            private TextSettingsItem AuthorItem { get; } = new TextSettingsItem();
            private TextSettingsItem VersionItem { get; } = new TextSettingsItem();
            private LinkSettingsItem LicenseItem { get; } = new LinkSettingsItem();
            private LinkSettingsItem ReadmeItem { get; } = new LinkSettingsItem();

            public InfoPanel(string idTag) : base(idTag, null)
            {
                InitControls();
            }

            // events
            private void LicenseItem_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt)
            {
                Process.Start(Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTrademarkAttribute), false)
                    .Cast<AssemblyTrademarkAttribute>().FirstOrDefault().Trademark);
            }
            private void ReadmeItem_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt)
            {
                Process.Start(Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
                    .Cast<AssemblyDescriptionAttribute>().FirstOrDefault().Description);
            }

            // public
            public override void PresetItems()
            {
                
            }
            public override void UpdateItems()
            {
                
            }

            // privates
            private void InitControls()
            {
                SuspendLayout();
                
                AppNameItem.Text = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false)
                    .Cast<AssemblyTitleAttribute>().FirstOrDefault().Title;
                AppNameItem.TagFont = new Font("Viner Hand ITC", 20.25F, FontStyle.Bold, GraphicsUnit.Point);

                LogoItem.Image = new Icon(IconRecources.Icon_AppBranding, 256, 256).ToBitmap();

                AuthorItem.Text = TitleRecources.Generic_Author;
                AuthorItem.NameTagAlign = ContentAlignment.MiddleRight;
                AuthorItem.Value = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false)
                    .Cast<AssemblyCompanyAttribute>().FirstOrDefault().Company;

                VersionItem.Text = TitleRecources.Generic_Version;
                VersionItem.NameTagAlign = ContentAlignment.MiddleRight;
                VersionItem.Value = Assembly.GetExecutingAssembly().GetName().Version.ToString(); ;

                LicenseItem.Text = TitleRecources.Generic_License;
                LicenseItem.NameTagAlign = ContentAlignment.MiddleRight;
                LicenseItem.Value = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)
                    .Cast<AssemblyCopyrightAttribute>().FirstOrDefault().Copyright;
                LicenseItem.LinkClicked += LicenseItem_LinkClicked;

                ReadmeItem.Text = TitleRecources.Generic_Manual;
                ReadmeItem.NameTagAlign = ContentAlignment.MiddleRight;
                ReadmeItem.Value = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false)
                    .Cast<AssemblyConfigurationAttribute>().FirstOrDefault().Configuration;
                ReadmeItem.LinkClicked += ReadmeItem_LinkClicked;

                AddSettingsItem(AppNameItem);
                AddSpacer(LogoItem);
                AddSettingsItem(AuthorItem);
                AddSettingsItem(VersionItem);
                AddSettingsItem(LicenseItem);
                AddSettingsItem(ReadmeItem);

                ResumeLayout(true);
            }
        }

        // item classes
        private abstract class SettingsTableItem : TableLayoutPanel
        {
            public override string Text
            {
                get
                {
                    return NameTag.Text;
                }
                set
                {
                    NameTag.Text = value;
                }
            }
            public abstract bool IsEnabled { get; set; }
            public ContentAlignment NameTagAlign
            {
                get
                {
                    return NameTag.TextAlign;
                }
                set
                {
                    NameTag.TextAlign = value;
                }
            }
            public ContentAlignment DataTagAlign
            {
                get
                {
                    return NameTag.TextAlign;
                }
                set
                {
                    NameTag.TextAlign = value;
                }
            }
            public Font TagFont
            {
                get
                {
                    return GetFont();
                }
                set
                {
                    NameTag.Font = value;
                    OnUpdateFont(value);
                }
            }

            protected Label NameTag { get; } = new Label();

            public SettingsTableItem()
            {
                SuspendLayout();

                AutoSize = true;
                ColumnCount = 2;
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                Controls.Add(NameTag, 0, 0);
                Dock = DockStyle.Fill;
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
                RowCount = 1;
                RowStyles.Add(new RowStyle());

                NameTag.AutoSize = true;
                NameTag.Dock = DockStyle.Fill;
                NameTag.Padding = new Padding(3);
                NameTag.TextAlign = ContentAlignment.MiddleLeft;
                
                ResumeLayout(true);
            }

            // public
            public abstract void SetToolTip(ToolTip toolTipContainer, string toolTip);

            // privates
            protected abstract void OnUpdateFont(Font font);
            protected abstract void OnUpdateDataTagAlign(ContentAlignment align);
            protected abstract Font GetFont();
            protected abstract ContentAlignment GetDataTagAlign();
        }
        private class TitleSettingsItem : SettingsTableItem
        {
            public override bool IsEnabled
            {
                get
                {
                    return NameTag.Enabled;
                }
                set
                {
                    NameTag.Enabled = value;
                }
            }

            public TitleSettingsItem() : base()
            {
                SetColumnSpan(NameTag, 2);

                NameTag.Padding = new Padding(5);
                NameTag.TextAlign = ContentAlignment.MiddleCenter;
            }

            // publics
            public override void SetToolTip(ToolTip toolTipContainer, string toolTip)
            {
                toolTipContainer?.SetToolTip(NameTag, toolTip);
            }

            // privates
            protected override void OnUpdateFont(Font font)
            {

            }
            protected override void OnUpdateDataTagAlign(ContentAlignment align)
            {

            }
            protected override Font GetFont()
            {
                return NameTag.Font;
            }
            protected override ContentAlignment GetDataTagAlign()
            {
                return NameTag.TextAlign;
            }
        }
        private class LogoSettingsItem : SettingsTableItem
        {
            public override bool IsEnabled
            {
                get
                {
                    return Logo.Enabled;
                }
                set
                {
                    Logo.Enabled = value;
                }
            }
            public Image Image
            {
                get
                {
                    return Logo.Image;
                }
                set
                {
                    Logo.Image = value;
                }
            }

            private PictureBox Logo { get; } = new PictureBox();

            public LogoSettingsItem() : base()
            {
                Controls.Remove(NameTag);

                Controls.Add(Logo);
                SetColumnSpan(Logo, 2);
                Logo.Dock = DockStyle.Fill;
                Logo.SizeMode = PictureBoxSizeMode.CenterImage;
            }

            // publics
            public override void SetToolTip(ToolTip toolTipContainer, string toolTip)
            {
                toolTipContainer?.SetToolTip(Logo, toolTip);
            }

            // privates
            protected override void OnUpdateFont(Font font)
            {

            }
            protected override void OnUpdateDataTagAlign(ContentAlignment align)
            {

            }
            protected override Font GetFont()
            {
                return NameTag.Font;
            }
            protected override ContentAlignment GetDataTagAlign()
            {
                return NameTag.TextAlign;
            }
        }
        private class TextSettingsItem : SettingsTableItem
        {
            public override bool IsEnabled
            {
                get
                {
                    return DataTag.Enabled;
                }
                set
                {
                    DataTag.Enabled = value;
                }
            }
            public string Value
            {
                get
                {
                    return DataTag.Text;
                }
                set
                {
                    DataTag.Text = value;
                }
            }

            private Label DataTag { get; } = new Label();

            public TextSettingsItem() : base()
            {
                Controls.Add(DataTag, 1, 0);
                DataTag.AutoSize = true;
                DataTag.Dock = DockStyle.Fill;
                DataTag.Padding = new Padding(3);
                DataTag.TextAlign = ContentAlignment.MiddleLeft;
            }

            // publics
            public override void SetToolTip(ToolTip toolTipContainer, string toolTip)
            {
                toolTipContainer?.SetToolTip(DataTag, toolTip);
            }

            // privates
            protected override void OnUpdateFont(Font font)
            {
                DataTag.Font = font;
            }
            protected override void OnUpdateDataTagAlign(ContentAlignment align)
            {
                DataTag.TextAlign = align;
            }
            protected override Font GetFont()
            {
                return DataTag.Font;
            }
            protected override ContentAlignment GetDataTagAlign()
            {
                return DataTag.TextAlign;
            }
        }
        private class LinkSettingsItem : SettingsTableItem
        {
            public event LinkLabelLinkClickedEventHandler LinkClicked;

            public override bool IsEnabled
            {
                get
                {
                    return DataTag.Enabled;
                }
                set
                {
                    DataTag.Enabled = value;
                }
            }
            public string Value
            {
                get
                {
                    return DataTag.Text;
                }
                set
                {
                    DataTag.Text = value;
                }
            }

            private LinkLabel DataTag { get; } = new LinkLabel();

            public LinkSettingsItem() : base()
            {
                Controls.Add(DataTag, 1, 0);
                DataTag.AutoSize = true;
                DataTag.Dock = DockStyle.Fill;
                DataTag.Padding = new Padding(3);
                DataTag.TextAlign = ContentAlignment.MiddleLeft;
                DataTag.LinkClicked += DataTag_LinkClicked;
            }

            // events
            private void DataTag_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt)
            {
                LinkClicked?.Invoke(sender, evt);
            }

            // publics
            public override void SetToolTip(ToolTip toolTipContainer, string toolTip)
            {
                toolTipContainer?.SetToolTip(DataTag, toolTip);
            }

            // privates
            protected override void OnUpdateFont(Font font)
            {
                DataTag.Font = font;
            }
            protected override void OnUpdateDataTagAlign(ContentAlignment align)
            {
                DataTag.TextAlign = align;
            }
            protected override Font GetFont()
            {
                return DataTag.Font;
            }
            protected override ContentAlignment GetDataTagAlign()
            {
                return DataTag.TextAlign;
            }
        }
        private abstract class SettingsItem : SettingsTableItem
        {
            public abstract event EventHandler SettingChanged;

            public SettingsItem() : base()
            {
                
            }
        }
        private class TextBoxSettingItem : SettingsItem
        {
            public override event EventHandler SettingChanged;

            public override bool IsEnabled
            {
                get
                {
                    return Setting.Enabled;
                }
                set
                {
                    Setting.Enabled = value;
                }
            }
            public string Value
            {
                get
                {
                    return Setting.Text;
                }
                set
                {
                    IsSuspendingEvents = true;
                    Setting.Text = value;
                    IsSuspendingEvents = false;
                }
            }

            private bool IsSuspendingEvents { get; set; } = false;

            private TextBox Setting { get; } = new TextBox();

            public TextBoxSettingItem() : base()
            {
                Controls.Add(Setting, 1, 0);

                Setting.Dock = DockStyle.Fill;
                Setting.TextChanged += Setting_TextChanged;
            }

            // events
            private void Setting_TextChanged(object sender, EventArgs evt)
            {
                if (!IsSuspendingEvents)
                {
                    SettingChanged?.Invoke(sender, evt);
                }
            }

            // public
            public override void SetToolTip(ToolTip toolTipContainer, string toolTip)
            {
                toolTipContainer?.SetToolTip(Setting, toolTip);
            }

            // privates
            protected override void OnUpdateFont(Font font)
            {
                Setting.Font = font;
            }
            protected override void OnUpdateDataTagAlign(ContentAlignment align)
            {
                switch (align)
                {
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.TopLeft:
                        Setting.TextAlign = HorizontalAlignment.Left;
                        break;
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.TopCenter:
                        Setting.TextAlign = HorizontalAlignment.Center;
                        break;
                    case ContentAlignment.BottomRight:
                    case ContentAlignment.MiddleRight:
                    case ContentAlignment.TopRight:
                        Setting.TextAlign = HorizontalAlignment.Right;
                        break;
                    default:
                        break;
                }
            }
            protected override Font GetFont()
            {
                return Setting.Font;
            }
            protected override ContentAlignment GetDataTagAlign()
            {
                switch (Setting.TextAlign)
                {
                    case HorizontalAlignment.Left: return ContentAlignment.MiddleLeft;
                    case HorizontalAlignment.Center: return ContentAlignment.MiddleCenter;
                    case HorizontalAlignment.Right: return ContentAlignment.MiddleRight;
                    default: return ContentAlignment.MiddleLeft;
                }
            }
        }
        private class ComboBoxSettingItem : SettingsItem
        {
            public override event EventHandler SettingChanged;

            public override bool IsEnabled
            {
                get
                {
                    return Setting.Enabled;
                }
                set
                {
                    Setting.Enabled = value;
                }
            }
            public object Value
            {
                get
                {
                    return Setting.SelectedItem;
                }
                set
                {
                    Setting.SelectedItem = value;
                    if (Setting.SelectedIndex < 0) { Setting.SelectedIndex = 0; }
                }
            }

            private ComboBox Setting { get; } = new ComboBox();

            public ComboBoxSettingItem() : base()
            {
                Controls.Add(Setting, 1, 0);

                Setting.Dock = DockStyle.Fill;
                Setting.DropDownStyle = ComboBoxStyle.DropDownList;
                Setting.FormattingEnabled = true;
                Setting.Sorted = true;

                Setting.SelectionChangeCommitted += Setting_SelectionChangeCommitted;
            }

            // events
            private void Setting_SelectionChangeCommitted(object sender, EventArgs evt)
            {
                SettingChanged?.Invoke(sender, evt);
            }

            // publics
            public override void SetToolTip(ToolTip toolTipContainer, string toolTip)
            {
                toolTipContainer?.SetToolTip(Setting, toolTip);
            }
            public void PresetItem(object[] entries)
            {
                Setting.BeginUpdate();
                Setting.Items.Clear();
                Setting.Items.AddRange(entries);
                Setting.EndUpdate();
            }

            // privates
            protected override void OnUpdateFont(Font font)
            {
                Setting.Font = font;
            }
            protected override void OnUpdateDataTagAlign(ContentAlignment align)
            {
                
            }
            protected override Font GetFont()
            {
                return Setting.Font;
            }
            protected override ContentAlignment GetDataTagAlign()
            {
                return ContentAlignment.MiddleLeft;
            }
        }
        private class CheckBoxSettingItem : SettingsItem
        {
            public override event EventHandler SettingChanged;

            public override bool IsEnabled
            {
                get
                {
                    return Setting.Enabled;
                }
                set
                {
                    Setting.Enabled = value;
                }
            }
            public bool Value
            {
                get
                {
                    return Setting.Checked;
                }
                set
                {
                    Setting.Checked = value;
                }
            }

            private CheckBox Setting { get; } = new CheckBox();

            public CheckBoxSettingItem() : base()
            {
                Controls.Add(Setting, 1, 0);

                Setting.AutoSize = true;
                Setting.Dock = DockStyle.Fill;
                Setting.UseVisualStyleBackColor = true;
                Setting.Click += Setting_Click;
            }

            // events
            private void Setting_Click(object sender, EventArgs evt)
            {
                SettingChanged?.Invoke(sender, evt);
            }

            // publics
            public override void SetToolTip(ToolTip toolTipContainer, string toolTip)
            {
                toolTipContainer?.SetToolTip(Setting, toolTip);
            }

            // privates
            protected override void OnUpdateFont(Font font)
            {
                
            }
            protected override void OnUpdateDataTagAlign(ContentAlignment align)
            {
                Setting.CheckAlign = align;
            }
            protected override Font GetFont()
            {
                return NameTag.Font;
            }
            protected override ContentAlignment GetDataTagAlign()
            {
                return Setting.CheckAlign;
            }
        }
        private class NumericSettingItem : SettingsItem
        {
            public override event EventHandler SettingChanged;

            public override bool IsEnabled
            {
                get
                {
                    return Setting.Enabled;
                }
                set
                {
                    Setting.Enabled = value;
                }
            }
            public decimal Value
            {
                get
                {
                    return Setting.Value;
                }
                set
                {
                    IsSuspendingEvents = true;
                    Setting.Value = value;
                    IsSuspendingEvents = false;
                }
            }
            public decimal Minimum
            {
                get
                {
                    return Setting.Minimum;
                }
                set
                {
                    IsSuspendingEvents = true;
                    Setting.Minimum = value;
                    IsSuspendingEvents = false;
                }
            }
            public decimal Maximum
            {
                get
                {
                    return Setting.Maximum;
                }
                set
                {
                    IsSuspendingEvents = true;
                    Setting.Maximum = value;
                    IsSuspendingEvents = false;
                }
            }
            
            private bool IsSuspendingEvents { get; set; } = false;

            private NumericUpDown Setting { get; } = new NumericUpDown();

            public NumericSettingItem() : base()
            {
                Controls.Add(Setting, 1, 0);

                Setting.AutoSize = true;
                Setting.Dock = DockStyle.Fill;

                Setting.ValueChanged += Setting_ValueChanged;
            }

            // events
            private void Setting_ValueChanged(object sender, EventArgs evt)
            {
                if (!IsSuspendingEvents)
                {
                    SettingChanged?.Invoke(sender, evt);
                }
            }

            // publics
            public override void SetToolTip(ToolTip toolTipContainer, string toolTip)
            {
                toolTipContainer?.SetToolTip(Setting, toolTip);
            }

            // privates
            protected override void OnUpdateFont(Font font)
            {
                Setting.Font = font;
            }
            protected override void OnUpdateDataTagAlign(ContentAlignment align)
            {
                switch (align)
                {
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.TopLeft:
                        Setting.TextAlign = HorizontalAlignment.Left;
                        break;
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.TopCenter:
                        Setting.TextAlign = HorizontalAlignment.Center;
                        break;
                    case ContentAlignment.BottomRight:
                    case ContentAlignment.MiddleRight:
                    case ContentAlignment.TopRight:
                        Setting.TextAlign = HorizontalAlignment.Right;
                        break;
                    default:
                        break;
                }
            }
            protected override Font GetFont()
            {
                return Setting.Font;
            }
            protected override ContentAlignment GetDataTagAlign()
            {
                switch (Setting.TextAlign)
                {
                    case HorizontalAlignment.Left: return ContentAlignment.MiddleLeft;
                    case HorizontalAlignment.Center: return ContentAlignment.MiddleCenter;
                    case HorizontalAlignment.Right: return ContentAlignment.MiddleRight;
                    default: return ContentAlignment.MiddleLeft;
                }
            }
        }
        
    }
}
