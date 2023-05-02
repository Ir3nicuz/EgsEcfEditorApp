using EgsEcfEditorApp.Properties;
using System;
using System.Windows.Forms;

namespace EgsEcfEditorApp
{
    public partial class OptionSelectorDialog : Form
    {
        public OptionSelectorDialog()
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

        // publics
        public DialogResult ShowDialog(IWin32Window parent, string header, OptionItem[] options)
        {
            Text = header;
            
            return ShowDialog(parent);
        }
        
        public class OptionItem
        {
            public string Text { get; }

            public OptionItem(string text)
            {
                Text = text;
            }
        }
    }
}
