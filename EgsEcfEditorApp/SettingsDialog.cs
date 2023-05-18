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

            SaveButton.Text = TitleRecources.Generic_Save;
            ResetButton.Text = TitleRecources.Generic_Reset;
            AbortButton.Text = TitleRecources.Generic_Abort;

            InitPanels();

            ChapterSelectorTreeView.SelectedNode = ChapterSelectorTreeView.Nodes[0];
            SwitchPanel(0);
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
            SwitchPanel(evt.Node.Index);
        }
        private void EcfSettingsDialog_Activated(object sender, EventArgs evt)
        {
            PreparePanels();
            UpdatePanels();
            ChapterSelectorTreeView.Focus();
        }

        // privates
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



        private void InitGeneralPanel(int index)
        {
            ChapterSelectorTreeView.Nodes[index].Text = TitleRecources.EcfSettingsDialog_GeneralPanel_Header;
        }
        private void InitCreationPanel(int index)
        {
            ChapterSelectorTreeView.Nodes[index].Text = TitleRecources.EcfSettingsDialog_CreationPanel_Header;
        }
        private void InitFilterPanel(int index)
        {
            ChapterSelectorTreeView.Nodes[index].Text = TitleRecources.EcfSettingsDialog_FilterPanel_Header;
        }
        private void InitSorterPanel(int index)
        {
            ChapterSelectorTreeView.Nodes[index].Text = TitleRecources.EcfSettingsDialog_SorterPanel_Header;
        }
        private void InitTechTreePanel(int index)
        {
            ChapterSelectorTreeView.Nodes[index].Text = TitleRecources.EcfSettingsDialog_TechTreePanel_Header;
        }
        private void InitItemHandlingSupportPanel(int index)
        {
            ChapterSelectorTreeView.Nodes[index].Text = TitleRecources.EcfSettingsDialog_ItemHandlingSupportPanel_Header;
        }
        private void InitInfoPanel(int index)
        {
            ChapterSelectorTreeView.Nodes[index].Text = TitleRecources.EcfSettingsDialog_InfoPanel_Header;
        }



        // panel classes
        private abstract class SettingsPanel : TableLayoutPanel
        {
            public bool HasUnsavedChanges { get; set; }

            public SettingsPanel()
            {
                SuspendLayout();

                AutoSize = true;
                ColumnCount = 1;
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                Dock = DockStyle.Fill;
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize;

                ResumeLayout(true);
            }

            public abstract void PresetItems();
            public abstract void UpdateItems();
        }
        private class GeneralSettingsPanel : SettingsPanel
        {
            private ComboBoxSettingItem GameMode { get; } = new ComboBoxSettingItem(); 

            public GeneralSettingsPanel(ToolTip toolTipContainer) : base()
            {
                SuspendLayout();

                Controls.Add(GameMode, 0, 0);
                RowCount = 2;
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                GameMode.Text = TitleRecources.EcfSettingsDialog_GeneralPanel_GameMode;
                GameMode.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_GameMode);
                GameMode.SettingChanged += GameMode_SettingChanged;

                ResumeLayout(true);
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
                GameMode.UpdateItem(UserSettings.Default.EgsEcfEditorApp_ActiveGameMode);
            }
        }
        private class CreationSettingsPanel : SettingsPanel
        {
            private CheckBoxSettingItem WriteOnlyValidItems { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem InvalidateParentsOnError { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem AllowFallbackToParsedData { get; } = new CheckBoxSettingItem();

            public CreationSettingsPanel(ToolTip toolTipContainer) : base()
            {
                SuspendLayout();

                Controls.Add(WriteOnlyValidItems, 0, 0);
                Controls.Add(InvalidateParentsOnError, 0, 1);
                Controls.Add(AllowFallbackToParsedData, 0, 2);
                RowCount = 4;
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                WriteOnlyValidItems.Text = TitleRecources.EcfSettingsDialog_CreationPanel_WriteOnlyValidItems;
                WriteOnlyValidItems.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_WriteOnlyValidItems);
                WriteOnlyValidItems.SettingChanged += WriteOnlyValidItems_SettingChanged;

                InvalidateParentsOnError.Text = TitleRecources.EcfSettingsDialog_CreationPanel_InvalidateParentOnError;
                InvalidateParentsOnError.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_InvalidateParentsOnError);
                InvalidateParentsOnError.SettingChanged += InvalidateParentsOnError_SettingChanged;

                AllowFallbackToParsedData.Text = TitleRecources.EcfSettingsDialog_CreationPanel_AllowFallbackToParsedData;
                AllowFallbackToParsedData.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_AllowFallbackToParsedData);
                AllowFallbackToParsedData.SettingChanged += AllowFallbackToParsedData_SettingChanged;

                ResumeLayout(true);
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
                WriteOnlyValidItems.UpdateItem(UserSettings.Default.EgsEcfEditorApp_FileCreation_WriteOnlyValidItems);
                InvalidateParentsOnError.UpdateItem(UserSettings.Default.EgsEcfEditorApp_FileCreation_InvalidateParentsOnError);
                AllowFallbackToParsedData.UpdateItem(UserSettings.Default.EgsEcfEditorApp_FileCreation_AllowFallbackToParsedData);
                UpdateAccessability();
            }

            // private
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

            public TreeFilterSettingsPanel(ToolTip toolTipContainer) : base()
            {
                SuspendLayout();

                Controls.Add(CommentsInitActive, 0, 0);
                Controls.Add(ParametersInitActive, 0, 1);
                Controls.Add(DataBlocksInitActive, 0, 2);
                RowCount = 4;
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                CommentsInitActive.Text = TitleRecources.EcfSettingsDialog_FilterPanel_TreeViewFilterCommentsInitActive;
                CommentsInitActive.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_TreeViewFilterCommentsInitActive);
                CommentsInitActive.SettingChanged += CommentsInitActive_SettingChanged;

                ParametersInitActive.Text = TitleRecources.EcfSettingsDialog_FilterPanel_TreeViewFilterParametersInitActive;
                ParametersInitActive.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_TreeViewFilterParametersInitActive);
                ParametersInitActive.SettingChanged += ParametersInitActive_SettingChanged;

                DataBlocksInitActive.Text = TitleRecources.EcfSettingsDialog_FilterPanel_TreeViewFilterDataBlocksInitActive;
                DataBlocksInitActive.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_TreeViewFilterDataBlocksInitActive);
                DataBlocksInitActive.SettingChanged += DataBlocksInitActive_SettingChanged;

                ResumeLayout(true);
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
                CommentsInitActive.UpdateItem(UserSettings.Default.EgsEcfControls_TreeViewFilterCommentsInitActive);
                ParametersInitActive.UpdateItem(UserSettings.Default.EgsEcfControls_TreeViewFilterParametersInitActive);
                DataBlocksInitActive.UpdateItem(UserSettings.Default.EgsEcfControls_TreeViewFilterDataBlocksInitActive);
            }
        }
        private class SorterSettingsPanel : SettingsPanel
        {
            private ComboBoxSettingItem TreeViewSorterInitCount { get; } = new ComboBoxSettingItem();
            private ComboBoxSettingItem ParameterViewSorterInitCount { get; } = new ComboBoxSettingItem();
            private ComboBoxSettingItem ErrorViewSorterInitCount { get; } = new ComboBoxSettingItem();

            public SorterSettingsPanel(ToolTip toolTipContainer) : base()
            {
                SuspendLayout();

                Controls.Add(TreeViewSorterInitCount, 0, 0);
                Controls.Add(ParameterViewSorterInitCount, 0, 1);
                Controls.Add(ErrorViewSorterInitCount, 0, 2);
                RowCount = 4;
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                TreeViewSorterInitCount.Text = TitleRecources.EcfSettingsDialog_SorterPanel_TreeViewSorterInitCount;
                TreeViewSorterInitCount.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_TreeViewSorterInitCount);
                TreeViewSorterInitCount.SettingChanged += TreeViewSorterInitCount_SettingChanged;

                ParameterViewSorterInitCount.Text = TitleRecources.EcfSettingsDialog_SorterPanel_ParameterViewSorterInitCount;
                ParameterViewSorterInitCount.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_ParameterViewSorterInitCount);
                ParameterViewSorterInitCount.SettingChanged += ParameterViewSorterInitCount_SettingChanged;

                ErrorViewSorterInitCount.Text = TitleRecources.EcfSettingsDialog_SorterPanel_ErrorViewSorterInitCount;
                ErrorViewSorterInitCount.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_ErrorViewSorterInitCount);
                ErrorViewSorterInitCount.SettingChanged += ErrorViewSorterInitCount_SettingChanged;

                ResumeLayout(true);
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
                TreeViewSorterInitCount.UpdateItem(UserSettings.Default.EgsEcfControls_TreeViewSorterInitCount.ToString());
                ParameterViewSorterInitCount.UpdateItem(UserSettings.Default.EgsEcfControls_ParameterViewSorterInitCount.ToString());
                ErrorViewSorterInitCount.UpdateItem(UserSettings.Default.EgsEcfControls_ErrorViewSorterInitCount.ToString());
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

            public TechTreeSettingsPanel(ToolTip toolTipContainer) : base()
            {
                SuspendLayout();

                Controls.Add(ParameterKeyTechTreeNames, 0, 0);
                Controls.Add(ParameterKeyTechTreeParentName, 0, 1);
                Controls.Add(ParameterKeyUnlockLevel, 0, 2);
                Controls.Add(DefaultValueUnlockLevel, 0, 3);
                Controls.Add(ParameterKeyUnlockCost, 0, 4);
                Controls.Add(DefaultValueUnlockCost, 0, 5);
                RowCount = 7;
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                ParameterKeyTechTreeNames.Text = TitleRecources.EcfSettingsDialog_TechTreePanel_ParameterKeyTechTreeNames;
                ParameterKeyTechTreeNames.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_ParameterKeyTechTreeNames);
                ParameterKeyTechTreeNames.SettingChanged += ParameterKeyTechTreeNames_SettingChanged;

                ParameterKeyTechTreeParentName.Text = TitleRecources.EcfSettingsDialog_TechTreePanel_ParameterKeyTechTreeParentName;
                ParameterKeyTechTreeParentName.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_ParameterKeyTechTreeParentName);
                ParameterKeyTechTreeParentName.SettingChanged += ParameterKeyTechTreeParentName_SettingChanged;

                ParameterKeyUnlockLevel.Text = TitleRecources.EcfSettingsDialog_TechTreePanel_ParameterKeyUnlockLevel;
                ParameterKeyUnlockLevel.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_ParameterKeyUnlockLevel);
                ParameterKeyUnlockLevel.SettingChanged += ParameterKeyUnlockLevel_SettingChanged;

                DefaultValueUnlockLevel.Text = TitleRecources.EcfSettingsDialog_TechTreePanel_DefaultValueUnlockLevel;
                DefaultValueUnlockLevel.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_DefaultValueUnlockLevel);
                DefaultValueUnlockLevel.SettingChanged += DefaultValueUnlockLevel_SettingChanged;

                ParameterKeyUnlockCost.Text = TitleRecources.EcfSettingsDialog_TechTreePanel_ParameterKeyUnlockCost;
                ParameterKeyUnlockCost.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_ParameterKeyUnlockCost);
                ParameterKeyUnlockCost.SettingChanged += ParameterKeyUnlockCost_SettingChanged;

                DefaultValueUnlockCost.Text = TitleRecources.EcfSettingsDialog_TechTreePanel_DefaultValueUnlockCost;
                DefaultValueUnlockCost.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_DefaultValueUnlockCost);
                DefaultValueUnlockCost.SettingChanged += DefaultValueUnlockCost_SettingChanged;

                ResumeLayout(true);
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

                ParameterKeyTechTreeNames.UpdateItem(UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeNames);
                ParameterKeyTechTreeParentName.UpdateItem(UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeParentName);
                ParameterKeyUnlockLevel.UpdateItem(UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockLevel);
                DefaultValueUnlockLevel.UpdateItem(UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockLevel);
                ParameterKeyUnlockCost.UpdateItem(UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockCost);
                DefaultValueUnlockCost.UpdateItem(UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockCost);
            }
        }
        private class ItemHandlingSupportSettingsPanel : SettingsPanel
        {
            private TextBoxSettingItem ParameterKeyTemplateRoot { get; } = new TextBoxSettingItem();
            private CheckBoxSettingItem DefaultDefinitionIsOptional { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefaultDefinitionHasValue { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefaultDefinitionIsAllowingBlank { get; } = new CheckBoxSettingItem();
            private CheckBoxSettingItem DefaultDefinitionIsForceEscaped { get; } = new CheckBoxSettingItem();
            private TextBoxSettingItem DefaultDefinitionInfo { get; } = new TextBoxSettingItem();

            public ItemHandlingSupportSettingsPanel(ToolTip toolTipContainer) : base()
            {
                SuspendLayout();

                Controls.Add(ParameterKeyTemplateRoot, 0, 0);
                Controls.Add(DefaultDefinitionIsOptional, 0, 1);
                Controls.Add(DefaultDefinitionHasValue, 0, 2);
                Controls.Add(DefaultDefinitionIsAllowingBlank, 0, 3);
                Controls.Add(DefaultDefinitionIsForceEscaped, 0, 4);
                Controls.Add(DefaultDefinitionInfo, 0, 5);
                RowCount = 7;
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                ParameterKeyTemplateRoot.Text = TitleRecources.EcfSettingsDialog_ItemHandlingSupportPanel_ParameterKeyTemplateRoot;
                ParameterKeyTemplateRoot.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_ParameterKeyTemplateRoot);
                ParameterKeyTemplateRoot.SettingChanged += ParameterKeyTemplateRoot_SettingChanged;

                DefaultDefinitionIsOptional.Text = TitleRecources.EcfSettingsDialog_ItemHandlingSupportPanel_DefaultDefinitionIsOptional;
                DefaultDefinitionIsOptional.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_DefaultDefinitionIsOptional);
                DefaultDefinitionIsOptional.SettingChanged += DefaultDefinitionIsOptional_SettingChanged;

                DefaultDefinitionHasValue.Text = TitleRecources.EcfSettingsDialog_ItemHandlingSupportPanel_DefaultDefinitionHasValue;
                DefaultDefinitionHasValue.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_DefaultDefinitionHasValue);
                DefaultDefinitionHasValue.SettingChanged += DefaultDefinitionHasValue_SettingChanged;

                DefaultDefinitionIsAllowingBlank.Text = TitleRecources.EcfSettingsDialog_ItemHandlingSupportPanel_DefaultDefinitionIaAllowingBlank;
                DefaultDefinitionIsAllowingBlank.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_DefaultDefinitionIaAllowingBlank);
                DefaultDefinitionIsAllowingBlank.SettingChanged += DefaultDefinitionIsAllowingBlank_SettingChanged;

                DefaultDefinitionIsForceEscaped.Text = TitleRecources.EcfSettingsDialog_ItemHandlingSupportPanel_DefaultDefinitionIsForceEscaped;
                DefaultDefinitionIsForceEscaped.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_DefaultDefinitionIsForceEscaped);
                DefaultDefinitionIsForceEscaped.SettingChanged += DefaultDefinitionIsForceEscaped_SettingChanged;

                DefaultDefinitionInfo.Text = TitleRecources.EcfSettingsDialog_ItemHandlingSupportPanel_DefaultDefinitionInfo;
                DefaultDefinitionInfo.SetToolTip(toolTipContainer, TextRecources.EcfSettingsDialog_ToolTip_DefaultDefinitionInfo);
                DefaultDefinitionInfo.SettingChanged += DefaultDefinitionInfo_SettingChanged;

                ResumeLayout(true);
            }

            // events
            private void ParameterKeyTemplateRoot_SettingChanged(object sender, EventArgs evt)
            {
                UserSettings.Default.ItemHandlingSupport_ParameterKey_TemplateName = ParameterKeyTemplateRoot.Value;
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
                ParameterKeyTemplateRoot.UpdateItem(UserSettings.Default.ItemHandlingSupport_ParameterKey_TemplateName);
                DefaultDefinitionIsOptional.UpdateItem(UserSettings.Default.ItemHandlingSupport_DefaultValue_DefinitionIsOptional);
                DefaultDefinitionHasValue.UpdateItem(UserSettings.Default.ItemHandlingSupport_DefaultValue_DefinitionHasValue);
                DefaultDefinitionIsAllowingBlank.UpdateItem(UserSettings.Default.ItemHandlingSupport_DefaultValue_DefinitionIsAllowingBlank);
                DefaultDefinitionIsForceEscaped.UpdateItem(UserSettings.Default.ItemHandlingSupport_DefaultValue_DefinitionIsForceEscaped);
                DefaultDefinitionInfo.UpdateItem(UserSettings.Default.ItemHandlingSupport_DefaultValue_DefinitionInfo);
            }
        }
        private class InfoSettingsPanel : SettingsPanel
        {
            private TitleSettingsItem AppNameItem { get; } = new TitleSettingsItem();
            private LogoSettingsItem LogoItem { get; } = new LogoSettingsItem();
            private TextSettingsItem AuthorItem { get; } = new TextSettingsItem();
            private TextSettingsItem VersionItem { get; } = new TextSettingsItem();
            private LinkSettingsItem LicenseItem { get; } = new LinkSettingsItem();
            private LinkSettingsItem ReadmeItem { get; } = new LinkSettingsItem();

            public InfoSettingsPanel() : base()
            {
                SuspendLayout();

                Controls.Add(AppNameItem, 0, 0);
                Controls.Add(LogoItem, 0, 1);
                Controls.Add(AuthorItem, 0, 2);
                Controls.Add(VersionItem, 0, 3);
                Controls.Add(LicenseItem, 0, 4);
                Controls.Add(ReadmeItem, 0, 5);
                RowCount = 6;
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());
                RowStyles.Add(new RowStyle());

                AppNameItem.Text = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false)
                    .Cast<AssemblyTitleAttribute>().FirstOrDefault().Title;
                AppNameItem.Font = new Font("Viner Hand ITC", 20.25F, FontStyle.Bold, GraphicsUnit.Point);

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

                ResumeLayout(true);
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
        }

        // item classes
        private abstract class SettingsItem : TableLayoutPanel
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
            protected bool UpdateRunning { get; private set; } = false;
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

            protected Label NameTag { get; } = new Label();

            public SettingsItem()
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
            public void UpdateItem(object value)
            {
                UpdateRunning = true;
                OnUpdateItem(value);
                UpdateRunning = false;
            }

            // privates
            protected abstract void OnUpdateItem(object value);
        }
        private class TextBoxSettingItem : SettingsItem
        {
            public event EventHandler SettingChanged;

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
            }

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
                if (!UpdateRunning)
                {
                    SettingChanged?.Invoke(sender, evt);
                }
            }

            // public
            public override void SetToolTip(ToolTip toolTipContainer, string toolTip)
            {
                toolTipContainer.SetToolTip(Setting, toolTip);
            }

            // privates
            protected override void OnUpdateItem(object value)
            {
                Setting.Text = Convert.ToString(value);
            }
        }
        private class ComboBoxSettingItem : SettingsItem
        {
            public event EventHandler SettingChanged;

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
                toolTipContainer.SetToolTip(Setting, toolTip);
            }
            public void PresetItem(object[] entries)
            {
                Setting.BeginUpdate();
                Setting.Items.Clear();
                Setting.Items.AddRange(entries);
                Setting.EndUpdate();
            }

            // privates
            protected override void OnUpdateItem(object value)
            {
                Setting.SelectedItem = value;
                if (Setting.SelectedIndex < 0) { Setting.SelectedIndex = 0; }
            }
        }
        private class CheckBoxSettingItem : SettingsItem
        {
            public event EventHandler SettingChanged;

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
                toolTipContainer.SetToolTip(Setting, toolTip);
            }

            // privates
            protected override void OnUpdateItem(object value)
            {
                if (!bool.TryParse(Convert.ToString(value), out bool result))
                {
                    result = false;
                }
                Setting.Checked = result;
            }
        }
        private class NumericSettingItem : SettingsItem
        {
            public event EventHandler SettingChanged;

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
            }
            public decimal Minimum
            {
                get
                {
                    return Setting.Minimum;
                }
                set
                {
                    Setting.Minimum = value;
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
                    Setting.Maximum = value;
                }
            }

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
                if (!UpdateRunning)
                {
                    SettingChanged?.Invoke(sender, evt);
                }
            }

            // publics
            public override void SetToolTip(ToolTip toolTipContainer, string toolTip)
            {
                toolTipContainer.SetToolTip(Setting, toolTip);
            }

            // privates
            protected override void OnUpdateItem(object value)
            {
                if (!decimal.TryParse(Convert.ToString(value), out decimal result))
                {
                    result = 0;
                }
                Setting.Value = result;
            }
        }
        private class TitleSettingsItem : SettingsItem
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
            public override Font Font
            {
                get
                {
                    return NameTag.Font;
                }
                set
                {
                    NameTag.Font = value;
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
                toolTipContainer.SetToolTip(NameTag, toolTip);
            }

            // privates
            protected override void OnUpdateItem(object value)
            {
                NameTag.Text = Convert.ToString(value);
            }
        }
        private class LogoSettingsItem : SettingsItem
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
                toolTipContainer.SetToolTip(Logo, toolTip);
            }

            // privates
            protected override void OnUpdateItem(object value)
            {
                NameTag.Text = Convert.ToString(value);
            }
        }
        private class TextSettingsItem : SettingsItem
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
                toolTipContainer.SetToolTip(DataTag, toolTip);
            }

            // privates
            protected override void OnUpdateItem(object value)
            {
                DataTag.Text = Convert.ToString(value);
            }
        }
        private class LinkSettingsItem : SettingsItem
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
                toolTipContainer.SetToolTip(DataTag, toolTip);
            }

            // privates
            protected override void OnUpdateItem(object value)
            {
                DataTag.Text = Convert.ToString(value);
            }
        }
    }
}
