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
        public string UnlockLevelParameterKey { get; private set; } = null;
        public string UnlockCostParameterKey { get; private set; } = null;
        public string TechTreeNamesParameterKey { get; private set; } = null;
        public string TechTreeParentParameterKey { get; private set; } = null;
        public HashSet<EcfTabPage> ChangedFileTabs { get; } = new HashSet<EcfTabPage>();
        private List<EcfTabPage> UniqueFileTabs { get; } = new List<EcfTabPage>();
        protected ElementNode LastCopiedElement { get; set; } = null;

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
                                treePage = new EcfTechTree(this, treeName);
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
            private EcfTechTreeDialog ParentForm { get; }
            private ContextMenuStrip TechTreeOperation { get; } = new ContextMenuStrip();

            public EcfTechTree(EcfTechTreeDialog parentForm, string name)
            {
                ParentForm = parentForm;
                TreeName = name ?? string.Empty;
                Text = TreeName;

                ElementTreeView.AllowDrop = true;
                ElementTreeView.Dock = DockStyle.Fill;
                ElementTreeView.ShowNodeToolTips = true;

                ElementTreeView.DragEnter += ElementTreeView_DragEnter;
                ElementTreeView.DragDrop += ElementTreeView_DragDrop;
                ElementTreeView.ItemDrag += ElementTreeView_ItemDrag;
                ElementTreeView.KeyUp += ElementTreeView_KeyUp;
                ElementTreeView.KeyPress += ElementTreeView_KeyPress;
                ElementTreeView.NodeMouseClick += ElementTreeView_NodeMouseClick;
                ElementTreeView.NodeMouseDoubleClick += ElementTreeView_NodeMouseDoubleClick;

                Controls.Add(ElementTreeView);

                TechTreeOperation.Items.Add(TitleRecources.Generic_Change, IconRecources.Icon_ChangeSimple, (sender, evt) => NodeChangeClicked(sender, evt));
                TechTreeOperation.Items.Add(TitleRecources.Generic_Add, IconRecources.Icon_Add, (sender, evt) => NodeAddClicked(sender, evt));
                TechTreeOperation.Items.Add(TitleRecources.Generic_Copy, IconRecources.Icon_Copy, (sender, evt) => NodeCopiedClicked(sender, evt));
                TechTreeOperation.Items.Add(TitleRecources.Generic_Paste, IconRecources.Icon_Paste, (sender, evt) => NodePastedClicked(sender, evt));
                TechTreeOperation.Items.Add(TitleRecources.Generic_Remove, IconRecources.Icon_Remove, (sender, evt) => NodeRemoveClicked(sender, evt));
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
                if (sourceNode != null)
                {
                    try
                    {
                        UpdateTechTreeNames(sourceNode);
                        UpdateTechTreeParent(GetNodeByCursor(new Point(evt.X, evt.Y))?.ElementName, sourceNode);

                        Add(sourceNode);
                        evt.Effect = DragDropEffects.Move;
                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, string.Format("{0}:{1}{2}", TextRecources.EcfTechTreeDialog_ElementSettingsUpdateError, Environment.NewLine, ex.Message), 
                            TitleRecources.Generic_Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
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
            private void ElementTreeView_KeyUp(object sender, KeyEventArgs evt)
            {
                if (evt.KeyCode == Keys.Delete) { NodeRemoveClicked(sender, evt); evt.Handled = true; }
                else if (evt.Control && evt.KeyCode == Keys.C) { NodeCopiedClicked(sender, evt); evt.Handled = true; }
                else if (evt.Control && evt.KeyCode == Keys.V) { NodePastedClicked(sender, evt); evt.Handled = true; }
            }
            private void ElementTreeView_KeyPress(object sender, KeyPressEventArgs evt)
            {
                // hack for sqirky "ding"
                evt.Handled = true;
            }
            private void ElementTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs evt)
            {
                if (evt.Button == MouseButtons.Right)
                {
                    TechTreeOperation.Show(this, evt.Location);
                }
            }
            private void ElementTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs evt)
            {
                NodeChangeClicked(sender, evt);
            }
            [Obsolete("needs work")]
            private void NodeChangeClicked(object sender, EventArgs evt)
            {

            }
            [Obsolete("needs work")]
            private void NodeCopiedClicked(object sender, EventArgs evt)
            {
                
                
                ParentForm.LastCopiedElement = new ElementNode(null, "1", "2", 3, 4);


            }
            [Obsolete("needs work")]
            private void NodePastedClicked(object sender, EventArgs evt)
            {
                
                
                
                ElementNode source = ParentForm.LastCopiedElement;



            }
            [Obsolete("needs work")]
            private void NodeAddClicked(object sender, EventArgs evt)
            {

            }
            [Obsolete("needs work")]
            private void NodeRemoveClicked(object sender, EventArgs evt)
            {

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
            private void UpdateTechTreeParent(string techTreeParentName, ElementNode sourceNode)
            {
                EcfParameter parameter = sourceNode.Element.FindOrCreateParameter(ParentForm.TechTreeParentParameterKey);
                parameter.ClearValues();
                parameter.AddValue(techTreeParentName ?? string.Empty);
                
                sourceNode.TechTreeParentName = techTreeParentName;
            }
            private void UpdateTechTreeNames(ElementNode sourceNode)
            {
                EcfParameter parameter = sourceNode.Element.FindOrCreateParameter(ParentForm.TechTreeNamesParameterKey);
                if (!parameter.ContainsValue(TreeName))
                {
                    parameter.AddValue(TreeName);
                }
            }
            
        }
        protected class ElementNode : TreeNode
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
