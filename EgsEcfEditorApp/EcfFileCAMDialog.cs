using EcfCAMTools;
using EcfFileViews;
using EcfToolBarControls;
using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using static EcfCAMTools.CompareSelectionTools;
using static Helpers.ImageAjustments;
using static EgsEcfParser.EcfStructureTools;
using System.Text;

namespace EgsEcfEditorApp
{
    public partial class EcfFileCAMDialog : Form
    {
        public HashSet<EcfTabPage> ChangedFileTabs { get; } = new HashSet<EcfTabPage>();
        
        private CompareSelectionTools FirstFileSelectionTools { get; } = new CompareSelectionTools();
        private CompareSelectionTools SecondFileSelectionTools { get; } = new CompareSelectionTools();
        private MergeActionTools ActionTools { get; } = new MergeActionTools();
        private DiffNavigationTools NavigationTools { get; } = new DiffNavigationTools();

        private ImageList CAMListViewIcons { get; } = new ImageList();
        private List<ComboBoxItem> AvailableFiles { get; } = new List<ComboBoxItem>();

        private List<CAMTreeNode> FirstFileNodes { get; } = new List<CAMTreeNode>();
        private List<CAMTreeNode> SecondFileNodes { get; } = new List<CAMTreeNode>();
        private static bool IsCrossAccessing { get; set; } = false;
        private int PageSize { get; set; } = 100;
        private int PageNumber { get; set; } = 1;
        private int PageCount { get; set; } = 1;

        public bool CAMAbortPending { get; set; } = false;
        private EcfFileCAMWorkerDialog CAMWorker { get; } = new EcfFileCAMWorkerDialog();

        public EcfFileCAMDialog()
        {
            InitializeComponent();
            InitForm();
        }

        // events
        private void InitForm()
        {
            Icon = IconRecources.Icon_AppBranding;
            Text = TitleRecources.EcfFileCAMDialog_Header;

            FirstFileTreeView.LinkTreeView(SecondFileTreeView);

            FirstFileSelectionContainer.Add(FirstFileSelectionTools);
            SecondFileSelectionContainer.Add(SecondFileSelectionTools);
            ActionContainer.Add(ActionTools);
            NavigationContainer.Add(NavigationTools);

            CAMListViewIcons.Images.Add(AddGap(IconRecources.Icon_Unequal, 16, 3, 1));
            CAMListViewIcons.Images.Add(AddGap(IconRecources.Icon_Add, 16, 3, 1));
            CAMListViewIcons.Images.Add(AddGap(IconRecources.Icon_Remove, 16, 3, 1));
            CAMListViewIcons.Images.Add(AddGap(IconRecources.Icon_Unknown, 16, 3, 1));

            FirstFileTreeView.ImageList = CAMListViewIcons;
            SecondFileTreeView.ImageList = CAMListViewIcons;

            FirstFileDetailsView.SelectionTabs = new int[] { 20, 40 };
            SecondFileDetailsView.SelectionTabs = new int[] { 20, 40 };

            InitEvents();
        }
        private void InitEvents()
        {
            FirstFileTreeView.NodeMouseClick += FirstFileTreeView_NodeMouseClick;
            SecondFileTreeView.NodeMouseClick += SecondFileTreeView_NodeMouseClick;

            FirstFileTreeView.BeforeExpand += FirstFileTreeView_BeforeExpand;
            FirstFileTreeView.BeforeCollapse += FirstFileTreeView_BeforeCollapse;
            SecondFileTreeView.BeforeExpand += SecondFileTreeView_BeforeExpand;
            SecondFileTreeView.BeforeCollapse += SecondFileTreeView_BeforeCollapse;

            FirstFileSelectionTools.SelectionChangeClicked += FirstFileSelectionTools_SelectionChangeClicked;
            SecondFileSelectionTools.SelectionChangeClicked += SecondFileSelectionTools_SelectionChangeClicked;

            ActionTools.MergeToRightClicked += ActionTools_MergeToRightClicked;
            ActionTools.MergeToLeftClicked += ActionTools_MergeToLeftClicked;
            NavigationTools.PageUpClicked += NavigationTools_PageUpClicked;
            NavigationTools.PageDownClicked += NavigationTools_PageDownClicked;
        }

