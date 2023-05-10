using CustomControls;
using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static EcfFileViews.ItemSelectorDialog;
using static EgsEcfParser.EcfFormatChecking;
using static Helpers.EnumLocalisation;

namespace EcfFileViews
{
    public partial class EcfItemEditingDialog : Form
    {
        public EcfStructureItem ResultItem { get; private set; } = null;

        public enum OperationModes
        {
            None,
            Comment,
            Parameter,
            RootBlock,
            ChildBlock,
            ParameterMatrix,
        }

        private OperationModes OperationMode { get; set; } = OperationModes.None;
        
        private ItemSelectorDialog ItemSelector { get; set; } = new ItemSelectorDialog()
        {
            Icon = IconRecources.Icon_AppBranding,
            OkButtonText = TitleRecources.Generic_Ok,
            AbortButtonText = TitleRecources.Generic_Abort,
            SearchToolTipText = TextRecources.ItemSelectorDialog_ToolTip_SearchInfo,
            DefaultItemText = TitleRecources.Generic_Replacement_Empty,
        };

        public EcfItemEditingDialog()
        {
            InitializeComponent();
            InitForm();
        }

        // events
        // unspecific
        private void InitForm()
        {
            Icon = IconRecources.Icon_AppBranding;
            OkButton.Text = TitleRecources.Generic_Ok;
            AbortButton.Text = TitleRecources.Generic_Abort;
            ResetButton.Text = TitleRecources.Generic_Reset;

            // Hack to hide tabs
            ViewPanel.SizeMode = TabSizeMode.Fixed;
            ViewPanel.ItemSize = new Size(0, 1);
        }
        private void AbortButton_Click(object sender, EventArgs evt)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }
        private void ResetButton_Click(object sender, EventArgs evt)
        {
            switch (OperationMode)
            {
                case OperationModes.Comment: CommentItem_UpdateView(); break;
                case OperationModes.Parameter: ParameterItem_UpdateView(); break;
                case OperationModes.ChildBlock: BlockItem_UpdateView(); break;
                case OperationModes.RootBlock: BlockItem_UpdateView(); break;
                case OperationModes.ParameterMatrix: ParameterMatrix_UpdateView(); break;
                default: break;
            }
        }
        private void OkButton_Click(object sender, EventArgs evt)
        {
            Generic_TryCloseSuccess();
        }
        private void EcfItemEditingDialog_Activated(object sender, EventArgs evt)
        {
            Generic_SetFocus();
        }

