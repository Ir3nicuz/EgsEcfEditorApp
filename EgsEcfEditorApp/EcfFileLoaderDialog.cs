using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace EgsEcfEditorApp
{
    public partial class EcfFileLoaderDialog : Form
    {
        private EgsEcfFile File { get; set; } = null;
        private BackgroundWorker Worker { get; } = new BackgroundWorker();
        private Progress<int> ProgressInterface { get; } = new Progress<int>();

        public EcfFileLoaderDialog()
        {
            InitializeComponent();
            InitForm();
        }

        // event
        private void InitForm()
        {
            Icon = IconRecources.Icon_AppBranding;
            InitGui();
            InitWorker();
        }
        private void InitGui()
        {
            Text = TitleRecources.EcfFileLoadingDialog_Header;

            AbortButton.Text = TitleRecources.Generic_Abort;

            ProgressIndicator.BarText = TitleRecources.EcfFileLoadingDialog_BarText;
        }
        private void InitWorker()
        {
            Worker.WorkerReportsProgress = true;
            Worker.WorkerSupportsCancellation = true;

            Worker.DoWork += Worker_DoWork;
            Worker.ProgressChanged += Worker_ProgressChanged;
            Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            ProgressInterface.ProgressChanged += ProgressInterface_ProgressChanged;
        }
        private void ProgressDialog_FormClosing(object sender, FormClosingEventArgs evt)
        {
            if (DialogResult != DialogResult.OK) { File.LoadAbortPending = true; }
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
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs evt)
        {
            UpdateProgress(evt.ProgressPercentage);
        }
        private void Worker_DoWork(object sender, DoWorkEventArgs evt)
        {
            if (evt.Argument is EgsEcfFile file)
            {
                file.Load(ProgressInterface);
                evt.Cancel = File.LoadAbortPending;
            }
        }
        private void ProgressInterface_ProgressChanged(object sender, int line)
        {
            UpdateProgress(line);
        }

        // public
        public DialogResult ShowDialog(IWin32Window parent, EgsEcfFile file)
        {
            File = file;
            PrepareProgress(file);
            Worker.RunWorkerAsync(file);
            return ShowDialog(parent);
        }
        public void PrepareProgress(EgsEcfFile file)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    PrepareProgressInvoked(file);
                });
            }
            else
            {
                PrepareProgressInvoked(file);
            }
        }
        public void UpdateProgress(int value)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    UpdateProgressInvoked(value);
                });
            }
            else
            {
                UpdateProgressInvoked(value);
            }
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
        private void PrepareProgressInvoked(EgsEcfFile file)
        {
            FilePathAndNameTextBox.Text = Path.Combine(file.FilePath, file.FileName);
            ProgressIndicator.Maximum = file.LineCount;
            ProgressIndicator.Value = ProgressIndicator.Minimum;
        }
        private void UpdateProgressInvoked(int value)
        {
            ProgressIndicator.Value = value;
        }
        private void CloseFormInvoked(DialogResult result)
        {
            DialogResult = result;
            Close();
        }
    }
}
