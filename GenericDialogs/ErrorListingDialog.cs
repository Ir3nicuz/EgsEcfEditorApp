using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GenericDialogs
{
    public partial class ErrorListingDialog : Form
    {
        public string YesButtonText
        {
            get
            {
                return YesButton.Text;
            }
            set
            {
                YesButton.Text = value;
            }
        }
        public string NoButtonText
        {
            get
            {
                return NoButton.Text;
            }
            set
            {
                NoButton.Text = value;
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

        public ErrorListingDialog()
        {
            InitializeComponent();
            InitForm();
        }

        // events
        private void InitForm()
        {
            QuestionIcon.Image = SystemIcons.Question.ToBitmap();
            ErrorRichTextBox.SelectionBullet = true;
        }
        private void ErrorListingDialog_Activated(object sender, EventArgs evt)
        {
            UpdateQuestionLabelWidth();
        }
        private void ErrorListingDialog_ResizeEnd(object sender, EventArgs evt)
        {
            UpdateQuestionLabelWidth();
        }
        private void YesButton_Click(object sender, EventArgs evt)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }
        private void NoButton_Click(object sender, EventArgs evt)
        {
            DialogResult = DialogResult.No;
            Close();
        }
        private void AbortButton_Click(object sender, EventArgs evt)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }

        // public
        public DialogResult ShowDialog(IWin32Window parent, string question, List<string> errors)
        {
            if (errors == null || errors.Count < 1) { return DialogResult.Yes; }
            QuestionLabel.Text = question ?? string.Empty;
            ErrorRichTextBox.Text = string.Join(Environment.NewLine, errors);
            return ShowDialog(parent);
        }

        // private
        private void UpdateQuestionLabelWidth()
        {
            int maxWidth = QuestionPanel.GetColumnWidths()[QuestionPanel.GetColumn(QuestionLabel)];
            QuestionLabel.MaximumSize = new Size(maxWidth, int.MaxValue);
        }
    }
}
