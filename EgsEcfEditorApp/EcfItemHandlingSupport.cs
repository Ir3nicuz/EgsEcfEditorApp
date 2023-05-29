using EcfFileViews;
using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using GenericDialogs;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static EcfFileViews.ItemSelectorDialog;
using static EgsEcfEditorApp.EcfItemListingDialog;
using static EgsEcfEditorApp.OptionSelectorDialog;
using static EgsEcfParser.EcfDefinitionHandling;
using static EgsEcfParser.EcfStructureTools;

namespace EgsEcfEditorApp
{
    internal class EcfItemHandlingSupport
    {
        private GuiMainForm ParentForm { get; }

        private EcfItemEditingDialog EditItemDialog { get; } = new EcfItemEditingDialog();
        private OptionSelectorDialog OptionsDialog { get; } = new OptionSelectorDialog()
        {
            Icon = IconRecources.Icon_AppBranding,
            OkButtonText = TitleRecources.Generic_Ok,
            AbortButtonText = TitleRecources.Generic_Abort,
        };
        private OptionItem[] AddDependencyOptionItems { get; } = new OptionItem[]
        {
            new OptionItem(AddDependencyOptions.SelectExisting, EnumLocalisation.GetLocalizedEnum(AddDependencyOptions.SelectExisting)),
            new OptionItem(AddDependencyOptions.CreateNewAsCopy, EnumLocalisation.GetLocalizedEnum(AddDependencyOptions.CreateNewAsCopy)),
            new OptionItem(AddDependencyOptions.CreateNewAsEmpty, EnumLocalisation.GetLocalizedEnum(AddDependencyOptions.CreateNewAsEmpty)),
        };
        private OptionItem[] AddToDefinitionOptionItems { get; } = new OptionItem[]
        {
            new OptionItem(AddToDefinitionOptions.AllDefinitions, EnumLocalisation.GetLocalizedEnum(AddToDefinitionOptions.AllDefinitions)),
            new OptionItem(AddToDefinitionOptions.SelectDefinition, EnumLocalisation.GetLocalizedEnum(AddToDefinitionOptions.SelectDefinition)),
        };
        private OptionItem[] RemoveDependencyOptionItems { get; } = new OptionItem[]
        {
            new OptionItem(RemoveDependencyOptions.RemoveLinkToItemOnly, EnumLocalisation.GetLocalizedEnum(RemoveDependencyOptions.RemoveLinkToItemOnly)),
            new OptionItem(RemoveDependencyOptions.DeleteItemComplete, EnumLocalisation.GetLocalizedEnum(RemoveDependencyOptions.DeleteItemComplete)),
        };
        private ItemSelectorDialog ItemsDialog { get; } = new ItemSelectorDialog()
        {
            Icon = IconRecources.Icon_AppBranding,
            OkButtonText = TitleRecources.Generic_Ok,
            AbortButtonText = TitleRecources.Generic_Abort,
        };
        private ErrorListingDialog ErrorDialog { get; } = new ErrorListingDialog()
        {
            Text = TitleRecources.Generic_Attention,
            Icon = IconRecources.Icon_AppBranding,
            OkButtonText = TitleRecources.Generic_Ok,
            YesButtonText = TitleRecources.Generic_Yes,
            NoButtonText = TitleRecources.Generic_No,
            AbortButtonText = TitleRecources.Generic_Abort,
        };

        private enum AddDependencyOptions
        {
            SelectExisting,
            CreateNewAsCopy,
            CreateNewAsEmpty,
        }
        private enum AddToDefinitionOptions
        {
            AllDefinitions,
            SelectDefinition,
        }
        private enum RemoveDependencyOptions
        {
            RemoveLinkToItemOnly,
            DeleteItemComplete,
        }

        public EcfItemHandlingSupport(GuiMainForm parentForm)
        {
            ParentForm = parentForm;
        }

