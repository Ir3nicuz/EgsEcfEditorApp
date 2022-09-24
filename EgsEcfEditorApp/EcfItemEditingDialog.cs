using EcfFileViewTools;
using EcfWinFormControls;
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
using static EgsEcfParser.EcfFormatChecking;
using static Helpers.EnumLocalisation;

namespace EcfFileViews
{
    public partial class EcfItemEditingDialog : Form
    {
        public EcfStructureItem ResultItem { get; private set; } = null;

        [Flags]
        public enum Modes
        {
            None = 0,
            Comment = 1 << 0,
            Parameter = 1 << 1,
            RootBlock = 1 << 2,
            ChildBlock = 1 << 3,
            ParameterMatrix = 1 << 4,
        }

        private EcfComment PresetComment { get; set; } = null;
        private EcfParameter PresetParameter { get; set; } = null;
        private EcfBlock PresetBlock { get; set; } = null;
        private EcfBlock ParentBlock { get; set; }
        private EgsEcfFile File { get; set; } = null;
        private Modes CreationModes { get; set; } = Modes.None;
        private Modes SelectedItemType { get; set; } = Modes.None;
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

        private ParameterPanel ParameterItemValuesPanel { get; } = new ParameterPanel(ParameterPanel.ParameterModes.Value);
        private ParameterPanel ParameterMatrixPanel { get; } = new ParameterPanel(ParameterPanel.ParameterModes.Matrix);
        private AttributesPanel ParameterItemAttributesPanel { get; } = new AttributesPanel();
        private AttributesPanel BlockItemAttributesPanel { get; } = new AttributesPanel();
        private ParameterPanel BlockItemParametersPanel { get; } = new ParameterPanel(ParameterPanel.ParameterModes.Block);

        public EcfItemEditingDialog(EgsEcfFile file)
        {
            InitializeComponent();
            InitForm(file);
        }

