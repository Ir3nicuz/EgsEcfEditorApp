using System;
using System.Windows.Forms;

namespace EgsEcfEditorApp
{
    public partial class TextInputDialog : Form
    {
        public string OkButtonText { 
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

        public string InputText { get; private set; } = null;

        public TextInputDialog()
        {
            InitializeComponent();
        }

        // events
        private void OkButton_Click(object sender, EventArgs evt)
        {
            InputText = InputTextBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
        private void AbortButton_Click(object sender, EventArgs evt)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }
        private void InputTextBox_KeyPress(object sender, KeyPressEventArgs evt)
        {
            if (evt.KeyChar == (char)Keys.Enter)
            {
                InputText = InputTextBox.Text;
                DialogResult = DialogResult.OK;
                Close();
                evt.Handled = true;
            }
        }
        private void TextInputDialog_Activated(object sender, EventArgs evt)
        {
            InputTextBox.SelectAll();
        }

        // publics
        public DialogResult ShowDialog(IWin32Window parent, string caption, string initText)
        {
            Text = caption;
            return ShowDialog(parent, initText);
        }
        public DialogResult ShowDialog(IWin32Window parent, string initText)
        {
            InputText = initText;
            InputTextBox.Text = initText;
            return ShowDialog(parent);
        }
    }
}
