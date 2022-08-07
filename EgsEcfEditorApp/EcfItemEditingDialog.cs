using EcfFileViewTools;
using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static EgsEcfParser.EcfFormatChecking;
using static Helpers.EnumLocalisation;

namespace EcfFileViews
{
    public partial class EcfItemEditingDialog : Form
    {
        public EcfStructureItem ResultItem { get; private set; } = null;

        [Flags]
        public enum CreateableItems
        {
            None = 0,
            Comment = 1 << 0,
            Parameter = 1 << 1,
            RootBlock = 1 << 2,
            ChildBlock = 1 << 3,
        }

        private EcfComment PresetComment { get; set; } = null;
        private EcfParameter PresetParameter { get; set; } = null;
        private EcfBlock PresetBlock { get; set; } = null;
        private EcfBlock ParentBlock { get; set; }
        private EgsEcfFile File { get; set; } = null;
        private CreateableItems CreateableItemTypes { get; set; } = CreateableItems.None;
        private CreateableItems SelectedItemType { get; set; } = CreateableItems.None;
        private ItemDefinition ParameterDefinition { get; set; } = null;
        private string PresetParameterCheckedKey { get; set; } = null;
        private ParameterKeyComparer ParamKeyComparer { get; } = new ParameterKeyComparer();
        private List<BlockValueDefinition> BlockTypeDefinitions { get; set; } = null;
        private List<ItemDefinition> BlockAttributeDefinitions { get; set; } = null;
        private string PresetBlockCheckedPreMark { get; set; } = null;
        private string PresetBlockCheckedDataType { get; set; } = null;
        private string PresetBlockCheckedPostMark { get; set; } = null;
        private List<EcfBlock> IdentifyingBlockList { get; set; } = null;
        private List<EcfBlock> ReferencedBlockList { get; set; } = null;
        private List<EcfBlock> ReferencingBlockList { get; set; } = null;
        private EcfItemSelectorDialog ItemSelector { get; set; } = new EcfItemSelectorDialog();

        private AttributesPanel ParameterItemAttributesPanel { get; } = new AttributesPanel();
        private AttributesPanel BlockItemAttributesPanel { get; } = new AttributesPanel();

        public EcfItemEditingDialog(EgsEcfFile file)
        {
            InitializeComponent();
            InitForm(file);
        }

