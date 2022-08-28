using EcfCAMTools;
using EcfFileViews;
using EcfToolBarControls;
using EgsEcfEditorApp.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static Helpers.ImageAjustments;

namespace EgsEcfEditorApp
{
    public partial class EcfFileCAMDialog : Form
    {
        public List<EcfTabPage> ChangedFileTabs { get; } = new List<EcfTabPage>();

        private CompareSelectionTools FirstFileSelectionTools { get; } = new CompareSelectionTools();
        private CompareSelectionTools SecondFileSelectionTools { get; } = new CompareSelectionTools();
        private MergeActionTools FirstFileActionTools { get; } = new MergeActionTools(IconRecources.Icon_MoveRight);
        private MergeActionTools SecondFileActionTools { get; } = new MergeActionTools(IconRecources.Icon_MoveLeft);

        private ImageList CAMListViewIcons { get; } = new ImageList();
        private List<ComboBoxItem> AvailableFileTabs { get; } = new List<ComboBoxItem>();

        public EcfFileCAMDialog()
        {
            InitializeComponent();
            InitForm();
        }

        // events
        private void InitForm()
        {
            Icon = IconRecources.Icon_AppBranding;
            Text = TitleRecources.EcfFileCAMDialog_Header;

            CloseButton.Text = TitleRecources.Generic_Close;

            FirstFileTreeView.LinkTreeView(SecondFileTreeView);

            FirstFileSelectionContainer.Add(FirstFileSelectionTools);
            FirstFileActionContainer.Add(FirstFileActionTools);
            SecondFileActionContainer.Add(SecondFileActionTools);
            SecondFileSelectionContainer.Add(SecondFileSelectionTools);

            CAMListViewIcons.Images.Add(AddGap(IconRecources.Icon_Add, 16, 3, 1));
            CAMListViewIcons.Images.Add(AddGap(IconRecources.Icon_Unequal, 16, 3, 1));
            CAMListViewIcons.Images.Add(AddGap(IconRecources.Icon_Remove, 16, 3, 1));

            FirstFileTreeView.ImageList = CAMListViewIcons;
            SecondFileTreeView.ImageList = CAMListViewIcons;
        }
        private void CloseButton_Click(object sender, EventArgs evt)
        {
            Close();
        }
        private void FirstFileComboBox_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            ComboBoxItem firstItem = FirstFileComboBox.SelectedItem as ComboBoxItem;
            RefreshFileSelectorBox(SecondFileComboBox, firstItem);
            CompareFiles(firstItem, SecondFileComboBox.SelectedItem as ComboBoxItem);
        }
        private void SecondFileComboBox_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            ComboBoxItem secondItem = SecondFileComboBox.SelectedItem as ComboBoxItem;
            RefreshFileSelectorBox(FirstFileComboBox, secondItem);
            CompareFiles(FirstFileComboBox.SelectedItem as ComboBoxItem, secondItem);
        }

        // public
        public DialogResult ShowDialog(IWin32Window parent, List<EcfTabPage> openedFileTabs)
        {
            ChangedFileTabs.Clear();
            AvailableFileTabs.Clear();
            AvailableFileTabs.AddRange(openedFileTabs.Select(tab => new ComboBoxItem(tab)));
            RefreshFileSelectorBox(FirstFileComboBox, null);
            RefreshFileSelectorBox(SecondFileComboBox, null);
            return ShowDialog(parent);
        }

        // private
        private void RefreshFileSelectorBox(ComboBox box, ComboBoxItem otherSelectedItem)
        {
            ComboBoxItem selectedItem = box.SelectedItem as ComboBoxItem;
            box.BeginUpdate();
            box.Items.Clear();
            box.Items.AddRange(AvailableFileTabs.Where(item => !item.Equals(otherSelectedItem)).ToArray());
            box.EndUpdate();
            box.SelectedItem = selectedItem;
        }
        private void CompareFiles(ComboBoxItem firstItem, ComboBoxItem secondItem)
        {
            if (firstItem == null || secondItem == null) { return; }

            FirstFileTreeView.BeginUpdate();
            SecondFileTreeView.BeginUpdate();
            FirstFileTreeView.Nodes.Clear();
            SecondFileTreeView.Nodes.Clear();


            





            FirstFileTreeView.Nodes.Add("0", "0", 0, 0);
            FirstFileTreeView.Nodes.Add("1", "1", 1, 1);
            FirstFileTreeView.Nodes.Add("2", "2", 2, 2);




            FirstFileTreeView.EndUpdate();
            SecondFileTreeView.EndUpdate();

            FirstFileSelectionTools.Reset();
            SecondFileSelectionTools.Reset();
        }

        // subclass
        private class ComboBoxItem : IComparable<ComboBoxItem>
        {
            public string DisplayText { get; }
            public EcfTabPage Item { get; }

            public ComboBoxItem(EcfTabPage item)
            {
                Item = item;
                DisplayText = item.File.FileName;
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

namespace EcfCAMTools
{
    public class CompareSelectionTools : EcfToolBox
    {
        public event EventHandler SelectAllAddItemsClicked;
        public event EventHandler SelectNoneAddItemsClicked;
        public event EventHandler SelectAllUnequalItemsClicked;
        public event EventHandler SelectNoneUnequalItemsClicked;
        public event EventHandler SelectAllRemoveItemsClicked;
        public event EventHandler SelectNoneRemoveItemsClicked;

        private EcfToolBarThreeStateCheckBox ChangeAllAddItemsButton { get; } = new EcfToolBarThreeStateCheckBox(
            TextRecources.EcfFileCAMDialog_ToolTip_ChangeAllAddItems, IconRecources.Icon_SomeAddItemsSet, IconRecources.Icon_AllAddItemsSet, IconRecources.Icon_NoneAddItemsSet);
        private EcfToolBarThreeStateCheckBox ChangeAllUnequalItemsButton { get; } = new EcfToolBarThreeStateCheckBox(
            TextRecources.EcfFileCAMDialog_ToolTip_ChangeAllUnequalItems, IconRecources.Icon_SomeUnequalItemsSet, IconRecources.Icon_AllUnequalItemsSet, IconRecources.Icon_NoneUnequalItemsSet);
        private EcfToolBarThreeStateCheckBox ChangeAllRemoveItemsButton { get; } = new EcfToolBarThreeStateCheckBox(
            TextRecources.EcfFileCAMDialog_ToolTip_ChangeAllRemoveItems, IconRecources.Icon_SomeRemoveItemsSet, IconRecources.Icon_AllRemoveItemsSet, IconRecources.Icon_NoneRemoveItemsSet);

        public CompareSelectionTools() : base()
        {
            Add(ChangeAllAddItemsButton).Click += ChangeAllAddsButton_Click;
            Add(ChangeAllUnequalItemsButton).Click += ChangeAllUpdatesButton_Click;
            Add(ChangeAllRemoveItemsButton).Click += ChangeAllRemovesButton_Click;
        }

        // events
        private void ChangeAllAddsButton_Click(object sender, EventArgs evt)
        {
            if (ChangeAllAddItemsButton.CheckState == CheckState.Indeterminate)
            {
                ChangeAllAddItemsButton.CheckState = CheckState.Unchecked;
                SelectNoneAddItemsClicked?.Invoke(sender, evt);
            }
            else if(ChangeAllAddItemsButton.CheckState == CheckState.Checked)
            {
                SelectAllAddItemsClicked?.Invoke(sender, evt);
            }
        }
        private void ChangeAllUpdatesButton_Click(object sender, EventArgs evt)
        {
            if (ChangeAllUnequalItemsButton.CheckState == CheckState.Indeterminate)
            {
                ChangeAllUnequalItemsButton.CheckState = CheckState.Unchecked;
                SelectNoneUnequalItemsClicked?.Invoke(sender, evt);
            }
            else if (ChangeAllUnequalItemsButton.CheckState == CheckState.Checked)
            {
                SelectAllUnequalItemsClicked?.Invoke(sender, evt);
            }
        }
        private void ChangeAllRemovesButton_Click(object sender, EventArgs evt)
        {
            if (ChangeAllRemoveItemsButton.CheckState == CheckState.Indeterminate)
            {
                ChangeAllRemoveItemsButton.CheckState = CheckState.Unchecked;
                SelectNoneRemoveItemsClicked?.Invoke(sender, evt);
            }
            else if (ChangeAllRemoveItemsButton.CheckState == CheckState.Checked)
            {
                SelectAllRemoveItemsClicked?.Invoke(sender, evt);
            }
        }

        // publics
        public void SetAllAddsState(CheckState state)
        {
            ChangeAllAddItemsButton.CheckState = state;
        }
        public void SetAllUpdatesButton(CheckState state)
        {
            ChangeAllUnequalItemsButton.CheckState = state;
        }
        public void SetAllRemovesButton(CheckState state)
        {
            ChangeAllRemoveItemsButton.CheckState = state;
        }
        public void Reset()
        {
            SetAllAddsState(CheckState.Checked);
            SetAllUpdatesButton(CheckState.Checked);
            SetAllRemovesButton(CheckState.Checked);
        }
    }
    public class MergeActionTools : EcfToolBox
    {
        public event EventHandler DoMergeClicked;

        public MergeActionTools(Image doMergeIcon) : base()
        {
            Add(new EcfToolBarButton(TextRecources.EcfFileCAMDialog_ToolTip_DoMerge, doMergeIcon, null)).Click += (sender, evt) => DoMergeClicked?.Invoke(sender, evt);
        }
    }
}
