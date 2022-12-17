using EcfFileViews;
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
        private string UnlockLevelParameterKey { get; set; } = null;
        private string UnlockCostParameterKey { get; set; } = null;
        private string TechTreeNamesParameterKey { get; set; } = null;
        private string TechTreeParentParameterKey { get; set; } = null;
        public HashSet<EcfTabPage> ChangedFileTabs { get; } = new HashSet<EcfTabPage>();
        private List<EcfTabPage> UniqueFileTabs { get; } = new List<EcfTabPage>();

        private EcfItemSelectorDialog FileTabSelector { get; } = new EcfItemSelectorDialog();

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
        }
        private void UnattachedElementsTreeView_ItemDrag(object sender, ItemDragEventArgs evt)
        {
            if (evt.Item is UnattachedElementNode node)
            {
                if (DoDragDrop(node, DragDropEffects.Move) == DragDropEffects.Move)
                {
                    UnattachedElementsTreeView.Nodes.Remove(node);
                }
            }
        }

        // public
        public DialogResult ShowDialog(IWin32Window parent, List<EcfTabPage> openedFileTabs)
        {
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

        // private
        private DialogResult UpdateUniqueFileTabs(List<EcfTabPage> openedFileTabs)
        {
            UniqueFileTabs.Clear();
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
                    block.HasParameter(TechTreeNamesParameterKey, out EcfParameter techTreeNames);

                    string techTreeParent = block.GetParameterFirstValue(TechTreeParentParameterKey);
                    string unlockLevel = block.GetParameterFirstValue(UnlockLevelParameterKey);
                    string unlockCost = block.GetParameterFirstValue(UnlockCostParameterKey);

                    if (techTreeNames != null)
                    {
                        foreach (string treeName in techTreeNames.GetAllValues())
                        {
                            EcfTechTree treePage = TechTreePageContainer.TabPages.Cast<EcfTechTree>().FirstOrDefault(tree => tree.Text.Equals(treeName));
                            
                            if (treePage == null)
                            {
                                treePage = new EcfTechTree(treeName);
                                TechTreePageContainer.TabPages.Add(treePage);
                            }

                            treePage.Add(block, techTreeParent, unlockLevel, unlockCost);
                        }
                    }
                    else if (unlockLevel != null || unlockCost != null || techTreeParent != null)
                    {
                        UnattachedElementsTreeView.Nodes.Add(new UnattachedElementNode(block, techTreeParent, unlockLevel, unlockCost));
                    }
                }
            });



            EcfBlock element = UniqueFileTabs.FirstOrDefault().File.GetDeepItemList<EcfBlock>().FirstOrDefault();
            UnattachedElementNode node1 = new UnattachedElementNode(element , "horst", "gerda", "inge");
            UnattachedElementNode node2 = new UnattachedElementNode(element , "kunibert", "elfriede", "otto");
            UnattachedElementsTreeView.Nodes.Add(node1);
            UnattachedElementsTreeView.Nodes.Add(node2);
            


            UnattachedElementsGroupBox.Visible = UnattachedElementsTreeView.Nodes.Count != 0;
            UnattachedElementsTreeView.EndUpdate();
            TechTreePageContainer.ResumeLayout();
        }

        // subclasses
        private class EcfTechTree : TabPage
        {
            private EcfTreeView ElementTreeView { get; } = new EcfTreeView();
            private EcfTreeView UnlockLevelListView { get; } = new EcfTreeView();
            private EcfTreeView UnlockCostListView { get; } = new EcfTreeView();

            private TableLayoutPanel ViewPanel { get; } = new TableLayoutPanel();

            public EcfTechTree(string name)
            {
                Text = name ?? string.Empty;

                ElementTreeView.LinkTreeView(UnlockLevelListView);
                ElementTreeView.LinkTreeView(UnlockCostListView);

                ViewPanel.Dock = DockStyle.Fill;

                InitTreeView(ElementTreeView);
                InitTreeView(UnlockLevelListView);
                InitTreeView(UnlockCostListView);

                ElementTreeView.NodeMouseClick += ElementTreeView_NodeMouseClick;
                UnlockLevelListView.NodeMouseClick += UnlockLevelListView_NodeMouseClick;
                UnlockCostListView.NodeMouseClick += UnlockCostListView_NodeMouseClick;

                ViewPanel.ColumnCount = 3;
                ViewPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1.0f));
                ViewPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                ViewPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                ViewPanel.RowCount = 2;
                ViewPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                ViewPanel.RowStyles.Add(new RowStyle(SizeType.Percent,1.0f));
                ViewPanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;

                ViewPanel.Controls.Add(new Label() { Text = TitleRecources.Generic_Name, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 0);
                ViewPanel.Controls.Add(new Label() { Text = TitleRecources.Generic_Level, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill }, 1, 0);
                ViewPanel.Controls.Add(new Label() { Text = TitleRecources.Generic_Cost, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill }, 2, 0);
                ViewPanel.Controls.Add(ElementTreeView, 0, 1);
                ViewPanel.Controls.Add(UnlockLevelListView, 1, 1);
                ViewPanel.Controls.Add(UnlockCostListView, 2, 1);
                Controls.Add(ViewPanel);
            }

            // events
            private void InitTreeView(EcfTreeView view)
            {
                view.AllowDrop = true;
                view.Dock = DockStyle.Fill;
                view.HideSelection = false;
                view.ShowPlusMinus = false;
                view.ShowRootLines = false;
                view.ShowNodeToolTips = true;

                view.DragEnter += View_DragEnter;
                view.DragDrop += View_DragDrop;
            }
            private void View_DragEnter(object sender, DragEventArgs evt)
            {
                // specifies the acceptance of the drop operation
                evt.Effect = DragDropEffects.Move;
            }
            [Obsolete("needs work.")]
            private void View_DragDrop(object sender, DragEventArgs evt)
            {
                if (sender is TreeView view)
                {
                    UnattachedElementNode sourceNode = (UnattachedElementNode)evt.Data.GetData(typeof(UnattachedElementNode));
                    TreeNode receiverNode = view.GetNodeAt(view.PointToClient(new Point(evt.X, evt.Y)));
                    if(sourceNode != null && receiverNode != null)
                    {
                        


                        if (sourceNode is UnattachedElementNode node)
                        {
                            Console.WriteLine(node.Text + " _ UnattachedElementNode dropped on _ " + receiverNode.Text);
                            evt.Effect = DragDropEffects.Move;
                            return;
                        }



                    }
                }
                evt.Effect = DragDropEffects.None;
            }

            private void ElementTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs evt)
            {
                Stack<int> indexPath = GetIndexPath(evt.Node);
                SelectIndexPath(indexPath, UnlockLevelListView);
                SelectIndexPath(indexPath, UnlockCostListView);
            }
            private void UnlockLevelListView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs evt)
            {
                Stack<int> indexPath = GetIndexPath(evt.Node);
                SelectIndexPath(indexPath, ElementTreeView);
                SelectIndexPath(indexPath, UnlockCostListView);
            }
            private void UnlockCostListView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs evt)
            {
                Stack<int> indexPath = GetIndexPath(evt.Node);
                SelectIndexPath(indexPath, ElementTreeView);
                SelectIndexPath(indexPath, UnlockLevelListView);
            }

            // publics
            public void Add(EcfBlock element, string techTreeParent, string unlockLevel, string unlockCost)
            {
                ElementNode elementNode = new ElementNode(element, techTreeParent);
                UnlockLevelNode levelNode = new UnlockLevelNode(element, unlockLevel);
                UnlockCostNode costNode = new UnlockCostNode(element, unlockCost);

                ElementTreeView.BeginUpdate();
                UnlockLevelListView.BeginUpdate();
                UnlockCostListView.BeginUpdate();

                ElementTreeView.Nodes.Cast<ElementNode>().ToList().ForEach(rootElement =>
                {
                    if (!string.IsNullOrEmpty(rootElement.TechTreeParentName) && string.Equals(rootElement.TechTreeParentName, elementNode.ElementName))
                    {
                        int index = ElementTreeView.Nodes.IndexOf(rootElement);

                        TreeNode levelParentNode = UnlockLevelListView.Nodes[index];
                        TreeNode costParentNode = UnlockCostListView.Nodes[index];

                        ElementTreeView.Nodes.RemoveAt(index);
                        UnlockLevelListView.Nodes.RemoveAt(index);
                        UnlockCostListView.Nodes.RemoveAt(index);

                        elementNode.Nodes.Add(rootElement);
                        levelNode.Nodes.Add(levelParentNode);
                        costNode.Nodes.Add(costParentNode);
                    }
                });

                ElementNode parent = FindParentNode(elementNode.TechTreeParentName, ElementTreeView.Nodes);
                if (parent != null)
                {
                    parent.Nodes.Add(elementNode);
                    Stack<int> indexPath = GetIndexPath(elementNode);
                    AddToIndexPath(indexPath, UnlockLevelListView, levelNode);
                    AddToIndexPath(indexPath, UnlockCostListView, costNode);
                }
                else
                {
                    ElementTreeView.Nodes.Add(elementNode);
                    UnlockLevelListView.Nodes.Add(levelNode);
                    UnlockCostListView.Nodes.Add(costNode);
                }

                ElementTreeView.ExpandAll();
                UnlockLevelListView.ExpandAll();
                UnlockCostListView.ExpandAll();

                ElementTreeView.EndUpdate();
                UnlockLevelListView.EndUpdate();
                UnlockCostListView.EndUpdate();
            }

            // privates
            private Stack<int> GetIndexPath(TreeNode node)
            {
                Stack<int> indexPath = new Stack<int>();

                while (node != null)
                {
                    indexPath.Push(node.Index);
                    node = node.Parent;
                }
                
                return indexPath;
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
            private void AddToIndexPath(Stack<int> indexPath, TreeView view, TreeNode node)
            {
                TreeNodeCollection nodes = view.Nodes;
                if (indexPath != null && indexPath.Count > 0)
                {
                    Stack<int> path = new Stack<int>(indexPath.Reverse());
                    while (path.Count > 1)
                    {
                        nodes = nodes[path.Pop()].Nodes;
                    }
                    nodes.Insert(path.Pop(), node);
                }
                else
                {
                    nodes.Add(node);
                }
            }
            private void SelectIndexPath(Stack<int> indexPath, TreeView view)
            {
                TreeNodeCollection nodes = view.Nodes;
                if (indexPath != null && indexPath.Count > 0)
                {
                    Stack<int> path = new Stack<int>(indexPath.Reverse());
                    while (path.Count > 1)
                    {
                        nodes = nodes[path.Pop()].Nodes;
                    }
                    view.SelectedNode = nodes[path.Pop()];
                }
            }
        }
        private class ElementNode : TreeNode
        {
            public string ElementName { get; }
            public string TechTreeParentName { get; }

            public EcfBlock Element { get; }

            public ElementNode(EcfBlock element, string techTreeParent)
            {
                ElementName = element.GetAttributeFirstValue(UserSettings.Default.EcfTechTreeDialog_ParameterKey_ReferenceName);
                TechTreeParentName = techTreeParent;
                Text = ElementName;

                Element = element;
                ToolTipText = element.BuildIdentification();
            }
        }
        private class UnlockLevelNode : TreeNode
        {
            public EcfBlock Element { get; }

            public UnlockLevelNode(EcfBlock element, string unlockLevel)
            {
                Text = unlockLevel ?? UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockLevel.ToString();

                Element = element;
                ToolTipText = element.BuildIdentification();
            }
        }
        private class UnlockCostNode : TreeNode
        {
            public EcfBlock Element { get; }

            public UnlockCostNode(EcfBlock element, string unlockCost)
            {
                Text = unlockCost ?? UserSettings.Default.EcfTechTreeDialog_DefaultValue_UnlockCost.ToString();

                Element = element;
                ToolTipText = element.BuildIdentification();
            }
        }
        private class UnattachedElementNode : TreeNode
        {
            public string ElementName { get; }
            public string TechTreeParentName { get; }
            public string UnlockLevel { get; }
            public string UnlockCost { get; }

            public EcfBlock Element { get; }

            public UnattachedElementNode(EcfBlock element, string techTreeParent, string unlockLevel, string unlockCost)
            {
                Element = element;
                ElementName = element.GetAttributeFirstValue(UserSettings.Default.EcfTechTreeDialog_ParameterKey_ReferenceName);
                UnlockLevel = unlockLevel;
                UnlockCost = unlockCost;
                TechTreeParentName = techTreeParent;

                Text = ElementName;
                ToolTipText = element.BuildIdentification();
            }
        }
    }
}
