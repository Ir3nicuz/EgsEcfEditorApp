using EgsEcfEditorApp.Properties;
using System;
using System.Linq;
using System.Windows.Forms;

namespace EgsEcfEditorApp
{
    public partial class OptionSelectorDialog : Form
    {
        public OptionItem SelectedOption { get; private set; } = null;
        private OptionItem[] OptionItemList { get; set; } = null;

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
        private void Option_CheckedChanged(object sender, EventArgs evt)
        {
            UpdateSelectedOption(sender as RadioButton);
        }

        // publics
        public DialogResult ShowDialog(IWin32Window parent, string header, OptionItem[] options)
        {
            Text = header;
            OptionItemList = options.ToArray();
            UpdateOptionPanel(options);
            return ShowDialog(parent);
        }
        
        //private
        private void UpdateOptionPanel(OptionItem[] options)
        {
            OkButton.Enabled = false;
            OptionPanel.Controls.Clear();
            foreach(OptionItem option in options)
            {
                RadioButton button = new RadioButton()
                {
                    AutoSize = true,
                    Text = option.DisplayText,
                };
                button.CheckedChanged += Option_CheckedChanged;
                OptionPanel.Controls.Add(button);
            }
        }
        private void UpdateSelectedOption(RadioButton optionButton)
        {
            if (optionButton != null && optionButton.Checked)
            {
                SelectedOption = OptionItemList[OptionPanel.Controls.IndexOf(optionButton)];
                OkButton.Enabled = true;
            }
        }

        public class OptionItem
        {
            public object Item { get; }
            public string DisplayText { get; }

            public OptionItem(object item)
            {
                Item = item;
                DisplayText = string.Format("\"{0}\"", Convert.ToString(item));
            }
            public OptionItem(object item, string displayText)
            {
                Item = item;
                DisplayText = displayText;
            }
        }
    }
}
