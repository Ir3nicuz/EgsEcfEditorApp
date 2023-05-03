using System;
using System.Linq;
using System.Windows.Forms;

namespace EcfFileViews
{
    public partial class ItemSelectorDialog : Form
    {
        public string OkButtonText
        {
            get
            {
                return OkButton.Text;
            }
            set
            {
                OkButton.Text = value;
            }
        }
        public string AbortButtonText
        {
            get
            {
                return AbortButton.Text;
            }
            set
            {
                AbortButton.Text = value;
            }
        }
        public string SearchToolTipText { get; set; } = string.Empty;
        public string DefaultItemText { get; set; } = string.Empty;
        public SelectorItem SelectedItem { get; private set; } = null;
        private SelectorItem[] FullItemList { get; set; } = null;

        public ItemSelectorDialog()
        {
            InitializeComponent();
        }

        // events
        private void ItemSelectorDialog_Activated(object sender, EventArgs evt)
        {
            SearchTextBox.Focus();
            SearchTextBox.SelectAll();
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
            SelectedItem = ItemSelectorComboBox.SelectedItem as SelectorItem;
            UpdateOkButton();
        }
        private void SearchTextBox_MouseHover(object sender, EventArgs evt)
        {
            Tip.SetToolTip(SearchTextBox, SearchToolTipText);
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
        public DialogResult ShowDialog(IWin32Window parent, SelectorItem[] items)
        {
            FullItemList = items.ToArray();
            OkButton.Enabled = false;
            UpdateItemSelector();
            return ShowDialog(parent);
        }

        // private
        private SelectorItem[] FilterList(SelectorItem[] items)
        {
            if (SearchTextBox.Text.Equals(string.Empty))
            {
                return items;
            }
            return items?.Where(item => item.DisplayText.Contains(SearchTextBox.Text)).ToArray();
        }
        private void UpdateItemSelector()
        {
            SelectorItem[] items = FilterList(FullItemList);
            ItemSelectorComboBox.BeginUpdate();
            ItemSelectorComboBox.Items.Clear();
            if (items == null)
            {
                ItemSelectorComboBox.Items.Add(DefaultItemText);
            }
            else
            {
                ItemSelectorComboBox.Items.AddRange(items);
                if (ItemSelectorComboBox.Items.Count > 0)
                {
                    ItemSelectorComboBox.SelectedItem = ItemSelectorComboBox.Items.Cast<SelectorItem>().FirstOrDefault(item => item.Item == SelectedItem);
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

        public class SelectorItem : IComparable<SelectorItem>
        {
            public string DisplayText { get; }
            public object Item { get; }

            public SelectorItem(object item)
            {
                Item = item;
                DisplayText = string.Format("\"{0}\"", Convert.ToString(item));
            }
            public SelectorItem(object item, string displayText)
            {
                Item = item;
                DisplayText = displayText;
            }

            public override string ToString()
            {
                return DisplayText;
            }

            public int CompareTo(SelectorItem other)
            {
                if (DisplayText == null || other.DisplayText == null) { return 0; }
                return DisplayText.CompareTo(other.DisplayText);
            }
        }
    }
}
