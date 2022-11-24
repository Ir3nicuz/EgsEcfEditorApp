using EcfFileViews;
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
            private ToolTip ToolTipContainer { get; }

            private TreeView Tree { get; } = new TreeView();

            public EcfTechTree(string name, ToolTip toolTipContainer)
            {
                Text = name;
                ToolTipContainer = toolTipContainer;

                Tree.Dock = DockStyle.Fill;

                Controls.Add(Tree);
            }

            // publics
            public void Add(TechTreeElement element)
            {
                Tree.BeginUpdate();

                Tree.Nodes.Cast<TechTreeElement>().ToList().ForEach(unboundElement =>
                {
                    if (!string.IsNullOrEmpty(unboundElement.TechTreeParent) && string.Equals(unboundElement.TechTreeParent, element.Element.Id))
                    {
                        element.Nodes.Add(unboundElement);
                        Tree.Nodes.Remove(unboundElement);
                    }
                });

                TechTreeElement parent = FindParent(element.TechTreeParent, Tree.Nodes);
                if (parent != null)
                {
                    parent.Nodes.Add(element);
                }
                else
                {
                    Tree.Nodes.Add(element);
                }

                Tree.EndUpdate();
            }

            // privates
            private TechTreeElement FindParent(string parentName, TreeNodeCollection nodes)
            {
                foreach (TechTreeElement node in nodes.Cast<TechTreeElement>())
                {
                    if (string.Equals(node.Element.Id, parentName))
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
            private string UnlockLevel { get; }
            private string UnlockCost { get; }
            public string TechTreeParent { get; }

            public EcfBlock Element { get; }

            public TechTreeElement(string unlockLevel, string unlockCost, string techTreeParent, EcfBlock element)
            {
                UnlockLevel = unlockLevel;
                UnlockCost = unlockCost;
                TechTreeParent = techTreeParent;
                Element = element;

                Text = string.Format("{0} / Level {1} / Cost {2}", element.BuildIdentification(), unlockLevel, unlockCost);
            }

        }
    }
}
