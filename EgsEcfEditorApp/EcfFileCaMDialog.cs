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
        private static bool IsCrossAccessing { get; set; } = false;

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

            FirstFileSelectionTools.SelectionChangeClicked += FirstFileSelectionTools_SelectionChangeClicked;
            SecondFileSelectionTools.SelectionChangeClicked += SecondFileSelectionTools_SelectionChangeClicked;

            FirstFileActionTools.DoMergeClicked += FirstFileActionTools_DoMergeClicked;
            SecondFileActionTools.DoMergeClicked += SecondFileActionTools_DoMergeClicked;
        }
        private void CloseButton_Click(object sender, EventArgs evt)
        {
            Close();
        }

        private void FirstFileTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs evt)
        {
            if (evt.Node is CAMTreeNode treeNode)
            {
                treeNode.UpdateCheckState(null, treeNode.Checked);
                RefreshSelectionTool(FirstFileSelectionTools, FirstFileNodes);
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
            ComboBoxItem firstItem = FirstFileComboBox.SelectedItem as ComboBoxItem;
            RefreshFileSelectorBox(SecondFileComboBox, firstItem, AvailableFileTabs);
            CompareFiles(firstItem, SecondFileComboBox.SelectedItem as ComboBoxItem);
        }
        private void FirstFileTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs evt)
        {
            InflateSubNodes(evt.Node);

        }
        private void FirstFileTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs evt)
        {
            ReduceSubNodes(evt.Node);
        }
        private void FirstFileActionTools_DoMergeClicked(object sender, EventArgs evt)
        {
            MergeFiles(SecondFileComboBox.SelectedItem as ComboBoxItem, FirstFileNodes);
            CompareFiles(FirstFileComboBox.SelectedItem as ComboBoxItem, SecondFileComboBox.SelectedItem as ComboBoxItem);
        }

        private void SecondFileTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs evt)
        {
            if (evt.Node is CAMTreeNode treeNode)
            {
                treeNode.UpdateCheckState(null, treeNode.Checked);
                RefreshSelectionTool(SecondFileSelectionTools, SecondFileNodes);
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
            ComboBoxItem secondItem = SecondFileComboBox.SelectedItem as ComboBoxItem;
            RefreshFileSelectorBox(FirstFileComboBox, secondItem, AvailableFileTabs);
            CompareFiles(FirstFileComboBox.SelectedItem as ComboBoxItem, secondItem);
        }
        private void SecondFileTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs evt)
        {
            InflateSubNodes(evt.Node);
        }
        private void SecondFileTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs evt)
        {
            ReduceSubNodes(evt.Node);
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
            CompareFiles(FirstFileComboBox.SelectedItem as ComboBoxItem, SecondFileComboBox.SelectedItem as ComboBoxItem);

            return ShowDialog(parent);
        }

        // private
        private void CompareFiles(ComboBoxItem firstItem, ComboBoxItem secondItem)
        {
            if (firstItem == null || secondItem == null) {
                FirstFileTreeView.Nodes.Clear();
                SecondFileTreeView.Nodes.Clear();
                FirstFileSelectionTools.Reset(CheckState.Indeterminate);
                SecondFileSelectionTools.Reset(CheckState.Indeterminate);
                return;
            }

            RefreshTreeNodeList(FirstFileNodes, firstItem.Item.File.ItemList, true);
            RefreshTreeNodeList(SecondFileNodes, secondItem.Item.File.ItemList, true);
            CompareTreeNodeLists(FirstFileNodes, SecondFileNodes);
            RefreshTreeViews(FirstFileTreeView, FirstFileNodes);
            RefreshTreeViews(SecondFileTreeView, SecondFileNodes);
            RefreshSelectionTool(FirstFileSelectionTools, FirstFileNodes);
            RefreshSelectionTool(SecondFileSelectionTools, SecondFileNodes);
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
                    concurrentNode = new CAMTreeNode(node);
                    node.UpdatePairingData(concurrentNode, CAMTreeNode.MergeActions.Add, true);
                    concurrentNode.UpdatePairingData(node, CAMTreeNode.MergeActions.Remove, true);
                    secondNodeList.Insert(firstNodeList.IndexOf(node), concurrentNode);
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
                    concurrentNode = new CAMTreeNode(node);
                    node.UpdatePairingData(concurrentNode, CAMTreeNode.MergeActions.Add, true);
                    concurrentNode.UpdatePairingData(node, CAMTreeNode.MergeActions.Remove, true);
                    firstNodeList.Insert(secondNodeList.IndexOf(node), concurrentNode);
                }
            });





            // find file insert index for add verdict


            // window init qol optimization


            // id text / update content display


        }
        private static void RefreshTreeViews(TreeView treeView, List<CAMTreeNode> nodeList)
        {
            treeView.BeginUpdate();
            treeView.Nodes.Clear();
            treeView.Nodes.AddRange(nodeList.Where(node => node.MergeAction != CAMTreeNode.MergeActions.Ignore).ToArray());
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

            selectionTool.Reset(ConverCheckStates(anyAddChecked, anyAddUnchecked), SelectionGroups.Add);
            selectionTool.Reset(ConverCheckStates(anyUpdateChecked, anyUpdateUnchecked), SelectionGroups.Unequal);
            selectionTool.Reset(ConverCheckStates(anyRemoveChecked, anyRemoveUnchecked), SelectionGroups.Remove);
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
        private static CheckState ConverCheckStates(bool anyChecked, bool anyUnchecked)
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
        public event EventHandler DoMergeClicked;

        public MergeActionTools(Image doMergeIcon) : base()
        {
            Add(new EcfToolBarButton(TextRecources.EcfFileCAMDialog_ToolTip_DoMerge, doMergeIcon, null)).Click += (sender, evt) => DoMergeClicked?.Invoke(sender, evt);
        }
    }
}
