using EgsEcfParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static EgsEcfControls.CheckComboBox;
using static EgsEcfControls.EcfBaseView;
using static EgsEcfControls.EcfSorter;
using static EgsEcfControls.ProgressIndicator;

namespace EgsEcfControls
{
    public class EcfTabPage : TabPage
    {
        public event EventHandler AnyViewResized;

        public EgsEcfFile File { get; }

        private EcfToolContainer ToolContainer { get; } = new EcfToolContainer();

        private EcfDataIndicator Indicator { get; } = null;
        private EcfFilterControl FilterControl { get; } = null;
        private EcfTreeFilter TreeFilter { get; } = null;
        private EcfParameterFilter ParameterFilter { get; } = null;

        private EcfFileContainer FileViewPanel { get; } = null;
        private EcfTreeView TreeView { get; } = null;
        private EcfParameterView ParameterView { get; } = null;
        private EcfErrorView ErrorView { get; } = null;
        private EcfInfoView InfoView { get; } = null;

        private BackgroundWorker UpdateAllViewsWorker { get; } = new BackgroundWorker();
        private BackgroundWorker UpdateErrorViewWorker { get; } = new BackgroundWorker();

        public bool IsUpdating { get; private set; } = false;
        public bool IsAnyViewUpdating { get; private set; } = false;

        public EcfTabPage(EgsEcfFile file) : base()
        {
            File = file;

            Indicator = new EcfDataIndicator(File.Definition.FileType);
            FilterControl = new EcfFilterControl();
            TreeFilter = new EcfTreeFilter(File.Definition.RootBlockAttributes.Select(item => item.Name).Concat(
                File.Definition.ChildBlockAttributes.Select(item => item.Name)).ToList());
            ParameterFilter = new EcfParameterFilter(File.Definition.BlockParameters.Select(item => item.Name).ToList());

            FileViewPanel = new EcfFileContainer();
            TreeView = new EcfTreeView(Properties.titles.TreeView_Header, File, ResizeableBorders.RightBorder);
            ParameterView = new EcfParameterView(Properties.titles.ParameterView_Header, File, ResizeableBorders.None);
            ErrorView = new EcfErrorView(Properties.titles.ErrorView_Header, File, ResizeableBorders.TopBorder);
            InfoView = new EcfInfoView(Properties.titles.InfoView_Header, File, ResizeableBorders.LeftBorder);

            ToolContainer.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            TreeView.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            ParameterView.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            InfoView.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            ErrorView.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            
            ToolContainer.Dock = DockStyle.Top;
            TreeView.Dock = DockStyle.Left;
            ParameterView.Dock = DockStyle.Fill;
            InfoView.Dock = DockStyle.Right;
            ErrorView.Dock = DockStyle.Bottom;

            Controls.Add(FileViewPanel);
            FileViewPanel.Add(ParameterView);
            FileViewPanel.Add(ErrorView);
            FileViewPanel.Add(InfoView);
            FileViewPanel.Add(TreeView);

            FileViewPanel.Add(ToolContainer);
            ToolContainer.Add(Indicator);
            ToolContainer.Add(FilterControl);
            ToolContainer.Add(TreeFilter);
            ToolContainer.Add(ParameterFilter);

            FilterControl.ApplyFilter += OnApplyFilter;
            FilterControl.ResetFilter += OnResetFilter;
            TreeFilter.ApplyFilter += OnApplyFilter;
            ParameterFilter.ApplyFilter += OnApplyFilter;

            TreeView.ViewResized += OnAnyViewResized;
            ParameterView.ViewResized += OnAnyViewResized;
            ErrorView.ViewResized += OnAnyViewResized;
            InfoView.ViewResized += OnAnyViewResized;

            FileViewPanel.ViewUpdateStateChanged += FileViewPanel_ViewUpdateStateChanged;
            TreeView.EcfItemSelected += TreeView_EcfItemSelected;
            ParameterView.EcfParameterSelected += ParameterView_EcfParameterSelected;

            UpdateAllViewsWorker.DoWork += UpdateAllViewsWorker_DoWork;
            UpdateAllViewsWorker.RunWorkerCompleted += UpdateAllViewsWorker_RunWorkerCompleted;
            UpdateErrorViewWorker.DoWork += UpdateErrorViewWorker_DoWork;
            UpdateErrorViewWorker.RunWorkerCompleted += UpdateErrorViewWorker_RunWorkerCompleted;

            UpdateTabDescription();
            UpdateAllViewsAsync();
        }

        public void UpdateAllViewsAsync()
        {
            if (IsUpdating)
            {
                throw new InvalidOperationException("Update already running");
            }
            UpdateAllViewsWorker.RunWorkerAsync();
        }
        public void UpdateTabDescription()
        {
            Text = string.Format("{0}{1}", File.FileName, File.HasUnsavedData ? " *" : "");
            ToolTipText = Path.Combine(File.FilePath, File.FileName);
        }
        public void UpdateErrorViewAsync()
        {
            if (IsUpdating)
            {
                throw new InvalidOperationException("Update already running");
            }
            UpdateErrorViewWorker.RunWorkerAsync();
        }
        public void InitViewSizes(int treeViewWidth, int errorViewHeight, int infoViewWidth)
        {
            TreeView.Width = treeViewWidth;
            ErrorView.Height = errorViewHeight;
            InfoView.Width = infoViewWidth;
        }

        private void UpdateAllViewsWorker_DoWork(object sender, DoWorkEventArgs evt)
        {
            UpdateAllViews();
        }
        private void UpdateAllViewsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs evt)
        {
            if (evt.Error != null)
            {
                SetContainerUpdateState(false);
                MessageBox.Show(this, string.Format("{0}:{1}{1}{2}", Properties.texts.BackgrounfWorker_Failed, Environment.NewLine, evt.Error.ToString()), 
                    Properties.titles.GenericAttention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void TreeView_EcfItemSelected(object sender, EventArgs e)
        {
            UpdateParameterView();
        }
        private void ParameterView_EcfParameterSelected(object sender, EventArgs e)
        {
            UpdateInfoView();
        }
        private void UpdateErrorViewWorker_DoWork(object sender, DoWorkEventArgs evt)
        {
            UpdateErrorView();
        }
        private void UpdateErrorViewWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs evt)
        {
            if (evt.Error != null)
            {
                SetContainerUpdateState(false);
                MessageBox.Show(this, string.Format("{0}:{1}{1}{2}", Properties.texts.BackgrounfWorker_Failed, Environment.NewLine, evt.Error.ToString()),
                    Properties.titles.GenericAttention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void FileViewPanel_ViewUpdateStateChanged(object sender, EventArgs evt)
        {
            TryUpdateStateInvoke();
        }

        private void UpdateAllViews()
        {
            SetContainerUpdateState(true);
            TreeView.UpdateView(TreeFilter, ParameterFilter);
            ParameterView.UpdateView(ParameterFilter, TreeView.SelectedEcfItem);
            InfoView.UpdateView(TreeView.SelectedEcfItem, ParameterView.SelectedEcfParameter);
            ErrorView.UpdateView();
            SetContainerUpdateState(false);
        }
        private void UpdateParameterView()
        {
            SetContainerUpdateState(true);
            ParameterView.UpdateView(ParameterFilter, TreeView.SelectedEcfItem);
            InfoView.UpdateView(TreeView.SelectedEcfItem, ParameterView.SelectedEcfParameter);
            SetContainerUpdateState(false);
        }
        private void UpdateInfoView()
        {
            SetContainerUpdateState(true);
            InfoView.UpdateView(ParameterView.SelectedEcfParameter);
            SetContainerUpdateState(false);
        }
        private void UpdateErrorView()
        {
            SetContainerUpdateState(true);
            ErrorView.UpdateView();
            SetContainerUpdateState(false);
        }
        private void OnApplyFilter(object sender, EventArgs evt)
        {
            UpdateAllViewsAsync();
        }
        private void OnResetFilter(object sender, EventArgs evt)
        {
            TreeFilter.Reset();
            ParameterFilter.Reset();
            UpdateAllViewsAsync();
        }
        private void OnAnyViewResized(object sender, EventArgs evt)
        {
            AnyViewResized?.Invoke(sender, evt);
        }
        private void SetContainerUpdateState(bool state)
        {
            IsUpdating = state;
            ShowUpdateState(state);
        }
        private void ShowUpdateState(bool state)
        {
            if (state)
            {
                Indicator.Activate();
            }
            else
            {
                Indicator.Deactivate();
            }
        }
        private void TryUpdateStateInvoke()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    TryUpdateState();
                });
            }
            else
            {
                TryUpdateState();
            }
        }
        private void TryUpdateState()
        {
            if (!IsUpdating)
            {
                ShowUpdateState(FileViewPanel.IsAnyViewUpdating());
            }
        }
    }
    
    public class EcfFileContainer : Panel
    {
        public event EventHandler ViewUpdateStateChanged;

        public EcfFileContainer() : base()
        {
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            Dock = DockStyle.Fill;
        }

        public void Add(Control view)
        {
            Controls.Add(view);
        }
        public bool IsAnyViewUpdating()
        {
            return Controls.Cast<Control>().Where(control => control is EcfBaseView).Cast<EcfBaseView>().Any(view => view.IsUpdating);
        }
        public void OnViewUpdateStateChanged(EcfBaseView sender)
        {
            ViewUpdateStateChanged?.Invoke(sender, null);
        }
    }
    public abstract class EcfBaseView : GroupBox
    {
        public event EventHandler ViewResized;

        protected EgsEcfFile File { get; } = null;
        private ResizeableBorders ResizeableBorder { get; } = ResizeableBorders.None;
        private ResizeableBorders DraggedBorder { get; set; } = ResizeableBorders.None;
        private int GrapSize { get; } = 0;
        protected bool IsDragged { get; private set; } = false;
        public string ViewName { get; } = string.Empty;
        public bool IsUpdating { get; private set; } = false;

