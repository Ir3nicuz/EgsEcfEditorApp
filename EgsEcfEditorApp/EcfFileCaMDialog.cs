using EcfCAMTools;
using EcfFileViews;
using EcfToolBarControls;
using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static EcfCAMTools.CompareSelectionTools;
using static Helpers.ImageAjustments;
using static EgsEcfParser.EcfStructureTools;

namespace EgsEcfEditorApp
{
    public partial class EcfFileCAMDialog : Form
    {
        public HashSet<EcfTabPage> ChangedFileTabs { get; } = new HashSet<EcfTabPage>();
        
        private CompareSelectionTools FirstFileSelectionTools { get; } = new CompareSelectionTools();
        private CompareSelectionTools SecondFileSelectionTools { get; } = new CompareSelectionTools();
        private MergeActionTools FirstFileActionTools { get; } = new MergeActionTools(IconRecources.Icon_MoveRight);
        private MergeActionTools SecondFileActionTools { get; } = new MergeActionTools(IconRecources.Icon_MoveLeft);

        private ImageList CAMListViewIcons { get; } = new ImageList();
        private List<ComboBoxItem> AvailableFileTabs { get; } = new List<ComboBoxItem>();

        private List<CAMTreeNode> FirstFileNodes { get; } = new List<CAMTreeNode>();
        private List<CAMTreeNode> SecondFileNodes { get; } = new List<CAMTreeNode>();
        private bool FirstFileCheckStateUpdate { get; set; } = false;
        private bool SecondFileCheckStateUpdate { get; set; } = false;

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

            CloseButton.Text = TitleRecources.Generic_Close;

            FirstFileTreeView.LinkTreeView(SecondFileTreeView);

            FirstFileSelectionContainer.Add(FirstFileSelectionTools);
            FirstFileActionContainer.Add(FirstFileActionTools);
            SecondFileActionContainer.Add(SecondFileActionTools);
            SecondFileSelectionContainer.Add(SecondFileSelectionTools);

            CAMListViewIcons.Images.Add(AddGap(IconRecources.Icon_Unequal, 16, 3, 1));
            CAMListViewIcons.Images.Add(AddGap(IconRecources.Icon_Add, 16, 3, 1));
            CAMListViewIcons.Images.Add(AddGap(IconRecources.Icon_Remove, 16, 3, 1));
            CAMListViewIcons.Images.Add(AddGap(IconRecources.Icon_Unknown, 16, 3, 1));

            FirstFileTreeView.ImageList = CAMListViewIcons;
            SecondFileTreeView.ImageList = CAMListViewIcons;

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

            FirstFileTreeView.AfterCheck += FirstFileTreeView_AfterCheck;
            SecondFileTreeView.AfterCheck += SecondFileTreeView_AfterCheck;

            FirstFileSelectionTools.SelectionChangeClicked += FirstFileSelectionTools_SelectionChangeClicked;
            SecondFileSelectionTools.SelectionChangeClicked += SecondFileSelectionTools_SelectionChangeClicked;

            FirstFileActionTools.DoMergeClicked += FirstFileActionTools_DoMergeClicked;
            SecondFileActionTools.DoMergeClicked += SecondFileActionTools_DoMergeClicked;
        }
        private void CloseButton_Click(object sender, EventArgs evt)
        {
            Close();
        }

