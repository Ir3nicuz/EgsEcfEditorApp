using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EgsEcfEditorApp
{
    public partial class EcfItemListingView : Form
    {
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

            SearchValueHeaderLabel.Text = string.Format("{0}:", TitleRecources.EcfItemListingView_SearchValueHeader);
            
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

        // publics
        public void Show(IWin32Window parent, List<EcfBlock> itemList)
        {
            Show(parent, null, itemList);
        }
        public void Show(IWin32Window parent, List<EcfParameter> itemList)
        {
            Show(parent, null, itemList);
        }
        public void Show(IWin32Window parent, string searchValue, List<EcfBlock> itemList)
        {
            RefreshSearchValue(searchValue);
            RefreshGridView(itemList);
            Show(parent);
        }
        public void Show(IWin32Window parent, string searchValue, List<EcfParameter> itemList)
        {
            RefreshSearchValue(searchValue);
            RefreshGridView(itemList);
            Show(parent);
        }

        // privates
        private void RefreshSearchValue(string searchValue)
        {
            bool isVisible = !string.IsNullOrEmpty(searchValue);
            if (isVisible)
            {
                SearchValueTextBox.Text = searchValue;
            }
            SearchValuePanel.Visible = isVisible;
        }
        private void RefreshGridView(List<EcfBlock> itemList)
        {
            ItemListingGrid.SuspendLayout();
            ItemListingGrid.Rows.Clear();

            ListingGridColumn_Parameter.Visible = false;

            int lineCounter = 1;
            itemList.ForEach(item =>
            {
                ItemListingGrid.Rows.Add(lineCounter, 
                    item.EcfFile?.FileName ?? TitleRecources.Generic_Replacement_Empty, 
                    item.GetFullPath());
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

            ListingGridColumn_Parameter.Visible = true;

            int lineCounter = 1;
            itemList.ForEach(item =>
            {
                ItemListingGrid.Rows.Add(lineCounter, 
                    item.EcfFile?.FileName ?? TitleRecources.Generic_Replacement_Empty, 
                    item.Parent?.GetFullPath() ?? TitleRecources.Generic_Replacement_Empty, 
                    item.Key);
                lineCounter++;
            });

            ItemListingGrid.AutoResizeColumns();
            ItemListingGrid.ClearSelection();
            ItemListingGrid.ResumeLayout();
        }
    }
}
