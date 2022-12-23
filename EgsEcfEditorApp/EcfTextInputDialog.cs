﻿using EgsEcfEditorApp.Properties;
using System;
using System.Windows.Forms;

namespace EgsEcfEditorApp
{
    public partial class EcfTextInputDialog : Form
    {
        public EcfTextInputDialog(string caption)
        {
            InitializeComponent();
            InitForm(caption);
        }

        // events
        private void InitForm(string caption)
        {
            Icon = IconRecources.Icon_AppBranding;
            Text = caption;

            OkButton.Text = TitleRecources.Generic_Ok;
            AbortButton.Text = TitleRecources.Generic_Abort;
        }
        private void OkButton_Click(object sender, EventArgs evt)
        {
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
                DialogResult = DialogResult.OK;
                Close();
                evt.Handled = true;
            }
        }
        private void EcfTextInputDialog_Activated(object sender, EventArgs evt)
        {
            OkButton.Focus();
        }

        // publics
        public DialogResult ShowDialog(IWin32Window parent, string caption, string initText)
        {
            Text = caption;
            return ShowDialog(parent, initText);
        }
        public DialogResult ShowDialog(IWin32Window parent, string initText)
        {
            InputTextBox.Text = initText;
            return ShowDialog(parent);
        }
        public string GetText()
        {
            return InputTextBox.Text;
        }

        private void EcfTextInputDialog_Activated_1(object sender, EventArgs e)
        {
            InputTextBox.SelectAll();
        }
    }
}
