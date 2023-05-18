using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GenericDialogs
{
    public static class GenericDialogs
    {
        [Obsolete("profits from generic exception dialog")]
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