        // publics
        public DialogResult ShowDialog(IWin32Window parent, EgsEcfFile file, EcfComment comment)
        {
            ResultItem = null;
            ParentBlock = comment?.Parent as EcfBlock;
            File = file;
            PresetComment = comment;
            OperationMode = OperationModes.Comment;

            CommentItem_ActivateView();

            if (PresetComment != null)
            {
                Text = TitleRecources.EcfItemEditingDialog_Header_EditComment;
            }
            else
            {
                Text = TitleRecources.EcfItemEditingDialog_Header_AddComment;
            }
            ResetButton.Enabled = PresetComment != null;

            // hack to prevent tab switch with tab key
            ViewPanel.TabPages.Clear();
            ViewPanel.TabPages.Add(CommentItemView);

            return ShowDialog(parent);
        }
        public DialogResult ShowDialog(IWin32Window parent, EgsEcfFile file, EcfParameter parameter)
        {
            ResultItem = null;
            ParentBlock = parameter?.Parent as EcfBlock;
            File = file;
            PresetParameter = parameter;
            OperationMode = OperationModes.Parameter;

            try
            {
                ParameterItem_PreActivationChecks_Editing();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return DialogResult.Abort;
            }
            ParameterItem_ActivateView();

            if (PresetParameter != null)
            {
                Text = TitleRecources.EcfItemEditingDialog_Header_EditParameter;
            }
            else
            {
                Text = TitleRecources.EcfItemEditingDialog_Header_AddParameter;
            }
            ResetButton.Enabled = PresetParameter != null;

            // hack to prevent tab switch with tab key
            ViewPanel.TabPages.Clear();
            ViewPanel.TabPages.Add(ParameterItemView);

            return ShowDialog(parent);
        }
        public DialogResult ShowDialog(IWin32Window parent, EgsEcfFile file, EcfBlock block)
        {
            ResultItem = null;
            ParentBlock = block?.Parent as EcfBlock;
            File = file;
            PresetBlock = block;
            OperationMode = (block?.IsRoot() ?? true) ? OperationModes.RootBlock : OperationModes.ChildBlock;

            try
            {
                BlockItem_PreActivationChecks_Editing();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return DialogResult.Abort;
            }
            BlockItem_ActivateView();

            if (PresetBlock != null)
            {
                if (PanelMode == BlockItemPanelModes.ChildBlock)
                {
                    Text = TitleRecources.EcfItemEditingDialog_Header_EditChildBlock;
                }
                else
                {
                    Text = TitleRecources.EcfItemEditingDialog_Header_EditRootBlock;
                }
            }
            else
            {
                if (PanelMode == BlockItemPanelModes.ChildBlock)
                {
                    Text = TitleRecources.EcfItemEditingDialog_Header_AddChildBlock;
                }
                else
                {
                    Text = TitleRecources.EcfItemEditingDialog_Header_AddRootBlock;
                }
            }
            ResetButton.Enabled = PresetBlock != null;

            // hack to prevent tab switch with tab key
            ViewPanel.TabPages.Clear();
            ViewPanel.TabPages.Add(BlockItemView);

            return ShowDialog(parent);
        }
        public DialogResult ShowDialog(IWin32Window parent, EgsEcfFile file, List<EcfParameter> parameters)
        {
            ResultItem = null;
            ParentBlock = null;
            File = file;
            PresetParameter = null;
            OperationMode = OperationModes.ParameterMatrix;

            try
            {
                ParameterMatrix_PreActivationChecks(parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return DialogResult.Abort;
            }
            ParameterMatrix_ActivateView(parameters);

            Text = TitleRecources.EcfItemEditingDialog_Header_EditParameterMatrix;
            ResetButton.Enabled = true;

            // hack to prevent tab switch with tab key
            ViewPanel.TabPages.Clear();
            ViewPanel.TabPages.Add(ParameterMatrixView);

            return ShowDialog(parent);
        }
        public DialogResult ShowDialog(IWin32Window parent, EgsEcfFile file, OperationModes creationMode, EcfBlock parentBlock)
        {
            ResultItem = null;
            ParentBlock = parentBlock;
            File = file;
            Generic_ClearPresets();
            OperationMode = creationMode;

            try
            {
                Generic_PreActivationChecks_Adding();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return DialogResult.Abort;
            }
            Generic_ActivateView();

            // header text?
            // reset button?
            // tab adding?

            return ShowDialog(parent);
        }
        
        // privates
        // generics
        private void Generic_SetFocus()
        {
            switch (OperationMode)
            {
                case OperationModes.Comment:
                    CommentItemRichTextBox.Focus();
                    CommentItemRichTextBox.SelectAll();
                    break;
                case OperationModes.Parameter:
                    ParameterItemValuesPanel.TryFocusFirstCell();
                    break;
                default:
                    OkButton.Focus();
                    break;
            }
        }
        private void Generic_TryCloseSuccess()
        {
            List<string> errors = Generic_ValidateInputs();
            if (errors.Count > 0)
            {
                Generic_ShowValidationErrors(errors);
                return;
            }
            ResultItem = Generic_PrepareResultItem();
            DialogResult = DialogResult.OK;
            Close();
        }
        private void Generic_ClearPresets()
        {
            PresetComment = null;
            PresetParameter = null;
            PresetBlock = null;
        }
        private void Generic_PreActivationChecks_Adding()
        {
            switch (OperationMode)
            {
                case OperationModes.Comment: break;
                case OperationModes.Parameter: ParameterItem_PreActivationChecks_Adding(); break;
                case OperationModes.RootBlock: 
                case OperationModes.ChildBlock: BlockItem_PreActivationChecks_Adding(); break;
                default: throw new ArgumentException(string.Format("Mode {0} not allowed for creation ...that shouldn't happen :)", OperationMode.ToString()));
            }
        }
        private void Generic_ActivateView()
        {
            switch (OperationMode)
            {
                case OperationModes.Comment: CommentItem_ActivateView(); break;
                case OperationModes.Parameter: ParameterItem_ActivateView(); break;
                case OperationModes.RootBlock:
                case OperationModes.ChildBlock: BlockItem_ActivateView(); break;
                default: break;
            }
        }
        private List<string> Generic_ValidateInputs()
        {
            switch (OperationMode)
            {
                case OperationModes.Comment: return CommentItem_ValidateInputs();
                case OperationModes.Parameter: return ParameterItem_ValidateInputs();
                case OperationModes.RootBlock: return BlockItem_ValidateInputs();
                case OperationModes.ChildBlock: return BlockItem_ValidateInputs();
                case OperationModes.ParameterMatrix: return ParameterMatrix_ValidateInputs();
                default: throw new ArgumentException(string.Format("No creator defined for item type {0}...that shouldn't happen", OperationMode.ToString()));
            }
        }
        private void Generic_ShowValidationErrors(List<string> errors)
        {
            StringBuilder message = new StringBuilder(TextRecources.Generic_ContinueImpossibleWithErrors);
            message.Append(Environment.NewLine);
            errors.ForEach(error =>
            {
                message.Append(Environment.NewLine);
                message.Append(error);
            });

            MessageBox.Show(this, message.ToString(), TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        private EcfStructureItem Generic_PrepareResultItem()
        {
            switch (OperationMode)
            {
                case OperationModes.Comment: return CommentItem_PrepareResultItem();
                case OperationModes.Parameter: return ParameterItem_PrepareResultItem();
                case OperationModes.RootBlock: return BlockItem_PrepareResultItem();
                case OperationModes.ChildBlock: return BlockItem_PrepareResultItem();
                case OperationModes.ParameterMatrix: return ParameterMatrix_PrepareResultItem();
                default: throw new ArgumentException(string.Format("No creator defined for item type {0}....that shouldn't happen", OperationMode.ToString()));
            }
        }

        // prechecks
        private void ParameterItem_PreActivationChecks_Editing()
        {
            if (ParentBlock == null) { throw new ArgumentException(TextRecources.EcfItemEditingDialog_NoParameterEditingWithoutParent); }

            List<string> definedParameters = File.Definition.BlockParameters.Select(param => param.Name).ToList();
            if (definedParameters.Count < 1)
            {
                throw new InvalidOperationException(TextRecources.EcfItemEditingDialog_NoParameterEditingWithoutDefinitions);
            }

            if (!definedParameters.Contains(PresetParameter.Key))
            {
                MessageBox.Show(this, string.Format("{0}: {1}", TextRecources.EcfItemEditingDialog_NoDefinitionFoundForParameter, PresetParameter?.Key ?? string.Empty),
                    TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                List<string> selectableParameters = definedParameters.Except(ParentBlock.ChildItems.Where(child => child is EcfParameter).Cast<EcfParameter>().Select(param => param.Key)).ToList();
                if (selectableParameters.Count < 1)
                {
                    throw new ArgumentException(TextRecources.EcfItemEditingDialog_NoSelectableParameterAvailable);
                }

                ItemSelector.Text = string.Format("{0}: {1}", TitleRecources.Generic_PickItem, TitleRecources.Generic_Parameter);
                if (ItemSelector.ShowDialog(this, selectableParameters.Select(param => new SelectorItem(param)).ToArray()) == DialogResult.OK)
                {
                    PresetParameterCheckedKey = Convert.ToString(ItemSelector.SelectedItem.Item);
                    return;
                }
                throw new ArgumentException(TextRecources.EcfItemEditingDialog_ParameterEditingAborted);
            }
            PresetParameterCheckedKey = PresetParameter.Key;
        }
        private void ParameterItem_PreActivationChecks_Adding()
        {
            if (ParentBlock == null) { throw new ArgumentException(TextRecources.EcfItemEditingDialog_NoParameterAddingWithoutParent); }
            List<string> definedParameters = File.Definition.BlockParameters.Select(param => param.Name).ToList();
            if (definedParameters.Count < 1) { throw new ArgumentException(TextRecources.EcfItemEditingDialog_NoParameterAddingWithoutDefinitions); }
            List<string> addableParameters = definedParameters.Except(ParentBlock.ChildItems.Where(child => 
                child is EcfParameter).Cast<EcfParameter>().Select(param => param.Key)).ToList();
            if (addableParameters.Count < 1) { throw new ArgumentException(TextRecources.EcfItemEditingDialog_NoAddableParameterAvailable); }
        }
        private void BlockItem_PreActivationChecks_Editing()
        {
            PresetBlockCheckedPreMark = BlockItem_PreActivationChecks_EditingDefinition(
                File.Definition.BlockTypePreMarks, PresetBlock?.PreMark, TitleRecources.Generic_PreMark);

            if (OperationMode == OperationModes.ChildBlock)
            {
                PresetBlockCheckedDataType = BlockItem_PreActivationChecks_EditingDefinition(
                    File.Definition.ChildBlockTypes, PresetBlock?.DataType, TitleRecources.Generic_DataType);
            }
            else if (OperationMode == OperationModes.RootBlock)
            {
                PresetBlockCheckedDataType = BlockItem_PreActivationChecks_EditingDefinition(
                    File.Definition.RootBlockTypes, PresetBlock?.DataType, TitleRecources.Generic_DataType);
            }

            PresetBlockCheckedPostMark = BlockItem_PreActivationChecks_EditingDefinition(
                File.Definition.BlockTypePostMarks, PresetBlock?.PostMark, TitleRecources.Generic_PostMark);
        }
        private string BlockItem_PreActivationChecks_EditingDefinition(
            ReadOnlyCollection<BlockValueDefinition> definition, string dataToCheck, string dataTypeName)
        {
            if (definition.Count > 0)
            {
                if (definition.Any(type => !type.IsOptional) && !definition.Any(mark => mark.Value.Equals(dataToCheck)))
                {
                    MessageBox.Show(this, string.Format("{0} {1} \"{2}\"", TextRecources.EcfItemEditingDialog_NoDefinitionAvailableFor, dataTypeName, dataToCheck ?? string.Empty), 
                        TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    ItemSelector.Text = string.Format("{0}: {1}", TitleRecources.Generic_PickItem, dataTypeName);
                    if (ItemSelector.ShowDialog(this, definition.Select(type => new SelectorItem(type.Value)).ToArray()) == DialogResult.OK)
                    {
                        return Convert.ToString(ItemSelector.SelectedItem.Item);
                    }
                    throw new InvalidOperationException(TextRecources.EcfItemEditingDialog_BlockEditingAborted);
                }
            }
            return dataToCheck;
        }
        private void BlockItem_PreActivationChecks_Adding()
        {
            if (File.Definition.BlockTypePostMarks.Count < 1)
            {
                throw new ArgumentException(TextRecources.EcfItemEditingDialog_NoBlockAddingWithoutPostMarkDefinition);
            }
            if (OperationMode == OperationModes.ChildBlock)
            {
                if (File.Definition.ChildBlockTypes.Count < 1)
                {
                    throw new ArgumentException(TextRecources.EcfItemEditingDialog_NoChildBlockAddingWithoutTypeDefinition);
                }
            }
            if (OperationMode == OperationModes.RootBlock)
            {
                if (File.Definition.RootBlockTypes.Count < 1)
                {
                    throw new ArgumentException(TextRecources.EcfItemEditingDialog_NoRootBlockAddingWithoutTypeDefinition);
                }
            }
        }
        private void ParameterMatrix_PreActivationChecks(List<EcfParameter> parameters)
        {
            if (parameters.Any(parameter => parameter.Definition == null))
            {
                throw new InvalidOperationException(TextRecources.EcfItemEditingDialog_ParameterMatrixForUnknownParametersNotAllowed);
            }
            if (parameters.Any(parameter => !parameter.Definition.HasValue))
            {
                throw new InvalidOperationException(TextRecources.EcfItemEditingDialog_ParameterMatrixForValuelessParametersNotAllowed);
            }
        }

        // subclasses
        private class ComboBoxItem : IComparable<ComboBoxItem>
        {
            public string DisplayText { get; }
            public string Value { get; }

            public ComboBoxItem(string value)
            {
                Value = value;
                DisplayText = BuildDisplayText(value);
            }

            private string BuildDisplayText(string value)
            {
                if (value == null) { return TitleRecources.Generic_Replacement_Empty; }
                return string.Format("\"{0}\"", value);
            }

            public override string ToString()
            {
                return DisplayText;
            }

            public int CompareTo(ComboBoxItem other)
            {
                if (DisplayText == null || other.DisplayText == null) { return 0; }
                return DisplayText.CompareTo(other.DisplayText);
            }
        }
        private class AttributesPanel : TableLayoutPanel
        {
            public event EventHandler InheritorChanged;
            
            private Label TableLabel { get; } = new Label();
            private FlowLayoutPanel ButtonPanel { get; } = new FlowLayoutPanel();
            private Button AddValueButton { get; } = new Button();
            private Button RemoveValueButton { get; } = new Button();
            private OptimizedDataGridView Grid { get; } = new OptimizedDataGridView();

            private ItemSelectorDialog ItemSelector { get; set; } = new ItemSelectorDialog()
            {
                Icon = IconRecources.Icon_AppBranding,
                Text = TitleRecources.Generic_PickItem,
                OkButtonText = TitleRecources.Generic_Ok,
                AbortButtonText = TitleRecources.Generic_Abort,
                SearchToolTipText = TextRecources.ItemSelectorDialog_ToolTip_SearchInfo,
                DefaultItemText = TitleRecources.Generic_Replacement_Empty,
            };
            public List<EcfBlock> ReferencedBlockList { get; set; } = null;

            private int PrefixColumnCount { get; } = 3;
            private DataGridViewCheckBoxColumn ActivationColumn { get; } = new DataGridViewCheckBoxColumn();
            private DataGridViewTextBoxColumn NameColumn { get; } = new DataGridViewTextBoxColumn();
            private DataGridViewTextBoxColumn InfoColumn { get; } = new DataGridViewTextBoxColumn();

            private ToolTip Tip { get; } = new ToolTip();

            public AttributesPanel() : base()
            {
                InitPanel();
            }

            // events
            private void InitPanel()
            {
                AutoSize = true;
                ColumnCount = 2;
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
                Dock = DockStyle.Fill;
                RowCount = 2;
                RowStyles.Add(new RowStyle(SizeType.AutoSize));
                RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize;

                InitHeader();

                InitGrid();
                
                Controls.Add(TableLabel, 0, 0); 
                Controls.Add(ButtonPanel, 1, 0);
                Controls.Add(Grid, 0, 1);

                SetColumnSpan(Grid, 2);
            }
            private void Grid_CellClick(object sender, DataGridViewCellEventArgs evt)
            {
                if (ReferencedBlockList != null && evt.RowIndex > -1 && Grid.Rows[evt.RowIndex] is AttributeRow row
                    && row.IsReferenceSource && evt.ColumnIndex != ActivationColumn.Index)
                {
                    if (ItemSelector.ShowDialog(this, ReferencedBlockList.Select(block => new SelectorItem(block, block.BuildRootId())).ToArray()) == DialogResult.OK)
                    {
                        EcfBlock inheritor = ItemSelector.SelectedItem.Item as EcfBlock;
                        row.SetActive(true);
                        row.SetInheritor(inheritor);
                        InheritorChanged?.Invoke(inheritor, null);
                    }
                }
            }
            private void Grid_CellContentClick(object sender, DataGridViewCellEventArgs evt)
            {
                if (evt.RowIndex > -1 && evt.ColumnIndex == ActivationColumn.Index && Grid.Rows[evt.RowIndex] is AttributeRow row)
                {
                    Grid.EndEdit();
                    bool activ = row.IsActive();
                    if (row.IsReferenceSource && ReferencedBlockList != null)
                    {
                        if (activ)
                        {
                            if (ItemSelector.ShowDialog(this, ReferencedBlockList.Select(block => new SelectorItem(block, block.BuildRootId())).ToArray()) == DialogResult.OK)
                            {
                                EcfBlock inheritor = ItemSelector.SelectedItem.Item as EcfBlock;
                                row.SetInheritor(inheritor);
                                InheritorChanged?.Invoke(inheritor, null);
                            }
                        }
                        else
                        {
                            row.SetInheritor(null);
                            row.Inactivate();
                            InheritorChanged?.Invoke(null, null);
                        }
                    }
                    else
                    {
                        if (activ)
                        {
                            if (row.ItemDef.HasValue)
                            {
                                ActivateNewValue(row, 0);
                            }
                        }
                        else
                        {
                            row.Inactivate();
                        }
                    }
                }
            }
            private void AddValueButton_Click(object sender, EventArgs evt)
            {
                if (TryGetSelection(out DataGridViewCell cell, out AttributeRow row) && row.ItemDef.HasValue)
                {
                    row.SetActive(true);
                    ActivateNewValue(row, cell.ColumnIndex + 1);
                    Grid.EndEdit();
                }
            }
            private void RemoveValueButton_Click(object sender, EventArgs evt)
            {
                if (TryGetSelection(out DataGridViewCell cell, out AttributeRow row))
                {
                    if (row.GetValues().Count > 1)
                    {
                        row.DeactivateLastUsedCell(cell.ColumnIndex);
                    }
                    else if (row.ItemDef.IsOptional)
                    {
                        row.SetActive(false);
                        row.Inactivate();
                    }
                    Grid.EndEdit();
                }
            }

            // publics
            public void GenerateAttributes(FormatDefinition definition, ReadOnlyCollection<ItemDefinition> attributes)
            {
                AttributeRow[] rows = attributes.Select(attr => new AttributeRow(attr, definition)).ToArray();
                Grid.SuspendLayout();
                Grid.Rows.Clear();
                Grid.Columns.Clear();
                Grid.Columns.Add(ActivationColumn);
                Grid.Columns.Add(NameColumn);
                Grid.Columns.Add(InfoColumn);
                Grid.Rows.AddRange(rows);
                Grid.ResumeLayout();
            }
            public void UpdateAttributes(EcfStructureItem presetItem, EcfBlock inheritorBlock)
            {
                List<EcfAttribute> presentAttributes = null;
                if (presetItem is EcfParameter param)
                {
                    presentAttributes = param.Attributes.ToList();
                }
                else if (presetItem is EcfBlock block)
                {
                    presentAttributes = block.Attributes.ToList();
                }
                Grid.SuspendLayout();
                foreach (DataGridViewRow row in Grid.Rows)
                {
                    if (row is AttributeRow attrRow)
                    {
                        EcfAttribute attribute = presentAttributes?.FirstOrDefault(attr => attr.Key.Equals(attrRow.ItemDef.Name));
                        EcfValueGroup group = attribute?.ValueGroups.FirstOrDefault();
                        bool activation = !attrRow.ItemDef.IsOptional || attribute != null;
                        if (activation && attrRow.ItemDef.HasValue && (group?.Values.Count ?? 0) < 1)
                        {
                            group = new EcfValueGroup(string.Empty);
                        }
                        UpdateValueColumns(group);
                        if (attrRow.IsReferenceSource)
                        {
                            attrRow.InitRow(activation, inheritorBlock);
                        }
                        else if (group != null)
                        {
                            attrRow.InitRow(activation, group.Values.ToArray());
                        }
                        else
                        {
                            attrRow.InitRow(activation);
                        }
                    }
                }
                Grid.AutoResizeColumns();
                Grid.ResumeLayout();
            }
            public List<string> ValidateAttributeValues()
            {
                List<string> errors = new List<string>();
                foreach (DataGridViewRow row in Grid.Rows)
                {
                    if (row is AttributeRow attr && attr.IsActive() && attr.ItemDef.HasValue)
                    {
                        List<EcfValueGroup> values = new List<EcfValueGroup>() { new EcfValueGroup(attr.GetValues()) };
                        foreach (EcfError error in CheckValuesValid(values, attr.ItemDef, attr.FormDef, EcfErrorGroups.Editing))
                        {
                            errors.Add(string.Format("{0} '{1}' {2} '{3}': {4}", TitleRecources.Generic_Attribute, attr.ItemDef.Name,
                                TextRecources.Generic_HasError, GetLocalizedEnum(error.Type), error.Info));
                        }
                    }
                }
                return errors;
            }
            public List<string> ValidateIdName(List<EcfBlock> identifyingBlockList, List<EcfBlock> referencingBlockList, List<EcfBlock> referencedBlockList, EcfBlock presetBlock)
            {
                List<string> errors = new List<string>();
                if (identifyingBlockList != null)
                {
                    AttributeRow idRow = Grid.Rows.Cast<AttributeRow>().FirstOrDefault(row => row.IsIdentification);
                    if (idRow != null)
                    {
                        string value = idRow.GetValues().FirstOrDefault();
                        if (value != null)
                        {
                            foreach (EcfBlock block in identifyingBlockList.Where(block => !(presetBlock?.Equals(block) ?? false)
                                && value.Equals(block.GetAttributeFirstValue(idRow.ItemDef.Name))))
                            {
                                errors.Add(string.Format("{0} '{1}' {2} {3}", TextRecources.EcfItemEditingDialog_TheIdAttributeValue,
                                    value, TextRecources.EcfItemEditingDialog_IsAlreadyUsedBy, block.BuildRootId()));
                            }
                        }
                    }
                }
                if (referencedBlockList != null)
                {
                    AttributeRow nameRow = Grid.Rows.Cast<AttributeRow>().FirstOrDefault(row => row.IsReferenceTarget);
                    if (nameRow != null)
                    {
                        string value = nameRow.GetValues().FirstOrDefault();
                        if (value != null)
                        {
                            if (referencingBlockList != null && referencingBlockList.Any(listedBlock => value.Equals(listedBlock.GetRefSource())))
                            {
                                foreach (EcfBlock block in referencedBlockList.Where(block => !(presetBlock?.Equals(block) ?? false)
                                    && value.Equals(block.GetAttributeFirstValue(nameRow.ItemDef.Name))))
                                {
                                    errors.Add(string.Format("{0} '{1}' {2} {3}", TextRecources.EcfItemEditingDialog_TheNameAttributeValue,
                                        value, TextRecources.EcfItemEditingDialog_IsAlreadyUsedBy, block.BuildRootId()));
                                }
                            }
                        }
                    }
                }
                return errors;
            }
            public List<string> ValidateRefTarget(List<EcfBlock> referencingBlockList, EcfBlock presetBlock)
            {
                List<string> errors = new List<string>();
                if (referencingBlockList != null && presetBlock != null)
                {
                    AttributeRow targetRefRow = Grid.Rows.Cast<AttributeRow>().FirstOrDefault(row => row.IsReferenceTarget);
                    if (targetRefRow != null)
                    {
                        string oldTargetValue = presetBlock.GetAttributeFirstValue(targetRefRow.FormDef.BlockReferenceTargetAttribute);
                        if (oldTargetValue != null && !oldTargetValue.Equals(targetRefRow.GetValues().FirstOrDefault()))
                        {
                            foreach (EcfBlock block in referencingBlockList.Where(block => 
                                block.GetAttributeFirstValue(targetRefRow.FormDef.BlockReferenceSourceAttribute).Equals(oldTargetValue)))
                            {
                                errors.Add(string.Format("{0} '{1}' {2} {3}", TextRecources.EcfItemEditingDialog_TheOldNameAttributeValue,
                                    oldTargetValue, TextRecources.EcfItemEditingDialog_IsStillReferencedBy, block.BuildRootId()));
                            }
                        }
                    }
                }
                return errors;
            }
            public List<string> ValidateRefSource(List<EcfBlock> referencedBlockList)
            {
                List<string> errors = new List<string>();
                if (referencedBlockList != null)
                {
                    AttributeRow sourceRefRow = Grid.Rows.Cast<AttributeRow>().FirstOrDefault(row => row.IsReferenceSource);
                    if (sourceRefRow != null && sourceRefRow.IsActive())
                    {
                        string value = sourceRefRow.GetValues().FirstOrDefault();
                        int targetBlockCount = referencedBlockList.Count(block => block.GetRefTarget().Equals(value));
                        if (targetBlockCount < 1)
                        {
                            errors.Add(string.Format("{0} '{1}' {2}", TextRecources.EcfItemEditingDialog_TheReferencedItem,
                                sourceRefRow.Inheritor?.BuildRootId(), TextRecources.EcfItemEditingDialog_CouldNotBeFoundInTheItemList));
                        }
                        else if (targetBlockCount > 1)
                        {
                            errors.Add(string.Format("{0} '{1}' {2}", TextRecources.EcfItemEditingDialog_TheReferencedItem,
                                sourceRefRow.Inheritor?.BuildRootId(), TextRecources.EcfItemEditingDialog_HasNoUniqueIdentifier));
                        }
                    }
                }
                return errors;
            }
            public List<EcfAttribute> PrepareResultAttributes()
            {
                List<EcfAttribute> attributes = new List<EcfAttribute>();
                foreach (DataGridViewRow row in Grid.Rows)
                {
                    if (row is AttributeRow attr && attr.IsActive())
                    {
                        attributes.Add(new EcfAttribute(attr.ItemDef.Name, attr.GetValues()));
                    }
                }
                return attributes;
            }
            public EcfBlock GetInheritor()
            {
                return Grid.Rows.Cast<AttributeRow>().FirstOrDefault(row => row.IsReferenceSource)?.Inheritor;
            }

            // privates
            private void InitHeader()
            {
                TableLabel.AutoSize = true;
                TableLabel.Dock = DockStyle.Fill;
                TableLabel.Text = TitleRecources.Generic_Attributes;
                TableLabel.TextAlign = ContentAlignment.MiddleLeft;

                ButtonPanel.AutoSize = true;
                ButtonPanel.Dock = DockStyle.Fill;
                ButtonPanel.FlowDirection = FlowDirection.RightToLeft;

                AddValueButton.AutoSize = true;
                AddValueButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                AddValueButton.FlatAppearance.BorderSize = 0;
                AddValueButton.FlatStyle = FlatStyle.Flat;
                AddValueButton.Image = IconRecources.Icon_AddValue;
                AddValueButton.UseVisualStyleBackColor = true;

                RemoveValueButton.AutoSize = true;
                RemoveValueButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                RemoveValueButton.FlatAppearance.BorderSize = 0;
                RemoveValueButton.FlatStyle = FlatStyle.Flat;
                RemoveValueButton.Image = IconRecources.Icon_RemoveValue;
                RemoveValueButton.UseVisualStyleBackColor = true;

                Tip.SetToolTip(AddValueButton, TextRecources.EcfItemEditingDialog_ToolTip_AddValue);
                Tip.SetToolTip(RemoveValueButton, TextRecources.EcfItemEditingDialog_ToolTip_RemoveValue);

                AddValueButton.Click += AddValueButton_Click;
                RemoveValueButton.Click += RemoveValueButton_Click;

                ButtonPanel.Controls.Add(RemoveValueButton);
                ButtonPanel.Controls.Add(AddValueButton);
            }
            private void InitGrid()
            {
                Grid.Dock = DockStyle.Fill;
                Grid.EditMode = DataGridViewEditMode.EditOnKeystroke;
                Grid.MultiSelect = false;
                Grid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
                Grid.SelectionMode = DataGridViewSelectionMode.CellSelect;

                ActivationColumn.HeaderText = TitleRecources.Generic_Active;
                ActivationColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                NameColumn.DefaultCellStyle.BackColor = Color.LightGray;
                NameColumn.HeaderText = TitleRecources.Generic_Name;
                NameColumn.ReadOnly = true;
                NameColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                InfoColumn.DefaultCellStyle.BackColor = Color.LightGray;
                InfoColumn.HeaderText = TitleRecources.Generic_Info;
                InfoColumn.ReadOnly = true;
                InfoColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                Grid.CellClick += Grid_CellClick;
                Grid.CellContentClick += Grid_CellContentClick;
            }
            private void UpdateValueColumns(EcfValueGroup group)
            {
                int maxCount = 0;
                if (group != null)
                {
                    maxCount = group.Values.Count;
                }
                while (Grid.Columns.Count - PrefixColumnCount < maxCount)
                {
                    AddValueColumn();
                }
            }
            private void ActivateNewValue(AttributeRow row, int newIndex)
            {
                if (!row.ActivateNextFreeCell(newIndex))
                {
                    AddValueColumn();
                    row.ActivateNextFreeCell(newIndex);
                }
            }
            private void AddValueColumn()
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn()
                {
                    HeaderText = string.Format("{0} {1}", TitleRecources.Generic_Value, Grid.Columns.Count - PrefixColumnCount + 1),
                    ReadOnly = true,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                };
                column.DefaultCellStyle.BackColor = Color.LightGray;
                Grid.Columns.Add(column);
            }
            private bool TryGetSelection(out DataGridViewCell cell, out AttributeRow row)
            {
                cell = Grid.SelectedCells.Cast<DataGridViewCell>().FirstOrDefault();
                row = cell?.OwningRow as AttributeRow;
                return cell != null;
            }
            protected override void Dispose(bool disposing)
            {
                Tip.Dispose();
                base.Dispose(disposing);
            }

            //sub classes
            private class AttributeRow : DataGridViewRow
            {
                public FormatDefinition FormDef { get; }
                public ItemDefinition ItemDef { get; }

                public bool IsIdentification { get; } = false;
                public bool IsReferenceSource { get; } = false;
                public bool IsReferenceTarget { get; } = false;
                public EcfBlock Inheritor { get; private set; } = null;

                private string ReferenceTargetAttribute { get; } = string.Empty;

                private int PrefixColumnCount { get; } = 0;
                private DataGridViewCheckBoxCell ActivationCell { get; } = new DataGridViewCheckBoxCell();
                private DataGridViewTextBoxCell NameCell { get; } = new DataGridViewTextBoxCell();
                private DataGridViewTextBoxCell InfoCell { get; } = new DataGridViewTextBoxCell();

                public AttributeRow(ItemDefinition definition, FormatDefinition format) : base()
                {
                    Cells.Add(ActivationCell);
                    Cells.Add(NameCell);
                    Cells.Add(InfoCell);
                    PrefixColumnCount = Cells.Count;

                    ItemDef = definition;
                    FormDef = format;

                    IsIdentification = definition.Name.Equals(format.BlockIdAttribute);
                    IsReferenceSource = definition.Name.Equals(format.BlockReferenceSourceAttribute);
                    IsReferenceTarget = definition.Name.Equals(format.BlockReferenceTargetAttribute);

                    ReferenceTargetAttribute = format.BlockReferenceTargetAttribute;

                    ActivationCell.ReadOnly = !definition.IsOptional;
                    if (ActivationCell.ReadOnly) { ActivationCell.Style.BackColor = Color.LightGray; }

                    NameCell.Value = definition.Name;
                    InfoCell.Value = definition.Info;
                }

                // publics
                public void InitRow(bool active, params string[] values)
                {
                    ActivationCell.Value = active;
                    SetValues(values);
                }
                public void InitRow(bool active, EcfBlock inheritor)
                {
                    ActivationCell.Value = active;
                    SetInheritor(inheritor);
                }
                public void SetInheritor(EcfBlock inheritor)
                {
                    Inheritor = inheritor;
                    if (inheritor != null)
                    {
                        SetValues(inheritor?.GetAttributeFirstValue(ReferenceTargetAttribute));
                    }
                    else
                    {
                        SetValues();
                    }
                }
                public bool IsActive()
                {
                    return Convert.ToBoolean(ActivationCell.Value);
                }
                public void SetActive(bool state)
                {
                    ActivationCell.Value = state;
                }
                public List<string> GetValues()
                {
                    return Cells.Cast<DataGridViewCell>().Skip(PrefixColumnCount).Where(cell => cell.Tag != null)
                        .Select(cell => Convert.ToString(cell.Value)).ToList();
                }
                public bool ActivateNextFreeCell(int newIndex)
                {
                    DataGridViewCell nextCell = Cells.Cast<DataGridViewCell>().Skip(PrefixColumnCount).FirstOrDefault(cell => cell.Tag == null);
                    if (nextCell == null) { return false; }
                    newIndex = Math.Max(newIndex, PrefixColumnCount);
                    newIndex = Math.Min(newIndex, Cells.Count - 1);
                    for (int index = nextCell.ColumnIndex; index > newIndex; index--)
                    {
                        Cells[index].Value = Cells[index - 1].Value;
                    }
                    Cells[newIndex].Value = string.Empty;
                    ActivateCell(nextCell);
                    return true;
                }
                public bool DeactivateLastUsedCell(int oldIndex)
                {
                    DataGridViewCell lastCell = Cells.Cast<DataGridViewCell>().Skip(PrefixColumnCount).LastOrDefault(cell => cell.Tag != null);
                    if (lastCell == null) { return false; }
                    oldIndex = Math.Max(oldIndex, PrefixColumnCount);
                    oldIndex = Math.Min(oldIndex, Cells.Count - 1);
                    for (int index = oldIndex; index < lastCell.ColumnIndex; index++)
                    {
                        Cells[index].Value = Cells[index + 1].Value;
                    }
                    lastCell.Value = string.Empty;
                    DeactivateCell(lastCell);
                    return true;
                }
                public void Inactivate()
                {
                    foreach (DataGridViewCell cell in Cells.Cast<DataGridViewCell>().Skip(PrefixColumnCount))
                    {
                        DeactivateCell(cell);
                    }
                }

                // privates
                private void SetValues(params string[] values)
                {
                    foreach (DataGridViewCell valueCell in Cells.Cast<DataGridViewCell>().Skip(PrefixColumnCount))
                    {
                        int index = valueCell.ColumnIndex - PrefixColumnCount;
                        if (index < values.Length)
                        {
                            valueCell.Value = values[index];
                            ActivateCell(valueCell);
                        }
                        else
                        {
                            valueCell.Value = string.Empty;
                            DeactivateCell(valueCell);
                        }
                    }
                }
                private void ActivateCell(DataGridViewCell cell)
                {
                    cell.Style.BackColor = Color.White;
                    cell.ReadOnly = IsReferenceSource;
                    cell.Tag = true;
                }
                private void DeactivateCell(DataGridViewCell cell)
                {
                    cell.Style.BackColor = Color.LightGray;
                    cell.ReadOnly = true;
                    cell.Tag = null;
                }
            }
        }
        private class ParameterPanel : TableLayoutPanel
        {
            public ParameterModes Mode { get; }
            
            private Label TableLabel { get; } = new Label();
            private FlowLayoutPanel ButtonPanel { get; } = new FlowLayoutPanel();
            private Button AddValueButton { get; } = new Button();
            private Button RemoveValueButton { get; } = new Button();
            private Button AddGroupButton { get; } = new Button();
            private Button RemoveGroupButton { get; } = new Button();
            private OptimizedDataGridView Grid { get; } = new OptimizedDataGridView();

            private ItemDefinition ParameterValuesDefinition { get; set; } = null;

            private int PrefixColumnCount { get; set; } = 0;
            private DataGridViewCheckBoxColumn ActivationColumn { get; } = new DataGridViewCheckBoxColumn();
            private DataGridViewCheckBoxColumn InheritColumn { get; } = new DataGridViewCheckBoxColumn();
            private DataGridViewTextBoxColumn NameColumn { get; } = new DataGridViewTextBoxColumn();
            private DataGridViewTextBoxColumn InfoColumn { get; } = new DataGridViewTextBoxColumn();
            private DataGridViewTextBoxColumn CommentColumn { get; } = new DataGridViewTextBoxColumn();

            private ToolTip Tip { get; } = new ToolTip();

            public enum ParameterModes
            {
                Value,
                Matrix,
                Block,
            }

            public ParameterPanel(ParameterModes mode) : base()
            {
                Mode = mode;
                InitPanel();
            }

            // events
            private void InitPanel()
            {
                AutoSize = true;
                ColumnCount = 2;
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
                Dock = DockStyle.Fill;
                RowCount = 2;
                RowStyles.Add(new RowStyle(SizeType.AutoSize));
                RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize;

                InitHeader();

                InitGrid();

                Controls.Add(TableLabel, 0, 0);
                Controls.Add(ButtonPanel, 1, 0);
                Controls.Add(Grid, 0, 1);

                SetColumnSpan(Grid, 2);
            }
            private void Grid_CellContentClick(object sender, DataGridViewCellEventArgs evt)
            {
                if (evt.RowIndex > -1 && evt.ColumnIndex > -1 && Grid.Columns[evt.ColumnIndex] == ActivationColumn && Grid.Rows[evt.RowIndex] is ParameterMatrixRow row)
                {
                    Grid.EndEdit();
                    if (row.IsActive())
                    {
                        if (row.ItemDef.HasValue)
                        {
                            ActivateNewValue(row, 0, true);
                        }
                    }
                    else
                    {
                        row.Inactivate();
                    }
                }
            }
            private void AddValueButton_Click(object sender, EventArgs evt)
            {
                if (TryGetSelection(out List<DataGridViewColumn> columns, out List<DataGridViewRow> rows))
                {
                    switch (Mode)
                    {
                        case ParameterModes.Value: AddGroupValues(columns.FirstOrDefault(), rows); break;
                        case ParameterModes.Block: AddMatrixValues(columns.FirstOrDefault(), rows); break;
                        case ParameterModes.Matrix: AddMatrixColumns(columns, rows.FirstOrDefault()); break;
                        default: throw new InvalidOperationException(string.Format("Add value operation not defined for {0}", Mode));
                    }
                    Grid.EndEdit();
                }
            }
            private void RemoveValueButton_Click(object sender, EventArgs evt)
            {
                if (TryGetSelection(out List<DataGridViewColumn> columns, out List<DataGridViewRow> rows))
                {
                    switch (Mode)
                    {
                        case ParameterModes.Value: RemoveGroupValues(columns.FirstOrDefault(), rows); break;
                        case ParameterModes.Block: RemoveMatrixValues(columns.FirstOrDefault(), rows); break;
                        case ParameterModes.Matrix: RemoveMatrixColumns(columns, rows.FirstOrDefault()); break;
                        default: throw new InvalidOperationException(string.Format("Remove value operation not defined for {0}", Mode));
                    }
                    Grid.EndEdit();
                }
            }
            private void AddGroupButton_Click(object sender, EventArgs evt)
            {
                if (TryGetSelection(out List<DataGridViewColumn> _, out List<DataGridViewRow> rows))
                {
                    AddGroupRows(rows);
                    Grid.EndEdit();
                }
            }
            private void RemoveGroupButton_Click(object sender, EventArgs evt)
            {
                if (TryGetSelection(out List<DataGridViewColumn> _, out List <DataGridViewRow> rows))
                {
                    RemoveGroupRows(rows);
                    Grid.EndEdit();
                }
            }

            // publics
            public void TryFocusFirstCell()
            {
                if (Grid.Rows.Count > 0 && Grid.Rows[0].Cells.Count > 0)
                {
                    Grid.CurrentCell = Grid.Rows[0].Cells[0];
                    Grid.BeginEdit(true);
                }
            }
            public void GenerateParameterMatrix(FormatDefinition definition, ReadOnlyCollection<ItemDefinition> parameters)
            {
                if (Mode != ParameterModes.Block) { throw new InvalidOperationException(string.Format("Not allowed in {0} mode", Mode)); }
                Grid.SuspendLayout();
                Grid.Rows.Clear();
                Grid.Columns.Clear();
                Grid.Columns.Add(ActivationColumn);
                Grid.Columns.Add(InheritColumn);
                Grid.Columns.Add(NameColumn);
                Grid.Columns.Add(InfoColumn);
                Grid.Columns.Add(CommentColumn);
                Grid.Rows.AddRange(parameters.Select(parameter => new ParameterMatrixRow(definition, parameter)).ToArray());
                Grid.ResumeLayout();
                PrefixColumnCount = Grid.Columns.Count;
            }
            public void GenerateParameterMatrix(FormatDefinition definition, List<EcfParameter> parameters)
            {
                if (Mode != ParameterModes.Matrix) { throw new InvalidOperationException(string.Format("Not allowed in {0} mode", Mode)); }
                Grid.SuspendLayout();
                Grid.Rows.Clear();
                Grid.Columns.Clear();
                Grid.Columns.Add(NameColumn);
                Grid.Columns.Add(InfoColumn);
                Grid.Columns.Add(CommentColumn);
                Grid.Rows.AddRange(parameters.Select(parameter => new ParameterMatrixRow(definition, parameter)).ToArray());
                Grid.ResumeLayout();
                PrefixColumnCount = Grid.Columns.Count;
            }
            public void UpdateParameterValues(ItemDefinition parameterDefinition, EcfParameter presetParameter)
            {
                if (Mode != ParameterModes.Value) { throw new InvalidOperationException(string.Format("Not allowed in {0} mode", Mode)); }
                ParameterValuesDefinition = parameterDefinition;
                Grid.SuspendLayout();
                if (presetParameter != null)
                {
                    Grid.Rows.Clear();
                    Grid.Columns.Clear();
                    if (presetParameter.HasValue())
                    {
                        foreach (EcfValueGroup group in presetParameter.ValueGroups)
                        {
                            UpdateValueColumns(group, false);
                            AddGroupRow(parameterDefinition, presetParameter, null, group.Values.ToArray());
                        }
                    }
                }
                if (ParameterValuesDefinition.HasValue)
                {
                    if (Grid.Rows.Count < 1)
                    {
                        AddValueColumn(true, false, 0);
                        AddGroupRow(parameterDefinition, presetParameter, null, string.Empty);
                    }
                }
                else
                {
                    Grid.Rows.Clear();
                    Grid.Columns.Clear();
                }
                UpdateParameterValuesColumnNumbering();
                UpdateParameterValuesRowNumbering();
                Grid.AutoResizeColumns();
                Grid.ResumeLayout();
            }
            public void UpdateParameterMatrix(EcfBlock presetBlock)
            {
                if (Mode != ParameterModes.Block) { throw new InvalidOperationException(string.Format("Not allowed in {0} mode", Mode)); }
                List<EcfParameter> presentParameters = presetBlock?.ChildItems.Where(child => child is EcfParameter).Cast<EcfParameter>().ToList();
                Grid.SuspendLayout();
                foreach (DataGridViewRow row in Grid.Rows)
                {
                    if (row is ParameterMatrixRow paramRow)
                    {
                        EcfParameter parameter = presentParameters?.FirstOrDefault(param => param.Key.Equals(paramRow.ItemDef.Name));
                        EcfValueGroup group = parameter?.ValueGroups.FirstOrDefault();
                        bool activation = !paramRow.ItemDef.IsOptional || parameter != null;
                        if (activation && paramRow.ItemDef.HasValue && (group?.Values.Count ?? 0) < 1)
                        {
                            group = new EcfValueGroup(string.Empty);
                        }
                        UpdateValueColumns(group, true);
                        if (group != null)
                        {
                            paramRow.InitRow(activation, parameter, group.Values.ToArray());
                        }
                        else
                        {
                            paramRow.InitRow(activation, parameter);
                        }
                    }
                }
                UpdateParameterValuesColumnNumbering();
                Grid.Sort(ActivationColumn, ListSortDirection.Descending);
                Grid.AutoResizeColumns();
                Grid.ResumeLayout();
            }
            public void UpdateParameterMatrix()
            {
                if (Mode != ParameterModes.Matrix) { throw new InvalidOperationException(string.Format("Not allowed in {0} mode", Mode)); }
                Grid.SuspendLayout();
                foreach (DataGridViewRow row in Grid.Rows)
                {
                    if (row is ParameterMatrixRow paramRow)
                    {
                        EcfValueGroup group = paramRow.PresetParameter.ValueGroups.FirstOrDefault();
                        UpdateValueColumns(group, true);
                        if (group != null)
                        {
                            paramRow.InitRow(true, paramRow.PresetParameter, group.Values.ToArray());
                        }
                        else
                        {
                            paramRow.InitRow(true, paramRow.PresetParameter);
                        }
                    }
                }
                UpdateParameterValuesColumnNumbering();
                Grid.AutoResizeColumns();
                Grid.ResumeLayout();
            }
            public void UpdateParameterInheritance(EcfBlock inheritor)
            {
                if (Mode != ParameterModes.Block) { throw new InvalidOperationException(string.Format("Not allowed in {0} mode", Mode)); }
                Grid.SuspendLayout();
                foreach (DataGridViewRow row in Grid.Rows)
                {
                    if (row is ParameterMatrixRow paramRow)
                    {
                        EcfParameter parameter = null;
                        inheritor?.IsInheritingParameter(paramRow.ItemDef.Name, out parameter);
                        paramRow.UpdateInherited(parameter);
                    }
                }
                Grid.AutoResizeColumns();
                Grid.ResumeLayout();
            }
            public List<string> ValidateParameterValues(FormatDefinition definition)
            {
                List<string> errors = new List<string>();
                if (ParameterValuesDefinition.HasValue)
                {
                    List<EcfValueGroup> checkGroups = Grid.Rows.Cast<ParameterGroupRow>().Select(row => new EcfValueGroup(row.GetValues())).ToList();

                    foreach (EcfError error in CheckValuesValid(checkGroups, ParameterValuesDefinition, definition, EcfErrorGroups.Editing))
                    {
                        errors.Add(string.Format("{0} {1} '{2}': {3}", TitleRecources.Generic_Value, TextRecources.Generic_HasError, GetLocalizedEnum(error.Type), error.Info));
                    }
                }
                return errors;
            }
            public List<string> ValidateParameterMatrix()
            {
                List<string> errors = new List<string>();
                foreach (DataGridViewRow row in Grid.Rows)
                {
                    if (row is ParameterMatrixRow param && param.IsActive() && param.ItemDef.HasValue)
                    {
                        List<EcfValueGroup> values = new List<EcfValueGroup>() { new EcfValueGroup(param.GetValues()) };
                        foreach (EcfError error in CheckValuesValid(values, param.ItemDef, param.FormDef, EcfErrorGroups.Editing))
                        {
                            errors.Add(string.Format("{0} '{1}' {2} '{3}': {4}", TitleRecources.Generic_Parameter, param.ItemDef.Name,
                                TextRecources.Generic_HasError, GetLocalizedEnum(error.Type), error.Info));
                        }
                    }
                }
                return errors;
            }
            public List<EcfValueGroup> PrepareResultValues()
            {
                List<EcfValueGroup> valueGroups = new List<EcfValueGroup>();
                if (ParameterValuesDefinition.HasValue)
                {
                    foreach (DataGridViewRow row in Grid.Rows)
                    {
                        if (row is ParameterGroupRow groupRow)
                        {
                            valueGroups.Add(new EcfValueGroup(groupRow.GetValues()));
                        }
                    }
                }
                return valueGroups;
            }
            public List<EcfParameter> PrepareResultParameters()
            {
                List<EcfParameter> parameters = new List<EcfParameter>();
                foreach (DataGridViewRow row in Grid.Rows)
                {
                    if (row is ParameterMatrixRow paramRow && paramRow.IsActive())
                    {
                        if (paramRow.PresetParameter == null)
                        {
                            parameters.Add(new EcfParameter(paramRow.ItemDef.Name, paramRow.GetValues(), null));
                        }
                        else
                        {
                            List<EcfValueGroup> valueGroups = new List<EcfValueGroup>()
                            {
                                new EcfValueGroup(paramRow.GetValues()),
                            };
                            paramRow.PresetParameter.ValueGroups.Skip(1).ToList().ForEach(group => valueGroups.Add(group));
                            paramRow.PresetParameter.ClearValues();
                            paramRow.PresetParameter.AddValue(valueGroups);
                            parameters.Add(paramRow.PresetParameter);
                        }
                    }
                }
                return parameters;
            }

            // privates
            private void InitHeader()
            {
                TableLabel.AutoSize = true;
                TableLabel.Dock = DockStyle.Fill;
                TableLabel.Text = Mode == ParameterModes.Value ? TitleRecources.Generic_Values : TitleRecources.Generic_Parameters;
                TableLabel.TextAlign = ContentAlignment.MiddleLeft;

                ButtonPanel.AutoSize = true;
                ButtonPanel.Dock = DockStyle.Fill;
                ButtonPanel.FlowDirection = FlowDirection.RightToLeft;

                AddValueButton.AutoSize = true;
                AddValueButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                AddValueButton.FlatAppearance.BorderSize = 0;
                AddValueButton.FlatStyle = FlatStyle.Flat;
                AddValueButton.Image = IconRecources.Icon_AddValue;
                AddValueButton.UseVisualStyleBackColor = true;

                RemoveValueButton.AutoSize = true;
                RemoveValueButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                RemoveValueButton.FlatAppearance.BorderSize = 0;
                RemoveValueButton.FlatStyle = FlatStyle.Flat;
                RemoveValueButton.Image = IconRecources.Icon_RemoveValue;
                RemoveValueButton.UseVisualStyleBackColor = true;

                AddGroupButton.AutoSize = true;
                AddGroupButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                AddGroupButton.FlatAppearance.BorderSize = 0;
                AddGroupButton.FlatStyle = FlatStyle.Flat;
                AddGroupButton.Image = IconRecources.Icon_AddValueGroup;
                AddGroupButton.UseVisualStyleBackColor = true;

                RemoveGroupButton.AutoSize = true;
                RemoveGroupButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                RemoveGroupButton.FlatAppearance.BorderSize = 0;
                RemoveGroupButton.FlatStyle = FlatStyle.Flat;
                RemoveGroupButton.Image = IconRecources.Icon_RemoveValueGroup;
                RemoveGroupButton.UseVisualStyleBackColor = true;

                Tip.SetToolTip(AddValueButton, TextRecources.EcfItemEditingDialog_ToolTip_AddValue);
                Tip.SetToolTip(RemoveValueButton, TextRecources.EcfItemEditingDialog_ToolTip_RemoveValue);
                Tip.SetToolTip(AddGroupButton, TextRecources.EcfItemEditingDialog_ToolTip_AddValueGroup);
                Tip.SetToolTip(RemoveGroupButton, TextRecources.EcfItemEditingDialog_ToolTip_RemoveValueGroup);

                AddValueButton.Click += AddValueButton_Click;
                RemoveValueButton.Click += RemoveValueButton_Click;
                AddGroupButton.Click += AddGroupButton_Click;
                RemoveGroupButton.Click += RemoveGroupButton_Click;

                if (Mode == ParameterModes.Value)
                {
                    ButtonPanel.Controls.Add(RemoveGroupButton);
                    ButtonPanel.Controls.Add(AddGroupButton);
                }
                ButtonPanel.Controls.Add(RemoveValueButton);
                ButtonPanel.Controls.Add(AddValueButton);
            }
            private void InitGrid()
            {
                Grid.Dock = DockStyle.Fill;
                Grid.EditMode = DataGridViewEditMode.EditOnKeystroke;
                Grid.MultiSelect = true;
                Grid.RowHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
                Grid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
                Grid.SelectionMode = DataGridViewSelectionMode.CellSelect;

                ActivationColumn.HeaderText = TitleRecources.Generic_Active;
                ActivationColumn.SortMode = DataGridViewColumnSortMode.Automatic;
                ActivationColumn.Frozen = true;

                InheritColumn.DefaultCellStyle.BackColor = Color.LightGray;
                InheritColumn.HeaderText = TitleRecources.Generic_Inherited;
                InheritColumn.ReadOnly = true;
                InheritColumn.SortMode = DataGridViewColumnSortMode.Automatic;
                InheritColumn.Frozen = true;

                NameColumn.DefaultCellStyle.BackColor = Color.LightGray;
                NameColumn.HeaderText = TitleRecources.Generic_Name;
                NameColumn.ReadOnly = true;
                NameColumn.SortMode = DataGridViewColumnSortMode.Automatic;
                NameColumn.Frozen = true;

                InfoColumn.DefaultCellStyle.BackColor = Color.LightGray;
                InfoColumn.HeaderText = TitleRecources.Generic_Info;
                InfoColumn.ReadOnly = true;
                InfoColumn.SortMode = DataGridViewColumnSortMode.Automatic;

                CommentColumn.DefaultCellStyle.BackColor = Color.LightGray;
                CommentColumn.HeaderText = TitleRecources.Generic_Comment;
                CommentColumn.ReadOnly = true;
                CommentColumn.SortMode = DataGridViewColumnSortMode.Automatic;

                Grid.CellContentClick += Grid_CellContentClick;
            }
            private void UpdateValueColumns(EcfValueGroup group, bool columnSortable)
            {
                int maxCount = 0;
                if (group != null)
                {
                    maxCount = group.Values.Count;
                }
                while (Grid.Columns.Count - PrefixColumnCount < maxCount)
                {
                    AddValueColumn(true, columnSortable, Grid.Columns.Count);
                }
            }
            private void UpdateParameterValuesColumnNumbering()
            {
                foreach (DataGridViewColumn column in Grid.Columns.Cast<DataGridViewColumn>().Skip(PrefixColumnCount))
                {
                    column.HeaderText = BuildValueColumnName(Grid.Columns.IndexOf(column));
                }
            }
            private void UpdateParameterValuesRowNumbering()
            {
                if (Grid.Rows.Count == 1)
                {
                    Grid.Rows[0].HeaderCell.Value = string.Empty;
                }
                else
                {
                    foreach (DataGridViewRow row in Grid.Rows)
                    {
                        row.HeaderCell.Value = string.Format("{0} {1}", TitleRecources.Generic_Group, row.Index + 1);
                    }
                }
            }
            private void ActivateNewValue(ParameterRow row, int newIndex, bool columnSortable)
            {
                if (!row.ActivateNextFreeCell(newIndex))
                {
                    AddValueColumn(true, columnSortable, Grid.Columns.Count);
                    UpdateParameterValuesColumnNumbering();
                    row.ActivateNextFreeCell(newIndex);
                }
            }
            private void AddValueColumn(bool readOnly, bool sortable, int newIndex)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn()
                {
                    ReadOnly = readOnly,
                    SortMode = sortable ? DataGridViewColumnSortMode.Automatic : DataGridViewColumnSortMode.NotSortable,
                };
                if (readOnly)
                {
                    column.DefaultCellStyle.BackColor = Color.LightGray;
                }
                Grid.Columns.Insert(newIndex, column);
            }
            private string BuildValueColumnName(int index)
            {
                return string.Format("{0} {1}", TitleRecources.Generic_Value, index - PrefixColumnCount + 1);
            }
            private void AddGroupRow(ItemDefinition parameterDefinition, EcfParameter presetParameter, int? newIndex, params string[] values)
            {
                ParameterGroupRow row = new ParameterGroupRow(parameterDefinition, presetParameter);
                Grid.Rows.AddRange(row);
                if (newIndex.HasValue)
                {
                    for (int index = Grid.Rows.Count - 1; index > newIndex.Value; index--)
                    {
                        if (Grid.Rows[index] is ParameterGroupRow targetRow && Grid.Rows[index - 1] is ParameterGroupRow sourceRow)
                        {
                            targetRow.InitRow(sourceRow.GetValues().ToArray());
                        }
                    }
                    (Grid.Rows[newIndex.Value] as ParameterGroupRow).InitRow(values);
                }
                else
                {
                    row.InitRow(values);
                }
            }
            private bool TryGetSelection(out List<DataGridViewColumn> columns, out List<DataGridViewRow> rows)
            {
                columns = Grid.SelectedCells.Cast<DataGridViewCell>().Select(cell => cell.OwningColumn).Distinct().ToList();
                rows = Grid.SelectedCells.Cast<DataGridViewCell>().Select(cell => cell.OwningRow).Distinct().ToList();
                return columns.Count > 0 && rows.Count > 0;
            }
            private void AddGroupValues(DataGridViewColumn column, List<DataGridViewRow> rows)
            {
                rows.ForEach(row =>
                {
                    if (row is ParameterGroupRow groupRow && groupRow.ItemDef.HasValue)
                    {
                        ActivateNewValue(groupRow, column.Index + 1, false);
                    }
                });
            }
            private void AddMatrixValues(DataGridViewColumn column, List<DataGridViewRow> rows)
            {
                rows.ForEach(row =>
                {
                    if (row is ParameterMatrixRow matrixRow && matrixRow.ItemDef.HasValue)
                    {
                        matrixRow.SetActive(true);
                        ActivateNewValue(matrixRow, column.Index + 1, true);
                    }
                });
            }
            private void AddMatrixColumns(List<DataGridViewColumn> columns, DataGridViewRow row)
            {
                if (row is ParameterMatrixRow matrixRow && matrixRow.ItemDef.HasValue)
                {
                    columns.ForEach(column =>
                    {
                        int newIndex = column.Index + 1;
                        newIndex = Math.Max(newIndex, PrefixColumnCount);
                        newIndex = Math.Min(newIndex, Grid.Columns.Count);
                        AddValueColumn(false, true, newIndex);
                    });
                    UpdateParameterValuesColumnNumbering();
                }
            }
            private void RemoveGroupValues(DataGridViewColumn column, List<DataGridViewRow> rows)
            {
                rows.ForEach(row =>
                {
                    if (row is ParameterGroupRow groupRow)
                    {
                        if (groupRow.GetValues().Count > 1)
                        {
                            groupRow.DeactivateLastUsedCell(column.Index);
                        }
                    }
                });
            }
            private void RemoveMatrixValues(DataGridViewColumn column, List<DataGridViewRow> rows)
            {
                rows.ForEach(row =>
                {
                    if (row is ParameterMatrixRow matrixRow)
                    {
                        if (matrixRow.GetValues().Count > 1)
                        {
                            matrixRow.DeactivateLastUsedCell(column.Index);
                        }
                        else if (matrixRow.ItemDef.IsOptional)
                        {
                            matrixRow.SetActive(false);
                            matrixRow.Inactivate();
                        }
                    }
                });
            }
            private void RemoveMatrixColumns(List<DataGridViewColumn> columns, DataGridViewRow row)
            {
                columns.ForEach(column =>
                {
                    if (row is ParameterMatrixRow matrixRow)
                    {
                        if (matrixRow.GetValues().Count > 1)
                        {
                            Grid.Columns.Remove(column);
                        }
                    }
                });
                UpdateParameterValuesColumnNumbering();
            }
            private void AddGroupRows(List<DataGridViewRow> rows)
            {
                rows.ForEach(row =>
                {
                    if (row is ParameterGroupRow groupRow && groupRow.ItemDef.HasValue)
                    {
                        AddGroupRow(groupRow.ItemDef, groupRow.PresetParameter, groupRow.Index + 1, string.Empty);
                    }
                });
                UpdateParameterValuesRowNumbering();
            }
            private void RemoveGroupRows(List<DataGridViewRow> rows)
            {
                rows.ForEach(row =>
                {
                    if (row is ParameterGroupRow groupRow)
                    {
                        if (Grid.Rows.Count > 1)
                        {
                            Grid.Rows.Remove(groupRow);
                        }
                    }
                });
                UpdateParameterValuesRowNumbering();
            }
            protected override void Dispose(bool disposing)
            {
                Tip.Dispose();
                base.Dispose(disposing);
            }

            // subclasses
            private abstract class ParameterRow : DataGridViewRow
            {
                public ItemDefinition ItemDef { get; } = null;
                public EcfParameter PresetParameter { get; protected set; } = null;
                
                protected int PrefixColumnCount { get; set; } = 0;

                public ParameterRow(EcfParameter presetParameter) : base()
                {
                    ItemDef = presetParameter.Definition;
                    PresetParameter = presetParameter;
                }
                public ParameterRow(ItemDefinition item) : base()
                {
                    ItemDef = item;
                    PresetParameter = null;
                }

                // publics
                public List<string> GetValues()
                {
                    return Cells.Cast<DataGridViewCell>().Skip(PrefixColumnCount).Where(cell => cell.Tag != null)
                        .Select(cell => Convert.ToString(cell.Value)).ToList();
                }
                public bool ActivateNextFreeCell(int newIndex)
                {
                    DataGridViewCell nextCell = Cells.Cast<DataGridViewCell>().Skip(PrefixColumnCount).FirstOrDefault(cell => cell.Tag == null);
                    if (nextCell == null) { return false; }
                    newIndex = Math.Max(newIndex, PrefixColumnCount);
                    newIndex = Math.Min(newIndex, Cells.Count - 1);
                    for (int index = nextCell.ColumnIndex; index > newIndex; index--)
                    {
                        Cells[index].Value = Cells[index - 1].Value;
                    }
                    Cells[newIndex].Value = string.Empty;
                    ActivateCell(nextCell);
                    return true;
                }
                public bool DeactivateLastUsedCell(int oldIndex)
                {
                    DataGridViewCell lastCell = Cells.Cast<DataGridViewCell>().Skip(PrefixColumnCount).LastOrDefault(cell => cell.Tag != null);
                    if (lastCell == null) { return false; }
                    oldIndex = Math.Max(oldIndex, PrefixColumnCount);
                    oldIndex = Math.Min(oldIndex, Cells.Count - 1);
                    for (int index = oldIndex; index < lastCell.ColumnIndex; index++)
                    {
                        Cells[index].Value = Cells[index + 1].Value;
                    }
                    lastCell.Value = string.Empty;
                    DeactivateCell(lastCell);
                    return true;
                }

                // privates
                protected void SetValues(params string[] values)
                {
                    foreach (DataGridViewCell valueCell in Cells.Cast<DataGridViewCell>().Skip(PrefixColumnCount))
                    {
                        int index = valueCell.ColumnIndex - PrefixColumnCount;
                        if (index < values.Length)
                        {
                            valueCell.Value = values[index];
                            ActivateCell(valueCell);
                        }
                        else
                        {
                            valueCell.Value = string.Empty;
                            DeactivateCell(valueCell);
                        }
                    }
                }
                protected void ActivateCell(DataGridViewCell cell)
                {
                    cell.Style.BackColor = Color.White;
                    cell.ReadOnly = false;
                    cell.Tag = true;
                }
                protected void DeactivateCell(DataGridViewCell cell)
                {
                    cell.Style.BackColor = Color.LightGray;
                    cell.ReadOnly = true;
                    cell.Tag = null;
                }
            }
            private class ParameterGroupRow : ParameterRow
            {
                public ParameterGroupRow(ItemDefinition item, EcfParameter presetParameter) : base(item)
                {
                    PresetParameter = presetParameter;
                    PrefixColumnCount = 0;
                }

                public void InitRow(params string[] values)
                {
                    SetValues(values);
                }
            }
            private class ParameterMatrixRow : ParameterRow
            {
                public FormatDefinition FormDef { get; } = null;

                private DataGridViewCheckBoxCell ActivationCell { get; } = new DataGridViewCheckBoxCell();
                private DataGridViewCheckBoxCell InheritedCell { get; } = new DataGridViewCheckBoxCell();
                private DataGridViewTextBoxCell NameCell { get; } = new DataGridViewTextBoxCell();
                private DataGridViewTextBoxCell InfoCell { get; } = new DataGridViewTextBoxCell();
                private DataGridViewTextBoxCell CommentCell { get; } = new DataGridViewTextBoxCell();

                public ParameterMatrixRow(FormatDefinition format, EcfParameter presetParameter) : base(presetParameter)
                {
                    Cells.Add(NameCell);
                    Cells.Add(InfoCell);
                    Cells.Add(CommentCell);
                    PrefixColumnCount = Cells.Count;

                    FormDef = format;

                    ActivationCell.Value = true;
                    NameCell.Value = presetParameter.GetFullPath();
                    InfoCell.Value = ItemDef.Info;
                }
                public ParameterMatrixRow(FormatDefinition format, ItemDefinition item) : base(item)
                {
                    Cells.Add(ActivationCell);
                    Cells.Add(InheritedCell);
                    Cells.Add(NameCell);
                    Cells.Add(InfoCell);
                    Cells.Add(CommentCell);
                    PrefixColumnCount = Cells.Count;

                    FormDef = format;

                    ActivationCell.ReadOnly = !item.IsOptional;
                    if (ActivationCell.ReadOnly) { ActivationCell.Style.BackColor = Color.LightGray; }

                    NameCell.Value = item.Name;
                    InfoCell.Value = item.Info;
                }

                // publics
                public void InitRow(bool active, EcfParameter presetParameter, params string[] values)
                {
                    PresetParameter = presetParameter;
                    ActivationCell.Value = active;
                    InheritedCell.Value = false;
                    CommentCell.Value = presetParameter != null ? string.Join(" / ", presetParameter.Comments) : string.Empty;
                    SetValues(values);
                }
                public bool IsActive()
                {
                    return Convert.ToBoolean(ActivationCell.Value);
                }
                public void UpdateInherited(EcfParameter parameter)
                {
                    bool isInherited = parameter != null;
                    InheritedCell.Value = isInherited;
                    InheritedCell.ToolTipText = isInherited ? string.Format("{0}: {1}", TitleRecources.Generic_Values, string.Join(", ", parameter.GetAllValues())) : string.Empty;
                }
                public void SetActive(bool state)
                {
                    ActivationCell.Value = state;
                }
                public void Inactivate()
                {
                    foreach (DataGridViewCell cell in Cells.Cast<DataGridViewCell>().Skip(PrefixColumnCount))
                    {
                        DeactivateCell(cell);
                    }
                }

            }
        }
        private class CommentItemPanel : RichTextBox
        {
            private EcfComment PresetComment { get; set; } = null;

            public CommentItemPanel()
            {
                InitForms();
            }

            // private
            private void InitForms()
            {
                AcceptsTab = true;
                Dock = DockStyle.Fill;
                Text = "";
            }
            private void ActivateView()
            {
                UpdateView();
            }
            private void UpdateView()
            {
                if (PresetComment == null)
                {
                    Clear();
                }
                else
                {
                    Lines = PresetComment.Comments.ToArray();
                }
            }
            private List<string> ValidateInputs()
            {
                List<string> errors = new List<string>();
                if (Lines.Any(line => line.Equals(string.Empty)))
                {
                    errors.Add(TextRecources.EcfItemEditingDialog_CommentItemError_Empty);
                }
                return errors;
            }
            private EcfComment PrepareResultItem()
            {
                if (PresetComment == null)
                {
                    PresetComment = new EcfComment(Lines.ToList());
                }
                else
                {
                    PresetComment.ClearComments();
                    PresetComment.AddComment(Lines.ToList());
                }
                return PresetComment;
            }
        }
        private class ParameterItemPanel : TableLayoutPanel
        {
            private EgsEcfFile File { get; set; } = null;
            private EcfParameter PresetParameter { get; set; } = null;
            private EcfBlock ParentBlock { get; set; } = null;
            private ItemDefinition ParameterDefinition { get; set; } = null;
            private string PresetParameterCheckedKey { get; set; } = null;

            private Label ParameterItemKeyLabel { get; } = new Label();
            private CheckBox ParameterItemIsOptionalCheckBox { get; } = new CheckBox();
            private ComboBox ParameterItemKeyComboBox { get; } = new ComboBox();

            private Label ParameterItemParentLabel { get; } = new Label();
            private TextBox ParameterItemParentTextBox { get; } = new TextBox();
            private Label ParameterItemInfoLabel { get; } = new Label();
            private TextBox ParameterItemInfoTextBox { get; } = new TextBox();
            private Label ParameterItemCommentLabel { get; } = new Label();
            private TextBox ParameterItemCommentTextBox { get; } = new TextBox();

            private TableLayoutPanel ParameterItemKeyPanel { get; } = new TableLayoutPanel();
            private TableLayoutPanel ParameterItemInfoPanel { get; } = new TableLayoutPanel();

            private ParameterPanel ParameterItemValuesPanel { get; } = new ParameterPanel(ParameterPanel.ParameterModes.Value);
            private AttributesPanel ParameterItemAttributesPanel { get; } = new AttributesPanel();

            public ParameterItemPanel()
            {
                InitForms();
                InitEvents();
            }

            // events
            private void ParameterItemKeyComboBox_SelectionChangeCommitted(object sender, EventArgs evt)
            {
                UpdateDefinition(Convert.ToString(ParameterItemKeyComboBox.SelectedItem));
                ParameterItemValuesPanel.UpdateParameterValues(ParameterDefinition, PresetParameter);
            }

            // private
            private void InitForms()
            {
                ParameterItemKeyLabel.AutoSize = true;
                ParameterItemKeyLabel.Dock = DockStyle.Fill;
                ParameterItemKeyLabel.Text = TitleRecources.Generic_Name;
                ParameterItemKeyLabel.TextAlign = ContentAlignment.MiddleLeft;

                ParameterItemIsOptionalCheckBox.AutoSize = true;
                ParameterItemIsOptionalCheckBox.Dock = DockStyle.Fill;
                ParameterItemIsOptionalCheckBox.Enabled = false;
                ParameterItemIsOptionalCheckBox.Text = TitleRecources.Generic_IsOptional;
                ParameterItemIsOptionalCheckBox.UseVisualStyleBackColor = true;

                ParameterItemKeyComboBox.Dock = DockStyle.Fill;
                ParameterItemKeyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                ParameterItemKeyComboBox.FormattingEnabled = true;
                ParameterItemKeyComboBox.Sorted = true;

                ParameterItemParentLabel.AutoSize = true;
                ParameterItemParentLabel.Dock = DockStyle.Fill;
                ParameterItemParentLabel.Text = TitleRecources.Generic_ParentElement;
                ParameterItemParentLabel.TextAlign = ContentAlignment.MiddleLeft;
                
                ParameterItemParentTextBox.Dock = DockStyle.Fill;
                ParameterItemParentTextBox.ReadOnly = true;

                ParameterItemInfoLabel.AutoSize = true;
                ParameterItemInfoLabel.Dock = DockStyle.Fill;
                ParameterItemInfoLabel.Text = TitleRecources.Generic_Info;
                ParameterItemInfoLabel.TextAlign = ContentAlignment.MiddleLeft;
                
                ParameterItemInfoTextBox.Dock = DockStyle.Fill;
                ParameterItemInfoTextBox.ReadOnly = true;

                ParameterItemCommentLabel.AutoSize = true;
                ParameterItemCommentLabel.Dock = DockStyle.Fill;
                ParameterItemCommentLabel.Text = TitleRecources.Generic_Comment;
                ParameterItemCommentLabel.TextAlign = ContentAlignment.MiddleLeft;
                
                ParameterItemCommentTextBox.Dock = DockStyle.Fill;

                ParameterItemKeyPanel.AutoSize = true;
                ParameterItemKeyPanel.ColumnCount = 2;
                ParameterItemKeyPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                ParameterItemKeyPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                ParameterItemKeyPanel.Controls.Add(ParameterItemKeyLabel, 0, 0);
                ParameterItemKeyPanel.Controls.Add(ParameterItemIsOptionalCheckBox, 1, 0);
                ParameterItemKeyPanel.Controls.Add(ParameterItemKeyComboBox, 0, 1);
                ParameterItemKeyPanel.SetColumnSpan(ParameterItemKeyComboBox, 2);
                ParameterItemKeyPanel.Dock = DockStyle.Fill;
                ParameterItemKeyPanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
                ParameterItemKeyPanel.RowCount = 3;
                ParameterItemKeyPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
                ParameterItemKeyPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
                ParameterItemKeyPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));

                ParameterItemInfoPanel.AutoSize = true;
                ParameterItemInfoPanel.ColumnCount = 2;
                ParameterItemInfoPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
                ParameterItemInfoPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
                ParameterItemInfoPanel.Controls.Add(ParameterItemParentLabel, 0, 0);
                ParameterItemInfoPanel.Controls.Add(ParameterItemParentTextBox, 1, 0);
                ParameterItemInfoPanel.Controls.Add(ParameterItemInfoLabel, 0, 1);
                ParameterItemInfoPanel.Controls.Add(ParameterItemInfoTextBox, 1, 1);
                ParameterItemInfoPanel.Controls.Add(ParameterItemCommentLabel, 0, 2);
                ParameterItemInfoPanel.Controls.Add(ParameterItemCommentTextBox, 1, 2);
                ParameterItemInfoPanel.Dock = DockStyle.Fill;
                ParameterItemInfoPanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
                ParameterItemInfoPanel.RowCount = 3;
                ParameterItemInfoPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
                ParameterItemInfoPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
                ParameterItemInfoPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));

                AutoSize = true;
                ColumnCount = 2;
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
                Controls.Add(ParameterItemKeyPanel, 0, 0);
                Controls.Add(ParameterItemInfoPanel, 1, 0);
                Dock = DockStyle.Fill;
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
                RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
                RowStyles.Add(new RowStyle(SizeType.Percent, 35F));
                RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

                Controls.Add(ParameterItemAttributesPanel, 0, 1);
                SetColumnSpan(ParameterItemAttributesPanel, 2);

                Controls.Add(ParameterItemValuesPanel, 0, 2);
                SetColumnSpan(ParameterItemValuesPanel, 2);
            }
            private void InitEvents()
            {
                ParameterItemKeyComboBox.SelectionChangeCommitted += ParameterItemKeyComboBox_SelectionChangeCommitted;
            }
            private void ActivateView()
            {
                ActivateKeyComboBox();

                ParameterItemParentTextBox.Text = ParentBlock?.BuildRootId() ?? string.Empty;
                ParameterItemAttributesPanel.GenerateAttributes(File.Definition, File.Definition.ParameterAttributes);

                UpdateView();
            }
            private void ActivateKeyComboBox()
            {
                ParameterItemKeyComboBox.BeginUpdate();
                ParameterItemKeyComboBox.Items.Clear();
                IEnumerable<string> definedParameters = File.Definition.BlockParameters.Select(param => param.Name);
                IEnumerable<string> addableParameters = definedParameters.Except(ParentBlock.ChildItems.Where(child => child is EcfParameter).Cast<EcfParameter>().Select(param => param.Key));
                ParameterItemKeyComboBox.Items.AddRange(addableParameters.ToArray());
                if (PresetParameter != null)
                {
                    if (!ParameterItemKeyComboBox.Items.Contains(PresetParameterCheckedKey))
                    {
                        ParameterItemKeyComboBox.Items.Add(PresetParameterCheckedKey);
                    }
                    ParameterItemKeyComboBox.SelectedItem = PresetParameterCheckedKey;
                }
                else
                {
                    ParameterItemKeyComboBox.SelectedIndex = 0;
                }
                ParameterItemKeyComboBox.Enabled = ParameterItemKeyComboBox.Items.Count > 1;
                ParameterItemKeyComboBox.EndUpdate();
                UpdateDefinition(Convert.ToString(ParameterItemKeyComboBox.SelectedItem));
            }
            private bool UpdateDefinition(string paramKey)
            {
                ParameterDefinition = File.Definition.BlockParameters.FirstOrDefault(param => param.Name.Equals(paramKey));
                if (ParameterDefinition == null) { return false; }
                ParameterItemIsOptionalCheckBox.Checked = ParameterDefinition.IsOptional;
                ParameterItemInfoTextBox.Text = ParameterDefinition.Info;
                return true;
            }
            private void UpdateView()
            {
                // comments
                ParameterItemCommentTextBox.Text = PresetParameter != null ? string.Join(" / ", PresetParameter.Comments) : string.Empty;

                // values
                ParameterItemValuesPanel.UpdateParameterValues(ParameterDefinition, PresetParameter);

                // attributes
                ParameterItemAttributesPanel.UpdateAttributes(PresetParameter, null);
            }
            private List<string> ValidateInputs()
            {
                List<string> errors = new List<string>();
                errors.AddRange(ParameterItemValuesPanel.ValidateParameterValues(File.Definition));
                errors.AddRange(ParameterItemAttributesPanel.ValidateAttributeValues());
                return errors;
            }
            private EcfParameter PrepareResultItem()
            {
                List<EcfValueGroup> valueGroups = ParameterItemValuesPanel.PrepareResultValues();
                List<EcfAttribute> attributes = ParameterItemAttributesPanel.PrepareResultAttributes();
                if (PresetParameter == null)
                {
                    PresetParameter = new EcfParameter(Convert.ToString(ParameterItemKeyComboBox.SelectedItem), valueGroups, attributes);
                }
                else
                {
                    PresetParameter.UpdateKey(Convert.ToString(ParameterItemKeyComboBox.SelectedItem));
                    PresetParameter.ClearValues();
                    PresetParameter.AddValue(valueGroups);
                    PresetParameter.ClearAttributes();
                    PresetParameter.AddAttribute(attributes);
                    PresetParameter.ClearComments();
                    PresetParameter.RemoveErrors(EcfErrors.ParameterUnknown, EcfErrors.AttributeMissing, EcfErrors.AttributeDoubled,
                        EcfErrors.ValueGroupEmpty, EcfErrors.ValueNull, EcfErrors.ValueEmpty, EcfErrors.ValueContainsProhibitedPhrases);
                }
                if (!string.Empty.Equals(ParameterItemCommentTextBox.Text))
                {
                    PresetParameter.AddComment(ParameterItemCommentTextBox.Text);
                }
                return PresetParameter;
            }
        }
        private class BlockItemPanel : TableLayoutPanel
        {
            BlockItemPanelModes PanelMode { get; set; } = BlockItemPanelModes.None;
            private EgsEcfFile File { get; set; } = null;
            private EcfBlock PresetBlock { get; set; } = null;
            private EcfBlock ParentBlock { get; set; } = null;
            private string PresetBlockCheckedPreMark { get; set; } = null;
            private string PresetBlockCheckedDataType { get; set; } = null;
            private string PresetBlockCheckedPostMark { get; set; } = null;
            private List<ItemDefinition> BlockAttributeDefinitions { get; set; } = null;
            private List<BlockValueDefinition> BlockTypeDefinitions { get; set; } = null;
            private List<EcfBlock> IdentifyingBlockList { get; set; } = null;
            private List<EcfBlock> ReferencedBlockList { get; set; } = null;
            private List<EcfBlock> ReferencingBlockList { get; set; } = null;

            private TableLayoutPanel BlockItemTypePanel { get; } = new TableLayoutPanel();
            private TableLayoutPanel BlockItemAddDataPanel { get; } = new TableLayoutPanel();

            private Label BlockItemPreMarkLabel { get; } = new Label();
            private ComboBox BlockItemPreMarkComboBox { get; } = new ComboBox();
            private Label BlockItemDataTypeLabel { get; } = new Label();
            private ComboBox BlockItemDataTypeComboBox { get; } = new ComboBox();
            private Label BlockItemPostMarkLabel { get; } = new Label();
            private ComboBox BlockItemPostMarkComboBox { get; } = new ComboBox();

            private Label BlockItemParentLabel { get; } = new Label();
            private TextBox BlockItemParentTextBox { get; } = new TextBox();
            private Label BlockItemInheritorLabel { get; } = new Label();
            private TextBox BlockItemInheritorTextBox { get; } = new TextBox();
            private Label BlockItemCommentLabel { get; } = new Label();
            private TextBox BlockItemCommentTextBox { get; } = new TextBox();

            private AttributesPanel BlockItemAttributesPanel { get; } = new AttributesPanel();
            private ParameterPanel BlockItemParametersPanel { get; } = new ParameterPanel(ParameterPanel.ParameterModes.Block);

            private ParameterKeyComparer ParamKeyComparer { get; } = new ParameterKeyComparer();

            private enum BlockItemPanelModes
            {
                None,
                RootBlock,
                ChildBlock,
            }

            public BlockItemPanel()
            {
                InitForms();
                InitEvents();
            }

            // events
            private void BlockItemAttributesPanel_InheritorChanged(object sender, EventArgs evt)
            {
                UpdateInheritance(sender as EcfBlock);
            }

            // private
            private void InitForms()
            {
                BlockItemPreMarkLabel.AutoSize = true;
                BlockItemPreMarkLabel.Dock = DockStyle.Fill;
                BlockItemPreMarkLabel.Text = TitleRecources.Generic_PreMark;
                BlockItemPreMarkLabel.TextAlign = ContentAlignment.MiddleLeft;

                BlockItemPreMarkComboBox.Dock = DockStyle.Fill;
                BlockItemPreMarkComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                BlockItemPreMarkComboBox.FormattingEnabled = true;

                BlockItemDataTypeLabel.AutoSize = true;
                BlockItemDataTypeLabel.Dock = DockStyle.Fill;
                BlockItemDataTypeLabel.Text = TitleRecources.Generic_DataType;
                BlockItemDataTypeLabel.TextAlign = ContentAlignment.MiddleLeft;

                BlockItemDataTypeComboBox.Dock = DockStyle.Fill;
                BlockItemDataTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                BlockItemDataTypeComboBox.FormattingEnabled = true;

                BlockItemPostMarkLabel.AutoSize = true;
                BlockItemPostMarkLabel.Dock = DockStyle.Fill;
                BlockItemPostMarkLabel.Text = TitleRecources.Generic_PostMark;
                BlockItemPostMarkLabel.TextAlign = ContentAlignment.MiddleLeft;
                
                BlockItemPostMarkComboBox.Dock = DockStyle.Fill;
                BlockItemPostMarkComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                BlockItemPostMarkComboBox.FormattingEnabled = true;

                BlockItemTypePanel.AutoSize = true;
                BlockItemTypePanel.ColumnCount = 2;
                BlockItemTypePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
                BlockItemTypePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
                BlockItemTypePanel.Controls.Add(BlockItemPreMarkLabel, 0, 0);
                BlockItemTypePanel.Controls.Add(BlockItemPreMarkComboBox, 1, 0);
                BlockItemTypePanel.Controls.Add(BlockItemDataTypeLabel, 0, 1);
                BlockItemTypePanel.Controls.Add(BlockItemDataTypeComboBox, 1, 1);
                BlockItemTypePanel.Controls.Add(BlockItemPostMarkLabel, 0, 2);
                BlockItemTypePanel.Controls.Add(BlockItemPostMarkComboBox, 1, 2);
                BlockItemTypePanel.Dock = DockStyle.Fill;
                BlockItemTypePanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
                BlockItemTypePanel.RowCount = 3;
                BlockItemTypePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
                BlockItemTypePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
                BlockItemTypePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
                
                BlockItemParentLabel.AutoSize = true;
                BlockItemParentLabel.Dock = DockStyle.Fill;
                BlockItemParentLabel.Text = TitleRecources.Generic_ParentElement;
                BlockItemParentLabel.TextAlign = ContentAlignment.MiddleLeft;
                
                BlockItemParentTextBox.Dock = DockStyle.Fill;
                BlockItemParentTextBox.ReadOnly = true;
                 
                BlockItemInheritorLabel.AutoSize = true;
                BlockItemInheritorLabel.Dock = DockStyle.Fill;
                BlockItemInheritorLabel.Text = TitleRecources.Generic_Inherited;
                BlockItemInheritorLabel.TextAlign = ContentAlignment.MiddleLeft;
                 
                BlockItemInheritorTextBox.Dock = DockStyle.Fill;
                BlockItemInheritorTextBox.ReadOnly = true;
                
                BlockItemCommentLabel.AutoSize = true;
                BlockItemCommentLabel.Dock = DockStyle.Fill;
                BlockItemCommentLabel.Text = TitleRecources.Generic_Comment;
                BlockItemCommentLabel.TextAlign = ContentAlignment.MiddleLeft;
                
                BlockItemCommentTextBox.Dock = DockStyle.Fill;

                BlockItemAddDataPanel.AutoSize = true;
                BlockItemAddDataPanel.ColumnCount = 2;
                BlockItemAddDataPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
                BlockItemAddDataPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
                BlockItemAddDataPanel.Controls.Add(BlockItemParentLabel, 0, 0);
                BlockItemAddDataPanel.Controls.Add(BlockItemParentTextBox, 1, 0);
                BlockItemAddDataPanel.Controls.Add(BlockItemInheritorLabel, 0, 1);
                BlockItemAddDataPanel.Controls.Add(BlockItemInheritorTextBox, 1, 1);
                BlockItemAddDataPanel.Controls.Add(BlockItemCommentLabel, 0, 2);
                BlockItemAddDataPanel.Controls.Add(BlockItemCommentTextBox, 1, 2);
                BlockItemAddDataPanel.Dock = DockStyle.Fill;
                BlockItemAddDataPanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
                BlockItemAddDataPanel.RowCount = 3;
                BlockItemAddDataPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
                BlockItemAddDataPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
                BlockItemAddDataPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));

                AutoSize = true;
                ColumnCount = 2;
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
                Controls.Add(BlockItemTypePanel, 0, 0);
                Controls.Add(BlockItemAddDataPanel, 1, 0);
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
                Dock = DockStyle.Fill;
                RowCount = 3;
                RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
                RowStyles.Add(new RowStyle(SizeType.Percent, 35F));
                RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

                Controls.Add(BlockItemAttributesPanel, 0, 1);
                SetColumnSpan(BlockItemAttributesPanel, 2);

                Controls.Add(BlockItemParametersPanel, 0, 2);
                SetColumnSpan(BlockItemParametersPanel, 2);
            }
            private void InitEvents()
            {
                BlockItemAttributesPanel.InheritorChanged += BlockItemAttributesPanel_InheritorChanged;
            }
            private void ActivateView()
            {
                UpdateDefinition();
                UpdateBlockCompareLists();

                PrepareTypeDataComboBox(BlockItemPreMarkComboBox, File.Definition.BlockTypePreMarks.ToList());
                PrepareTypeDataComboBox(BlockItemDataTypeComboBox, BlockTypeDefinitions);
                PrepareTypeDataComboBox(BlockItemPostMarkComboBox, File.Definition.BlockTypePostMarks.ToList());

                BlockItemParametersPanel.GenerateParameterMatrix(File.Definition, File.Definition.BlockParameters);
                UpdateView();
            }
            private void PrepareTypeDataComboBox(ComboBox box, List<BlockValueDefinition> definitions)
            {
                box.BeginUpdate();
                box.Items.Clear();
                box.Sorted = true;
                box.Items.AddRange(definitions.Where(mark => !mark.IsOptional).Select(mark => new ComboBoxItem(mark.Value)).ToArray());
                if (box.Items.Count == 0)
                {
                    box.Items.AddRange(definitions.Where(mark => mark.IsOptional).Select(mark => new ComboBoxItem(mark.Value)).ToArray());
                    box.Sorted = false;
                    box.Items.Insert(0, new ComboBoxItem(null));
                }
                box.Sorted = false;
                box.Enabled = box.Items.Count > 1;
                box.SelectedItem = box.Items[0];
                box.EndUpdate();
            }
            private void UpdateBlockCompareLists()
            {
                List<EcfBlock> blocks = File?.GetDeepItemList<EcfBlock>();
                IdentifyingBlockList = blocks?.Where(block => block.HasAttribute(File.Definition.BlockIdAttribute, out _)).ToList();
                ReferencingBlockList = blocks?.Where(block => block.HasAttribute(File.Definition.BlockReferenceSourceAttribute, out _)).ToList();
                ReferencedBlockList = blocks?.Where(block => block.HasAttribute(File.Definition.BlockReferenceTargetAttribute, out _)).ToList();
                BlockItemAttributesPanel.ReferencedBlockList = ReferencedBlockList;
            }
            private void UpdateView()
            {
                // marks und type
                if (PresetBlock != null)
                {
                    BlockItemPreMarkComboBox.SelectedItem = BlockItemPreMarkComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => string.Equals(item.Value, PresetBlockCheckedPreMark));
                    BlockItemDataTypeComboBox.SelectedItem = BlockItemDataTypeComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => string.Equals(item.Value, PresetBlockCheckedDataType));
                    BlockItemPostMarkComboBox.SelectedItem = BlockItemPostMarkComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => string.Equals(item.Value, PresetBlockCheckedPostMark));
                }

                // parent
                BlockItemParentTextBox.Text = ParentBlock?.BuildRootId() ?? string.Empty;

                // inheritance
                EcfBlock inheritorBlock = PresetBlock?.Inheritor;

                // comments
                BlockItemCommentTextBox.Text = PresetBlock != null ? string.Join(" / ", PresetBlock.Comments) : string.Empty;

                // attributes
                BlockItemAttributesPanel.GenerateAttributes(File.Definition, BlockAttributeDefinitions.AsReadOnly());
                BlockItemAttributesPanel.UpdateAttributes(PresetBlock, inheritorBlock);

                // parameter
                BlockItemParametersPanel.UpdateParameterMatrix(PresetBlock);
                UpdateInheritance(inheritorBlock);
            }
            private void UpdateInheritance(EcfBlock inheritor)
            {
                BlockItemParametersPanel.UpdateParameterInheritance(inheritor);
                BlockItemInheritorTextBox.Text = inheritor?.BuildRootId() ?? string.Empty;
            }
            private void UpdateDefinition()
            {
                switch (PanelMode)
                {
                    case BlockItemPanelModes.ChildBlock:
                        BlockTypeDefinitions = File.Definition.ChildBlockTypes.ToList();
                        BlockAttributeDefinitions = File.Definition.ChildBlockAttributes.ToList();
                        break;
                    case BlockItemPanelModes.RootBlock:
                        BlockTypeDefinitions = File.Definition.RootBlockTypes.ToList();
                        BlockAttributeDefinitions = File.Definition.RootBlockAttributes.ToList();
                        break;
                    default: throw new ArgumentException(string.Format("No creator defined for item type {0}....that shouldn't happen :)", PanelMode.ToString()));
                }
            }
            private List<string> ValidateInputs()
            {
                List<string> errors = new List<string>();
                errors.AddRange(ValidateTypeData());
                errors.AddRange(BlockItemAttributesPanel.ValidateIdName(IdentifyingBlockList, ReferencingBlockList, ReferencedBlockList, PresetBlock));
                errors.AddRange(BlockItemAttributesPanel.ValidateRefTarget(ReferencingBlockList, PresetBlock));
                errors.AddRange(BlockItemAttributesPanel.ValidateRefSource(ReferencedBlockList));
                errors.AddRange(BlockItemAttributesPanel.ValidateAttributeValues());
                errors.AddRange(BlockItemParametersPanel.ValidateParameterMatrix());
                return errors;
            }
            private List<string> ValidateTypeData()
            {
                List<string> errors = new List<string>();
                if (File.Definition.BlockTypePreMarks.Count > 0 && BlockItemPreMarkComboBox.SelectedIndex < 0)
                {
                    errors.Add(string.Format("{0} {1}", TitleRecources.Generic_PreMark, TextRecources.EcfItemEditingDialog_NotSelected));
                }
                if (BlockTypeDefinitions.Count > 0 && BlockItemDataTypeComboBox.SelectedIndex < 0)
                {
                    errors.Add(string.Format("{0} {1}", TitleRecources.Generic_DataType, TextRecources.EcfItemEditingDialog_NotSelected));
                }
                if (File.Definition.BlockTypePostMarks.Count > 0 && BlockItemPostMarkComboBox.SelectedIndex < 0)
                {
                    errors.Add(string.Format("{0} {1}", TitleRecources.Generic_PostMark, TextRecources.EcfItemEditingDialog_NotSelected));
                }
                return errors;
            }
            private EcfBlock PrepareResultItem()
            {
                List<EcfParameter> activeParameters = BlockItemParametersPanel.PrepareResultParameters();
                List<EcfAttribute> attributes = BlockItemAttributesPanel.PrepareResultAttributes();
                if (PresetBlock == null)
                {
                    PresetBlock = new EcfBlock(
                        (BlockItemPreMarkComboBox.SelectedItem as ComboBoxItem)?.Value,
                        (BlockItemDataTypeComboBox.SelectedItem as ComboBoxItem)?.Value,
                        (BlockItemPostMarkComboBox.SelectedItem as ComboBoxItem)?.Value,
                        attributes, activeParameters);
                    PresetBlock.UpdateStructureData(File, null, -1);
                    foreach (EcfParameter parameter in activeParameters)
                    {
                        parameter.Revalidate();
                    }
                }
                else
                {
                    PresetBlock.UpdateTypeData(
                        (BlockItemPreMarkComboBox.SelectedItem as ComboBoxItem)?.Value,
                        (BlockItemDataTypeComboBox.SelectedItem as ComboBoxItem)?.Value,
                        (BlockItemPostMarkComboBox.SelectedItem as ComboBoxItem)?.Value);
                    PrepareResultParameterUpdate(PresetBlock, activeParameters);
                    PresetBlock.ClearAttributes();
                    PresetBlock.AddAttribute(attributes);
                    PresetBlock.ClearComments();
                    PresetBlock.RemoveErrors(EcfErrors.BlockIdNotUnique, EcfErrors.BlockInheritorMissing,
                        EcfErrors.BlockPreMarkMissing, EcfErrors.BlockPreMarkUnknown,
                        EcfErrors.BlockDataTypeMissing, EcfErrors.BlockDataTypeUnknown,
                        EcfErrors.BlockPostMarkMissing, EcfErrors.BlockPostMarkUnknown,
                        EcfErrors.ParameterMissing, EcfErrors.ParameterDoubled,
                        EcfErrors.AttributeMissing, EcfErrors.AttributeDoubled);
                }
                PresetBlock.Inheritor = BlockItemAttributesPanel.GetInheritor();
                if (!string.Empty.Equals(BlockItemCommentTextBox.Text))
                {
                    PresetBlock.AddComment(BlockItemCommentTextBox.Text);
                }
                return PresetBlock;
            }
            private void PrepareResultParameterUpdate(EcfBlock block, List<EcfParameter> activeParameters)
            {
                List<EcfParameter> presentParameters = block.ChildItems.Where(child => child is EcfParameter).Cast<EcfParameter>().ToList();

                List<EcfParameter> doubledParameters = presentParameters.Except(presentParameters.Distinct(ParamKeyComparer)).ToList();
                List<EcfParameter> removedParameters = presentParameters.Except(activeParameters, ParamKeyComparer).ToList();
                removedParameters.RemoveAll(parameter => parameter.Definition == null);
                List<EcfParameter> createdParameters = activeParameters.Except(presentParameters, ParamKeyComparer).ToList();

                block.RemoveChild(doubledParameters);
                block.RemoveChild(removedParameters);
                block.AddChild(createdParameters);

                foreach (EcfParameter parameter in createdParameters)
                {
                    parameter.Revalidate();
                }
            }

            // subclasses
            private class ParameterKeyComparer : IEqualityComparer<EcfParameter>
            {
                public bool Equals(EcfParameter x, EcfParameter y)
                {
                    return x.Key.Equals(y.Key);
                }
                public int GetHashCode(EcfParameter obj)
                {
                    return obj.Key.GetHashCode();
                }
            }
        }
        private class ParameterMatrixPanel : ParameterPanel
        {
            private EgsEcfFile File { get; set; } = null;

            public ParameterMatrixPanel() : base(ParameterModes.Matrix)
            {

            }

            //private
            private void ActivateView(List<EcfParameter> parameters)
            {
                GenerateParameterMatrix(File.Definition, parameters);
                UpdateView();
            }
            private void UpdateView()
            {
                UpdateParameterMatrix();
            }
            private List<string> ValidateInputs()
            {
                return ValidateParameterMatrix();
            }
            private List<EcfParameter> PrepareResultItem()
            {
                List<EcfParameter> editedParameters = PrepareResultParameters();

                foreach (EcfParameter parameter in editedParameters)
                {
                    parameter.Revalidate();
                }

                return editedParameters;
            }
        }
    }
}