        private void FirstFileTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs evt)
        {
            if (evt.Node is CAMTreeNode treeNode)
            {
                treeNode.UpdateCheckState(null, treeNode.Checked);
                RefreshSelectionTool(FirstFileSelectionTools, FirstFileNodes);
                UpdateDiffDetailsView(FirstFileDetailsView, SecondFileDetailsView, treeNode);
            }
        }
        private void FirstFileSelectionTools_SelectionChangeClicked(object sender, SelectionChangeEventArgs evt)
        {
            switch (evt.Group)
            {
                case SelectionGroups.Unequal:
                    FirstFileNodes.ForEach(node => node.UpdateCheckState(CAMTreeNode.MergeActions.Update, evt.Type == SelectionTypes.All));
                    break;
                case SelectionGroups.Add:
                    FirstFileNodes.ForEach(node => node.UpdateCheckState(CAMTreeNode.MergeActions.Add, evt.Type == SelectionTypes.All));
                    break;
                case SelectionGroups.Remove:
                    FirstFileNodes.ForEach(node => node.UpdateCheckState(CAMTreeNode.MergeActions.Remove, evt.Type == SelectionTypes.All));
                    break;
                default: 
                    break;
            }
            RefreshSelectionTool(FirstFileSelectionTools, FirstFileNodes);
        }
        private void FirstFileComboBox_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            RefreshFileSelectorBox(SecondFileComboBox, AvailableFiles, FirstFileComboBox.SelectedItem as ComboBoxItem);
            RefreshViews();
        }
        private void FirstFileTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs evt)
        {
            InflateSubNodes(evt.Node);

        }
        private void FirstFileTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs evt)
        {
            ReduceSubNodes(evt.Node);
        }

        private void SecondFileTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs evt)
        {
            if (evt.Node is CAMTreeNode treeNode)
            {
                treeNode.UpdateCheckState(null, treeNode.Checked);
                RefreshSelectionTool(SecondFileSelectionTools, SecondFileNodes);
                UpdateDiffDetailsView(SecondFileDetailsView, FirstFileDetailsView, treeNode);
            }
        }
        private void SecondFileSelectionTools_SelectionChangeClicked(object sender, SelectionChangeEventArgs evt)
        {
            switch (evt.Group)
            {
                case SelectionGroups.Unequal: 
                    SecondFileNodes.ForEach(node => node.UpdateCheckState(CAMTreeNode.MergeActions.Update, evt.Type == SelectionTypes.All)); 
                    break;
                case SelectionGroups.Add: 
                    SecondFileNodes.ForEach(node => node.UpdateCheckState(CAMTreeNode.MergeActions.Add, evt.Type == SelectionTypes.All)); 
                    break;
                case SelectionGroups.Remove: 
                    SecondFileNodes.ForEach(node => node.UpdateCheckState(CAMTreeNode.MergeActions.Remove, evt.Type == SelectionTypes.All)); 
                    break;
                default: 
                    break;
            }
            RefreshSelectionTool(SecondFileSelectionTools, SecondFileNodes);
        }
        private void SecondFileComboBox_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            RefreshFileSelectorBox(FirstFileComboBox, AvailableFiles, SecondFileComboBox.SelectedItem as ComboBoxItem);
            RefreshViews();
        }
        private void SecondFileTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs evt)
        {
            InflateSubNodes(evt.Node);
        }
        private void SecondFileTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs evt)
        {
            ReduceSubNodes(evt.Node);
        }

        private void ActionTools_MergeToRightClicked(object sender, EventArgs evt)
        {
            Merge(SecondFileComboBox.SelectedItem as ComboBoxItem, FirstFileNodes, false);
        }
        private void ActionTools_MergeToLeftClicked(object sender, EventArgs evt)
        {
            Merge(FirstFileComboBox.SelectedItem as ComboBoxItem, SecondFileNodes, true);
        }
        private void NavigationTools_PageUpClicked(object sender, EventArgs evt)
        {
            if (PageNumber > 1) { PageNumber--; }
            RefreshTreeViews(FirstFileTreeView, FirstFileNodes, PageSize, PageNumber);
            RefreshTreeViews(SecondFileTreeView, SecondFileNodes, PageSize, PageNumber);
            NavigationTools.UpdatePageButtons(PageNumber, PageCount);
            UpdatePageIndicator(PageNumber, PageCount, FirstFileNodes.Count(node => node.MergeAction != CAMTreeNode.MergeActions.Ignore));
        }
        private void NavigationTools_PageDownClicked(object sender, EventArgs evt)
        {
            if (PageNumber < PageCount) { PageNumber++; }
            RefreshTreeViews(FirstFileTreeView, FirstFileNodes, PageSize, PageNumber);
            RefreshTreeViews(SecondFileTreeView, SecondFileNodes, PageSize, PageNumber);
            NavigationTools.UpdatePageButtons(PageNumber, PageCount);
            UpdatePageIndicator(PageNumber, PageCount, FirstFileNodes.Count(node => node.MergeAction != CAMTreeNode.MergeActions.Ignore));
        }

        // public
        public DialogResult ShowDialog(IWin32Window parent, List<EcfTabPage> openedFileTabs, EcfTabPage selectedPage)
        {
            PageSize = InternalSettings.Default.EcfFileCAMDialog_PageSize;
            
            ChangedFileTabs.Clear();
            AvailableFiles.Clear();
            AvailableFiles.AddRange(openedFileTabs.Select(tab => new ComboBoxItem(tab)));

            ComboBoxItem firstItem = AvailableFiles.FirstOrDefault(tab => tab.Item == selectedPage);
            ComboBoxItem secondItem = AvailableFiles.FirstOrDefault(tab => tab.Item != selectedPage);
            RefreshFileSelectorBox(FirstFileComboBox, AvailableFiles, secondItem, firstItem);
            RefreshFileSelectorBox(SecondFileComboBox, AvailableFiles, firstItem, secondItem);

            RefreshViews();

            return ShowDialog(parent);
        }

        // private
        private void RefreshViews()
        {
            if (!(FirstFileComboBox.SelectedItem is ComboBoxItem firstItem) || !(SecondFileComboBox.SelectedItem is ComboBoxItem secondItem))
            {
                ResetViews();
                return;
            }

            if (CAMWorker.ShowDialog(this, TextRecources.EcfFileCamWorkerDialog_Comparing, false,
                this, () => Compare(FirstFileNodes, firstItem.Item.File.ItemList, SecondFileNodes, secondItem.Item.File.ItemList)) != DialogResult.OK)
            {
                ResetViews();
                return;
            }

            PageNumber = 1;
            PageCount = (int)Math.Ceiling(FirstFileNodes.Count(node => node.MergeAction != CAMTreeNode.MergeActions.Ignore) / (double)PageSize);
            UpdatePageIndicator(PageNumber, PageCount, FirstFileNodes.Count(node => node.MergeAction != CAMTreeNode.MergeActions.Ignore));

            RefreshTreeViews(FirstFileTreeView, FirstFileNodes, PageSize, PageNumber);
            RefreshTreeViews(SecondFileTreeView, SecondFileNodes, PageSize, PageNumber);
            UpdateDiffDetailsView(FirstFileDetailsView, SecondFileDetailsView, null);
            RefreshSelectionTool(FirstFileSelectionTools, FirstFileNodes);
            RefreshSelectionTool(SecondFileSelectionTools, SecondFileNodes);

            ActionTools.EnableMergeToRightButton(FirstFileTreeView.Nodes.Count > 0);
            ActionTools.EnableMergeToLeftButton(SecondFileTreeView.Nodes.Count > 0);
            NavigationTools.UpdatePageButtons(PageNumber, PageCount);
        }
        private void ResetViews()
        {
            FirstFileTreeView.BeginUpdate();
            FirstFileTreeView.Nodes.Clear();
            FirstFileTreeView.EndUpdate();
            SecondFileTreeView.BeginUpdate();
            SecondFileTreeView.Nodes.Clear();
            SecondFileTreeView.EndUpdate();

            ActionTools.EnableMergeToRightButton(false);
            ActionTools.EnableMergeToLeftButton(false);
            NavigationTools.UpdatePageButtons(0, 0);
            UpdatePageIndicator(0, 0, 0);

            FirstFileNodes.Clear();
            SecondFileNodes.Clear();
            FirstFileDetailsView.Clear();
            SecondFileDetailsView.Clear();
            FirstFileSelectionTools.Reset(CheckState.Indeterminate);
            SecondFileSelectionTools.Reset(CheckState.Indeterminate);
        }
        private void Compare(
            List<CAMTreeNode> firstTargetNodeList, ReadOnlyCollection<EcfStructureItem> firstSourceItemList,
            List<CAMTreeNode> secondTargetNodeList, ReadOnlyCollection<EcfStructureItem> secondSourceItemList)
        {
            RefreshTreeNodeList(firstTargetNodeList, firstSourceItemList, true);
            RefreshTreeNodeList(secondTargetNodeList, secondSourceItemList, true);
            CompareTreeNodeLists(firstTargetNodeList, secondTargetNodeList);
        }
        private void RefreshTreeNodeList(List<CAMTreeNode> nodeList, ReadOnlyCollection<EcfStructureItem> itemList, bool initState)
        {
            nodeList.Clear();
            foreach (EcfStructureItem item in itemList)
            {
                if (CAMAbortPending)
                {
                    return;
                }

                CAMTreeNode node = new CAMTreeNode(item, initState);
                nodeList.Add(node);
                if (item is EcfBlock block)
                {
                    RefreshTreeNodeList(node.AllNodes, block.ChildItems, initState);
                    node.ReduceSubNodes();
                }
            }
        }
        private void CompareTreeNodeLists(List<CAMTreeNode> firstNodeList, List<CAMTreeNode> secondNodeList)
        {
            CAMTreeNode concurrentNode;
            firstNodeList.ForEach(node =>
            {
                if (CAMAbortPending)
                {
                    return;
                }

                concurrentNode = secondNodeList.FirstOrDefault(otherNode =>
                    otherNode.MergeAction == CAMTreeNode.MergeActions.Unknown && StructureItemIdEquals(node.Item, otherNode.Item));
                if (concurrentNode == null)
                {
                    BuildTreeNodePair(node, firstNodeList, secondNodeList);
                }
                else
                {
                    CompareTreeNodeLists(node.AllNodes, concurrentNode.AllNodes);
                    CAMTreeNode.MergeActions action;
                    if (node.AllNodes.Any(subnode => subnode.MergeAction != CAMTreeNode.MergeActions.Ignore) ||
                        concurrentNode.AllNodes.Any(subnode => subnode.MergeAction != CAMTreeNode.MergeActions.Ignore) ||
                        !node.Item.ContentEquals(concurrentNode.Item))
                    {
                        action = CAMTreeNode.MergeActions.Update;
                    }
                    else
                    {
                        action = CAMTreeNode.MergeActions.Ignore;
                    }
                    node.UpdatePairingData(concurrentNode, action);
                    concurrentNode.UpdatePairingData(node, action);
                }
            });

            secondNodeList.ForEach(node =>
            {
                if (CAMAbortPending)
                {
                    return;
                }

                if (node.MergeAction == CAMTreeNode.MergeActions.Unknown)
                {
                    BuildTreeNodePair(node, secondNodeList, firstNodeList);
                }
            });
        }
        private void Merge(ComboBoxItem targetItem, List<CAMTreeNode> sourceNodes, bool reverseAnimation)
        {
            if (sourceNodes.Count == 0 || targetItem == null) { return; }

            ChangedFileTabs.Add(targetItem.Item);
            if (CAMWorker.ShowDialog(this, TextRecources.EcfFileCamWorkerDialog_Merging, reverseAnimation,
                this, () => MergeFile(targetItem.Item.File, sourceNodes)) != DialogResult.OK)
            {
                ResetViews();
                return;
            }

            RefreshViews();
        }
        private void MergeFile(EgsEcfFile file, List<CAMTreeNode> sourceList)
        {
            EcfStructureItem concurrentItem;
            foreach (CAMTreeNode node in sourceList)
            {
                if (CAMAbortPending)
                {
                    return;
                }

                if (node.Checked)
                {
                    switch (node.MergeAction)
                    {
                        case CAMTreeNode.MergeActions.Add:
                            concurrentItem = CopyStructureItem(node.Item);
                            concurrentItem.Revalidate();
                            file.AddItem(concurrentItem, sourceList.IndexOf(node));
                            break;
                        case CAMTreeNode.MergeActions.Update:
                            concurrentItem = node.ConcurrentNode.Item;
                            concurrentItem.UpdateContent(node.Item);
                            if (concurrentItem is EcfBlock block)
                            {
                                MergeBlock(block, node.AllNodes);
                            }
                            concurrentItem.Revalidate();
                            break;
                        case CAMTreeNode.MergeActions.Remove:
                            file.RemoveItem(node.Item);
                            break;
                        default: break;
                    }
                }
            }
        }
        private static void InflateSubNodes(TreeNode node)
        {
            if (node is CAMTreeNode treeNode)
            {
                treeNode.InflateSubNodes();
                if (!IsCrossAccessing && treeNode.ConcurrentNode is CAMTreeNode concurrentNode)
                {
                    IsCrossAccessing = true;
                    if (!concurrentNode.IsExpanded)
                    {
                        concurrentNode.Expand();
                    }
                    IsCrossAccessing = false;
                }
            }
        }
        private static void ReduceSubNodes(TreeNode node)
        {
            if (node is CAMTreeNode treeNode)
            {
                treeNode.ReduceSubNodes();
                if (!IsCrossAccessing && treeNode.ConcurrentNode is CAMTreeNode concurrentNode)
                {
                    IsCrossAccessing = true;
                    if (concurrentNode.IsExpanded)
                    {
                        concurrentNode.Collapse();
                    }
                    IsCrossAccessing = false;
                }
            }
        }
        private static void RefreshFileSelectorBox(ComboBox box, List<ComboBoxItem> availableFiles, ComboBoxItem otherSelectedItem, ComboBoxItem itemToSelect)
        {
            box.BeginUpdate();
            box.Items.Clear();
            box.Items.AddRange(availableFiles.Where(item => !item.Equals(otherSelectedItem)).ToArray());
            box.EndUpdate();
            box.SelectedItem = itemToSelect;
        }
        private static void RefreshFileSelectorBox(ComboBox box, List<ComboBoxItem> availableFiles, ComboBoxItem otherSelectedItem)
        {
            RefreshFileSelectorBox(box, availableFiles, otherSelectedItem, box.SelectedItem as ComboBoxItem);
        }
        private static void BuildTreeNodePair(CAMTreeNode node, List<CAMTreeNode> queryNodeList, List<CAMTreeNode> concurrentNodeList)
        {
            CAMTreeNode concurrentNode = new CAMTreeNode(node);
            concurrentNodeList.Insert(queryNodeList.IndexOf(node), concurrentNode);
            UpdateTreePairingData(node, concurrentNode);
        }
        private static void UpdateTreePairingData(CAMTreeNode node, CAMTreeNode concurrentNode)
        {
            node.UpdatePairingData(concurrentNode, CAMTreeNode.MergeActions.Add);
            concurrentNode.UpdatePairingData(node, CAMTreeNode.MergeActions.Remove);
            for (int index = 0; index < node.AllNodes.Count; index++)
            {
                UpdateTreePairingData(node.AllNodes[index], concurrentNode.AllNodes[index]);
            }
        }
        private static void RefreshTreeViews(TreeView treeView, List<CAMTreeNode> nodeList, int pageSize, int pageNumber)
        {
            treeView.BeginUpdate();
            treeView.Nodes.Clear();
            treeView.Nodes.AddRange(nodeList.Where(node => 
                node.MergeAction != CAMTreeNode.MergeActions.Ignore).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray());
            treeView.EndUpdate();
        }
        private static void RefreshSelectionTool(CompareSelectionTools selectionTool, List<CAMTreeNode> nodes)
        {
            bool anyAddChecked = false;
            bool anyAddUnchecked = false;
            bool anyUpdateChecked = false;
            bool anyUpdateUnchecked = false;
            bool anyRemoveChecked = false;
            bool anyRemoveUnchecked = false;

            ReadCheckStates(nodes, 
                ref anyAddChecked, ref anyAddUnchecked, ref anyUpdateChecked, 
                ref anyUpdateUnchecked, ref anyRemoveChecked, ref anyRemoveUnchecked);

            selectionTool.Reset(ConvertCheckStates(anyAddChecked, anyAddUnchecked), SelectionGroups.Add);
            selectionTool.Reset(ConvertCheckStates(anyUpdateChecked, anyUpdateUnchecked), SelectionGroups.Unequal);
            selectionTool.Reset(ConvertCheckStates(anyRemoveChecked, anyRemoveUnchecked), SelectionGroups.Remove);
        }
        private static void ReadCheckStates(List<CAMTreeNode> nodes, 
            ref bool anyAddChecked, ref bool anyAddUnchecked, ref bool anyUpdateChecked, 
            ref bool anyUpdateUnchecked, ref bool anyRemoveChecked, ref bool anyRemoveUnchecked)
        {
            foreach (CAMTreeNode node in nodes)
            {
                switch (node.MergeAction)
                {
                    case CAMTreeNode.MergeActions.Add:
                        if (node.Checked) { anyAddChecked = true; }
                        else { anyAddUnchecked = true; }
                        break;
                    case CAMTreeNode.MergeActions.Update:
                        if (node.Checked) { anyUpdateChecked = true; }
                        else { anyUpdateUnchecked = true; }
                        break;
                    case CAMTreeNode.MergeActions.Remove:
                        if (node.Checked) { anyRemoveChecked = true; }
                        else { anyRemoveUnchecked = true; }
                        break;
                    default: break;
                }
                ReadCheckStates(node.AllNodes, 
                    ref anyAddChecked, ref anyAddUnchecked, ref anyUpdateChecked, 
                    ref anyUpdateUnchecked, ref anyRemoveChecked, ref anyRemoveUnchecked);
            }
        }
        private static CheckState ConvertCheckStates(bool anyChecked, bool anyUnchecked)
        {
            if (anyChecked == anyUnchecked)
            {
                return CheckState.Indeterminate;
            }
            else if (anyChecked)
            {
                return CheckState.Unchecked;
            }
            else
            {
                return CheckState.Checked;
            }
        }
        private static void MergeBlock(EcfBlock targetBlock, List<CAMTreeNode> sourceList)
        {
            foreach (CAMTreeNode node in sourceList)
            {
                if (node.Checked)
                {
                    switch (node.MergeAction)
                    {
                        case CAMTreeNode.MergeActions.Add:
                            targetBlock.AddChild(CopyStructureItem(node.Item), sourceList.IndexOf(node));
                            break;
                        case CAMTreeNode.MergeActions.Update:
                            EcfStructureItem concurrentItem = node.ConcurrentNode.Item;
                            concurrentItem.UpdateContent(node.Item);
                            if (concurrentItem is EcfBlock block)
                            {
                                MergeBlock(block, node.AllNodes);
                            }
                            break;
                        case CAMTreeNode.MergeActions.Remove:
                            targetBlock.RemoveChild(node.Item);
                            break;
                        default: break;
                    }
                }
            }
        }
        private void UpdatePageIndicator(int pageNumber, int pageCount, int elementCount)
        {
            string pageText;
            if (pageNumber < 1 || pageCount < 1 || elementCount < 1)
            {
                pageText = string.Empty;
            }
            else
            {
                pageText = string.Format("{0} {1} {2} {3} - {4} {5}", TextRecources.Generic_Page, pageNumber, TextRecources.Generic_Of, pageCount, 
                    elementCount, TextRecources.EcfFileCAMDialog_RootElementsOverall);
            }
            FirstFileTreeBorderPanel.Text = pageText;
            SecondFileTreeBorderPanel.Text = pageText;
        }
        private static void UpdateDiffDetailsView(RichTextBox firstBox, RichTextBox secondBox, CAMTreeNode treeNode)
        {
            switch (treeNode?.MergeAction)
            {
                case CAMTreeNode.MergeActions.Add:
                    UpdateDiffDetailsText(firstBox, treeNode.Item);
                    secondBox.Text = string.Empty;
                    break;
                case CAMTreeNode.MergeActions.Update: 
                    UpdateDiffDetailsText(firstBox, secondBox, treeNode.Item, treeNode.ConcurrentNode.Item);
                    break;
                default:
                    firstBox.Text = string.Empty;
                    secondBox.Text = string.Empty;
                    break;
            }
        }
        private static void UpdateDiffDetailsText(RichTextBox detailBox, EcfStructureItem item)
        {
            detailBox.SuspendLayout();
            detailBox.Clear();

            if (item.Comments.Count > 0)
            {
                detailBox.AppendText(BuildCommentDiffDetails(item));
            }

            if (item is EcfParameter parameter && parameter.Attributes.Count > 0)
            {
                detailBox.AppendText(BuildAttributeDiffDetails(parameter));
            }

            if (item is EcfKeyValueItem kvItem && kvItem.ValueGroups.Count > 0)
            {
                detailBox.AppendText(BuildValueDiffDetails(kvItem));
            }

            detailBox.ResumeLayout();
        }
        private static void UpdateDiffDetailsText(RichTextBox firstBox, RichTextBox secondBox, EcfStructureItem primaryItem, EcfStructureItem secondaryItem)
        {
            firstBox.SuspendLayout();
            secondBox.SuspendLayout();
            firstBox.Clear();
            secondBox.Clear();

            if (!ValueListEquals(primaryItem.Comments, secondaryItem.Comments))
            {
                firstBox.AppendText(BuildCommentDiffDetails(primaryItem));
                secondBox.AppendText(BuildCommentDiffDetails(secondaryItem));
            }

            if (primaryItem is EcfParameter primaryParameter && secondaryItem is EcfParameter secondaryParameter && 
                !AttributeListEquals(primaryParameter.Attributes, secondaryParameter.Attributes))
            {
                firstBox.AppendText(BuildAttributeDiffDetails(primaryParameter));
                secondBox.AppendText(BuildAttributeDiffDetails(secondaryParameter));
            }

            if (primaryItem is EcfParameter primaryKVItem && secondaryItem is EcfParameter secondaryKVItem &&
                !ValueGroupListEquals(primaryKVItem.ValueGroups, secondaryKVItem.ValueGroups))
            {
                firstBox.AppendText(BuildValueDiffDetails(primaryKVItem));
                secondBox.AppendText(BuildValueDiffDetails(secondaryKVItem));
            }

            firstBox.ResumeLayout();
            secondBox.ResumeLayout();
        }
        private static string BuildCommentDiffDetails(EcfStructureItem item)
        {
            StringBuilder detailsText = new StringBuilder();

            detailsText.Append(TitleRecources.Generic_Comments);
            detailsText.Append(":");
            detailsText.Append(Environment.NewLine);
            foreach (string comment in item.Comments)
            {
                detailsText.Append("\t");
                detailsText.Append(comment);
                detailsText.Append(Environment.NewLine);
            }

            return detailsText.ToString();
        }
        private static string BuildAttributeDiffDetails(EcfParameter parameter)
        {
            StringBuilder detailsText = new StringBuilder();

            detailsText.Append(TitleRecources.Generic_Attributes);
            detailsText.Append(":");
            detailsText.Append(Environment.NewLine);
            foreach (EcfAttribute attribute in parameter.Attributes)
            {
                detailsText.Append("\t");
                detailsText.Append(attribute.Key);
                detailsText.Append(":");
                detailsText.Append(Environment.NewLine);
                foreach (EcfValueGroup group in attribute.ValueGroups)
                {
                    detailsText.Append("\t\t");
                    detailsText.Append(string.Join(", ", group.Values));
                    detailsText.Append(Environment.NewLine);
                }
            }

            return detailsText.ToString();
        }
        private static string BuildValueDiffDetails(EcfKeyValueItem kvItem)
        {
            StringBuilder detailsText = new StringBuilder();
            
            detailsText.Append(TitleRecources.Generic_Values);
            detailsText.Append(":");
            detailsText.Append(Environment.NewLine);
            foreach (EcfValueGroup group in kvItem.ValueGroups)
            {
                detailsText.Append("\t");
                detailsText.Append(string.Join(", ", group.Values));
                detailsText.Append(Environment.NewLine);
            }

            return detailsText.ToString();
        }

        // subclass
        private class ComboBoxItem : IComparable<ComboBoxItem>
        {
            public string DisplayText { get; }
            public EcfTabPage Item { get; }

            public ComboBoxItem(EcfTabPage item)
            {
                Item = item;
                DisplayText = item.File.FileName;
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
        private class CAMTreeNode : TreeNode
        {
            public MergeActions MergeAction { get; private set; } = MergeActions.Unknown;
            public CAMTreeNode ConcurrentNode { get; private set; } = null;
            public EcfStructureItem Item { get; } = null;
            public List<CAMTreeNode> AllNodes { get; } = new List<CAMTreeNode>();

            public enum MergeActions
            {
                Unknown,
                Ignore,
                Update,
                Add,
                Remove,
            }
            
            public CAMTreeNode(EcfStructureItem item, bool checkState)
            {
                Item = item;
                Checked = checkState;
                Text = BuildNodeText(item);
                SetMergeAction(MergeActions.Unknown);
            }
            public CAMTreeNode(CAMTreeNode template)
            {
                Item = template.Item;
                Checked = template.Checked;
                Text = template.Text;
                SetMergeAction(template.MergeAction);
                AllNodes.AddRange(template.AllNodes.Select(subNode => new CAMTreeNode(subNode)));
                ReduceSubNodes();
            }

            // publics
            public void UpdatePairingData(CAMTreeNode concurrentNode, MergeActions action)
            {
                ConcurrentNode = concurrentNode;
                UpdateMergeAction(action);
            }
            public void InflateSubNodes()
            {
                Nodes.Clear();
                Nodes.AddRange(AllNodes.Where(subNode => subNode.MergeAction != MergeActions.Ignore).ToArray());
            }
            public void ReduceSubNodes()
            {
                Nodes.Clear();
                if (AllNodes.Count > 0)
                {
                    Nodes.Add(new CAMTreeNode(null, false));
                }
            }
            public void UpdateCheckState(MergeActions? action, bool state)
            {
                if (MergeAction == action)
                {
                    SetState(state);
                    if (!state) { action = null; }
                }
                else if (action == null)
                {
                    switch (MergeAction)
                    {
                        case MergeActions.Update:
                        case MergeActions.Add:
                        case MergeActions.Remove:
                            SetState(state); break;
                        default:
                            SetState(false); break;
                    }
                }
                AllNodes.ForEach(node => node.UpdateCheckState(action, state));
            }

            // privates
            private void UpdateMergeAction(MergeActions action)
            {
                if (SetMergeAction(action))
                {
                    Checked = false;
                }
            }
            private bool SetMergeAction(MergeActions action)
            {
                MergeAction = action;
                switch (action)
                {
                    // "ignore" has no specific icon since ignored items should never be visible in the treeview
                    case MergeActions.Update: SetImage(0); return false;
                    case MergeActions.Add: SetImage(1); return false;
                    case MergeActions.Remove: SetImage(2); return false;
                    default: SetImage(3); return true;
                }
            }
            private void SetImage(int imageListIndex)
            {
                SelectedImageIndex = imageListIndex;
                ImageIndex = imageListIndex;
            }
            private void SetParentChecked()
            {
                if (Parent is CAMTreeNode parent)
                {
                    parent.Checked = true;
                    parent.SetParentChecked();
                }
            }
            private void SetState(bool state)
            {
                Checked = state;
                if (state) { SetParentChecked(); }
            }
            private string BuildNodeText(EcfStructureItem item)
            {
                switch (item)
                {
                    case EcfBlock block: return block.BuildRootId();
                    case EcfParameter param: return string.Format("{0} {1}", 
                        TitleRecources.Generic_Parameter, param.Key);
                    case EcfComment comment: return string.Format("{0} {1}", 
                        TitleRecources.Generic_Comment, comment.GetIndexInStructureLevel<EcfComment>().ToString());
                    default: return "---";
                }
            }
        }
    }
}

