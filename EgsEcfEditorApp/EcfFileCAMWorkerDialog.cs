using EgsEcfEditorApp.Properties;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace EgsEcfEditorApp
{
    public partial class EcfFileCAMWorkerDialog : Form
    {
        private EcfFileCAMDialog CAMDialog { get; set; } = null;
        private BackgroundWorker Worker { get; } = new BackgroundWorker();

        public EcfFileCAMWorkerDialog()
        {
            InitializeComponent();
            InitForm();
        }

        // events
        private void InitForm()
        {
            Icon = IconRecources.Icon_AppBranding;
            InitGui();
            InitWorker();
        }
        private void InitGui()
        {
            Text = TitleRecources.EcfFileCAMDialog_Header;

            AbortButton.Text = TitleRecources.Generic_Abort;
        }
        private void InitWorker()
        {
            Worker.WorkerSupportsCancellation = true;

            Worker.DoWork += Worker_DoWork;
            Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }
        private void EcfFileCAMWorkerDialog_FormClosing(object sender, FormClosingEventArgs evt)
        {
            if (DialogResult != DialogResult.OK) { CAMDialog.CAMAbortPending = true; }
            if (Worker.IsBusy) { evt.Cancel = true; }
        }
        private void AbortButton_Click(object sender, EventArgs evt)
        {
            CloseForm(DialogResult.Abort);
        }
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs evt)
        {
            CloseForm(evt.Cancelled ? DialogResult.Abort : DialogResult.OK);
        }
        private void Worker_DoWork(object sender, DoWorkEventArgs evt)
        {
            if (evt.Argument is Action action)
            {
                action();
                evt.Cancel = CAMDialog.CAMAbortPending;
            }
        }

        //publics
        public DialogResult ShowDialog(IWin32Window parent, string actionName, EcfFileCAMDialog cam, Action action)
        {
            CAMDialog = cam;
            CAMActionLabel.Text = actionName;
            Worker.RunWorkerAsync(action);
            return ShowDialog(parent);
        }
        public void CloseForm(DialogResult result)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    CloseFormInvoked(result);
                });
            }
            else
            {
                CloseFormInvoked(result);
            }
        }

        // private
        private void CloseFormInvoked(DialogResult result)
        {
            DialogResult = result;
            Close();
        }
    }
}