        protected EcfBaseView(string headline, EgsEcfFile file, ResizeableBorders borderMode)
        {
            ViewName = headline;
            Text = headline;
            File = file;
            ResizeableBorder = borderMode;
            GrapSize = Width - DisplayRectangle.Width;
        }

        private ResizeableBorders IsInDragArea(MouseEventArgs evt)
        {
            switch (ResizeableBorder)
            {
                case ResizeableBorders.LeftBorder: return IsOnLeftBorder(evt) ? ResizeableBorders.LeftBorder : ResizeableBorders.None;
                case ResizeableBorders.RightBorder: return IsOnRightBorder(evt) ? ResizeableBorders.RightBorder : ResizeableBorders.None;
                case ResizeableBorders.TopBorder: return IsOnTopBorder(evt) ? ResizeableBorders.TopBorder : ResizeableBorders.None;
                case ResizeableBorders.BottomBorder: return IsOnBottomBorder(evt) ? ResizeableBorders.BottomBorder : ResizeableBorders.None;
                default: return ResizeableBorders.None;
            }
        }
        private bool IsOnLeftBorder(MouseEventArgs evt)
        {
            return evt.X > (0 - GrapSize) && evt.X < GrapSize;
        }
        private bool IsOnRightBorder(MouseEventArgs evt)
        {
            return evt.X > (Width - GrapSize) && evt.X < Width;
        }
        private bool IsOnTopBorder(MouseEventArgs evt)
        {
            return evt.Y > (0 - GrapSize) && evt.Y < GrapSize;
        }
        private bool IsOnBottomBorder(MouseEventArgs evt)
        {
            return evt.Y > (Height - GrapSize) && evt.Y < Height;
        }
        private void ResizeBounds(MouseEventArgs evt)
        {
            switch (DraggedBorder)
            {
                case ResizeableBorders.LeftBorder: Width -= PointToClient(Cursor.Position).X; break;
                case ResizeableBorders.RightBorder: Width = evt.X; break;
                case ResizeableBorders.TopBorder: Height -= PointToClient(Cursor.Position).Y; break;
                case ResizeableBorders.BottomBorder: Height = evt.Y; break;
                default: break;
            }
        }
        private void UpdateCursor(MouseEventArgs evt)
        {
            switch (IsInDragArea(evt))
            {
                case ResizeableBorders.LeftBorder:
                case ResizeableBorders.RightBorder:
                    RefreshCursor(Cursors.SizeWE);
                    break;
                case ResizeableBorders.TopBorder:
                case ResizeableBorders.BottomBorder:
                    RefreshCursor(Cursors.SizeNS);
                    break;
                default:
                    RefreshCursor(DefaultCursor);
                    break;
            }
        }
        private void RefreshCursor(Cursor cursor)
        {
            if (Cursor != cursor)
            {
                Cursor = cursor;
            }
        }

        protected override void OnMouseDown(MouseEventArgs evt)
        {
            if (evt.Button == MouseButtons.Left)
            {
                DraggedBorder = IsInDragArea(evt);
                if (DraggedBorder != ResizeableBorders.None)
                {
                    IsDragged = true;
                }
            }
        }
        protected override void OnMouseMove(MouseEventArgs evt)
        {
            if (IsDragged)
            {
                ResizeBounds(evt);
            }
            else
            {
                UpdateCursor(evt);
            }
        }
        protected override void OnMouseUp(MouseEventArgs evt)
        {
            if (IsDragged)
            {
                ViewResized?.Invoke(this, null);
            }
            IsDragged = false;
        }
        protected override void OnMouseLeave(EventArgs evt)
        {
            if (IsDragged)
            {
                ViewResized?.Invoke(this, null);
            }
            IsDragged = false;
            RefreshCursor(DefaultCursor);
        }
        protected void ChangeViewUpdateState(bool state)
        {
            IsUpdating = state;
            if (Parent is EcfFileContainer container)
            {
                container.OnViewUpdateStateChanged(this);
            }
        }

        protected bool IsTreeNodeLike(string treeNodeName, string isLike)
        {
            if (string.IsNullOrEmpty(isLike)) { return true; }
            if (string.IsNullOrEmpty(treeNodeName)) { return false; }
            return treeNodeName.Contains(isLike);
        }
        protected bool IsParameterValueLike(ReadOnlyCollection<string> values, string isLike)
        {
            if (string.IsNullOrEmpty(isLike) || values.Count < 1) { return true; }
            return values.Any(value => value.Contains(isLike));
        }

        public enum ResizeableBorders
        {
            None,
            LeftBorder,
            RightBorder,
            TopBorder,
            BottomBorder,
        }
    }
    public class EcfTreeView : EcfBaseView
    {
        public event EventHandler EcfItemSelected;

        public EcfItem SelectedEcfItem { get; private set; } = null;

        private Panel View { get; } = new Panel();
        private EcfToolContainer ToolContainer { get; } = new EcfToolContainer();
        private EcfSorter StructureSorter { get; } 
        private TreeView Tree { get; } = new TreeView();
        private List<EcfTreeNode> TreeNodes { get; set; }

        public EcfTreeView(string headline, EgsEcfFile file, ResizeableBorders mode) : base(headline, file, mode)
        {
            StructureSorter = new EcfSorter(
                Properties.texts.ToolTip_TreeItemCountSelector,
                Properties.texts.ToolTip_TreeItemGroupSelector,
                Properties.texts.ToolTip_TreeSorterDirection,
                Properties.texts.ToolTip_TreeSorterOriginOrder,
                Properties.texts.ToolTip_TreeSorterAlphabeticOrder);
            StructureSorter.SortingChanged += StructureSorter_SortingChanged;

            View.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            View.Dock = DockStyle.Fill;

            ToolContainer.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            ToolContainer.Dock = DockStyle.Top;

            Tree.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Tree.Dock = DockStyle.Fill;
            Tree.HideSelection = false;
            Tree.TreeViewNodeSorter = new EcfStructureComparer(StructureSorter, file);

            Tree.AfterSelect += Tree_AfterSelect;

            ToolContainer.Add(StructureSorter);
            View.Controls.Add(Tree);
            View.Controls.Add(ToolContainer);
            Controls.Add(View);
        }

        public void UpdateView(EcfTreeFilter treeFilter, EcfParameterFilter parameterFilter)
        {
            if (!IsUpdating)
            {
                ChangeViewUpdateState(true);
                TreeNodes = BuildNodesTree(treeFilter, parameterFilter);
                UpdateSorterInvoke();
                RefreshViewInvoke();
                ChangeViewUpdateState(false);
            }
            else
            {
                TryFindSelectedEcfItem();
            }
        }

        private void Tree_AfterSelect(object sender, TreeViewEventArgs evt)
        {
            if (!IsUpdating)
            {
                TryFindSelectedEcfItem();
                EcfItemSelected?.Invoke(SelectedEcfItem, null);
            }
        }
        private void StructureSorter_SortingChanged(object sender, EventArgs evt)
        {
            if (!IsUpdating)
            {
                ChangeViewUpdateState(true);
                RefreshView();
                ChangeViewUpdateState(false);
                EcfItemSelected?.Invoke(SelectedEcfItem, null);
            }
        }

