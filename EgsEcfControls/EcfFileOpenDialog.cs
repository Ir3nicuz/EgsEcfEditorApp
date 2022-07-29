using EgsEcfParser;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static EgsEcfParser.EcfFormatting;

namespace EgsEcfControls
{
    public partial class EcfFileOpenDialog : Form
    {
        public string FilePathAndName { get; private set; } = null;
        public FormatDefinition FileDefinition { get; private set; } = null;
        public Encoding FileEncoding { get; private set; } = null;
        public EcfFileNewLineSymbols FileNewLineSymbol { get; private set; } = EcfFileNewLineSymbols.CrLf;

        private SaveFileDialog CreateFileDialog { get; } = new SaveFileDialog();
        private OpenFileDialog FindFileDialog { get; } = new OpenFileDialog();

        private ToolTip FormatDefinitionTooltip { get; } = new ToolTip();
        private ToolTip EncodingTooltip { get; } = new ToolTip();
        private ToolTip NewLineSymbolTooltip { get; } = new ToolTip();

        private Func<IWin32Window, DialogResult> FileOperation { get; set; }

        public EcfFileOpenDialog(Icon appIcon, string fileFilter, string fileExtension)
        {
            InitializeComponent();
            InitForm(appIcon, fileFilter, fileExtension);
        }

        //events
        private void InitForm(Icon appIcon, string fileFilter, string fileExtension)
        {
            Icon = appIcon;

            InitFilePathAndNameBox("");
            InitFormatDefinitionBox(null);
            InitEncodingBox();
            InitNewLineSymbolBox();

            InitCreateFileDialog(fileFilter, fileExtension);
            InitFindFileDialog(fileFilter, fileExtension);

            OkButton.Text = Properties.titles.Generic_Ok;
            AbortButton.Text = Properties.titles.Generic_Abort;
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
            FileDefinition = GetDefinition(Convert.ToString(FormatDefinitionComboBox.SelectedItem));
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
                MessageBox.Show(this, Properties.texts.EcfFilePropertiesDialog_PropertiesIncomplete, 
                    Properties.titles.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        public DialogResult ShowDialogNewFile(IWin32Window parent, ReadOnlyCollection<string> supportedFileTypes)
        {
            FileOperation = CreateFile;
            RefreshFormatDefinitionBox(supportedFileTypes);
            SetAppearanceNewFile();
            DialogResult result = FileOperation(parent);
            if (result != DialogResult.OK) { return result; }
            return ShowDialog(parent);
        }
        public DialogResult ShowDialogOpenFile(IWin32Window parent, ReadOnlyCollection<string> supportedFileTypes)
        {
            FileOperation = FindFile;
            RefreshFormatDefinitionBox(supportedFileTypes);
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

        // privates
        private void InitFilePathAndNameBox(string filePathAndName)
        {
            FilePathAndNameLabel.Text = Properties.titles.EcfFileOpenDialog_FilePathAndNameBox;
            FilePathAndNameTextBox.Text = filePathAndName;
            FilePathAndNameTextBox.Click += FilePathAndNameTextBox_Click;
        }
        private void InitFormatDefinitionBox(ReadOnlyCollection<string> supportedFileTypes)
        {
            FormatDefinitionLabel.Text = Properties.titles.EcfFileOpenDialog_FormatDefinitionBox;
            FormatDefinitionTooltip.SetToolTip(FormatDefinitionComboBox, Properties.texts.ToolTip_EcfFilePropertiesDialog_FormatDefinition);
            RefreshFormatDefinitionBox(supportedFileTypes);
        }
        private void RefreshFormatDefinitionBox(ReadOnlyCollection<string> supportedFileTypes)
        {
            FormatDefinitionComboBox.Items.Clear();
            if (supportedFileTypes != null)
            {
                FormatDefinitionComboBox.Items.AddRange(supportedFileTypes.ToArray());
            }
        }
        private FormatDefinition GetFileDefinition()
        {
            if (TryGetDefinition(FilePathAndName, out FormatDefinition definition))
            {
                return definition;
            }
            else
            {
                return null;
            }
        }
        private void InitEncodingBox()
        {
            EncodingLabel.Text = Properties.titles.EcfFileOpenDialog_EncodingBox;
            EncodingTooltip.SetToolTip(EncodingComboBox, Properties.texts.ToolTip_EcfFilePropertiesDialog_Encoding);
            EncodingComboBox.Items.AddRange(Encoding.GetEncodings().Select(encoding => encoding.Name).ToArray());
            EncodingComboBox.SelectedItem = Encoding.UTF8.WebName;
            FileEncoding = Encoding.UTF8;
        }
        private void InitNewLineSymbolBox()
        {
            NewLineSymbolLabel.Text = Properties.titles.EcfFileOpenDialog_NewLineSymbolBox;
            NewLineSymbolTooltip.SetToolTip(NewLineSymbolComboBox, Properties.texts.ToolTip_EcfFilePropertiesDialog_NewLineSymbol);
            NewLineSymbolComboBox.Items.AddRange(Enum.GetValues(typeof(EcfFileNewLineSymbols)).Cast<EcfFileNewLineSymbols>().Skip(1).Select(value => value.ToString()).ToArray());
            NewLineSymbolComboBox.SelectedItem = EcfFileNewLineSymbols.CrLf.ToString();
            FileNewLineSymbol = EcfFileNewLineSymbols.CrLf;
        }
        private void InitCreateFileDialog(string fileFilter, string fileExtension)
        {
            CreateFileDialog.AddExtension = true;
            CreateFileDialog.DefaultExt = fileExtension;
            CreateFileDialog.Filter = fileFilter;
            CreateFileDialog.Title = Properties.titles.EcfFileDialog_CreateFileDialog;
            CreateFileDialog.ShowHelp = false;
            CreateFileDialog.FileName = Properties.titles.EcfFileDialog_CreateFileName;
        }
        private void InitFindFileDialog(string fileFilter, string fileExtension)
        {
            FindFileDialog.AddExtension = true;
            FindFileDialog.DefaultExt = fileExtension;
            FindFileDialog.Filter = fileFilter;
            FindFileDialog.Title = Properties.titles.EcfFileOpenDialog_FindFileDialog;
            FindFileDialog.Multiselect = false;
            FindFileDialog.ShowHelp = false;
        }
        private void SetAppearanceNewFile()
        {
            EncodingComboBox.Enabled = true;
            NewLineSymbolComboBox.Enabled = true;

            Text = Properties.titles.EcfFileOpenDialog_NewFileDialog;
        }
        private void SetAppearanceOpenFile()
        {
            EncodingComboBox.Enabled = false;
            NewLineSymbolComboBox.Enabled = false;

            Text = Properties.titles.EcfFileOpenDialog_OpenFileDialog;
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