        private void FirstFileSelectionTools_SelectionChangeClicked(object sender, SelectionChangeEventArgs evt)
        {
            FirstFileCheckStateUpdate = true;
            switch (evt.Group)
            {
                case SelectionGroups.Unequal: ChangeAllCheckStates(FirstFileNodes, CAMTreeNode.MergeActions.Update, evt.Type == SelectionTypes.All); break;
                case SelectionGroups.Add: ChangeAllCheckStates(FirstFileNodes, CAMTreeNode.MergeActions.Add, evt.Type == SelectionTypes.All); break;
                case SelectionGroups.Remove: ChangeAllCheckStates(FirstFileNodes, CAMTreeNode.MergeActions.Remove, evt.Type == SelectionTypes.All); break;
                default: break;
            }
            FirstFileCheckStateUpdate = false;
        }
        private void FirstFileComboBox_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            ComboBoxItem firstItem = FirstFileComboBox.SelectedItem as ComboBoxItem;
            RefreshFileSelectorBox(SecondFileComboBox, firstItem, AvailableFileTabs);
            CompareFiles(firstItem, SecondFileComboBox.SelectedItem as ComboBoxItem);
        }
        private void FirstFileTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs evt)
        {
            TransferExpandState(evt.Node);
        }
        private void FirstFileTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs evt)
        {
            InflateSubNodes(evt.Node);
        }
        private void FirstFileTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs evt)
        {
            ReduceSubNodes(evt.Node);
        }
        private void FirstFileTreeView_AfterCheck(object sender, TreeViewEventArgs evt)
        {
            if (!FirstFileCheckStateUpdate && evt.Node is CAMTreeNode treeNode)
            {
                FirstFileCheckStateUpdate = true;
                ChangeAllCheckStates(treeNode.AllNodes, null, treeNode.Checked);
                FirstFileCheckStateUpdate = false;
                FirstFileSelectionTools.ResetTo(CheckState.Indeterminate);
            }
        }
        private void FirstFileActionTools_DoMergeClicked(object sender, EventArgs evt)
        {
            MergeFiles(SecondFileComboBox.SelectedItem as ComboBoxItem, FirstFileNodes);
            CompareFiles(FirstFileComboBox.SelectedItem as ComboBoxItem, SecondFileComboBox.SelectedItem as ComboBoxItem);
        }

        private void SecondFileSelectionTools_SelectionChangeClicked(object sender, SelectionChangeEventArgs evt)
        {
            SecondFileCheckStateUpdate = true;
            switch (evt.Group)
            {
                case SelectionGroups.Unequal: ChangeAllCheckStates(SecondFileNodes, CAMTreeNode.MergeActions.Update, evt.Type == SelectionTypes.All); break;
                case SelectionGroups.Add: ChangeAllCheckStates(SecondFileNodes, CAMTreeNode.MergeActions.Add, evt.Type == SelectionTypes.All); break;
                case SelectionGroups.Remove: ChangeAllCheckStates(SecondFileNodes, CAMTreeNode.MergeActions.Remove, evt.Type == SelectionTypes.All); break;
                default: break;
            }
            SecondFileCheckStateUpdate = false;
        }
        private void SecondFileComboBox_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            ComboBoxItem secondItem = SecondFileComboBox.SelectedItem as ComboBoxItem;
            RefreshFileSelectorBox(FirstFileComboBox, secondItem, AvailableFileTabs);
            CompareFiles(FirstFileComboBox.SelectedItem as ComboBoxItem, secondItem);
        }
        private void SecondFileTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs evt)
        {
            TransferExpandState(evt.Node);
            
        }
        private void SecondFileTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs evt)
        {
            InflateSubNodes(evt.Node);
        }
        private void SecondFileTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs evt)
        {
            ReduceSubNodes(evt.Node);
        }
        private void SecondFileTreeView_AfterCheck(object sender, TreeViewEventArgs evt)
        {
            if (!SecondFileCheckStateUpdate && evt.Node is CAMTreeNode treeNode)
            {
                SecondFileCheckStateUpdate = true;
                ChangeAllCheckStates(treeNode.AllNodes, null, treeNode.Checked);
                SecondFileCheckStateUpdate = false;
                SecondFileSelectionTools.ResetTo(CheckState.Indeterminate);
            }
        }
        private void SecondFileActionTools_DoMergeClicked(object sender, EventArgs evt)
        {
            MergeFiles(FirstFileComboBox.SelectedItem as ComboBoxItem, SecondFileNodes);
            CompareFiles(FirstFileComboBox.SelectedItem as ComboBoxItem, SecondFileComboBox.SelectedItem as ComboBoxItem);
        }

        // public
        public DialogResult ShowDialog(IWin32Window parent, List<EcfTabPage> openedFileTabs)
        {
            ChangedFileTabs.Clear();
            AvailableFileTabs.Clear();
            AvailableFileTabs.AddRange(openedFileTabs.Select(tab => new ComboBoxItem(tab)));
            RefreshFileSelectorBox(FirstFileComboBox, null, AvailableFileTabs);
            RefreshFileSelectorBox(SecondFileComboBox, null, AvailableFileTabs);
            CompareFiles(null, null);
            return ShowDialog(parent);
        }

        // private
        private void CompareFiles(ComboBoxItem firstItem, ComboBoxItem secondItem)
        {
            if (firstItem == null || secondItem == null) {
                FirstFileTreeView.Nodes.Clear();
                SecondFileTreeView.Nodes.Clear();
                return;
            }

            CheckState initState = CheckState.Checked;
            RefreshTreeNodeList(FirstFileNodes, firstItem.Item.File.ItemList, initState == CheckState.Checked);
            RefreshTreeNodeList(SecondFileNodes, secondItem.Item.File.ItemList, initState == CheckState.Checked);
            CompareTreeNodeLists(FirstFileNodes, SecondFileNodes);
            RefreshTreeViews(FirstFileTreeView, FirstFileNodes);
            RefreshTreeViews(SecondFileTreeView, SecondFileNodes);

            FirstFileSelectionTools.ResetTo(initState);
            SecondFileSelectionTools.ResetTo(initState);
        }
        private static void ChangeAllCheckStates(List<CAMTreeNode> nodes, CAMTreeNode.MergeActions? action, bool state)
        {
            foreach(CAMTreeNode node in nodes)
            {
                if (node.MergeAction == action)
                {
                    node.Checked = state;
                }
                else if (action == null)
                {
                    switch (node.MergeAction)
                    {
                        case CAMTreeNode.MergeActions.Update:
                        case CAMTreeNode.MergeActions.Add:
                        case CAMTreeNode.MergeActions.Remove:
                            node.Checked = state;
                            break;
                        default:
                            node.Checked = false;
                            break;
                    }
                }
                ChangeAllCheckStates(node.AllNodes, action, state);
            }
        }
        private static void TransferExpandState(TreeNode node)
        {
            if (node is CAMTreeNode treeNode)
            {
                if (treeNode.IsExpanded)
                {
                    treeNode.ConcurrentNode?.Expand();
                }
                else
                {
                    treeNode.ConcurrentNode?.Collapse();
                }
            }
        }
        private static void InflateSubNodes(TreeNode node)
        {
            if (node is CAMTreeNode treeNode)
            {
                treeNode.InflateSubNodes();
            }
        }
        private static void ReduceSubNodes(TreeNode node)
        {
            if (node is CAMTreeNode treeNode)
            {
                treeNode.ReduceSubNodes();
            }
        }
        private static void RefreshFileSelectorBox(ComboBox box, ComboBoxItem otherSelectedItem, List<ComboBoxItem> availableFiles)
        {
            ComboBoxItem selectedItem = box.SelectedItem as ComboBoxItem;
            box.BeginUpdate();
            box.Items.Clear();
            box.Items.AddRange(availableFiles.Where(item => !item.Equals(otherSelectedItem)).ToArray());
            box.EndUpdate();
            box.SelectedItem = selectedItem;
        }
        private static void RefreshTreeNodeList(List<CAMTreeNode> nodeList, ReadOnlyCollection<EcfStructureItem> itemList, bool initState)
        {
            nodeList.Clear();
            foreach (EcfStructureItem item in itemList)
            {
                CAMTreeNode node = new CAMTreeNode(item, initState);
                nodeList.Add(node);
                if (item is EcfBlock block)
                {
                    RefreshTreeNodeList(node.AllNodes, block.ChildItems, initState);
                    node.ReduceSubNodes();
                }
            }
        }
        private static void CompareTreeNodeLists(List<CAMTreeNode> firstNodeList, List<CAMTreeNode> secondNodeList)
        {
            CAMTreeNode concurrentNode;
            firstNodeList.ForEach(node =>
            {
                concurrentNode = secondNodeList.FirstOrDefault(otherNode => 
                    otherNode.MergeAction == CAMTreeNode.MergeActions.Unknown && StructureItemIdEquals(node.Item, otherNode.Item));
                if (concurrentNode == null)
                {
                    concurrentNode = new CAMTreeNode(null, node.Checked);
                    node.UpdatePairingData(concurrentNode, CAMTreeNode.MergeActions.Add, true);
                    concurrentNode.UpdatePairingData(node, CAMTreeNode.MergeActions.Remove, false);
                }
                else
                {
                    CompareTreeNodeLists(node.AllNodes, concurrentNode.AllNodes);
                    if (node.AllNodes.Any(subnode => subnode.MergeAction != CAMTreeNode.MergeActions.Ignore) ||
                        concurrentNode.AllNodes.Any(subnode => subnode.MergeAction != CAMTreeNode.MergeActions.Ignore) ||
                        !node.Item.ContentEquals(concurrentNode.Item, false))
                    {
                        node.UpdatePairingData(concurrentNode, CAMTreeNode.MergeActions.Update, false);
                        concurrentNode.UpdatePairingData(node, CAMTreeNode.MergeActions.Update, false);
                    }
                    else
                    {
                        node.UpdatePairingData(concurrentNode, CAMTreeNode.MergeActions.Ignore, false);
                        concurrentNode.UpdatePairingData(node, CAMTreeNode.MergeActions.Ignore, false);
                    }
                }
            });

            secondNodeList.ForEach(node =>
            {
                if (node.MergeAction == CAMTreeNode.MergeActions.Unknown)
                {
                    concurrentNode = new CAMTreeNode(null, node.Checked);
                    node.UpdatePairingData(concurrentNode, CAMTreeNode.MergeActions.Add, true);
                    concurrentNode.UpdatePairingData(node, CAMTreeNode.MergeActions.Remove, false);
                }
            });

            

            // find insert index for add verdict? -> add removing node to second list???

            // upwards / downswards check inherittance

        }
        private static void RefreshTreeViews(TreeView treeView, List<CAMTreeNode> nodeList)
        {
            treeView.BeginUpdate();
            treeView.Nodes.Clear();
            treeView.Nodes.AddRange(nodeList.Where(node => node.MergeAction != CAMTreeNode.MergeActions.Ignore).ToArray());
            treeView.EndUpdate();
        }
        [Obsolete("needs work")]
        private void MergeFiles(ComboBoxItem targetFile, List<CAMTreeNode> sourceNodes)
        {
            if (sourceNodes.Count == 0 || targetFile == null) { return; }





            ChangedFileTabs.Add(targetFile.Item);
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
                Text = item?.BuildIdentification() ?? "---";
                SetMergeAction(MergeActions.Unknown);
            }

            // publics
            public void UpdatePairingData(CAMTreeNode concurrentNode, MergeActions action, bool inheritAction)
            {
                ConcurrentNode = concurrentNode;
                UpdateMergeAction(action);
                if (inheritAction)
                {
                    InheritMergeAction(AllNodes, action);
                }
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

            // privates
            private void InheritMergeAction(List<CAMTreeNode> subNodes, MergeActions action)
            {
                subNodes.ForEach(node =>
                {
                    node.UpdateMergeAction(action);
                    InheritMergeAction(node.AllNodes, action);
                });
            }
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
        }
    }
}

