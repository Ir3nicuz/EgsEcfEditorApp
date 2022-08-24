using EcfCAMTools;
using EcfFileViews;
using EcfToolBarControls;
using EgsEcfEditorApp.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EgsEcfEditorApp
{
    public partial class EcfFileCAMDialog : Form
    {
        public List<EcfTabPage> ChangedFileTabs { get; } = new List<EcfTabPage>();

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

            FirstFileSelectionContainer.Add(new CompareSelectionTools());
            FirstFileActionContainer.Add(new MergeActionTools());
            SecondFileActionContainer.Add(new MergeActionTools());
            SecondFileSelectionContainer.Add(new CompareSelectionTools());
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
    }
}

namespace EcfCAMTools
{
    public class CompareSelectionTools : EcfToolBox
    {
        public event EventHandler SelectAllAddingsClicked;

        public CompareSelectionTools() : base()
        {
            Add(new EcfToolBarThreeStateCheckBox("Test",
                IconRecources.Icon_CompareAndMerge,
                IconRecources.Icon_AddValue,
                IconRecources.Icon_ApplyFilter)
                ).Click += (sender, evt) => SelectAllAddingsClicked?.Invoke(sender, evt);
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