        private List<EcfTreeNode> BuildNodesTree(EcfTreeFilter treeFilter, EcfParameterFilter parameterFilter)
        {
            List<EcfTreeNode> nodes = new List<EcfTreeNode>();

            bool commentsActive = treeFilter.IsCommentsActive();
            bool blocksActive = treeFilter.IsDataBlocksActive();
            string treeLikeText = treeFilter.GetLikeInputText();
            List<string> activeAttributes = treeFilter.GetCheckedAttributes();
            string parameterLikeText = parameterFilter.GetLikeInputText();
            List<string> activeParameters = parameterFilter.GetCheckedItems();

            foreach (EcfItem item in File.ItemList)
            {
                if (TryBuildNode(out EcfTreeNode rootNode, item, commentsActive, blocksActive, activeAttributes, parameterLikeText, activeParameters))
                {
                    if (rootNode.ParameterFilterPassed || rootNode.Nodes.Cast<EcfTreeNode>().Any(node => node.ParameterFilterPassed))
                    {
                        if (IsTreeNodeLike(rootNode.Text, treeLikeText))
                        {
                            nodes.Add(rootNode);
                        }
                        else
                        {
                            EcfTreeNode[] validSubNodes = rootNode.Nodes.Cast<EcfTreeNode>().Where(node => IsTreeNodeLike(node.Text, treeLikeText)).ToArray();
                            if (validSubNodes.Length > 0)
                            {
                                rootNode.Nodes.Clear();
                                rootNode.Nodes.AddRange(validSubNodes);
                                nodes.Add(rootNode);
                            }
                        }
                    }
                }
            }
            return nodes;
        }
        private bool TryBuildNode(out EcfTreeNode node, EcfItem item, 
            bool commentsActive, bool blocksActive, List<string> activeAttributes, 
            string parameterLikeText, List<string> activeParameters)
        {
            node = null;
            if (item is EcfComment comment)
            {
                if (commentsActive)
                {
                    node = new EcfTreeNode(comment, string.Format("{0}: {1}", Properties.titles.TreeView_CommentNodeName, string.Join(" / ", comment.Comments)));
                }
            }
            else if (item is EcfBlock block)
            {
                if (blocksActive)
                {
                    node = new EcfTreeNode(block, BuildBlockName(block, activeAttributes));
                }
                else if (commentsActive && block.ChildItems.Any(child => child is EcfComment))
                {
                    node = new EcfTreeNode(Properties.titles.TreeView_UnnamedNodeName);
                }
                if (node != null)
                {
                    foreach (EcfItem childItem in block.ChildItems)
                    {
                        if (childItem is EcfComment || childItem is EcfBlock)
                        {
                            if (TryBuildNode(out EcfTreeNode childNode, childItem, commentsActive, blocksActive, activeAttributes, parameterLikeText, activeParameters))
                            {
                                node.Nodes.Add(childNode);
                            }
                        }
                        else if (!node.ParameterFilterPassed && childItem is EcfParameter parameter)
                        {
                            node.ParameterFilterPassed = activeParameters.Contains(parameter.Key) && IsParameterValueLike(parameter.GetAllValues(), parameterLikeText);
                        }
                    }
                }
            }
            else
            {
                node = new EcfTreeNode(item.ToString());
            }
            return node != null;
        }
        private string BuildBlockName(EcfBlock block, List<string> visibleAttributes)
        {
            StringBuilder blockNameBuilder = new StringBuilder();
            if (block.BlockDataType != null)
            {
                blockNameBuilder.Append(block.BlockDataType);
            }
            foreach(EcfAttribute attr in block.Attributes)
            {
                if (visibleAttributes.Contains(attr.Key))
                {
                    if (blockNameBuilder.Length > 0) 
                    { 
                        blockNameBuilder.Append(" / ");
                    }
                    blockNameBuilder.Append(attr.Key);
                    if (attr.HasValue())
                    {
                        blockNameBuilder.Append(": ");
                        blockNameBuilder.Append(attr.GetFirstValue());
                    }
                }
            }
            if (blockNameBuilder.Length < 1) { blockNameBuilder.Append(Properties.titles.TreeView_UnnamedNodeName); }
            return blockNameBuilder.ToString();
        }
        private void UpdateSorterInvoke()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    StructureSorter.SetOverallItemCount(TreeNodes.Count);
                });
            }
            else
            {
                StructureSorter.SetOverallItemCount(TreeNodes.Count);
            }
        }
        private void RefreshViewInvoke()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    RefreshView();
                });
            }
            else
            {
                RefreshView();
            }
        }
        private void RefreshView()
        {
            int? selectedIndex = Tree.SelectedNode?.Index;
            Tree.BeginUpdate();
            Tree.Nodes.Clear();
            Tree.Nodes.AddRange(TreeNodes.Skip(StructureSorter.ItemCount * (StructureSorter.ItemGroup - 1)).Take(StructureSorter.ItemCount).ToArray());
            Tree.Sort();
            Tree.EndUpdate();
            Text = string.Format("{0} - {1} {3} - {2} {4}", ViewName, TreeNodes.Count, 
                TreeNodes.Sum(node => CountEcfSubItems(node)),
                Properties.titles.TreeView_Header_RootElements, 
                Properties.titles.TreeView_Header_SubElements);
            TrySelectNode(selectedIndex);
        }
        private void TrySelectNode(int? index)
        {
            EcfTreeNode selectedNode = null;
            if (index != null)
            {
                selectedNode = Tree.Nodes.Cast<EcfTreeNode>().FirstOrDefault(node => node.Index.Equals(index));
                if (selectedNode == null)
                {
                    selectedNode = Tree.Nodes.Cast<EcfTreeNode>().Last();
                }
            } 
            else if (Tree.Nodes.Count > 0)
            {
                selectedNode = Tree.Nodes.Cast<EcfTreeNode>().First();
            }
            if (selectedNode != null)
            {
                Tree.SelectedNode = selectedNode;
                Tree.Focus();
            }
            TryFindSelectedEcfItem();
        }
        private void TryFindSelectedEcfItem()
        {
            SelectedEcfItem = (Tree.SelectedNode as EcfTreeNode)?.Item;
        }
        private int CountEcfSubItems(EcfTreeNode node)
        {
            int count = 0;
            foreach (EcfTreeNode subNode in node.Nodes)
            {
                count++;
                count += CountEcfSubItems(subNode);
            }
            return count;
        }

        private class EcfStructureComparer : IComparer
        {
            private EcfSorter Sorter { get; } = null;
            private EgsEcfFile File { get; } = null;

            public EcfStructureComparer(EcfSorter sorter, EgsEcfFile file)
            {
                Sorter = sorter;
                File = file;
            }
            
            public int Compare(object first, object second)
            {
                int compare = 1;
                if (first is EcfTreeNode node1 && second is EcfTreeNode node2)
                {
                    switch (Sorter.SortingType)
                    {
                        case SortingTypes.Alphabetical: compare = string.Compare(node1.Text, node2.Text); break;
                        default: compare = File.ItemList.IndexOf(node1.Item) - File.ItemList.IndexOf(node2.Item); break;
                    }
                    compare *= Sorter.IsAscending ? 1 : -1;
                }
                return compare;
            }
        }
        private class EcfTreeNode : TreeNode
        {
            public EcfItem Item { get; } = null;
            public bool ParameterFilterPassed { get; set; } = false;

            public EcfTreeNode(EcfItem item, string name) : base()
            {
                Item = item;
                Text = name;
                ParameterFilterPassed = !(Item is EcfBlock);
            }
            public EcfTreeNode(string name) : this(null, name)
            {

            }
        }
    }
    public class EcfParameterView : EcfBaseView
    {
        public event EventHandler EcfParameterSelected;

        public EcfParameter SelectedEcfParameter { get; private set; } = null;

        private Panel View { get; } = new Panel();
        private EcfToolContainer ToolContainer { get; } = new EcfToolContainer();
        private EcfSorter ParameterSorter { get; }
        private DataGridView Grid { get; } = new DataGridView();
        private List<EcfParameterRow> ParameterRows { get; set; }

        private DataGridViewTextBoxColumn ParameterNumberColumn { get; } = new DataGridViewTextBoxColumn();
        private DataGridViewCheckBoxColumn ParameterInheritedColumn { get; } = new DataGridViewCheckBoxColumn();
        private DataGridViewCheckBoxColumn ParameterOverwritingColumn { get; } = new DataGridViewCheckBoxColumn();
        private DataGridViewTextBoxColumn ParameterParentColumn { get; } = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn ParameterNameColumn { get; } = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn ParameterValueColumn { get; } = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn ParameterCommentColumn { get; } = new DataGridViewTextBoxColumn();

        public EcfParameterView(string headline, EgsEcfFile file, ResizeableBorders mode) : base(headline, file, mode)
        {
            ParameterSorter = new EcfSorter(
                Properties.texts.ToolTip_ParameterCountSelector,
                Properties.texts.ToolTip_ParameterGroupSelector,
                Properties.texts.ToolTip_ParameterSorterDirection,
                Properties.texts.ToolTip_ParameterSorterOriginOrder,
                Properties.texts.ToolTip_ParameterSorterAlphabeticOrder);
            ParameterSorter.SortingChanged += ParameterSorter_SortingChanged;

            View.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            View.Dock = DockStyle.Fill;

            ToolContainer.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            ToolContainer.Dock = DockStyle.Top;

            Grid.AllowUserToAddRows = false;
            Grid.AllowUserToDeleteRows = false;
            Grid.AllowDrop = false;
            Grid.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Grid.Dock = DockStyle.Fill;
            Grid.EditMode = DataGridViewEditMode.EditProgrammatically;

            Grid.ColumnHeaderMouseClick += Grid_ColumnHeaderMouseClick;
            Grid.RowHeaderMouseClick += Grid_RowHeaderMouseClick;
            Grid.SelectionChanged += Grid_SelectionChanged;

            ParameterNumberColumn.HeaderText = Properties.titles.ParameterView_ParameterNumberColumn;
            ParameterInheritedColumn.HeaderText = Properties.titles.ParameterView_ParameterInheritedColumn;
            ParameterOverwritingColumn.HeaderText = Properties.titles.ParameterView_ParameterOverwritingColumn;
            ParameterParentColumn.HeaderText = Properties.titles.ParameterView_ParameterParentColumn;
            ParameterNameColumn.HeaderText = Properties.titles.ParameterView_ParameterNameColumn;
            ParameterValueColumn.HeaderText = Properties.titles.ParameterView_ParameterValueColumn;
            ParameterCommentColumn.HeaderText = Properties.titles.ParameterView_ParameterCommentColumn;

            ParameterNumberColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            ParameterInheritedColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            ParameterOverwritingColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            ParameterParentColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            ParameterNameColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            ParameterValueColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            ParameterCommentColumn.SortMode = DataGridViewColumnSortMode.Programmatic;

            ParameterInheritedColumn.ToolTipText = Properties.texts.ToolTip_ParameterView_InheritedColumn;
            ParameterOverwritingColumn.ToolTipText = Properties.texts.ToolTip_ParameterView_OverwritingColumn;
            ParameterValueColumn.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            Grid.Columns.Add(ParameterNumberColumn);
            Grid.Columns.Add(ParameterInheritedColumn);
            Grid.Columns.Add(ParameterOverwritingColumn);
            Grid.Columns.Add(ParameterParentColumn);
            Grid.Columns.Add(ParameterNameColumn);
            Grid.Columns.Add(ParameterValueColumn);
            Grid.Columns.Add(ParameterCommentColumn);

            ToolContainer.Add(ParameterSorter);
            View.Controls.Add(Grid);
            View.Controls.Add(ToolContainer);
            Controls.Add(View);
        }

        public void UpdateView(EcfParameterFilter filter, EcfItem item)
        {
            if (!IsUpdating)
            {
                ChangeViewUpdateState(true);
                ParameterRows = BuildGridViewRows(filter, item);
                UpdateSorterInvoke();
                RefreshViewInvoke();
                ChangeViewUpdateState(false);
            }
            else
            {
                TryFindSelectedEcfParameter();
            }
        }

        private void Grid_SelectionChanged(object sender, EventArgs evt)
        {
            if (!IsUpdating)
            {
                if (Grid.SelectedCells.Count > 0)
                {
                    SelectedEcfParameter = (Grid.SelectedCells[0].OwningRow as EcfParameterRow)?.Parameter;
                }
                else
                {
                    SelectedEcfParameter = null;
                }
                EcfParameterSelected?.Invoke(SelectedEcfParameter, null);
            }
        }
        private void Grid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs evt)
        {
            {
                if (Grid.SelectionMode != DataGridViewSelectionMode.ColumnHeaderSelect)
                {
                    Grid.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
                    Grid.Columns[evt.ColumnIndex].Selected = true;
                }
            }
        }
        private void Grid_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs evt)
        {
            {
                if (Grid.SelectionMode != DataGridViewSelectionMode.RowHeaderSelect)
                {
                    Grid.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                    Grid.Rows[evt.RowIndex].Selected = true;
                }
            }
        }
        private void ParameterSorter_SortingChanged(object sender, EventArgs evt)
        {
            if (!IsUpdating)
            {
                ChangeViewUpdateState(true);
                RefreshView();
                ChangeViewUpdateState(false);
                EcfParameterSelected?.Invoke(SelectedEcfParameter, null);
            }
        }

        private List<EcfParameterRow> BuildGridViewRows(EcfParameterFilter filter, EcfItem item)
        {
            List<EcfParameterRow> rows = new List<EcfParameterRow>();

            string parameterLikeText = filter.GetLikeInputText();
            List<string> activeParameters = filter.GetCheckedItems();
            string refSourceTag = File.Definition.BlockReferenceSourceAttribute;
            string refTargetTag = File.Definition.BlockReferenceTargetAttribute;

            if (item is EcfBlock block)
            {
                if (!string.IsNullOrEmpty(refSourceTag) && !string.IsNullOrEmpty(refTargetTag))
                {
                    BuildParentBlockRows(rows, block, parameterLikeText, activeParameters, refSourceTag, refTargetTag);
                }
                BuildDataBlockRowGroup(rows, block, parameterLikeText, activeParameters, false);
            }

            return rows;
        }
        private void BuildParentBlockRows(List<EcfParameterRow> rows, EcfBlock block, string parameterLikeText, List<string> activeParameters, string refSourceTag, string refTargetTag)
        {
            string reference = block.GetAttributeFirstValue(refSourceTag);
            if (reference != null)
            {
                EcfBlock inheritedBlock = File.ItemList.Where(item => item is EcfBlock).Cast<EcfBlock>()
                    .FirstOrDefault(parentBlock => parentBlock.GetAttributeFirstValue(refTargetTag).Equals(reference));
                if (inheritedBlock != null)
                {
                    BuildParentBlockRows(rows, inheritedBlock, parameterLikeText, activeParameters, refSourceTag, refTargetTag);
                    BuildDataBlockRowGroup(rows, inheritedBlock, parameterLikeText, activeParameters, true);
                }
            }
        }
        private void BuildDataBlockRowGroup(List<EcfParameterRow> rows, EcfBlock block, string parameterLikeText, List<string> activeParameters, bool isInherited)
        {
            foreach (EcfItem subItem in block.ChildItems)
            {
                if (subItem is EcfParameter parameter)
                {
                    if (activeParameters.Contains(parameter.Key) && IsParameterValueLike(parameter.GetAllValues(), parameterLikeText))
                    {
                        EcfParameterRow overwrittenRow = rows.LastOrDefault(row => row.Parameter.Key.Equals(parameter.Key) && row.IsInherited());
                        rows.Add(new EcfParameterRow(rows.Count + 1, block.BuildIdentification(), parameter, isInherited, overwrittenRow)); 
                    }
                }
                else if (subItem is EcfBlock subBlock) 
                {
                    BuildDataBlockRowGroup(rows, subBlock, parameterLikeText, activeParameters, isInherited);
                }
            }
        }
        private void UpdateSorterInvoke()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    ParameterSorter.SetOverallItemCount(ParameterRows.Count);
                });
            }
            else
            {
                ParameterSorter.SetOverallItemCount(ParameterRows.Count);
            }
        }
        private void RefreshViewInvoke()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    RefreshView();
                });
            }
            else
            {
                RefreshView();
            }
        }
        private void RefreshView()
        {
            List<KeyValuePair<int, int>> selectedCells = Grid.SelectedCells.Cast<DataGridViewCell>().Select(cell => new KeyValuePair<int, int>(cell.RowIndex, cell.ColumnIndex)).ToList();
            Grid.SuspendLayout();
            Grid.Rows.Clear();
            Grid.Rows.AddRange(ParameterRows.Skip(ParameterSorter.ItemCount * (ParameterSorter.ItemGroup - 1)).Take(ParameterSorter.ItemCount).ToArray());
            Grid.Sort(GetSortingColumn(ParameterSorter), ParameterSorter.IsAscending ? ListSortDirection.Ascending : ListSortDirection.Descending);
            Grid.AutoResizeColumns();
            Grid.AutoResizeRows();
            Grid.ResumeLayout();
            TrySelectCells(selectedCells);
            Text = string.Format("{0} - {1} {4} - {2} {5} - {3} {6}", ViewName, Grid.RowCount,
                Grid.Rows.Cast<DataGridViewRow>().Select(row => row.Cells[ParameterInheritedColumn.Index]).Cast<DataGridViewCheckBoxCell>().Count(cell => Convert.ToBoolean(cell.Value)),
                Grid.Rows.Cast<DataGridViewRow>().Select(row => row.Cells[ParameterOverwritingColumn.Index]).Cast<DataGridViewCheckBoxCell>().Count(cell => Convert.ToBoolean(cell.Value)),
                Properties.titles.ParameterView_Header_OverallParameters,
                Properties.titles.ParameterView_Header_InheritedParameters,
                Properties.titles.ParameterView_Header_OverwritingParameters);
        }
        private DataGridViewColumn GetSortingColumn(EcfSorter sorter)
        {
            switch (sorter.SortingType)
            {
                case SortingTypes.Alphabetical: return ParameterNameColumn;
                default: return ParameterNumberColumn;
            }
        }
        private void TrySelectCells(List<KeyValuePair<int, int>> selectedCells)
        {
            Grid.ClearSelection();
            int rowCount = Grid.RowCount;
            foreach (KeyValuePair<int, int> cell in selectedCells)
            {
                if (cell.Key < rowCount)
                {
                    Grid.Rows[cell.Key].Cells[cell.Value].Selected = true;
                }
            }
            TryFindSelectedEcfParameter();
        }
        private void TryFindSelectedEcfParameter()
        {
            SelectedEcfParameter =  Grid.SelectedCells.Count > 0 ? (Grid.SelectedCells[0].OwningRow as EcfParameterRow)?.Parameter : null;
        }

        private class EcfParameterRow : DataGridViewRow
        {
            public EcfParameter Parameter { get; }
            public EcfParameterRow OverwrittenRow { get; }

            private DataGridViewTextBoxCell NumberCell { get; }
            private DataGridViewCheckBoxCell IsInheritedCell { get; }
            private DataGridViewCheckBoxCell IsOverwritingCell { get; }
            private DataGridViewTextBoxCell ParentNameCell { get; }
            private DataGridViewTextBoxCell ParameterNameCell { get; }
            private DataGridViewTextBoxCell ValueCell { get; }
            private DataGridViewTextBoxCell CommentsCell { get; }

            public EcfParameterRow(int number, string  parentName, EcfParameter parameter, bool isInherited, EcfParameterRow overwrittenRow) : base()
            {
                Parameter = parameter;
                OverwrittenRow = overwrittenRow;

                NumberCell = new DataGridViewTextBoxCell() { Value = number };
                IsInheritedCell = new DataGridViewCheckBoxCell() { Value = isInherited };
                IsOverwritingCell = new DataGridViewCheckBoxCell() { Value = IsOverwriting() };
                ParentNameCell = new DataGridViewTextBoxCell() { Value = parentName };
                ParameterNameCell = new DataGridViewTextBoxCell() { Value = parameter.Key };
                ValueCell = new DataGridViewTextBoxCell() { Value = BuildValueText() };
                CommentsCell = new DataGridViewTextBoxCell() { Value = string.Join(", ", parameter.Comments) };

                Cells.Add(NumberCell);
                Cells.Add(IsInheritedCell);
                Cells.Add(IsOverwritingCell);
                Cells.Add(ParentNameCell);
                Cells.Add(ParameterNameCell);
                Cells.Add(ValueCell);
                Cells.Add(CommentsCell);
            }

            public bool IsInherited()
            {
                return Convert.ToBoolean(IsInheritedCell.Value);
            }
            public bool IsOverwriting()
            {
                return OverwrittenRow != null;
            }
            public string GetParentName()
            {
                return Convert.ToString(ParentNameCell.Value);
            }

            private string BuildValueText()
            {
                string valueSeperator = string.Format("{0} ", Properties.texts.ParameterView_ValueSeperator);
                if (!Parameter.IsUsingGroups())
                {
                    return string.Join(valueSeperator, Parameter.GetAllValues());
                }
                else
                {
                    return BuildValueGroupText(valueSeperator);
                }
            }
            private string BuildValueGroupText(string valueSeperator)
            {
                List<string> valueGroups = new List<string>();
                StringBuilder valueGroup = new StringBuilder();

                foreach (EcfValueGroup group in Parameter.ValueGroups)
                {
                    valueGroup.Clear();
                    
                    valueGroup.Append(Properties.titles.ParameterView_ValueGroup);
                    valueGroup.Append(" ");
                    valueGroup.Append(Parameter.IndexOf(group) + 1);
                    valueGroup.Append(Properties.texts.ParameterView_GroupSeperator);
                    valueGroup.Append(" ");
                    valueGroup.Append(string.Join(valueSeperator, group.Values));
                    
                    valueGroups.Add(valueGroup.ToString());
                }
                return string.Join(Environment.NewLine, valueGroups);
            }
        }
    }
    public class EcfInfoView : EcfBaseView
    {
        private FlowLayoutPanel View { get; } = new FlowLayoutPanel();
        private EcfTableLayoutPanel AddDataView { get; } = new EcfTableLayoutPanel(Properties.titles.InfoView_AddData, 120);
        private EcfTableLayoutPanel BlockAttributeView { get; } = new EcfTableLayoutPanel(Properties.titles.InfoView_ElementAttributes, 120);
        private EcfTableLayoutPanel ParameterAttributeView { get; } = new EcfTableLayoutPanel(Properties.titles.InfoView_ParameterAttributes, 120);

        public EcfInfoView(string headline, EgsEcfFile file, ResizeableBorders mode) : base(headline, file, mode)
        {
            View.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            View.Dock = DockStyle.Fill;
            View.AutoScroll = true;
            View.FlowDirection = FlowDirection.TopDown;
            
            AddDataView.AddRow(Properties.titles.InfoView_ElementPreSign, null);
            AddDataView.AddRow(Properties.titles.InfoView_ElementType, null);
            AddDataView.AddRow(Properties.titles.InfoView_ElementChildCount, null);
            AddDataView.AddRow(Properties.titles.InfoView_ElementChildBlockCount, null);
            AddDataView.AddRow(Properties.titles.InfoView_ElementComment, null);
            AddDataView.AddRow(Properties.titles.InfoView_ParameterComment, null);

            View.Controls.Add(AddDataView);
            View.Controls.Add(BlockAttributeView);
            View.Controls.Add(ParameterAttributeView);
            Controls.Add(View);
        }

        public void UpdateView(EcfItem item)
        {
            if (!IsUpdating)
            {
                ChangeViewUpdateState(true);
                RefreshViewInvoke(item as EcfBlock);
                ChangeViewUpdateState(false);
            }
        }
        public void UpdateView(EcfParameter parameter)
        {
            if (!IsUpdating)
            {
                ChangeViewUpdateState(true);
                RefreshViewInvoke(parameter);
                ChangeViewUpdateState(false);
            }
        }
        public void UpdateView(EcfItem item, EcfParameter parameter)
        {
            if (!IsUpdating)
            {
                ChangeViewUpdateState(true);
                RefreshViewInvoke(item as EcfBlock);
                RefreshViewInvoke(parameter);
                ChangeViewUpdateState(false);
            }
        }

        private void RefreshViewInvoke(EcfBlock block)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    RefreshView(block);
                });
            }
            else
            {
                RefreshView(block);
            }
        }
        private void RefreshViewInvoke(EcfParameter parameter)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    RefreshView(parameter);
                });
            }
            else
            {
                RefreshView(parameter);
            }
        }
        private void RefreshView(EcfBlock block)
        {
            View.SuspendLayout();
            RefreshAddDataView(block);
            RefreshBlockAttributeView(block);
            View.ResumeLayout();
        }
        private void RefreshView(EcfParameter parameter)
        {
            View.SuspendLayout();
            RefreshAddDataView(parameter);
            RefreshParameterAttributeView(parameter);
            View.ResumeLayout();
        }
        private void RefreshAddDataView(EcfBlock block)
        {
            AddDataView.SuspendLayout();
            if (block != null)
            {
                AddDataView.UpdateRowValue(0, block.TypePreMark ?? string.Empty);
                AddDataView.UpdateRowValue(1, block.BlockDataType ?? string.Empty);
                AddDataView.UpdateRowValue(2, block.ChildItems.Count().ToString());
                AddDataView.UpdateRowValue(3, block.ChildItems.Count(item => item is EcfBlock).ToString());
                AddDataView.UpdateRowValue(4, string.Join(", ", block.Comments));
            }
            else
            {
                AddDataView.UpdateRowValue(0, "");
                AddDataView.UpdateRowValue(1, "");
                AddDataView.UpdateRowValue(2, "");
                AddDataView.UpdateRowValue(3, "");
                AddDataView.UpdateRowValue(4, "");
            }
            AddDataView.ResumeLayout();
        }
        private void RefreshAddDataView(EcfParameter parameter)
        {
            AddDataView.SuspendLayout();
            if (parameter != null)
            {
                AddDataView.UpdateRowValue(5, string.Join(", ", parameter.Comments));
            }
            else
            {
                AddDataView.UpdateRowValue(5, "");
            }
            AddDataView.ResumeLayout();
        }
        private void RefreshBlockAttributeView(EcfBlock block)
        {
            BlockAttributeView.SuspendLayout();
            BlockAttributeView.Clear();
            BlockAttributeView.AddRows(block?.Attributes.Select(attr => new KeyValuePair<string, string>(attr.Key, string.Join(", ", attr.GetAllValues()))).ToList());
            BlockAttributeView.ResumeLayout();
        }
        private void RefreshParameterAttributeView(EcfParameter parameter)
        {
            ParameterAttributeView.SuspendLayout();
            ParameterAttributeView.Clear();
            ParameterAttributeView.AddRows(parameter?.Attributes.Select(attr => new KeyValuePair<string, string>(attr.Key, string.Join(", ", attr.GetAllValues()))).ToList());
            ParameterAttributeView.ResumeLayout();
        }

        private class EcfTableLayoutPanel : TableLayoutPanel
        {
            public int MinColumnWidth { get; }
            
            public EcfTableLayoutPanel(string headerText, int minColumnWidth) : base()
            {
                MinColumnWidth = minColumnWidth;

                AutoSize = true;
                AutoSizeMode = AutoSizeMode.GrowAndShrink;
                ColumnCount = 2;
                RowCount = 1;

                Label header = new Label() { Text = headerText ?? "null" };
                header.AutoSize = true;
                header.Margin = new Padding(0, 3, 0, 3);
                Controls.Add(header, 0, 0);
                SetColumnSpan(header, 2);
            }
            public EcfTableLayoutPanel(string headerText) : this(headerText, 50)
            {
                
            }
            public EcfTableLayoutPanel() : this("Header", 50)
            {

            }

            public void Clear()
            {
                Controls.Cast<Control>().Where(control => GetRow(control) > 0).ToList().ForEach(control =>
                {
                    Controls.Remove(control);
                });
                RowCount = 1;
            }
            public void AddRow(string nameText, string valueText)
            {
                Label name = new Label() { Text = nameText ?? "null" };
                Label value = new Label() { Text = valueText ?? "null" };

                name.AutoSize = true;
                value.AutoSize = true;
                name.Margin = new Padding(5, 1, 0, 1);
                name.MinimumSize = new Size(MinColumnWidth, 0);
                value.MinimumSize = new Size(MinColumnWidth, 0);

                RowCount++;
                Controls.Add(name, 0, RowCount);
                Controls.Add(value, 1, RowCount);
            }
            public void AddRows(List<KeyValuePair<string, string>> rows)
            {
                rows?.ForEach(row =>
                {
                    AddRow(row.Key, row.Value);
                });
            }
            public void UpdateRowValue(int valueRowIndex, string newValue)
            {
                try
                {
                    Control value = GetControlFromPosition(1, valueRowIndex + 2);
                    if (value != null)
                    {
                        value.Text = newValue ?? "null";
                    }
                }
                catch (Exception) { }
            }
        }
    }
    public class EcfErrorView : EcfBaseView
    {
        private ListBox Errors { get; } = new ListBox();

        public EcfErrorView(string headline, EgsEcfFile file, ResizeableBorders mode) : base(headline, file, mode)
        {
            Errors.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            Errors.Dock = DockStyle.Fill;
            Errors.HorizontalScrollbar = true;
            Errors.IntegralHeight = false;
            Errors.SelectionMode = SelectionMode.MultiExtended;

            Controls.Add(Errors);
        }

        public void UpdateView()
        {
            if (!IsUpdating)
            {
                ChangeViewUpdateState(true);
                List<string> list = File.ErrorList.Select(error => error.ToString()).ToList();
                RefreshViewInvoke(list);
                ChangeViewUpdateState(false);
            }
        }

        private void RefreshViewInvoke(List<string> list)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    RefreshView(list);
                });
            }
            else
            {
                RefreshView(list);
            }
        }
        private void RefreshView(List<string> list)
        {
            Errors.BeginUpdate();
            Errors.Items.Clear();
            Errors.Items.AddRange(list.ToArray());
            Errors.EndUpdate();
            Text = string.Format("{0} - {1} {2}", ViewName, list.Count, Properties.titles.ErrorView_Header_Errors);
        }
    }

    public class EcfToolContainer : FlowLayoutPanel
    {
        public EcfToolContainer()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Margin = new Padding(Margin.Left, 0, Margin.Right, 0);
        }

        public void Add(EcfToolGroup toolGroup)
        {
            Controls.Add(toolGroup);
        }
    }
    public abstract class EcfToolGroup : FlowLayoutPanel
    {
        public EcfToolGroup() : base()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Margin = new Padding(Margin.Left, 0, Margin.Right, 0);
        }
        public void Add(Control control)
        {
            Controls.Add(control);
        }
    }
    public class EcfFilterControl : EcfToolGroup
    {
        public event EventHandler ApplyFilter;
        public event EventHandler ResetFilter;

        private Button RefreshButton { get; } = new Button() { Text = "R", Size = new Size(20,20) };
        private Button ClearButton { get; } = new Button() { Text = "C", Size = new Size(20, 20) };

        public EcfFilterControl() : base()
        {
            RefreshButton.Click += OnApplyFilter;
            ClearButton.Click += OnResetFilter;

            new ToolTip().SetToolTip(RefreshButton, Properties.texts.ToolTip_FilterApplyButton);
            new ToolTip().SetToolTip(ClearButton, Properties.texts.ToolTip_FilterClearButton);

            Add(RefreshButton);
            Add(ClearButton);
        }

        private void OnApplyFilter(object sender, EventArgs evt)
        {
            ApplyFilter.Invoke(sender, evt);
        }
        private void OnResetFilter(object sender, EventArgs evt)
        {
            ResetFilter.Invoke(sender, evt);
        }
    }
    public abstract class EcfBaseFilter : EcfToolGroup
    {
        public event EventHandler ApplyFilter;
        
        private ToolTip LikeToolTip { get; } = new ToolTip();
        private TextBox LikeInput { get; } = new TextBox();
        protected CheckComboBox ItemSelector { get; set; } = null;

        private string LikeToolTipText { get; }

        public EcfBaseFilter(List<CheckableNameItem> items, string likeToolTip, string itemTypeName, string itemSelectorTooltip) : base()
        {
            LikeToolTipText = likeToolTip;

            ItemSelector = new CheckComboBox(itemTypeName, itemSelectorTooltip)
            {
                MaxDropDownItems = 10
            };
            ItemSelector.SetItems(items);

            LikeInput.MouseHover += LikeInput_MouseHover;
            LikeInput.KeyUp += LikeInput_KeyUp;

            Add(LikeInput);
            Add(ItemSelector);
        }

        private void LikeInput_MouseHover(object sender, EventArgs evt)
        {
            LikeToolTip.SetToolTip(LikeInput, LikeToolTipText); 
        }
        private void LikeInput_KeyUp(object sender, KeyEventArgs evt)
        {
            if (evt.KeyCode == Keys.Enter)
            {
                ApplyFilter?.Invoke(this, null);
            }
        }

        public string GetLikeInputText()
        {
            if (InvokeRequired)
            {
                return (string)Invoke((Func<string>)delegate
                {
                    return GetLikeInputTextInvoked();
                });
            }
            else
            {
                return GetLikeInputTextInvoked();
            }
        }
        public bool IsLike(string text)
        {
            if (InvokeRequired)
            {
                return (bool)Invoke((Func<bool>)delegate
                {
                    return IsLikeInvoked(text);
                });
            }
            else
            {
                return IsLikeInvoked(text);
            }
        }
        public List<string> GetCheckedItems()
        {
            if (InvokeRequired)
            {
                return (List<string>)Invoke((Func<List<string>>)delegate
                {
                    return GetCheckedItemsInvoked();
                });
            }
            else
            {
                return GetCheckedItemsInvoked();
            }
        }
        public List<string> GetUncheckedItems()
        {
            if (InvokeRequired)
            {
                return (List<string>)Invoke((Func<List<string>>)delegate
                {
                    return GetUncheckedItemsInvoked();
                });
            }
            else
            {
                return GetUncheckedItemsInvoked();
            }
        }
        public void Reset()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate 
                {
                    ResetInvoked();
                });
            }
            else
            {
                ResetInvoked();
            }
        }

        private string GetLikeInputTextInvoked()
        {
            return LikeInput.Text;
        }
        private bool IsLikeInvoked(string text)
        {
            if (string.IsNullOrEmpty(LikeInput.Text)) { return true; }
            if (string.IsNullOrEmpty(text)) { return false; }
            return text.Contains(LikeInput.Text);
        }
        private List<string> GetCheckedItemsInvoked()
        {
            return ItemSelector.GetCheckedItems().Select(item => item.Id).ToList();
        }
        private List<string> GetUncheckedItemsInvoked()
        {
            return ItemSelector.GetUncheckedItems().Select(item => item.Id).ToList();
        }
        protected virtual void ResetInvoked()
        {
            LikeInput.Clear();
            ItemSelector.Reset();
        }
    }
    public class EcfTreeFilter : EcfBaseFilter
    {
        private CheckComboBox AttributeSelector { get; set; } = null;

        private enum SelectableItems
        {
            Comments,
            DataBlocks,
        }

        public EcfTreeFilter(List<string> attributes) : base(
            Enum.GetValues(typeof(SelectableItems)).Cast<SelectableItems>().Select(item => new CheckableNameItem(item.ToString(), GetSelectableItemsDisplayName(item))).ToList(),
            Properties.texts.ToolTip_TreeLikeInput,
            Properties.titles.TreeView_FilterSelector_Elements,
            Properties.texts.ToolTip_TreeItemTypeSelector)
        {
            AttributeSelector = new CheckComboBox(Properties.titles.TreeView_FilterSelector_Attributes, Properties.texts.ToolTip_TreeAttributeSelector)
            {
                MaxDropDownItems = 10
            };
            AttributeSelector.SetItems(attributes.Select(attr => new CheckableNameItem(attr)).ToList());

            Add(AttributeSelector);
        }

        private static string GetSelectableItemsDisplayName(SelectableItems item)
        {
            switch (item)
            {
                case SelectableItems.Comments: return Properties.titles.TreeView_SelectableElements_Comments;
                case SelectableItems.DataBlocks: return Properties.titles.TreeView_SelectableElements_DataBlocks;
                default: throw new ArgumentException(item.ToString());
            }
        }

        public bool IsCommentsActive()
        {
            if (InvokeRequired)
            {
                return (bool)Invoke((Func<bool>)delegate
                {
                    return IsCommentsActiveInvoked();
                });
            }
            else
            {
                return IsCommentsActiveInvoked();
            }
        }
        public bool IsDataBlocksActive()
        {
            if (InvokeRequired)
            {
                return (bool)Invoke((Func<bool>)delegate
                {
                    return IsDataBlocksActiveInvoked();
                });
            }
            else
            {
                return IsDataBlocksActiveInvoked();
            }
        }
        public List<string> GetCheckedAttributes()
        {
            if (InvokeRequired)
            {
                return (List<string>)Invoke((Func<List<string>>)delegate
                {
                    return GetCheckedAttributesInvoked();
                });
            }
            else
            {
                return GetCheckedAttributesInvoked();
            }
        }
        public List<string> GetUncheckedAttributes()
        {
            if (InvokeRequired)
            {
                return (List<string>)Invoke((Func<List<string>>)delegate
                {
                    return GetUncheckedAttributesInvoked();
                });
            }
            else
            {
                return GetUncheckedAttributesInvoked();
            }
        }

        private bool IsCommentsActiveInvoked()
        {
            return ItemSelector.IsItemChecked(SelectableItems.Comments.ToString());
        }
        private bool IsDataBlocksActiveInvoked()
        {
            return ItemSelector.IsItemChecked(SelectableItems.DataBlocks.ToString());
        }
        private List<string> GetCheckedAttributesInvoked()
        {
            return AttributeSelector.GetCheckedItems().Select(param => param.Id).ToList();
        }
        private List<string> GetUncheckedAttributesInvoked()
        {
            return AttributeSelector.GetUncheckedItems().Select(param => param.Id).ToList();
        }
        protected override void ResetInvoked()
        {
            base.ResetInvoked();
            AttributeSelector.Reset();
        }
    }
    public class EcfParameterFilter : EcfBaseFilter
    {
        public EcfParameterFilter(List<string> items) : base(
            items.Select(item => new CheckableNameItem(item)).ToList(),
            Properties.texts.ToolTip_ParameterLikeInput,
            Properties.titles.ParameterView_FilterSelector_Parameters,
            Properties.texts.ToolTip_ParameterSelector)
        {

        }
    }
    public class EcfSorter : EcfToolGroup
    {
        public event EventHandler SortingChanged;

        private ComboBox ItemCountSelector { get; } = new ComboBox();
        private NumericUpDown ItemGroupSelector { get; } = new NumericUpDown();
        private CheckBox DirectionSelector { get; } = new CheckBox();
        private RadioButton OriginOrderSelector { get; } = new RadioButton();
        private RadioButton AlphabeticOrderSelector { get; } = new RadioButton();

        public int ItemCount { get; private set; } = 100;
        public int ItemGroup { get; private set; } = 1;
        public bool IsAscending { get; private set; } = true;
        public SortingTypes SortingType { get; private set; } = SortingTypes.Original;

        private int OverallItemCount { get; set; } = 100;
        private bool IsItemGroupSelectorUpdating { get; set; } = false;

        public enum SortingTypes
        {
            Original,
            Alphabetical,
        }

        public EcfSorter(string itemCountSelectorTooltip, string itemGroupSelectorTooltip, string directionToolTip, string originToolTip, string aplhabeticToolTip) : base()
        {
            DirectionSelector.Text = "D";
            OriginOrderSelector.Text = "O";
            AlphabeticOrderSelector.Text = "A";
            DirectionSelector.Size = new Size(20, 20);
            OriginOrderSelector.Size = new Size(20, 20);
            AlphabeticOrderSelector.Size = new Size(20, 20);





            



            ItemCountSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            ItemCountSelector.Items.AddRange(new object[] { "10", "25", "50", "100", "250", "500" });
            ItemCountSelector.Width = ItemCountSelector.Items.Cast<string>().Max(x => TextRenderer.MeasureText(x, ItemCountSelector.Font).Width) + SystemInformation.VerticalScrollBarWidth;
            ItemCountSelector.SelectedItem = ItemCount.ToString();
            new ToolTip().SetToolTip(ItemCountSelector, itemCountSelectorTooltip);

            ItemGroupSelector.Minimum = ItemGroup;
            UpdateItemGroupSelector();
            new ToolTip().SetToolTip(ItemGroupSelector, itemGroupSelectorTooltip);

            OriginOrderSelector.Checked = true;

            DirectionSelector.Appearance = Appearance.Button;
            OriginOrderSelector.Appearance = Appearance.Button;
            AlphabeticOrderSelector.Appearance = Appearance.Button;

            new ToolTip().SetToolTip(DirectionSelector, directionToolTip);
            new ToolTip().SetToolTip(OriginOrderSelector, originToolTip);
            new ToolTip().SetToolTip(AlphabeticOrderSelector, aplhabeticToolTip);

            Add(ItemCountSelector);
            Add(ItemGroupSelector);
            Add(DirectionSelector);
            Add(OriginOrderSelector);
            Add(AlphabeticOrderSelector);

            ItemCountSelector.SelectionChangeCommitted += ItemCountSelector_SelectionChangeCommitted;
            ItemGroupSelector.ValueChanged += ItemGroupSelector_ValueChanged;
            DirectionSelector.Click += DirectionSelector_Click;
            OriginOrderSelector.CheckedChanged += OriginOrderSelector_CheckedChanged;
            AlphabeticOrderSelector.CheckedChanged += AlphabeticOrderSelector_CheckedChanged;
        }

        public void SetOverallItemCount(int overallItemCount)
        {
            OverallItemCount = overallItemCount;
            UpdateItemGroupSelector();
        }
        private void UpdateItemGroupSelector()
        {
            IsItemGroupSelectorUpdating = true;
            ItemGroupSelector.Maximum = Math.Max((int)Math.Ceiling(OverallItemCount / (double)ItemCount), 1);
            int biggestGroup = OverallItemCount / Convert.ToInt32(ItemCountSelector.Items[0]);
            ItemGroupSelector.Width = TextRenderer.MeasureText(biggestGroup.ToString(), ItemGroupSelector.Font).Width + SystemInformation.VerticalScrollBarWidth;
            ItemGroupSelector.Value = ItemGroupSelector.Minimum;
            IsItemGroupSelectorUpdating = false;
        }

        private void ItemCountSelector_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            ItemCount = Convert.ToInt32(ItemCountSelector.SelectedItem);
            UpdateItemGroupSelector();
            SortingChanged?.Invoke(this, null);
        }
        private void ItemGroupSelector_ValueChanged(object sender, EventArgs evt)
        {
            ItemGroup = Convert.ToInt32(ItemGroupSelector.Value);
            if (!IsItemGroupSelectorUpdating)
            {
                SortingChanged?.Invoke(this, null);
            }
        }
        private void DirectionSelector_Click(object sender, EventArgs evt)
        {
            IsAscending = !DirectionSelector.Checked;
            SortingChanged?.Invoke(this, null);
        }
        private void OriginOrderSelector_CheckedChanged(object sender, EventArgs evt)
        {
            if (OriginOrderSelector.Checked)
            {
                SortingType = SortingTypes.Original;
                SortingChanged?.Invoke(this, null);
            }
        }
        private void AlphabeticOrderSelector_CheckedChanged(object sender, EventArgs evt)
        {
            if (AlphabeticOrderSelector.Checked)
            {
                SortingType = SortingTypes.Alphabetical;
                SortingChanged?.Invoke(this, null);
            }
        }
    }
    public class EcfDataIndicator : EcfToolGroup
    {
        private Label ConfigType { get; } = new Label();
        private ProgressIndicator Indicator { get; } = new ProgressIndicator();

        public EcfDataIndicator(string configType)
        {
            ConfigType.AutoSize = true;
            ConfigType.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            ConfigType.Dock = DockStyle.Fill;
            ConfigType.Text = configType;
            ConfigType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            Indicator.Size = new Size(ConfigType.Height, ConfigType.Height);
            Indicator.SetParameter(VisualModes.Circle);

            Add(ConfigType);
            Add(Indicator);
        }

        public void Activate()
        {
            Indicator.Activate();
        }
        public void Deactivate()
        {
            Indicator.Deactivate();
        }
    }

    public class CheckComboBox : Panel
    {
        public event EventHandler SelectionChangeCommitted;

        private int InitWidth { get; } = 100;
        private int InitHeight { get; } = 20;

        private ToolTip BoxToolTip { get; } = new ToolTip();
        private TextBox ResultBox { get; } = new TextBox();
        private DropDownButton DropButton { get; } = new DropDownButton();
        private DropDownList ItemList { get; }

        public string ToolTipText { get; }
        public string LocalisedOf { get; }
        public string LocalisedName { get; }
        public int MaxDropDownItems { get; set; } = 5;

        public CheckComboBox(string TypeText, string toolTip)
        {
            ToolTipText = toolTip;
            LocalisedOf = Properties.texts.FilterSelector_Of;
            LocalisedName = TypeText;

            Size = new Size(InitWidth, InitHeight);

            ResultBox.ReadOnly = true;
            ResultBox.Margin = new Padding(0);
            ResultBox.Location = new Point(0, 0);

            DropButton.Margin = new Padding(0);
            DropButton.DropStateChanged += OnDropStateChanged;

            ItemList = new DropDownList(Properties.texts.FilterSelector_ChangeAll);
            ItemList.SetParent(this);

            ResultBox.MouseHover += OnResultBoxMouseHover;
            ItemList.ItemStateChanged += OnItemStateChanged;
            ItemList.DropDownFocusLost += OnDropDownFocusLost;

            Controls.Add(ResultBox);
            Controls.Add(DropButton);
        }

        private void OnResultBoxMouseHover(object sender, EventArgs e)
        {
            BoxToolTip.SetToolTip(ResultBox, ToolTipText);
        }
        private void OnDropDownFocusLost(object sender, EventArgs evt)
        {
            if (!DropButton.IsUnderCursor)
            {
                DropButton.Reset();
            }
        }
        private void OnDropStateChanged(object sender, EventArgs evt)
        {
            if (DropButton.State == ComboBoxState.Pressed)
            {
                ItemList.ShowPopup(this);
            }
            else
            {
                ItemList.Hide();
                SelectionChangeCommitted?.Invoke(this, null);
            }
        }
        private void OnItemStateChanged(object sender, ItemCheckEventArgs evt)
        {
            UpdateResult();
        }
        protected override void OnResize(EventArgs evt)
        {
            ResultBox.Size = new Size(Width - Height, Height);
            DropButton.Location = new Point(ResultBox.Width, 0);
            DropButton.Size = new Size(Height, Height);
            base.OnResize(evt);
        }
        private void UpdateResult()
        {
            ResultBox.Text = ToString();
            using (Graphics gfx = ResultBox.CreateGraphics())
            {
                gfx.PageUnit = GraphicsUnit.Pixel;
                float width = gfx.MeasureString(ResultBox.Text, ResultBox.Font).Width;
                if (width > ResultBox.Width)
                {
                    Width = (int)width + DropButton.Width;
                }
            }
        }

        public void Reset()
        {
            ItemList.Reset();
        }
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", GetCheckedItems().Count, LocalisedOf, GetItems().Count, LocalisedName);
        }
        public string GetResult()
        {
            return ResultBox.Text;
        }
        public List<CheckableNameItem> GetItems()
        {
            return ItemList.GetItems();
        }
        public List<CheckableNameItem> GetCheckedItems()
        {
            return ItemList.GetCheckedItems();
        }
        public List<CheckableNameItem> GetUncheckedItems()
        {
            return ItemList.GetUncheckedItems();
        }
        public void SetItems(List<CheckableNameItem> items)
        {
            if (items != null)
            {
                items.Sort();
                ItemList.SetItems(items);
                UpdateResult();
            }
        }
        public bool IsItemChecked(string itemId)
        {
            return ItemList.GetCheckedItems().Any(item => item.Id.Equals(itemId));
        }

        private class DropDownList : Form
        {
            public event ItemCheckEventHandler ItemStateChanged;
            public event EventHandler DropDownFocusLost;

            private Control ParentControl { get; set; } = null;
            private CheckedListBox ItemList { get; } = new CheckedListBox();

            public string LocalisedChangeAll { get; }

            public DropDownList(string changAllText) : base()
            {
                AutoSize = true;
                AutoSizeMode = AutoSizeMode.GrowAndShrink;
                FormBorderStyle = FormBorderStyle.None;
                LocalisedChangeAll = changAllText;
                ShowInTaskbar = false;
                StartPosition = FormStartPosition.Manual;
                
                ItemList.IntegralHeight = true;
                ItemList.CheckOnClick = true;
                ItemList.Margin = new Padding(0);
                ItemList.ItemCheck += OnItemStateChanged;
                ItemList.LostFocus += OnLostFocus;

                Controls.Add(ItemList);
            }

            private void OnLostFocus(object sender, EventArgs evt)
            {
                DropDownFocusLost?.Invoke(this, null);
            }
            private void OnItemStateChanged(object sender, ItemCheckEventArgs evt)
            {
                if (ItemList.Items[evt.Index] is CheckableItem item)
                {
                    item.State = (evt.NewValue == CheckState.Checked);
                    if (evt.Index == 0)
                    {
                        ChangeAllStates(item.State);
                    }
                    ItemStateChanged?.Invoke(item, evt);
                }
            }
            private void ChangeAllStates(bool state)
            {
                for (int i = 1; i < ItemList.Items.Count; i++) {
                    ItemList.SetItemChecked(i, state);
                }
            }
            private void ResizeDropDown()
            {
                if (ParentControl is CheckComboBox box && ItemList.Items.Count > 0)
                {
                    int itemCount = Math.Min(ItemList.Items.Count, box.MaxDropDownItems);
                    int height = (ItemList.GetItemHeight(0) + 5) * itemCount;

                    int width = 0;
                    using (Graphics gfx = ItemList.CreateGraphics())
                    {
                        gfx.PageUnit = GraphicsUnit.Pixel;
                        foreach (CheckableItem item in ItemList.Items)
                        {
                            width = Math.Max(width, (int)gfx.MeasureString(item.Name.Display, ItemList.Font).Width);
                        }
                        if (ItemList.Items.Count > box.MaxDropDownItems)
                        {
                            width += SystemInformation.VerticalScrollBarWidth;
                        }
                        width += CheckBoxRenderer.GetGlyphSize(gfx, CheckBoxState.CheckedNormal).Width + 5;
                        width = Math.Max(width, box.Width);
                    }

                    ItemList.Size = new Size(width, height);
                }
            }

            public void Reset()
            {
                ItemList.SetItemChecked(0, true);
                ChangeAllStates(true);
            }
            public void SetParent(Control parent)
            {
                ParentControl = parent;
            }
            public void SetItems(List<CheckableNameItem> items)
            {
                ItemList.BeginUpdate();
                ItemList.Items.Clear();
                ItemList.Items.Add(new CheckableItem(string.Format("#{0}", LocalisedChangeAll), true));
                ItemList.Items.AddRange(items.Select(item => new CheckableItem(item, true)).ToArray());
                Reset();
                ResizeDropDown();
                ItemList.EndUpdate();
            }
            public void ShowPopup(IWin32Window parent)
            {
                if (ParentControl is CheckComboBox box)
                {
                    Location = box.PointToScreen(new Point(0, box.Height));
                    Width = Math.Max(Width, box.Width);
                }
                Show(parent);
            }
            public List<CheckableNameItem> GetItems()
            {
                return ItemList.Items.Cast<CheckableItem>().Skip(1).Select(item => item.Name).ToList();
            }
            public List<CheckableNameItem> GetCheckedItems()
            {
                return ItemList.Items.Cast<CheckableItem>().Skip(1).Where(item => item.State == true).Select(item => item.Name).ToList();
            }
            public List<CheckableNameItem> GetUncheckedItems()
            {
                return ItemList.Items.Cast<CheckableItem>().Skip(1).Where(item => item.State == false).Select(item => item.Name).ToList();
            }
        }
        public class CheckableNameItem : IComparable
        {
            public string Id { get; }
            public string Display { get; }
            public CheckableNameItem(string id, string display)
            {
                Id = id;
                Display = display;
            }
            public CheckableNameItem(string display) : this(display, display)
            {

            }

            public int CompareTo(object obj)
            {
                if (obj is CheckableNameItem nameItem)
                {
                    return Id.CompareTo(nameItem.Id);
                }
                return 1;
            }
        }
        public class CheckableItem
        {
            public CheckableNameItem Name { get; }
            public bool State { get; set; }

            public CheckableItem(CheckableNameItem name, bool initState)
            {
                Name = name;
                State = initState;
            }
            public CheckableItem(string id, string displayName, bool initState) : this(new CheckableNameItem(id, displayName), initState)
            {
                
            }
            public CheckableItem(string displayName, bool initState) : this(new CheckableNameItem(displayName), initState)
            {

            }

            public override string ToString()
            {
                return Name.Display;
            }
        }
        private class DropDownButton : ButtonBase
        {
            public event EventHandler DropStateChanged;

            public bool IsUnderCursor { get; private set; } = false;

            public ComboBoxState State { get; private set; } = ComboBoxState.Normal;

            protected override void OnPaint(PaintEventArgs evt)
            {
                ComboBoxRenderer.DrawDropDownButton(evt.Graphics, evt.ClipRectangle, State);
            }

            public void Reset()
            {
                if (!State.Equals(ComboBoxState.Normal))
                {
                    State = ComboBoxState.Normal;
                    DropStateChanged?.Invoke(this, null);
                    Invalidate();
                }
            }
            protected override void OnClick(EventArgs evt)
            {
                ComboBoxState oldState = State;
                switch (State)
                {
                    case ComboBoxState.Normal: State = ComboBoxState.Pressed; break;
                    default: State = ComboBoxState.Normal; break;
                }
                if (!State.Equals(oldState))
                {
                    DropStateChanged?.Invoke(this, null);
                    Invalidate();
                }
            }
            protected override void OnMouseEnter(EventArgs e)
            {
                IsUnderCursor = true;
            }
            protected override void OnMouseLeave(EventArgs e)
            {
                IsUnderCursor = false;
            }
        }
    }
    public class ProgressIndicator : Control
    {
        public bool IsActive { get; private set; } = false;
        public bool IsAutomaticCounting { get; private set; } = true;
        public bool IsUpcounting { get; private set; } = true;
        public int AutomaticCountInterval { get; private set; } = 25;
        public int Counter { get; private set; } = 0;
        public int CountStep { get; private set; } = 1;
        public int MinCount { get; private set; } = 0;
        public int MaxCount { get; private set; } = 100;
        public VisualModes VisualMode { get; private set; } = VisualModes.Default;

        private bool Reverse { get; set; } = false;
        private System.Timers.Timer Updater { get; } = new System.Timers.Timer();
        private Pen CircleBackgroundPen { get; } = new Pen(SystemBrushes.ActiveBorder, 4)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
        };
        private Pen CircleForegroundPen { get; } = new Pen(SystemBrushes.HotTrack, 3)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
        };

        public enum VisualModes
        {
            Default,
            Circle,
            DotArray,
        }

        public ProgressIndicator() : base()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable | ControlStyles.ContainerControl | ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, false);

            int length = new Button().Height;
            Size = new Size(length, length);

            Updater.Enabled = true;
            Updater.AutoReset = true;
            Updater.Elapsed += Updater_Elapsed;
        }

        public void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
                InitCounter();
                if (IsAutomaticCounting) {
                    Updater.Interval = AutomaticCountInterval;
                    Updater.Start(); 
                }
            }
        }
        public void Deactivate()
        {
            if (IsActive)
            {
                IsActive = false;
                Updater.Stop();
                Reset();
            }
        }
        public void PerformStep()
        {
            if (!IsAutomaticCounting)
            {
                UpdateCounter();
            }
        }
        public void SetParameter(int step, int min, int max)
        {
            if (!IsActive)
            {
                CountStep = step;
                MinCount = min;
                MaxCount = max;
            }
        }
        public void SetParameter(bool automatic, bool upCounting)
        {
            if (!IsActive)
            {
                IsAutomaticCounting = automatic;
                IsUpcounting = upCounting;
            }
        }
        public void SetParameter(int automaticInterval)
        {
            if (!IsActive)
            {
                AutomaticCountInterval = automaticInterval;
            }
        }
        public void SetParameter(VisualModes mode)
        {
            if (!IsActive)
            {
                VisualMode = mode;
            }
        }

        private void InitCounter()
        {
            Reverse = false;
            Counter = IsUpcounting ? MinCount : MaxCount;
        }
        private void Updater_Elapsed(object sender, ElapsedEventArgs evt)
        {
            UpdateCounter();
        }
        private void UpdateCounter()
        {
            Counter += CountStep * (IsUpcounting ? 1 : -1);
            if (IsAutomaticCounting)
            {
                if (Counter < MinCount) { Counter = MaxCount; Reverse = !Reverse; }
                if (Counter > MaxCount) { Counter = MinCount; Reverse = !Reverse; }
            }
            else
            {
                if (Counter < MinCount || Counter > MaxCount) {
                    Deactivate();
                    return;
                }
            }
            Invalidate();
        }
        private void Reset()
        {
            InitCounter();
            Invalidate();
        }

        private void VisualMode_Default(Graphics gfx)
        {
            gfx.FillEllipse(SystemBrushes.ActiveBorder, ClientRectangle);
            gfx.FillPie(SystemBrushes.HotTrack, ClientRectangle, -90, 360 * Counter / MaxCount);
        }
        private void VisualMode_Circle(Graphics gfx)
        {
            int penWidth = (int)Math.Max(CircleBackgroundPen.Width, CircleForegroundPen.Width);
            int penHalfWidth = penWidth / 2;
            Rectangle area = new Rectangle(
                ClientRectangle.X + penHalfWidth,
                ClientRectangle.Y + penHalfWidth,
                ClientRectangle.Width - penWidth,
                ClientRectangle.Height - penWidth);
            float angle = 360 * Counter / MaxCount;
            gfx.DrawEllipse(CircleBackgroundPen, area);
            if (!Reverse)
            {
                gfx.DrawArc(CircleForegroundPen, area, -90, angle);
            }
            else
            {
                gfx.DrawArc(CircleForegroundPen, area, -90 + angle, 360 - angle);
            }
        }
        private void VisualMode_DotArray(Graphics gfx)
        {
            Rectangle area = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1);

            int dotGap = (int)Math.Floor(area.Width / (double)15);
            int dotSize = dotGap * 5;
            int dotDistance = dotSize + dotGap;
            int dotXCount = (int)Math.Floor(area.Width / (double)dotDistance);
            int dotYCount = (int)Math.Floor(area.Height / (double)dotDistance);
            int dotCap = (int)Math.Floor(dotXCount * dotYCount * (Counter / (double)MaxCount));

            Rectangle dot = new Rectangle(0, 0, dotSize, dotSize);
            int dotCount = 0;
            for (int y = 0; y < dotYCount; y++)
            {
                dot.Y = y * dotDistance;
                for (int x = 0; x < dotXCount; x++)
                {
                    dot.X = x * dotDistance;
                    if (dotCount < dotCap)
                    {
                        gfx.FillEllipse(SystemBrushes.HotTrack, dot);
                    }
                    dotCount++;
                }
            }
        }
        
        protected override void OnPaint(PaintEventArgs evt)
        {
            base.OnPaint(evt);
            
            Graphics gfx = evt.Graphics;
            gfx.SmoothingMode = SmoothingMode.HighQuality;

            switch (VisualMode)
            {
                case VisualModes.DotArray: VisualMode_DotArray(gfx); return;
                case VisualModes.Circle: VisualMode_Circle(gfx); return;
                default: VisualMode_Default(gfx); return;
            }
        }
    }
}
