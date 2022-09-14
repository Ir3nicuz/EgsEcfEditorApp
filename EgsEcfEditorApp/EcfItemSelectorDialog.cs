using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.Linq;
using System.Windows.Forms;

namespace EcfFileViews
{
    public partial class EcfItemSelectorDialog : Form
    {
        public object SelectedItem { get; set; } = default;
        private object[] FullItemList { get; set; } = null;

        public EcfItemSelectorDialog()
        {
            InitializeComponent();
            InitForm();
        }

        // events
        private void InitForm()
        {
            Icon = IconRecources.Icon_AppBranding;

            AbortButton.Text = TitleRecources.Generic_Abort;
            OkButton.Text = TitleRecources.Generic_Ok;

            SearchLabel.Text = TitleRecources.Generic_Search;
        }
        private void EcfItemSelectorDialog_Activated(object sender, EventArgs evt)
        {
            SearchTextBox.Focus();
        }
        private void AbortButton_Click(object sender, EventArgs evt)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }
        private void OkButton_Click(object sender, EventArgs evt)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        private void ItemSelectorComboBox_SelectedIndexChanged(object sender, EventArgs evt)
        {
            SelectedItem = (ItemSelectorComboBox.SelectedItem as ComboBoxItem)?.Item;
            UpdateOkButton();
        }
        private void SearchTextBox_MouseHover(object sender, EventArgs evt)
        {
            Tip.SetToolTip(SearchTextBox, TextRecources.EcfItemSelectorDialog_ToolTip_SearchInfo);
        }
        private void SearchTextBox_KeyPress(object sender, KeyPressEventArgs evt)
        {
            if (evt.KeyChar == (char)Keys.Enter)
            {
                UpdateItemSelector();
                PopulateResult();
                evt.Handled = true;
            }
        }

        // publics
        public DialogResult ShowDialog(IWin32Window parent, object[] items)
        {
            return ShowDialog(parent, TitleRecources.Generic_PickItem, items);
        }
        public DialogResult ShowDialog(IWin32Window parent, string header, object[] items)
        {
            Text = header;
            FullItemList = items;
            OkButton.Enabled = false;
            UpdateItemSelector();
            return ShowDialog(parent);
        }

        // private
        private ComboBoxItem[] FilterList(object[] items)
        {
            if (SearchTextBox.Text.Equals(string.Empty))
            {
                return items?.Select(item => new ComboBoxItem(item)).ToArray();
            }
            else
            {
                return items?.Select(item => new ComboBoxItem(item)).Where(item => item.DisplayText.Contains(SearchTextBox.Text)).ToArray();
            }
        }
        private void UpdateItemSelector()
        {
            ComboBoxItem[] items = FilterList(FullItemList);
            ItemSelectorComboBox.BeginUpdate();
            ItemSelectorComboBox.Items.Clear();
            if (items == null)
            {
                ItemSelectorComboBox.Items.Add("null");
            }
            else
            {
                ItemSelectorComboBox.Items.AddRange(items);
                if (ItemSelectorComboBox.Items.Count > 0)
                {
                    ItemSelectorComboBox.SelectedItem = ItemSelectorComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Item == SelectedItem);
                    if (ItemSelectorComboBox.SelectedIndex < 0)
                    {
                        ItemSelectorComboBox.SelectedIndex = 0;
                    }
                }
                else
                {
                    SelectedItem = default;
                    UpdateOkButton();
                }
            }
            ItemSelectorComboBox.EndUpdate();
        }
        private void UpdateOkButton()
        {
            OkButton.Enabled = SelectedItem != null;
        }
        private void PopulateResult()
        {
            if (ItemSelectorComboBox.Items.Count > 1)
            {
                ItemSelectorComboBox.DroppedDown = true;
                Cursor.Current = Cursors.Default;
            }
        }

        private class ComboBoxItem : IComparable<ComboBoxItem>
        {
            public string DisplayText { get; }
            public object Item { get; }

            public ComboBoxItem(object item)
            {
                Item = item;
                DisplayText = BuildDisplayText(item);
            }

            private string BuildDisplayText(object item)
            {
                switch (item)
                {
                    case EcfAttribute attr: return attr.Key;
                    case EcfParameter param: return param.Key;
                    case EcfBlock block: return block.BuildIdentification();
                    default: return string.Format("\"{0}\"", Convert.ToString(item));
                }
            }

            public override string ToString()
            {
                return DisplayText;
            }

            public int CompareTo(ComboBoxItem other)
            {
                if (DisplayText == null || other.DisplayText == null) { return 0; }
                return DisplayText.CompareTo(other.DisplayText);
            }
        }
    }
}