        // events
        private void ItemListingDialog_ShowItem(object sender, ItemRowClickedEventArgs evt)
        {
            EcfStructureItem itemToShow = evt.StructureItem;
            EcfTabPage tabPageToShow = ParentForm.GetTabPage(itemToShow.EcfFile);
            if (tabPageToShow == null)
            {
                MessageBox.Show(ParentForm, string.Format("{0}: {1}",
                    TextRecources.ItemHandlingSupport_SelectedFileNotOpened, itemToShow?.EcfFile?.FileName ?? TitleRecources.Generic_Replacement_Empty),
                    TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            tabPageToShow.ShowSpecificItem(itemToShow);
        }

        // publics
        public void ShowTemplateUsers(EcfBlock sourceTemplate)
        {
            List<EcfBlock> userList = GetBlockListByParameterValue(ParentForm.GetOpenedFiles(new Func<EgsEcfFile, bool>(file => file.Definition.IsDefiningItems)),
                true, true, sourceTemplate.GetName(), UserSettings.Default.ItemHandlingSupport_ParamKey_TemplateName.ToSeperated<string>().ToArray());
            ShowListingView(TextRecources.ItemHandlingSupport_AllElementsWithTemplate, sourceTemplate.BuildRootId(), userList);
        }
        public void ShowItemUsingTemplates(EcfBlock sourceItem)
        {
            List<EcfBlock> templateList = GetBlockListByParameterKey(ParentForm.GetOpenedFiles(new Func<EgsEcfFile, 
                bool>(file => file.Definition.IsDefiningTemplates)), true, sourceItem.GetName());
            ShowListingView(TextRecources.ItemHandlingSupport_AllTemplatesWithItem, sourceItem.BuildRootId(), templateList);
        }
        public void ShowParameterUsers(EcfParameter sourceParameter)
        {
            List<EcfBlock> itemList = ParentForm.GetOpenedFiles().SelectMany(file =>
                file.GetDeepItemList<EcfBlock>().Where(item => item.HasParameter(sourceParameter.Key, out _))).ToList();
            ShowListingView(TextRecources.ItemHandlingSupport_AllItemsWithParameter, sourceParameter.Key, itemList);
        }
        public void ShowValueUsers(EcfParameter sourceParameter)
        {
            if (sourceParameter.HasValue())
            {
                List<EcfParameter> paramList = ParentForm.GetOpenedFiles().SelectMany(file =>
                    file.GetDeepItemList<EcfParameter>().Where(parameter => ValueGroupListEquals(parameter.ValueGroups, sourceParameter.ValueGroups))).ToList();
                ShowListingView(TextRecources.ItemHandlingSupport_AllParametersWithValue, string.Join(", ", sourceParameter.GetAllValues()), paramList);
            }
            else
            {
                MessageBox.Show(ParentForm, string.Format("{0} {1} {2}", TitleRecources.Generic_Parameter, sourceParameter.Key, TextRecources.Generic_HasNoValue),
                    TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        public void ShowBlockUsingBlockGroups(EcfBlock sourceItem)
        {
            List<EcfBlock> blockGroupList = GetBlockListByParameterValue(ParentForm.GetOpenedFiles(new Func<EgsEcfFile, bool>(file => file.Definition.IsDefiningBuildBlockGroups)),
                false, false, sourceItem.GetName(), UserSettings.Default.ItemHandlingSupport_ParamKey_Blocks.ToSeperated<string>().ToArray());
            ShowListingView(TextRecources.ItemHandlingSupport_AllBlockGroupsWithBlock, sourceItem.BuildRootId(), blockGroupList);
        }
        public void ShowGlobalDefUsers(EcfBlock sourceGlobalDef)
        {
            List<EcfBlock> userList = GetBlockListByParameterValue(ParentForm.GetOpenedFiles(new Func<EgsEcfFile, bool>(file => file.Definition.IsDefiningGlobalMacroUsers)),
                false, true, sourceGlobalDef.GetName(), UserSettings.Default.ItemHandlingSupport_ParamKeys_GlobalRef.ToSeperated<string>().ToArray());
            ShowListingView(TextRecources.ItemHandlingSupport_AllElementsWithGlobalDef, sourceGlobalDef.BuildRootId(), userList);
        }
        public void ShowInheritedGlobalDefs(EcfBlock sourceItem)
        {
            List<EcfBlock> globalDefList = GetBlockListByNameOrParamValue(ParentForm.GetOpenedFiles(new Func<EgsEcfFile, bool>(file => file.Definition.IsDefiningGlobalMacros)),
                false, sourceItem, UserSettings.Default.ItemHandlingSupport_ParamKeys_GlobalRef.ToSeperated<string>().ToArray());
            ShowListingView(TextRecources.ItemHandlingSupport_AllGlobalDefsInheritedInItem, sourceItem.BuildRootId(), globalDefList);
        }
        public void ShowLinkedTemplate(EcfBlock sourceItem)
        {
            List<EcfBlock> templateList = GetBlockListByNameOrParamValue(ParentForm.GetOpenedFiles(new Func<EgsEcfFile, bool>(file => file.Definition.IsDefiningTemplates)),
                true, sourceItem, UserSettings.Default.ItemHandlingSupport_ParamKey_TemplateName.ToSeperated<string>().ToArray());
            ShowLinkedBlocks(templateList, sourceItem, TextRecources.ItemHandlingSupport_NoTemplatesForItem, TextRecources.ItemHandlingSupport_AllTemplatesForItem);
        }
        public void AddTemplateToItem(EcfBlock sourceItem)
        {
            try
            {
                if (!AddDependencyToItem_TryFindTargetFiles(new Func<EgsEcfFile, bool>(file => file.Definition.IsDefiningTemplates),
                    TextRecources.ItemHandlingSupport_NoTemplateFileOpened, out SelectorItem[] presentTemplateFiles)) { return; }

                List<EcfBlock> templateList = GetBlockListByNameOrParamValue(presentTemplateFiles.Select(item => item.Item as EgsEcfFile).ToList(),
                    true, sourceItem, UserSettings.Default.ItemHandlingSupport_ParamKey_TemplateName.ToSeperated<string>().ToArray());
                if (templateList.Count < 1)
                {
                    AddDependencyToItem_PerformSelectedOption(TitleRecources.ItemHandlingSupport_AddTemplateOptionSelector, 
                        () => AddTemplateToItem_SelectFromExisting(sourceItem), 
                        () => AddTemplateToItem_CreateCopy(sourceItem, presentTemplateFiles), 
                        () => AddTemplateToItem_CreateNew(sourceItem, presentTemplateFiles));
                    return;
                }
                MessageBox.Show(ParentForm, TextRecources.ItemHandlingSupport_ElementHasAlreadyTemplate,
                    TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                ErrorDialog.ShowDialog(ParentForm, TextRecources.ItemHandlingSupport_AddTemplateFailed, ex);
            }
        }
        public void AddItemToTemplateDefinition(EcfBlock sourceItem)
        {
            try
            {
                if (!AddItemToDefinition_TryGetFiles(TextRecources.ItemHandlingSupport_NoTemplateDefinitionFileFound,
                    TitleRecources.ItemHandlingSupport_AddToTemplateDefinitionOptionSelector,
                    out List<FormatDefinition> templateDefinitions, new Func<FormatDefinition, bool>(def => def.IsDefiningTemplates)))
                {
                    return;
                }

                ItemDefinition newParameter = new ItemDefinition(sourceItem.GetName(),
                    UserSettings.Default.ItemHandlingSupport_DefVal_Ingredient_DefIsOptional,
                    UserSettings.Default.ItemHandlingSupport_DefVal_Ingredient_DefHasValue,
                    UserSettings.Default.ItemHandlingSupport_DefVal_Ingredient_DefIsAllowingBlank,
                    UserSettings.Default.ItemHandlingSupport_DefVal_Ingredient_DefIsForceEscaped,
                    UserSettings.Default.ItemHandlingSupport_DefVal_Ingredient_DefInfo);
                AddItemToDefinition_SaveToFiles(templateDefinitions,
                    out List<FormatDefinition> modifiedDefinitions, out List<FormatDefinition> unmodifiedDefinitions,
                    newParameter);

                AddItemToDefinition_ReloadDefinitions(modifiedDefinitions, TextRecources.ItemHandlingSupport_UpdateTemplateFileDefinitionsQuestion,
                    new Func<EcfTabPage, bool>(page => page.File.Definition.IsDefiningTemplates));

                AddItemToDefinition_ShowReport(newParameter, modifiedDefinitions, unmodifiedDefinitions);
            }
            catch (Exception ex)
            {
                ErrorDialog.ShowDialog(ParentForm, TextRecources.ItemHandlingSupport_AddToTemplateDefinitionFailed, ex);
            }
        }
        public void AddItemToGlobalDefinition(EcfParameter sourceItem)
        {
            try
            {
                if (!AddItemToDefinition_TryGetFiles(TextRecources.ItemHandlingSupport_NoGlobalDefinitionFileFound,
                    TitleRecources.ItemHandlingSupport_AddToGlobalDefinitionOptionSelector,
                    out List<FormatDefinition> globalDefinitions, new Func<FormatDefinition, bool>(def => def.IsDefiningGlobalMacros)))
                {
                    return;
                }

                ItemDefinition newParameter = new ItemDefinition(sourceItem.Key,
                    UserSettings.Default.ItemHandlingSupport_DefVal_GlobalParam_DefIsOptional,
                    UserSettings.Default.ItemHandlingSupport_DefVal_GlobalParam_DefHasValue,
                    UserSettings.Default.ItemHandlingSupport_DefVal_GlobalParam_DefIsAllowingBlank,
                    UserSettings.Default.ItemHandlingSupport_DefVal_GlobalParam_DefIsForceEscaped,
                    UserSettings.Default.ItemHandlingSupport_DefVal_GlobalParam_DefInfo);
                ItemDefinition[] newAttributes = sourceItem.Attributes.Select(attribute =>
                {
                    return new ItemDefinition(attribute.Key,
                        UserSettings.Default.ItemHandlingSupport_DefVal_GlobalAttr_DefIsOptional,
                        UserSettings.Default.ItemHandlingSupport_DefVal_GlobalAttr_DefHasValue,
                        UserSettings.Default.ItemHandlingSupport_DefVal_GlobalAttr_DefIsAllowingBlank,
                        UserSettings.Default.ItemHandlingSupport_DefVal_GlobalAttr_DefIsForceEscaped,
                        UserSettings.Default.ItemHandlingSupport_DefVal_GlobalAttr_DefInfo);
                }).ToArray();

                AddItemToDefinition_SaveToFiles(globalDefinitions,
                    out List<FormatDefinition> modifiedDefinitions, out List<FormatDefinition> unmodifiedDefinitions,
                    newParameter, newAttributes);

                AddItemToDefinition_ReloadDefinitions(modifiedDefinitions, TextRecources.ItemHandlingSupport_UpdateGlobalDefFileDefinitionsQuestion,
                    new Func<EcfTabPage, bool>(page => page.File.Definition.IsDefiningGlobalMacros));

                AddItemToDefinition_ShowReport(newParameter, modifiedDefinitions, unmodifiedDefinitions);
            }
            catch (Exception ex)
            {
                ErrorDialog.ShowDialog(ParentForm, TextRecources.ItemHandlingSupport_AddToTemplateDefinitionFailed, ex);
            }
        }
        public void RemoveTemplateFromItem(EcfBlock sourceItem)
        {
            try
            {
                string[] parameterKeys = UserSettings.Default.ItemHandlingSupport_ParamKey_TemplateName.ToSeperated<string>().ToArray();

                if (!RemoveDependencyFromItem_TryFindTargetItems(new Func<EgsEcfFile, bool>(file => file.Definition.IsDefiningTemplates),
                    sourceItem, true, TextRecources.ItemHandlingSupport_NoTemplatesForItem, out List<EcfBlock> templateList, parameterKeys)) { return; }

                if (!RemoveDependencyFromItem_TryGetTargetItem(templateList, sourceItem, 
                    TextRecources.ItemHandlingSupport_AllTemplatesForItem, out EcfBlock templateToRemove)) { return; }

                if (RemoveDependencyFromItem_OnlyRemoveLink(sourceItem, templateToRemove, TitleRecources.ItemHandlingSupport_RemoveTemplateOptionSelector,
                    out List <EcfParameter> templateParameters, parameterKeys)) { return; }

                if (RemoveDependencyFromItem_CrossUsageCheck(new Func<EgsEcfFile, bool>(file => file.Definition.IsDefiningItems), 
                    templateToRemove, true, true, out List <EcfBlock> userList, parameterKeys)) { return; }

                RemoveDependencyFromItem_DeleteItem(templateToRemove, userList, templateParameters);
                ParentForm.GetTabPage(sourceItem.EcfFile)?.UpdateAllViews();
                RemoveDependencyFromItem_ShowReport(templateToRemove);
            }
            catch (Exception ex)
            {
                ErrorDialog.ShowDialog(ParentForm, TextRecources.ItemHandlingSupport_RemoveTemplateFailed, ex);
            }
        }

        // privates for show
        private void ShowListingView(string searchTitle, string searchValue, List<EcfBlock> results)
        {
            EcfItemListingDialog view = new EcfItemListingDialog();
            view.ItemRowClicked += ItemListingDialog_ShowItem;
            view.Show(ParentForm, string.Format("{0}: {1}", searchTitle, searchValue), results);
        }
        private void ShowListingView(string searchTitle, string searchValue, List<EcfParameter> results)
        {
            EcfItemListingDialog view = new EcfItemListingDialog();
            view.ItemRowClicked += ItemListingDialog_ShowItem;
            view.Show(ParentForm, string.Format("{0}: {1}", searchTitle, searchValue), results);
        }
        private void ShowLinkedBlocks(List<EcfBlock> linkedBlocks, EcfBlock sourceItem, string noBlockMessage, string listTitle)
        {
            if (linkedBlocks.Count < 1)
            {
                MessageBox.Show(ParentForm, string.Format("{0}: {1}", noBlockMessage, sourceItem.BuildRootId()),
                    TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (linkedBlocks.Count == 1)
            {
                EcfStructureItem itemToShow = linkedBlocks.FirstOrDefault();
                ParentForm.GetTabPage(itemToShow.EcfFile)?.ShowSpecificItem(itemToShow);
            }
            else
            {
                EcfItemListingDialog blockView = new EcfItemListingDialog();
                blockView.ItemRowClicked += ItemListingDialog_ShowItem;
                blockView.Show(ParentForm, string.Format("{0}: {1}", listTitle, sourceItem.BuildRootId()), linkedBlocks);
            }
        }
        
        // privates for generic dependency handling
        private bool AddDependencyToItem_TryFindTargetFiles(Func<EgsEcfFile, bool> fileFilter, string nothingFoundText, out SelectorItem[] targetFiles)
        {
            targetFiles = ParentForm.GetOpenedFiles(fileFilter).Select(file => new SelectorItem(file, file.FileName)).ToArray();
            if (targetFiles.Length < 1)
            {
                MessageBox.Show(ParentForm, nothingFoundText, TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }
        private void AddDependencyToItem_PerformSelectedOption(string selectorTitle,
            Action selectExistingOperation, Action createNewAsCopyOperation, Action createNewAsEmptyOperation)
        {
            OptionsDialog.Text = selectorTitle;
            if (OptionsDialog.ShowDialog(ParentForm, AddDependencyOptionItems) != DialogResult.OK) { return; }
            switch ((AddDependencyOptions)OptionsDialog.SelectedOption.Item)
            {
                case AddDependencyOptions.SelectExisting: selectExistingOperation(); break;
                case AddDependencyOptions.CreateNewAsCopy: createNewAsCopyOperation(); break;
                case AddDependencyOptions.CreateNewAsEmpty: createNewAsEmptyOperation(); break;
                default: break;
            }
        }
        private bool AddDependencyToItem_TrySelectItem(Func<EgsEcfFile, bool> fileFilter, string selectorTitle, out EcfBlock selectedItem)
        {
            selectedItem = null;

            SelectorItem[] presentItems = ParentForm.GetOpenedFiles(fileFilter)
                .SelectMany(file => file.ItemList.Where(item => item is EcfBlock).Cast<EcfBlock>())
                .Select(template => new SelectorItem(template, template.BuildRootId())).ToArray();

            ItemsDialog.Text = selectorTitle;
            if (ItemsDialog.ShowDialog(ParentForm, presentItems) != DialogResult.OK)
            {
                return false;
            }
            selectedItem = ItemsDialog.SelectedItem.Item as EcfBlock;
            return true;
        }
        private EcfBlock AddDependencyToItem_CreateEmptyTemplate(EcfBlock sourceItem, SelectorItem[] presentFileItems)
        {
            EgsEcfFile templateFile = (EgsEcfFile)presentFileItems.FirstOrDefault().Item;

            EcfBlock itemToAddTemplate;
            if (templateFile.ItemList.FirstOrDefault(item => item is EcfBlock) is EcfBlock templateTemplate)
            {
                itemToAddTemplate = new EcfBlock(templateTemplate);
                itemToAddTemplate.ClearParameters();
                itemToAddTemplate.GetDeepChildList<EcfBlock>().ForEach(child => child.ClearParameters());
            }
            else
            {
                itemToAddTemplate = new EcfBlock(
                    templateFile.Definition.BlockTypePreMarks.FirstOrDefault().Value,
                    templateFile.Definition.RootBlockTypes.FirstOrDefault().Value,
                    templateFile.Definition.BlockTypePostMarks.FirstOrDefault().Value);
            }
            itemToAddTemplate.SetName(sourceItem.GetName());
            return itemToAddTemplate;
        }
        private bool AddDependencyToItem_TryEditAndInsertItem(EcfBlock itemToAdd, SelectorItem[] presentFileItems, string fileSelectorTitle)
        {
            EgsEcfFile targetFile;
            if (presentFileItems.Length > 1)
            {
                ItemsDialog.Text = fileSelectorTitle;
                if (ItemsDialog.ShowDialog(ParentForm, presentFileItems) != DialogResult.OK) { return false; }
                targetFile = (EgsEcfFile)ItemsDialog.SelectedItem.Item;
            }
            else
            {
                targetFile = (EgsEcfFile)presentFileItems.FirstOrDefault().Item;
            }
            if (EditItemDialog.ShowDialog(ParentForm, ParentForm.GetOpenedFiles(), targetFile, itemToAdd) != DialogResult.OK) { return false; }
            itemToAdd.Revalidate();
            targetFile.AddItem(itemToAdd);
            return true;
        }
        private void AddDependencyToItem_ShowReport(EcfBlock addedItem, EcfBlock targetItem)
        {
            string messageText = string.Format("{2} {0} {3} {4} {1}!", addedItem.BuildRootId(), targetItem.BuildRootId(),
                addedItem.DataType, TextRecources.Generic_AddedTo, TitleRecources.Generic_Item);
            MessageBox.Show(ParentForm, messageText, TitleRecources.Generic_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private bool RemoveDependencyFromItem_TryFindTargetItems(Func<EgsEcfFile, bool> fileFilter, EcfBlock sourceItem, bool withBlockNameCheck,
            string noItemMessage, out List<EcfBlock> targetItems, params string[] parameterKeys)
        {
            targetItems = GetBlockListByNameOrParamValue(ParentForm.GetOpenedFiles(fileFilter), withBlockNameCheck, sourceItem, parameterKeys);
            if (targetItems.Count() < 1)
            {
                string messageText = string.Format("{0}: {1}", noItemMessage, sourceItem.BuildRootId());
                MessageBox.Show(ParentForm, messageText, TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }
        private bool RemoveDependencyFromItem_TryGetTargetItem(List<EcfBlock> itemsToRemove, EcfBlock sourceItem,
            string itemSelectorTitle, out EcfBlock itemToRemove)
        {
            itemToRemove = null;
            if (itemsToRemove.Count() > 1)
            {
                EcfItemListingDialog templateSelector = new EcfItemListingDialog();
                string messageText = string.Format("{0}: {1}", itemSelectorTitle, sourceItem.BuildRootId());
                if (templateSelector.ShowDialog(ParentForm, messageText, itemsToRemove) != DialogResult.OK)
                {
                    return false;
                }
                itemToRemove = templateSelector.SelectedStructureItem as EcfBlock;
            }
            else
            {
                itemToRemove = itemsToRemove.FirstOrDefault();
            }
            return true;
        }
        private bool RemoveDependencyFromItem_OnlyRemoveLink(EcfBlock sourceItem, EcfBlock itemToRemove, string selectorTitle,
            out List<EcfParameter> parametersWithItem, params string[] parameterKeys)
        {
            string itemToRemoveName = itemToRemove.GetName();
            parametersWithItem = new List<EcfParameter>();
            foreach (string key in parameterKeys)
            {
                if (sourceItem.HasParameter(key, out EcfParameter parameter) && parameter.ContainsValue(itemToRemoveName))
                {
                    parametersWithItem.Add(parameter);
                }
            }

            if (parametersWithItem.Count > 0)
            {
                OptionsDialog.Text = selectorTitle;
                if (OptionsDialog.ShowDialog(ParentForm, RemoveDependencyOptionItems) != DialogResult.OK) { return true; }
                if ((RemoveDependencyOptions)OptionsDialog.SelectedOption.Item == RemoveDependencyOptions.RemoveLinkToItemOnly)
                {
                    foreach (EcfParameter parameter in parametersWithItem)
                    {
                        parameter.ClearValues();
                        parameter.AddValue(string.Empty);
                    }

                    string messageText = string.Format("{2} {0} {3} {4} {1}!", itemToRemove.GetName(), sourceItem.GetName(),
                        itemToRemove.DataType, TextRecources.Generic_RemovedFrom, TitleRecources.Generic_Item);
                    MessageBox.Show(ParentForm, messageText, TitleRecources.Generic_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
            }
            return false;
        }
        private bool RemoveDependencyFromItem_CrossUsageCheck(Func<EgsEcfFile, bool> fileFilter, EcfBlock itemToRemove, bool withBlockNameCheck, bool withInheritedParams,
            out List<EcfBlock> userList, params string[] parameterKeys)
        {
            userList = GetBlockListByParameterValue(ParentForm.GetOpenedFiles(fileFilter), withBlockNameCheck, withInheritedParams, itemToRemove.GetName(), parameterKeys);
            if (userList.Count() > 1)
            {
                List<string> errors = userList.Select(user => string.Format("{0} {1}: {2}", itemToRemove.DataType,
                    TextRecources.ItemHandlingSupport_StillUsedWith, user.BuildRootId())).ToList();
                if (ErrorDialog.ShowDialog(ParentForm, TextRecources.Generic_ContinueOperationWithErrorsQuestion, errors) != DialogResult.Yes)
                {
                    return true;
                }
            }
            return false;
        }
        private void RemoveDependencyFromItem_DeleteItem(EcfBlock itemToRemove, List<EcfBlock> userList, List<EcfParameter> parametersWithItem)
        {
            itemToRemove.EcfFile.RemoveItem(itemToRemove);
            userList.ForEach(user =>
            {
                parametersWithItem.ForEach(parameter =>
                {
                    EcfParameter parameterToRemove = user.FindOrCreateParameter(parameter.Key);
                    parameterToRemove.ClearValues();
                    parameterToRemove.AddValue(string.Empty);
                });
            });
        }
        private void RemoveDependencyFromItem_ShowReport(EcfBlock itemToRemove)
        {
            string messageText = string.Format("{2} {0} {3} {4} {1}!", itemToRemove.GetName(), itemToRemove.EcfFile.FileName,
                    itemToRemove.DataType, TextRecources.Generic_RemovedFrom, TitleRecources.Generic_File);
            MessageBox.Show(ParentForm, messageText, TitleRecources.Generic_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // privatey for definition handling
        private bool AddItemToDefinition_TryGetFiles(string noFileMessage, string optionSelectorTitle,
            out List<FormatDefinition> definitions, Func<FormatDefinition, bool> filter = null)
        {
            definitions = GetSupportedFileTypes(UserSettings.Default.EgsEcfEditorApp_ActiveGameMode).Where(filter ?? (result => true)).ToList();
            if (definitions.Count < 1)
            {
                MessageBox.Show(ParentForm, noFileMessage, TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            if (definitions.Count > 1)
            {
                OptionsDialog.Text = optionSelectorTitle;
                if (OptionsDialog.ShowDialog(ParentForm, AddToDefinitionOptionItems) != DialogResult.OK) { return false; }
                switch ((AddToDefinitionOptions)OptionsDialog.SelectedOption.Item)
                {
                    case AddToDefinitionOptions.AllDefinitions: break;
                    case AddToDefinitionOptions.SelectDefinition:
                        if (ItemsDialog.ShowDialog(ParentForm, definitions.Select(def => new SelectorItem(def, def.FilePathAndName))
                            .ToArray()) != DialogResult.OK) { return false; }
                        definitions.Clear();
                        definitions.Add((FormatDefinition)ItemsDialog.SelectedItem.Item);
                        break;
                    default: break;
                }
            }
            return true;
        }
        private void AddItemToDefinition_SaveToFiles(List<FormatDefinition> definitions,
            out List<FormatDefinition> modifiedDefinitions, out List<FormatDefinition> unmodifiedDefinitions,
            ItemDefinition parameter, params ItemDefinition[] attributes)
        {
            modifiedDefinitions = new List<FormatDefinition>();
            unmodifiedDefinitions = new List<FormatDefinition>();
            foreach (FormatDefinition definition in definitions)
            {
                bool isModified = SaveItemToDefinitionFile(definition, ChangeableDefinitionChapters.BlockParameters, parameter);
                foreach (ItemDefinition attribute in attributes ?? Enumerable.Empty<ItemDefinition>())
                {
                    if (SaveItemToDefinitionFile(definition, ChangeableDefinitionChapters.ParameterAttributes, attribute))
                    {
                        isModified = true;
                    }
                }
                if (isModified)
                {
                    modifiedDefinitions.Add(definition);
                }
                else
                {
                    unmodifiedDefinitions.Add(definition);
                }
            }
        }
        private void AddItemToDefinition_ReloadDefinitions(List<FormatDefinition> definitions, string openedPageUpdateQuestion, Func<EcfTabPage, bool> ecfPageFilter = null)
        {
            if (definitions.Count > 0)
            {
                ReloadDefinitions();

                List<EcfTabPage> fileTabsToUpdate = ParentForm.GetTabPages(ecfPageFilter);
                if (fileTabsToUpdate.Count > 0)
                {
                    if (MessageBox.Show(ParentForm, openedPageUpdateQuestion, TitleRecources.Generic_Attention,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        foreach (EcfTabPage filePage in fileTabsToUpdate)
                        {
                            ParentForm.ReplaceDefinitionInFile(filePage.File);
                            filePage.UpdateDefinitionPresets();
                            filePage.UpdateAllViews();
                        }
                    }
                }
            }
        }
        private void AddItemToDefinition_ShowReport(ItemDefinition newParameter, List<FormatDefinition> modifiedDefinitions, List<FormatDefinition> unmodifiedDefinitions)
        {
            StringBuilder messageText = new StringBuilder();
            messageText.AppendLine(string.Format("{0} {1}", TitleRecources.Generic_Parameter, newParameter.Name));
            if (modifiedDefinitions.Count > 0)
            {
                messageText.AppendLine();
                messageText.AppendLine(string.Format("{0}:", TextRecources.Generic_AddedTo));
                messageText.AppendLine(string.Join(Environment.NewLine, modifiedDefinitions.Select(def => def.FilePathAndName)));
            }
            if (unmodifiedDefinitions.Count > 0)
            {
                messageText.AppendLine();
                messageText.AppendLine(string.Format("{0}:", TextRecources.Generic_IsAlreadyPresentIn));
                messageText.AppendLine(string.Join(Environment.NewLine, unmodifiedDefinitions.Select(def => def.FilePathAndName)));
            }
            MessageBox.Show(ParentForm, messageText.ToString(), TitleRecources.Generic_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // privates for specific dependency handling
        private void AddTemplateToItem_SelectFromExisting(EcfBlock sourceItem)
        {
            if (!AddDependencyToItem_TrySelectItem(new Func<EgsEcfFile, bool>(file => file.Definition.IsDefiningTemplates), 
                TitleRecources.ItemHandlingSupport_AddExistingTemplateSelector, out EcfBlock templateToAdd)) { return; }

            AddTemplateToItem_UpdateTemplateRoot(templateToAdd, sourceItem);
            AddDependencyToItem_ShowReport(templateToAdd, sourceItem);
            ParentForm.GetTabPage(sourceItem.EcfFile)?.UpdateAllViews();
        }
        private void AddTemplateToItem_CreateCopy(EcfBlock sourceItem, SelectorItem[] presentTemplateFiles)
        {
            if (!AddDependencyToItem_TrySelectItem(new Func<EgsEcfFile, bool>(file => file.Definition.IsDefiningTemplates),
                TitleRecources.ItemHandlingSupport_CreateFromCopyTemplateSelector, out EcfBlock templateToCopy)) { return; }

            EcfBlock templateToAdd = new EcfBlock(templateToCopy);
            templateToAdd.SetName(sourceItem.GetName());
            if (!AddDependencyToItem_TryEditAndInsertItem(templateToAdd, presentTemplateFiles, 
                TitleRecources.ItemHandlingSupport_TargetTemplateFileSelector)) { return; }

            AddTemplateToItem_UpdateTemplateRoot(templateToAdd, sourceItem);
            AddDependencyToItem_ShowReport(templateToAdd, sourceItem);
            ParentForm.GetTabPage(sourceItem.EcfFile)?.UpdateAllViews();
        }
        private void AddTemplateToItem_CreateNew(EcfBlock sourceItem, SelectorItem[] presentTemplateFiles)
        {
            EcfBlock templateToAdd = AddDependencyToItem_CreateEmptyTemplate(sourceItem, presentTemplateFiles);

            if (!AddDependencyToItem_TryEditAndInsertItem(templateToAdd, presentTemplateFiles, 
                TitleRecources.ItemHandlingSupport_TargetTemplateFileSelector)) { return; }

            AddTemplateToItem_UpdateTemplateRoot(templateToAdd, sourceItem);
            AddDependencyToItem_ShowReport(templateToAdd, sourceItem);
            ParentForm.GetTabPage(sourceItem.EcfFile)?.UpdateAllViews();
        }
        private void AddTemplateToItem_UpdateTemplateRoot(EcfBlock templateToAdd, EcfBlock targetItem)
        {
            EcfParameter templateParameter = targetItem.FindOrCreateParameter(UserSettings.Default.ItemHandlingSupport_ParamKey_TemplateName);
            templateParameter.ClearValues();
            string templateName = templateToAdd.GetName();
            templateParameter.AddValue(string.Equals(templateName, targetItem.GetName()) ? string.Empty : templateName);
        }
    }
}