namespace EcfCAMTools
{
    public class CompareSelectionTools : EcfToolBox
    {
        public event EventHandler<SelectionChangeEventArgs> SelectionChangeClicked;

        private EcfToolBarThreeStateCheckBox ChangeAllAddItemsButton { get; } = new EcfToolBarThreeStateCheckBox(
            TextRecources.EcfFileCAMDialog_ToolTip_ChangeAllAddItems, IconRecources.Icon_SomeAddItemsSet, IconRecources.Icon_AllAddItemsSet, IconRecources.Icon_NoneAddItemsSet);
        private EcfToolBarThreeStateCheckBox ChangeAllUnequalItemsButton { get; } = new EcfToolBarThreeStateCheckBox(
            TextRecources.EcfFileCAMDialog_ToolTip_ChangeAllUnequalItems, IconRecources.Icon_SomeUnequalItemsSet, IconRecources.Icon_AllUnequalItemsSet, IconRecources.Icon_NoneUnequalItemsSet);
        private EcfToolBarThreeStateCheckBox ChangeAllRemoveItemsButton { get; } = new EcfToolBarThreeStateCheckBox(
            TextRecources.EcfFileCAMDialog_ToolTip_ChangeAllRemoveItems, IconRecources.Icon_SomeRemoveItemsSet, IconRecources.Icon_AllRemoveItemsSet, IconRecources.Icon_NoneRemoveItemsSet);

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
                SelectionGroups.Add, ChangeAllAddItemsButton.CheckState == CheckState.Checked ? SelectionTypes.All : SelectionTypes.None));
        }
        private void ChangeAllUnequalItemsButton_Click(object sender, EventArgs evt)
        {
            if (ChangeAllUnequalItemsButton.CheckState == CheckState.Indeterminate)
            {
                ChangeAllUnequalItemsButton.CheckState = CheckState.Unchecked;
                
            }
            SelectionChangeClicked?.Invoke(sender, new SelectionChangeEventArgs(
                SelectionGroups.Unequal, ChangeAllUnequalItemsButton.CheckState == CheckState.Checked ? SelectionTypes.All : SelectionTypes.None));
        }
        private void ChangeAllRemoveItemsButton_Click(object sender, EventArgs evt)
        {
            if (ChangeAllRemoveItemsButton.CheckState == CheckState.Indeterminate)
            {
                ChangeAllRemoveItemsButton.CheckState = CheckState.Unchecked;
            }
            SelectionChangeClicked?.Invoke(sender, new SelectionChangeEventArgs(
                SelectionGroups.Remove, ChangeAllRemoveItemsButton.CheckState == CheckState.Checked ? SelectionTypes.All : SelectionTypes.None));
        }

        // publics
        public void SetAllAddsState(CheckState state)
        {
            ChangeAllAddItemsButton.CheckState = state;
        }
        public void SetAllUpdatesButton(CheckState state)
        {
            ChangeAllUnequalItemsButton.CheckState = state;
        }
        public void SetAllRemovesButton(CheckState state)
        {
            ChangeAllRemoveItemsButton.CheckState = state;
        }
        public void Reset()
        {
            ResetTo(CheckState.Checked);
        }
        public void ResetTo(CheckState state)
        {
            SetAllAddsState(state);
            SetAllUpdatesButton(state);
            SetAllRemovesButton(state);
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
        public event EventHandler DoMergeClicked;

        public MergeActionTools(Image doMergeIcon) : base()
        {
            Add(new EcfToolBarButton(TextRecources.EcfFileCAMDialog_ToolTip_DoMerge, doMergeIcon, null)).Click += (sender, evt) => DoMergeClicked?.Invoke(sender, evt);
        }
    }
}