namespace EcfCAMTools
{
    public class CompareSelectionTools : EcfToolBox
    {
        public event EventHandler<SelectionChangeEventArgs> SelectionChangeClicked;

        private EcfToolBarThreeStateCheckBox ChangeAllAddItemsButton { get; } = new EcfToolBarThreeStateCheckBox(
            TextRecources.EcfFileCAMDialog_ToolTip_ChangeAllAddItems, IconRecources.Icon_SomeAddItemsSet, IconRecources.Icon_NoneAddItemsSet, IconRecources.Icon_AllAddItemsSet);
        private EcfToolBarThreeStateCheckBox ChangeAllUnequalItemsButton { get; } = new EcfToolBarThreeStateCheckBox(
            TextRecources.EcfFileCAMDialog_ToolTip_ChangeAllUnequalItems, IconRecources.Icon_SomeUnequalItemsSet, IconRecources.Icon_NoneUnequalItemsSet, IconRecources.Icon_AllUnequalItemsSet);
        private EcfToolBarThreeStateCheckBox ChangeAllRemoveItemsButton { get; } = new EcfToolBarThreeStateCheckBox(
            TextRecources.EcfFileCAMDialog_ToolTip_ChangeAllRemoveItems, IconRecources.Icon_SomeRemoveItemsSet, IconRecources.Icon_NoneRemoveItemsSet, IconRecources.Icon_AllRemoveItemsSet);

