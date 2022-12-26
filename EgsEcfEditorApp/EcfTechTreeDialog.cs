﻿using EcfFileViews;
using EcfToolBarControls;
using EcfWinFormControls;
using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EgsEcfEditorApp
{
    public partial class EcfTechTreeDialog : Form
    {
        public string ReferenceNameAttributeKey { get; private set; } = null;
        public string UnlockLevelParameterKey { get; private set; } = null;
        public string UnlockCostParameterKey { get; private set; } = null;
        public string TechTreeNamesParameterKey { get; private set; } = null;
        public string TechTreeParentParameterKey { get; private set; } = null;
        
        public HashSet<EcfTabPage> ChangedFileTabs { get; } = new HashSet<EcfTabPage>();
        protected List<EcfTabPage> UniqueFileTabs { get; } = new List<EcfTabPage>();
        
        private EcfItemSelectorDialog FileTabSelector { get; } = new EcfItemSelectorDialog();
        private TreeAlteratingTools TreeTools { get; } = new TreeAlteratingTools();
        private EcfTextInputDialog TreeNameSelector { get; } = new EcfTextInputDialog(TitleRecources.EcfTechTreeDialog_TreeNameInputHeader);
        protected EcfTechTreeItemEditorDialog TreeItemEditor { get; } = new EcfTechTreeItemEditorDialog();
        protected ContextMenuStrip TechTreeOperationMenu { get; } = new ContextMenuStrip();
        private ToolStripMenuItem NodeChangeItem { get; } = new ToolStripMenuItem(TitleRecources.Generic_Change, IconRecources.Icon_ChangeSimple);
        private ToolStripMenuItem NodeAddItem { get; } = new ToolStripMenuItem(TitleRecources.Generic_Add, IconRecources.Icon_Add);
        private ToolStripMenuItem NodeCopyItem { get; } = new ToolStripMenuItem(TitleRecources.Generic_Copying, IconRecources.Icon_Copy);
        private ToolStripMenuItem NodePasteItem { get; } = new ToolStripMenuItem(TitleRecources.Generic_Paste, IconRecources.Icon_Paste);
        private ToolStripMenuItem NodeRemoveItem { get; } = new ToolStripMenuItem(TitleRecources.Generic_Remove, IconRecources.Icon_Remove);

        protected List<EcfBlock> AvailableElements { get; } = new List<EcfBlock>();
        protected ElementNode LastCopiedElement { get; set; } = null;
        private EcfTechTree LastCopiedTree { get; set; } = null;

        public EcfTechTreeDialog()
        {
            InitializeComponent();
            InitForm();
        }

        // events
        private void InitForm()
        {
            Icon = IconRecources.Icon_AppBranding;
            Text = TitleRecources.EcfTechTreeDialog_Header;

            UnattachedElementsGroupBox.Text = TitleRecources.EcfTechTreeDialog_UnattachedElementsHeader;

            ToolContainer.Add(TreeTools);

            TreeTools.AddTreeClicked += TreeTools_AddTreeClicked;
            TreeTools.RemoveTreeClicked += TreeTools_RemoveTreeClicked;
            TreeTools.RenameTreeClicked += TreeTools_RenameTreeClicked;
            TreeTools.CopyTreeClicked += TreeTools_CopyTreeClicked;
            TreeTools.PasteTreeClicked += TreeTools_PasteTreeClicked;

            NodeChangeItem.Click += NodeChangeItem_Click;
            NodeAddItem.Click += NodeAddItem_Click;
            NodeCopyItem.Click += NodeCopyItem_Click;
            NodePasteItem.Click += NodePasteItem_Click;
            NodeRemoveItem.Click += NodeRemoveItem_Click;

            TechTreeOperationMenu.Items.Add(NodeChangeItem);
            TechTreeOperationMenu.Items.Add(NodeAddItem);
            TechTreeOperationMenu.Items.Add(NodeCopyItem);
            TechTreeOperationMenu.Items.Add(NodePasteItem);
            TechTreeOperationMenu.Items.Add(NodeRemoveItem);
        }

        // events
        private void UnattachedElementsTreeView_ItemDrag(object sender, ItemDragEventArgs evt)
        {
            if (evt.Item is ElementNode node)
            {
                int index = node.Index;
                UnattachedElementsTreeView.Nodes.Remove(node);
                if (DoDragDrop(node, DragDropEffects.Move) != DragDropEffects.Move)
                {
                    UnattachedElementsTreeView.Nodes.Insert(index, node);
                }
            }
        }
        private void TreeTools_AddTreeClicked(object sender, EventArgs evt)
        {
            string treeName = PromptTreeNameEdit(TextRecources.EcfTechTreeDialog_NewTreeName, null);
            if (treeName != null)
            {
                EcfTechTree newTree = new EcfTechTree(this, treeName);
                TechTreePageContainer.TabPages.Add(newTree);
                TechTreePageContainer.SelectedTab = newTree;
            }
        }
        private void TreeTools_RemoveTreeClicked(object sender, EventArgs evt)
        {
            if (TechTreePageContainer.SelectedTab is EcfTechTree tree) 
            {
                if (MessageBox.Show(this, string.Format("{0}{1}{1}{2}", TextRecources.EcfTechTreeDialog_ReallyRemoveTechTreeQuestion, Environment.NewLine, tree.TreeName),
                    TitleRecources.Generic_Attention, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    tree.Clear();
                    TechTreePageContainer.TabPages.Remove(tree);
                }
            }
        }
        private void TreeTools_RenameTreeClicked(object sender, EventArgs evt)
        {
            if (TechTreePageContainer.SelectedTab is EcfTechTree selectedTree)
            {
                string treeName = PromptTreeNameEdit(selectedTree.TreeName, selectedTree);
                if (treeName != null)
                {
                    selectedTree.SetTreeName(treeName);
                }
            }
        }
        private void TreeTools_CopyTreeClicked(object sender, EventArgs evt)
        {
            if (TechTreePageContainer.SelectedTab is EcfTechTree tree)
            {
                LastCopiedTree = new EcfTechTree(tree);
            }
        }
        private void TreeTools_PasteTreeClicked(object sender, EventArgs evt)
        {
            if (LastCopiedTree != null)
            {
                string treeName = PromptTreeNameEdit(string.Format("{0} - {1}", LastCopiedTree.TreeName, TitleRecources.Generic_Copy), null);
                if (treeName != null)
                {
                    EcfTechTree copiedTree = new EcfTechTree(LastCopiedTree);
                    copiedTree.SetTreeName(treeName);
                    TechTreePageContainer.TabPages.Add(copiedTree);
                }
            }
        }
        private void NodeChangeItem_Click(object sender, EventArgs evt)
        {
            if (TechTreePageContainer.SelectedTab is EcfTechTree tree)
            {
                tree.ChangeSelectedNode();
            }
        }
        private void NodeAddItem_Click(object sender, EventArgs evt)
        {
            if (TechTreePageContainer.SelectedTab is EcfTechTree tree)
            {
                tree.AddNodeToSelectedNode();
            }
        }
        private void NodeCopyItem_Click(object sender, EventArgs evt)
        {
            if (TechTreePageContainer.SelectedTab is EcfTechTree tree)
            {
                tree.CopySelectedNode();
            }
        }
        private void NodePasteItem_Click(object sender, EventArgs evt)
        {
            if (TechTreePageContainer.SelectedTab is EcfTechTree tree)
            {
                tree.PasteNodeToSelectedNode();
            }
        }
        private void NodeRemoveItem_Click(object sender, EventArgs evt)
        {
            if (TechTreePageContainer.SelectedTab is EcfTechTree tree)
            {
                tree.RemoveSelectedNode();
            }
        }

        // public
        public DialogResult ShowDialog(IWin32Window parent, List<EcfTabPage> openedFileTabs)
        {
            ReferenceNameAttributeKey = UserSettings.Default.EcfTechTreeDialog_AttributeKey_ReferenceName;
            UnlockLevelParameterKey = UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockLevel;
            UnlockCostParameterKey = UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockCost;
            TechTreeNamesParameterKey = UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeNames;
            TechTreeParentParameterKey = UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeParentName;

            ChangedFileTabs.Clear();

            DialogResult result = UpdateUniqueFileTabs(openedFileTabs);
            if (result != DialogResult.OK) { return result; }
            UpdateTechTrees();
            return ShowDialog(parent);
        }
        public void HideNodeSpecificMenuStripItems(bool hide)
        {
            NodeChangeItem.Visible = !hide;
            NodeCopyItem.Visible = !hide;
            NodeRemoveItem.Visible = !hide;
        }

        // private
        private string PromptTreeNameEdit(string treeName, EcfTechTree editedTree)
        {
            bool treeNameValid = false;
            DialogResult result = DialogResult.OK;
            while (result == DialogResult.OK && !treeNameValid)
            {
                result = TreeNameSelector.ShowDialog(this, treeName);
                treeName = TreeNameSelector.GetText();
                treeNameValid = !TechTreePageContainer.TabPages.Cast<EcfTechTree>().Where(tree => tree != editedTree).Any(tree => tree.TreeName.Equals(treeName));
                if (result == DialogResult.OK && !treeNameValid)
                {
                    MessageBox.Show(this, TextRecources.EcfTechTreeDialog_TechTreeNameAlreadyUsed,
                        TitleRecources.Generic_Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            if (result != DialogResult.OK)
            {
                treeName = null;
            }
            return treeName;
        }
        private DialogResult UpdateUniqueFileTabs(List<EcfTabPage> openedFileTabs)
        {
            UniqueFileTabs.Clear();
            AvailableElements.Clear();
            foreach (EcfTabPage openedTab in openedFileTabs)
            {
                string openedTabFileType = openedTab.File.Definition.FileType;
                if (!UniqueFileTabs.Any(uniqueTab => uniqueTab.File.Definition.FileType.Equals(openedTabFileType)))
                {
                    List<EcfTabPage> typeSpecificFileTabs = openedFileTabs.Where(tab => tab.File.Definition.FileType.Equals(openedTabFileType)).ToList();
                    if (typeSpecificFileTabs.Count > 1)
                    {
                        string header = string.Format("{0}: {1}", TextRecources.EcfTechTreeDialog_SelectFileForType, openedTabFileType);
                        DialogResult result = FileTabSelector.ShowDialog(this, header, typeSpecificFileTabs.ToArray());
                        if (result != DialogResult.OK) { return result; }
                        if (FileTabSelector.SelectedItem is EcfTabPage selectedPage) { UniqueFileTabs.Add(selectedPage); }
                    }
                    else
                    {
                        UniqueFileTabs.Add(typeSpecificFileTabs.FirstOrDefault());
                    }
                }
            }
            AvailableElements.AddRange(UniqueFileTabs.SelectMany(tab => tab.File.ItemList.Where(item => item is EcfBlock).Cast<EcfBlock>()));
            return DialogResult.OK;
        }
        [Obsolete("has test data.")]
        private void UpdateTechTrees()
        {
            TechTreePageContainer.SuspendLayout();
            TechTreePageContainer.TabPages.Clear();
            UnattachedElementsTreeView.BeginUpdate();
            UnattachedElementsTreeView.Nodes.Clear();

            UniqueFileTabs.ForEach(tab =>
            {
                foreach(EcfBlock block in tab.File.ItemList.Where(item => item is EcfBlock))
                {
                    block.HasParameter(TechTreeNamesParameterKey, true, out EcfParameter techTreeNames);

                    string elementName = block.GetAttributeFirstValue(ReferenceNameAttributeKey);
                    string techTreeParent = block.GetParameterFirstValue(TechTreeParentParameterKey, true);
                    string unlockLevel = block.GetParameterFirstValue(UnlockLevelParameterKey, true);
                    string unlockCost = block.GetParameterFirstValue(UnlockCostParameterKey, true);

                    if (!int.TryParse(unlockLevel, out int unlockLevelValue)) {
                        unlockLevelValue = UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockLevel;
                    }
                    if (!int.TryParse(unlockCost, out int unlockCostValue))
                    {
                        unlockCostValue = UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockCost;
                    }

                    if (techTreeNames != null)
                    {
                        foreach (string treeName in techTreeNames.GetAllValues())
                        {
                            EcfTechTree treePage = TechTreePageContainer.TabPages.Cast<EcfTechTree>().FirstOrDefault(tree => tree.Text.Equals(treeName));
                            
                            if (treePage == null)
                            {
                                treePage = new EcfTechTree(this, treeName);
                                TechTreePageContainer.TabPages.Add(treePage);
                            }

                            treePage.Add(new ElementNode(tab, block, elementName, techTreeParent, unlockLevelValue, unlockCostValue));
                        }
                    }
                    else if (unlockLevel != null || unlockCost != null || techTreeParent != null)
                    {
                        UnattachedElementsTreeView.Nodes.Add(new ElementNode(tab, block, elementName, techTreeParent, unlockLevelValue, unlockCostValue));
                    }
                }
            });



            EcfTabPage testtab = UniqueFileTabs.FirstOrDefault();
            EcfBlock element = testtab?.File.GetDeepItemList<EcfBlock>().FirstOrDefault();
            ElementNode node1 = new ElementNode(testtab, element, "inge", "horst", 0, 258);
            ElementNode node2 = new ElementNode(testtab, element, "gerda", "kunibert", 0, 12);
            UnattachedElementsTreeView.Nodes.Add(node1);
            UnattachedElementsTreeView.Nodes.Add(node2);
            


            UnattachedElementsGroupBox.Visible = UnattachedElementsTreeView.Nodes.Count != 0;
            UnattachedElementsTreeView.EndUpdate();
            TechTreePageContainer.ResumeLayout();
        }

        // subclasses
        private class EcfTechTree : TabPage
        {
            public string TreeName { get; private set; }
            private EcfTreeView ElementTreeView { get; } = new EcfTreeView();
            private EcfTechTreeDialog ParentForm { get; }
            private List<EcfBlock> AvailableElements { get; } = new List<EcfBlock>();

            public EcfTechTree(EcfTechTreeDialog parentForm, string name)
            {
                ParentForm = parentForm;
                SetTreeName(name);

                ElementTreeView.AllowDrop = true;
                ElementTreeView.Dock = DockStyle.Fill;
                ElementTreeView.ShowNodeToolTips = true;

                ElementTreeView.DragEnter += ElementTreeView_DragEnter;
                ElementTreeView.DragDrop += ElementTreeView_DragDrop;
                ElementTreeView.ItemDrag += ElementTreeView_ItemDrag;
                ElementTreeView.KeyUp += ElementTreeView_KeyUp;
                ElementTreeView.KeyPress += ElementTreeView_KeyPress;
                ElementTreeView.MouseUp += ElementTreeView_MouseUp;
                ElementTreeView.NodeMouseClick += ElementTreeView_NodeMouseClick;
                ElementTreeView.NodeMouseDoubleClick += ElementTreeView_NodeMouseDoubleClick;

                Controls.Add(ElementTreeView);
            }
            public EcfTechTree(EcfTechTree template) : this(template.ParentForm, template.TreeName)
            {
                ElementTreeView.Nodes.AddRange(template.ElementTreeView.Nodes.Cast<ElementNode>().Select(node => new ElementNode(node)).ToArray());
            }

            // events
            private void ElementTreeView_DragEnter(object sender, DragEventArgs evt)
            {
                // specifies the acceptance of the drop operation
                evt.Effect = DragDropEffects.Move;
            }
            private void ElementTreeView_DragDrop(object sender, DragEventArgs evt)
            {
                ElementNode sourceNode = (ElementNode)evt.Data.GetData(typeof(ElementNode));
                CursorAlignment alignment = GetTreeCursorAlignment(evt.X, evt.Y, out ElementNode targetNode);
                int? index = null;
                switch (alignment)
                {
                    case CursorAlignment.LowerEdge:
                        index = targetNode?.Index + 1;
                        targetNode = targetNode.Parent as ElementNode;
                        break;
                    case CursorAlignment.UpperEdge:
                        index = targetNode?.Index;
                        targetNode = targetNode.Parent as ElementNode;
                        break;
                    default: break;
                }
                if (TryInsertNode(targetNode, index, sourceNode))
                {
                    evt.Effect = DragDropEffects.Move;
                }
                else
                {
                    evt.Effect = DragDropEffects.None;
                }
            }
            private void ElementTreeView_ItemDrag(object sender, ItemDragEventArgs evt)
            {
                if (evt.Item is ElementNode node)
                {
                    int index = node.Index;
                    TreeNodeCollection nodes = node.Parent != null ? node.Parent.Nodes : ElementTreeView.Nodes;
                    nodes.Remove(node);
                    if (DoDragDrop(node, DragDropEffects.Move) != DragDropEffects.Move)
                    {
                        nodes.Insert(index, node);
                    }
                }
            }
            private void ElementTreeView_KeyUp(object sender, KeyEventArgs evt)
            {
                if (evt.KeyCode == Keys.Delete) { RemoveNode(ElementTreeView.SelectedNode); evt.Handled = true; }
                else if (evt.Control && evt.KeyCode == Keys.C) { CopySelectedNode(); evt.Handled = true; }
                else if (evt.Control && evt.KeyCode == Keys.V) { PasteNodeToSelectedNode(); evt.Handled = true; }
            }
            private void ElementTreeView_KeyPress(object sender, KeyPressEventArgs evt)
            {
                // hack for sqirky "ding"
                evt.Handled = true;
            }
            private void ElementTreeView_MouseUp(object sender, MouseEventArgs evt)
            {
                if (ElementTreeView.Nodes.Count < 1 && evt.Button == MouseButtons.Right)
                {
                    ParentForm.HideNodeSpecificMenuStripItems(true);
                    ParentForm.TechTreeOperationMenu.Show(this, evt.Location);
                }
            }
            private void ElementTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs evt)
            {
                ElementTreeView.SelectedNode = evt.Node;
                if (evt.Button == MouseButtons.Right)
                {
                    ParentForm.HideNodeSpecificMenuStripItems(false);
                    ParentForm.TechTreeOperationMenu.Show(this, evt.Location);
                }
            }
            private void ElementTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs evt)
            {
                ChangeSelectedNode();
            }

            // publics
            public void Add(ElementNode elementNode)
            {
                Add(elementNode, null);
            }
            public void Add(ElementNode elementNode, int? index)
            {
                ElementTreeView.BeginUpdate();

                ElementTreeView.Nodes.Cast<ElementNode>().ToList().ForEach(rootElement =>
                {
                    if (!string.IsNullOrEmpty(rootElement.TechTreeParentName) && string.Equals(rootElement.TechTreeParentName, elementNode.ElementName))
                    {
                        ElementTreeView.Nodes.Remove(rootElement);
                        elementNode.Nodes.Add(rootElement);
                    }
                });

                ElementNode parent = FindParentNode(elementNode.TechTreeParentName, ElementTreeView.Nodes);
                if (parent != null)
                {
                    parent.Nodes.Insert(index ?? parent.Nodes.Count, elementNode);
                }
                else
                {
                    ElementTreeView.Nodes.Insert(index ?? ElementTreeView.Nodes.Count, elementNode);
                }

                ElementTreeView.EndUpdate();
            }
            public void Clear()
            {
                ElementTreeView.Nodes.Cast<ElementNode>().ToList().ForEach(node => RemoveNode(node));
            }
            public void SetTreeName(string name)
            {
                TreeName = name ?? string.Empty;
                Text = TreeName;
                try
                {
                    UpdateTechTreeNames(ElementTreeView.Nodes);
                }
                catch (Exception ex)
                {
                    ShowUpdateErrorMessage(ex.Message);
                }
            }
            [Obsolete("needs work")]
            public void ChangeSelectedNode()
            {
                

                // cross tree view update?
                // daten in altem element zurücksetzen?


                if (ElementTreeView.SelectedNode is ElementNode sourceNode)
                {
                    EcfBlock sourceElement = sourceNode.Element;
                    int unlockLevel = sourceNode.UnlockLevel;
                    int unlockCost = sourceNode.UnlockCost;

                    AvailableElements.Clear();
                    AvailableElements.AddRange(ParentForm.AvailableElements.Except(BuildElementList(ElementTreeView.Nodes)));
                    if (!AvailableElements.Contains(sourceElement)) { AvailableElements.Insert(0, sourceElement); }

                    if (ParentForm.TreeItemEditor.ShowDialog(this, sourceElement, AvailableElements, unlockLevel, unlockCost) == DialogResult.OK)
                    {
                        EcfBlock element = ParentForm.TreeItemEditor.GetSelectedElement();
                        EcfTabPage tab = ParentForm.UniqueFileTabs.FirstOrDefault(page => page.File.ItemList.Contains(element));
                        unlockLevel = ParentForm.TreeItemEditor.GetUnlockLevel();
                        unlockCost = ParentForm.TreeItemEditor.GetUnlockCost();

                        string elementName = element.GetAttributeFirstValue(ParentForm.ReferenceNameAttributeKey);
                        ElementNode newNode = new ElementNode(tab, element, elementName);
                        TrySetUnlockData(newNode, unlockLevel, unlockCost);

                        TryInsertNode(ElementTreeView.SelectedNode as ElementNode, null, newNode);
                    }
                }



            }
            [Obsolete("needs work")]
            public void AddNodeToSelectedNode()
            {
                int unlockLevel = UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockLevel;
                int unlockCost = UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockCost;

                AvailableElements.Clear();
                AvailableElements.AddRange(ParentForm.AvailableElements.Except(BuildElementList(ElementTreeView.Nodes)));

                if (ParentForm.TreeItemEditor.ShowDialog(this, null, AvailableElements, unlockLevel, unlockCost) == DialogResult.OK)
                {
                    EcfBlock element = ParentForm.TreeItemEditor.GetSelectedElement();
                    EcfTabPage tab = ParentForm.UniqueFileTabs.FirstOrDefault(page => page.File.ItemList.Contains(element));
                    unlockLevel = ParentForm.TreeItemEditor.GetUnlockLevel();
                    unlockCost = ParentForm.TreeItemEditor.GetUnlockCost();

                    string elementName = element.GetAttributeFirstValue(ParentForm.ReferenceNameAttributeKey);
                    ElementNode newNode = new ElementNode(tab, element, elementName);
                    TrySetUnlockData(newNode, unlockLevel, unlockCost);

                    TryInsertNode(ElementTreeView.SelectedNode as ElementNode, null, newNode);
                }
            }
            public void CopySelectedNode()
            {
                if (ElementTreeView.SelectedNode is ElementNode node)
                {
                    ParentForm.LastCopiedElement = new ElementNode(node);
                }
            }
            public void PasteNodeToSelectedNode()
            {
                ElementNode sourceNode = ParentForm.LastCopiedElement;
                if (sourceNode != null)
                {
                    ElementNode newNode = new ElementNode(sourceNode);
                    TryInsertNode(ElementTreeView.SelectedNode as ElementNode, null, newNode);
                }
            }
            public void RemoveSelectedNode()
            {
                RemoveNode(ElementTreeView.SelectedNode);
            }
            
            // privates
            private void ShowUpdateErrorMessage(string message)
            {
                MessageBox.Show(this, string.Format("{0}:{1}{1}{2}", TextRecources.EcfTechTreeDialog_ElementSettingsUpdateError, Environment.NewLine, message),
                    TitleRecources.Generic_Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            private ElementNode FindParentNode(string parentName, TreeNodeCollection nodes)
            {
                foreach (ElementNode node in nodes.Cast<ElementNode>())
                {
                    if  (string.Equals(parentName, node.ElementName))
                    {
                        return node;
                    }
                    else
                    {
                        ElementNode subNode = FindParentNode(parentName, node.Nodes);
                        if (subNode != null)
                        {
                            return subNode;
                        }
                    }
                }
                return null;
            }
            private CursorAlignment GetTreeCursorAlignment(int globalCursorX, int globalCursorY, out ElementNode node)
            {
                Point localCursorPosition = ElementTreeView.PointToClient(new Point(globalCursorX, globalCursorY));
                node = ElementTreeView.GetNodeAt(localCursorPosition) as ElementNode;
                if (node == null) { return CursorAlignment.None; }
                
                int borderThickness = node.Bounds.Height * 20 / 100;
                if (localCursorPosition.Y < node.Bounds.Y + borderThickness)
                {
                    return CursorAlignment.UpperEdge;
                }
                else if (localCursorPosition.Y > node.Bounds.Y + node.Bounds.Height - 2 * borderThickness)
                {
                    return CursorAlignment.LowerEdge;
                }
                else
                {
                    return CursorAlignment.Center;
                }
            }
            private bool TryInsertNode(ElementNode targetNode, int? targetIndex, ElementNode sourceNode)
            {
                if (sourceNode != null)
                {
                    List<EcfBlock> treeElements = BuildElementList(ElementTreeView.Nodes);
                    List<EcfBlock> insertingElements = BuildElementList(sourceNode);
                    List<EcfBlock> forbiddenElements = insertingElements.Where(element => treeElements.Contains(element)).ToList();
                    if (forbiddenElements.Count > 0)
                    {
                        MessageBox.Show(this,
                            string.Format("{0}:{1}{1}{2}", TextRecources.EcfTechTreeDialog_InsertNotPossibleWhileElementInTree, Environment.NewLine,
                            string.Join(Environment.NewLine, forbiddenElements.Select(element => element.BuildIdentification()))),
                            TitleRecources.Generic_Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        try
                        {
                            UpdateTechTreeNames(sourceNode);
                            UpdateTechTreeParent(targetNode?.ElementName, sourceNode);
                            Add(sourceNode, targetIndex);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            ShowUpdateErrorMessage(ex.Message);
                        }
                    }
                }
                return false;
            }
            private bool TrySetUnlockData(ElementNode sourceNode, int unlockLevel, int unlockCost)
            {
                try
                {
                    EcfParameter levelParameter = sourceNode.Element.FindOrCreateParameter(ParentForm.UnlockLevelParameterKey);
                    EcfParameter costParameter = sourceNode.Element.FindOrCreateParameter(ParentForm.UnlockCostParameterKey);

                    levelParameter.ClearValues();
                    costParameter.ClearValues();
                    levelParameter.AddValue(unlockLevel.ToString());
                    costParameter.AddValue(unlockCost.ToString());
                    ParentForm.ChangedFileTabs.Add(sourceNode.Tab);

                    sourceNode.SetUnlockLevel(unlockLevel);
                    sourceNode.SetUnlockCost(unlockCost);

                    return true;
                }
                catch (Exception ex)
                {
                    ShowUpdateErrorMessage(ex.Message);
                }
                return false;
            }
            private void UpdateTechTreeParent(string techTreeParentName, ElementNode sourceNode)
            {
                EcfParameter parameter = sourceNode.Element.FindOrCreateParameter(ParentForm.TechTreeParentParameterKey);
                
                parameter.ClearValues();
                parameter.AddValue(techTreeParentName ?? string.Empty);
                ParentForm.ChangedFileTabs.Add(sourceNode.Tab);

                sourceNode.TechTreeParentName = techTreeParentName;
            }
            private void UpdateTechTreeNames(TreeNodeCollection sourceNodes)
            {
                sourceNodes.Cast<ElementNode>().ToList().ForEach(node => UpdateTechTreeNames(node));
            }
            private void UpdateTechTreeNames(ElementNode sourceNode)
            {
                EcfParameter parameter = sourceNode.Element.FindOrCreateParameter(ParentForm.TechTreeNamesParameterKey);
                if (!parameter.ContainsValue(TreeName))
                {
                    parameter.AddValue(TreeName);
                    ParentForm.ChangedFileTabs.Add(sourceNode.Tab);
                }
                UpdateTechTreeNames(sourceNode.Nodes);
            }
            private void RemoveTechTreeName(ElementNode targetNode)
            {
                EcfBlock block = targetNode.Element;
                if (block.HasParameter(ParentForm.TechTreeNamesParameterKey, out EcfParameter treeNameParameter))
                {
                    treeNameParameter.RemoveValue(TreeName);
                    bool noTreeNameLeft = !treeNameParameter.HasValue();
                    if (noTreeNameLeft)
                    {
                        treeNameParameter.AddValue("");
                    }
                    if (noTreeNameLeft || (treeNameParameter.ContainsValue("") && treeNameParameter.CountValues() == 1))
                    {
                        block.RemoveParameter(ParentForm.TechTreeParentParameterKey);
                        block.RemoveParameter(ParentForm.UnlockLevelParameterKey);
                        block.RemoveParameter(ParentForm.UnlockCostParameterKey);
                    }
                    ParentForm.ChangedFileTabs.Add(targetNode.Tab);
                }
            }
            private void RemoveNode(TreeNode targetNode)
            {
                if (targetNode is ElementNode node)
                {
                    foreach (TreeNode subNode in node.Nodes)
                    {
                        RemoveNode(subNode);
                    }
                    RemoveTechTreeName(node);
                    node.Remove();
                }
            }
            private List<EcfBlock> BuildElementList(ElementNode node)
            {
                List<EcfBlock> elements = new List<EcfBlock> { node.Element };
                elements.AddRange(BuildElementList(node.Nodes));
                return elements;
            }
            private List<EcfBlock> BuildElementList(TreeNodeCollection nodes)
            {
                List<EcfBlock> elements = new List<EcfBlock>();
                foreach (ElementNode node in nodes.Cast<ElementNode>())
                {
                    elements.Add(node.Element);
                    elements.AddRange(BuildElementList(node.Nodes));
                }
                return elements;
            }
        }
        private enum CursorAlignment
        {
            None,
            Center,
            UpperEdge,
            LowerEdge,
        } 
        public class ElementNode : TreeNode
        {
            public string ElementName { get; }
            public string TechTreeParentName { get; set; } = null;
            public int UnlockLevel { get; private set; } = UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockLevel;
            public int UnlockCost { get; private set; } = UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockCost;

            public EcfTabPage Tab { get; }
            public EcfBlock Element { get; }

            public ElementNode(EcfTabPage tab, EcfBlock element, string elementName)
            {
                Tab = tab;
                Element = element;
                ElementName = elementName ?? TitleRecources.Generic_Replacement_Empty;

                ToolTipText = element?.BuildIdentification() ?? TitleRecources.Generic_Replacement_Empty;

                UpdateNodeText();
            }
            public ElementNode(EcfTabPage tab, EcfBlock element, string elementName, string techTreeParentName, int unlockLevel, int unlockCost)
            {
                Tab = tab;
                Element = element;
                ElementName = elementName ?? TitleRecources.Generic_Replacement_Empty;

                TechTreeParentName = techTreeParentName;
                UnlockLevel = unlockLevel;
                UnlockCost = unlockCost;

                ToolTipText = element?.BuildIdentification() ?? TitleRecources.Generic_Replacement_Empty;

                UpdateNodeText();
            }
            public ElementNode(ElementNode template) : this(template.Tab, template.Element, template.ElementName, template.TechTreeParentName, template.UnlockLevel, template.UnlockCost)
            {
                Nodes.AddRange(template.Nodes.Cast<ElementNode>().Select(node => new ElementNode(node)).ToArray());
            }

            // public
            public void SetUnlockLevel(int unlockLevel)
            {
                UnlockLevel = unlockLevel;
                UpdateNodeText();
            }
            public void SetUnlockCost(int unlockCost)
            {
                UnlockCost = unlockCost;
                UpdateNodeText();
            }

            // private 
            private void UpdateNodeText()
            {
                Text = string.Format("{0}: {1} // {2}: {3} // {4}: {5}",
                    TitleRecources.Generic_Name, ElementName,
                    TitleRecources.Generic_Level, UnlockLevel,
                    TitleRecources.Generic_Cost, UnlockCost);
            }
        }
        private class TreeAlteratingTools : EcfToolBox
        {
            public event EventHandler AddTreeClicked;
            public event EventHandler RemoveTreeClicked;
            public event EventHandler RenameTreeClicked;
            public event EventHandler CopyTreeClicked;
            public event EventHandler PasteTreeClicked;

            private EcfToolBarButton AddTreeButton { get; } = new EcfToolBarButton(TextRecources.EcfTechTreeDialog_ToolTip_AddTree, IconRecources.Icon_Add, null);
            private EcfToolBarButton RemoveTreeButton { get; } = new EcfToolBarButton(TextRecources.EcfTechTreeDialog_ToolTip_RemoveTree, IconRecources.Icon_Remove, null);
            private EcfToolBarButton RenameTreeButton { get; } = new EcfToolBarButton(TextRecources.EcfTechTreeDialog_ToolTip_RenameTree, IconRecources.Icon_ChangeSimple, null);
            private EcfToolBarButton CopyTreeButton { get; } = new EcfToolBarButton(TextRecources.EcfTechTreeDialog_ToolTip_CopyTree, IconRecources.Icon_Copy, null);
            private EcfToolBarButton PasteTreeButton { get; } = new EcfToolBarButton(TextRecources.EcfTechTreeDialog_ToolTip_PasteTree, IconRecources.Icon_Paste, null);

            public TreeAlteratingTools() : base()
            {
                Add(AddTreeButton);
                Add(RemoveTreeButton);
                Add(RenameTreeButton);
                Add(CopyTreeButton);
                Add(PasteTreeButton);

                AddTreeButton.Click += (sender, evt) => AddTreeClicked?.Invoke(sender, evt);
                RenameTreeButton.Click += (sender, evt) => RenameTreeClicked?.Invoke(sender, evt);
                RemoveTreeButton.Click += (sender, evt) => RemoveTreeClicked?.Invoke(sender, evt);
                CopyTreeButton.Click += (sender, evt) => CopyTreeClicked?.Invoke(sender, evt);
                PasteTreeButton.Click += (sender, evt) => PasteTreeClicked?.Invoke(sender, evt);
            }
        }
    }
}
