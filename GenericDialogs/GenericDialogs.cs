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
    }
}