        public enum SelectionGroups
        {
            Unequal,
            Add,
            Remove,
        }
        public enum SelectionTypes
        {
            All,
            None,
        }

        public CompareSelectionTools() : base()
        {
            Add(ChangeAllAddItemsButton).Click += ChangeAllAddItemsButton_Click;
            Add(ChangeAllUnequalItemsButton).Click += ChangeAllUnequalItemsButton_Click;
            Add(ChangeAllRemoveItemsButton).Click += ChangeAllRemoveItemsButton_Click;
        }

        // events
        private void ChangeAllAddItemsButton_Click(object sender, EventArgs evt)
        {
            if (ChangeAllAddItemsButton.CheckState == CheckState.Indeterminate)
            {
                ChangeAllAddItemsButton.CheckState = CheckState.Unchecked;
            }
            SelectionChangeClicked?.Invoke(sender, new SelectionChangeEventArgs(
                SelectionGroups.Add, ChangeAllAddItemsButton.CheckState == CheckState.Unchecked ? SelectionTypes.All : SelectionTypes.None));
        }
        private void ChangeAllUnequalItemsButton_Click(object sender, EventArgs evt)
        {
            if (ChangeAllUnequalItemsButton.CheckState == CheckState.Indeterminate)
            {
                ChangeAllUnequalItemsButton.CheckState = CheckState.Unchecked;
                
            }
            SelectionChangeClicked?.Invoke(sender, new SelectionChangeEventArgs(
                SelectionGroups.Unequal, ChangeAllUnequalItemsButton.CheckState == CheckState.Unchecked ? SelectionTypes.All : SelectionTypes.None));
        }
        private void ChangeAllRemoveItemsButton_Click(object sender, EventArgs evt)
        {
            if (ChangeAllRemoveItemsButton.CheckState == CheckState.Indeterminate)
            {
                ChangeAllRemoveItemsButton.CheckState = CheckState.Unchecked;
            }
            SelectionChangeClicked?.Invoke(sender, new SelectionChangeEventArgs(
                SelectionGroups.Remove, ChangeAllRemoveItemsButton.CheckState == CheckState.Unchecked ? SelectionTypes.All : SelectionTypes.None));
        }

