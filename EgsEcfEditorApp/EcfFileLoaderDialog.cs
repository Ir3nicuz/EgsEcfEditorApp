using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EgsEcfEditorApp
{
    public partial class EcfFileLoaderDialog : Form
    {
        private WorkItem Item { get; set; } = null;
        private Progress<int> ProgressInterface { get; } = new Progress<int>();

        private BackgroundWorker FileLoadWorker { get; } = new BackgroundWorker();
        private BackgroundWorker DefinitionReplaceWorker { get; } = new BackgroundWorker();

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
            FileLoadWorker.WorkerReportsProgress = true;
            FileLoadWorker.WorkerSupportsCancellation = true;
            FileLoadWorker.DoWork += FileLoadWorker_DoWork;
            FileLoadWorker.ProgressChanged += Worker_ProgressChanged;
            FileLoadWorker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            DefinitionReplaceWorker.WorkerReportsProgress = true;
            DefinitionReplaceWorker.WorkerSupportsCancellation = true;
            DefinitionReplaceWorker.DoWork += DefinitionReplaceWorker_DoWork;
            DefinitionReplaceWorker.ProgressChanged += Worker_ProgressChanged;
            DefinitionReplaceWorker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            ProgressInterface.ProgressChanged += ProgressInterface_ProgressChanged;
        }
        private void ProgressDialog_FormClosing(object sender, FormClosingEventArgs evt)
        {
            if (DialogResult != DialogResult.OK) { Item.File.LoadAbortPending = true; }
            if (FileLoadWorker.IsBusy || DefinitionReplaceWorker.IsBusy) { evt.Cancel = true; }
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
        private void FileLoadWorker_DoWork(object sender, DoWorkEventArgs evt)
        {
            if (evt.Argument is WorkItem workItem)
            {
                workItem.File.Load(ProgressInterface);
                evt.Cancel = workItem.File.LoadAbortPending;
            }
        }
        private void DefinitionReplaceWorker_DoWork(object sender, DoWorkEventArgs evt)
        {
            if (evt.Argument is WorkItem workItem)
            {
                workItem.File.ReplaceDefinition(workItem.Definition, ProgressInterface);
                evt.Cancel = workItem.File.LoadAbortPending;
            }
        }
        private void ProgressInterface_ProgressChanged(object sender, int line)
        {
            UpdateProgress(line);
        }

        // public
        public DialogResult ShowDialog(IWin32Window parent, EgsEcfFile file)
        {
            Item = new WorkItem(file, null);
            PrepareProgress();
            FileLoadWorker.RunWorkerAsync(Item);
            return ShowDialog(parent);
        }
        public DialogResult ShowDialog(IWin32Window parent, EgsEcfFile file, FormatDefinition newDefinition)
        {
            Item = new WorkItem(file, newDefinition);
            PrepareProgress();
            DefinitionReplaceWorker.RunWorkerAsync(Item);
            return ShowDialog(parent);
        }
        public void PrepareProgress()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    PrepareProgressInvoked(Item.File);
                });
            }
            else
            {
                PrepareProgressInvoked(Item.File);
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

        // classes
        private class WorkItem
        {
            public EgsEcfFile File { get; }
            public FormatDefinition Definition { get; }

            public WorkItem(EgsEcfFile file, FormatDefinition definition)
            {
                File = file;
                Definition = definition;
            }
        }
        private class TextProgressBar : ProgressBar
        {
            public string BarText { get; set; } = string.Empty;

            public TextProgressBar() : base()
            {
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            }

            protected override void OnPaint(PaintEventArgs evt)
            {
                Rectangle barArea = ClientRectangle;
                Graphics gfx = evt.Graphics;
                gfx.PageUnit = GraphicsUnit.Pixel;

                ProgressBarRenderer.DrawHorizontalBar(gfx, barArea);
                barArea.Inflate(-3, -3);
                Font barTextFont = new Font(Font.FontFamily, barArea.Height - 3, GraphicsUnit.Pixel);
                if (Value > 0)
                {
                    Rectangle chunkArea = new Rectangle(barArea.X, barArea.Y, (int)((float)Value / Maximum * barArea.Width), barArea.Height);
                    ProgressBarRenderer.DrawHorizontalChunks(gfx, chunkArea);
                }
                string barText = string.Format("{0} {1} / {2}", BarText, Value, Maximum);
                float barTextStart = (barArea.Width - gfx.MeasureString(barText, barTextFont).Width) / 2;
                gfx.DrawString(barText, barTextFont, SystemBrushes.ControlText, barTextStart, 1);
            }
        }
    }
}
