using EcfFileViews;
using EcfFileViewTools;
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
        }
        private void CloseButton_Click(object sender, EventArgs evt)
        {
            Close();
        }
        private void FirstFileGrid_Scroll(object sender, ScrollEventArgs evt)
        {
            if (evt.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                SecondFileGrid.VerticalScrollingOffset = FirstFileGrid.VerticalScrollingOffset;
            }
            else
            {
                SecondFileGrid.HorizontalScrollingOffset = FirstFileGrid.HorizontalScrollingOffset;
            }
        }
        private void SecondFileGrid_Scroll(object sender, ScrollEventArgs evt)
        {
            if (evt.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                FirstFileGrid.VerticalScrollingOffset = SecondFileGrid.VerticalScrollingOffset;
            }
            else
            {
                FirstFileGrid.HorizontalScrollingOffset = SecondFileGrid.HorizontalScrollingOffset;
            }
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
