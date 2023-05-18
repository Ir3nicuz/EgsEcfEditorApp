using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

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

        public ErrorListingDialog()
        {
            InitializeComponent();
        }

        // events
        private void ErrorListingDialog_Activated(object sender, EventArgs evt)
        {
            UpdateMessageLabelWidth();
        }
        private void ErrorListingDialog_ResizeEnd(object sender, EventArgs evt)
        {
            UpdateMessageLabelWidth();
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

        // public
        public DialogResult ShowDialog(IWin32Window parent, string question, List<string> errors)
        {
            if (errors == null || errors.Count < 1) { return DialogResult.Yes; }
            PrepareErrorListing();
            MessageLabel.Text = question ?? string.Empty;
            ErrorRichTextBox.Text = string.Join(Environment.NewLine, errors);
            return ShowDialog(parent);
        }
        public DialogResult ShowDialog(IWin32Window parent, string customReason, Exception ex)
        {
            PrepareExcepotionListing();
            MessageLabel.Text = customReason ?? string.Empty;
            ErrorRichTextBox.Text = PrepareExceptionText(ex);
            return ShowDialog(parent);
        }

        // private
        private void PrepareErrorListing()
        {
            MessageIcon.Image = SystemIcons.Question.ToBitmap();
            ErrorRichTextBox.SelectionBullet = true;
            OkButton.Visible = false;
            YesButton.Visible = true;
            NoButton.Visible = true;
            AbortButton.Visible = true;
        }
        private void PrepareExcepotionListing()
        {
            MessageIcon.Image = SystemIcons.Exclamation.ToBitmap();
            ErrorRichTextBox.SelectionBullet = false;
            OkButton.Visible = true;
            YesButton.Visible = false;
            NoButton.Visible = false;
            AbortButton.Visible = false;
        }
        private string PrepareExceptionText(Exception ex)
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine(string.Format("HResult:"));
            text.AppendLine(string.Format("\t{0}", ex.HResult));

            if (!string.IsNullOrEmpty(ex.Message))
            {
                text.AppendLine();
                text.AppendLine(string.Format("Exception:"));
                text.AppendLine(string.Format("\t{0}", ex.Message));
            }
            if (!string.IsNullOrEmpty(ex.Source))
            {
                text.AppendLine();
                text.AppendLine(string.Format("Source:"));
                text.AppendLine(string.Format("\t{0}", ex.Source));
            }
            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                text.AppendLine();
                text.AppendLine(string.Format("StackTrace:"));
                text.AppendLine(string.Format("\t{0}", ex.StackTrace));
            }
            if (ex.InnerException != null)
            {
                text.AppendLine();
                text.AppendLine(string.Format("InnerException:"));
                text.AppendLine();
                text.AppendLine(PrepareExceptionText(ex.InnerException));
            }

            return text.ToString();
        }
        private void UpdateMessageLabelWidth()
        {
            int maxWidth = MessagePanel.GetColumnWidths()[MessagePanel.GetColumn(MessageLabel)];
            MessageLabel.MaximumSize = new Size(maxWidth, int.MaxValue);
        }
    }
}