        // publics
        public void Reset()
        {
            Reset(CheckState.Unchecked);
        }
        public void Reset(CheckState state)
        {
            Reset(state, SelectionGroups.Add);
            Reset(state, SelectionGroups.Unequal);
            Reset(state, SelectionGroups.Remove);
        }
        public void Reset(CheckState state, SelectionGroups group)
        {
            switch (group)
            {
                case SelectionGroups.Add: ChangeAllAddItemsButton.CheckState = state; break;
                case SelectionGroups.Unequal: ChangeAllUnequalItemsButton.CheckState = state; break;
                case SelectionGroups.Remove: ChangeAllRemoveItemsButton.CheckState = state; break;
                default: break;
            }
        }

        // subclasses
        public class SelectionChangeEventArgs : EventArgs
        {
            public SelectionGroups Group { get; }
            public SelectionTypes Type { get; }

            public SelectionChangeEventArgs(SelectionGroups group, SelectionTypes type)
            {
                Group = group;
                Type = type;
            }
        }
    }
    public class MergeActionTools : EcfToolBox
    {
        public event EventHandler MergeToRightClicked;
        public event EventHandler MergeToLeftClicked;

        private EcfToolBarButton MergeToRightButton { get; } = new EcfToolBarButton(TextRecources.EcfFileCAMDialog_ToolTip_DoMergeRight, IconRecources.Icon_MoveRight, null);
        private EcfToolBarButton MergeToLeftButton { get; } = new EcfToolBarButton(TextRecources.EcfFileCAMDialog_ToolTip_DoMergeLeft, IconRecources.Icon_MoveLeft, null);
        
