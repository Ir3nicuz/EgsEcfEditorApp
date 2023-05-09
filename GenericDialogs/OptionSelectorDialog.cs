using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace EgsEcfEditorApp
{
    public partial class OptionSelectorDialog : Form
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
        public OptionItem SelectedOption { get; private set; } = null;
        private OptionItem[] OptionItemList { get; set; } = null;

        public OptionSelectorDialog()
        {
            InitializeComponent();
        }

        // events
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
        private void Option_MouseDoubleClick(object sender, MouseEventArgs evt)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        // publics
        public DialogResult ShowDialog(IWin32Window parent, OptionItem[] options)
        {
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
                DoubleClickRadioButton button = new DoubleClickRadioButton()
                {
                    AutoSize = true,
                    Text = option.DisplayText,
                };
                button.CheckedChanged += Option_CheckedChanged;
                button.MouseDoubleClick += Option_MouseDoubleClick;
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
        private class DoubleClickRadioButton : RadioButton
        {
            [EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
            public new event MouseEventHandler MouseDoubleClick;

            public DoubleClickRadioButton() : base()
            {
                SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true);

                AutoSize = true;
            }

            protected override void OnMouseDoubleClick(MouseEventArgs evt)
            {
                MouseDoubleClick?.Invoke(this, evt);
            }
        }
    }
}
