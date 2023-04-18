using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EgsEcfEditorApp
{
    public partial class EcfItemListingView : Form
    {
        public event EventHandler<ItemRowClickedEventArgs> ItemRowClicked;

        private List<EcfStructureItem> RowItems { get; } = new List<EcfStructureItem>();

        public EcfItemListingView()
        {
            InitializeComponent();
            InitForm();
        }

        // events
        private void InitForm()
        {
            Text = TitleRecources.EcfItemListingView_Header;
            Icon = IconRecources.Icon_AppBranding;

            ListingGridColumn_Number.HeaderText = TitleRecources.Generic_Number_Short;
            ListingGridColumn_File.HeaderText = TitleRecources.Generic_File;
            ListingGridColumn_Item.HeaderText = TitleRecources.Generic_Item;
            ListingGridColumn_Parameter.HeaderText = TitleRecources.Generic_Parameter;

            CloseButton.Text = TitleRecources.Generic_Close;
        }
        private void CloseButton_Click(object sender, EventArgs evt)
        {
            Close();
        }
        private void ItemListingGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs evt)
        {
            if (evt.RowIndex >= 0 && evt.RowIndex < RowItems.Count)
            {
                ItemRowClicked?.Invoke(this, new ItemRowClickedEventArgs(RowItems[evt.RowIndex]));
            }
        }

        // publics
        public void Show(IWin32Window parent, List<EcfBlock> itemList)
        {
            Show(parent, null, itemList);
        }
        public void Show(IWin32Window parent, List<EcfParameter> itemList)
        {
            Show(parent, null, itemList);
        }
        public void Show(IWin32Window parent, string searchDescription, List<EcfBlock> itemList)
        {
            RefreshInfoDescription(searchDescription, itemList?.Count ?? 0);
            RefreshGridView(itemList);
            Show(parent);
        }
        public void Show(IWin32Window parent, string searchDescription, List<EcfParameter> itemList)
        {
            RefreshInfoDescription(searchDescription, itemList?.Count ?? 0);
            RefreshGridView(itemList);
            Show(parent);
        }

        // privates
        private void RefreshInfoDescription(string searchDescription, int hitCount)
        {
            SearchHitsLabel.Text = string.Format("{0} {1}", hitCount, TitleRecources.Generic_SearchHits);
            if (!string.IsNullOrEmpty(searchDescription))
            {
                Text = string.Format("{0} - {1}", TitleRecources.EcfItemListingView_Header, searchDescription);
            }
        }
        private void RefreshGridView(List<EcfBlock> itemList)
        {
            ItemListingGrid.SuspendLayout();
            ItemListingGrid.Rows.Clear();
            RowItems.Clear();

            ListingGridColumn_Parameter.Visible = false;

            int lineCounter = 1;
            itemList.ForEach(item =>
            {
                ItemListingGrid.Rows.Add(lineCounter, 
                    item.EcfFile?.FileName ?? TitleRecources.Generic_Replacement_Empty, 
                    item.GetFullPath());
                RowItems.Add(item);
                lineCounter++;
            });

            ItemListingGrid.AutoResizeColumns();
            ItemListingGrid.ClearSelection();
            ItemListingGrid.ResumeLayout();
        }
        private void RefreshGridView(List<EcfParameter> itemList)
        {
            ItemListingGrid.SuspendLayout();
            ItemListingGrid.Rows.Clear();
            RowItems.Clear();

            ListingGridColumn_Parameter.Visible = true;

            int lineCounter = 1;
            itemList.ForEach(item =>
            {
                ItemListingGrid.Rows.Add(lineCounter, 
                    item.EcfFile?.FileName ?? TitleRecources.Generic_Replacement_Empty, 
                    item.Parent?.GetFullPath() ?? TitleRecources.Generic_Replacement_Empty, 
                    item.Key);
                RowItems.Add(item);
                lineCounter++;
            });

            ItemListingGrid.AutoResizeColumns();
            ItemListingGrid.ClearSelection();
            ItemListingGrid.ResumeLayout();
        }

        // subclasses
        public class ItemRowClickedEventArgs : EventArgs
        {
            public EcfStructureItem RowItem { get; }

            public ItemRowClickedEventArgs(EcfStructureItem rowItem) : base()
            {
                RowItem = rowItem;
            }
        }
    }
}
