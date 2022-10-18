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
        private Size ItemSize { get; } = new Size(InternalSettings.Default.EcfTechTreeDialog_ItemEdgeLenght, InternalSettings.Default.EcfTechTreeDialog_ItemEdgeLenght);

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
                                treePage = new EcfTechTree(treeName, ItemSize, Tip);
                                TechTreePageContainer.TabPages.Add(treePage);
                            }

                            treePage.AddItem(unlockLevel.GetFirstValue(), unlockCost.GetFirstValue(), techTreeParent?.GetFirstValue(), block);
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
            private Size ItemSize { get; }
            private ToolTip ToolTipContainer { get; }

            private TableLayoutPanel Tree { get; } = new TableLayoutPanel();

            public EcfTechTree(string name, Size itemSize, ToolTip toolTipContainer)
            {
                Text = name;
                ItemSize = itemSize;
                ToolTipContainer = toolTipContainer;

                Tree.AutoScroll = true;
                Tree.Dock = DockStyle.Fill;

                Controls.Add(Tree);
            }

            public void AddItem(string unlockLevel, string unlockCost, string techTreeParent, EcfBlock element)
            {
                int columnIndex = FindOrAddColumn(unlockLevel);
                int rowIndex = FindOrAddRow(columnIndex, techTreeParent);

                TechTreeItemCell cell = new TechTreeItemCell(unlockCost, element, ItemSize, ToolTipContainer);
                Tree.Controls.Add(cell, columnIndex, rowIndex);


                //new TechTreeRoutingCell(RoutingTypes.SplitVerticalLeft, ItemSize);
                //new TechTreeItemCell(unlockCost, element, ItemSize, ToolTipContainer);



            }

            private int FindOrAddColumn(string unlockLevel)
            {
                List<TechTreeHeaderCell> headerCells = Tree.Controls.Cast<Control>().Where(cell => cell is TechTreeHeaderCell).Cast<TechTreeHeaderCell>().ToList();

                TechTreeHeaderCell headerCell = headerCells.FirstOrDefault(cell => cell.UnlockLevel.Equals(unlockLevel));
                if (headerCell != null) { return Tree.GetColumn(headerCell); }

                headerCell = new TechTreeHeaderCell(unlockLevel, ItemSize);
                Tree.ColumnCount++;
                Tree.Controls.Add(headerCell, Tree.ColumnCount - 1, 0);

                //sort??

                return Tree.GetColumn(headerCell);
            }
            private int FindOrAddRow(int columnIndex, string techTreeParent)
            {
                if (string.IsNullOrEmpty(techTreeParent))
                {

                }
                else
                {

                }

                int rowIndex = Tree.RowCount - 1;
                while(rowIndex >= 0 && Tree.GetControlFromPosition(columnIndex, rowIndex) == null)
                {
                    rowIndex--;
                }
                rowIndex += 2;
                if (rowIndex >= Tree.RowCount)
                {
                    Tree.RowCount += rowIndex - Tree.RowCount + 1;
                }
                return rowIndex;
            }

            private enum RoutingTypes
            {
                StraightVertical,
                StraightHorizontal,
                SplitVerticalRight,
                SplitVerticalLeft,
                SplitHorizontalUp,
                SplitHorizontalDown
            }
            private class TechTreeHeaderCell : Label
            {
                public string UnlockLevel { get; }

                public TechTreeHeaderCell(string unlockLevel, Size itemSize)
                {
                    UnlockLevel = unlockLevel;
                    Text = string.Format("{0} {1}", "Level", UnlockLevel);

                    Size headerSize = new Size(itemSize.Width, itemSize.Height * 2);

                    Size = headerSize;
                    MinimumSize = headerSize;
                    MaximumSize = headerSize;
                    Dock = DockStyle.Fill;
                    TextAlign = ContentAlignment.BottomCenter;
                }
            }
            private class TechTreeRoutingCell : Label
            {
                public TechTreeRoutingCell(RoutingTypes routingType, Size itemSize)
                {
                    Size = itemSize;
                    MinimumSize = itemSize;
                    MaximumSize = itemSize;
                    Dock = DockStyle.Fill;

                    switch (routingType)
                    {
                        case RoutingTypes.StraightVertical: Image = new Bitmap(IconRecources.Icon_ChangeComplex, itemSize); break;
                        case RoutingTypes.StraightHorizontal: Image = new Bitmap(IconRecources.Icon_ChangeComplex, itemSize); break;
                        case RoutingTypes.SplitVerticalRight: Image = new Bitmap(IconRecources.Icon_ChangeComplex, itemSize); break;
                        case RoutingTypes.SplitVerticalLeft: Image = new Bitmap(IconRecources.Icon_ChangeComplex, itemSize); break;
                        case RoutingTypes.SplitHorizontalUp: Image = new Bitmap(IconRecources.Icon_ChangeComplex, itemSize); break;
                        case RoutingTypes.SplitHorizontalDown: Image = new Bitmap(IconRecources.Icon_ChangeComplex, itemSize); break;
                        default: Image = new Bitmap(IconRecources.Icon_Unknown, itemSize); break;
                    }
                }
            }
            private class TechTreeItemCell : FlowLayoutPanel
            {
                public EcfBlock Element { get; }
                public string UnlockCost { get; }

                private Label Id { get; } = new Label();
                private Label Cost { get; } = new Label();

                public TechTreeItemCell(string unlockCost, EcfBlock block, Size itemSize, ToolTip toolTipContainer)
                {
                    Element = block;
                    UnlockCost = unlockCost;
                    Id.Text = block.Id;
                    Cost.Text = UnlockCost;

                    toolTipContainer.SetToolTip(Id, block.BuildIdentification());

                    Size = itemSize;
                    MinimumSize = itemSize;
                    MaximumSize = itemSize;

                    BorderStyle = BorderStyle.FixedSingle;
                    Dock = DockStyle.Fill;
                    FlowDirection = FlowDirection.TopDown;

                    Controls.Add(Id);
                    Controls.Add(Cost);
                }
            }
        }
        
    }
}
