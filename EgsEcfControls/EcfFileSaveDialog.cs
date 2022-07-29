using System;
using System.Drawing;
using System.Windows.Forms;

namespace EgsEcfControls
{
    public partial class EcfFileSaveDialog : Form
    {
        public string FilePathAndName { get; private set; } = null;

        private SaveFileDialog CreateFileDialog { get; } = new SaveFileDialog();

        public EcfFileSaveDialog(Icon appIcon, string fileFilter, string fileExtension)
        {
            InitializeComponent();
            InitForm(appIcon, fileFilter, fileExtension);
        }

        // events
        private void InitForm(Icon appIcon, string fileFilter, string fileExtension)
        {
            Icon = appIcon;

            InitCreateFileDialog(fileFilter, fileExtension);
        }

        // publics
        public DialogResult ShowDialogSaveAs(IWin32Window parent)
        {
            return SaveAsOperation(parent);
        }
        public DialogResult ShowDialogSaveAsFiltered(IWin32Window parent)
        {
            


            return ShowDialog(parent);
        }
        public void SetInitDirectory(string directory)
        {
            CreateFileDialog.InitialDirectory = directory;
        }

        // privates
        private void InitCreateFileDialog(string fileFilter, string fileExtension)
        {
            CreateFileDialog.AddExtension = true;
            CreateFileDialog.DefaultExt = fileExtension;
            CreateFileDialog.Filter = fileFilter;
            CreateFileDialog.Title = Properties.titles.EcfFileDialog_CreateFileDialog;
            CreateFileDialog.ShowHelp = false;
            CreateFileDialog.FileName = Properties.titles.EcfFileDialog_CreateFileName;
        }
        private DialogResult SaveAsOperation(IWin32Window parent)
        {
            DialogResult result = CreateFileDialog.ShowDialog(parent);
            if (result != DialogResult.OK) { return result; }

            FilePathAndName = CreateFileDialog.FileName;

            return result;
        }
    }
}
