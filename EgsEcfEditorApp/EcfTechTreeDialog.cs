using EcfFileViews;
using EcfToolBarControls;
using EcfWinFormControls;
using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace EgsEcfEditorApp
{
    public partial class EcfTechTreeDialog : Form
    {
        public HashSet<EcfTabPage> ChangedFileTabs { get; } = new HashSet<EcfTabPage>();
        protected List<EcfTabPage> UniqueFileTabs { get; } = new List<EcfTabPage>();
        
        private EcfItemSelectorDialog FileTabSelector { get; } = new EcfItemSelectorDialog();
        private TreeAlteratingTools TreeTools { get; } = new TreeAlteratingTools();
        private EcfTextInputDialog TreeNameSelector { get; } = new EcfTextInputDialog(TitleRecources.EcfTechTreeDialog_TreeNameInputHeader);
        protected EcfTechTreeItemEditorDialog TreeItemEditor { get; } = new EcfTechTreeItemEditorDialog();
        protected ContextMenuStrip TechTreeOperationMenu { get; } = new ContextMenuStrip();
        private ToolStripMenuItem NodeChangeItem { get; } = new ToolStripMenuItem(TitleRecources.Generic_Change, IconRecources.Icon_ChangeSimple);
        private ToolStripMenuItem NodeAddRootItem { get; } = new ToolStripMenuItem(TitleRecources.Generic_Add, IconRecources.Icon_Add);
        private ToolStripMenuItem NodeAddSubItem { get; } = new ToolStripMenuItem(TitleRecources.Generic_Add, IconRecources.Icon_Add);
        private ToolStripMenuItem NodeCopyItem { get; } = new ToolStripMenuItem(TitleRecources.Generic_Copying, IconRecources.Icon_Copy);
        private ToolStripMenuItem NodePasteItem { get; } = new ToolStripMenuItem(TitleRecources.Generic_Paste, IconRecources.Icon_Paste);
        private ToolStripMenuItem NodeRemoveFromThisItem { get; } = new ToolStripMenuItem(TitleRecources.EcfTechTreeDialog_RemoveFromThisMenuItem, IconRecources.Icon_Remove);
        private ToolStripMenuItem NodeRemoveFromAllItem { get; } = new ToolStripMenuItem(TitleRecources.EcfTechTreeDialog_RemoveFromAllMenuItem, IconRecources.Icon_Remove);

        protected List<EcfBlock> AvailableElements { get; } = new List<EcfBlock>();
        protected TechTreeNode LastCopiedTechTreeNode { get; set; } = null;
        private List<TechTreeNode> TechTreeRootNodes { get; } = new List<TechTreeNode>();
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

            ToolContainer.Add(TreeTools);

            TreeTools.AddTreeClicked += TreeTools_AddTreeClicked;
            TreeTools.RemoveTreeClicked += TreeTools_RemoveTreeClicked;
            TreeTools.RenameTreeClicked += TreeTools_RenameTreeClicked;
            TreeTools.CopyTreeClicked += TreeTools_CopyTreeClicked;
            TreeTools.PasteTreeClicked += TreeTools_PasteTreeClicked;

            NodeChangeItem.Click += NodeChangeItem_Click;
            NodeAddRootItem.Click += NodeAddRootItem_Click;
            NodeAddSubItem.Click += NodeAddSubItem_Click;
            NodeCopyItem.Click += NodeCopyItem_Click;
            NodePasteItem.Click += NodePasteItem_Click;
            NodeRemoveFromThisItem.Click += NodeRemoveFromThisItem_Click;
            NodeRemoveFromAllItem.Click += NodeRemoveFromAllItem_Click;

            TechTreeOperationMenu.Items.Add(NodeChangeItem);
            TechTreeOperationMenu.Items.Add(NodeAddRootItem);
            TechTreeOperationMenu.Items.Add(NodeAddSubItem);
            TechTreeOperationMenu.Items.Add(NodeCopyItem);
            TechTreeOperationMenu.Items.Add(NodePasteItem);
            TechTreeOperationMenu.Items.Add(NodeRemoveFromThisItem);
            TechTreeOperationMenu.Items.Add(NodeRemoveFromAllItem);
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
            if (TechTreePageContainer.SelectedTab is EcfTechTree selectedTree) 
            {
                if (MessageBox.Show(this, string.Format("{0}:{1}{1}{2}", TextRecources.EcfTechTreeDialog_ReallyRemoveTechTreeQuestion, Environment.NewLine, selectedTree.TreeName),
                    TitleRecources.Generic_Attention, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    selectedTree.ClearTreeNameFromAllNodes(true);
                    TechTreePageContainer.TabPages.Remove(selectedTree);
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
                    selectedTree.ResetTreeName(treeName);
                }
            }
        }
        private void TreeTools_CopyTreeClicked(object sender, EventArgs evt)
        {
            if (TechTreePageContainer.SelectedTab is EcfTechTree selectedTree)
            {
                LastCopiedTree = new EcfTechTree(selectedTree);
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
                    TechTreePageContainer.SelectedTab = copiedTree;
                }
            }
        }
        private void NodeChangeItem_Click(object sender, EventArgs evt)
        {
            if (TechTreePageContainer.SelectedTab is EcfTechTree selectedTree)
            {
                selectedTree.ChangeSelectedNode();
            }
        }
        private void NodeAddRootItem_Click(object sender, EventArgs evt)
        {
            if (TechTreePageContainer.SelectedTab is EcfTechTree selectedTree)
            {
                selectedTree.AddNodeToRoot();
            }
        }
        private void NodeAddSubItem_Click(object sender, EventArgs evt)
        {
            if (TechTreePageContainer.SelectedTab is EcfTechTree selectedTree)
            {
                selectedTree.AddNodeToSelectedNode();
            }
        }
        private void NodeCopyItem_Click(object sender, EventArgs evt)
        {
            if (TechTreePageContainer.SelectedTab is EcfTechTree selectedTree)
            {
                selectedTree.CopySelectedRootNode();
            }
        }
        private void NodePasteItem_Click(object sender, EventArgs evt)
        {
            if (TechTreePageContainer.SelectedTab is EcfTechTree selectedTree)
            {
                selectedTree.PasteRootNode();
            }
        }
        private void NodeRemoveFromThisItem_Click(object sender, EventArgs evt)
        {
            if (TechTreePageContainer.SelectedTab is EcfTechTree selectedTree)
            {
                selectedTree.RemoveSelectedNode(false);
            }
        }
        private void NodeRemoveFromAllItem_Click(object sender, EventArgs evt)
        {
            if (TechTreePageContainer.SelectedTab is EcfTechTree selectedTree)
            {
                selectedTree.RemoveSelectedNode(true);
            }
        }

        // public
        public DialogResult ShowDialog(IWin32Window parent, List<EcfTabPage> openedFileTabs)
        {
            ChangedFileTabs.Clear();
            DialogResult result = UpdateUniqueFileTabs(openedFileTabs);
            if (result != DialogResult.OK) { return result; }
            LoadTechTreeRootNodes();
            LoadTechTrees();
            return ShowDialog(parent);
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
                treeNameValid = !string.IsNullOrEmpty(treeName) || 
                    !TechTreePageContainer.TabPages.Cast<EcfTechTree>().Where(tree => tree != editedTree).Any(tree => tree.TreeName.Equals(treeName));
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
        private void LoadTechTrees()
        {
            HashSet<string> techTreeNames = GetTechTreeNames(TechTreeRootNodes.AsReadOnly());
            TechTreePageContainer.SuspendLayout();
            TechTreePageContainer.TabPages.Clear();
            foreach (string treeName in techTreeNames)
            {
                if (!string.IsNullOrEmpty(treeName)) 
                {
                    EcfTechTree treePage = new EcfTechTree(this, treeName);
                    treePage.Reload();
                    TechTreePageContainer.TabPages.Add(treePage);
                }
            }
            TechTreePageContainer.ResumeLayout();
        }
        private void LoadTechTreeRootNodes()
        {
            TechTreeRootNodes.Clear();
            UniqueFileTabs.ForEach(tab =>
            {
                foreach (EcfBlock element in tab.File.ItemList.Where(item => item is EcfBlock))
                {
                    TechTreeNode newNode = new TechTreeNode(this, tab, element);
                    if (!newNode.IsTechTreeNamesEmpty())
                    {
                        AddTechTreeNode(newNode, null);
                    }
                }
            });
        }
        protected void AddTechTreeNode(TechTreeNode techTreeNode, int? index)
        {
            TechTreeRootNodes.ToList().ForEach(rootElement =>
            {
                if (!string.IsNullOrEmpty(rootElement.TechTreeParentName) && string.Equals(rootElement.TechTreeParentName, techTreeNode.ElementName))
                {
                    TechTreeRootNodes.Remove(rootElement);
                    techTreeNode.Add(rootElement);
                }
            });
            TechTreeNode parent = FindParentNode(techTreeNode.TechTreeParentName, TechTreeRootNodes.AsReadOnly());
            if (parent != null)
            {
                parent.Insert(index ?? parent.Nodes.Count, techTreeNode);
            }
            else
            {
                TechTreeRootNodes.Insert(index ?? TechTreeRootNodes.Count, techTreeNode);
            }
        }
        protected void RemoveTechTreeNode(TechTreeNode techTreeNode)
        {
            if(techTreeNode.Parent != null)
            {
                techTreeNode.Parent.Remove(techTreeNode);
            }
            else
            {
                TechTreeRootNodes.Remove(techTreeNode);
            }
        }
        private TechTreeNode FindParentNode(string parentName, ReadOnlyCollection<TechTreeNode> nodes)
        {
            foreach (TechTreeNode node in nodes)
            {
                if (string.Equals(parentName, node.ElementName))
                {
                    return node;
                }
                else
                {
                    TechTreeNode subNode = FindParentNode(parentName, node.Nodes);
                    if (subNode != null)
                    {
                        return subNode;
                    }
                }
            }
            return null;
        }
        private HashSet<string> GetTechTreeNames(ReadOnlyCollection<TechTreeNode> nodes)
        {
            HashSet<string> treeNames = new HashSet<string>();
            foreach (TechTreeNode node in nodes)
            {
                foreach (string name in node.TechTreeNames) 
                {
                    treeNames.Add(name);
                }
                foreach (string name in GetTechTreeNames(node.Nodes))
                {
                    treeNames.Add(name);
                }
            }
            return treeNames;
        }
        protected List<TechTreeNode> GetRootTechTreeNodes(string treeName)
        {
            return TechTreeRootNodes.Where(node => node.ContainsTreeName(treeName)).ToList();
        }
        private void HideNodeSpecificMenuStripItems(bool hide)
        {
            NodeChangeItem.Visible = !hide;
            NodeAddRootItem.Visible = hide;
            NodeAddSubItem.Visible = !hide;
            NodeCopyItem.Visible = !hide;
            NodeRemoveFromThisItem.Visible = !hide;
            NodeRemoveFromAllItem.Visible = !hide;
        }
        protected void UpdateCoUseTechTrees(TechTreeNode changedNode, EcfTechTree changedTechTree)
        {
            if (changedNode != null)
            {
                TechTreePageContainer.TabPages.Cast<EcfTechTree>().Where(tree => tree != changedTechTree && tree.HasNode(changedNode)).ToList().ForEach(tree => tree.Reload());
            }
        }
        private List<EcfBlock> BuildElementList(ReadOnlyCollection<TechTreeNode> nodes)
        {
            List<EcfBlock> elements = new List<EcfBlock>();
            foreach (TechTreeNode node in nodes)
            {
                elements.Add(node.Element);
                elements.AddRange(BuildElementList(node.Nodes));
            }
            return elements;
        }
        protected List<EcfBlock> BuildUnusedElementsList()
        {
            return AvailableElements.Except(BuildElementList(TechTreeRootNodes.AsReadOnly())).ToList();
        }

        // subclasses
        protected class EcfTechTree : TabPage
        {
            public string TreeName { get; private set; }
            private EcfTreeView ElementTreeView { get; } = new EcfTreeView();
            private EcfTechTreeDialog Dialog { get; }

            public EcfTechTree(EcfTechTreeDialog dialog, string name)
            {
                Dialog = dialog;
                SetTreeName(name);

                ElementTreeView.Dock = DockStyle.Fill;
                ElementTreeView.ShowNodeToolTips = true;

                ElementTreeView.KeyUp += ElementTreeView_KeyUp;
                ElementTreeView.KeyPress += ElementTreeView_KeyPress;
                ElementTreeView.MouseClick += ElementTreeView_MouseClick;
                ElementTreeView.MouseDoubleClick += ElementTreeView_MouseDoubleClick;

                Controls.Add(ElementTreeView);
            }
            public EcfTechTree(EcfTechTree template) : this(template.Dialog, template.TreeName)
            {
                ElementTreeView.Nodes.AddRange(template.ElementTreeView.Nodes.Cast<TechTreeDisplayNode>().Select(node => new TechTreeDisplayNode(node)).ToArray());
            }

            // events
            private void ElementTreeView_KeyUp(object sender, KeyEventArgs evt)
            {
                if (evt.KeyCode == Keys.Delete) { RemoveNode(ElementTreeView.SelectedNode, false); evt.Handled = true; }
                else if (evt.Control && evt.KeyCode == Keys.C) { CopySelectedRootNode(); evt.Handled = true; }
                else if (evt.Control && evt.KeyCode == Keys.V) { PasteRootNode(); evt.Handled = true; }
            }
            private void ElementTreeView_KeyPress(object sender, KeyPressEventArgs evt)
            {
                // hack for sqirky "ding"
                evt.Handled = true;
            }
            private void ElementTreeView_MouseClick(object sender, MouseEventArgs evt)
            {
                TreeNode node = ElementTreeView.GetNodeAt(evt.Location);
                if (evt.Button == MouseButtons.Right)
                {
                    if (node != null)
                    {
                        ElementTreeView.SelectedNode = node;
                        Dialog.HideNodeSpecificMenuStripItems(false);
                        Dialog.TechTreeOperationMenu.Show(this, evt.Location);
                    }
                    else
                    {
                        Dialog.HideNodeSpecificMenuStripItems(true);
                        Dialog.TechTreeOperationMenu.Show(this, evt.Location);
                    }
                }
            }
            private void ElementTreeView_MouseDoubleClick(object sender, MouseEventArgs evt)
            {
                TreeNode node = ElementTreeView.GetNodeAt(evt.Location);
                if (node != null && evt.Button == MouseButtons.Left)
                {
                    ChangeNode(node);
                }
            }

            // public
            public void ClearTreeNameFromAllNodes(bool checkUnattached)
            {
                foreach (TechTreeDisplayNode node in ElementTreeView.Nodes.Cast<TechTreeDisplayNode>())
                {
                    RemoveTechTreeName(node.SourceNode, checkUnattached);
                }
            }
            public void ResetTreeName(string treeName)
            {
                ClearTreeNameFromAllNodes(false);
                SetTreeName(treeName);
            }
            public void SetTreeName(string treeName)
            {
                TreeName = string.IsNullOrEmpty(treeName) ? TitleRecources.Generic_Replacement_Empty : treeName;
                Text = TreeName;
                try
                {
                    AddTechTreeNameToAllNodes(ElementTreeView.Nodes.Cast<TechTreeDisplayNode>().Select(node => node.SourceNode).ToList().AsReadOnly(), treeName);
                }
                catch (Exception ex)
                {
                    ShowUpdateErrorMessage(ex.Message);
                }
            }
            public void ChangeSelectedNode()
            {
                ChangeNode(ElementTreeView.SelectedNode);
            }
            public void AddNodeToRoot()
            {
                AddNode(null);
            }
            public void AddNodeToSelectedNode()
            {
                AddNode(ElementTreeView.SelectedNode);
            }
            public void CopySelectedRootNode()
            {
                if (ElementTreeView.SelectedNode is TechTreeDisplayNode selectedNode)
                {
                    Dialog.LastCopiedTechTreeNode = selectedNode.SourceNode;
                }
            }
            public void PasteRootNode()
            {
                TechTreeNode pastingNode = Dialog.LastCopiedTechTreeNode?.FindRoot();
                if (pastingNode != null && InsertionAllowed(pastingNode))
                {
                    TrySetStructureData(pastingNode, TreeName, null);
                    Reload();
                }
            }
            public void RemoveSelectedNode(bool removeFromAllTrees)
            {
                RemoveNode(ElementTreeView.SelectedNode, removeFromAllTrees);
            }
            public bool HasNode(TechTreeNode node)
            {
                return BuildElementList(ElementTreeView.Nodes).Contains(node);
            }
            [Obsolete("needs work")]
            public void Reload()
            {
                ElementTreeView.SuspendLayout();
                ElementTreeView.Nodes.Clear();
                ElementTreeView.Nodes.AddRange(Dialog.GetRootTechTreeNodes(TreeName).Select(node => node.BuildNodeTree()).ToArray());
                ElementTreeView.ExpandAll();
                ElementTreeView.ResumeLayout();
            }

            // private
            private void ShowUpdateErrorMessage(string message)
            {
                MessageBox.Show(this, string.Format("{0}:{1}{1}{2}", TextRecources.EcfTechTreeDialog_ElementSettingsUpdateError, Environment.NewLine, message),
                    TitleRecources.Generic_Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            private void AddTechTreeNameToAllNodes(TechTreeNode targetNode, string treeName)
            {
                targetNode.AddTechTreeName(treeName);
                AddTechTreeNameToAllNodes(targetNode.Nodes, treeName);
            }
            private void AddTechTreeNameToAllNodes(ReadOnlyCollection<TechTreeNode> nodes, string treeName)
            {
                foreach (TechTreeNode node in nodes.Cast<TechTreeNode>())
                {
                    node.AddTechTreeName(treeName);
                    AddTechTreeNameToAllNodes(node.Nodes, treeName);
                }
            }
            private bool InsertionAllowed(TechTreeNode sourceNode)
            {
                List<TechTreeNode> treeElements = BuildElementList(ElementTreeView.Nodes);
                List<TechTreeNode> insertingElements = BuildElementList(sourceNode);
                List<TechTreeNode> forbiddenElements = insertingElements.Where(element => treeElements.Contains(element)).ToList();
                if (forbiddenElements.Count > 0)
                {
                    MessageBox.Show(this,
                        string.Format("{0}:{1}{1}{2}", TextRecources.EcfTechTreeDialog_InsertNotPossibleWhileElementInTree, Environment.NewLine,
                        string.Join(Environment.NewLine, forbiddenElements.Select(element => element.Element.BuildIdentification()))),
                        TitleRecources.Generic_Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                return true;
            }
            private bool TrySetStructureData(TechTreeNode targetNode, string treeName, string techTreeParentName)
            {
                try
                {
                    AddTechTreeNameToAllNodes(targetNode, treeName);
                    targetNode.SetTechTreeParent(techTreeParentName);
                    return true;
                }
                catch (Exception ex)
                {
                    ShowUpdateErrorMessage(ex.Message);
                }
                return false;
            }
            private bool TrySetUnlockData(TechTreeNode targetNode, int unlockLevel, int unlockCost)
            {
                try
                {
                    targetNode.SetUnlockLevel(unlockLevel);
                    targetNode.SetUnlockCost(unlockCost);
                    return true;
                }
                catch (Exception ex)
                {
                    ShowUpdateErrorMessage(ex.Message);
                }
                return false;
            }
            private bool TrySwapElement(TechTreeNode targetNode, EcfBlock element)
            {
                if (element != targetNode.Element)
                {
                    try
                    {
                        EcfTabPage tab = Dialog.UniqueFileTabs.FirstOrDefault(page => page.File.ItemList.Contains(element));
                        targetNode.SwapElement(tab, element);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        ShowUpdateErrorMessage(ex.Message);
                    }
                }
                return false;
            }
            private List<TechTreeNode> BuildElementList(TechTreeNode node)
            {
                List<TechTreeNode> elements = new List<TechTreeNode> { node };
                foreach (TechTreeNode subNode in node.Nodes)
                {
                    elements.AddRange(BuildElementList(subNode));
                }
                return elements;
            }
            private List<TechTreeNode> BuildElementList(TreeNodeCollection nodes)
            {
                List<TechTreeNode> elements = new List<TechTreeNode>();
                foreach (TechTreeDisplayNode node in nodes.Cast<TechTreeDisplayNode>())
                {
                    elements.Add(node.SourceNode);
                    elements.AddRange(BuildElementList(node.Nodes));
                }
                return elements;
            }
            private void ChangeNode(TreeNode targetNode)
            {
                if (targetNode is TechTreeDisplayNode node)
                {
                    TechTreeNode changedNode = node.SourceNode;
                    EcfBlock element = changedNode.Element;
                    int unlockLevel = changedNode.UnlockLevel;
                    int unlockCost = changedNode.UnlockCost;

                    List<EcfBlock> availableElements = Dialog.BuildUnusedElementsList();
                    if (!availableElements.Contains(element)) { availableElements.Insert(0, element); }

                    if (Dialog.TreeItemEditor.ShowDialog(this, element, availableElements, unlockLevel, unlockCost) == DialogResult.OK)
                    {
                        element = Dialog.TreeItemEditor.GetSelectedElement();
                        unlockLevel = Dialog.TreeItemEditor.GetUnlockLevel();
                        unlockCost = Dialog.TreeItemEditor.GetUnlockCost();
                        TrySwapElement(changedNode, element);
                        TrySetUnlockData(changedNode, unlockLevel, unlockCost);
                        Dialog.UpdateCoUseTechTrees(changedNode, this);
                        Reload();
                    }
                }
            }
            private void AddNode(TreeNode targetNode)
            {
                TechTreeDisplayNode targetDisplayNode = targetNode as TechTreeDisplayNode;
                int unlockLevel = UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockLevel;
                int unlockCost = UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockCost;
                List<EcfBlock> availableElements = Dialog.BuildUnusedElementsList();

                if (Dialog.TreeItemEditor.ShowDialog(this, null, availableElements, unlockLevel, unlockCost) == DialogResult.OK)
                {
                    EcfBlock element = Dialog.TreeItemEditor.GetSelectedElement();
                    EcfTabPage tab = Dialog.UniqueFileTabs.FirstOrDefault(page => page.File == element.EcfFile);
                    TechTreeNode newNode = new TechTreeNode(Dialog, tab, element);
                    TrySetUnlockData(newNode, Dialog.TreeItemEditor.GetUnlockLevel(), Dialog.TreeItemEditor.GetUnlockCost());
                    TrySetStructureData(newNode, TreeName, targetDisplayNode?.SourceNode.ElementName);
                    Dialog.AddTechTreeNode(newNode, null);
                    Dialog.UpdateCoUseTechTrees(targetDisplayNode?.SourceNode, this);
                    Reload();
                }
            }
            private void RemoveNode(TreeNode targetNode, bool removeFromAllTrees)
            {
                if (targetNode is TechTreeDisplayNode node)
                {
                    if (removeFromAllTrees)
                    {
                        RemoveNodeFromAllTechTrees(node.SourceNode);
                    }
                    else
                    {
                        RemoveTechTreeName(node.SourceNode.FindRoot(), true);
                    }
                    Dialog.UpdateCoUseTechTrees(node.SourceNode, this);
                    Reload();
                }
            }
            private void RemoveNodeFromAllTechTrees(TechTreeNode targetNode)
            {
                targetNode.Nodes.ToList().ForEach(node =>
                {
                    RemoveNodeFromAllTechTrees(node);
                });
                targetNode.RemoveTechTreeNames();
                Dialog.RemoveTechTreeNode(targetNode);
            }
            private void RemoveTechTreeName(TechTreeNode targetNode, bool checkUnattached)
            {
                targetNode.Nodes.ToList().ForEach(node =>
                {
                    RemoveTechTreeName(node, checkUnattached);
                });
                targetNode.RemoveTechTreeName(TreeName);
                if (checkUnattached && targetNode.IsTechTreeNamesEmpty())
                {
                    Dialog.RemoveTechTreeNode(targetNode);
                }
            }
        }
        public class TechTreeNode
        {
            public string ElementName { get; private set; } = null;
            public List<string> TechTreeNames { get; } = new List<string>();
            public string TechTreeParentName { get; private set; } = null;
            public int UnlockLevel { get; private set; } = UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockLevel;
            public int UnlockCost { get; private set; } = UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockCost;
            public string ToolTip { get; private set; } = null;

            public TechTreeNode Parent { get; set; } = null;
            public ReadOnlyCollection<TechTreeNode> Nodes { get; }
            private List<TechTreeNode> InternalNodes { get; } = new List<TechTreeNode>();

            private EcfTechTreeDialog Dialog { get; }
            public EcfTabPage Tab { get; private set; }
            public EcfBlock Element { get; private set; }

            public TechTreeNode(EcfTechTreeDialog dialog, EcfTabPage tab, EcfBlock element)
            {
                Dialog = dialog;
                Tab = tab;
                Element = element;

                Nodes = InternalNodes.AsReadOnly();

                UpdateFromElement();
            }

            // public
            public void Add(TechTreeNode node)
            {
                node.Parent = this;
                InternalNodes.Add(node);
            }
            public void Insert(int index, TechTreeNode node)
            {
                node.Parent = this;
                InternalNodes.Insert(index, node);
            }
            public void Remove(TechTreeNode node)
            {
                InternalNodes.Remove(node);
            }
            public TechTreeDisplayNode BuildTreeNode()
            {
                return new TechTreeDisplayNode(this, string.Format("{0} // {1}: {2} // {3}: {4}",
                    ElementName, TitleRecources.Generic_Level, UnlockLevel, TitleRecources.Generic_Cost, UnlockCost), ToolTip);
            }
            public TechTreeDisplayNode BuildNodeTree()
            {
                return BuildNodeTree(FindRoot(this));
            }
            public void UpdateFromElement()
            {
                UpdateElementName();
                UpdateTechTreeSettings();
            }
            public bool ContainsTreeName(string treeName)
            {
                return ContainsTreeName(treeName, FindRoot(this));
            }
            public void RemoveTechTreeNames()
            {
                TechTreeNames.Clear();
                Element.HasParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeNames, out EcfParameter treeNameParameter);
                treeNameParameter.ClearValues();
                treeNameParameter.AddValue(string.Empty);
                Element.RemoveParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeParentName);
                Element.RemoveParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockLevel);
                Element.RemoveParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockCost);
                Dialog.ChangedFileTabs.Add(Tab);
            }
            public void RemoveTechTreeName(string treeName)
            {
                if (Element.HasParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeNames, out EcfParameter treeNameParameter))
                {
                    TechTreeNames.Remove(treeName);
                    treeNameParameter.RemoveValue(treeName);
                    if (!treeNameParameter.HasValue())
                    {
                        treeNameParameter.AddValue(string.Empty);
                    }
                    if (IsTechTreeNamesEmpty())
                    {
                        Element.RemoveParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeParentName);
                        Element.RemoveParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockLevel);
                        Element.RemoveParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockCost);
                    }
                    Dialog.ChangedFileTabs.Add(Tab);
                }
            }
            public bool IsTechTreeNamesEmpty()
            {
                return TechTreeNames.Count == 0 || (TechTreeNames.Contains(string.Empty) && TechTreeNames.Count == 1);
            }
            public void AddTechTreeName(string treeName)
            {
                EcfParameter parameter = Element.FindOrCreateParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeNames);
                TechTreeNames.Remove(string.Empty);
                parameter.RemoveValue(string.Empty);
                if (!TechTreeNames.Contains(treeName))
                {
                    TechTreeNames.Add(treeName);
                    parameter.AddValue(treeName);
                    Dialog.ChangedFileTabs.Add(Tab);
                }
            }
            public void SetTechTreeNames(List<string> treeNames)
            {
                TechTreeNames.Clear();
                if (treeNames != null) { TechTreeNames.AddRange(treeNames); }
                SetParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeNames, treeNames);
            }
            public void SetTechTreeParent(string parentName)
            {
                TechTreeParentName = parentName;
                SetParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeParentName, parentName ?? string.Empty);
            }
            public void SetUnlockLevel(int level)
            {
                UnlockLevel = level;
                SetParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockLevel, level.ToString());
            }
            public void SetUnlockCost(int cost)
            {
                UnlockCost = cost;
                SetParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockCost, cost.ToString());
            }
            public void SwapElement(EcfTabPage tab, EcfBlock element)
            {
                Element.RemoveParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeNames);
                Element.RemoveParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeParentName);
                Element.RemoveParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockLevel);
                Element.RemoveParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockCost);
                Dialog.ChangedFileTabs.Add(Tab);

                Tab = tab;
                Element = element;
                UpdateElementName();

                foreach(TechTreeNode node in Nodes)
                {
                    node.SetTechTreeParent(ElementName);
                }

                SetTechTreeNames(TechTreeNames.ToList());
                SetTechTreeParent(TechTreeParentName);
                SetUnlockLevel(UnlockLevel);
                SetUnlockCost(UnlockCost);
                Dialog.ChangedFileTabs.Add(tab);
            }
            public TechTreeNode FindRoot()
            {
                return FindRoot(this);
            }

            // private
            private TechTreeNode FindRoot(TechTreeNode techNode)
            {
                return techNode.Parent == null ? techNode : FindRoot(techNode.Parent);
            }
            private bool ContainsTreeName(string treeName, TechTreeNode techNode)
            {
                if (techNode.TechTreeNames.Contains(treeName))
                {
                    return true;
                }
                else
                {
                    return techNode.Nodes.Cast<TechTreeNode>().Any(node => ContainsTreeName(treeName, node));
                }
            }
            private TechTreeDisplayNode BuildNodeTree(TechTreeNode techNode)
            {
                TechTreeDisplayNode treeNode = techNode.BuildTreeNode();
                treeNode.Nodes.AddRange(techNode.Nodes.Select(node => BuildNodeTree(node)).ToArray());
                return treeNode;
            }
            private void SetParameter(string paramKey, string value)
            {
                EcfParameter parameter = Element.FindOrCreateParameter(paramKey);
                parameter.ClearValues();
                parameter.AddValue(value);
                Dialog.ChangedFileTabs.Add(Tab);
            }
            private void SetParameter(string paramKey, List<string> values)
            {
                EcfParameter parameter = Element.FindOrCreateParameter(paramKey);
                parameter.ClearValues();
                parameter.AddValue(values);
                Dialog.ChangedFileTabs.Add(Tab);
            }
            private void UpdateElementName()
            {
                string elementName = Element.GetAttributeFirstValue(UserSettings.Default.EcfTechTreeDialog_AttributeKey_ReferenceName);
                ElementName = elementName ?? TitleRecources.Generic_Replacement_Empty;
                ToolTip = Element?.BuildIdentification() ?? TitleRecources.Generic_Replacement_Empty;
            }
            private void UpdateTechTreeSettings()
            {
                Element.HasParameter(UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeNames, true, out EcfParameter techTreeNames);
                string techTreeParentName = Element.GetParameterFirstValue(UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeParentName, true);
                string unlockLevel = Element.GetParameterFirstValue(UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockLevel, true);
                string unlockCost = Element.GetParameterFirstValue(UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockCost, true);

                if (!int.TryParse(unlockLevel, out int unlockLevelValue))
                {
                    unlockLevelValue = UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockLevel;
                }
                if (!int.TryParse(unlockCost, out int unlockCostValue))
                {
                    unlockCostValue = UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockCost;
                }

                TechTreeNames.Clear();
                EcfValueGroup group = techTreeNames?.ValueGroups.FirstOrDefault();
                if (group != null) { TechTreeNames.AddRange(group.Values); }
                TechTreeParentName = techTreeParentName;
                UnlockLevel = unlockLevelValue;
                UnlockCost = unlockCostValue;
            }
        }
        public class TechTreeDisplayNode : TreeNode
        {
            public TechTreeNode SourceNode { get; }

            public TechTreeDisplayNode(TechTreeNode sourceNode, string text, string toolTip)
            {
                SourceNode = sourceNode;
                Text = text;
                ToolTipText = toolTip;
            }
            public TechTreeDisplayNode(TechTreeDisplayNode template) : this(template.SourceNode, template.Text, template.ToolTipText)
            {
                Nodes.AddRange(template.Nodes.Cast<TechTreeDisplayNode>().Select(node => new TechTreeDisplayNode(node)).ToArray());
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
