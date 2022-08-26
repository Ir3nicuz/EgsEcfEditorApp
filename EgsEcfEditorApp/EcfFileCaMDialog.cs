using EcfCAMTools;
using EcfFileViews;
using EcfToolBarControls;
using EgsEcfEditorApp.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EgsEcfEditorApp
{
    public partial class EcfFileCAMDialog : Form
    {
        public List<EcfTabPage> ChangedFileTabs { get; } = new List<EcfTabPage>();

        private CompareSelectionTools FirstFileSelectionTools { get; } = new CompareSelectionTools();
        private CompareSelectionTools SecondFileSelectionTools { get; } = new CompareSelectionTools();
        private MergeActionTools FirstFileActionTools { get; } = new MergeActionTools();
        private MergeActionTools SecondFileActionTools { get; } = new MergeActionTools();

        private ImageList CAMListViewIcons { get; } = new ImageList();

        public EcfFileCAMDialog()
        {
            InitializeComponent();
            InitForm();
        }

        // events
        private void InitForm()
        {
            Icon = IconRecources.Icon_App;
            Text = TitleRecources.EcfFileCAMDialog_Header;

            CloseButton.Text = TitleRecources.Generic_Close;

            FirstFileTreeView.LinkTreeView(SecondFileTreeView);

            FirstFileSelectionContainer.Add(FirstFileSelectionTools);
            FirstFileActionContainer.Add(FirstFileActionTools);
            SecondFileActionContainer.Add(SecondFileActionTools);
            SecondFileSelectionContainer.Add(SecondFileSelectionTools);

            CAMListViewIcons.Images.Add(AddIconImageGap(IconRecources.Icon_Add));
            CAMListViewIcons.Images.Add(AddIconImageGap(IconRecources.Icon_Unequal));
            CAMListViewIcons.Images.Add(AddIconImageGap(IconRecources.Icon_Remove));

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
        private static Bitmap AddIconImageGap(Bitmap icon)
        {
            Point destPt = new Point(6, 0);
            Size size = new Size(22, 16);
            
            Bitmap bmp = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(icon, destPt);
            g.Dispose();
            return bmp;
            
        }
    }
}

namespace EcfCAMTools
{
    public class CompareSelectionTools : EcfToolBox
    {
        public event EventHandler SelectAllAddsClicked;
        public event EventHandler SelectNoneAddsClicked;
        public event EventHandler SelectAllUpdatesClicked;
        public event EventHandler SelectNoneUpdatesClicked;
        public event EventHandler SelectAllRemovesClicked;
        public event EventHandler SelectNoneRemovesClicked;

        private EcfToolBarThreeStateCheckBox ChangeAllAddsButton { get; } = new EcfToolBarThreeStateCheckBox(
            TextRecources.EcfFileCAMDialog_ToolTip_ChangeAllAdds, IconRecources.Icon_ReloadFile, IconRecources.Icon_Add, IconRecources.Icon_Remove);
        private EcfToolBarThreeStateCheckBox ChangeAllUpdatesButton { get; } = new EcfToolBarThreeStateCheckBox(
            TextRecources.EcfFileCAMDialog_ToolTip_ChangeAllUpdates, IconRecources.Icon_ReloadFile, IconRecources.Icon_Add, IconRecources.Icon_Remove);
        private EcfToolBarThreeStateCheckBox ChangeAllRemovesButton { get; } = new EcfToolBarThreeStateCheckBox(
            TextRecources.EcfFileCAMDialog_ToolTip_ChangeAllRemoves, IconRecources.Icon_ReloadFile, IconRecources.Icon_Add, IconRecources.Icon_Remove);

        public CompareSelectionTools() : base()
        {
            Add(ChangeAllAddsButton).Click += ChangeAllAddsButton_Click;
            Add(ChangeAllUpdatesButton).Click += ChangeAllUpdatesButton_Click;
            Add(ChangeAllRemovesButton).Click += ChangeAllRemovesButton_Click;
        }

        // events
        private void ChangeAllAddsButton_Click(object sender, EventArgs evt)
        {
            if (ChangeAllAddsButton.CheckState == CheckState.Indeterminate)
            {
                ChangeAllAddsButton.CheckState = CheckState.Unchecked;
                SelectNoneAddsClicked?.Invoke(sender, evt);
            }
            else if(ChangeAllAddsButton.CheckState == CheckState.Checked)
            {
                SelectAllAddsClicked?.Invoke(sender, evt);
            }
        }
        private void ChangeAllUpdatesButton_Click(object sender, EventArgs evt)
        {
            if (ChangeAllUpdatesButton.CheckState == CheckState.Indeterminate)
            {
                ChangeAllUpdatesButton.CheckState = CheckState.Unchecked;
                SelectNoneUpdatesClicked?.Invoke(sender, evt);
            }
            else if (ChangeAllUpdatesButton.CheckState == CheckState.Checked)
            {
                SelectAllUpdatesClicked?.Invoke(sender, evt);
            }
        }
        private void ChangeAllRemovesButton_Click(object sender, EventArgs evt)
        {
            if (ChangeAllRemovesButton.CheckState == CheckState.Indeterminate)
            {
                ChangeAllRemovesButton.CheckState = CheckState.Unchecked;
                SelectNoneRemovesClicked?.Invoke(sender, evt);
            }
            else if (ChangeAllRemovesButton.CheckState == CheckState.Checked)
            {
                SelectAllRemovesClicked?.Invoke(sender, evt);
            }
        }

        // publics
        public void SetAllAddsState(CheckState state)
        {
            ChangeAllAddsButton.CheckState = state;
        }
        public void SetAllUpdatesButton(CheckState state)
        {
            ChangeAllUpdatesButton.CheckState = state;
        }
        public void SetAllRemovesButton(CheckState state)
        {
            ChangeAllRemovesButton.CheckState = state;
        }
    }
    public class MergeActionTools : EcfToolBox
    {
        public event EventHandler SelectAllAddingsClicked;

        public MergeActionTools() : base()
        {
            Add(new EcfToolBarThreeStateCheckBox("Test", 
                IconRecources.Icon_CompareAndMerge,
                IconRecources.Icon_AddValue,
                IconRecources.Icon_ApplyFilter)
                ).Click += (sender, evt) => SelectAllAddingsClicked?.Invoke(sender, evt);
        }
    }
}