        // events
        // unspecific
        private void InitForm(EgsEcfFile file)
        {
            Icon = IconRecources.Icon_AppBranding;
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
            ParameterMatrix_InitView();
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
                case Modes.Comment: CommentItem_UpdateView(); break;
                case Modes.Parameter: ParameterItem_UpdateView(); break;
                case Modes.ChildBlock: BlockItem_UpdateView(); break;
                case Modes.RootBlock: BlockItem_UpdateView(); break;
                case Modes.ParameterMatrix: ParameterMatrix_UpdateView(); break;
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
        private void SelectCommentItemButton_Click(object sender, EventArgs evt)
        {
            CommentItem_ActivateView();
        }
        private void SelectParameterItemButton_Click(object sender, EventArgs evt)
        {
            ParameterItem_ActivateView();
        }
        private void SelectChildBlockItemButton_Click(object sender, EventArgs evt)
        {
            BlockItem_ActivateView(Modes.ChildBlock);
        }
        private void SelectRootBlockItemButton_Click(object sender, EventArgs evt)
        {
            BlockItem_ActivateView(Modes.RootBlock);
        }
        // parameter
        private void ParameterItemKeyComboBox_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            ParameterItem_UpdateDefinition(Convert.ToString(ParameterItemKeyComboBox.SelectedItem));
            ParameterItemValuesPanel.UpdateParameterValues(ParameterDefinition, PresetParameter);
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
            CreationModes = Modes.Comment;

            CommentItem_ActivateView();

            return ShowDialog(parent);
        }
        public DialogResult ShowDialog(IWin32Window parent, EgsEcfFile file, EcfParameter presetParameter)
        {
            ResultItem = null;
            ParentBlock = presetParameter?.Parent as EcfBlock;
            File = file;
            PresetParameter = presetParameter;
            CreationModes = Modes.Parameter;

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
            CreationModes = presetBlock.IsRoot() ? Modes.RootBlock : Modes.ChildBlock;

            try
            {
                BlockItem_PreActivationChecks_Editing();
            }
            catch (Exception)
            {
                return DialogResult.Abort;
            }
            BlockItem_ActivateView(CreationModes);

            return ShowDialog(parent);
        }
        public DialogResult ShowDialog(IWin32Window parent, EgsEcfFile file, List<EcfParameter> parameters)
        {
            ResultItem = null;
            ParentBlock = null;
            File = file;
            PresetParameter = null;
            CreationModes = Modes.ParameterMatrix;

            try
            {
                ParameterMatrix_PreActivationChecks(parameters);
            }
            catch (Exception)
            {
                return DialogResult.Abort;
            }
            ParameterMatrix_ActivateView(parameters);

            return ShowDialog(parent);
        }
        public DialogResult ShowDialog(IWin32Window parent, EgsEcfFile file, Modes createable, EcfBlock parentBlock)
        {
            ResultItem = null;
            ParentBlock = parentBlock;
            File = file;
            Generic_BuildBlockCompareLists(file);
            Generic_ClearPresets();
            CreationModes = createable;

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
            IdentifyingBlockList = blocks?.Where(block => block.HasAttribute(file.Definition.BlockIdentificationAttribute, out _)).ToList();
            ReferencingBlockList = blocks?.Where(block => block.HasAttribute(file.Definition.BlockReferenceSourceAttribute, out _)).ToList();
            ReferencedBlockList = blocks?.Where(block => block.HasAttribute(file.Definition.BlockReferenceTargetAttribute, out _)).ToList();
            BlockItemAttributesPanel.ReferencedBlockList = ReferencedBlockList;
        }
        private void Generic_PreActivationChecks_AddingMulti()
        {
            ParameterItem_PreActivationChecks_Adding();
            BlockItem_PreActivationChecks_Adding();
            if (CreationModes == Modes.None)
            {
                throw new InvalidOperationException("No item type to add left");
            }
            switch (CreationModes)
            {
                case Modes.Comment: CommentItem_ActivateView(); break;
                case Modes.Parameter: ParameterItem_ActivateView(); break;
                case Modes.RootBlock: BlockItem_ActivateView(Modes.RootBlock); break;
                case Modes.ChildBlock: BlockItem_ActivateView(Modes.ChildBlock); break;
                default: SelectItem_ActivateView(); break;
            }
        }
        private List<string> Generic_ValidateInputs()
        {
            switch (SelectedItemType)
            {
                case Modes.Comment: return CommentItem_ValidateInputs();
                case Modes.Parameter: return ParameterItem_ValidateInputs();
                case Modes.RootBlock: return BlockItem_ValidateInputs();
                case Modes.ChildBlock: return BlockItem_ValidateInputs();
                case Modes.ParameterMatrix: return ParameterMatrix_ValidateInputs();
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
                case Modes.Comment: return CommentItem_PrepareResultItem();
                case Modes.Parameter: return ParameterItem_PrepareResultItem();
                case Modes.RootBlock: return BlockItem_PrepareResultItem();
                case Modes.ChildBlock: return BlockItem_PrepareResultItem();
                case Modes.ParameterMatrix: return ParameterMatrix_PrepareResultItem();
                default: throw new ArgumentException(string.Format("No creator defined for item type {0}....that shouldn't happen", SelectedItemType.ToString()));
            }
        }

        // selecting section
        private void SelectItem_InitView()
        {
            SelectCommentItemButton.Text = TitleRecources.Generic_Comment;
            SelectParameterItemButton.Text = TitleRecources.Generic_Parameter;
            SelectChildBlockItemButton.Text = TitleRecources.Generic_ChildElement;
            SelectRootBlockItemButton.Text = TitleRecources.Generic_RootElement;
        }
        private void SelectItem_ActivateView()
        {
            BackButton.Enabled = false;
            ResetButton.Enabled = false;
            SelectedItemType = Modes.None;

            Text = TitleRecources.EcfItemEditingDialog_Header_SelectItem;

            SelectCommentItemButton.Enabled = CreationModes.HasFlag(Modes.Comment);
            SelectParameterItemButton.Enabled = CreationModes.HasFlag(Modes.Parameter);
            SelectChildBlockItemButton.Enabled = CreationModes.HasFlag(Modes.ChildBlock);
            SelectRootBlockItemButton.Enabled = CreationModes.HasFlag(Modes.RootBlock);

            // hack to prevent tab switch with tab key
            ViewPanel.TabPages.Clear();
            ViewPanel.TabPages.Add(SelectItemView);

            OkButton.Focus();
        }

