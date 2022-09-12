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
        public List<EcfFileSetting> Files { get; } = new List<EcfFileSetting>();
        private EcfFileSetting ActualEditedFile { get; set; } = null;

        private List<FormatDefinition> Definitions { get; set; } = new List<FormatDefinition>();

        private SaveFileDialog CreateFileDialog { get; } = new SaveFileDialog();
        private OpenFileDialog FindFileDialog { get; } = new OpenFileDialog();

        public EcfFileOpenDialog()
        {
            InitializeComponent();
            InitForm();
        }

        //events
        private void InitForm()
        {
            Icon = IconRecources.Icon_AppBranding;
            
            InitFilePathAndNameBox("");
            InitFormatDefinitionBox();
            InitEncodingBox();
            InitNewLineSymbolBox();

            InitCreateFileDialog();
            InitFindFileDialog();

            RefreshEncodingBox();
            RefreshNewLineSymbolBox();

            OkButton.Text = TitleRecources.Generic_Ok;
            AbortButton.Text = TitleRecources.Generic_Abort;
        }
        private void EcfFilePropertiesDialog_Activated(object sender, EventArgs evt)
        {
            OkButton.Focus();
        }
        private void FormatDefinitionComboBox_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            ActualEditedFile.SetDefinition(Definitions.FirstOrDefault(definition => definition.FileType.Equals(Convert.ToString(FormatDefinitionComboBox.SelectedItem))));
        }
        private void EncodingComboBox_SelectedIndexChanged(object sender, EventArgs evt)
        {
            ActualEditedFile.SetEncoding(EncodingComboBox.SelectedItem);
        }
        private void NewLineSymbolComboBox_SelectedIndexChanged(object sender, EventArgs evt)
        {
            ActualEditedFile.SetNewLineSymbol(NewLineSymbolComboBox.SelectedItem);
        }
        private void OkButton_Click(object sender, EventArgs evt)
        {
            if (ActualEditedFile.IsValid())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show(this, TextRecources.EcfFilePropertiesDialog_PropertiesIncomplete,
                    TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            Files.Clear();
            Definitions = GetSupportedFileTypes(gameVersion);
            RefreshFormatDefinitionBox();
            SetAppearanceNewFile();
            DialogResult result = CreateFile(parent);
            if (result != DialogResult.OK || Files.Count < 1) { return result; }
            UpdateControls(Files.FirstOrDefault());
            return ShowDialog(parent);
        }
        public DialogResult ShowDialogOpenFile(IWin32Window parent, string gameVersion)
        {
            Files.Clear();
            Definitions = GetSupportedFileTypes(gameVersion);
            RefreshFormatDefinitionBox();
            SetAppearanceOpenFile();
            DialogResult result = FindFiles(parent);
            if (result != DialogResult.OK || Files.Count < 1) { return DialogResult.Abort; }

            if (Files.Count > 1 && Files.All(file => file.IsValid()))
            {
                return DialogResult.OK;
            }

            if (Files.Count == 1)
            {
                UpdateControls(Files.FirstOrDefault());
                return ShowDialog(parent);
            }
            
            foreach (EcfFileSetting setting in Files.Where(file => !file.IsValid()))
            {
                UpdateControls(setting);
                result = ShowDialog(parent);
                if (result != DialogResult.OK) { return DialogResult.Abort; }
            }
            
            return DialogResult.OK;
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
            FindFileDialog.Multiselect = true;
            FindFileDialog.ShowHelp = false;
        }
        private void InitFilePathAndNameBox(string filePathAndName)
        {
            FilePathAndNameLabel.Text = TitleRecources.EcfFileOpenDialog_FilePathAndNameBox;
            FilePathAndNameTextBox.Text = filePathAndName;
        }
        private void InitFormatDefinitionBox()
        {
            FormatDefinitionLabel.Text = TitleRecources.EcfFileOpenDialog_FormatDefinitionBox;
            Tip.SetToolTip(FormatDefinitionComboBox, TextRecources.EcfFileOpenDialog_ToolTip_FormatDefinition);
        }
        private void InitEncodingBox()
        {
            EncodingLabel.Text = TitleRecources.EcfFileOpenDialog_EncodingBox;
            Tip.SetToolTip(EncodingComboBox, TextRecources.EcfFileOpenDialog_ToolTip_Encoding);
        }
        private void InitNewLineSymbolBox()
        {
            NewLineSymbolLabel.Text = TitleRecources.EcfFileOpenDialog_NewLineSymbolBox;
            Tip.SetToolTip(NewLineSymbolComboBox, TextRecources.EcfFileOpenDialog_ToolTip_NewLineSymbol);
        }
        private void RefreshFormatDefinitionBox()
        {
            FormatDefinitionComboBox.BeginUpdate();
            FormatDefinitionComboBox.Items.Clear();
            FormatDefinitionComboBox.Items.AddRange(Definitions.Select(definition => definition.FileType).ToArray());
            FormatDefinitionComboBox.EndUpdate();
        }
        private void RefreshEncodingBox()
        {
            EncodingComboBox.BeginUpdate();
            EncodingComboBox.Items.Clear();
            EncodingComboBox.Items.AddRange(Encoding.GetEncodings().Select(encoding => encoding.Name).ToArray());
            EncodingComboBox.EndUpdate();
        }
        private void RefreshNewLineSymbolBox()
        {
            NewLineSymbolComboBox.BeginUpdate();
            NewLineSymbolComboBox.Items.Clear();
            NewLineSymbolComboBox.Items.AddRange(Enum.GetValues(typeof(EcfFileNewLineSymbols)).Cast<EcfFileNewLineSymbols>().Skip(1).Select(value => value.ToString()).ToArray());
            NewLineSymbolComboBox.EndUpdate();
        }
        private void UpdateControls(EcfFileSetting setting)
        {
            ActualEditedFile = setting;
            FilePathAndNameTextBox.Text = setting.PathAndName;
            FormatDefinitionComboBox.SelectedItem = setting.Definition?.FileType;
            EncodingComboBox.SelectedItem = setting.Encoding?.WebName ?? Encoding.UTF8.WebName;
            NewLineSymbolComboBox.SelectedItem = setting.NewLineSymbol.ToString();
        }
        private FormatDefinition GetFileDefinition(string filePathAndName)
        {
            return Definitions.FirstOrDefault(definition => filePathAndName.Contains(definition.FileType));
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

            string fileName = CreateFileDialog.FileName;
            Files.Add(new EcfFileSetting(fileName, GetFileDefinition(fileName)));

            return result;
        }
        private DialogResult FindFiles(IWin32Window parent)
        {
            DialogResult result = FindFileDialog.ShowDialog(parent);
            if (result != DialogResult.OK) { return result; }

            Files.AddRange(FindFileDialog.FileNames.Select(fileName => 
                new EcfFileSetting(fileName, GetFileDefinition(fileName), GetFileEncoding(fileName), GetNewLineSymbol(fileName))));

            return result;
        }

        public class EcfFileSetting
        {
            public string PathAndName { get; } = null;
            public FormatDefinition Definition { get; private set; } = null;
            public Encoding Encoding { get; private set; } = Encoding.UTF8;
            public EcfFileNewLineSymbols NewLineSymbol { get; private set; } = EcfFileNewLineSymbols.CrLf;

            public EcfFileSetting(string pathAndName, FormatDefinition definition, Encoding encoding, EcfFileNewLineSymbols newLineSymbol)
            {
                PathAndName = pathAndName;
                Definition = definition;
                Encoding = encoding;
                NewLineSymbol = newLineSymbol;
            }
            public EcfFileSetting(string pathAndName, FormatDefinition definition)
            {
                PathAndName = pathAndName;
                Definition = definition;
            }

            // publics
            public void SetDefinition(FormatDefinition definition)
            {
                Definition = definition;
            }
            public void SetEncoding(object encoding)
            {
                Encoding = Encoding.GetEncoding(Convert.ToString(encoding));
            }
            public void SetNewLineSymbol(object symbol)
            {
                NewLineSymbol = (EcfFileNewLineSymbols)Enum.Parse(typeof(EcfFileNewLineSymbols), Convert.ToString(symbol));
            }
            public bool IsValid()
            {
                return !string.IsNullOrEmpty(PathAndName) && Definition != null && Encoding != null && NewLineSymbol != EcfFileNewLineSymbols.Unknown;
            }
        }
    }
}
