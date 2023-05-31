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
            Text = TitleRecources.SettingsDialog_Header;

            InitControls();
        }
        private void EcfSettingsDialog_FormClosing(object sender, FormClosingEventArgs evt)
        {
            if (HasUnsavedChanges())
            {
                if (MessageBox.Show(this, TextRecources.SettingsDialog_UnsavedReallyCloseQuestion, 
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
            if (MessageBox.Show(this, TextRecources.SettingsDialog_ReallyResetAllQuestion, 
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

            SettingsPanels.Add(new GeneralSettingsPanel(TitleRecources.SettingsDialog_General_Header, Tip));
            SettingsPanels.Add(new CreationSettingsPanel(TitleRecources.SettingsDialog_Creation_Header, Tip));
            SettingsPanels.Add(new TreeFilterSettingsPanel(TitleRecources.SettingsDialog_Filter_Header, Tip));
            SettingsPanels.Add(new SorterSettingsPanel(TitleRecources.SettingsDialog_Sorter_Header, Tip));
            SettingsPanels.Add(new TechTreeSettingsPanel(TitleRecources.SettingsDialog_TechTree_Header, Tip));
            SettingsPanels.Add(new ItemHandlingSupportSettingsPanel(TitleRecources.SettingsDialog_ItemHandlingSupport_Header, Tip));
            SettingsPanels.Add(new InfoPanel(TitleRecources.SettingsDialog_Info_Header));

            ChapterSelectorTreeView.Nodes.Add(TitleRecources.SettingsDialog_General_Header);
            ChapterSelectorTreeView.Nodes.Add(TitleRecources.SettingsDialog_Creation_Header);
            ChapterSelectorTreeView.Nodes.Add(TitleRecources.SettingsDialog_Filter_Header);
            ChapterSelectorTreeView.Nodes.Add(TitleRecources.SettingsDialog_Sorter_Header);
            ChapterSelectorTreeView.Nodes.Add(TitleRecources.SettingsDialog_TechTree_Header);
            ChapterSelectorTreeView.Nodes.Add(TitleRecources.SettingsDialog_ItemHandlingSupport_Header);
            ChapterSelectorTreeView.Nodes.Add(TitleRecources.SettingsDialog_Info_Header);

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

                AutoScroll = true;
                AutoSize = true;
                ColumnCount = 1;
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                Dock = DockStyle.Fill;
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
                HorizontalScroll.Enabled = false;
                // hack to give autoscroll a suitable startvalue to calculate scrollbar size correct
                Size = new Size(1, 1);

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
                RowStyles.Add(new RowStyle(SizeType.AutoSize));
                ItemsCount++;
                RowCount = ItemsCount;
            }
            protected void AddSpacer(SettingsTableItem item)
            {
                item.Dock = DockStyle.Fill;
                Controls.Add(item, 0, ItemsCount);
                ItemsCount++;
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

                AddSettingsItem(GameMode, TitleRecources.SettingsDialog_General_GameMode,
                    TextRecources.SettingsDialog_ToolTip_GameMode, GameMode_SettingChanged);

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

                AddSettingsItem(WriteOnlyValidItems, TitleRecources.SettingsDialog_Creation_WriteOnlyValidItems,
                    TextRecources.SettingsDialog_ToolTip_WriteOnlyValidItems, WriteOnlyValidItems_SettingChanged);
                AddSettingsItem(InvalidateParentsOnError, TitleRecources.SettingsDialog_Creation_InvalidateParentOnError,
                    TextRecources.SettingsDialog_ToolTip_InvalidateParentsOnError, InvalidateParentsOnError_SettingChanged);
                AddSettingsItem(AllowFallbackToParsedData, TitleRecources.SettingsDialog_Creation_AllowFallbackToParsedData,
                    TextRecources.SettingsDialog_ToolTip_AllowFallbackToParsedData, AllowFallbackToParsedData_SettingChanged);

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

                AddSettingsItem(CommentsInitActive, TitleRecources.SettingsDialog_Filter_TreeViewFilterCommentsInitActive,
                    TextRecources.SettingsDialog_ToolTip_TreeViewFilterCommentsInitActive, CommentsInitActive_SettingChanged);
                AddSettingsItem(ParametersInitActive, TitleRecources.SettingsDialog_Filter_TreeViewFilterParametersInitActive,
                    TextRecources.SettingsDialog_ToolTip_TreeViewFilterParametersInitActive, ParametersInitActive_SettingChanged);
                AddSettingsItem(DataBlocksInitActive, TitleRecources.SettingsDialog_Filter_TreeViewFilterDataBlocksInitActive,
                    TextRecources.SettingsDialog_ToolTip_TreeViewFilterDataBlocksInitActive, DataBlocksInitActive_SettingChanged);

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

                AddSettingsItem(TreeViewSorterInitCount, TitleRecources.SettingsDialog_Sorter_TreeViewSorterInitCount,
                    TextRecources.SettingsDialog_ToolTip_TreeViewSorterInitCount, TreeViewSorterInitCount_SettingChanged);
                AddSettingsItem(ParameterViewSorterInitCount, TitleRecources.SettingsDialog_Sorter_ParameterViewSorterInitCount,
                    TextRecources.SettingsDialog_ToolTip_ParameterViewSorterInitCount, ParameterViewSorterInitCount_SettingChanged);
                AddSettingsItem(ErrorViewSorterInitCount, TitleRecources.SettingsDialog_Sorter_ErrorViewSorterInitCount,
                    TextRecources.SettingsDialog_ToolTip_ErrorViewSorterInitCount, ErrorViewSorterInitCount_SettingChanged);

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
                UserSettings.Default.EcfTechTreeDialog_ParamKey_TechTreeNames = ParameterKeyTechTreeNames.Value;
                HasUnsavedChanges = true;
            }
            private void ParameterKeyTechTreeParentName_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EcfTechTreeDialog_ParamKey_TechTreeParentName = ParameterKeyTechTreeParentName.Value;
                HasUnsavedChanges = true;
            }
            private void ParameterKeyUnlockLevel_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EcfTechTreeDialog_ParamKey_UnlockLevel = ParameterKeyUnlockLevel.Value;
                HasUnsavedChanges = true;
            }
            private void DefaultValueUnlockLevel_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EcfTechTreeDialog_DefVal_UnlockLevel = Convert.ToInt32(DefaultValueUnlockLevel.Value);
                HasUnsavedChanges = true;
            }
            private void ParameterKeyUnlockCost_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EcfTechTreeDialog_ParamKey_UnlockCost = ParameterKeyUnlockCost.Value;
                HasUnsavedChanges = true;
            }
            private void DefaultValueUnlockCost_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.EcfTechTreeDialog_DefVal_UnlockCost = Convert.ToInt32(DefaultValueUnlockCost.Value);
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

                ParameterKeyTechTreeNames.Value = UserSettings.Default.EcfTechTreeDialog_ParamKey_TechTreeNames;
                ParameterKeyTechTreeParentName.Value = UserSettings.Default.EcfTechTreeDialog_ParamKey_TechTreeParentName;
                ParameterKeyUnlockLevel.Value = UserSettings.Default.EcfTechTreeDialog_ParamKey_UnlockLevel;
                DefaultValueUnlockLevel.Value = UserSettings.Default.EcfTechTreeDialog_DefVal_UnlockLevel;
                ParameterKeyUnlockCost.Value = UserSettings.Default.EcfTechTreeDialog_ParamKey_UnlockCost;
                DefaultValueUnlockCost.Value = UserSettings.Default.EcfTechTreeDialog_DefVal_UnlockCost;
            }

            // private
            private void InitControls()
            {
                SuspendLayout();

                AddSettingsItem(ParameterKeyTechTreeNames, TitleRecources.SettingsDialog_TechTree_ParamKey_TechTreeNames,
                    TextRecources.SettingsDialog_ToolTip_ParamKey_TechTreeNames, ParameterKeyTechTreeNames_SettingChanged);
                AddSettingsItem(ParameterKeyTechTreeParentName, TitleRecources.SettingsDialog_TechTree_ParamKey_TechTreeParentName,
                    TextRecources.SettingsDialog_ToolTip_ParamKey_TechTreeParentName, ParameterKeyTechTreeParentName_SettingChanged);
                AddSettingsItem(ParameterKeyUnlockLevel, TitleRecources.SettingsDialog_TechTree_ParamKey_UnlockLevel,
                    TextRecources.SettingsDialog_ToolTip_ParamKey_UnlockLevel, ParameterKeyUnlockLevel_SettingChanged);
                AddSettingsItem(DefaultValueUnlockLevel, TitleRecources.SettingsDialog_TechTree_DefVal_UnlockLevel,
                    TextRecources.SettingsDialog_ToolTip_DefVal_UnlockLevel, DefaultValueUnlockLevel_SettingChanged);
                AddSettingsItem(ParameterKeyUnlockCost, TitleRecources.SettingsDialog_TechTree_ParamKey_UnlockCost,
                    TextRecources.SettingsDialog_ToolTip_ParamKey_UnlockCost, ParameterKeyUnlockCost_SettingChanged);
                AddSettingsItem(DefaultValueUnlockCost, TitleRecources.SettingsDialog_TechTree_DefVal_UnlockCost,
                    TextRecources.SettingsDialog_ToolTip_DefVal_UnlockCost, DefaultValueUnlockCost_SettingChanged);

                ResumeLayout(true);
            }
        }
        private class ItemHandlingSupportSettingsPanel : SettingsPanel
        {
            private CheckBoxSettingItem InterFileChecksActive { get; } = new CheckBoxSettingItem();
            private TextBoxSettingItem ParamKey_TemplateRoot { get; } = new TextBoxSettingItem();
            private CheckBoxSettingItem DefVal_Ingredient_DefIsOptional { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefVal_Ingredient_DefHasValue { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefVal_Ingredient_DefIsAllowingBlank { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefVal_Ingredient_DefIsForceEscaped { get; } = new CheckBoxSettingItem();
            private TextBoxSettingItem DefVal_Ingredient_DefInfo { get; } = new TextBoxSettingItem();
            private TextBoxSettingItem ParamKey_Blocks { get; } = new TextBoxSettingItem();
            private TextBoxSettingItem ParamKeys_GlobalRef { get; } = new TextBoxSettingItem();
            private CheckBoxSettingItem DefVal_GlobalDefParam_DefIsOptional { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefVal_GlobalDefParam_DefHasValue { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefVal_GlobalDefParam_DefIsAllowingBlank { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefVal_GlobalDefParam_DefIsForceEscaped { get; } = new CheckBoxSettingItem();
            private TextBoxSettingItem DefVal_GlobalDefParam_DefInfo { get; } = new TextBoxSettingItem();
            private CheckBoxSettingItem DefVal_GlobalDefAttr_DefIsOptional { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefVal_GlobalDefAttr_DefHasValue { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefVal_GlobalDefAttr_DefIsAllowingBlank { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefVal_GlobalDefAttr_DefIsForceEscaped { get; } = new CheckBoxSettingItem();
            private TextBoxSettingItem DefVal_GlobalDefAttr_DefInfo { get; } = new TextBoxSettingItem();
            private CheckBoxSettingItem DefVal_FileParam_DefIsOptional { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefVal_FileParam_DefHasValue { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefVal_FileParam_DefIsAllowingBlank { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefVal_FileParam_DefIsForceEscaped { get; } = new CheckBoxSettingItem();
            private TextBoxSettingItem DefVal_FileParam_DefInfo { get; } = new TextBoxSettingItem();

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
            private void ParamKey_TemplateRoot_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_ParamKey_TemplateName = ParamKey_TemplateRoot.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_Ingredient_DefIsOptional_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_Ingredient_DefIsOptional = DefVal_Ingredient_DefIsOptional.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_Ingredient_DefHasValue_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_Ingredient_DefHasValue = DefVal_Ingredient_DefHasValue.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_Ingredient_DefIsAllowingBlank_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_Ingredient_DefIsAllowingBlank = DefVal_Ingredient_DefIsAllowingBlank.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_Ingredient_DefIsForceEscaped_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_Ingredient_DefIsForceEscaped = DefVal_Ingredient_DefIsForceEscaped.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_Ingredient_DefInfo_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_Ingredient_DefInfo = DefVal_Ingredient_DefInfo.Value;
                HasUnsavedChanges = true;
            }
            private void ParamKey_Blocks_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_ParamKey_Blocks = ParamKey_Blocks.Value;
                HasUnsavedChanges = true;
            }
            private void ParamKeys_GlobalRef_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_ParamKeys_GlobalRef = ParamKeys_GlobalRef.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_GlobalDefParam_DefIsOptional_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefParam_DefIsOptional = DefVal_GlobalDefParam_DefIsOptional.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_GlobalDefParam_DefHasValue_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefParam_DefHasValue = DefVal_GlobalDefParam_DefHasValue.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_GlobalDefParam_DefIsAllowingBlank_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefParam_DefIsAllowingBlank = DefVal_GlobalDefParam_DefIsAllowingBlank.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_GlobalDefParam_DefIsForceEscaped_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefParam_DefIsForceEscaped = DefVal_GlobalDefParam_DefIsForceEscaped.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_GlobalDefParam_DefInfo_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefParam_DefInfo = DefVal_GlobalDefParam_DefInfo.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_GlobalDefAttr_DefIsOptional_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefAttr_DefIsOptional = DefVal_GlobalDefAttr_DefIsOptional.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_GlobalDefAttr_DefHasValue_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefAttr_DefHasValue = DefVal_GlobalDefAttr_DefHasValue.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_GlobalDefAttr_DefIsAllowingBlank_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefAttr_DefIsAllowingBlank = DefVal_GlobalDefAttr_DefIsAllowingBlank.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_GlobalDefAttr_DefIsForceEscaped_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefAttr_DefIsForceEscaped = DefVal_GlobalDefAttr_DefIsForceEscaped.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_GlobalDefAttr_DefInfo_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefAttr_DefInfo = DefVal_GlobalDefAttr_DefInfo.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_FileParam_DefIsOptional_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_FileParam_DefIsOptional = DefVal_FileParam_DefIsOptional.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_FileParam_DefHasValue_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_FileParam_DefHasValue = DefVal_FileParam_DefHasValue.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_FileParam_DefIsAllowingBlank_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_FileParam_DefIsAllowingBlank = DefVal_FileParam_DefIsAllowingBlank.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_FileParam_DefIsForceEscaped_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_FileParam_DefIsForceEscaped = DefVal_FileParam_DefIsForceEscaped.Value;
                HasUnsavedChanges = true;
            }
            private void DefVal_FileParam_DefInfo_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_DefVal_FileParam_DefInfo = DefVal_FileParam_DefInfo.Value;
                HasUnsavedChanges = true;
            }

            // public
            public override void PresetItems()
            {

            }
            public override void UpdateItems()
            {
                InterFileChecksActive.Value = UserSettings.Default.ItemHandlingSupport_InterFileChecksActive;
                ParamKey_TemplateRoot.Value = UserSettings.Default.ItemHandlingSupport_ParamKey_TemplateName;
                DefVal_Ingredient_DefIsOptional.Value = UserSettings.Default.ItemHandlingSupport_DefVal_Ingredient_DefIsOptional;
                DefVal_Ingredient_DefHasValue.Value = UserSettings.Default.ItemHandlingSupport_DefVal_Ingredient_DefHasValue;
                DefVal_Ingredient_DefIsAllowingBlank.Value = UserSettings.Default.ItemHandlingSupport_DefVal_Ingredient_DefIsAllowingBlank;
                DefVal_Ingredient_DefIsForceEscaped.Value = UserSettings.Default.ItemHandlingSupport_DefVal_Ingredient_DefIsForceEscaped;
                DefVal_Ingredient_DefInfo.Value = UserSettings.Default.ItemHandlingSupport_DefVal_Ingredient_DefInfo;
                ParamKey_Blocks.Value = UserSettings.Default.ItemHandlingSupport_ParamKey_Blocks;
                ParamKeys_GlobalRef.Value = UserSettings.Default.ItemHandlingSupport_ParamKeys_GlobalRef;
                DefVal_GlobalDefParam_DefIsOptional.Value = UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefParam_DefIsOptional;
                DefVal_GlobalDefParam_DefHasValue.Value = UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefParam_DefHasValue;
                DefVal_GlobalDefParam_DefIsAllowingBlank.Value = UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefParam_DefIsAllowingBlank;
                DefVal_GlobalDefParam_DefIsForceEscaped.Value = UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefParam_DefIsForceEscaped;
                DefVal_GlobalDefParam_DefInfo.Value = UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefParam_DefInfo;
                DefVal_GlobalDefAttr_DefIsOptional.Value = UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefAttr_DefIsOptional;
                DefVal_GlobalDefAttr_DefHasValue.Value = UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefAttr_DefHasValue;
                DefVal_GlobalDefAttr_DefIsAllowingBlank.Value = UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefAttr_DefIsAllowingBlank;
                DefVal_GlobalDefAttr_DefIsForceEscaped.Value = UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefAttr_DefIsForceEscaped;
                DefVal_GlobalDefAttr_DefInfo.Value = UserSettings.Default.ItemHandlingSupport_DefVal_GlobalDefAttr_DefInfo;
                DefVal_FileParam_DefIsOptional.Value = UserSettings.Default.ItemHandlingSupport_DefVal_FileParam_DefIsOptional;
                DefVal_FileParam_DefHasValue.Value = UserSettings.Default.ItemHandlingSupport_DefVal_FileParam_DefHasValue;
                DefVal_FileParam_DefIsAllowingBlank.Value = UserSettings.Default.ItemHandlingSupport_DefVal_FileParam_DefIsAllowingBlank;
                DefVal_FileParam_DefIsForceEscaped.Value = UserSettings.Default.ItemHandlingSupport_DefVal_FileParam_DefIsForceEscaped;
                DefVal_FileParam_DefInfo.Value = UserSettings.Default.ItemHandlingSupport_DefVal_FileParam_DefInfo;
            }

            // privates
            private void InitControls()
            {
                SuspendLayout();

                AddSettingsItem(new TitleItem()
                {
                    Text = TitleRecources.SettingsDialog_ItemHandlingSupport_General_Header,
                    TagFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold),
                });
                AddSettingsItem(InterFileChecksActive, TitleRecources.SettingsDialog_ItemHandlingSupport_InterFileChecksActive,
                    TextRecources.SettingsDialog_ToolTip_InterFileChecksActive, InterFileChecksActive_SettingChanged);

                AddSettingsItem(new TitleItem()
                {
                    Text = TitleRecources.SettingsDialog_ItemHandlingSupport_TemplatesConfig_Header,
                    TagFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold),
                });
                AddSettingsItem(ParamKey_TemplateRoot, TitleRecources.SettingsDialog_ItemHandlingSupport_ParamKey_TemplateRoot, 
                    TextRecources.SettingsDialog_ToolTip_ParamKey_TemplateRoot, ParamKey_TemplateRoot_SettingChanged);
                AddSettingsItem(DefVal_Ingredient_DefIsOptional, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_Ingredient_DefIsOptional,
                    TextRecources.SettingsDialog_ToolTip_DefVal_Ingredient_DefIsOptional, DefVal_Ingredient_DefIsOptional_SettingChanged);
                AddSettingsItem(DefVal_Ingredient_DefHasValue, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_Ingredient_DefHasValue,
                    TextRecources.SettingsDialog_ToolTip_DefVal_Ingredient_DefHasValue, DefVal_Ingredient_DefHasValue_SettingChanged);
                AddSettingsItem(DefVal_Ingredient_DefIsAllowingBlank, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_Ingredient_DefIsAllowingBlank,
                    TextRecources.SettingsDialog_ToolTip_DefVal_Ingredient_DefIsAllowingBlank, DefVal_Ingredient_DefIsAllowingBlank_SettingChanged);
                AddSettingsItem(DefVal_Ingredient_DefIsForceEscaped, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_Ingredient_DefIsForceEscaped,
                    TextRecources.SettingsDialog_ToolTip_DefVal_Ingredient_DefIsForceEscaped, DefVal_Ingredient_DefIsForceEscaped_SettingChanged);
                AddSettingsItem(DefVal_Ingredient_DefInfo, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_Ingredient_DefInfo,
                    TextRecources.SettingsDialog_ToolTip_DefVal_Ingredient_DefInfo, DefVal_Ingredient_DefInfo_SettingChanged);

                AddSettingsItem(new TitleItem()
                {
                    Text = TitleRecources.SettingsDialog_ItemHandlingSupport_BlockGroupsConfig_Header,
                    TagFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold),
                });
                AddSettingsItem(ParamKey_Blocks, TitleRecources.SettingsDialog_ItemHandlingSupport_ParamKey_Blocks,
                    TextRecources.SettingsDialog_ToolTip_ParamKey_Blocks, ParamKey_Blocks_SettingChanged);

                AddSettingsItem(new TitleItem()
                {
                    Text = TitleRecources.SettingsDialog_ItemHandlingSupport_GlobalDefConfig_Header,
                    TagFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold),
                });
                AddSettingsItem(ParamKeys_GlobalRef, TitleRecources.SettingsDialog_ItemHandlingSupport_ParamKeys_GlobalRef,
                    TextRecources.SettingsDialog_ToolTip_ParamKeys_GlobalRef, ParamKeys_GlobalRef_SettingChanged);
                AddSettingsItem(DefVal_GlobalDefParam_DefIsOptional, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_GlobalDefParam_DefIsOptional,
                    TextRecources.SettingsDialog_ToolTip_DefVal_GlobalDefParam_DefIsOptional, DefVal_GlobalDefParam_DefIsOptional_SettingChanged);
                AddSettingsItem(DefVal_GlobalDefParam_DefHasValue, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_GlobalDefParam_DefHasValue,
                    TextRecources.SettingsDialog_ToolTip_DefVal_GlobalDefParam_DefHasValue, DefVal_GlobalDefParam_DefHasValue_SettingChanged);
                AddSettingsItem(DefVal_GlobalDefParam_DefIsAllowingBlank, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_GlobalDefParam_DefIsAllowingBlank,
                    TextRecources.SettingsDialog_ToolTip_DefVal_GlobalDefParam_DefIsAllowingBlank, DefVal_GlobalDefParam_DefIsAllowingBlank_SettingChanged);
                AddSettingsItem(DefVal_GlobalDefParam_DefIsForceEscaped, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_GlobalDefParam_DefIsForceEscaped,
                    TextRecources.SettingsDialog_ToolTip_DefVal_GlobalDefParam_DefIsForceEscaped, DefVal_GlobalDefParam_DefIsForceEscaped_SettingChanged);
                AddSettingsItem(DefVal_GlobalDefParam_DefInfo, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_GlobalDefParam_DefInfo,
                    TextRecources.SettingsDialog_ToolTip_DefVal_GlobalDefParam_DefInfo, DefVal_GlobalDefParam_DefInfo_SettingChanged);
                AddSettingsItem(DefVal_GlobalDefAttr_DefIsOptional, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_GlobalDefAttr_DefIsOptional,
                    TextRecources.SettingsDialog_ToolTip_DefVal_GlobalDefAttr_DefIsOptional, DefVal_GlobalDefAttr_DefIsOptional_SettingChanged);
                AddSettingsItem(DefVal_GlobalDefAttr_DefHasValue, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_GlobalDefAttr_DefHasValue,
                    TextRecources.SettingsDialog_ToolTip_DefVal_GlobalDefAttr_DefHasValue, DefVal_GlobalDefAttr_DefHasValue_SettingChanged);
                AddSettingsItem(DefVal_GlobalDefAttr_DefIsAllowingBlank, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_GlobalDefAttr_DefIsAllowingBlank,
                    TextRecources.SettingsDialog_ToolTip_DefVal_GlobalDefAttr_DefIsAllowingBlank, DefVal_GlobalDefAttr_DefIsAllowingBlank_SettingChanged);
                AddSettingsItem(DefVal_GlobalDefAttr_DefIsForceEscaped, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_GlobalDefAttr_DefIsForceEscaped,
                    TextRecources.SettingsDialog_ToolTip_DefVal_GlobalDefAttr_DefIsForceEscaped, DefVal_GlobalDefAttr_DefIsForceEscaped_SettingChanged);
                AddSettingsItem(DefVal_GlobalDefAttr_DefInfo, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_GlobalDefAttr_DefInfo,
                    TextRecources.SettingsDialog_ToolTip_DefVal_GlobalDefAttr_DefInfo, DefVal_GlobalDefAttr_DefInfo_SettingChanged);

                AddSettingsItem(new TitleItem()
                {
                    Text = TitleRecources.SettingsDialog_ItemHandlingSupport_FileParamConfig_Header,
                    TagFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold),
                });
                AddSettingsItem(DefVal_FileParam_DefIsOptional, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_FileParam_DefIsOptional,
                    TextRecources.SettingsDialog_ToolTip_DefVal_FileParam_DefIsOptional, DefVal_FileParam_DefIsOptional_SettingChanged);
                AddSettingsItem(DefVal_FileParam_DefHasValue, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_FileParam_DefHasValue,
                    TextRecources.SettingsDialog_ToolTip_DefVal_FileParam_DefHasValue, DefVal_FileParam_DefHasValue_SettingChanged);
                AddSettingsItem(DefVal_FileParam_DefIsAllowingBlank, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_FileParam_DefIsAllowingBlank,
                    TextRecources.SettingsDialog_ToolTip_DefVal_FileParam_DefIsAllowingBlank, DefVal_FileParam_DefIsAllowingBlank_SettingChanged);
                AddSettingsItem(DefVal_FileParam_DefIsForceEscaped, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_FileParam_DefIsForceEscaped,
                    TextRecources.SettingsDialog_ToolTip_DefVal_FileParam_DefIsForceEscaped, DefVal_FileParam_DefIsForceEscaped_SettingChanged);
                AddSettingsItem(DefVal_FileParam_DefInfo, TitleRecources.SettingsDialog_ItemHandlingSupport_DefVal_FileParam_DefInfo,
                    TextRecources.SettingsDialog_ToolTip_DefVal_FileParam_DefInfo, DefVal_FileParam_DefInfo_SettingChanged);

                ResumeLayout(true);
            }
        }
        private class InfoPanel : SettingsPanel
        {
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

                AddSettingsItem(new TitleItem()
                {
                    Text = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false)
                        .Cast<AssemblyTitleAttribute>().FirstOrDefault().Title,
                    TagFont = new Font("Viner Hand ITC", 20.25F, FontStyle.Bold, GraphicsUnit.Point),
                });
                AddSpacer(new LogoItem()
                {
                    Image = new Icon(IconRecources.Icon_AppBranding, 256, 256).ToBitmap(),
                });
                AddSettingsItem(new TextItem()
                {
                    Text = TitleRecources.Generic_Author,
                    NameTagAlign = ContentAlignment.MiddleRight,
                    Value = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false)
                        .Cast<AssemblyCompanyAttribute>().FirstOrDefault().Company,
                });
                AddSettingsItem(new TextItem()
                {
                    Text = TitleRecources.Generic_Version,
                    NameTagAlign = ContentAlignment.MiddleRight,
                    Value = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                });
                LinkItem licenseItem = new LinkItem()
                {
                    Text = TitleRecources.Generic_License,
                    NameTagAlign = ContentAlignment.MiddleRight,
                    Value = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)
                        .Cast<AssemblyCopyrightAttribute>().FirstOrDefault().Copyright,
                };
                licenseItem.LinkClicked += LicenseItem_LinkClicked;
                AddSettingsItem(licenseItem);
                LinkItem readmeItem = new LinkItem()
                {
                    Text = TitleRecources.Generic_Manual,
                    NameTagAlign = ContentAlignment.MiddleRight,
                    Value = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false)
                        .Cast<AssemblyConfigurationAttribute>().FirstOrDefault().Configuration,
                };
                readmeItem.LinkClicked += ReadmeItem_LinkClicked;
                AddSettingsItem(readmeItem);

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
                AutoSizeMode = AutoSizeMode.GrowAndShrink;
                ColumnCount = 2;
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                Controls.Add(NameTag, 0, 0);
                Dock = DockStyle.Top;
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
        private class TitleItem : SettingsTableItem
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

            public TitleItem() : base()
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
        private class LogoItem : SettingsTableItem
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

            public LogoItem() : base()
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
        private class TextItem : SettingsTableItem
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

            public TextItem() : base()
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
        private class LinkItem : SettingsTableItem
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

            public LinkItem() : base()
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
    public static class SettingsConverter
    {
        public static IEnumerable<T> ToSeperated<T>(this string multiValueSetting)
        {
            return multiValueSetting?.Split(InternalSettings.Default.SettingsDialog_MultiValueSeperator)
                .Select(value => (T)Convert.ChangeType(value.Trim(), typeof(T)));
        }
    }
}
