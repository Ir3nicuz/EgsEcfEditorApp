using EcfFileViews;
using EcfWinFormControls;
using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

                    string elementName = block.GetAttributeFirstValue(UserSettings.Default.EcfTechTreeDialog_ParameterKey_ReferenceName);
                    string techTreeParent = block.GetParameterFirstValue(TechTreeParentParameterKey);
                    string unlockLevel = block.GetParameterFirstValue(UnlockLevelParameterKey);
                    string unlockCost = block.GetParameterFirstValue(UnlockCostParameterKey);

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
                                treePage = new EcfTechTree(treeName);
                                TechTreePageContainer.TabPages.Add(treePage);
                            }

                            treePage.Add(new ElementNode(block, elementName, techTreeParent, unlockLevelValue, unlockCostValue));
                        }
                    }
                    else if (unlockLevel != null || unlockCost != null || techTreeParent != null)
                    {
                        UnattachedElementsTreeView.Nodes.Add(new ElementNode(block, elementName, techTreeParent, unlockLevelValue, unlockCostValue));
                    }
                }
            });



            EcfBlock element = UniqueFileTabs.FirstOrDefault().File.GetDeepItemList<EcfBlock>().FirstOrDefault();
            ElementNode node1 = new ElementNode(element , "inge", "horst", 0, 258);
            ElementNode node2 = new ElementNode(element , "gerda", "kunibert", 0, 12);
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

            public EcfTechTree(string name)
            {
                TreeName = name ?? string.Empty;
                Text = TreeName;

                ElementTreeView.AllowDrop = true;
                ElementTreeView.Dock = DockStyle.Fill;
                ElementTreeView.ShowNodeToolTips = true;

                ElementTreeView.DragEnter += ElementTreeView_DragEnter;
                ElementTreeView.DragDrop += ElementTreeView_DragDrop;
                ElementTreeView.ItemDrag += ElementTreeView_ItemDrag;

                Controls.Add(ElementTreeView);
            }

            // events
            private void ElementTreeView_DragEnter(object sender, DragEventArgs evt)
            {
                // specifies the acceptance of the drop operation
                evt.Effect = DragDropEffects.Move;
            }
            [Obsolete("needs work.")]
            private void ElementTreeView_DragDrop(object sender, DragEventArgs evt)
            {
                ElementNode sourceNode = (ElementNode)evt.Data.GetData(typeof(ElementNode));




                string techTreeParentName = GetNodeByCursor(new Point(evt.X, evt.Y))?.ElementName;

                // -> block parameter tree names ergänzen -> TreeName -> SetParameter (key: create if not exist, check if allowed, value: forceuniqueness?) 
                // -> tree parent parameter auf basis des dropnode aktualisieren -> ElementName -> SetParameter (key: create if not exist, check if allowed, value: isSingle?)




                if (sourceNode != null)
                {
                    Add(sourceNode);
                    evt.Effect = DragDropEffects.Move;
                    return;
                }
                evt.Effect = DragDropEffects.None;
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

            // publics
            public void Add(ElementNode elementNode)
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
                    parent.Nodes.Add(elementNode);
                }
                else
                {
                    ElementTreeView.Nodes.Add(elementNode);
                }

                ElementTreeView.EndUpdate();
            }

            // privates
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
            private ElementNode GetNodeByCursor(Point cursorPosition)
            {
                Point position = ElementTreeView.PointToClient(cursorPosition);
                TreeNode node = ElementTreeView.GetNodeAt(position);
                if (node != null)
                {
                    Rectangle innerNodeBounds = node.Bounds;
                    int borderThickness = innerNodeBounds.Height * 10 / 100;
                    innerNodeBounds.Offset(0, borderThickness);
                    innerNodeBounds.Inflate(0, -2 * borderThickness);

                    if (innerNodeBounds.Contains(position) && node is ElementNode receiverNode)
                    {
                        return receiverNode;
                    }
                    else if (node.Parent is ElementNode receiverParentNode)
                    {
                        return receiverParentNode;
                    }
                }
                return null;
            }
        }
        private class ElementNode : TreeNode
        {
            public string ElementName { get; }
            public string TechTreeParentName { get; set; }
            public int UnlockLevel { get; private set; }
            public int UnlockCost { get; private set; }

            public EcfBlock Element { get; }

            public ElementNode(EcfBlock element, string elementName, string techTreeParentName, int unlockLevel, int unlockCost)
            {
                Element = element;
                ElementName = elementName ?? TitleRecources.Generic_Replacement_Empty;
                TechTreeParentName = techTreeParentName;
                UnlockLevel = unlockLevel;
                UnlockCost = unlockCost;

                ToolTipText = element?.BuildIdentification() ?? TitleRecources.Generic_Replacement_Empty;

                UpdateNodeText();
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
    }
}