        public MergeActionTools() : base()
        {
            Add(MergeToRightButton);
            Add(MergeToLeftButton);
            
            MergeToRightButton.Click += (sender, evt) => MergeToRightClicked?.Invoke(sender, evt);
            MergeToLeftButton.Click += (sender, evt) => MergeToLeftClicked?.Invoke(sender, evt);
        }

        public void EnableMergeToRightButton(bool enabled)
        {
            MergeToRightButton.Enabled = enabled;
        }
        public void EnableMergeToLeftButton(bool enabled)
        {
            MergeToLeftButton.Enabled = enabled;
        }
    }
    public class DiffNavigationTools : EcfToolBox
    {
        public event EventHandler PageUpClicked;
        public event EventHandler PageDownClicked;

        private EcfToolBarButton PageUpButton { get; } = new EcfToolBarButton(TextRecources.EcfFileCAMDialog_ToolTip_PageUp, IconRecources.Icon_MoveUp, null);
        private EcfToolBarButton PageDownButton { get; } = new EcfToolBarButton(TextRecources.EcfFileCAMDialog_ToolTip_PageDown, IconRecources.Icon_MoveDown, null);

        public DiffNavigationTools() : base()
        {
            Add(PageUpButton);
            Add(PageDownButton);

            PageUpButton.Click += (sender, evt) => PageUpClicked?.Invoke(sender, evt);
            PageDownButton.Click += (sender, evt) => PageDownClicked?.Invoke(sender, evt);
        }

        public void UpdatePageButtons(int pageNumber, int pageCount)
        {
            if (pageNumber < 1 || pageCount < 1)
            {
                PageUpButton.Enabled = false;
                PageDownButton.Enabled = false;
            }
            else
            {
                PageUpButton.Enabled = pageNumber > 1;
                PageDownButton.Enabled = pageNumber < pageCount;
            }
        }
    }
}
