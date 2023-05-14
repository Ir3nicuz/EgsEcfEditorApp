using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GenericDialogs
{
    public static class GenericDialogs
    {
        public static DialogResult ShowOperationSafetyQuestionDialog(IWin32Window parent, string header, string message, List<string> problems)
        {
            if (problems.Count < 1) { return DialogResult.Yes; }

            StringBuilder messageBuilder = new StringBuilder(message);
            messageBuilder.Append(Environment.NewLine);
            problems.ForEach(problem =>
            {
                messageBuilder.Append(Environment.NewLine);
                messageBuilder.Append(problem);
            });

            return MessageBox.Show(parent, message.ToString(), header, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
        }
        public static void ShowExceptionMessageDialog(IWin32Window parent, Exception ex, string title, string preDescription = null)
        {
            string message;
            if (string.IsNullOrEmpty(preDescription))
            {
                message = ex.Message;
            }
            else
            {
                message = string.Format("{0}{1}{1}{2}", preDescription, Environment.NewLine, ex.Message);
            }
            MessageBox.Show(parent, message, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
