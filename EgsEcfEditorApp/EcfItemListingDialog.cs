using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EgsEcfEditorApp
{
    public partial class EcfItemListingDialog : Form
    {
        public event EventHandler<ItemRowClickedEventArgs> ItemRowClicked;

        public EgsEcfFile SelectedFileItem { get; private set; } = null;
        public EcfStructureItem SelectedStructureItem { get; private set; } = null;

        private List<EgsEcfFile> FileItems { get; } = new List<EgsEcfFile>();
        private List<EcfStructureItem> StructureItems { get; } = new List<EcfStructureItem>();

        public EcfItemListingDialog()
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
            ListingGridColumn_Element.HeaderText = TitleRecources.Generic_Element;
            ListingGridColumn_Parameter.HeaderText = TitleRecources.Generic_Parameter;

            CloseButton.Text = TitleRecources.Generic_Close;
        }
        private void CloseButton_Click(object sender, EventArgs evt)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }
        private void ItemListingGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs evt)
        {
            if (evt.RowIndex >= 0)
            {
                SelectedFileItem = (evt.RowIndex < FileItems.Count) ? FileItems[evt.RowIndex] : null;
                SelectedStructureItem = (evt.RowIndex < StructureItems.Count) ? StructureItems[evt.RowIndex] : null;

                ItemRowClicked?.Invoke(this, new ItemRowClickedEventArgs(SelectedFileItem, SelectedStructureItem));
                if (Modal)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
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
        public DialogResult ShowDialog(IWin32Window parent, List<EcfBlock> itemList)
        {
            return ShowDialog(parent, null, itemList);
        }
        public DialogResult ShowDialog(IWin32Window parent, List<EcfParameter> itemList)
        {
            return ShowDialog(parent, null, itemList);
        }
        public DialogResult ShowDialog(IWin32Window parent, string searchDescription, List<EcfBlock> itemList)
        {
            RefreshInfoDescription(searchDescription, itemList?.Count ?? 0);
            RefreshGridView(itemList);
            return ShowDialog(parent);
        }
        public DialogResult ShowDialog(IWin32Window parent, string searchDescription, List<EcfParameter> itemList)
        {
            RefreshInfoDescription(searchDescription, itemList?.Count ?? 0);
            RefreshGridView(itemList);
            return ShowDialog(parent);
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
            StructureItems.Clear();

            ListingGridColumn_Parameter.Visible = false;

            int lineCounter = 1;
            itemList.ForEach(item =>
            {
                ItemListingGrid.Rows.Add(lineCounter, 
                    item.EcfFile?.FileName ?? TitleRecources.Generic_Replacement_Empty, 
                    item.GetFullPath());
                StructureItems.Add(item);
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
            StructureItems.Clear();

            ListingGridColumn_Parameter.Visible = true;

            int lineCounter = 1;
            itemList.ForEach(item =>
            {
                ItemListingGrid.Rows.Add(lineCounter, 
                    item.EcfFile?.FileName ?? TitleRecources.Generic_Replacement_Empty, 
                    item.Parent?.GetFullPath() ?? TitleRecources.Generic_Replacement_Empty, 
                    item.Key);
                StructureItems.Add(item);
                lineCounter++;
            });

            ItemListingGrid.AutoResizeColumns();
            ItemListingGrid.ClearSelection();
            ItemListingGrid.ResumeLayout();
        }

        // subclasses
        public class ItemRowClickedEventArgs : EventArgs
        {
            public EgsEcfFile FileItem { get; }
            public EcfStructureItem StructureItem { get; }

            public ItemRowClickedEventArgs(EgsEcfFile fileItem, EcfStructureItem structureItem) : base()
            {
                FileItem = fileItem;
                StructureItem = structureItem;
            }
        }
    }
}