        // events
        // unspecific
        private void InitForm(EgsEcfFile file)
        {
            Icon = IconRecources.Icon_App;
            File = file;

            // Hack to hide tabs
            ViewPanel.SizeMode = TabSizeMode.Fixed;
            ViewPanel.ItemSize = new Size(0, 1);

            OkButton.Text = TitleRecources.Generic_Ok;
            AbortButton.Text = TitleRecources.Generic_Abort;
            BackButton.Text = TitleRecources.Generic_BackButton;
            ResetButton.Text = TitleRecources.Generic_Reset;

            SelectItem_InitView();
            CommentItem_InitView();
            ParameterItem_InitView();
            BlockItem_InitView();
        }
        private void AbortButton_Click(object sender, EventArgs evt)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }
        private void ResetButton_Click(object sender, EventArgs evt)
        {
            switch (SelectedItemType)
            {
                case CreateableItems.Comment: CommentItem_UpdateView(); break;
                case CreateableItems.Parameter: ParameterItem_UpdateView(); break;
                case CreateableItems.ChildBlock: BlockItem_UpdateView(); break;
                case CreateableItems.RootBlock: BlockItem_UpdateView(); break;
                default: break;
            }
        }
        private void BackButton_Click(object sender, EventArgs evt)
        {
            SelectItem_ActivateView();
        }
        private void OkButton_Click(object sender, EventArgs evt)
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
        // selector
        private void SelectCommentItemRadioButton_CheckedChanged(object sender, EventArgs evt)
        {
            if (SelectCommentItemRadioButton.Checked)
            {
                CommentItem_ActivateView();
            }
        }
        private void SelectParameterItemRadioButton_CheckedChanged(object sender, EventArgs evt)
        {
            if (SelectParameterItemRadioButton.Checked)
            {
                ParameterItem_ActivateView();
            }
        }
        private void SelectChildBlockItemRadioButton_CheckedChanged(object sender, EventArgs evt)
        {
            if (SelectChildBlockItemRadioButton.Checked)
            {
                BlockItem_ActivateView(CreateableItems.ChildBlock);
            }
        }
        private void SelectRootBlockItemRadioButton_CheckedChanged(object sender, EventArgs evt)
        {
            if (SelectRootBlockItemRadioButton.Checked)
            {
                BlockItem_ActivateView(CreateableItems.RootBlock);
            }
        }
        // parameter
        private void ParameterItemKeyComboBox_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            ParameterItem_UpdateDefinition(Convert.ToString(ParameterItemKeyComboBox.SelectedItem));
            ParameterItem_UpdateValuesGrid();
        }
        private void ParameterItemAddValueButton_Click(object sender, EventArgs evt)
        {
            if (ParameterItem_TryGetValuesGridCell(ParameterItemValuesGrid.GetCellCount(DataGridViewElementStates.Visible) < 1, out DataGridViewCell cell))
            {
                ParameterItemValuesGrid.Columns.Insert(cell.ColumnIndex + 1, new ParameterItem_ValueColumn());
            }
            else
            {
                ParameterItemValuesGrid.Columns.Add(new ParameterItem_ValueColumn());
            }
            ParameterItem_UpdateValuesGridButtons();
            ParameterItem_UpdateValuesGridColumnNumbering();
            ParameterItem_UpdateValuesGridRowNumbering();
        }
        private void ParameterItemRemoveValueButton_Click(object sender, EventArgs evt)
        {
            if (ParameterItem_TryGetValuesGridCell(false, out DataGridViewCell cell))
            {
                ParameterItemValuesGrid.Columns.RemoveAt(cell.ColumnIndex);
                ParameterItem_UpdateValuesGridButtons();
                ParameterItem_UpdateValuesGridColumnNumbering();
                ParameterItem_UpdateValuesGridRowNumbering();
            }
        }
        private void ParameterItemAddGroupButton_Click(object sender, EventArgs evt)
        {
            if (ParameterItem_TryGetValuesGridCell(false, out DataGridViewCell cell))
            {
                ParameterItemValuesGrid.Rows.Insert(cell.RowIndex + 1, new DataGridViewRow());
            }
            else
            {
                ParameterItemValuesGrid.Rows.Add(new DataGridViewRow());
            }
            ParameterItem_UpdateValuesGridButtons();
            ParameterItem_UpdateValuesGridColumnNumbering();
            ParameterItem_UpdateValuesGridRowNumbering();
        }
        private void ParameterItemRemoveGroupButton_Click(object sender, EventArgs evt)
        {
            if (ParameterItem_TryGetValuesGridCell(false, out DataGridViewCell cell))
            {
                ParameterItemValuesGrid.Rows.RemoveAt(cell.RowIndex);
                ParameterItem_UpdateValuesGridButtons();
                ParameterItem_UpdateValuesGridColumnNumbering();
                ParameterItem_UpdateValuesGridRowNumbering();
            }
        }
        // block
        private void BlockItemAttributesPanel_InheritorChanged(object sender, EventArgs evt)
        {
            BlockItem_UpdateParametersInheritance(sender as EcfBlock);
        }

        // publics
        public DialogResult ShowDialog(IWin32Window parent, EgsEcfFile file, EcfComment presetComment)
        {
            ResultItem = null;
            ParentBlock = null;
            File = file;
            PresetComment = presetComment;
            CreateableItemTypes = CreateableItems.Comment;

            CommentItem_ActivateView();

            return ShowDialog(parent);
        }
        public DialogResult ShowDialog(IWin32Window parent, EgsEcfFile file, EcfParameter presetParameter)
        {
            ResultItem = null;
            ParentBlock = presetParameter?.Parent as EcfBlock;
            File = file;
            PresetParameter = presetParameter;
            CreateableItemTypes = CreateableItems.Parameter;

            try
            {
                ParameterItem_PreActivationChecks_Editing();
            }
            catch (Exception)
            {
                return DialogResult.Abort;
            }
            ParameterItem_ActivateView();

            return ShowDialog(parent);
        }
        public DialogResult ShowDialog(IWin32Window parent, EgsEcfFile file, EcfBlock presetBlock)
        {
            ResultItem = null;
            ParentBlock = null;
            File = file;
            Generic_BuildBlockCompareLists(file);
            PresetBlock = presetBlock;
            CreateableItemTypes = presetBlock.IsRoot() ? CreateableItems.RootBlock : CreateableItems.ChildBlock;

            try
            {
                BlockItem_PreActivationChecks_Editing();
            }
            catch (Exception)
            {
                return DialogResult.Abort;
            }
            BlockItem_ActivateView(CreateableItemTypes);

            return ShowDialog(parent);
        }
        public DialogResult ShowDialog(IWin32Window parent, EgsEcfFile file, CreateableItems createable, EcfBlock parentBlock)
        {
            ResultItem = null;
            ParentBlock = parentBlock;
            File = file;
            Generic_BuildBlockCompareLists(file);
            Generic_ClearPresets();
            CreateableItemTypes = createable;

            try
            {
                Generic_PreActivationChecks_AddingMulti();
            }
            catch (Exception)
            {
                return DialogResult.Abort;
            }
            
            return ShowDialog(parent);
        }
        
        // privates
        // generics
        private void Generic_ClearPresets()
        {
            PresetComment = null;
            PresetParameter = null;
            PresetBlock = null;
        }
        private void Generic_BuildBlockCompareLists(EgsEcfFile file)
        {
            List<EcfBlock> blocks = file?.GetDeepItemList<EcfBlock>();
            IdentifyingBlockList = blocks?.Where(block => block.HasAttribute(file.Definition.BlockIdentificationAttribute)).ToList();
            ReferencingBlockList = blocks?.Where(block => block.HasAttribute(file.Definition.BlockReferenceSourceAttribute)).ToList();
            ReferencedBlockList = blocks?.Where(block => block.HasAttribute(file.Definition.BlockReferenceTargetAttribute)).ToList();
            BlockItemAttributesPanel.ReferencedBlockList = ReferencedBlockList;
        }
        private void Generic_PreActivationChecks_AddingMulti()
        {
            ParameterItem_PreActivationChecks_Adding();
            BlockItem_PreActivationChecks_Adding();
            if (CreateableItemTypes == CreateableItems.None)
            {
                throw new InvalidOperationException("No item type to add left");
            }
            switch (CreateableItemTypes)
            {
                case CreateableItems.Comment: CommentItem_ActivateView(); break;
                case CreateableItems.Parameter: ParameterItem_ActivateView(); break;
                case CreateableItems.RootBlock: BlockItem_ActivateView(CreateableItems.RootBlock); break;
                case CreateableItems.ChildBlock: BlockItem_ActivateView(CreateableItems.ChildBlock); break;
                default: SelectItem_ActivateView(); break;
            }
        }
        private List<string> Generic_ValidateInputs()
        {
            switch (SelectedItemType)
            {
                case CreateableItems.Comment: return CommentItem_ValidateInputs();
                case CreateableItems.Parameter: return ParameterItem_ValidateInputs();
                case CreateableItems.RootBlock: return BlockItem_ValidateInputs();
                case CreateableItems.ChildBlock: return BlockItem_ValidateInputs();
                default: throw new ArgumentException(string.Format("No creator defined for item type {0}...that shouldn't happen", SelectedItemType.ToString()));
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
            switch (SelectedItemType)
            {
                case CreateableItems.Comment: return CommentItem_PrepareResultItem();
                case CreateableItems.Parameter: return ParameterItem_PrepareResultItem();
                case CreateableItems.RootBlock: return BlockItem_PrepareResultItem();
                case CreateableItems.ChildBlock: return BlockItem_PrepareResultItem();
                default: throw new ArgumentException(string.Format("No creator defined for item type {0}....that shouldn't happen", SelectedItemType.ToString()));
            }
        }

        // comment section
        private void CommentItem_InitView()
        {

        }
        private void CommentItem_ActivateView()
        {
            BackButton.Enabled = CreateableItemTypes != CreateableItems.Comment;
            ResetButton.Enabled = PresetComment != null;
            SelectedItemType = CreateableItems.Comment;

            CommentItem_ActivateViewHeader();

            CommentItem_UpdateView();

            // hack to prevent tab switch with tab key
            ViewPanel.TabPages.Clear();
            ViewPanel.TabPages.Add(CommentItemView);

            CommentItemTextBox.Focus();
        }
        private void CommentItem_ActivateViewHeader()
        {
            if (PresetComment != null)
            {
                Text = TitleRecources.EcfItemEditingDialog_Header_EditComment;
            }
            else
            {
                Text = TitleRecources.EcfItemEditingDialog_Header_AddComment;
            }
        }
        private void CommentItem_UpdateView()
        {
            CommentItemTextBox.Text = PresetComment == null ? string.Empty : string.Join(" / ", PresetComment.Comments);
        }
        private List<string> CommentItem_ValidateInputs()
        {
            List<string> errors = new List<string>();
            if (CommentItemTextBox.Text.Equals(string.Empty))
            {
                errors.Add(TextRecources.EcfItemEditingDialog_CommentItemError_Empty);
            }
            return errors;
        }
        private EcfComment CommentItem_PrepareResultItem()
        {
            if (PresetComment == null)
            {
                PresetComment = new EcfComment(CommentItemTextBox.Text);
            }
            else
            {
                PresetComment.ClearComments();
                PresetComment.AddComment(CommentItemTextBox.Text);
            }
            return PresetComment;
        }

        // parameter section
        private void ParameterItem_InitView()
        {
            ParameterItemAddValueButton.Text = string.Empty;
            ParameterItemRemoveValueButton.Text = string.Empty;
            ParameterItemAddGroupButton.Text = string.Empty;
            ParameterItemRemoveGroupButton.Text = string.Empty;

            ParameterItemAddValueButton.Image = IconRecources.Icon_AddValue;
            ParameterItemRemoveValueButton.Image = IconRecources.Icon_RemoveValue;
            ParameterItemAddGroupButton.Image = IconRecources.Icon_AddValueGroup;
            ParameterItemRemoveGroupButton.Image = IconRecources.Icon_RemoveValueGroup;

            new ToolTip().SetToolTip(ParameterItemAddValueButton, TextRecources.EcfItemEditingDialog_ToolTip_AddValue);
            new ToolTip().SetToolTip(ParameterItemRemoveValueButton, TextRecources.EcfItemEditingDialog_ToolTip_RemoveValue);
            new ToolTip().SetToolTip(ParameterItemAddGroupButton, TextRecources.EcfItemEditingDialog_ToolTip_AddValueGroup);
            new ToolTip().SetToolTip(ParameterItemRemoveGroupButton, TextRecources.EcfItemEditingDialog_ToolTip_RemoveValueGroup);

            ParameterItemKeyLabel.Text = TitleRecources.Generic_Name;
            ParameterItemIsOptionalCheckBox.Text = TitleRecources.Generic_IsOptional;
            ParameterItemParentLabel.Text = TitleRecources.Generic_ParentElement;
            ParameterItemInfoLabel.Text = TitleRecources.Generic_Info;
            ParameterItemCommentLabel.Text = TitleRecources.Generic_Comment;
            ParameterItemValuesLabel.Text = TitleRecources.Generic_Values;

            ParameterItemViewPanel.Controls.Add(ParameterItemAttributesPanel, 0, 2);
            ParameterItemViewPanel.SetColumnSpan(ParameterItemAttributesPanel, 2);
        }
        private void ParameterItem_PreActivationChecks_Editing()
        {
            if (ParentBlock == null) { throw new ArgumentException("No Parameter Editing without parent element"); }

            List<string> definedParameters = File.Definition.BlockParameters.Select(param => param.Name).ToList();
            if (definedParameters.Count < 1)
            {
                MessageBox.Show(this, TextRecources.EcfItemEditingDialog_NoDefinitionForParameters,
                    TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                throw new InvalidOperationException("No editing without Definition");
            }

            if (!definedParameters.Contains(PresetParameter.Key))
            {
                MessageBox.Show(this, TextRecources.EcfItemEditingDialog_ParameterNotDefined,
                    TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                List<string> addableParameter = definedParameters.Except(ParentBlock.ChildItems.Where(child => child is EcfParameter).Cast<EcfParameter>().Select(param => param.Key)).ToList();
                if (addableParameter.Count < 1)
                {
                    MessageBox.Show(this, TextRecources.EcfItemEditingDialog_NoAddableParameter,
                        TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    throw new ArgumentException("no addable parameter left");
                }

                if (ItemSelector.ShowDialog(this, addableParameter.ToArray()) == DialogResult.OK)
                {
                    PresetParameterCheckedKey = Convert.ToString(ItemSelector.SelectedItem);
                    return;
                }
                throw new ArgumentException("Parameter selecting aborted");
            }
            PresetParameterCheckedKey = PresetParameter.Key;
        }
        private void ParameterItem_PreActivationChecks_Adding()
        {
            if (CreateableItemTypes.HasFlag(CreateableItems.Parameter))
            {
                if (ParentBlock == null) { CreateableItemTypes &= ~CreateableItems.Parameter; return; }

                List<string> definedParameters = File.Definition.BlockParameters.Select(param => param.Name).ToList();
                if (definedParameters.Count < 1) { CreateableItemTypes &= ~CreateableItems.Parameter; return; }

                List<string> addableParameters = definedParameters.Except(ParentBlock.ChildItems.Where(child => 
                    child is EcfParameter).Cast<EcfParameter>().Select(param => param.Key)).ToList();
                if (addableParameters.Count < 1) { CreateableItemTypes &= ~CreateableItems.Parameter; return; }
            }
        }
        private void ParameterItem_ActivateView()
        {
            BackButton.Enabled = CreateableItemTypes != CreateableItems.Parameter;
            ResetButton.Enabled = PresetParameter != null;
            SelectedItemType = CreateableItems.Parameter;

            ParameterItem_ActivateViewHeader();
            ParameterItem_ActivateKeyComboBox();

            ParameterItemParentTextBox.Text = ParentBlock?.BuildIdentification();

            ParameterItemAttributesPanel.GenerateAttributes(File.Definition, File.Definition.ParameterAttributes);

            ParameterItem_UpdateView();

            // hack to prevent tab switch with tab key
            ViewPanel.TabPages.Clear();
            ViewPanel.TabPages.Add(ParameterItemView);

            OkButton.Focus();
        }
        private void ParameterItem_ActivateViewHeader()
        {
            if (PresetParameter != null)
            {
                Text = TitleRecources.EcfItemEditingDialog_Header_EditParameter;
            }
            else
            {
                Text = TitleRecources.EcfItemEditingDialog_Header_AddParameter;
            }
        }
        private void ParameterItem_UpdateView()
        {
            // comments
            ParameterItemCommentTextBox.Text = PresetParameter != null ? string.Join(" / ", PresetParameter.Comments) : string.Empty;

            // values
            ParameterItem_UpdateValuesGrid();

            // attributes
            ParameterItemAttributesPanel.UpdateAttributes(PresetParameter, null);
        }
        private void ParameterItem_ActivateKeyComboBox()
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
            ParameterItem_UpdateDefinition(Convert.ToString(ParameterItemKeyComboBox.SelectedItem));
        }
        private bool ParameterItem_UpdateDefinition(string paramKey)
        {
            ParameterDefinition = File.Definition.BlockParameters.FirstOrDefault(param => param.Name.Equals(paramKey));
            if (ParameterDefinition == null) { return false; }
            ParameterItemIsOptionalCheckBox.Checked = ParameterDefinition.IsOptional;
            ParameterItemInfoTextBox.Text = ParameterDefinition.Info;
            return true;
        }
        private void ParameterItem_UpdateValuesGrid()
        {
            ParameterItemValuesGrid.SuspendLayout();
            if (PresetParameter != null)
            {
                ParameterItemValuesGrid.Rows.Clear();
                ParameterItemValuesGrid.Columns.Clear();
                if (PresetParameter.HasValue())
                {
                    for (int count = 0; count < PresetParameter.ValueGroups.Max(group => group.Values.Count); count++)
                    {
                        ParameterItemValuesGrid.Columns.Add(new ParameterItem_ValueColumn());
                    }
                    foreach (EcfValueGroup group in PresetParameter.ValueGroups)
                    {
                        DataGridViewRow groupRow = new DataGridViewRow();
                        foreach (string value in group.Values)
                        {
                            groupRow.Cells.Add(new DataGridViewTextBoxCell() { Value = value });
                        }
                        ParameterItemValuesGrid.Rows.Add(groupRow);
                    }
                }
            }
            if (ParameterDefinition.HasValue)
            {
                if (ParameterItemValuesGrid.Rows.Count < 1)
                {
                    ParameterItemValuesGrid.Columns.Add(new ParameterItem_ValueColumn());
                    ParameterItemValuesGrid.Rows.Add(new DataGridViewRow());
                }
            }
            else
            {
                ParameterItemValuesGrid.Rows.Clear();
                ParameterItemValuesGrid.Columns.Clear();
            }
            ParameterItemValuesGrid.ResumeLayout();
            ParameterItem_UpdateValuesGridButtons();
            ParameterItem_UpdateValuesGridColumnNumbering();
            ParameterItem_UpdateValuesGridRowNumbering();
        }
        private void ParameterItem_UpdateValuesGridButtons()
        {
            if (!ParameterDefinition.HasValue)
            {
                ParameterItemAddValueButton.Enabled = false;
                ParameterItemRemoveValueButton.Enabled = false;
                ParameterItemAddGroupButton.Enabled = false;
                ParameterItemRemoveGroupButton.Enabled = false;
                return;
            }
            int valueCount = ParameterItemValuesGrid.Columns.Count;
            int groupCount = ParameterItemValuesGrid.Rows.Count;
            ParameterItemAddValueButton.Enabled = true;
            ParameterItemRemoveValueButton.Enabled = valueCount > 1;
            ParameterItemAddGroupButton.Enabled = valueCount > 0;
            ParameterItemRemoveGroupButton.Enabled = groupCount > 1;
        }
        private void ParameterItem_UpdateValuesGridColumnNumbering()
        {
            ParameterItemValuesGrid.SuspendLayout();
            foreach (DataGridViewColumn column in ParameterItemValuesGrid.Columns)
            {
                column.HeaderText = string.Format("{0} {1}", TitleRecources.Generic_Value, column.Index + 1);
            }
            ParameterItemValuesGrid.ResumeLayout();
        }
        private void ParameterItem_UpdateValuesGridRowNumbering()
        {
            ParameterItemValuesGrid.SuspendLayout();
            if (ParameterItemValuesGrid.Rows.Count == 1)
            {
                ParameterItemValuesGrid.Rows[0].HeaderCell.Value = string.Empty;
            }
            else
            {
                foreach (DataGridViewRow row in ParameterItemValuesGrid.Rows)
                {
                    row.HeaderCell.Value = string.Format("{0} {1}", TitleRecources.Generic_Group, row.Index + 1);
                }
            }
            ParameterItemValuesGrid.ResumeLayout();
        }
        private bool ParameterItem_TryGetValuesGridCell(bool noMessage, out DataGridViewCell cell)
        {
            cell = ParameterItemValuesGrid.SelectedCells.Cast<DataGridViewCell>().FirstOrDefault();
            if (cell == null && !noMessage)
            {
                MessageBox.Show(this, TextRecources.Generic_NoSuitableSelection, TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return cell != null;
        }
        private List<string> ParameterItem_ValidateInputs()
        {
            List<string> errors = new List<string>();
            errors.AddRange(ParameterItem_ValidateValuesGrid());
            errors.AddRange(ParameterItemAttributesPanel.ValidateAttributeValues());
            return errors;
        }
        private List<string> ParameterItem_ValidateValuesGrid()
        {
            List<string> errors = new List<string>();
            if (ParameterDefinition.HasValue)
            {
                List<EcfValueGroup> checkGroups =
                    ParameterItemValuesGrid.Rows.Cast<DataGridViewRow>().Select(row =>
                    new EcfValueGroup(row.Cells.Cast<DataGridViewCell>().Select(cell =>
                    Convert.ToString(cell.Value)).ToList())).ToList();

                foreach (EcfError error in CheckValuesValid(checkGroups, ParameterDefinition, File.Definition, EcfErrorGroups.Editing))
                {
                    errors.Add(string.Format("{0} {1} '{2}': {3}", TitleRecources.Generic_Value, TextRecources.Generic_HasError, GetLocalizedEnum(error.Type), error.Info));
                }
            }
            return errors;
        }
        private EcfParameter ParameterItem_PrepareResultItem()
        {
            List<EcfValueGroup> valueGroups = ParameterItem_PrepareResultValues();
            List<EcfAttribute> attributes = ParameterItemAttributesPanel.PrepareResultAttributes();
            if (PresetParameter == null)
            {
                PresetParameter = new EcfParameter(Convert.ToString(ParameterItemKeyComboBox.SelectedItem), valueGroups, attributes);
            }
            else
            {
                PresetParameter.UpdateKey(Convert.ToString(ParameterItemKeyComboBox.SelectedItem));
                PresetParameter.ClearValues();
                PresetParameter.AddValues(valueGroups);
                PresetParameter.ClearAttributes();
                PresetParameter.AddAttributes(attributes);
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
        private List<EcfValueGroup> ParameterItem_PrepareResultValues()
        {
            List<EcfValueGroup> valueGroups = new List<EcfValueGroup>();
            if (ParameterDefinition.HasValue)
            {
                foreach (DataGridViewRow row in ParameterItemValuesGrid.Rows)
                {
                    valueGroups.Add(new EcfValueGroup(row.Cells.Cast<DataGridViewTextBoxCell>().Select(cell => Convert.ToString(cell.Value)).ToList()));
                }
            }
            return valueGroups;
        }

        // block section
        [Obsolete("attributes panel in ursprünglicher reihe bis parameter panel überarbeitung")]
        private void BlockItem_InitView()
        {
            BlockItemPreMarkLabel.Text = TitleRecources.Generic_PreMark;
            BlockItemDataTypeLabel.Text = TitleRecources.Generic_DataType;
            BlockItemPostMarkLabel.Text = TitleRecources.Generic_PostMark;
            BlockItemInheritorLabel.Text = TitleRecources.Generic_Inherited;
            BlockItemCommentLabel.Text = TitleRecources.Generic_Comment;
            BlockItemParametersLabel.Text = TitleRecources.Generic_Parameters;

            BlockItem_ParametersGrid_ActivateColumn.HeaderText = TitleRecources.Generic_Active;
            BlockItem_ParametersGrid_InheritColumn.HeaderText = TitleRecources.Generic_Inherited;
            BlockItem_ParametersGrid_NameColumn.HeaderText = TitleRecources.Generic_Name;
            BlockItem_ParametersGrid_InfoColumn.HeaderText = TitleRecources.Generic_Info;
            BlockItem_ParametersGrid_CommentColumn.HeaderText = TitleRecources.Generic_Comment;

            BlockItem_PrepareTypeDataComboBox(BlockItemPreMarkComboBox, File.Definition.BlockTypePreMarks.ToList());
            BlockItem_PrepareTypeDataComboBox(BlockItemPostMarkComboBox, File.Definition.BlockTypePostMarks.ToList());

            BlockItemViewPanel.Controls.Add(BlockItemAttributesPanel, 0, 2);
            BlockItemViewPanel.SetColumnSpan(BlockItemAttributesPanel, 2);
            BlockItemAttributesPanel.InheritorChanged += BlockItemAttributesPanel_InheritorChanged;

            BlockItemParametersGrid.SuspendLayout();
            BlockItemParametersGrid.Rows.Clear();
            BlockItemParametersGrid.Rows.AddRange(File.Definition.BlockParameters.Select(param => new ParameterRow(param)).ToArray());
            BlockItemParametersGrid.ResumeLayout();
        }
        private void BlockItem_PrepareTypeDataComboBox(ComboBox box, List<BlockValueDefinition> definitions)
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
        private void BlockItem_PreActivationChecks_Editing()
        {
            PresetBlockCheckedPreMark = BlockItem_PreActivationChecks_Editing_DataType(File.Definition.BlockTypePreMarks, PresetBlock.PreMark,
                TextRecources.EcfItemEditingDialog_NoDefinitionForThisPreMark);

            if (CreateableItemTypes == CreateableItems.ChildBlock)
            {
                PresetBlockCheckedDataType = BlockItem_PreActivationChecks_Editing_DataType(File.Definition.ChildBlockTypes, PresetBlock.DataType,
                    TextRecources.EcfItemEditingDialog_NoDefinitionForThisDataType);
            }
            else if (CreateableItemTypes == CreateableItems.RootBlock)
            {
                PresetBlockCheckedDataType = BlockItem_PreActivationChecks_Editing_DataType(File.Definition.RootBlockTypes, PresetBlock.DataType,
                    TextRecources.EcfItemEditingDialog_NoDefinitionForThisDataType);
            }

            PresetBlockCheckedPostMark = BlockItem_PreActivationChecks_Editing_DataType(File.Definition.BlockTypePostMarks, PresetBlock.PostMark,
                TextRecources.EcfItemEditingDialog_NoDefinitionForThisPostMark);
        }
        private string BlockItem_PreActivationChecks_Editing_DataType(
            ReadOnlyCollection<BlockValueDefinition> definition, string typeToCheck, string typeUnknownText)
        {
            if (definition.Count > 0)
            {
                if (definition.Any(type => !type.IsOptional) && !definition.Any(mark => mark.Value.Equals(typeToCheck)))
                {
                    MessageBox.Show(this, typeUnknownText, TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    if (ItemSelector.ShowDialog(this, definition.Select(type => type.Value).ToArray()) == DialogResult.OK)
                    {
                        return Convert.ToString(ItemSelector.SelectedItem);
                    }
                    throw new InvalidOperationException("No editing without Definition");
                }
            }
            return typeToCheck;
        }
        private void BlockItem_UpdateDefinition(CreateableItems selectedBlockType)
        {
            switch (selectedBlockType)
            {
                case CreateableItems.ChildBlock:
                    BlockTypeDefinitions = File.Definition.ChildBlockTypes.ToList();
                    BlockAttributeDefinitions = File.Definition.ChildBlockAttributes.ToList();
                    break;
                case CreateableItems.RootBlock:
                    BlockTypeDefinitions = File.Definition.RootBlockTypes.ToList();
                    BlockAttributeDefinitions = File.Definition.RootBlockAttributes.ToList();
                    break;
                default: throw new ArgumentException(string.Format("No creator defined for item type {0}....that shouldn't happen :)", selectedBlockType.ToString()));
            }
        }
        private void BlockItem_PreActivationChecks_Adding()
        {
            if (File.Definition.BlockTypePostMarks.Count < 1)
            {
                CreateableItemTypes &= ~CreateableItems.ChildBlock;
                CreateableItemTypes &= ~CreateableItems.RootBlock;
                return;
            }
            if (CreateableItemTypes.HasFlag(CreateableItems.ChildBlock))
            {
                if (File.Definition.ChildBlockTypes.Count < 1)
                {
                    CreateableItemTypes &= ~CreateableItems.ChildBlock;
                }
            }
            if (CreateableItemTypes.HasFlag(CreateableItems.RootBlock))
            {
                if (File.Definition.RootBlockTypes.Count < 1)
                {
                    CreateableItemTypes &= ~CreateableItems.RootBlock;
                }
            }
        }
        private void BlockItem_ActivateView(CreateableItems selectedBlockType)
        {
            BackButton.Enabled = CreateableItemTypes != selectedBlockType;
            ResetButton.Enabled = PresetBlock != null;
            SelectedItemType = selectedBlockType;

            BlockItem_ActivateViewHeader();

            BlockItem_UpdateDefinition(selectedBlockType);

            BlockItem_PrepareTypeDataComboBox(BlockItemDataTypeComboBox, BlockTypeDefinitions);

            BlockItem_UpdateView();

            // hack to prevent tab switch with tab key
            ViewPanel.TabPages.Clear();
            ViewPanel.TabPages.Add(BlockItemView);

            OkButton.Focus();
        }
        private void BlockItem_ActivateViewHeader()
        {
            if (PresetBlock != null) 
            {
                if (SelectedItemType == CreateableItems.ChildBlock)
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
                if (SelectedItemType == CreateableItems.ChildBlock)
                {
                    Text = TitleRecources.EcfItemEditingDialog_Header_AddChildBlock;
                }
                else
                {
                    Text = TitleRecources.EcfItemEditingDialog_Header_AddRootBlock;
                }
            }
        }
        private void BlockItem_UpdateView()
        {
            // comments
            BlockItemCommentTextBox.Text = PresetBlock != null ? string.Join(" / ", PresetBlock.Comments) : string.Empty;

            // marks und type
            if (PresetBlock != null)
            {
                BlockItemPreMarkComboBox.SelectedItem = BlockItemPreMarkComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Value == PresetBlockCheckedPreMark);
                BlockItemDataTypeComboBox.SelectedItem = BlockItemDataTypeComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Value == PresetBlockCheckedDataType);
                BlockItemPostMarkComboBox.SelectedItem = BlockItemPostMarkComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Value == PresetBlockCheckedPostMark);
            }

            EcfBlock inheritorBlock = PresetBlock?.Inheritor;

            // attributes
            BlockItemAttributesPanel.GenerateAttributes(File.Definition, BlockAttributeDefinitions.AsReadOnly());
            BlockItemAttributesPanel.UpdateAttributes(PresetBlock, inheritorBlock);

            // parameter
            BlockItem_UpdateParametersGrid();
            BlockItem_UpdateParametersInheritance(inheritorBlock);
        }
        private void BlockItem_UpdateParametersGrid()
        {
            List<EcfParameter> presentParameters = PresetBlock?.ChildItems.Where(child => child is EcfParameter).Cast<EcfParameter>().ToList();
            BlockItemParametersGrid.SuspendLayout();
            foreach (DataGridViewRow row in BlockItemParametersGrid.Rows)
            {
                if (row is ParameterRow paramRow)
                {
                    EcfParameter parameter = presentParameters?.FirstOrDefault(param => param.Key.Equals(paramRow.Definition.Name));
                    if (parameter == null)
                    {
                        paramRow.InitRow(false, false, "");
                    }
                    else
                    {
                        paramRow.InitRow(true, false, string.Join(" / ", parameter.Comments));
                    }
                }
            }
            BlockItemParametersGrid.ResumeLayout();
        }
        private void BlockItem_UpdateParametersInheritance(EcfBlock inheritor)
        {
            BlockItemParametersGrid.SuspendLayout();
            foreach (DataGridViewRow row in BlockItemParametersGrid.Rows)
            {
                if (row is ParameterRow paramRow)
                {
                    paramRow.UpdateInherited(inheritor?.HasParameter(paramRow.Definition.Name) ?? false);
                }
            }
            BlockItemParametersGrid.ResumeLayout();
            BlockItemInheritorTextBox.Text = inheritor?.BuildIdentification() ?? string.Empty;
        }
        private List<string> BlockItem_ValidateInputs()
        {
            List<string> errors = new List<string>();
            errors.AddRange(BlockItem_ValidateTypeData());
            errors.AddRange(BlockItemAttributesPanel.ValidateIdName(IdentifyingBlockList, ReferencedBlockList, PresetBlock));
            errors.AddRange(BlockItemAttributesPanel.ValidateRefTarget(ReferencingBlockList, PresetBlock));
            errors.AddRange(BlockItemAttributesPanel.ValidateRefSource(ReferencedBlockList));
            errors.AddRange(BlockItemAttributesPanel.ValidateAttributeValues());
            return errors;
        }
        private List<string> BlockItem_ValidateTypeData()
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
        private EcfBlock BlockItem_PrepareResultItem()
        {
            List<EcfParameter> activeParameters = BlockItem_PrepareActiveParameters();
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
                BlockItem_PrepareResultParameterUpdate(PresetBlock, activeParameters);
                PresetBlock.ClearAttributes();
                PresetBlock.AddAttributes(attributes);
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
        private List<EcfParameter> BlockItem_PrepareActiveParameters()
        {
            List<EcfParameter> parameters = new List<EcfParameter>();
            foreach (DataGridViewRow row in BlockItemParametersGrid.Rows)
            {
                if (row is ParameterRow param && param.IsActive())
                {
                    parameters.Add(new EcfParameter(param.Definition.Name));
                }
            }
            return parameters;
        }
        private void BlockItem_PrepareResultParameterUpdate(EcfBlock block, List<EcfParameter> activeParameters)
        {
            List<EcfParameter> presentParameters = block.ChildItems.Where(child => child is EcfParameter).Cast<EcfParameter>().ToList();

            List<EcfParameter> unknownParameters = presentParameters.Where(parameter => parameter.Definition == null).ToList();
            List<EcfParameter> doubledParameters = presentParameters.Except(presentParameters.Distinct(ParamKeyComparer)).ToList();
            List<EcfParameter> removedParameters = presentParameters.Except(activeParameters, ParamKeyComparer).ToList();
            List<EcfParameter> createdParameters = activeParameters.Except(presentParameters, ParamKeyComparer).ToList();

            block.RemoveChilds(unknownParameters);
            block.RemoveChilds(doubledParameters);
            block.RemoveChilds(removedParameters);
            block.AddChilds(createdParameters);

            foreach (EcfParameter parameter in createdParameters)
            {
                parameter.Revalidate();
            }
        }

        // selecting section
        private void SelectItem_InitView()
        {
            SelectCommentItemRadioButton.Text = TitleRecources.Generic_Comment;
            SelectParameterItemRadioButton.Text = TitleRecources.Generic_Parameter;
            SelectChildBlockItemRadioButton.Text = TitleRecources.Generic_ChildElement;
            SelectRootBlockItemRadioButton.Text = TitleRecources.Generic_RootElement;
        }
        private void SelectItem_ActivateView()
        {
            BackButton.Enabled = false;
            ResetButton.Enabled = false;
            SelectedItemType = CreateableItems.None;

            Text = TitleRecources.EcfItemEditingDialog_Header_SelectItem;

            SelectCommentItemRadioButton.Visible = CreateableItemTypes.HasFlag(CreateableItems.Comment);
            SelectParameterItemRadioButton.Visible = CreateableItemTypes.HasFlag(CreateableItems.Parameter);
            SelectChildBlockItemRadioButton.Visible = CreateableItemTypes.HasFlag(CreateableItems.ChildBlock);
            SelectRootBlockItemRadioButton.Visible = CreateableItemTypes.HasFlag(CreateableItems.RootBlock);

            SelectCommentItemRadioButton.Checked = false;
            SelectParameterItemRadioButton.Checked = false;
            SelectChildBlockItemRadioButton.Checked = false;
            SelectRootBlockItemRadioButton.Checked = false;

            // hack to prevent tab switch with tab key
            ViewPanel.TabPages.Clear();
            ViewPanel.TabPages.Add(SelectItemView);

            OkButton.Focus();
        }

        // subclasses
        [Obsolete("useless struct for just one property")]
        private class ParameterItem_ValueColumn : DataGridViewTextBoxColumn
        {
            public ParameterItem_ValueColumn() : base()
            {
                SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        private class ParameterRow : DataGridViewRow
        {
            public ItemDefinition Definition { get; }

            private DataGridViewCheckBoxCell ActiveCell { get; } = new DataGridViewCheckBoxCell();
            private DataGridViewCheckBoxCell InheritedCell { get; } = new DataGridViewCheckBoxCell();
            private DataGridViewTextBoxCell NameCell { get; } = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell InfoCell { get; } = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell CommentCell { get; } = new DataGridViewTextBoxCell();

            public ParameterRow(ItemDefinition definition) : base()
            {
                Cells.Add(ActiveCell);
                Cells.Add(InheritedCell);
                Cells.Add(NameCell);
                Cells.Add(InfoCell);
                Cells.Add(CommentCell);

                Definition = definition;

                ActiveCell.ReadOnly = !definition.IsOptional;
                if (ActiveCell.ReadOnly) { ActiveCell.Style.BackColor = Color.LightGray; }

                NameCell.Value = definition.Name;
                InfoCell.Value = definition.Info;
            }

            public void InitRow(bool active, bool inherited, string comment)
            {
                if (Definition.IsOptional)
                {
                    ActiveCell.Value = active;
                }
                else
                {
                    ActiveCell.Value = true;
                }
                InheritedCell.Value = inherited;
                CommentCell.Value = comment;
            }
            public bool IsActive()
            {
                return Convert.ToBoolean(ActiveCell.Value);
            }
            public void UpdateInherited(bool state)
            {
                InheritedCell.Value = state;
            }
        }
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
            private EcfDataGridView Grid { get; } = new EcfDataGridView();

            private EcfItemSelectorDialog ItemSelector { get; set; } = new EcfItemSelectorDialog();
            public List<EcfBlock> ReferencedBlockList { get; set; } = null;

            private int PrefixColumnCount { get; } = 3;
            private DataGridViewCheckBoxColumn ActivationColumn { get; } = new DataGridViewCheckBoxColumn();
            private DataGridViewTextBoxColumn InfoColumn { get; } = new DataGridViewTextBoxColumn();
            private DataGridViewTextBoxColumn NameColumn { get; } = new DataGridViewTextBoxColumn();

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
                RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
                RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
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
                    if (ItemSelector.ShowDialog(this, ReferencedBlockList.ToArray()) == DialogResult.OK)
                    {
                        EcfBlock inheritor = ItemSelector.SelectedItem as EcfBlock;
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
                            if (ItemSelector.ShowDialog(this, ReferencedBlockList.ToArray()) == DialogResult.OK)
                            {
                                EcfBlock inheritor = ItemSelector.SelectedItem as EcfBlock;
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
                                ActivateNewValue(row);
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
                if (TryFindSelectedRow(out AttributeRow row) && row.ItemDef.HasValue)
                {
                    row.SetActive(true);
                    ActivateNewValue(row);
                }
            }
            private void RemoveValueButton_Click(object sender, EventArgs evt)
            {
                if (TryFindSelectedRow(out AttributeRow row))
                {
                    if (!row.DeactivateLastUsedCell())
                    {
                        row.SetActive(false);
                    }
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
                Grid.Columns.Add(InfoColumn);
                Grid.Columns.Add(NameColumn);
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
                UpdateValueColumns(presentAttributes);
                foreach (DataGridViewRow row in Grid.Rows)
                {
                    if (row is AttributeRow attrRow)
                    {
                        EcfAttribute attribute = presentAttributes?.FirstOrDefault(attr => attr.Key.Equals(attrRow.ItemDef.Name));
                        if (attribute == null)
                        {
                            attrRow.InitRow(false);
                        }
                        else if (attrRow.IsReferenceSource)
                        {
                            attrRow.InitRow(true, inheritorBlock);
                        }
                        else
                        {
                            attrRow.InitRow(true, attribute.GetAllValues().ToArray());
                        }
                    }
                }
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
            public List<string> ValidateIdName(List<EcfBlock> identifyingBlockList, List<EcfBlock> referencedBlockList, EcfBlock presetBlock)
            {
                List<string> errors = new List<string>();
                if (identifyingBlockList != null)
                {
                    AttributeRow idRow = Grid.Rows.Cast<AttributeRow>().FirstOrDefault(row => row.IsIdentification);
                    if (idRow != null)
                    {
                        string value = idRow.GetValues().FirstOrDefault();
                        foreach (EcfBlock block in identifyingBlockList.Where(block => !(presetBlock?.Equals(block) ?? false)
                            && value.Equals(block.GetAttributeFirstValue(idRow.ItemDef.Name))))
                        {
                            errors.Add(string.Format("{0} '{1}' {2} {3}", TextRecources.EcfItemEditingDialog_TheIdAttributeValue,
                                value, TextRecources.EcfItemEditingDialog_IsAlreadyUsedBy, block.BuildIdentification()));
                        }
                    }
                }
                if (referencedBlockList != null)
                {
                    AttributeRow nameRow = Grid.Rows.Cast<AttributeRow>().FirstOrDefault(row => row.IsReferenceTarget);
                    if (nameRow != null)
                    {
                        string value = nameRow.GetValues().FirstOrDefault();
                        foreach (EcfBlock block in referencedBlockList.Where(block => !(presetBlock?.Equals(block) ?? false)
                             && value.Equals(block.GetAttributeFirstValue(nameRow.ItemDef.Name))))
                        {
                            errors.Add(string.Format("{0} '{1}' {2} {3}", TextRecources.EcfItemEditingDialog_TheNameAttributeValue,
                                value, TextRecources.EcfItemEditingDialog_IsAlreadyUsedBy, block.BuildIdentification()));
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
                                    oldTargetValue, TextRecources.EcfItemEditingDialog_IsStillReferencedBy, block.BuildIdentification()));
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
                        if (!referencedBlockList.Any(block => block.Equals(sourceRefRow.Inheritor)))
                        {
                            errors.Add(string.Format("{0} '{1}' {2}", TextRecources.EcfItemEditingDialog_TheReferencedItem,
                                sourceRefRow.Inheritor?.BuildIdentification(), TextRecources.EcfItemEditingDialog_CouldNotBeFoundInTheItemList));
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

                new ToolTip().SetToolTip(AddValueButton, TextRecources.EcfItemEditingDialog_ToolTip_AddValue);
                new ToolTip().SetToolTip(RemoveValueButton, TextRecources.EcfItemEditingDialog_ToolTip_RemoveValue);

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
                ActivationColumn.Name = TitleRecources.Generic_Active;

                InfoColumn.DefaultCellStyle.BackColor = Color.LightGray;
                InfoColumn.HeaderText = TitleRecources.Generic_Info;
                InfoColumn.Name = TitleRecources.Generic_Active;
                InfoColumn.ReadOnly = true;
                InfoColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                NameColumn.DefaultCellStyle.BackColor = Color.LightGray;
                NameColumn.HeaderText = TitleRecources.Generic_Name;
                NameColumn.Name = TitleRecources.Generic_Active;
                NameColumn.ReadOnly = true;
                NameColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                Grid.CellClick += Grid_CellClick;
                Grid.CellContentClick += Grid_CellContentClick;
            }
            private void UpdateValueColumns(List<EcfAttribute> presentAttributes)
            {
                int maxCount = 0;
                if (presentAttributes != null && presentAttributes.Count > 0)
                {
                    maxCount = presentAttributes.Max(attr => attr.GetAllValues().Count);
                }
                while (Grid.Columns.Count - PrefixColumnCount < maxCount)
                {
                    AddValueColumn();
                }
            }
            private void ActivateNewValue(AttributeRow row)
            {
                if (!row.ActivateNextFreeCell())
                {
                    AddValueColumn();
                    row.ActivateNextFreeCell();
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
            private bool TryFindSelectedRow(out AttributeRow row)
            {
                DataGridViewCell cell = Grid.SelectedCells.Cast<DataGridViewCell>().FirstOrDefault();
                row = cell == null ? null : Grid.Rows[cell.RowIndex] as AttributeRow;
                return cell != null;
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

                private int PrefixColumnCount { get; } = 3;
                private DataGridViewCheckBoxCell ActivationCell { get; } = new DataGridViewCheckBoxCell();
                private DataGridViewTextBoxCell InfoCell { get; } = new DataGridViewTextBoxCell();
                private DataGridViewTextBoxCell NameCell { get; } = new DataGridViewTextBoxCell();

                public AttributeRow(ItemDefinition definition, FormatDefinition format) : base()
                {
                    Cells.Add(ActivationCell);
                    Cells.Add(InfoCell);
                    Cells.Add(NameCell);

                    ItemDef = definition;
                    FormDef = format;

                    IsIdentification = definition.Name.Equals(format.BlockIdentificationAttribute);
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
                    SetActiveState(active);
                    SetValues(values);
                }
                public void InitRow(bool active, EcfBlock inheritor)
                {
                    SetActiveState(active);
                    SetInheritor(inheritor);
                }
                public void SetInheritor(EcfBlock inheritor)
                {
                    Inheritor = inheritor;
                    SetValues(inheritor?.GetAttributeFirstValue(ReferenceTargetAttribute) ?? string.Empty);
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
                public bool ActivateNextFreeCell()
                {
                    DataGridViewCell nextCell = Cells.Cast<DataGridViewCell>().Skip(PrefixColumnCount).FirstOrDefault(cell => cell.Tag == null);
                    if (nextCell == null) { return false; }
                    ActivateCell(nextCell);
                    return true;
                }
                public bool DeactivateLastUsedCell()
                {
                    DataGridViewCell lastCell = Cells.Cast<DataGridViewCell>().Skip(PrefixColumnCount).LastOrDefault(cell => cell.Tag != null);
                    if (lastCell == null) { return false; }
                    DeactivateCell(lastCell);
                    return true;
                }
                public void Inactivate()
                {
                    foreach(DataGridViewCell cell in Cells.Cast<DataGridViewCell>().Skip(PrefixColumnCount))
                    {
                        DeactivateCell(cell);
                    }
                }

                // privates
                private void SetActiveState(bool state)
                {
                    if (ItemDef.IsOptional)
                    {
                        ActivationCell.Value = state;
                    }
                    else
                    {
                        ActivationCell.Value = true;
                    }
                }
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
    }
}
