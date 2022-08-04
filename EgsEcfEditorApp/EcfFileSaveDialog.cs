using EgsEcfEditorApp.Properties;
using System.Windows.Forms;

namespace EcfFileViews
{
    public partial class EcfFileSaveDialog : Form
    {
        public string FilePathAndName { get; private set; } = null;

        private SaveFileDialog CreateFileDialog { get; } = new SaveFileDialog();

        public EcfFileSaveDialog()
        {
            InitializeComponent();
            InitForm();
        }

        // events
        private void InitForm()
        {
            Icon = IconRecources.Icon_App;

            InitCreateFileDialog();
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
        public void SetInitFileName(string fileName)
        {
            CreateFileDialog.FileName = fileName;
        }

        // privates
        private void InitCreateFileDialog()
        {
            CreateFileDialog.AddExtension = true;
            CreateFileDialog.DefaultExt = InternalSettings.Default.EgsEcfEditorApp_FileDialogExtension;
            CreateFileDialog.Filter = InternalSettings.Default.EgsEcfEditorApp_FileDialogFilter;
            CreateFileDialog.Title = TitleRecources.EcfFileDialog_CreateFileDialog;
            CreateFileDialog.ShowHelp = false;
            SetInitFileName(TitleRecources.EcfFileDialog_CreateFileName);
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