        // comment section
        private void CommentItem_InitView()
        {

        }
        private void CommentItem_ActivateView()
        {
            BackButton.Enabled = CreationModes != Modes.Comment;
            ResetButton.Enabled = PresetComment != null;
            SelectedItemType = Modes.Comment;

            CommentItem_ActivateViewHeader();

            CommentItem_UpdateView();

            // hack to prevent tab switch with tab key
            ViewPanel.TabPages.Clear();
            ViewPanel.TabPages.Add(CommentItemView);

            CommentItemRichTextBox.Focus();
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
            if (PresetComment == null)
            {
                CommentItemRichTextBox.Clear();
            }
            else
            {
                CommentItemRichTextBox.Lines = PresetComment.Comments.ToArray();
            }
        }
        private List<string> CommentItem_ValidateInputs()
        {
            List<string> errors = new List<string>();
            if (CommentItemRichTextBox.Lines.Any(line => line.Equals(string.Empty)))
            {
                errors.Add(TextRecources.EcfItemEditingDialog_CommentItemError_Empty);
            }
            return errors;
        }
        private EcfComment CommentItem_PrepareResultItem()
        {
            if (PresetComment == null)
            {
                PresetComment = new EcfComment(CommentItemRichTextBox.Lines.ToList());
            }
            else
            {
                PresetComment.ClearComments();
                PresetComment.AddComments(CommentItemRichTextBox.Lines.ToList());
            }
            return PresetComment;
        }

