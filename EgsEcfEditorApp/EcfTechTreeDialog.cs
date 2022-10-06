using EcfFileViews;
using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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



            EcfTechTreeTabPage test = new EcfTechTreeTabPage("test");
            TechTreePageContainer.TabPages.Add(test);

            UniqueFileTabs.ForEach(tab =>
            {
                foreach(EcfBlock block in tab.File.ItemList.Where(item => item is EcfBlock))
                {
                    block.HasParameter(unlockLevelKey, out EcfParameter unlockLevel);
                    block.HasParameter(unlockCostKey, out EcfParameter unlockCost);
                    block.HasParameter(techTreeNamesKey, out EcfParameter techTreeNames);
                    block.HasParameter(techTreeParentKey, out EcfParameter techTreeParent);

                    if (unlockLevel != null || unlockCost != null || techTreeNames != null || techTreeParent != null)
                    {
                        
                        StringBuilder nodeName = new StringBuilder(block.BuildIdentification());
                        nodeName.Append(", Missing: ");
                        if (unlockLevel == null)
                        {
                            nodeName.Append(unlockLevelKey);
                            nodeName.Append(" | ");
                        }
                        if (unlockCost == null)
                        {
                            nodeName.Append(unlockCostKey);
                            nodeName.Append(" | ");
                        }
                        if (techTreeNames == null)
                        {
                            nodeName.Append(techTreeNamesKey);
                            nodeName.Append(" | ");
                        }
                        if (techTreeParent == null)
                        {
                            nodeName.Append(techTreeParentKey);
                            nodeName.Append(" | ");
                        }

                        if (unlockLevel == null || unlockCost == null || techTreeNames == null)
                        {
                            test.AddNode(new EcfTechTreeNode(nodeName.ToString(), block));
                        }
                        
                    }
                }
            });



            TechTreePageContainer.ResumeLayout();
        }

        // subclasses
        private class EcfTechTreeTabPage : TabPage
        {
            private TreeView Tree { get; } = new TreeView();

            public EcfTechTreeTabPage(string name)
            {
                Text = name;
                
                Tree.Dock = DockStyle.Fill;
                
                Controls.Add(Tree);
            }

            public void AddNode(EcfTechTreeNode node)
            {
                Tree.Nodes.Add(node);
            }
        }
        private class EcfTechTreeNode : TreeNode
        {
            public EcfBlock Block { get; }

            public EcfTechTreeNode(string nodeName, EcfBlock block)
            {
                Text = nodeName;
                Block = block;
            }
        }
    }
}
