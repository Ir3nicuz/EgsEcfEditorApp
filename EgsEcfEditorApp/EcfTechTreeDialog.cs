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

                            treePage.AddCell(unlockLevel.GetFirstValue(), unlockCost.GetFirstValue(), techTreeParent?.GetFirstValue(), block);
                        }
                    }
                    else if (techTreeNames != null || unlockLevel != null || unlockCost != null || techTreeParent != null)
                    {
                        // incomplete





                    }
                }

            });

            foreach (EcfTechTree tree in TechTreePageContainer.TabPages.Cast<EcfTechTree>())
            {
                tree.SortAndLinkCells();
            }

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
                Tree.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;

                Controls.Add(Tree);
            }

            // publics
            public void AddCell(string unlockLevel, string unlockCost, string techTreeParent, EcfBlock element)
            {
                TechTreeColumn column = FindOrAddColumn(unlockLevel);
                TechTreeItemCell cell = new TechTreeItemCell(unlockCost, techTreeParent, element, ItemSize, ToolTipContainer);
                column.Add(cell);
            }
            public void SortAndLinkCells()
            {





            }

            // privates
            private TechTreeColumn FindOrAddColumn(string unlockLevel)
            {
                List<TechTreeColumn> columns = Tree.Controls.Cast<TechTreeColumn>().ToList();
                TechTreeColumn column = columns.FirstOrDefault(control => control.UnlockLevel.Equals(unlockLevel));
                if (column != null) { return column; }

                column = new TechTreeColumn(unlockLevel, ItemSize);
                columns.Add(column);
                columns.Sort();

                Tree.Controls.Clear();
                columns.ForEach(control =>
                {
                    Tree.ColumnCount++;
                    Tree.Controls.Add(control, Tree.ColumnCount - 1, 0);
                });
                
                return column;
            }
            private int FindInsertRow(TechTreeColumn column, string techTreeParent)
            {
                
                
                if (string.IsNullOrEmpty(techTreeParent))
                {

                }
                else
                {

                }

                int rowIndex = column.RowCount - 1;
                while (rowIndex >= 0 && column.GetControlFromPosition(0, rowIndex) == null)
                {
                    rowIndex--;
                }

                rowIndex = Math.Max(rowIndex, 0);
                rowIndex += 2;

                return rowIndex;
            }

            // subclasses
            private class TechTreeColumn : TableLayoutPanel, IComparable
            {
                public string UnlockLevel { get; }

                public TechTreeColumn(string unlockLevel, Size itemSize)
                {
                    UnlockLevel = unlockLevel;

                    AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    AutoSize = true;
                    GrowStyle = TableLayoutPanelGrowStyle.AddRows;

                    Size headerSize = new Size(itemSize.Width, itemSize.Height * 2);
                    Controls.Add(new Label()
                    {
                        Text = string.Format("{0} {1}", "Level", UnlockLevel),
                        Size = headerSize,
                        MinimumSize = headerSize,
                        MaximumSize = headerSize,
                        BorderStyle = BorderStyle.Fixed3D,
                    }, 0, 0);
                }

                public void Add(Control cell)
                {
                    Controls.Add(cell);
                }
                public void Insert(Control cell, int rowIndex)
                {
                    if (rowIndex >= RowCount)
                    {
                        RowCount += rowIndex - RowCount + 1;
                    }
                    else if (RowCount > 0)
                    {
                        RowCount++;
                        for (int rowCounter = RowCount - 1; rowCounter > rowIndex; rowCounter--)
                        {
                            SetRow(GetControlFromPosition(0, rowCounter - 1), rowCounter);
                        }
                    }
                    Controls.Add(cell, 0, rowIndex);
                }
                public int CompareTo(object other)
                {
                    if (!(other is TechTreeColumn otherColumn)) { return 1; }
                    if (double.TryParse(UnlockLevel, NumberStyles.Any, CultureInfo.InvariantCulture, out double thisLevel) && 
                        double.TryParse(otherColumn.UnlockLevel, NumberStyles.Any, CultureInfo.InvariantCulture, out double otherLevel))
                    {
                        return thisLevel.CompareTo(otherLevel);
                    }
                    return string.Compare(UnlockLevel, otherColumn.UnlockLevel);
                }
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
                public string TechTreeParent { get; }
                public EcfBlock Element { get; }

                private Label IdNumberLabel { get; } = new Label();
                private Label RefTargetLabel { get; } = new Label();
                private Label UnlockCostLabel { get; } = new Label();

                public TechTreeItemCell(string unlockCost, string techTreeParent, EcfBlock block, Size itemSize, ToolTip toolTipContainer)
                {
                    TechTreeParent = techTreeParent;
                    Element = block;

                    IdNumberLabel.Text = block.Id;
                    RefTargetLabel.Text = block.RefTarget;
                    UnlockCostLabel.Text = unlockCost;

                    toolTipContainer.SetToolTip(IdNumberLabel, block.BuildIdentification());

                    Size = itemSize;
                    MinimumSize = itemSize;
                    MaximumSize = itemSize;

                    BorderStyle = BorderStyle.FixedSingle;
                    FlowDirection = FlowDirection.TopDown;

                    Controls.Add(IdNumberLabel);
                    Controls.Add(RefTargetLabel);
                    Controls.Add(UnlockCostLabel);
                }
            }
        }
    }
}
