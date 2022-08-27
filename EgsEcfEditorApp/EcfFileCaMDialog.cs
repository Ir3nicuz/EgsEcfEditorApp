using EcfCAMTools;
using EcfFileViews;
using EcfToolBarControls;
using EgsEcfEditorApp.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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

            CAMListViewIcons.Images.Add(PrepareIcon(IconRecources.Icon_Add, 16, 3, 1));
            CAMListViewIcons.Images.Add(PrepareIcon(IconRecources.Icon_Unequal, 16, 3, 1));
            CAMListViewIcons.Images.Add(PrepareIcon(IconRecources.Icon_Remove, 16, 3, 1));

            FirstFileTreeView.ImageList = CAMListViewIcons;
            SecondFileTreeView.ImageList = CAMListViewIcons;





            FirstFileTreeView.Nodes.Add("0", "0", 0);
            FirstFileTreeView.Nodes.Add("1", "1", 1);
            FirstFileTreeView.Nodes.Add("2", "2", 2);

        }
        private void CloseButton_Click(object sender, EventArgs evt)
        {
            Close();
        }

        // public
        [Obsolete("needs works")]
        public DialogResult ShowDialog(IWin32Window parent, List<EcfTabPage> openedFileTabs)
        {
            ChangedFileTabs.Clear();



            return ShowDialog(parent);
        }

        // private
        private static Bitmap PrepareIcon(Bitmap icon, int edge, int xGap, int yGap)
        {
            Bitmap bmp = new Bitmap(edge + 2 * xGap, edge + 2 * yGap);
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.CompositingMode = CompositingMode.SourceCopy;
                gfx.CompositingQuality = CompositingQuality.HighQuality;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gfx.DrawImage(icon, new Rectangle(xGap, yGap, edge, edge));
            }
            return bmp;
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