        // parameter section
        private void ParameterItem_InitView()
        {
            ParameterItemKeyLabel.Text = TitleRecources.Generic_Name;
            ParameterItemIsOptionalCheckBox.Text = TitleRecources.Generic_IsOptional;
            ParameterItemParentLabel.Text = TitleRecources.Generic_ParentElement;
            ParameterItemInfoLabel.Text = TitleRecources.Generic_Info;
            ParameterItemCommentLabel.Text = TitleRecources.Generic_Comment;

            ParameterItemViewPanel.Controls.Add(ParameterItemAttributesPanel, 0, 1);
            ParameterItemViewPanel.SetColumnSpan(ParameterItemAttributesPanel, 2);

            ParameterItemViewPanel.Controls.Add(ParameterItemValuesPanel, 0, 2);
            ParameterItemViewPanel.SetColumnSpan(ParameterItemValuesPanel, 2);
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
            if (CreationModes.HasFlag(Modes.Parameter))
            {
                if (ParentBlock == null) { CreationModes &= ~Modes.Parameter; return; }

                List<string> definedParameters = File.Definition.BlockParameters.Select(param => param.Name).ToList();
                if (definedParameters.Count < 1) { CreationModes &= ~Modes.Parameter; return; }

                List<string> addableParameters = definedParameters.Except(ParentBlock.ChildItems.Where(child => 
                    child is EcfParameter).Cast<EcfParameter>().Select(param => param.Key)).ToList();
                if (addableParameters.Count < 1) { CreationModes &= ~Modes.Parameter; return; }
            }
        }
        private void ParameterItem_ActivateView()
        {
            BackButton.Enabled = CreationModes != Modes.Parameter;
            ResetButton.Enabled = PresetParameter != null;
            SelectedItemType = Modes.Parameter;

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
            ParameterItemValuesPanel.UpdateParameterValues(ParameterDefinition, PresetParameter);

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
        private List<string> ParameterItem_ValidateInputs()
        {
            List<string> errors = new List<string>();
            errors.AddRange(ParameterItemValuesPanel.ValidateParameterValues(File.Definition));
            errors.AddRange(ParameterItemAttributesPanel.ValidateAttributeValues());
            return errors;
        }
        private EcfParameter ParameterItem_PrepareResultItem()
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

        // block section
        private void BlockItem_InitView()
        {
            BlockItemPreMarkLabel.Text = TitleRecources.Generic_PreMark;
            BlockItemDataTypeLabel.Text = TitleRecources.Generic_DataType;
            BlockItemPostMarkLabel.Text = TitleRecources.Generic_PostMark;
            BlockItemInheritorLabel.Text = TitleRecources.Generic_Inherited;
            BlockItemCommentLabel.Text = TitleRecources.Generic_Comment;

            BlockItem_PrepareTypeDataComboBox(BlockItemPreMarkComboBox, File.Definition.BlockTypePreMarks.ToList());
            BlockItem_PrepareTypeDataComboBox(BlockItemPostMarkComboBox, File.Definition.BlockTypePostMarks.ToList());

            BlockItemViewPanel.Controls.Add(BlockItemAttributesPanel, 0, 1);
            BlockItemViewPanel.SetColumnSpan(BlockItemAttributesPanel, 2);
            BlockItemAttributesPanel.InheritorChanged += BlockItemAttributesPanel_InheritorChanged;

            BlockItemViewPanel.Controls.Add(BlockItemParametersPanel, 0, 2);
            BlockItemViewPanel.SetColumnSpan(BlockItemParametersPanel, 2);
            BlockItemParametersPanel.GenerateParameterMatrix(File.Definition, File.Definition.BlockParameters);
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

            if (CreationModes == Modes.ChildBlock)
            {
                PresetBlockCheckedDataType = BlockItem_PreActivationChecks_Editing_DataType(File.Definition.ChildBlockTypes, PresetBlock.DataType,
                    TextRecources.EcfItemEditingDialog_NoDefinitionForThisDataType);
            }
            else if (CreationModes == Modes.RootBlock)
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
        private void BlockItem_UpdateDefinition(Modes selectedBlockType)
        {
            switch (selectedBlockType)
            {
                case Modes.ChildBlock:
                    BlockTypeDefinitions = File.Definition.ChildBlockTypes.ToList();
                    BlockAttributeDefinitions = File.Definition.ChildBlockAttributes.ToList();
                    break;
                case Modes.RootBlock:
                    BlockTypeDefinitions = File.Definition.RootBlockTypes.ToList();
                    BlockAttributeDefinitions = File.Definition.RootBlockAttributes.ToList();
                    break;
                default: throw new ArgumentException(string.Format("No creator defined for item type {0}....that shouldn't happen :)", selectedBlockType.ToString()));
            }
        }
        private void BlockItem_UpdateParametersInheritance(EcfBlock inheritor)
        {
            BlockItemParametersPanel.UpdateParameterInheritance(inheritor);
            BlockItemInheritorTextBox.Text = inheritor?.BuildIdentification() ?? string.Empty;
        }
        private void BlockItem_PreActivationChecks_Adding()
        {
            if (File.Definition.BlockTypePostMarks.Count < 1)
            {
                CreationModes &= ~Modes.ChildBlock;
                CreationModes &= ~Modes.RootBlock;
                return;
            }
            if (CreationModes.HasFlag(Modes.ChildBlock))
            {
                if (File.Definition.ChildBlockTypes.Count < 1)
                {
                    CreationModes &= ~Modes.ChildBlock;
                }
            }
            if (CreationModes.HasFlag(Modes.RootBlock))
            {
                if (File.Definition.RootBlockTypes.Count < 1)
                {
                    CreationModes &= ~Modes.RootBlock;
                }
            }
        }
        private void BlockItem_ActivateView(Modes selectedBlockType)
        {
            BackButton.Enabled = CreationModes != selectedBlockType;
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
                if (SelectedItemType == Modes.ChildBlock)
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
                if (SelectedItemType == Modes.ChildBlock)
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
            BlockItemParametersPanel.UpdateParameterMatrix(PresetBlock);
            BlockItem_UpdateParametersInheritance(inheritorBlock);
        }
        private List<string> BlockItem_ValidateInputs()
        {
            List<string> errors = new List<string>();
            errors.AddRange(BlockItem_ValidateTypeData());
            errors.AddRange(BlockItemAttributesPanel.ValidateIdName(IdentifyingBlockList, ReferencingBlockList, ReferencedBlockList, PresetBlock));
            errors.AddRange(BlockItemAttributesPanel.ValidateRefTarget(ReferencingBlockList, PresetBlock));
            errors.AddRange(BlockItemAttributesPanel.ValidateRefSource(ReferencedBlockList));
            errors.AddRange(BlockItemAttributesPanel.ValidateAttributeValues());
            errors.AddRange(BlockItemParametersPanel.ValidateParameterMatrix());
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
        private void BlockItem_PrepareResultParameterUpdate(EcfBlock block, List<EcfParameter> activeParameters)
        {
            List<EcfParameter> presentParameters = block.ChildItems.Where(child => child is EcfParameter).Cast<EcfParameter>().ToList();

            List<EcfParameter> doubledParameters = presentParameters.Except(presentParameters.Distinct(ParamKeyComparer)).ToList();
            List<EcfParameter> removedParameters = presentParameters.Except(activeParameters, ParamKeyComparer).ToList();
            removedParameters.RemoveAll(parameter => parameter.Definition == null);
            List<EcfParameter> createdParameters = activeParameters.Except(presentParameters, ParamKeyComparer).ToList();

            block.RemoveChilds(doubledParameters);
            block.RemoveChilds(removedParameters);
            block.AddChilds(createdParameters);

            foreach (EcfParameter parameter in createdParameters)
            {
                parameter.Revalidate();
            }
        }

        // parameter matrix section
        private void ParameterMatrix_InitView()
        {
            ParameterMatrixView.Controls.Add(ParameterMatrixPanel);
        }
        private void ParameterMatrix_PreActivationChecks(List<EcfParameter> parameters)
        {
            if (parameters.Any(parameter => parameter.Definition == null))
            {
                MessageBox.Show(this, TextRecources.EcfItemEditingDialog_ParameterMatrixForUnknownParametersNotAllowed,
                    TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                throw new InvalidOperationException("No matrix editing without Definition");
            }
            if (parameters.Any(parameter => !parameter.Definition.HasValue))
            {
                MessageBox.Show(this, TextRecources.EcfItemEditingDialog_ParameterMatrixForValueslessParametersNotAllowed,
                    TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                throw new InvalidOperationException("No matrix editing for parameters without values");
            }
        }
        private void ParameterMatrix_ActivateView(List<EcfParameter> parameters)
        {
            BackButton.Enabled = false;
            ResetButton.Enabled = true;
            SelectedItemType = Modes.ParameterMatrix;

            Text = TitleRecources.EcfItemEditingDialog_Header_EditParameterMatrix;

            ParameterMatrixPanel.GenerateParameterMatrix(File.Definition, parameters);

            ParameterMatrix_UpdateView();

            // hack to prevent tab switch with tab key
            ViewPanel.TabPages.Clear();
            ViewPanel.TabPages.Add(ParameterMatrixView);

            OkButton.Focus();
        }
        private void ParameterMatrix_UpdateView()
        {
            ParameterMatrixPanel.UpdateParameterMatrix();
        }
        private List<string> ParameterMatrix_ValidateInputs()
        {
            List<string> errors = new List<string>();
            errors.AddRange(ParameterMatrixPanel.ValidateParameterMatrix());
            return errors;
        }
        private EcfParameter ParameterMatrix_PrepareResultItem()
        {
            List<EcfParameter> editedParameters = ParameterMatrixPanel.PrepareResultParameters();

            foreach (EcfParameter parameter in editedParameters)
            {
                parameter.Revalidate();
            }

            return PresetParameter;
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
                                    value, TextRecources.EcfItemEditingDialog_IsAlreadyUsedBy, block.BuildIdentification()));
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
                            if (referencingBlockList != null && referencingBlockList.Any(listedBlock => value.Equals(listedBlock.RefSource)))
                            {
                                foreach (EcfBlock block in referencedBlockList.Where(block => !(presetBlock?.Equals(block) ?? false)
                                    && value.Equals(block.GetAttributeFirstValue(nameRow.ItemDef.Name))))
                                {
                                    errors.Add(string.Format("{0} '{1}' {2} {3}", TextRecources.EcfItemEditingDialog_TheNameAttributeValue,
                                        value, TextRecources.EcfItemEditingDialog_IsAlreadyUsedBy, block.BuildIdentification()));
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
                        string value = sourceRefRow.GetValues().FirstOrDefault();
                        int targetBlockCount = referencedBlockList.Count(block => block.RefTarget.Equals(value));
                        if (targetBlockCount < 1)
                        {
                            errors.Add(string.Format("{0} '{1}' {2}", TextRecources.EcfItemEditingDialog_TheReferencedItem,
                                sourceRefRow.Inheritor?.BuildIdentification(), TextRecources.EcfItemEditingDialog_CouldNotBeFoundInTheItemList));
                        }
                        else if (targetBlockCount > 1)
                        {
                            errors.Add(string.Format("{0} '{1}' {2}", TextRecources.EcfItemEditingDialog_TheReferencedItem,
                                sourceRefRow.Inheritor?.BuildIdentification(), TextRecources.EcfItemEditingDialog_HasNoUniqueIdentifier));
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
            private EcfDataGridView Grid { get; } = new EcfDataGridView();

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
                            paramRow.PresetParameter.AddValues(valueGroups);
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
    }
}
