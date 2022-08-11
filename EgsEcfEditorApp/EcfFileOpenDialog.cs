using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static EgsEcfParser.EcfDefinitionHandling;

namespace EcfFileViews
{
    public partial class EcfFileOpenDialog : Form
    {
        public string FilePathAndName { get; private set; } = null;
        public FormatDefinition FileDefinition { get; private set; } = null;
        public Encoding FileEncoding { get; private set; } = null;
        public EcfFileNewLineSymbols FileNewLineSymbol { get; private set; } = EcfFileNewLineSymbols.CrLf;

        private List<FormatDefinition> Definitions { get; set; } = new List<FormatDefinition>();

        private SaveFileDialog CreateFileDialog { get; } = new SaveFileDialog();
        private OpenFileDialog FindFileDialog { get; } = new OpenFileDialog();

        private ToolTip FormatDefinitionTooltip { get; } = new ToolTip();
        private ToolTip EncodingTooltip { get; } = new ToolTip();
        private ToolTip NewLineSymbolTooltip { get; } = new ToolTip();

        private Func<IWin32Window, DialogResult> FileOperation { get; set; }

        public EcfFileOpenDialog()
        {
            InitializeComponent();
            InitForm();
        }

        //events
        private void InitForm()
        {
            Icon = IconRecources.Icon_App;

            InitFilePathAndNameBox("");
            InitFormatDefinitionBox();
            InitEncodingBox();
            InitNewLineSymbolBox();

            InitCreateFileDialog();
            InitFindFileDialog();

            OkButton.Text = TitleRecources.Generic_Ok;
            AbortButton.Text = TitleRecources.Generic_Abort;
        }
        private void EcfFilePropertiesDialog_Activated(object sender, EventArgs evt)
        {
            OkButton.Focus();
        }
        private void FilePathAndNameTextBox_Click(object sender, EventArgs evt)
        {
            FileOperation(this);
        }
        private void FormatDefinitionComboBox_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            FileDefinition = Definitions.FirstOrDefault(definition => definition.FileType.Equals(Convert.ToString(FormatDefinitionComboBox.SelectedItem)));
        }
        private void EncodingComboBox_SelectedIndexChanged(object sender, EventArgs evt)
        {
            FileEncoding = Encoding.GetEncoding(Convert.ToString(EncodingComboBox.SelectedItem));
        }
        private void NewLineSymbolComboBox_SelectedIndexChanged(object sender, EventArgs evt)
        {
            FileNewLineSymbol = (EcfFileNewLineSymbols)Enum.Parse(typeof(EcfFileNewLineSymbols), Convert.ToString(NewLineSymbolComboBox.SelectedItem));
        }
        private void OkButton_Click(object sender, EventArgs evt)
        {
            if (string.IsNullOrEmpty(FilePathAndName) || FileDefinition == null || 
                FileEncoding == null || FileNewLineSymbol == EcfFileNewLineSymbols.Unknown)
            {
                MessageBox.Show(this, TextRecources.EcfFilePropertiesDialog_PropertiesIncomplete, 
                    TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
        private void AbortButton_Click(object sender, EventArgs evt)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }

        // publics
        public DialogResult ShowDialogNewFile(IWin32Window parent, string gameVersion)
        {
            Definitions = GetSupportedFileTypes(gameVersion);
            FileOperation = CreateFile;
            RefreshFormatDefinitionBox();
            SetAppearanceNewFile();
            DialogResult result = FileOperation(parent);
            if (result != DialogResult.OK) { return result; }
            return ShowDialog(parent);
        }
        public DialogResult ShowDialogOpenFile(IWin32Window parent, string gameVersion)
        {
            Definitions = GetSupportedFileTypes(gameVersion);
            FileOperation = FindFile;
            RefreshFormatDefinitionBox();
            SetAppearanceOpenFile();
            DialogResult result = FileOperation(parent);
            if (result != DialogResult.OK) { return result; }
            return ShowDialog(parent);
        }
        public void SetInitDirectory(string directory)
        {
            CreateFileDialog.InitialDirectory = directory;
            FindFileDialog.InitialDirectory = directory;
        }
        public void SetInitFileName(string fileName)
        {
            CreateFileDialog.FileName = fileName;
            FindFileDialog.FileName = fileName;
        }

        // privates
        private void InitFilePathAndNameBox(string filePathAndName)
        {
            FilePathAndNameLabel.Text = TitleRecources.EcfFileOpenDialog_FilePathAndNameBox;
            FilePathAndNameTextBox.Text = filePathAndName;
            FilePathAndNameTextBox.Click += FilePathAndNameTextBox_Click;
        }
        private void InitFormatDefinitionBox()
        {
            FormatDefinitionLabel.Text = TitleRecources.EcfFileOpenDialog_FormatDefinitionBox;
            FormatDefinitionTooltip.SetToolTip(FormatDefinitionComboBox, TextRecources.EcfFileOpenDialog_ToolTip_FormatDefinition);
        }
        private void RefreshFormatDefinitionBox()
        {
            FormatDefinitionComboBox.Items.Clear();
            FormatDefinitionComboBox.Items.AddRange(Definitions.Select(definition => definition.FileType).ToArray());
        }
        private FormatDefinition GetFileDefinition()
        {
            return Definitions.FirstOrDefault(definition => FilePathAndName.Contains(definition.FileType));
        }
        private void InitEncodingBox()
        {
            EncodingLabel.Text = TitleRecources.EcfFileOpenDialog_EncodingBox;
            EncodingTooltip.SetToolTip(EncodingComboBox, TextRecources.EcfFileOpenDialog_ToolTip_Encoding);
            EncodingComboBox.Items.AddRange(Encoding.GetEncodings().Select(encoding => encoding.Name).ToArray());
            EncodingComboBox.SelectedItem = Encoding.UTF8.WebName;
            FileEncoding = Encoding.UTF8;
        }
        private void InitNewLineSymbolBox()
        {
            NewLineSymbolLabel.Text = TitleRecources.EcfFileOpenDialog_NewLineSymbolBox;
            NewLineSymbolTooltip.SetToolTip(NewLineSymbolComboBox, TextRecources.EcfFileOpenDialog_ToolTip_NewLineSymbol);
            NewLineSymbolComboBox.Items.AddRange(Enum.GetValues(typeof(EcfFileNewLineSymbols)).Cast<EcfFileNewLineSymbols>().Skip(1).Select(value => value.ToString()).ToArray());
            NewLineSymbolComboBox.SelectedItem = EcfFileNewLineSymbols.CrLf.ToString();
            FileNewLineSymbol = EcfFileNewLineSymbols.CrLf;
        }
        private void InitCreateFileDialog()
        {
            CreateFileDialog.AddExtension = true;
            CreateFileDialog.DefaultExt = InternalSettings.Default.EgsEcfEditorApp_FileDialogExtension;
            CreateFileDialog.Filter = InternalSettings.Default.EgsEcfEditorApp_FileDialogFilter;
            CreateFileDialog.Title = TitleRecources.EcfFileDialog_CreateFileDialog;
            CreateFileDialog.ShowHelp = false;
            CreateFileDialog.FileName = TitleRecources.EcfFileDialog_CreateFileName;
        }
        private void InitFindFileDialog()
        {
            FindFileDialog.AddExtension = true;
            FindFileDialog.DefaultExt = InternalSettings.Default.EgsEcfEditorApp_FileDialogExtension;
            FindFileDialog.Filter = InternalSettings.Default.EgsEcfEditorApp_FileDialogFilter;
            FindFileDialog.Title = TitleRecources.EcfFileOpenDialog_FindFileDialog;
            FindFileDialog.Multiselect = false;
            FindFileDialog.ShowHelp = false;
        }
        private void SetAppearanceNewFile()
        {
            EncodingComboBox.Enabled = true;
            NewLineSymbolComboBox.Enabled = true;

            Text = TitleRecources.EcfFileOpenDialog_NewFileDialog;
        }
        private void SetAppearanceOpenFile()
        {
            EncodingComboBox.Enabled = false;
            NewLineSymbolComboBox.Enabled = false;

            Text = TitleRecources.EcfFileOpenDialog_OpenFileDialog;
        }
        private DialogResult CreateFile(IWin32Window parent)
        {
            DialogResult result = CreateFileDialog.ShowDialog(parent);
            if (result != DialogResult.OK) { return result; }

            FilePathAndName = CreateFileDialog.FileName;
            FileDefinition = GetFileDefinition();

            FilePathAndNameTextBox.Text = FilePathAndName;
            FormatDefinitionComboBox.SelectedItem = FileDefinition?.FileType;

            return result;
        }
        private DialogResult FindFile(IWin32Window parent)
        {
            DialogResult result = FindFileDialog.ShowDialog(parent);
            if (result != DialogResult.OK) { return result; }

            FilePathAndName = FindFileDialog.FileName;
            FileDefinition = GetFileDefinition();
            FileEncoding = GetFileEncoding(FilePathAndName);
            FileNewLineSymbol = GetNewLineSymbol(FilePathAndName);

            FilePathAndNameTextBox.Text = FilePathAndName;
            FormatDefinitionComboBox.SelectedItem = FileDefinition?.FileType;
            EncodingComboBox.SelectedItem = FileEncoding.WebName;
            NewLineSymbolComboBox.SelectedItem = FileNewLineSymbol.ToString();

            return result;
        }
    }
}
