using EgsEcfEditorApp.Properties;
using System;
using System.Windows.Forms;

namespace EgsEcfEditorApp
{
    public partial class EcfFileCAMDialog : Form
    {
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
        [Obsolete("needs works")]
        private void CloseButton_Click(object sender, EventArgs evt)
        {

        }

        // public
        

        
    }
}
