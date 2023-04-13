using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.Collections.Generic;
using System.Linq;
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
            string fileTest = itemList?.FirstOrDefault()?.EcfFile.FileName;
            string pathTest = itemList?.FirstOrDefault()?.GetFullPath();
            string parameterTest = null;



        }
        private void RefreshGridView(List<EcfParameter> itemList)
        {
            string fileTest = itemList?.FirstOrDefault()?.EcfFile.FileName;
            string pathTest = itemList?.FirstOrDefault()?.Parent.GetFullPath();
            string parameterTest = itemList?.FirstOrDefault()?.Key;



        }
    }
}
