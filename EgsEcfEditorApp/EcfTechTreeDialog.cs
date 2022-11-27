using EcfFileViews;
using EcfWinFormControls;
using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EgsEcfEditorApp
{
    public partial class EcfTechTreeDialog : Form
    {
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
        }

        // public
        public DialogResult ShowDialog(IWin32Window parent, List<EcfTabPage> openedFileTabs)
        {
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
        private void UpdateTechTrees()
        {
            string unlockLevelKey = UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockLevel;
            string unlockCostKey = UserSettings.Default.EcfTechTreeDialog_ParameterKey_UnlockCost;
            string techTreeNamesKey = UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeNames;
            string techTreeParentKey = UserSettings.Default.EcfTechTreeDialog_ParameterKey_TechTreeParent;
            
            TechTreePageContainer.SuspendLayout();
            TechTreePageContainer.TabPages.Clear();

            UniqueFileTabs.ForEach(tab =>
            {
                foreach(EcfBlock block in tab.File.ItemList.Where(item => item is EcfBlock))
                {
                    block.HasParameter(unlockLevelKey, out EcfParameter unlockLevel);
                    block.HasParameter(unlockCostKey, out EcfParameter unlockCost);
                    block.HasParameter(techTreeNamesKey, out EcfParameter techTreeNames);
                    block.HasParameter(techTreeParentKey, out EcfParameter techTreeParent);

                    if (techTreeNames != null && unlockLevel != null && unlockCost != null)
                    {
                        // supposed to be listed
                        foreach (string treeName in techTreeNames.GetAllValues())
                        {
                            EcfTechTree treePage = TechTreePageContainer.TabPages.Cast<EcfTechTree>().FirstOrDefault(tree => tree.Text.Equals(treeName));
                            
                            if (treePage == null)
                            {
                                treePage = new EcfTechTree(treeName, Tip);
                                TechTreePageContainer.TabPages.Add(treePage);
                            }

                            treePage.Add(new TechTreeElement(unlockLevel.GetFirstValue(), unlockCost.GetFirstValue(), techTreeParent?.GetFirstValue(), block));
                        }
                    }
                    else if (techTreeNames != null || unlockLevel != null || unlockCost != null || techTreeParent != null)
                    {
                        // incomplete





                    }
                }

            });

            TechTreePageContainer.ResumeLayout();
        }

        // subclasses
        private class EcfTechTree : TabPage
        {
            private EcfTreeView ElementTreeView { get; } = new EcfTreeView();
            private EcfTreeView UnlockLevelListView { get; } = new EcfTreeView();
            private EcfTreeView UnlockCostListView { get; } = new EcfTreeView();

            private TableLayoutPanel ViewPanel { get; } = new TableLayoutPanel();
            private ToolTip ToolTipContainer { get; }

            public EcfTechTree(string name, ToolTip toolTipContainer)
            {
                Text = name;
                ToolTipContainer = toolTipContainer;

                ElementTreeView.LinkTreeView(UnlockLevelListView);
                ElementTreeView.LinkTreeView(UnlockCostListView);

                ElementTreeView.Dock = DockStyle.Fill;
                UnlockLevelListView.Dock = DockStyle.Fill;
                UnlockCostListView.Dock = DockStyle.Fill;
                ViewPanel.Dock = DockStyle.Fill;

                ElementTreeView.ShowPlusMinus = false;
                UnlockLevelListView.ShowPlusMinus = false;
                UnlockLevelListView.ShowRootLines = false;
                UnlockCostListView.ShowPlusMinus = false;
                UnlockCostListView.ShowRootLines = false;

                ViewPanel.ColumnCount = 3;
                ViewPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                ViewPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0.2f));
                ViewPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0.2f));
                ViewPanel.RowCount = 2;
                ViewPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                ViewPanel.RowStyles.Add(new RowStyle(SizeType.Percent,1.0f));
                ViewPanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;

                ViewPanel.Controls.Add(new Label() { Text = "Name", TextAlign = ContentAlignment.MiddleCenter }, 0, 0);
                ViewPanel.Controls.Add(new Label() { Text = "Level", TextAlign = ContentAlignment.MiddleCenter }, 1, 0);
                ViewPanel.Controls.Add(new Label() { Text = "Cost", TextAlign = ContentAlignment.MiddleCenter }, 2, 0);
                ViewPanel.Controls.Add(ElementTreeView, 0, 1);
                ViewPanel.Controls.Add(UnlockLevelListView, 1, 1);
                ViewPanel.Controls.Add(UnlockCostListView, 2, 1);
                Controls.Add(ViewPanel);
            }

            // publics
            public void Add(TechTreeElement element)
            {
                ElementTreeView.BeginUpdate();

                ElementTreeView.Nodes.Cast<TechTreeElement>().ToList().ForEach(unboundElement =>
                {
                    if (!string.IsNullOrEmpty(unboundElement.TechTreeParentName) && string.Equals(unboundElement.TechTreeParentName, element.ElementName))
                    {
                        element.Nodes.Add(unboundElement);
                        ElementTreeView.Nodes.Remove(unboundElement);
                    }
                });

                TechTreeElement parent = FindParent(element.TechTreeParentName, ElementTreeView.Nodes);
                if (parent != null)
                {
                    parent.Nodes.Add(element);
                }
                else
                {
                    ElementTreeView.Nodes.Add(element);
                }

                ElementTreeView.ExpandAll();
                ElementTreeView.EndUpdate();



                // still unsorted/unlinked
                UnlockLevelListView.Nodes.Add(element.UnlockLevel);
                UnlockCostListView.Nodes.Add(element.UnlockCost);



            }

            // privates
            private TechTreeElement FindParent(string parentName, TreeNodeCollection nodes)
            {
                foreach (TechTreeElement node in nodes.Cast<TechTreeElement>())
                {
                    if (string.Equals(node.ElementName, parentName))
                    {
                        return node;
                    }
                    TechTreeElement parent = FindParent(parentName, node.Nodes);
                    if (parent != null)
                    {
                        return parent;
                    }
                }
                return null;
            }
        }
        private class TechTreeElement : TreeNode
        {
            public string UnlockLevel { get; }
            public string UnlockCost { get; }
            public string ElementName { get; }
            public string TechTreeParentName { get; }

            public EcfBlock Element { get; }

            public TechTreeElement(string unlockLevel, string unlockCost, string techTreeParentName, EcfBlock element)
            {
                UnlockLevel = unlockLevel;
                UnlockCost = unlockCost;
                ElementName = element.GetAttributeFirstValue("Name");
                TechTreeParentName = techTreeParentName;
                Element = element;

                Text = element.BuildIdentification();
            }

        }
    }
}
