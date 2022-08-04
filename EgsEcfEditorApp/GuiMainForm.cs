using EgsEcfParser;
using Microsoft.Win32;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using EcfFileViews;
using System.Collections.Generic;
using static EcfFileViews.EcfTabPage;
using static EcfFileViewTools.EcfSorter;
using static EcfFileViews.EcfTabPage.CopyPasteClickedEventArgs;
using EgsEcfEditorApp.Properties;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Forms.VisualStyles;
using static EcfFileViews.EcfItemEditingDialog;
using static EcfToolBarControls.EcfToolBarCheckComboBox;
using static EcfFileViewTools.EcfTreeFilter;
using System.ComponentModel;
using static Helpers.EnumLocalisation;
using static Helpers.FileHandling;
using EcfToolBarControls;
using EcfFileViewTools;
using static EcfFileViews.EcfBaseView;
using static EgsEcfParser.EcfDefinitionHandling;

namespace EgsEcfEditorApp
{
    public partial class GuiMainForm : Form
    {
        private List<EcfStructureItem> CopyClipboard { get; set; } = null;

        private EcfToolContainer FileOperationContainer { get; } = new EcfToolContainer();
        private EcfBasicFileOperations BasicFileOperations { get; } = new EcfBasicFileOperations();
        private EcfExtendedFileOperations ExtendedFileOperations { get; } = new EcfExtendedFileOperations();

        private EcfToolContainer SettingsOperationContainer { get; } = new EcfToolContainer();
        private EcfSettingOperations SettingOperations { get; } = new EcfSettingOperations();

        private TableLayoutPanel AppControlsPanel { get; } = new TableLayoutPanel();
        private EcfTabContainer FileViewPanel { get; } = new EcfTabContainer();
        private EcfFileOpenDialog OpenDialog { get; } = new EcfFileOpenDialog();
        private EcfFileSaveDialog SaveDialog { get; } = new EcfFileSaveDialog();
        private DeprecatedDefinitionsDialog DeprecatedDefinitions { get; } = new DeprecatedDefinitionsDialog();
        private EcfFileLoaderDialog FileLoader { get; } = new EcfFileLoaderDialog();
        private EcfSettingsDialog SettingsDialog { get; } = new EcfSettingsDialog();

        public GuiMainForm()
        {
            InitializeComponent();
            InitForm();
        }

        // Events
        private void InitForm()
        {
            // MainForm settings
            Text = string.Format("{0} - {1}", TitleRecources.EgsEcfEditorApp_AppName, Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Icon = IconRecources.Icon_App;
            
            RestoreWindowSettings();
            RestoreFilterSettings();
            RestoreDefinitionSettings();

            InitGuiControls();
            InitEvents();
        }
        private void GuiMainForm_FormClosing(object sender, FormClosingEventArgs evt)
        {
            evt.Cancel = AppClosing();
        }
        private void FileViewPanel_TreeViewResized(object sender, EventArgs evt)
        {
            if (sender is EcfTreeView treeView)
            {
                WindowSettings.Default.EgsEcfControls_TreeViewInitWidth = treeView.Width;
            }
        }
        private void FileViewPanel_InfoViewResized(object sender, EventArgs evt)
        {
            if (sender is EcfInfoView infoView)
            {
                WindowSettings.Default.EgsEcfControls_InfoViewInitWidth = infoView.Width;
            }
        }
        private void FileViewPanel_ErrorViewResized(object sender, EventArgs evt)
        {
            if (sender is EcfErrorView errorView)
            {
                WindowSettings.Default.EgsEcfControls_ErrorViewInitHeight = errorView.Height;
            }
        }
        private void FileViewPanel_CopyClicked(object sender, CopyPasteClickedEventArgs evt)
        {
            CopyElements(sender, evt);
        }
        private void FileViewPanel_PasteClicked(object sender, CopyPasteClickedEventArgs evt)
        {
            PasteElements(sender, evt);
        }
        private void BasicFileOperations_NewFileClicked(object sender, EventArgs evt)
        {
            NewEcfFile();
        }
        private void BasicFileOperations_OpenFileClicked(object sender, EventArgs evt)
        {
            OpenEcfFile();
        }
        private void BasicFileOperations_ReloadFileClicked(object sender, EventArgs evt)
        {
            ReloadEcfFile();
        }
        private void BasicFileOperations_SaveFileClicked(object sender, EventArgs evt)
        {
            SaveEcfFile();
        }
        private void BasicFileOperations_SaveAsFileClicked(object sender, EventArgs evt)
        {
            SaveAsEcfFile();
        }
        private void BasicFileOperations_SaveAsFilteredFileClicked(object sender, EventArgs evt)
        {
            SaveAsFilteredEcfFile();
        }
        private void BasicFileOperations_SaveAllFilesClicked(object sender, EventArgs evt)
        {
            SaveAllEcfFiles();
        }
        private void BasicFileOperations_CloseFileClicked(object sender, EventArgs evt)
        {
            CloseEcfFile();
        }
        private void BasicFileOperations_CloseAllFilesClicked(object sender, EventArgs evt)
        {
            CloseAllEcfFiles();
        }
        private void ExtendedFileOperations_ReloadDefinitionsClicked(object sender, EventArgs evt)
        {
            ReloadDefinitions();
        }
        private void ExtendedFileOperations_CheckDefinitionClicked(object sender, EventArgs evt)
        {
            CheckDefinition();
        }
        private void ExtendedFileOperations_CompareFilesClicked(object sender, EventArgs evt)
        {
            CompareFiles();
        }
        private void ExtendedFileOperations_MergeFilesClicked(object sender, EventArgs evt)
        {
            MergeFiles();
        }
        private void ExtendedFileOperations_BuildTechTreePreviewClicked(object sender, EventArgs evt)
        {
            BuildTechTreePreview();
        }
        private void SettingOperations_GameVersionClicked(object sender, EventArgs evt)
        {
            UserSettings.Default.EgsEcfEditorApp_ActiveGameVersion = Convert.ToString(sender);
        }
        private void SettingOperations_OpenSettingsDialogClicked(object sender, EventArgs evt)
        {
            ChangeSettings();
        }

        // Settings
        private void ChangeSettings()
        {
            SettingsDialog.ShowDialog(this);
            RestoreDefinitionSettings();
            RestoreFilterSettings();
        }
        private void RestoreWindowSettings()
        {
            Rectangle screen = Screen.FromControl(this).Bounds;

            Rectangle window = new Rectangle(
                WindowSettings.Default.EgsEcfEditorApp_X,
                WindowSettings.Default.EgsEcfEditorApp_Y,
                WindowSettings.Default.EgsEcfEditorApp_Width,
                WindowSettings.Default.EgsEcfEditorApp_Height);

            if (screen.Contains(window))
            {
                Location = new Point(window.X, window.Y);
                Size = new Size(window.Width, window.Height);
            }
            else
            {
                WindowSettings.Default.Reset();
                Location = new Point(
                    WindowSettings.Default.EgsEcfEditorApp_X,
                    WindowSettings.Default.EgsEcfEditorApp_Y);
                Size = new Size(
                    WindowSettings.Default.EgsEcfEditorApp_Width,
                    WindowSettings.Default.EgsEcfEditorApp_Height);
            }

            FileViewPanel.TreeViewInitWidth = WindowSettings.Default.EgsEcfControls_TreeViewInitWidth;
            FileViewPanel.InfoViewInitWidth = WindowSettings.Default.EgsEcfControls_InfoViewInitWidth;
            FileViewPanel.ErrorViewInitHeight = WindowSettings.Default.EgsEcfControls_ErrorViewInitHeight;
        }
        private void StoreWindowSettings()
        {
            WindowSettings.Default.EgsEcfEditorApp_Width = Width;
            WindowSettings.Default.EgsEcfEditorApp_Height = Height;
            WindowSettings.Default.EgsEcfEditorApp_X = Location.X;
            WindowSettings.Default.EgsEcfEditorApp_Y = Location.Y;
        }
        private void RestoreFilterSettings()
        {
            FileViewPanel.TreeViewSorterInitItemCount = (VisibleItemCount)Enum.Parse(typeof(VisibleItemCount), 
                UserSettings.Default.EgsEcfControls_TreeViewSorterInitCount.ToString());
            FileViewPanel.ParameterViewSorterInitItemCount = (VisibleItemCount)Enum.Parse(typeof(VisibleItemCount), 
                UserSettings.Default.EgsEcfControls_ParameterViewSorterInitCount.ToString());
            FileViewPanel.ErrorViewSorterInitItemCount = (VisibleItemCount)Enum.Parse(typeof(VisibleItemCount), 
                UserSettings.Default.EgsEcfControls_ErrorViewSorterInitCount.ToString());

            FileViewPanel.TreeViewFilterCommentInitActive = UserSettings.Default.EgsEcfControls_TreeViewFilterCommentsInitActive;
            FileViewPanel.TreeViewFilterParameterInitActive = UserSettings.Default.EgsEcfControls_TreeViewFilterParametersInitActive;
            FileViewPanel.TreeViewFilterDataBlocksInitActive = UserSettings.Default.EgsEcfControls_TreeViewFilterDataBlocksInitActive;
        }
        private void RestoreDefinitionSettings()
        {
            DefaultBaseFolder = InternalSettings.Default.EgsEcfEditorApp_DefinitionDefaultBaseFolder;
            TemplateFileName = InternalSettings.Default.EgsEcfEditorApp_DefinitionTemplateFileName;
            try
            {
                List<string> gameVersions = GetGameModes();
                if (!gameVersions.Contains(UserSettings.Default.EgsEcfEditorApp_ActiveGameVersion))
                {
                    UserSettings.Default.EgsEcfEditorApp_ActiveGameVersion = gameVersions.FirstOrDefault();
                }
                SettingOperations.SetGameVersion(UserSettings.Default.EgsEcfEditorApp_ActiveGameVersion);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("{0}{1}{1}{2}", TextRecources.EgsEcfEditorApp_ReloadFileDefinitionsFailed, Environment.NewLine, ex.Message),
                    TitleRecources.Generic_Warning, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        // App Handling
        private void InitGuiControls()
        {
            AppControlsPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            AppControlsPanel.AutoSize = true;
            AppControlsPanel.Dock = DockStyle.Top;
            AppControlsPanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;

            AppControlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0.8f));
            AppControlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0.2f));
            AppControlsPanel.ColumnCount = 2;
            AppControlsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 1.0f));
            AppControlsPanel.RowCount = 1;

            FileOperationContainer.Dock = DockStyle.Fill;

            SettingsOperationContainer.Dock = DockStyle.Fill;
            SettingsOperationContainer.FlowDirection = FlowDirection.RightToLeft;

            FileViewPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            FileViewPanel.Dock = DockStyle.Fill;

            FileOperationContainer.Add(BasicFileOperations);
            FileOperationContainer.Add(ExtendedFileOperations);

            SettingsOperationContainer.Add(SettingOperations);

            AppControlsPanel.Controls.Add(FileOperationContainer, 0, 0);
            AppControlsPanel.Controls.Add(SettingsOperationContainer, 1, 0);
            
            Controls.Add(FileViewPanel);
            Controls.Add(AppControlsPanel);
        }
        private void InitEvents()
        {
            BasicFileOperations.NewFileClicked += BasicFileOperations_NewFileClicked;
            BasicFileOperations.OpenFileClicked += BasicFileOperations_OpenFileClicked;
            BasicFileOperations.ReloadFileClicked += BasicFileOperations_ReloadFileClicked;
            BasicFileOperations.SaveFileClicked += BasicFileOperations_SaveFileClicked;
            BasicFileOperations.SaveAsFileClicked += BasicFileOperations_SaveAsFileClicked;
            BasicFileOperations.SaveAsFilteredFileClicked += BasicFileOperations_SaveAsFilteredFileClicked;
            BasicFileOperations.SaveAllFilesClicked += BasicFileOperations_SaveAllFilesClicked;
            BasicFileOperations.CloseFileClicked += BasicFileOperations_CloseFileClicked;
            BasicFileOperations.CloseAllFilesClicked += BasicFileOperations_CloseAllFilesClicked;

            ExtendedFileOperations.ReloadDefinitionsClicked += ExtendedFileOperations_ReloadDefinitionsClicked;
            ExtendedFileOperations.CheckDefinitionClicked += ExtendedFileOperations_CheckDefinitionClicked;
            ExtendedFileOperations.CompareFilesClicked += ExtendedFileOperations_CompareFilesClicked;
            ExtendedFileOperations.MergeFilesClicked += ExtendedFileOperations_MergeFilesClicked;
            ExtendedFileOperations.BuildTechTreePreviewClicked += ExtendedFileOperations_BuildTechTreePreviewClicked;

            SettingOperations.GameVersionClicked += SettingOperations_GameVersionClicked;
            SettingOperations.OpenSettingsDialogClicked += SettingOperations_OpenSettingsDialogClicked;

            FileViewPanel.TreeViewResized += FileViewPanel_TreeViewResized;
            FileViewPanel.InfoViewResized += FileViewPanel_InfoViewResized;
            FileViewPanel.ErrorViewResized += FileViewPanel_ErrorViewResized;
            FileViewPanel.CopyClicked += FileViewPanel_CopyClicked;
            FileViewPanel.PasteClicked += FileViewPanel_PasteClicked;
        }
        private bool AppClosing()
        {
            bool cancelClosing = false;
            if (FileViewPanel.TabPages.Cast<EcfTabPage>().Any(tab => tab.File.HasUnsavedData))
            {
                cancelClosing = MessageBox.Show(this, TextRecources.EgsEcfEditorApp_CloseAppWithUnsaved, TitleRecources.Generic_Attention, 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes;
            }
            if (!cancelClosing)
            {
                StoreWindowSettings();
                WindowSettings.Default.Save();
                AppSettings.Default.Save();
            }
            return cancelClosing;
        }
        private bool TryGetSelectedTab(out EcfTabPage ecfTab)
        {
            ecfTab = null;
            if (FileViewPanel.SelectedTab is EcfTabPage ecfPage)
            {
                ecfTab = ecfPage;
                return true;
            }
            MessageBox.Show(this, TextRecources.EgsEcfEditorApp_NoTabSelected, TitleRecources.Generic_Warning, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
        }

        // FileHandling
        private void NewEcfFile()
        {
            try
            {
                OpenDialog.SetInitDirectory(FindFileDialogInitDirectory());
                if (OpenDialog.ShowDialogNewFile(this, UserSettings.Default.EgsEcfEditorApp_ActiveGameVersion) == DialogResult.OK)
                {
                    AppSettings.Default.EgsEcfEditorApp_LastVisitedDirectory = Path.GetDirectoryName(OpenDialog.FilePathAndName);

                    EgsEcfFile file = new EgsEcfFile(OpenDialog.FilePathAndName, OpenDialog.FileDefinition, OpenDialog.FileEncoding, OpenDialog.FileNewLineSymbol);

                    FileViewPanel.SelectedTab = FileViewPanel.Add(file);
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex, TextRecources.EgsEcfEditorApp_CreateEcfFileFailed);
            }
        }
        private void OpenEcfFile()
        {
            try
            {
                OpenDialog.SetInitDirectory(FindFileDialogInitDirectory());
                if (OpenDialog.ShowDialogOpenFile(this, UserSettings.Default.EgsEcfEditorApp_ActiveGameVersion) != DialogResult.OK) { 
                    return; 
                }
                AppSettings.Default.EgsEcfEditorApp_LastVisitedDirectory = Path.GetDirectoryName(OpenDialog.FilePathAndName);

                EgsEcfFile file = new EgsEcfFile(OpenDialog.FilePathAndName, OpenDialog.FileDefinition);
                if (FileLoader.ShowDialog(this, file) != DialogResult.OK) { return; }

                FileViewPanel.SelectedTab = FileViewPanel.Add(file);
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex, TextRecources.EgsEcfEditorApp_OpenEcfFileFailed);
            }
        }
        private void ReloadEcfFile()
        {
            if (TryGetSelectedTab(out EcfTabPage tab))
            {
                bool cancelOverride = false;
                if (tab.File.HasUnsavedData)
                {
                    cancelOverride = MessageBox.Show(this, TextRecources.EgsEcfEditorApp_OverrideTabWithUnsaved, TitleRecources.Generic_Attention,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes;
                }
                if (!cancelOverride)
                {
                    try
                    {
                        if (FileLoader.ShowDialog(this, tab.File) != DialogResult.OK) { return; }

                        tab.UpdateTabDescription();
                        tab.UpdateAllViews();
                    }
                    catch (Exception ex)
                    {
                        ShowExceptionMessage(ex, TextRecources.EgsEcfEditorApp_ReloadEcfFileFailed);
                    }
                }
            }
        }
        private void SaveEcfFile()
        {
            if (TryGetSelectedTab(out EcfTabPage ecf) && ecf.File.HasUnsavedData)
            {
                try
                {
                    ecf.File.Save(
                        UserSettings.Default.EgsEcfEditorApp_WriteOnlyValidItems, 
                        UserSettings.Default.EgsEcfEditorApp_InvalidateParentsOnError);
                    ecf.UpdateTabDescription();
                    ecf.UpdateErrorView();
                }
                catch (Exception ex)
                {
                    ShowExceptionMessage(ex, TextRecources.EgsEcfEditorApp_SaveEcfFileFailed);
                }
            }
        }
        private void SaveAsEcfFile()
        {
            if (TryGetSelectedTab(out EcfTabPage ecf))
            {
                SaveDialog.SetInitDirectory(FindFileDialogInitDirectory());
                SaveDialog.SetInitFileName(ecf.File.FileName);
                if (SaveDialog.ShowDialogSaveAs(this) == DialogResult.OK)
                {
                    AppSettings.Default.EgsEcfEditorApp_LastVisitedDirectory = Path.GetDirectoryName(SaveDialog.FilePathAndName);
                    try
                    {
                        ecf.File.Save(SaveDialog.FilePathAndName, 
                            UserSettings.Default.EgsEcfEditorApp_WriteOnlyValidItems, 
                            UserSettings.Default.EgsEcfEditorApp_InvalidateParentsOnError);
                        ecf.UpdateTabDescription();
                        ecf.UpdateErrorView();
                    }
                    catch (Exception ex)
                    {
                        ShowExceptionMessage(ex, TextRecources.EgsEcfEditorApp_SaveEcfFileFailed);
                    }
                }
            }
        }
        private void SaveAsFilteredEcfFile()
        {
            MessageBox.Show(this, "not implemented yet! :)");
        }
        private void SaveAllEcfFiles()
        {
            try
            {
                foreach (TabPage tab in FileViewPanel.TabPages)
                {
                    if (tab is EcfTabPage ecf && ecf.File.HasUnsavedData)
                    {
                        ecf.File.Save(
                            UserSettings.Default.EgsEcfEditorApp_WriteOnlyValidItems, 
                            UserSettings.Default.EgsEcfEditorApp_InvalidateParentsOnError);
                        ecf.UpdateTabDescription();
                        ecf.UpdateErrorView();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex, TextRecources.EgsEcfEditorApp_SaveEcfFileFailed);
            }
        }
        private void CloseEcfFile()
        {
            if (TryGetSelectedTab(out EcfTabPage tab))
            {
                bool cancelClosing = false;
                if (tab.File.HasUnsavedData)
                {
                    cancelClosing = MessageBox.Show(this, TextRecources.EgsEcfEditorApp_CloseTabWithUnsaved, TitleRecources.Generic_Attention,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes;
                }
                if (!cancelClosing)
                {
                    FileViewPanel.TabPages.Remove(tab);
                }
            }
        }
        private void CloseAllEcfFiles()
        {
            bool cancelClosing = false;
            if (FileViewPanel.TabPages.Cast<EcfTabPage>().Any(tab => tab.File.HasUnsavedData))
            {
                cancelClosing = MessageBox.Show(this, TextRecources.EgsEcfEditorApp_CloseAllTabsWithUnsaved, TitleRecources.Generic_Attention,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes;
            }
            if (!cancelClosing)
            {
                FileViewPanel.TabPages.Clear();
            }
        }
        private void ShowExceptionMessage(Exception ex, string header)
        {
            MessageBox.Show(this, string.Format("{0}{1}{1}{2}", header, Environment.NewLine, ex.Message),
                TitleRecources.Generic_Warning, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        // definition handling
        private void ReloadDefinitions()
        {
            try
            {
                EcfDefinitionHandling.ReloadDefinitions();
                MessageBox.Show(this, TextRecources.EgsEcfEditorApp_ReloadFileDefinitionsSuccess, 
                    TitleRecources.Generic_Info, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("{0}{1}{1}{2}", TextRecources.EgsEcfEditorApp_ReloadFileDefinitionsFailed, Environment.NewLine, ex.Message), 
                    TitleRecources.Generic_Warning, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void CheckDefinition()
        {
            if (TryGetSelectedTab(out EcfTabPage tab))
            {
                DeprecatedDefinitions.ShowDialog(this, tab.File);
            }
        }
        
        // Content handling
        private void CompareFiles()
        {
            MessageBox.Show(this, "not implemented yet! :)");
        }
        private void MergeFiles()
        {
            MessageBox.Show(this, "not implemented yet! :)");
        }
        private void CopyElements(object sender, CopyPasteClickedEventArgs evt)
        {
            CopyClipboard = evt.CopiedItems;
        }
        private void PasteElements(object sender, CopyPasteClickedEventArgs evt)
        {
            switch (evt.Mode)
            {
                case CopyPasteModes.PasteTo:
                    evt.Source.PasteTo(evt.SelectedItems.FirstOrDefault(), CopyClipboard);
                    break;
                case CopyPasteModes.PasteAfter:
                    evt.Source.PasteAfter(evt.SelectedItems.LastOrDefault(), CopyClipboard);
                    break;
                default:
                    break;
            }
        }
        private void BuildTechTreePreview()
        {
            MessageBox.Show(this, "not implemented yet! :)");
        }
    }
}

// main gui display views
namespace EcfFileViews
{
    public class EcfTabContainer : TabControl
    {
        public event EventHandler TreeViewResized;
        public event EventHandler InfoViewResized;
        public event EventHandler ErrorViewResized;
        public event EventHandler<CopyPasteClickedEventArgs> CopyClicked;
        public event EventHandler<CopyPasteClickedEventArgs> PasteClicked;

        public int TreeViewInitWidth { get; set; } = 100;
        public int InfoViewInitWidth { get; set; } = 100;
        public int ErrorViewInitHeight { get; set; } = 100;

        public VisibleItemCount TreeViewSorterInitItemCount { get; set; } = VisibleItemCount.TwentyFive;
        public VisibleItemCount ParameterViewSorterInitItemCount { get; set; } = VisibleItemCount.OneHundred;
        public VisibleItemCount ErrorViewSorterInitItemCount { get; set; } = VisibleItemCount.Ten;

        public bool TreeViewFilterCommentInitActive { get; set; } = true;
        public bool TreeViewFilterParameterInitActive { get; set; } = true;
        public bool TreeViewFilterDataBlocksInitActive { get; set; } = true;

        public EcfTabContainer() : base()
        {
            ShowToolTips = true;
        }

        // events
        private void TabPage_TreeViewResized(object sender, EventArgs evt)
        {
            if (sender is EcfTreeView treeView)
            {
                TreeViewInitWidth = treeView.Width;
            }
            TreeViewResized?.Invoke(sender, evt);
        }
        private void TabPage_InfoViewResized(object sender, EventArgs evt)
        {
            if (sender is EcfInfoView infoView)
            {
                InfoViewInitWidth = infoView.Width;
            }
            InfoViewResized?.Invoke(sender, evt);
        }
        private void TabPage_ErrorViewResized(object sender, EventArgs evt)
        {
            if (sender is EcfErrorView errorView)
            {
                ErrorViewInitHeight = errorView.Height;
            }
            ErrorViewResized?.Invoke(sender, evt);
        }
        private void TabPage_CopyClicked(object sender, CopyPasteClickedEventArgs evt)
        {
            CopyClicked?.Invoke(sender, evt);
        }
        private void TabPage_PasteClicked(object sender, CopyPasteClickedEventArgs evt)
        {
            PasteClicked?.Invoke(sender, evt);
        }

        // publics 
        public EcfTabPage Add(EgsEcfFile ecfFile)
        {
            EcfTabPage tabPage = new EcfTabPage(ecfFile, this);

            tabPage.TreeViewResized += TabPage_TreeViewResized;
            tabPage.InfoViewResized += TabPage_InfoViewResized;
            tabPage.ErrorViewResized += TabPage_ErrorViewResized;
            tabPage.CopyClicked += TabPage_CopyClicked;
            tabPage.PasteClicked += TabPage_PasteClicked;

            TabPages.Add(tabPage);
            return tabPage;
        }
    }
    public class EcfTabPage : TabPage
    {
        public event EventHandler TreeViewResized;
        public event EventHandler InfoViewResized;
        public event EventHandler ErrorViewResized;
        public event EventHandler<CopyPasteClickedEventArgs> CopyClicked;
        public event EventHandler<CopyPasteClickedEventArgs> PasteClicked;

        public EgsEcfFile File { get; }
        private EcfItemEditingDialog ItemEditor { get; }

        private EcfToolContainer ToolContainer { get; } = new EcfToolContainer();
        private EcfFilterControl FilterControl { get; }
        private EcfTreeFilter TreeFilter { get; }
        private EcfParameterFilter ParameterFilter { get; }
        private EcfContentOperations ContentOperations { get; }

        private EcfFileContainer FileViewPanel { get; } = new EcfFileContainer();
        private EcfTreeView TreeView { get; }
        private EcfParameterView ParameterView { get; }
        private EcfErrorView ErrorView { get; }
        private EcfInfoView InfoView { get; }

        public bool IsUpdating { get; private set; } = false;
        private EcfBaseView LastFocusedView { get; set; } = null;

        public EcfTabPage(EgsEcfFile file, EcfTabContainer container) : base()
        {
            File = file;
            ItemEditor = new EcfItemEditingDialog(file);

            FilterControl = new EcfFilterControl(File.Definition.GameMode, File.Definition.FileType);
            TreeFilter = new EcfTreeFilter(container.TreeViewFilterCommentInitActive, container.TreeViewFilterParameterInitActive, container.TreeViewFilterDataBlocksInitActive);
            ParameterFilter = new EcfParameterFilter(File.Definition.BlockParameters.Select(item => item.Name).ToList());
            ContentOperations = new EcfContentOperations();

            FileViewPanel = new EcfFileContainer();
            TreeView = new EcfTreeView(TitleRecources.EcfTreeView_Header, File, ResizeableBorders.RightBorder, container.TreeViewSorterInitItemCount);
            ParameterView = new EcfParameterView(TitleRecources.EcfParameterView_Header, File, ResizeableBorders.None, container.ParameterViewSorterInitItemCount);
            InfoView = new EcfInfoView(TitleRecources.EcfInfoView_Header, File, ResizeableBorders.LeftBorder);
            ErrorView = new EcfErrorView(TitleRecources.EcfErrorView_Header, File, ResizeableBorders.TopBorder, container.ErrorViewSorterInitItemCount);

            TreeView.Width = container.TreeViewInitWidth;
            InfoView.Width = container.InfoViewInitWidth;
            ErrorView.Height = container.ErrorViewInitHeight;

            AddControls();
            SetEventHandlers();
            UpdateAllViews();
        }

        // events
        private void TreeView_ItemsSelected(object sender, EventArgs evt)
        {
            LastFocusedView = TreeView;
            UpdateParameterView();
        }
        private void TreeView_DisplayedDataChanged(object sender, EventArgs evt)
        {
            ReselectTreeView();
            UpdateParameterView();
        }
        private void ParameterView_ParametersSelected(object sender, EventArgs evt)
        {
            LastFocusedView = ParameterView;
            UpdateInfoView();
        }
        private void ParameterView_DisplayedDataChanged(object sender, EventArgs evt)
        {
            ReselectParameterView();
            UpdateInfoView();
        }
        private void ContentOperations_UndoClicked(object sender, EventArgs e)
        {
            MessageBox.Show(this, "not implemented yet! :)");
        }
        private void ContentOperations_RedoClicked(object sender, EventArgs e)
        {
            MessageBox.Show(this, "not implemented yet! :)");
        }
        private void ContentOperations_AddClicked(object sender, EventArgs evt)
        {
            if (LastFocusedView is EcfTreeView treeView)
            {
                if (treeView.SelectedItems.LastOrDefault() is EcfStructureItem preceedingItem)
                {
                    AddTreeItemTo(preceedingItem);
                    return;
                }
            }
            else if (LastFocusedView is EcfParameterView parameterView)
            {
                if (parameterView.SelectedParameters.LastOrDefault()?.Parent is EcfBlock parentBlock)
                {
                    AddParameterItem(parentBlock, null);
                    return;
                }
            }
            AddTreeItemTo(null);
        }
        private void ContentOperations_RemoveClicked(object sender, EventArgs evt)
        {
            if (LastFocusedView is EcfTreeView treeView)
            {
                List<EcfStructureItem> items = treeView.SelectedItems;
                if (items.Count > 0)
                {
                    RemoveTreeItem(items);
                    return;
                }
            }
            else if (LastFocusedView is EcfParameterView parameterView)
            {
                List<EcfParameter> parameters = parameterView.SelectedParameters;
                if (parameters.Count > 0)
                {
                    RemoveParameterItem(parameters);
                    return;
                }
            }
            MessageBox.Show(this, TextRecources.Generic_NoSuitableSelection, TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        private void ContentOperations_ChangeSingleClicked(object sender, EventArgs evt)
        {
            if (LastFocusedView is EcfTreeView treeView)
            {
                EcfStructureItem item = treeView.SelectedItems.FirstOrDefault();
                if (ChangeTreeItem(item)) { return; }
            }
            else if (LastFocusedView is EcfParameterView parameterView)
            {
                if (parameterView.SelectedParameters.FirstOrDefault() is EcfParameter parameter)
                {
                    ChangeParameterItem(parameter);
                    return;
                }
            }
            MessageBox.Show(this, TextRecources.Generic_NoSuitableSelection, TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        private void ContentOperations_ChangeMultiClicked(object sender, EventArgs evt)
        {
            MessageBox.Show(this, "not implemented yet! :)");
        }
        private void ContentOperations_MoveUpClicked(object sender, EventArgs evt)
        {
            MessageBox.Show(this, "not implemented yet! :)");
        }
        private void ContentOperations_MoveDownClicked(object sender, EventArgs evt)
        {
            MessageBox.Show(this, "not implemented yet! :)");
        }
        private void ContentOperations_CopyClicked(object sender, EventArgs evt)
        {
            CopyItems();
        }
        private void ContentOperations_PasteClicked(object sender, EventArgs evt)
        {
            PasteItems();
        }
        private void AnyFilterControl_ApplyFilterFired(object sender, EventArgs evt)
        {
            UpdateAllViews();
        }
        private void FilterControl_ClearFilterClicked(object sender, EventArgs evt)
        {
            UpdateAllViews();
        }
        private void TreeView_NodeDoubleClicked(object sender, TreeNodeMouseClickEventArgs evt)
        {
            ChangeTreeItem(TreeView.SelectedItems.FirstOrDefault());
        }
        private void TreeView_ChangeItemClicked(object sender, EventArgs evt)
        {
            ChangeTreeItem(TreeView.SelectedItems.FirstOrDefault());
        }
        private void TreeView_AddToItemClicked(object sender, EventArgs evt)
        {
            AddTreeItemTo(TreeView.SelectedItems.FirstOrDefault());
        }
        private void TreeView_AddAfterItemClicked(object sender, EventArgs evt)
        {
            AddTreeItemAfter(TreeView.SelectedItems.FirstOrDefault());
        }
        private void TreeView_CopyItemClicked(object sender, EventArgs evt)
        {
            CopyClicked?.Invoke(this, new CopyPasteClickedEventArgs(CopyPasteModes.Copy, this, TreeView.SelectedItems));
        }
        private void TreeView_PasteToItemClicked(object sender, EventArgs evt)
        {
            PasteClicked?.Invoke(this, new CopyPasteClickedEventArgs(CopyPasteModes.PasteTo, this, TreeView.SelectedItems));
        }
        private void TreeView_PasteAfterItemClicked(object sender, EventArgs evt)
        {
            PasteClicked?.Invoke(this, new CopyPasteClickedEventArgs(CopyPasteModes.PasteAfter, this, TreeView.SelectedItems));
        }
        private void TreeView_RemoveItemClicked(object sender, EventArgs evt)
        {
            RemoveTreeItem(TreeView.SelectedItems);
        }
        private void TreeView_DelKeyPressed(object sender, EventArgs evt)
        {
            RemoveTreeItem(TreeView.SelectedItems);
        }
        private void TreeView_CopyKeyPressed(object sender, EventArgs evt)
        {
            CopyItems();
        }
        private void TreeView_PasteKeyPressed(object sender, EventArgs evt)
        {
            PasteItems();
        }
        private void ParameterView_CellDoubleClicked(object sender, DataGridViewCellEventArgs evt)
        {
            ChangeParameterItem(ParameterView.SelectedParameters.FirstOrDefault());
        }
        private void ParameterView_ChangeItemClicked(object sender, EventArgs evt)
        {
            ChangeParameterItem(ParameterView.SelectedParameters.FirstOrDefault());
        }
        private void ParameterView_AddAfterItemClicked(object sender, EventArgs evt)
        {
            EcfParameter preceedingParameter = ParameterView.SelectedParameters.LastOrDefault();
            EcfBlock parentBlock = preceedingParameter?.Parent as EcfBlock;
            AddParameterItem(parentBlock, preceedingParameter);
        }
        private void ParameterView_CopyItemClicked(object sender, EventArgs evt)
        {
            CopyClicked?.Invoke(this, new CopyPasteClickedEventArgs(CopyPasteModes.Copy, this, ParameterView.SelectedParameters));
        }
        private void ParameterView_PasteAfterItemClicked(object sender, EventArgs evt)
        {
            PasteClicked?.Invoke(this, new CopyPasteClickedEventArgs(CopyPasteModes.PasteAfter, this, ParameterView.SelectedParameters));
        }
        private void ParameterView_RemoveItemClicked(object sender, EventArgs evt)
        {
            RemoveParameterItem(ParameterView.SelectedParameters);
        }
        private void ParameterView_DelKeyPressed(object sender, EventArgs evt)
        {
            RemoveParameterItem(ParameterView.SelectedParameters);
        }
        private void ParameterView_CopyKeyPressed(object sender, EventArgs evt)
        {
            CopyItems();
        }
        private void ParameterView_PasteKeyPressed(object sender, EventArgs evt)
        {
            PasteItems();
        }
        private void ErrorView_ShowInEditorClicked(object sender, EventArgs evt)
        {
            TreeFilter.Reset();
            ParameterFilter.Reset();
            ShowSpecificItem(ErrorView.SelectedError.Item);
        }
        private void ErrorView_ShowInFileClicked(object sender, EventArgs evt)
        {
            string filePathAndName = string.Format("\"{0}\"", Path.Combine(File.FilePath, File.FileName));
            StringBuilder result = new StringBuilder(1024);
            try
            {
                FindExecutable(filePathAndName, string.Empty, result);
                string assocApp = result?.ToString();
                if (string.IsNullOrEmpty(assocApp))
                {
                    Process.Start(filePathAndName);
                }
                else
                {
                    string arguments = string.Format("{0} -n{1}", filePathAndName, ErrorView.SelectedError.LineInFile);
                    assocApp = string.Format("\"{0}\"", assocApp);
                    Process.Start(assocApp, arguments);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("{0}{1}{1}{2}", TextRecources.Generic_FileCouldNotBeOpened, Environment.NewLine, ex.Message),
                    TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        // public
        public void ShowSpecificItem(EcfStructureItem item)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    ShowSpecificItemInvoked(item);
                });
            }
            else
            {
                ShowSpecificItemInvoked(item);
            }
        }
        public void UpdateAllViews()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    UpdateAllViewsInvoked();
                });
            }
            else
            {
                UpdateAllViewsInvoked();
            }
        }
        public void UpdateParameterView()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    UpdateParameterViewInvoked();
                });
            }
            else
            {
                UpdateParameterViewInvoked();
            }
        }
        public void UpdateInfoView()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    UpdateInfoViewInvoked();
                });
            }
            else
            {
                UpdateInfoViewInvoked();
            }
        }
        public void UpdateErrorView()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    UpdateErrorViewInvoked();
                });
            }
            else
            {
                UpdateErrorViewInvoked();
            }
        }
        public void UpdateTabDescription()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    UpdateTabDescriptionInvoked();
                });
            }
            else
            {
                UpdateTabDescriptionInvoked();
            }
        }
        public void ReselectTreeView()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    ReselectTreeViewInvoked();
                });
            }
            else
            {
                ReselectTreeViewInvoked();
            }
        }
        public void ReselectParameterView()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    ReselectParameterViewInvoked();
                });
            }
            else
            {
                ReselectParameterViewInvoked();
            }
        }
        public int PasteTo(EcfStructureItem target, List<EcfStructureItem> source)
        {
            int pasteCount = 0;
            if (target != null && source != null && source.Count > 0)
            {
                if (target is EcfBlock block)
                {
                    pasteCount = PasteChildItems(block, null, source);
                }
                else
                {
                    PasteAfter(target, source);
                }
            }
            return pasteCount;
        }
        public int PasteAfter(EcfStructureItem target, List<EcfStructureItem> source)
        {
            int pasteCount = 0;
            if (target != null && source != null && source.Count > 0)
            {
                if (target.Parent is EcfBlock block)
                {
                    pasteCount = PasteChildItems(block, target, source);
                }
                else
                {
                    pasteCount = PasteRootItems(target, source);
                }
            }
            return pasteCount;
        }

        // view updating
        private void ShowSpecificItemInvoked(EcfStructureItem item)
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                FilterControl.SetSpecificItem(item);
                if (EcfBaseItem.GetRootItem(item) is EcfBlock block) { item = block; }
                TreeView.ShowSpecificItem(item);
                ParameterView.ShowSpecificItem(item);
                InfoView.ShowSpecificItem(item);
                IsUpdating = false;
            }
        }
        private void UpdateAllViewsInvoked()
        {
            if (FilterControl.SpecificItem != null)
            {
                ShowSpecificItemInvoked(FilterControl.SpecificItem);
            }
            else if (!IsUpdating)
            {
                IsUpdating = true;
                TreeView.UpdateView(TreeFilter, ParameterFilter);
                EcfStructureItem firstSelectedItem = TreeView.SelectedItems.FirstOrDefault();
                ParameterView.UpdateView(ParameterFilter, firstSelectedItem);
                InfoView.UpdateView(firstSelectedItem, ParameterView.SelectedParameters.FirstOrDefault());
                ErrorView.UpdateView();
                UpdateTabDescriptionInvoked();
                IsUpdating = false;
            }
        }
        private void UpdateParameterViewInvoked()
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                EcfStructureItem firstSelectedItem = TreeView.SelectedItems.FirstOrDefault();
                ParameterView.UpdateView(ParameterFilter, firstSelectedItem);
                InfoView.UpdateView(firstSelectedItem, ParameterView.SelectedParameters.FirstOrDefault());
                IsUpdating = false;
            }
        }
        private void UpdateInfoViewInvoked()
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                InfoView.UpdateView(ParameterView.SelectedParameters.FirstOrDefault());
                IsUpdating = false;
            }
        }
        private void UpdateErrorViewInvoked()
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                ErrorView.UpdateView();
                IsUpdating = false;
            }
        }
        private void UpdateTabDescriptionInvoked()
        {
            Text = string.Format("{0}{1}", File.FileName, File.HasUnsavedData ? " *" : "");
            ToolTipText = Path.Combine(File.FilePath, File.FileName);
        }

        // Reselecting 
        private void ReselectTreeViewInvoked()
        {
            TreeView.TryReselect();
        }
        private void ReselectParameterViewInvoked()
        {
            ParameterView.TryReselect();
        }

        // content operation
        private void RemoveTreeItem(List<EcfStructureItem> items)
        {
            List<string> problems = new List<string>();
            List<EcfBlock> allBlocks = File.GetDeepItemList<EcfBlock>();

            List<EcfParameter> parametersToRemove = items.Where(item => item is EcfParameter).Cast<EcfParameter>().ToList();
            problems.AddRange(CheckMandatoryParameters(parametersToRemove));

            List<EcfBlock> blocksToRemove = items.Where(item => item is EcfBlock).Cast<EcfBlock>().ToList();
            problems.AddRange(CheckBlockReferences(blocksToRemove, allBlocks, out HashSet<EcfBlock> inheritingBlocks));

            if (OperationSafetyQuestion(problems) == DialogResult.Yes)
            {
                HashSet<EcfBlock> changedParents = RemoveStructureItems(items);
                changedParents.ToList().ForEach(block => block.RevalidateParameters());
                if (blocksToRemove.Count > 0)
                {
                    allBlocks = allBlocks.Except(blocksToRemove).ToList();
                    inheritingBlocks.ToList().ForEach(block => block.RevalidateReferenceHighLevel(allBlocks));
                    allBlocks.ForEach(block => block.RevalidateUniqueness(allBlocks));
                }
                UpdateAllViews();
            }
        }
        private void RemoveParameterItem(List<EcfParameter> parameters)
        {
            List<string> problems = CheckMandatoryParameters(parameters);
            if (OperationSafetyQuestion(problems) == DialogResult.Yes)
            {
                HashSet<EcfBlock> changedParents = RemoveStructureItems(parameters.Cast<EcfStructureItem>().ToList());
                changedParents.ToList().ForEach(block => block.RevalidateParameters());
                UpdateAllViews();
            }
        }
        private List<string> CheckMandatoryParameters(List<EcfParameter> parameters)
        {
            List<string> mandatoryParameters = File.Definition.BlockParameters.Where(parameter => !parameter.IsOptional).Select(parameter => parameter.Name).ToList();
            return parameters.Where(parameter => mandatoryParameters.Contains(parameter.Key))
                .Select(parameter => string.Format("{0} {1}", parameter.BuildIdentification(), TextRecources.Generic_IsNotOptional)).ToList();
        }
        private List<string> CheckBlockReferences(List<EcfBlock> blockToCheck, List<EcfBlock> completeBlockList, out HashSet<EcfBlock> inheritingBlocks)
        {
            List<string> problems = new List<string>();
            HashSet<EcfBlock> foundBlocks = new HashSet<EcfBlock>();
            blockToCheck.ForEach(block =>
            {
                List<EcfBlock> inheritors = completeBlockList.Where(listedBlock => block.Equals(listedBlock.Inheritor)).ToList();
                inheritors.ForEach(inheritor =>
                {
                    foundBlocks.Add(inheritor);
                    problems.Add(string.Format("{0} {1} {2}", block.BuildIdentification(), TextRecources.Generic_IsReferencedBy, inheritor.BuildIdentification()));
                });
            });
            inheritingBlocks = foundBlocks;
            return problems;
        }
        private DialogResult OperationSafetyQuestion(List<string> problems)
        {
            if (problems.Count < 1) { return DialogResult.Yes; }

            StringBuilder message = new StringBuilder(TextRecources.Generic_ContinueOperationWithErrorsQuestion);
            message.Append(Environment.NewLine);
            problems.ForEach(problem =>
            {
                message.Append(Environment.NewLine);
                message.Append(problem);
            });

            return MessageBox.Show(this, message.ToString(), TitleRecources.Generic_Attention, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
        }
        private HashSet<EcfBlock> RemoveStructureItems(List<EcfStructureItem> items)
        {
            HashSet<EcfBlock> changedParents = new HashSet<EcfBlock>();
            items.ForEach(item =>
            {
                if (item.IsRoot())
                {
                    File.RemoveItem(item);
                }
                else if (item.Parent is EcfBlock block)
                {
                    block.RemoveChild(item);
                    if (item is EcfParameter)
                    {
                        changedParents.Add(block);
                    }
                }
            });
            return changedParents;
        }
        private void AddTreeItemTo(EcfStructureItem item)
        {
            CreateableItems addable;
            if (item == null)
            {
                addable = CreateableItems.Comment | CreateableItems.RootBlock;
            }
            else if (item is EcfBlock)
            {
                addable = CreateableItems.Comment | CreateableItems.ChildBlock | CreateableItems.Parameter;
            }
            else
            {
                AddTreeItemAfter(item);
                return;
            }
            EcfBlock parent = item as EcfBlock;
            if (ItemEditor.ShowDialog(this, File, addable, parent) == DialogResult.OK)
            {
                EcfStructureItem createdItem = ItemEditor.ResultItem;
                if (item == null)
                {
                    File.AddItem(createdItem, null);
                }
                else
                {
                    parent?.AddChild(createdItem, null);
                }
                if (createdItem is EcfBlock)
                {
                    List<EcfBlock> completeBlockList = File.GetDeepItemList<EcfBlock>();
                    completeBlockList.ForEach(block => block.RevalidateReferenceRepairing(completeBlockList));
                }
                UpdateAllViews();
            }
        }
        private void AddTreeItemAfter(EcfStructureItem item)
        {
            CreateableItems addable;
            if (item == null || item.IsRoot())
            {
                addable = CreateableItems.Comment | CreateableItems.RootBlock;
            }
            else
            {
                addable = CreateableItems.Comment | CreateableItems.Parameter | CreateableItems.ChildBlock;
            }
            EcfBlock parent = item?.Parent as EcfBlock;
            if (ItemEditor.ShowDialog(this, File, addable, parent) == DialogResult.OK)
            {
                EcfStructureItem createdItem = ItemEditor.ResultItem;
                if (item == null || parent == null)
                {
                    File.AddItem(createdItem, item);
                }
                else
                {
                    parent.AddChild(createdItem, item);
                }
                if (createdItem is EcfBlock)
                {
                    List<EcfBlock> completeBlockList = File.GetDeepItemList<EcfBlock>();
                    completeBlockList.ForEach(block => block.RevalidateReferenceRepairing(completeBlockList));
                }
                UpdateAllViews();
            }
        }
        private void AddParameterItem(EcfBlock parentBlock, EcfStructureItem preceedingItem)
        {
            if (ItemEditor.ShowDialog(this, File, CreateableItems.Parameter, parentBlock) == DialogResult.OK)
            {
                EcfStructureItem createdItem = ItemEditor.ResultItem;
                parentBlock.AddChild(createdItem, preceedingItem);
                parentBlock.RevalidateParameters();
                UpdateAllViews();
            }
        }
        private bool ChangeTreeItem(EcfStructureItem item)
        {
            if (item is EcfComment comment)
            {
                if (ItemEditor.ShowDialog(this, File, comment) == DialogResult.OK)
                {
                    UpdateAllViews();
                }
                return true;
            }
            else if (item is EcfParameter parameter)
            {
                if (ItemEditor.ShowDialog(this, File, parameter) == DialogResult.OK)
                {
                    if (parameter.Parent is EcfBlock block)
                    {
                        block.RevalidateParameters();
                    }
                    UpdateAllViews();
                }
                return true;
            }
            else if (item is EcfBlock block)
            {
                if (ItemEditor.ShowDialog(this, File, block) == DialogResult.OK)
                {
                    List<EcfBlock> completeBlockList = File.GetDeepItemList<EcfBlock>();
                    completeBlockList.ForEach(listedBlock => listedBlock.RevalidateUniqueness(completeBlockList));
                    completeBlockList.ForEach(listedBlock => listedBlock.RevalidateReferenceRepairing(completeBlockList));
                    UpdateAllViews();
                }
                return true;
            }
            return false;
        }
        private void ChangeParameterItem(EcfParameter parameter)
        {
            if (ItemEditor.ShowDialog(this, File, parameter) == DialogResult.OK)
            {
                if (parameter.Parent is EcfBlock block)
                {
                    block.RevalidateParameters();
                }
                UpdateAllViews();
            }
        }
        private void CopyItems()
        {
            if (LastFocusedView is EcfTreeView treeView)
            {
                CopyClicked?.Invoke(this, new CopyPasteClickedEventArgs(CopyPasteModes.Copy, this, treeView.SelectedItems));
                return;
            }
            else if (LastFocusedView is EcfParameterView parameterView)
            {
                CopyClicked?.Invoke(this, new CopyPasteClickedEventArgs(CopyPasteModes.Copy, this, parameterView.SelectedParameters));
                return;
            }
            MessageBox.Show(this, TextRecources.Generic_NoSuitableSelection, TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        private void PasteItems()
        {
            if (LastFocusedView is EcfTreeView treeView)
            {
                PasteClicked?.Invoke(this, new CopyPasteClickedEventArgs(CopyPasteModes.PasteTo, this, treeView.SelectedItems));
                return;
            }
            else if (LastFocusedView is EcfParameterView parameterView)
            {
                PasteClicked?.Invoke(this, new CopyPasteClickedEventArgs(CopyPasteModes.PasteAfter, this, parameterView.SelectedParameters));
                return;
            }
            MessageBox.Show(this, TextRecources.Generic_NoSuitableSelection, TitleRecources.Generic_Attention, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        private int PasteRootItems(EcfStructureItem target, List<EcfStructureItem> source)
        {
            int itemCount = 0;
            int blockCount = 0;
            foreach (EcfStructureItem item in source)
            {
                // parameter ignorieren, da als root nicht zulässig
                if (item is EcfParameter) { continue; }

                EcfStructureItem newItem = item.BuildDeepCopy();
                File.AddItem(newItem, target);

                itemCount++;
                if (newItem is EcfBlock block)
                {
                    blockCount++;
                    block.Revalidate();
                }
            }
            if (blockCount > 0)
            {
                List<EcfBlock> completeBlockList = File.GetDeepItemList<EcfBlock>();
                completeBlockList.ForEach(listedBlock => listedBlock.RevalidateUniqueness(completeBlockList));
                completeBlockList.ForEach(listedBlock => listedBlock.RevalidateReferenceRepairing(completeBlockList));
            }
            if (itemCount > 0)
            {
                UpdateAllViews();
            }
            return itemCount;
        }
        private int PasteChildItems(EcfBlock parent, EcfStructureItem after, List<EcfStructureItem> source)
        {

            int itemCount = 0;
            int parameterCount = 0;
            int blockCount = 0;
            foreach (EcfStructureItem item in source)
            {
                EcfStructureItem newItem = item.BuildDeepCopy();
                parent.AddChild(newItem, after);

                itemCount++;
                if (newItem is EcfParameter parameter)
                {
                    parameterCount++;
                    parameter.Revalidate();
                }
                else if (newItem is EcfBlock block)
                {
                    blockCount++;
                    block.Revalidate();
                }
            }
            if (parameterCount > 0)
            {
                parent.RevalidateParameters();
            }
            if (blockCount > 0)
            {
                List<EcfBlock> completeBlockList = File.GetDeepItemList<EcfBlock>();
                completeBlockList.ForEach(listedBlock => listedBlock.RevalidateUniqueness(completeBlockList));
                completeBlockList.ForEach(listedBlock => listedBlock.RevalidateReferenceRepairing(completeBlockList));
            }
            UpdateAllViews();
            return itemCount;
        }

        // helper
        private void AddControls()
        {
            ToolContainer.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            TreeView.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            ParameterView.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            InfoView.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            ErrorView.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            ToolContainer.Dock = DockStyle.Top;
            TreeView.Dock = DockStyle.Left;
            ParameterView.Dock = DockStyle.Fill;
            InfoView.Dock = DockStyle.Right;
            ErrorView.Dock = DockStyle.Bottom;

            Controls.Add(FileViewPanel);
            FileViewPanel.Add(ParameterView);
            FileViewPanel.Add(ErrorView);
            FileViewPanel.Add(InfoView);
            FileViewPanel.Add(TreeView);

            FileViewPanel.Add(ToolContainer);
            ToolContainer.Add(FilterControl);
            ToolContainer.Add(TreeFilter);
            ToolContainer.Add(ParameterFilter);
            ToolContainer.Add(ContentOperations);

            FilterControl.Add(TreeFilter);
            FilterControl.Add(ParameterFilter);
        }
        private void SetEventHandlers()
        {
            FilterControl.ApplyFilterClicked += AnyFilterControl_ApplyFilterFired;
            FilterControl.ClearFilterClicked += FilterControl_ClearFilterClicked;

            TreeFilter.ApplyFilterRequested += AnyFilterControl_ApplyFilterFired;

            ParameterFilter.ApplyFilterRequested += AnyFilterControl_ApplyFilterFired;

            ContentOperations.UndoClicked += ContentOperations_UndoClicked;
            ContentOperations.RedoClicked += ContentOperations_RedoClicked;
            ContentOperations.AddClicked += ContentOperations_AddClicked;
            ContentOperations.RemoveClicked += ContentOperations_RemoveClicked;
            ContentOperations.ChangeSimpleClicked += ContentOperations_ChangeSingleClicked;
            ContentOperations.ChangeComplexClicked += ContentOperations_ChangeMultiClicked;
            ContentOperations.MoveUpClicked += ContentOperations_MoveUpClicked;
            ContentOperations.MoveDownClicked += ContentOperations_MoveDownClicked;
            ContentOperations.CopyClicked += ContentOperations_CopyClicked;
            ContentOperations.PasteClicked += ContentOperations_PasteClicked;

            TreeView.ViewResized += (sender, evt) => TreeViewResized(sender, evt);
            InfoView.ViewResized += (sender, evt) => InfoViewResized(sender, evt);
            ErrorView.ViewResized += (sender, evt) => ErrorViewResized(sender, evt);

            TreeView.ItemsSelected += TreeView_ItemsSelected;
            TreeView.DisplayedDataChanged += TreeView_DisplayedDataChanged;
            ParameterView.ParametersSelected += ParameterView_ParametersSelected;
            ParameterView.DisplayedDataChanged += ParameterView_DisplayedDataChanged;

            TreeView.NodeDoubleClicked += TreeView_NodeDoubleClicked;
            TreeView.ChangeItemClicked += TreeView_ChangeItemClicked;
            TreeView.AddToItemClicked += TreeView_AddToItemClicked;
            TreeView.AddAfterItemClicked += TreeView_AddAfterItemClicked;
            TreeView.CopyItemClicked += TreeView_CopyItemClicked;
            TreeView.PasteToItemClicked += TreeView_PasteToItemClicked;
            TreeView.PasteAfterItemClicked += TreeView_PasteAfterItemClicked;
            TreeView.RemoveItemClicked += TreeView_RemoveItemClicked;
            TreeView.DelKeyPressed += TreeView_DelKeyPressed;
            TreeView.CopyKeyPressed += TreeView_CopyKeyPressed;
            TreeView.PasteKeyPressed += TreeView_PasteKeyPressed;

            ParameterView.CellDoubleClicked += ParameterView_CellDoubleClicked;
            ParameterView.ChangeItemClicked += ParameterView_ChangeItemClicked;
            ParameterView.AddAfterItemClicked += ParameterView_AddAfterItemClicked;
            ParameterView.CopyItemClicked += ParameterView_CopyItemClicked;
            ParameterView.PasteAfterItemClicked += ParameterView_PasteAfterItemClicked;
            ParameterView.RemoveItemClicked += ParameterView_RemoveItemClicked;
            ParameterView.DelKeyPressed += ParameterView_DelKeyPressed;
            ParameterView.CopyKeyPressed += ParameterView_CopyKeyPressed;
            ParameterView.PasteKeyPressed += ParameterView_PasteKeyPressed;

            ErrorView.ShowInEditorClicked += ErrorView_ShowInEditorClicked;
            ErrorView.ShowInFileClicked += ErrorView_ShowInFileClicked;
        }

        public class CopyPasteClickedEventArgs : EventArgs
        {
            public CopyPasteModes Mode { get; }
            public EcfTabPage Source { get; }
            public List<EcfStructureItem> SelectedItems { get; }
            public List<EcfStructureItem> CopiedItems { get; }

            public enum CopyPasteModes
            {
                Copy,
                PasteTo,
                PasteAfter,
            }

            public CopyPasteClickedEventArgs(CopyPasteModes mode, EcfTabPage source, List<EcfStructureItem> selectedItems) :
                base()
            {
                Mode = mode;
                Source = source;
                SelectedItems = selectedItems;
                CopiedItems = mode == CopyPasteModes.Copy ? SelectedItems.Select(item => item.BuildDeepCopy()).ToList() : null;
            }
            public CopyPasteClickedEventArgs(CopyPasteModes mode, EcfTabPage source, List<EcfParameter> selectedItems) :
                this(mode, source, selectedItems.Cast<EcfStructureItem>().ToList())
            {

            }
        }
    }

    public class EcfFileContainer : Panel
    {
        public EcfFileContainer() : base()
        {
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            Dock = DockStyle.Fill;
        }

        public void Add(Control view)
        {
            Controls.Add(view);
        }
    }
    public abstract class EcfBaseView : GroupBox
    {
        public event EventHandler ViewResized;

        public string ViewName { get; } = string.Empty;
        public bool IsUpdating { get; protected set; } = false;

        protected EgsEcfFile File { get; } = null;
        private ResizeableBorders ResizeableBorder { get; } = ResizeableBorders.None;
        private ResizeableBorders DraggedBorder { get; set; } = ResizeableBorders.None;
        private int GrapSize { get; } = 0;
        protected bool IsDragged { get; private set; } = false;

        public enum ResizeableBorders
        {
            None,
            LeftBorder,
            RightBorder,
            TopBorder,
            BottomBorder,
        }

        protected EcfBaseView(string headline, EgsEcfFile file, ResizeableBorders borderMode)
        {
            ViewName = headline;
            Text = headline;
            File = file;
            ResizeableBorder = borderMode;
            GrapSize = Width - DisplayRectangle.Width;
        }

        // events
        protected override void OnMouseDown(MouseEventArgs evt)
        {
            if (evt.Button == MouseButtons.Left)
            {
                DraggedBorder = IsInDragArea(evt);
                if (DraggedBorder != ResizeableBorders.None)
                {
                    IsDragged = true;
                }
            }
        }
        protected override void OnMouseMove(MouseEventArgs evt)
        {
            if (IsDragged)
            {
                ResizeBounds(evt);
            }
            else
            {
                UpdateCursor(evt);
            }
        }
        protected override void OnMouseUp(MouseEventArgs evt)
        {
            if (IsDragged)
            {
                ViewResized?.Invoke(this, null);
            }
            IsDragged = false;
        }
        protected override void OnMouseLeave(EventArgs evt)
        {
            if (IsDragged)
            {
                ViewResized?.Invoke(this, null);
            }
            IsDragged = false;
            RefreshCursor(DefaultCursor);
        }

        // public

        // privates
        private ResizeableBorders IsInDragArea(MouseEventArgs evt)
        {
            switch (ResizeableBorder)
            {
                case ResizeableBorders.LeftBorder: return IsOnLeftBorder(evt) ? ResizeableBorders.LeftBorder : ResizeableBorders.None;
                case ResizeableBorders.RightBorder: return IsOnRightBorder(evt) ? ResizeableBorders.RightBorder : ResizeableBorders.None;
                case ResizeableBorders.TopBorder: return IsOnTopBorder(evt) ? ResizeableBorders.TopBorder : ResizeableBorders.None;
                case ResizeableBorders.BottomBorder: return IsOnBottomBorder(evt) ? ResizeableBorders.BottomBorder : ResizeableBorders.None;
                default: return ResizeableBorders.None;
            }
        }
        private bool IsOnLeftBorder(MouseEventArgs evt)
        {
            return evt.X > (0 - GrapSize) && evt.X < GrapSize;
        }
        private bool IsOnRightBorder(MouseEventArgs evt)
        {
            return evt.X > (Width - GrapSize) && evt.X < Width;
        }
        private bool IsOnTopBorder(MouseEventArgs evt)
        {
            return evt.Y > (0 - GrapSize) && evt.Y < GrapSize;
        }
        private bool IsOnBottomBorder(MouseEventArgs evt)
        {
            return evt.Y > (Height - GrapSize) && evt.Y < Height;
        }
        private void ResizeBounds(MouseEventArgs evt)
        {
            switch (DraggedBorder)
            {
                case ResizeableBorders.LeftBorder: Width -= PointToClient(Cursor.Position).X; break;
                case ResizeableBorders.RightBorder: Width = evt.X; break;
                case ResizeableBorders.TopBorder: Height -= PointToClient(Cursor.Position).Y; break;
                case ResizeableBorders.BottomBorder: Height = evt.Y; break;
                default: break;
            }
        }
        private void UpdateCursor(MouseEventArgs evt)
        {
            switch (IsInDragArea(evt))
            {
                case ResizeableBorders.LeftBorder:
                case ResizeableBorders.RightBorder:
                    RefreshCursor(Cursors.SizeWE);
                    break;
                case ResizeableBorders.TopBorder:
                case ResizeableBorders.BottomBorder:
                    RefreshCursor(Cursors.SizeNS);
                    break;
                default:
                    RefreshCursor(DefaultCursor);
                    break;
            }
        }
        private void RefreshCursor(Cursor cursor)
        {
            if (Cursor != cursor)
            {
                Cursor = cursor;
            }
        }
    }
    public class EcfTreeView : EcfBaseView
    {
        public event EventHandler DisplayedDataChanged;
        public event EventHandler ItemsSelected;

        public event EventHandler<TreeNodeMouseClickEventArgs> NodeDoubleClicked;
        public event EventHandler ChangeItemClicked;
        public event EventHandler AddToItemClicked;
        public event EventHandler AddAfterItemClicked;
        public event EventHandler CopyItemClicked;
        public event EventHandler PasteToItemClicked;
        public event EventHandler PasteAfterItemClicked;
        public event EventHandler RemoveItemClicked;

        public event EventHandler DelKeyPressed;
        public event EventHandler CopyKeyPressed;
        public event EventHandler PasteKeyPressed;

        public List<EcfStructureItem> SelectedItems { get; } = new List<EcfStructureItem>();

        private Panel View { get; } = new Panel();
        private EcfToolContainer ToolContainer { get; } = new EcfToolContainer();
        private EcfSorter StructureSorter { get; }
        private TreeView Tree { get; } = new TreeView();
        private ContextMenuStrip TreeMenu { get; } = new ContextMenuStrip();
        private List<EcfTreeNode> RootTreeNodes { get; } = new List<EcfTreeNode>();
        private List<EcfTreeNode> AllTreeNodes { get; } = new List<EcfTreeNode>();
        private List<EcfTreeNode> SelectedNodes { get; } = new List<EcfTreeNode>();

        private bool IsSelectionUpdating { get; set; } = false;

        public EcfTreeView(string headline, EgsEcfFile file, ResizeableBorders mode, VisibleItemCount sorterItemCount) : base(headline, file, mode)
        {
            StructureSorter = new EcfSorter(
                TextRecources.EcfTreeView_ToolTip_TreeItemCountSelector,
                TextRecources.EcfTreeView_ToolTip_TreeItemGroupSelector,
                TextRecources.EcfTreeView_ToolTip_TreeSorterDirection,
                TextRecources.EcfTreeView_ToolTip_TreeSorterOriginOrder,
                TextRecources.EcfTreeView_ToolTip_TreeSorterAlphabeticOrder,
                sorterItemCount);
            StructureSorter.SortingUserChanged += StructureSorter_SortingUserChanged;

            View.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            View.Dock = DockStyle.Fill;

            ToolContainer.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            ToolContainer.Dock = DockStyle.Top;

            Tree.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Tree.Dock = DockStyle.Fill;
            Tree.CheckBoxes = true;
            Tree.HideSelection = false;
            Tree.TreeViewNodeSorter = new EcfStructureComparer(StructureSorter, file);

            Tree.NodeMouseClick += Tree_NodeMouseClick;
            Tree.NodeMouseDoubleClick += Tree_NodeMouseDoubleClick;
            Tree.KeyPress += Tree_KeyPress;
            Tree.KeyUp += Tree_KeyUp;
            Tree.BeforeExpand += Tree_BeforeExpand;
            Tree.BeforeCollapse += Tree_BeforeCollapse;

            ToolContainer.Add(StructureSorter);
            View.Controls.Add(Tree);
            View.Controls.Add(ToolContainer);
            Controls.Add(View);

            TreeMenu.Items.Add(TitleRecources.Generic_Change, IconRecources.Icon_ChangeSimple, (sender, evt) => ChangeItemClicked?.Invoke(sender, evt));
            TreeMenu.Items.Add(TitleRecources.Generic_AddTo, IconRecources.Icon_Add, (sender, evt) => AddToItemClicked?.Invoke(sender, evt));
            TreeMenu.Items.Add(TitleRecources.Generic_AddAfter, IconRecources.Icon_Add, (sender, evt) => AddAfterItemClicked?.Invoke(sender, evt));
            TreeMenu.Items.Add(TitleRecources.Generic_Copy, IconRecources.Icon_Copy, (sender, evt) => CopyItemClicked?.Invoke(sender, evt));
            TreeMenu.Items.Add(TitleRecources.Generic_PasteTo, IconRecources.Icon_Paste, (sender, evt) => PasteToItemClicked?.Invoke(sender, evt));
            TreeMenu.Items.Add(TitleRecources.Generic_PasteAfter, IconRecources.Icon_Paste, (sender, evt) => PasteAfterItemClicked?.Invoke(sender, evt));
            TreeMenu.Items.Add(TitleRecources.Generic_Remove, IconRecources.Icon_Remove, (sender, evt) => RemoveItemClicked?.Invoke(sender, evt));
        }

        // publics
        public void UpdateView(EcfTreeFilter treeFilter, EcfParameterFilter parameterFilter)
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                BuildNodesTree(treeFilter, parameterFilter);
                UpdateSorterInvoke();
                RefreshViewInvoke();
                IsUpdating = false;
                DisplayedDataChanged?.Invoke(this, null);
            }
        }
        public void TryReselect()
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                List<EcfTreeNode> newRootNodes = Tree.Nodes.Cast<EcfTreeNode>().ToList();
                List<EcfTreeNode> newSubNodes = newRootNodes.SelectMany(rootNode => GetSubNodes(rootNode)).ToList();
                List<EcfTreeNode> completeNodes = newRootNodes.Concat(newSubNodes).ToList();
                SelectedNodes.ForEach(oldNode =>
                {
                    TrySelectSimilarNode(oldNode, completeNodes);
                });
                FindSelectedItems();
                IsUpdating = false;
            }
        }
        public void ShowSpecificItem(EcfStructureItem item)
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                BuildNodesTree(item);
                UpdateSorterInvoke();
                RefreshViewInvoke();
                IsUpdating = false;
                DisplayedDataChanged?.Invoke(this, null);
            }
        }

        // events
        private void Tree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs evt)
        {
            UpdateCheckedNotes(evt.Node as EcfTreeNode);
            NodeDoubleClicked?.Invoke(sender, evt);
        }
        private void Tree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs evt)
        {
            UpdateCheckedNotes(evt.Node as EcfTreeNode);
            if (evt.Button == MouseButtons.Right)
            {
                TreeMenu.Show(Tree, evt.Location);
            }
        }
        private void Tree_BeforeExpand(object sender, TreeViewCancelEventArgs evt)
        {
            // hack für urst langsames treeview (painted auch collapsed nodes @.@)
            if (evt.Node is EcfTreeNode node)
            {
                node.Nodes.Clear();
                node.Nodes.AddRange(node.PreparedNodes.AsEnumerable().Reverse().ToArray());
            }
        }
        private void Tree_BeforeCollapse(object sender, TreeViewCancelEventArgs evt)
        {
            // hack für urst langsames treeview (painted auch collapsed nodes @.@)
            if (evt.Node is EcfTreeNode node)
            {
                node.Nodes.Clear();
                node.Nodes.Add(new EcfTreeNode(""));
            }
        }
        private void Tree_KeyUp(object sender, KeyEventArgs evt)
        {
            if (evt.KeyCode == Keys.Delete) { DelKeyPressed?.Invoke(sender, evt); evt.Handled = true; }
            else if (evt.Control && evt.KeyCode == Keys.C) { CopyKeyPressed?.Invoke(sender, evt); evt.Handled = true; }
            else if (evt.Control && evt.KeyCode == Keys.V) { PasteKeyPressed?.Invoke(sender, evt); evt.Handled = true; }
        }
        private void Tree_KeyPress(object sender, KeyPressEventArgs evt)
        {
            // hack for sqirky "ding"
            evt.Handled = true;
        }
        private void StructureSorter_SortingUserChanged(object sender, EventArgs evt)
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                RefreshView();
                IsUpdating = false;
                DisplayedDataChanged?.Invoke(this, null);
            }
        }

        // privates
        private void TrySelectSimilarNode(EcfTreeNode oldNode, List<EcfTreeNode> newNodes)
        {
            EcfTreeNode newNode = newNodes.FirstOrDefault(node => node.Item?.Equals(oldNode.Item) ?? false);
            if (newNode != null)
            {
                newNode.Checked = true;
                EnsureTreeVisiblity(newNode);
            }
            else if (oldNode.Parent != null)
            {
                TrySelectSimilarNode(oldNode.Parent as EcfTreeNode, newNodes);
            }
        }
        private void EnsureTreeVisiblity(EcfTreeNode node)
        {
            List<EcfTreeNode> parents = new List<EcfTreeNode>();
            while (node.PreparedParent != null)
            {
                node.EnsureVisible();
                node = node.PreparedParent;
                parents.Add(node);
            }
            parents.Reverse();
            parents.ForEach(parent => parent.Expand());
        }
        private void UpdateCheckedNotes(EcfTreeNode clickedNode)
        {
            if (!IsUpdating && !IsSelectionUpdating)
            {
                IsSelectionUpdating = true;

                if (ModifierKeys == Keys.Control || ModifierKeys == Keys.Shift)
                {
                    clickedNode.Checked = true;
                }
                else
                {
                    CheckOnlySingleNode(clickedNode);
                }
                if (ModifierKeys == Keys.Shift)
                {
                    CheckNodeRange(RootTreeNodes.FirstOrDefault(node => node.Checked), RootTreeNodes.LastOrDefault(node => node.Checked));
                }
                Tree.SelectedNode = clickedNode;

                FindSelectedItems();
                ItemsSelected?.Invoke(this, null);

                IsSelectionUpdating = false;
            }
        }
        private void BuildNodesTree(EcfTreeFilter treeFilter, EcfParameterFilter parameterFilter)
        {
            RootTreeNodes.Clear();
            AllTreeNodes.Clear();

            foreach (EcfStructureItem item in File.ItemList)
            {
                BuildNodesTree(item, treeFilter, parameterFilter);
            }
        }
        private void BuildNodesTree(EcfStructureItem item)
        {
            RootTreeNodes.Clear();
            AllTreeNodes.Clear();
            BuildNodesTree(item, null, null);
        }
        private void BuildNodesTree(EcfStructureItem item, EcfTreeFilter treeFilter, EcfParameterFilter parameterFilter)
        {
            if (TryBuildNode(out EcfTreeNode rootNode, item, treeFilter, parameterFilter))
            {
                if (treeFilter != null && treeFilter.ErrorDisplayMode == ErrorDisplayModes.ShowOnlyFaultyItems && !rootNode.HasError)
                {
                    return;
                }
                if (treeFilter != null && treeFilter.ErrorDisplayMode == ErrorDisplayModes.ShowOnlyNonFaultyItems && rootNode.HasError)
                {
                    return;
                }
                if (treeFilter?.IsLike(rootNode.Text) ?? true)
                {
                    RootTreeNodes.Add(rootNode);
                    AllTreeNodes.Add(rootNode);
                    AllTreeNodes.AddRange(GetSubNodes(rootNode));
                    // hack für urst langsames treeview (painted auch collapsed nodes @.@)
                    if (rootNode.PreparedNodes.Count > 0) { rootNode.Nodes.Add(new EcfTreeNode("")); }
                }
            }
        }
        private bool TryBuildNode(out EcfTreeNode node, EcfStructureItem item, EcfTreeFilter treeFilter, EcfParameterFilter parameterFilter)
        {
            node = null;
            if (item is EcfComment comment)
            {
                if (treeFilter?.IsCommentsActive ?? true)
                {
                    node = new EcfTreeNode(comment, string.Format("{0}: {1}", TitleRecources.Generic_Comment, string.Join(" / ", comment.Comments)));
                }
            }
            else if (item is EcfParameter parameter)
            {
                if ((treeFilter?.IsParametersActive ?? true) && (parameterFilter?.IsParameterVisible(parameter) ?? true))
                {
                    node = new EcfTreeNode(parameter, parameter.BuildIdentification());
                }
            }
            else if (item is EcfBlock block)
            {
                if (treeFilter?.IsDataBlocksActive ?? true)
                {
                    node = new EcfTreeNode(block, block.BuildIdentification());
                    foreach (EcfStructureItem childItem in block.ChildItems)
                    {
                        if (TryBuildNode(out EcfTreeNode childNode, childItem, treeFilter, parameterFilter))
                        {
                            childNode.PreparedParent = node;
                            node.PreparedNodes.Add(childNode);
                        }
                    }
                    //if (node.PreparedNodes.Count == 0 && !(parameterFilter?.IsLikeText.Equals(string.Empty) ?? true))
                    if (node.PreparedNodes.Count == 0 && (parameterFilter?.AnyFilterSet() ?? false))
                    {
                        return false;
                    }
                    // hack für urst langsames treeview (painted auch collapsed nodes @.@)
                    if (node.PreparedNodes.Count > 0) { node.Nodes.Add(new EcfTreeNode("")); }
                }
            }
            else
            {
                node = new EcfTreeNode(item.ToString());
            }
            return node != null;
        }
        private void UpdateSorterInvoke()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    StructureSorter.SetOverallItemCount(RootTreeNodes.Count);
                });
            }
            else
            {
                StructureSorter.SetOverallItemCount(RootTreeNodes.Count);
            }
        }
        private void RefreshViewInvoke()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    RefreshView();
                });
            }
            else
            {
                RefreshView();
            }
        }
        private void RefreshView()
        {
            Tree.BeginUpdate();
            Tree.Nodes.Clear();
            Tree.Nodes.AddRange(RootTreeNodes.Skip(StructureSorter.ItemCount * (StructureSorter.ItemGroup - 1)).Take(StructureSorter.ItemCount).ToArray());
            Tree.Sort();
            AllTreeNodes.ForEach(node => node.Checked = false);
            Tree.EndUpdate();
            Text = string.Format("{0} - {1} {3} - {2} {4}", ViewName, RootTreeNodes.Count,
                RootTreeNodes.Sum(node => GetSubNodes(node).Count),
                TitleRecources.Generic_RootElements,
                TitleRecources.Generic_ChildElements);
        }
        private void CheckOnlySingleNode(EcfTreeNode node)
        {
            AllTreeNodes.ForEach(treeNode =>
            {
                treeNode.Checked = treeNode.Equals(node);
            });
        }
        private void CheckNodeRange(EcfTreeNode first, EcfTreeNode last)
        {
            if (first != null && last != null)
            {
                for (int index = first.Index; index <= last.Index; index++)
                {
                    if (Tree.Nodes[index] is EcfTreeNode node)
                    {
                        node.Checked = true;
                        GetSubNodes(node).ForEach(subNode =>
                        {
                            subNode.Checked = false;
                        });
                    }
                }
            }
        }
        private void FindSelectedItems()
        {
            SelectedNodes.Clear();
            SelectedNodes.AddRange(AllTreeNodes.Where(node => node.IsSelected || node.Checked));
            SelectedItems.Clear();
            SelectedItems.AddRange(SelectedNodes.Select(node => node.Item));
        }
        private List<EcfTreeNode> GetSubNodes(EcfTreeNode node)
        {
            List<EcfTreeNode> nodes = new List<EcfTreeNode>();
            nodes.AddRange(node.PreparedNodes.Cast<EcfTreeNode>());
            foreach (EcfTreeNode subNode in node.PreparedNodes)
            {
                nodes.AddRange(GetSubNodes(subNode)); ;
            }
            return nodes;
        }

        private class EcfStructureComparer : IComparer
        {
            private EcfSorter Sorter { get; } = null;
            private EgsEcfFile File { get; } = null;

            public EcfStructureComparer(EcfSorter sorter, EgsEcfFile file)
            {
                Sorter = sorter;
                File = file;
            }

            public int Compare(object first, object second)
            {
                int compare = 1;
                if (first is EcfTreeNode node1 && second is EcfTreeNode node2)
                {
                    switch (Sorter.SortingType)
                    {
                        case SortingTypes.Alphabetical: compare = string.Compare(node1.Text, node2.Text); break;
                        default: compare = File.ItemList.IndexOf(node1.Item) - File.ItemList.IndexOf(node2.Item); break;
                    }
                    compare *= Sorter.IsAscending ? 1 : -1;
                }
                return compare;
            }
        }
        private class EcfTreeNode : TreeNode
        {
            public EcfStructureItem Item { get; } = null;
            public bool HasError { get; } = false;

            public EcfTreeNode PreparedParent { get; set; } = null;
            public List<EcfTreeNode> PreparedNodes { get; } = new List<EcfTreeNode>();

            public EcfTreeNode(EcfStructureItem item, string name) : base()
            {
                Item = item;
                Text = name;
                HasError = item is EcfStructureItem structureItem && structureItem.GetDeepErrorList(true).Count > 0;
                if (HasError)
                {
                    ForeColor = Color.Red;
                }
            }
            public EcfTreeNode(string name) : this(null, name)
            {

            }
        }
    }
    public class EcfParameterView : EcfBaseView
    {
        public event EventHandler DisplayedDataChanged;
        public event EventHandler ParametersSelected;

        public event EventHandler<DataGridViewCellEventArgs> CellDoubleClicked;
        public event EventHandler ChangeItemClicked;
        public event EventHandler AddAfterItemClicked;
        public event EventHandler CopyItemClicked;
        public event EventHandler PasteAfterItemClicked;
        public event EventHandler RemoveItemClicked;

        public event EventHandler DelKeyPressed;
        public event EventHandler CopyKeyPressed;
        public event EventHandler PasteKeyPressed;

        public List<EcfParameter> SelectedParameters { get; } = new List<EcfParameter>();

        private Panel View { get; } = new Panel();
        private EcfToolContainer ToolContainer { get; } = new EcfToolContainer();
        private EcfSorter ParameterSorter { get; }
        private DataGridView Grid { get; } = new DataGridView();
        private ContextMenuStrip GridMenu { get; } = new ContextMenuStrip();
        private List<EcfParameterRow> ParameterRows { get; } = new List<EcfParameterRow>();
        private List<EcfParameterRow> SelectedRows { get; } = new List<EcfParameterRow>();

        private DataGridViewTextBoxColumn ParameterNumberColumn { get; } = new DataGridViewTextBoxColumn();
        private DataGridViewCheckBoxColumn ParameterInheritedColumn { get; } = new DataGridViewCheckBoxColumn();
        private DataGridViewCheckBoxColumn ParameterOverwritingColumn { get; } = new DataGridViewCheckBoxColumn();
        private DataGridViewTextBoxColumn ParameterParentColumn { get; } = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn ParameterNameColumn { get; } = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn ParameterValueColumn { get; } = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn ParameterCommentColumn { get; } = new DataGridViewTextBoxColumn();

        private bool IsSelectionUpdating { get; set; } = false;

        public EcfParameterView(string headline, EgsEcfFile file, ResizeableBorders mode, VisibleItemCount sorterItemCount) : base(headline, file, mode)
        {
            ParameterSorter = new EcfSorter(
                TextRecources.EcfParameterView_ToolTip_ParameterCountSelector,
                TextRecources.EcfParameterView_ToolTip_ParameterGroupSelector,
                TextRecources.EcfParameterView_ToolTip_ParameterSorterDirection,
                TextRecources.EcfParameterView_ToolTip_ParameterSorterOriginOrder,
                TextRecources.EcfParameterView_ToolTip_ParameterSorterAlphabeticOrder,
                sorterItemCount);
            ParameterSorter.SortingUserChanged += ParameterSorter_SortingUserChanged;

            View.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            View.Dock = DockStyle.Fill;

            ToolContainer.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            ToolContainer.Dock = DockStyle.Top;

            InitGridViewColumns();
            InitGridView();

            GridMenu.Items.Add(TitleRecources.Generic_Change, IconRecources.Icon_ChangeSimple, (sender, evt) => ChangeItemClicked?.Invoke(sender, evt));
            GridMenu.Items.Add(TitleRecources.Generic_AddAfter, IconRecources.Icon_Add, (sender, evt) => AddAfterItemClicked?.Invoke(sender, evt));
            GridMenu.Items.Add(TitleRecources.Generic_Copy, IconRecources.Icon_Copy, (sender, evt) => CopyItemClicked?.Invoke(sender, evt));
            GridMenu.Items.Add(TitleRecources.Generic_PasteAfter, IconRecources.Icon_Paste, (sender, evt) => PasteAfterItemClicked?.Invoke(sender, evt));
            GridMenu.Items.Add(TitleRecources.Generic_Remove, IconRecources.Icon_Remove, (sender, evt) => RemoveItemClicked?.Invoke(sender, evt));

            ToolContainer.Add(ParameterSorter);
            View.Controls.Add(Grid);
            View.Controls.Add(ToolContainer);
            Controls.Add(View);
        }

        // events
        private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs evt)
        {
            if (evt.RowIndex > -1 && evt.ColumnIndex > -1)
            {
                UpdateSelectedCells(evt.RowIndex);
                CellDoubleClicked?.Invoke(sender, evt);
            }
        }
        private void Grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs evt)
        {
            if (evt.RowIndex > -1 && evt.ColumnIndex > -1)
            {
                UpdateSelectedCells(evt.RowIndex);
                if (evt.Button == MouseButtons.Right)
                {
                    Point cellLocation = Grid.GetCellDisplayRectangle(evt.ColumnIndex, evt.RowIndex, false).Location;
                    GridMenu.Show(Grid, new Point(cellLocation.X + evt.X, cellLocation.Y + evt.Y));
                }
            }
        }
        private void Grid_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs evt)
        {
            UpdateSelectedCells(evt.RowIndex);
        }
        private void Grid_KeyUp(object sender, KeyEventArgs evt)
        {
            if (evt.KeyCode == Keys.Delete) { DelKeyPressed?.Invoke(sender, evt); evt.Handled = true; }
            else if (evt.Control && evt.KeyCode == Keys.C) { CopyKeyPressed?.Invoke(sender, evt); evt.Handled = true; }
            else if (evt.Control && evt.KeyCode == Keys.V) { PasteKeyPressed?.Invoke(sender, evt); evt.Handled = true; }
        }
        private void ParameterSorter_SortingUserChanged(object sender, EventArgs evt)
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                RefreshViewInvoke();
                IsUpdating = false;
                DisplayedDataChanged?.Invoke(this, null);
            }
        }

        // publics
        public void UpdateView(EcfParameterFilter filter, EcfStructureItem item)
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                BuildGridViewRows(filter, item);
                UpdateSorterInvoke();
                RefreshViewInvoke();
                IsUpdating = false;
                DisplayedDataChanged?.Invoke(this, null);
            }
        }
        public void TryReselect()
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                SelectedRows.ForEach(oldRow =>
                {
                    EcfParameterRow row = Grid.Rows.Cast<EcfParameterRow>().FirstOrDefault(newRow => newRow.Parameter.Equals(oldRow.Parameter));
                    if (row != null)
                    {
                        row.Selected = true;
                    }
                });
                FindSelectedParameters();
                EcfParameterRow firstRow = SelectedRows.FirstOrDefault();
                if (firstRow != null)
                {
                    Grid.FirstDisplayedScrollingRowIndex = firstRow.Index;
                }
                IsUpdating = false;
            }
        }
        public void ShowSpecificItem(EcfStructureItem item)
        {
            IsUpdating = true;
            BuildGridViewRows(null, item);
            UpdateSorterInvoke();
            RefreshViewInvoke();
            IsUpdating = false;
            DisplayedDataChanged?.Invoke(this, null);
        }

        // privates
        private void InitGridViewColumns()
        {
            ParameterNumberColumn.HeaderText = TitleRecources.EcfParameterView_ParameterNumberColumn;
            ParameterInheritedColumn.HeaderText = TitleRecources.Generic_Inherited;
            ParameterOverwritingColumn.HeaderText = TitleRecources.EcfParameterView_ParameterOverwritingColumn;
            ParameterParentColumn.HeaderText = TitleRecources.EcfParameterView_ParameterParentColumn;
            ParameterNameColumn.HeaderText = TitleRecources.EcfParameterView_ParameterNameColumn;
            ParameterValueColumn.HeaderText = TitleRecources.Generic_Value;
            ParameterCommentColumn.HeaderText = TitleRecources.Generic_Comment;

            ParameterNumberColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            ParameterInheritedColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            ParameterOverwritingColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            ParameterParentColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            ParameterNameColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            ParameterValueColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            ParameterCommentColumn.SortMode = DataGridViewColumnSortMode.Programmatic;

            ParameterInheritedColumn.ToolTipText = TextRecources.EcfParameterView_ToolTip_InheritedColumn;
            ParameterOverwritingColumn.ToolTipText = TextRecources.EcfParameterView_ToolTip_OverwritingColumn;
            ParameterValueColumn.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }
        private void InitGridView()
        {
            Grid.AllowUserToAddRows = false;
            Grid.AllowUserToDeleteRows = false;
            Grid.AllowDrop = false;
            Grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            Grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            Grid.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Grid.Dock = DockStyle.Fill;
            Grid.EditMode = DataGridViewEditMode.EditProgrammatically;
            Grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            Grid.RowHeaderMouseClick += Grid_RowHeaderMouseClick;
            Grid.CellMouseClick += Grid_CellMouseClick;
            Grid.CellDoubleClick += Grid_CellDoubleClick;
            Grid.KeyUp += Grid_KeyUp;

            Grid.Columns.Add(ParameterNumberColumn);
            Grid.Columns.Add(ParameterInheritedColumn);
            Grid.Columns.Add(ParameterOverwritingColumn);
            Grid.Columns.Add(ParameterParentColumn);
            Grid.Columns.Add(ParameterNameColumn);
            Grid.Columns.Add(ParameterValueColumn);
            Grid.Columns.Add(ParameterCommentColumn);
        }
        private void UpdateSelectedCells(int clickedRow)
        {
            if (!IsUpdating && !IsSelectionUpdating)
            {
                IsSelectionUpdating = true;
                if (ModifierKeys == Keys.Control || ModifierKeys == Keys.Shift)
                {
                    Grid.Rows[clickedRow].Selected = true;
                }
                else
                {
                    Grid.ClearSelection();
                    Grid.Rows[clickedRow].Selected = true;
                }
                if (ModifierKeys == Keys.Shift)
                {
                    IEnumerable<DataGridViewCell> selectedCells = Grid.Rows.Cast<DataGridViewRow>().SelectMany(row => row.Cells.Cast<DataGridViewCell>().Where(cell => cell.Selected));
                    int firstRow = selectedCells.Min(cell => cell.RowIndex);
                    int lastRow = selectedCells.Max(cell => cell.RowIndex);
                    for (int row = firstRow; row <= lastRow; row++)
                    {
                        Grid.Rows[row].Selected = true;
                    }
                }
                FindSelectedParameters();
                ParametersSelected?.Invoke(this, null);

                IsSelectionUpdating = false;
            }
        }
        private void BuildGridViewRows(EcfParameterFilter filter, EcfStructureItem item)
        {
            ParameterRows.Clear();

            if (item is EcfBlock block)
            {
                BuildParentBlockRows(block, filter);
                BuildDataBlockRowGroup(block, filter, false);
            }
            else if (item is EcfParameter parameter)
            {
                BuildParameterRow(parameter, false, null);
            }
        }
        private void BuildParentBlockRows(EcfBlock block, EcfParameterFilter filter)
        {
            EcfBlock inheritedBlock = block.Inheritor;
            if (inheritedBlock != null)
            {
                BuildParentBlockRows(inheritedBlock, filter);
                BuildDataBlockRowGroup(inheritedBlock, filter, true);
            }
        }
        private void BuildDataBlockRowGroup(EcfBlock block, EcfParameterFilter filter, bool isInherited)
        {
            foreach (EcfStructureItem subItem in block.ChildItems)
            {
                if (subItem is EcfParameter parameter)
                {
                    if (filter?.IsParameterVisible(parameter) ?? true)
                    {
                        EcfParameterRow overwrittenRow = ParameterRows.LastOrDefault(row => row.Parameter.Key.Equals(parameter.Key) && row.IsInherited());
                        BuildParameterRow(parameter, isInherited, overwrittenRow);
                    }
                }
                else if (subItem is EcfBlock subBlock)
                {
                    BuildDataBlockRowGroup(subBlock, filter, isInherited);
                }
            }
        }
        private void BuildParameterRow(EcfParameter parameter, bool isInherited, EcfParameterRow overwrittenRow)
        {
            ParameterRows.Add(new EcfParameterRow(ParameterRows.Count + 1, parameter.Parent?.BuildIdentification(), parameter, isInherited, overwrittenRow));
        }
        private void UpdateSorterInvoke()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    ParameterSorter.SetOverallItemCount(ParameterRows.Count);
                });
            }
            else
            {
                ParameterSorter.SetOverallItemCount(ParameterRows.Count);
            }
        }
        private void RefreshViewInvoke()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    RefreshView();
                });
            }
            else
            {
                RefreshView();
            }
        }
        private void RefreshView()
        {
            Grid.SuspendLayout();
            Grid.Rows.Clear();
            Grid.Rows.AddRange(ParameterRows.Skip(ParameterSorter.ItemCount * (ParameterSorter.ItemGroup - 1)).Take(ParameterSorter.ItemCount).ToArray());
            Grid.Sort(GetSortingColumn(ParameterSorter), ParameterSorter.IsAscending ? ListSortDirection.Ascending : ListSortDirection.Descending);
            Grid.AutoResizeColumns();
            Grid.AutoResizeRows();
            Grid.ClearSelection();
            Grid.ResumeLayout();
            Text = string.Format("{0} - {1} {4} - {2} {5} - {3} {6}", ViewName, ParameterRows.Count,
                ParameterRows.Count(row => row.IsInherited()),
                ParameterRows.Count(row => row.IsOverwriting()),
                TitleRecources.Generic_Parameters,
                TitleRecources.EcfParameterView_Header_InheritedParameters,
                TitleRecources.EcfParameterView_Header_OverwritingParameters);
        }
        private DataGridViewColumn GetSortingColumn(EcfSorter sorter)
        {
            switch (sorter.SortingType)
            {
                case SortingTypes.Alphabetical: return ParameterNameColumn;
                default: return ParameterNumberColumn;
            }
        }
        private void FindSelectedParameters()
        {
            SelectedRows.Clear();
            SelectedRows.AddRange(Grid.Rows.Cast<EcfParameterRow>().Where(row => row.Cells.Cast<DataGridViewCell>().Any(cell => cell.Selected)));
            SelectedParameters.Clear();
            SelectedParameters.AddRange(SelectedRows.Select(row => row.Parameter));
        }

        private class EcfParameterRow : DataGridViewRow
        {
            public EcfParameter Parameter { get; }
            public EcfParameterRow OverwrittenRow { get; }
            public bool HasError { get; }

            private DataGridViewTextBoxCell NumberCell { get; }
            private DataGridViewCheckBoxCell IsInheritedCell { get; }
            private DataGridViewCheckBoxCell IsOverwritingCell { get; }
            private DataGridViewTextBoxCell ParentNameCell { get; }
            private DataGridViewTextBoxCell ParameterNameCell { get; }
            private DataGridViewTextBoxCell ValueCell { get; }
            private DataGridViewTextBoxCell CommentsCell { get; }

            public EcfParameterRow(int number, string parentName, EcfParameter parameter, bool isInherited, EcfParameterRow overwrittenRow) : base()
            {
                Parameter = parameter;
                OverwrittenRow = overwrittenRow;
                HasError = parameter.GetDeepErrorList(true).Count > 0;

                NumberCell = new DataGridViewTextBoxCell() { Value = number };
                IsInheritedCell = new DataGridViewCheckBoxCell() { Value = isInherited };
                IsOverwritingCell = new DataGridViewCheckBoxCell() { Value = IsOverwriting() };
                ParentNameCell = new DataGridViewTextBoxCell() { Value = parentName };
                ParameterNameCell = new DataGridViewTextBoxCell() { Value = parameter.Key };
                ValueCell = new DataGridViewTextBoxCell() { Value = BuildValueText() };
                CommentsCell = new DataGridViewTextBoxCell() { Value = string.Join(", ", parameter.Comments) };

                if (HasError)
                {
                    DefaultCellStyle.BackColor = Color.Red;
                }
                else if (isInherited)
                {
                    DefaultCellStyle.BackColor = Color.LightGray;
                }

                Cells.Add(NumberCell);
                Cells.Add(IsInheritedCell);
                Cells.Add(IsOverwritingCell);
                Cells.Add(ParentNameCell);
                Cells.Add(ParameterNameCell);
                Cells.Add(ValueCell);
                Cells.Add(CommentsCell);
            }

            public bool IsInherited()
            {
                return Convert.ToBoolean(IsInheritedCell.Value);
            }
            public bool IsOverwriting()
            {
                return OverwrittenRow != null;
            }
            public string GetParentName()
            {
                return Convert.ToString(ParentNameCell.Value);
            }

            private string BuildValueText()
            {
                string valueSeperator = string.Format("{0} ", TextRecources.EcfParameterView_ValueSeperator);
                if (!Parameter.IsUsingGroups())
                {
                    return string.Join(valueSeperator, Parameter.GetAllValues());
                }
                else
                {
                    return BuildValueGroupText(valueSeperator);
                }
            }
            private string BuildValueGroupText(string valueSeperator)
            {
                List<string> valueGroups = new List<string>();
                StringBuilder valueGroup = new StringBuilder();

                foreach (EcfValueGroup group in Parameter.ValueGroups)
                {
                    valueGroup.Clear();

                    valueGroup.Append(TitleRecources.Generic_Group);
                    valueGroup.Append(" ");
                    valueGroup.Append(Parameter.IndexOf(group) + 1);
                    valueGroup.Append(TextRecources.EcfParameterView_GroupSeperator);
                    valueGroup.Append(" ");
                    valueGroup.Append(string.Join(valueSeperator, group.Values));

                    valueGroups.Add(valueGroup.ToString());
                }
                return string.Join(Environment.NewLine, valueGroups);
            }
        }
    }
    public class EcfInfoView : EcfBaseView
    {
        private TableLayoutPanel View { get; } = new TableLayoutPanel();
        private InfoViewGroupBox<EcfBlock> ElementView { get; } = new InfoViewGroupBox<EcfBlock>(TitleRecources.EcfInfoView_ElementData);
        private InfoViewGroupBox<EcfParameter> ParameterView { get; } = new InfoViewGroupBox<EcfParameter>(TitleRecources.EcfInfoView_ParameterData);

        public EcfInfoView(string headline, EgsEcfFile file, ResizeableBorders mode) : base(headline, file, mode)
        {
            View.AutoSize = true;
            View.Dock = DockStyle.Fill;
            View.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;

            View.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1.0f));
            View.ColumnCount = 1;
            View.RowStyles.Add(new RowStyle(SizeType.Percent, 0.5f));
            View.RowStyles.Add(new RowStyle(SizeType.Percent, 0.5f));
            View.RowCount = 2;

            ElementView.AutoSize = true;
            ElementView.Dock = DockStyle.Fill;

            ParameterView.AutoSize = true;
            ParameterView.Dock = DockStyle.Fill;

            View.Controls.Add(ElementView, 0, 0);
            View.Controls.Add(ParameterView, 0, 1);
            Controls.Add(View);
        }

        // publics
        public void UpdateView(EcfStructureItem item)
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                if (item is EcfBlock block)
                {
                    RefreshViewInvoke(block);
                }
                else if (item is EcfParameter parameter)
                {
                    RefreshViewInvoke(parameter);
                }
                IsUpdating = false;
            }
        }
        public void UpdateView(EcfStructureItem item, EcfParameter parameter)
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                RefreshViewInvoke(item as EcfBlock);
                RefreshViewInvoke(parameter);
                IsUpdating = false;
            }
        }
        public void ShowSpecificItem(EcfStructureItem item)
        {
            if (item?.Parent is EcfBlock block && item is EcfParameter parameter)
            {
                UpdateView(block, parameter);
            }
            else
            {
                UpdateView(item);
            }
        }

        // privares
        private void RefreshViewInvoke(EcfBlock block)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    RefreshView(block);
                });
            }
            else
            {
                RefreshView(block);
            }
        }
        private void RefreshViewInvoke(EcfParameter parameter)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    RefreshView(parameter);
                });
            }
            else
            {
                RefreshView(parameter);
            }
        }
        private void RefreshView(EcfBlock block)
        {
            View.SuspendLayout();
            ElementView.Refresh(block);
            View.ResumeLayout();
        }
        private void RefreshView(EcfParameter parameter)
        {
            View.SuspendLayout();
            ParameterView.Refresh(parameter);
            View.ResumeLayout();
        }

        private class InfoViewGroupBox<T> : GroupBox where T : EcfStructureItem
        {
            private TreeView InfoList { get; } = new TreeView();

            private TreeNode ElementProperties { get; } = new TreeNode(TitleRecources.Generic_Properties);
            private TreeNode ParameterDefinitions { get; } = new TreeNode(TitleRecources.Generic_Definitions);

            private TreeNode AttributesNode { get; } = new TreeNode(TitleRecources.Generic_Attributes);
            private TreeNode CommentsNode { get; } = new TreeNode(TitleRecources.Generic_Comments);
            private TreeNode ErrorsNode { get; } = new TreeNode(TitleRecources.Generic_Errors);

            public InfoViewGroupBox(string header) : base()
            {
                Text = header;

                InfoList.Dock = DockStyle.Fill;
                InfoList.FullRowSelect = false;
                InfoList.HideSelection = true;
                InfoList.LabelEdit = false;
                InfoList.Scrollable = true;

                if (typeof(EcfBlock).IsAssignableFrom(typeof(T)))
                {
                    InfoList.Nodes.Add(ElementProperties);
                }
                else if (typeof(EcfParameter).IsAssignableFrom(typeof(T)))
                {
                    InfoList.Nodes.Add(ParameterDefinitions);
                }

                InfoList.Nodes.Add(AttributesNode);
                InfoList.Nodes.Add(CommentsNode);
                InfoList.Nodes.Add(ErrorsNode);

                Controls.Add(InfoList);
            }

            // publics
            public void Refresh(EcfBlock block)
            {
                InfoList.BeginUpdate();
                Clear();
                if (block != null)
                {
                    BuildElementPropertiesNode(block);
                    AttributesNode.Nodes.AddRange(block.Attributes.Select(attribute => new TreeNode(BuildAttributeEntry(attribute))).ToArray());
                    CommentsNode.Nodes.AddRange(block.Comments.Select(comment => new TreeNode(comment)).ToArray());
                    ErrorsNode.Nodes.AddRange(block.GetDeepErrorList(false).Select(error => new TreeNode(BuildErrorEntry(error))).ToArray());
                    InfoList.ExpandAll();
                }
                InfoList.EndUpdate();
            }
            public void Refresh(EcfParameter parameter)
            {
                InfoList.BeginUpdate();
                Clear();
                if (parameter != null)
                {
                    BuildParameterDefinitionNode(parameter);
                    AttributesNode.Nodes.AddRange(parameter.Attributes.Select(attribute => new TreeNode(BuildAttributeEntry(attribute))).ToArray());
                    CommentsNode.Nodes.AddRange(parameter.Comments.Select(comment => new TreeNode(comment)).ToArray());
                    ErrorsNode.Nodes.AddRange(parameter.GetDeepErrorList(false).Select(error => new TreeNode(BuildErrorEntry(error))).ToArray());
                    InfoList.ExpandAll();
                }
                InfoList.EndUpdate();
            }

            // privates
            private void Clear()
            {
                if (typeof(EcfBlock).IsAssignableFrom(typeof(T)))
                {
                    ElementProperties.Nodes.Clear();
                }
                else if (typeof(EcfParameter).IsAssignableFrom(typeof(T)))
                {
                    ParameterDefinitions.Nodes.Clear();
                }

                AttributesNode.Nodes.Clear();
                CommentsNode.Nodes.Clear();
                ErrorsNode.Nodes.Clear();
            }
            private void BuildElementPropertiesNode(EcfBlock block)
            {
                ElementProperties.Nodes.Add(BuildValueNode(TitleRecources.Generic_PreMark, block.PreMark, true));
                ElementProperties.Nodes.Add(BuildValueNode(TitleRecources.Generic_DataType, block.DataType, false));
                ElementProperties.Nodes.Add(BuildValueNode(TitleRecources.Generic_PostMark, block.PostMark, true));
                ElementProperties.Nodes.Add(BuildValueNode(TitleRecources.Generic_Inherited,
                    block.Inheritor != null ? block.Inheritor.BuildIdentification() : TitleRecources.Generic_No, false));
            }
            private void BuildParameterDefinitionNode(EcfParameter parameter)
            {
                if (parameter.Definition != null)
                {
                    ParameterDefinitions.Nodes.Add(BuildStateNode(TitleRecources.Generic_IsOptional, parameter.Definition?.IsOptional ?? false));
                    ParameterDefinitions.Nodes.Add(BuildValueNode(TitleRecources.Generic_Info, parameter.Definition?.Info ?? string.Empty, false));
                }
                else
                {
                    ParameterDefinitions.Nodes.Add(new TreeNode(TextRecources.EcfInfoView_NoDefinition));
                }
            }
            private TreeNode BuildStateNode(string key, bool state)
            {
                StringBuilder entry = new StringBuilder(key);
                entry.Append(": ");
                entry.Append(state ? TitleRecources.Generic_Yes : TitleRecources.Generic_No);
                return new TreeNode(entry.ToString());
            }
            private TreeNode BuildValueNode(string key, string value, bool valueEscaped)
            {
                StringBuilder entry = new StringBuilder(key);
                entry.Append(": ");
                if (valueEscaped) { entry.Append("\""); }
                entry.Append(value ?? TitleRecources.Generic_Replacement_Empty);
                if (valueEscaped) { entry.Append("\""); }
                return new TreeNode(entry.ToString());
            }
            private string BuildAttributeEntry(EcfAttribute attribute)
            {
                StringBuilder entry = new StringBuilder(attribute.Key);
                if (attribute.HasValue())
                {
                    entry.Append(": ");
                    entry.Append(string.Join(", ", attribute.GetAllValues()));
                }
                return entry.ToString();
            }
            private string BuildErrorEntry(EcfError error)
            {
                StringBuilder entry = new StringBuilder(GetLocalizedEnum(error.Type));
                entry.Append(": ");
                entry.Append(error.Info);
                return entry.ToString();
            }
        }
    }
    public class EcfErrorView : EcfBaseView
    {
        public event EventHandler ShowInEditorClicked;
        public event EventHandler ShowInFileClicked;

        public EcfError SelectedError { get; private set; } = null;

        private Panel View { get; } = new Panel();
        private EcfToolContainer ToolContainer { get; } = new EcfToolContainer();
        private EcfSorter ErrorSorter { get; }
        private DataGridView Grid { get; } = new DataGridView();
        private ContextMenuStrip GridMenu { get; } = new ContextMenuStrip();
        private ToolStripMenuItem GridMenuItemShowInEditor { get; } = new ToolStripMenuItem();
        private ToolStripMenuItem GridMenuItemShowInFile { get; } = new ToolStripMenuItem();
        private List<EcfErrorRow> ErrorRows { get; } = new List<EcfErrorRow>();

        private DataGridViewTextBoxColumn ErrorNumberColumn { get; } = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn ErrorGroupColumn { get; } = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn ErrorTypeColumn { get; } = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn ElementNameColumn { get; } = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn LineNumberColumn { get; } = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn ErrorInfoColumn { get; } = new DataGridViewTextBoxColumn();

        public EcfErrorView(string headline, EgsEcfFile file, ResizeableBorders mode, VisibleItemCount sorterItemCount) : base(headline, file, mode)
        {
            ErrorSorter = new EcfSorter(
                TextRecources.EcfErrorView_ToolTip_ErrorCountSelector,
                TextRecources.EcfErrorView_ToolTip_ErrorGroupSelector,
                TextRecources.EcfErrorView_ToolTip_ErrorSorterDirection,
                TextRecources.EcfErrorView_ToolTip_ErrorSorterOriginOrder,
                TextRecources.EcfErrorView_ToolTip_ErrorSorterAlphabeticOrder,
                sorterItemCount);
            ErrorSorter.SortingUserChanged += ErrorSorter_SortingUserChanged;

            View.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            View.Dock = DockStyle.Fill;

            ToolContainer.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            ToolContainer.Dock = DockStyle.Top;

            InitGridView();

            GridMenuItemShowInEditor.Text = TitleRecources.EcfErrorView_ShowInEditor;
            GridMenuItemShowInEditor.Image = IconRecources.Icon_ShowInEditor;
            GridMenuItemShowInEditor.Click += (sender, evt) => ShowInEditorClicked?.Invoke(sender, evt);

            GridMenuItemShowInFile.Text = TitleRecources.EcfErrorView_ShowInFile;
            GridMenuItemShowInFile.Image = IconRecources.Icon_ShowInFile;
            GridMenuItemShowInFile.Click += (sender, evt) => ShowInFileClicked?.Invoke(sender, evt);

            GridMenu.Items.Add(GridMenuItemShowInEditor);
            GridMenu.Items.Add(GridMenuItemShowInFile);

            ToolContainer.Add(ErrorSorter);
            View.Controls.Add(Grid);
            View.Controls.Add(ToolContainer);
            Controls.Add(View);
        }

        // events
        private void ErrorSorter_SortingUserChanged(object sender, EventArgs evt)
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                RefreshViewInvoke();
                IsUpdating = false;
            }
        }
        private void Grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs evt)
        {
            if (evt.RowIndex > -1 && evt.ColumnIndex > -1)
            {
                Grid.ClearSelection();
                if (Grid.Rows[evt.RowIndex] is EcfErrorRow row)
                {
                    row.Cells[evt.ColumnIndex].Selected = true;
                    SelectedError = row.Error;
                    if (evt.Button == MouseButtons.Right)
                    {
                        GridMenuItemShowInFile.Visible = row.Error.IsFromParsing();
                        Point cellLocation = Grid.GetCellDisplayRectangle(evt.ColumnIndex, evt.RowIndex, false).Location;
                        GridMenu.Show(Grid, new Point(cellLocation.X + evt.X, cellLocation.Y + evt.Y));
                    }
                }

            }
        }

        // publics
        public void UpdateView()
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                BuildGridViewRows();
                UpdateSorterInvoke();
                RefreshViewInvoke();
                IsUpdating = false;
            }
        }

        // privates
        private void InitGridView()
        {
            Grid.AllowUserToAddRows = false;
            Grid.AllowUserToDeleteRows = false;
            Grid.AllowDrop = false;
            Grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            Grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            Grid.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Grid.Dock = DockStyle.Fill;
            Grid.EditMode = DataGridViewEditMode.EditProgrammatically;

            ErrorNumberColumn.HeaderText = TitleRecources.EcfErrorView_ErrorNumberColumn;
            ErrorGroupColumn.HeaderText = TitleRecources.EcfErrorView_ErrorGroupColumn;
            ErrorTypeColumn.HeaderText = TitleRecources.Generic_Type;
            ElementNameColumn.HeaderText = TitleRecources.Generic_Name;
            LineNumberColumn.HeaderText = TitleRecources.Generic_LineNumber;
            ErrorInfoColumn.HeaderText = TitleRecources.Generic_Info;

            Grid.CellMouseClick += Grid_CellMouseClick;

            Grid.Columns.Add(ErrorNumberColumn);
            Grid.Columns.Add(ErrorGroupColumn);
            Grid.Columns.Add(ErrorTypeColumn);
            Grid.Columns.Add(ElementNameColumn);
            Grid.Columns.Add(LineNumberColumn);
            Grid.Columns.Add(ErrorInfoColumn);
        }
        private void BuildGridViewRows()
        {
            ErrorRows.Clear();
            List<EcfError> preSortedErrors = File.GetErrorList().OrderBy(error => !error.IsFromParsing()).ThenBy(error => error.LineInFile).ToList();
            foreach (EcfError error in preSortedErrors)
            {
                ErrorRows.Add(new EcfErrorRow(preSortedErrors.IndexOf(error) + 1, error));
            }
        }
        private void UpdateSorterInvoke()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    ErrorSorter.SetOverallItemCount(ErrorRows.Count);
                });
            }
            else
            {
                ErrorSorter.SetOverallItemCount(ErrorRows.Count);
            }
        }
        private void RefreshViewInvoke()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    RefreshView();
                });
            }
            else
            {
                RefreshView();
            }
        }
        private void RefreshView()
        {
            Grid.SuspendLayout();
            Grid.Rows.Clear();
            Grid.Rows.AddRange(ErrorRows.Skip(ErrorSorter.ItemCount * (ErrorSorter.ItemGroup - 1)).Take(ErrorSorter.ItemCount).ToArray());
            Grid.Sort(GetSortingColumn(ErrorSorter), ErrorSorter.IsAscending ? ListSortDirection.Ascending : ListSortDirection.Descending);
            Grid.AutoResizeColumns();
            Grid.AutoResizeRows();
            Grid.ClearSelection();
            Grid.ResumeLayout();
            Text = string.Format("{0} - {1} {2} - {3} {4} - {5} {6} - {7} {8}", ViewName,
                ErrorRows.Count, TitleRecources.Generic_Errors,
                ErrorRows.Count(error => error.Error.Group == EcfErrorGroups.Fatal), TitleRecources.EcfErrorView_FatalErrors,
                ErrorRows.Count(error => error.Error.Group == EcfErrorGroups.Parsing), TitleRecources.EcfErrorView_ParsingErrors,
                ErrorRows.Count(error => error.Error.Group == EcfErrorGroups.Editing), TitleRecources.EcfErrorView_EditingErrors);
        }
        private DataGridViewColumn GetSortingColumn(EcfSorter sorter)
        {
            switch (sorter.SortingType)
            {
                case SortingTypes.Alphabetical: return ElementNameColumn;
                default: return ErrorNumberColumn;
            }
        }

        private class EcfErrorRow : DataGridViewRow
        {
            public EcfError Error { get; } = null;

            private DataGridViewTextBoxCell ErrorNumberCell { get; }
            private DataGridViewTextBoxCell ErrorGroupCell { get; }
            private DataGridViewTextBoxCell LineNumberCell { get; }
            private DataGridViewTextBoxCell ElementNameCell { get; }
            private DataGridViewTextBoxCell ErrorTypeCell { get; }
            private DataGridViewTextBoxCell ErrorInfoCell { get; }

            public EcfErrorRow(int number, EcfError error) : base()
            {
                Error = error;

                ErrorNumberCell = new DataGridViewTextBoxCell() { Value = number };
                ErrorGroupCell = new DataGridViewTextBoxCell() { Value = GetLocalizedEnum(error.Group) };
                ErrorTypeCell = new DataGridViewTextBoxCell() { Value = GetLocalizedEnum(error.Type) };
                ElementNameCell = new DataGridViewTextBoxCell() { Value = error.Item?.GetFullName() ?? string.Empty };
                LineNumberCell = new DataGridViewTextBoxCell() { Value = error.IsFromParsing() ? error.LineInFile.ToString() : string.Empty };
                ErrorInfoCell = new DataGridViewTextBoxCell() { Value = error.Info };

                Cells.Add(ErrorNumberCell);
                Cells.Add(ErrorGroupCell);
                Cells.Add(ErrorTypeCell);
                Cells.Add(ElementNameCell);
                Cells.Add(LineNumberCell);
                Cells.Add(ErrorInfoCell);
            }
        }
    }
}
    
// specific tool controls
namespace EcfFileViewTools
{
    public class EcfToolContainer : FlowLayoutPanel
    {
        public EcfToolContainer()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Margin = new Padding(Margin.Left, 0, Margin.Right, 0);
        }

        public void Add(EcfToolBox toolGroup)
        {
            Controls.Add(toolGroup);
        }
    }
    public abstract class EcfToolBox : FlowLayoutPanel
    {
        public EcfToolBox() : base()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Margin = new Padding(Margin.Left, 0, Margin.Right, 0);
        }
        protected Control Add(Control control)
        {
            control.AutoSize = true;
            control.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            control.Dock = DockStyle.Fill;
            Controls.Add(control);
            return control;
        }
    }
    public class EcfBasicFileOperations : EcfToolBox
    {
        public event EventHandler NewFileClicked;
        public event EventHandler OpenFileClicked;
        public event EventHandler ReloadFileClicked;
        public event EventHandler SaveFileClicked;
        public event EventHandler SaveAsFileClicked;
        public event EventHandler SaveAsFilteredFileClicked;
        public event EventHandler SaveAllFilesClicked;
        public event EventHandler CloseFileClicked;
        public event EventHandler CloseAllFilesClicked;

        public EcfBasicFileOperations() : base()
        {
            Add(new EcfToolBarButton(TextRecources.EgsEcfEditorApp_ToolTip_New, IconRecources.Icon_NewFile, null))
                .Click += (sender, evt) => NewFileClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EgsEcfEditorApp_ToolTip_Open, IconRecources.Icon_OpenFile, null))
                .Click += (sender, evt) => OpenFileClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EgsEcfEditorApp_ToolTip_Reload, IconRecources.Icon_ReloadFile, null))
                .Click += (sender, evt) => ReloadFileClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EgsEcfEditorApp_ToolTip_Save, IconRecources.Icon_SaveFile, null))
                .Click += (sender, evt) => SaveFileClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EgsEcfEditorApp_ToolTip_SaveAs, IconRecources.Icon_SaveAsFile, null))
                .Click += (sender, evt) => SaveAsFileClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EgsEcfEditorApp_ToolTip_SaveAsFiltered, IconRecources.Icon_SaveAsFilteredFile, null))
                .Click += (sender, evt) => SaveAsFilteredFileClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EgsEcfEditorApp_ToolTip_SaveAll, IconRecources.Icon_SaveAllFiles, null))
                .Click += (sender, evt) => SaveAllFilesClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EgsEcfEditorApp_ToolTip_Close, IconRecources.Icon_CloseFile, null))
                .Click += (sender, evt) => CloseFileClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EgsEcfEditorApp_ToolTip_CloseAll, IconRecources.Icon_CloseAllFiles, null))
                .Click += (sender, evt) => CloseAllFilesClicked?.Invoke(sender, evt);
        }
    }
    public class EcfExtendedFileOperations : EcfToolBox
    {
        public event EventHandler ReloadDefinitionsClicked;
        public event EventHandler CheckDefinitionClicked;
        public event EventHandler CompareFilesClicked;
        public event EventHandler MergeFilesClicked;

        public event EventHandler BuildTechTreePreviewClicked;

        public EcfExtendedFileOperations() : base()
        {
            Add(new EcfToolBarButton(TextRecources.EgsEcfEditorApp_ToolTip_CompareFiles, IconRecources.Icon_Compare, null))
                .Click += (sender, evt) => CompareFilesClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EgsEcfEditorApp_ToolTip_MergeFiles, IconRecources.Icon_Merge, null))
                .Click += (sender, evt) => MergeFilesClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EgsEcfEditorApp_ToolTip_BuildTechTreePreview, IconRecources.Icon_BuildTechTreePreview, null))
                .Click += (sender, evt) => BuildTechTreePreviewClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EgsEcfEditorApp_ToolTip_ReloadDefinitions, IconRecources.Icon_ReloadDefinitions, null))
                .Click += (sender, evt) => ReloadDefinitionsClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EgsEcfEditorApp_ToolTip_CheckDefinition, IconRecources.Icon_CheckDefinition, null))
                .Click += (sender, evt) => CheckDefinitionClicked?.Invoke(sender, evt);
        }
    }
    public class EcfSettingOperations : EcfToolBox
    {
        public event EventHandler GameVersionClicked;
        public event EventHandler OpenSettingsDialogClicked;

        private EcfToolBarLabel GameVersion { get; }

        private ContextMenuStrip GameVersionMenu { get; } = new ContextMenuStrip();

        public EcfSettingOperations() : base()
        {
            GameVersion = Add(new EcfToolBarLabel("", true)) as EcfToolBarLabel;
            Add(new EcfToolBarButton(TextRecources.EgsEcfEditorApp_ToolTip_OpenSettingsDialog, IconRecources.Icon_Settings, null))
                .Click += (sender, evt) => OpenSettingsDialogClicked?.Invoke(sender, evt);

            GameVersion.MouseClick += GameVersion_MouseClick;
            GameVersionMenu.ItemClicked += GameVersionMenu_ItemClicked;
        }

        // events
        private void GameVersionMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs evt)
        {
            SetGameVersionInvoked(evt.ClickedItem.Text);
            GameVersionClicked?.Invoke(evt.ClickedItem.Text, null);
        }
        private void GameVersion_MouseClick(object sender, MouseEventArgs evt)
        {
            GameVersionMenu.Items.Clear();
            GameVersionMenu.Items.AddRange(GetGameModes().Select(mode => new ToolStripMenuItem(mode)).ToArray());
            GameVersionMenu.Show(GameVersion, evt.Location);
        }

        // publics
        public void SetGameVersion(string version)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    SetGameVersionInvoked(version);
                });
            }
            else
            {
                SetGameVersionInvoked(version);
            }
        }

        // private 
        private void SetGameVersionInvoked(string version)
        {
            GameVersion.Text = version;
        }
    }
    public class EcfContentOperations : EcfToolBox
    {
        public event EventHandler UndoClicked;
        public event EventHandler RedoClicked;
        public event EventHandler AddClicked;
        public event EventHandler RemoveClicked;
        public event EventHandler ChangeSimpleClicked;
        public event EventHandler ChangeComplexClicked;
        public event EventHandler MoveUpClicked;
        public event EventHandler MoveDownClicked;
        public event EventHandler CopyClicked;
        public event EventHandler PasteClicked;

        public EcfContentOperations() : base()
        {
            Add(new EcfToolBarButton(TextRecources.EcfTabPage_ToolTip_Undo, IconRecources.Icon_Undo, null))
                .Click += (sender, evt) => UndoClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EcfTabPage_ToolTip_Redo, IconRecources.Icon_Redo, null))
                .Click += (sender, evt) => RedoClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EcfTabPage_ToolTip_Add, IconRecources.Icon_Add, null))
                .Click += (sender, evt) => AddClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EcfTabPage_ToolTip_Remove, IconRecources.Icon_Remove, null))
                .Click += (sender, evt) => RemoveClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EcfTabPage_ToolTip_ChangeSimple, IconRecources.Icon_ChangeSimple, null))
                .Click += (sender, evt) => ChangeSimpleClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EcfTabPage_ToolTip_ChangeComplex, IconRecources.Icon_ChangeComplex, null))
                .Click += (sender, evt) => ChangeComplexClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EcfTabPage_ToolTip_MoveUp, IconRecources.Icon_MoveUp, null))
                .Click += (sender, evt) => MoveUpClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EcfTabPage_ToolTip_MoveDown, IconRecources.Icon_MoveDown, null))
                .Click += (sender, evt) => MoveDownClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EcfTabPage_ToolTip_CopyElements, IconRecources.Icon_Copy, null))
                .Click += (sender, evt) => CopyClicked?.Invoke(sender, evt);
            Add(new EcfToolBarButton(TextRecources.EcfTabPage_ToolTip_PasteElements, IconRecources.Icon_Paste, null))
                .Click += (sender, evt) => PasteClicked?.Invoke(sender, evt);
        }
    }
    public class EcfFilterControl : EcfToolBox
    {
        public event EventHandler ApplyFilterClicked;
        public event EventHandler ClearFilterClicked;

        public EcfStructureItem SpecificItem { get; private set; } = null;

        private List<EcfBaseFilter> AttachedFilters { get; } = new List<EcfBaseFilter>();

        private EcfToolBarButton ApplyFilterButton { get; } = new EcfToolBarButton(TextRecources.EcfTabPage_ToolTip_FilterApplyButton, IconRecources.Icon_ApplyFilter, null);
        private EcfToolBarButton ClearFilterButton { get; } = new EcfToolBarButton(TextRecources.EcfTabPage_ToolTip_FilterClearButton, IconRecources.Icon_ClearFilter, null);

        public EcfFilterControl(string gameMode, string fileType) : base()
        {
            Add(new EcfToolBarLabel(gameMode, true));
            Add(new EcfToolBarLabel(fileType, true));

            Add(ApplyFilterButton).Click += ApplyFilterButton_Click;
            Add(ClearFilterButton).Click += ClearFilterButton_Click;
        }

        // events
        public void ApplyFilterButton_Click(object sender, EventArgs evt)
        {
            ApplyFilterClicked?.Invoke(sender, evt);
        }
        public void ClearFilterButton_Click(object sender, EventArgs evt)
        {
            SetSpecificItem(null);
            AttachedFilters.ForEach(filter => filter.Reset());
            ClearFilterClicked?.Invoke(sender, evt);
        }

        // publics
        public void Add(EcfBaseFilter filter)
        {
            AttachedFilters.Add(filter);
        }
        public void Remove(EcfBaseFilter filter)
        {
            AttachedFilters.Remove(filter);
        }
        public void SetSpecificItem(EcfStructureItem item)
        {
            SpecificItem = item;
            if (SpecificItem != null) { Disable(); } else { Enable(); }
        }
        public void Disable()
        {
            ApplyFilterButton.Enabled = false;
            AttachedFilters.ForEach(filter => filter.Disable());
        }
        public void Enable()
        {
            ApplyFilterButton.Enabled = true;
            AttachedFilters.ForEach(filter => filter.Enable());
        }
    }
    public abstract class EcfBaseFilter : EcfToolBox
    {
        public event EventHandler ApplyFilterRequested;

        public string IsLikeText { get; private set; } = string.Empty;
        public ReadOnlyCollection<string> CheckedItems { get; }
        public ReadOnlyCollection<string> UncheckedItems { get; }

        private EcfToolBarTextBox LikeInput { get; }
        protected EcfToolBarCheckComboBox ItemSelector { get; }

        private List<string> InternalCheckedItems { get; } = new List<string>();
        private List<string> InternalUncheckedItems { get; } = new List<string>();

        public EcfBaseFilter(string likeToolTip, string typeName, string itemSelectorTooltip) : base()
        {
            CheckedItems = InternalCheckedItems.AsReadOnly();
            UncheckedItems = InternalUncheckedItems.AsReadOnly();

            LikeInput = (EcfToolBarTextBox)Add(new EcfToolBarTextBox(likeToolTip));
            ItemSelector = (EcfToolBarCheckComboBox)Add(new EcfToolBarCheckComboBox(typeName, itemSelectorTooltip));

            LikeInput.KeyPress += LikeInput_KeyPress;
            LikeInput.TextChanged += LikeInput_TextChanged;
            ItemSelector.SelectionChangeCommitted += ItemSelector_SelectionChangeCommitted;
        }

        // events
        private void LikeInput_KeyPress(object sender, KeyPressEventArgs evt)
        {
            if (evt.KeyChar == (char)Keys.Enter)
            {
                ApplyFilterRequested?.Invoke(sender, evt);
                evt.Handled = true;
            }
        }
        private void LikeInput_TextChanged(object sender, EventArgs evt)
        {
            IsLikeText = LikeInput.Text;
        }
        private void ItemSelector_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            LoadItems();
        }

        public void Reset()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    ResetInvoked();
                });
            }
            else
            {
                ResetInvoked();
            }
        }
        public void Enable()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    EnableInvoked();
                });
            }
            else
            {
                EnableInvoked();
            }
        }
        public void Disable()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    DisableInvoked();
                });
            }
            else
            {
                DisableInvoked();
            }
        }
        public bool IsLike(string text)
        {
            if (string.IsNullOrEmpty(IsLikeText)) { return true; }
            if (string.IsNullOrEmpty(text)) { return false; }
            return text.Contains(IsLikeText);
        }
        public virtual bool AnyFilterSet()
        {
            return !IsLikeText.Equals(string.Empty) || UncheckedItems.Count > 0;
        }

        protected virtual void ResetInvoked()
        {
            LikeInput.Clear();
            IsLikeText = string.Empty;
            ItemSelector.Reset();
            LoadItems();
        }
        protected virtual void EnableInvoked()
        {
            LikeInput.Enabled = true;
            ItemSelector.Enabled = true;
        }
        protected virtual void DisableInvoked()
        {
            LikeInput.Enabled = false;
            ItemSelector.Enabled = false;
        }

        private void LoadItems()
        {
            InternalCheckedItems.Clear();
            InternalUncheckedItems.Clear();
            InternalCheckedItems.AddRange(ItemSelector.GetCheckedItems().Select(item => item.Id));
            InternalUncheckedItems.AddRange(ItemSelector.GetUncheckedItems().Select(item => item.Id));
        }
    }
    public class EcfTreeFilter : EcfBaseFilter
    {
        public bool IsCommentsActive { get; private set; } = false;
        public bool IsParametersActive { get; private set; } = false;
        public bool IsDataBlocksActive { get; private set; } = false;
        public ErrorDisplayModes ErrorDisplayMode { get; private set; } = ErrorDisplayModes.ShowAllItems;

        private EcfToolBarTreeStateCheckBox ErrorDisplaySelector { get; } =
            new EcfToolBarTreeStateCheckBox(
                TextRecources.EcfTabPage_ToolTip_TreeErrorDisplayModeSelector, IconRecources.Icon_ShowAllItems,
                IconRecources.Icon_ShowOnlyFaultyItems, IconRecources.Icon_ShowOnlyNonFaultyItems);

        public enum ErrorDisplayModes
        {
            ShowAllItems,
            ShowOnlyFaultyItems,
            ShowOnlyNonFaultyItems,
        }
        private enum SelectableItems
        {
            Comments,
            Parameters,
            DataBlocks,
        }

        public EcfTreeFilter(bool commentInitActive, bool parameterInitActive, bool dataBlockInitActive)
            : base(TextRecources.EcfTabPage_ToolTip_TreeLikeInput, TitleRecources.EcfTreeView_FilterSelector_Elements, TextRecources.EcfTabPage_ToolTip_TreeItemTypeSelector)
        {
            List<CheckableItem> itemList = new List<CheckableItem>
            {
                new CheckableItem(SelectableItems.Comments.ToString(), GetLocalizedEnum(SelectableItems.Comments), commentInitActive),
                new CheckableItem(SelectableItems.Parameters.ToString(), GetLocalizedEnum(SelectableItems.Parameters), parameterInitActive),
                new CheckableItem(SelectableItems.DataBlocks.ToString(), GetLocalizedEnum(SelectableItems.DataBlocks), dataBlockInitActive)
            };
            ItemSelector.SetItems(itemList.OrderBy(item => item.Display).ToList());

            ItemSelector.SelectionChangeCommitted += ItemSelector_SelectionChangeCommitted;

            Add(ErrorDisplaySelector).Click += ChangeErrorDisplayButton_Click;

            Reset();
        }

        // events
        private void ItemSelector_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            LoadItemSelectionStates();
        }
        private void ChangeErrorDisplayButton_Click(object sender, EventArgs evt)
        {
            LoadErrrorDisplayState((sender as EcfToolBarTreeStateCheckBox).CheckState);
        }

        // publics
        public override bool AnyFilterSet()
        {
            return ErrorDisplayMode != ErrorDisplayModes.ShowAllItems || base.AnyFilterSet();
        }

        // privates
        private void LoadItemSelectionStates()
        {
            IsCommentsActive = ItemSelector.IsItemChecked(SelectableItems.Comments.ToString());
            IsParametersActive = ItemSelector.IsItemChecked(SelectableItems.Parameters.ToString());
            IsDataBlocksActive = ItemSelector.IsItemChecked(SelectableItems.DataBlocks.ToString());
        }
        private void LoadErrrorDisplayState(CheckState state)
        {
            switch (state)
            {
                case CheckState.Checked: ErrorDisplayMode = ErrorDisplayModes.ShowOnlyFaultyItems; break;
                case CheckState.Unchecked: ErrorDisplayMode = ErrorDisplayModes.ShowOnlyNonFaultyItems; break;
                default: ErrorDisplayMode = ErrorDisplayModes.ShowAllItems; break;
            }
        }
        protected override void ResetInvoked()
        {
            base.ResetInvoked();
            LoadItemSelectionStates();
            ErrorDisplaySelector.Reset();
            LoadErrrorDisplayState(ErrorDisplaySelector.CheckState);
        }
        protected override void EnableInvoked()
        {
            base.EnableInvoked();
            ErrorDisplaySelector.Enabled = true;
        }
        protected override void DisableInvoked()
        {
            base.DisableInvoked();
            ErrorDisplaySelector.Enabled = false;
        }
    }
    public class EcfParameterFilter : EcfBaseFilter
    {
        public EcfParameterFilter(List<string> items) : base(
            TextRecources.EcfTabPage_ToolTip_ParameterLikeInput, TitleRecources.Generic_Parameters, TextRecources.EcfTabPage_ToolTip_ParameterSelector)
        {
            ItemSelector.SetItems(items.OrderBy(item => item).Select(item => new CheckableItem(item, true)).ToList());

            Reset();
        }

        public bool IsParameterVisible(EcfParameter parameter)
        {
            return (parameter.ContainsError(EcfErrors.ParameterUnknown) || CheckedItems.Contains(parameter.Key))
                && IsParameterValueLike(parameter.GetAllValues(), IsLikeText);
        }

        private bool IsParameterValueLike(ReadOnlyCollection<string> values, string isLike)
        {
            if (string.IsNullOrEmpty(isLike) || values.Count < 1) { return true; }
            return values.Any(value => value.Contains(isLike));
        }
    }
    public class EcfSorter : EcfToolBox
    {
        public event EventHandler SortingUserChanged;

        private ComboBox ItemCountSelector { get; }
        private NumericUpDown ItemGroupSelector { get; }
        private CheckBox DirectionSelector { get; }
        private RadioButton OriginOrderSelector { get; }
        private RadioButton AlphabeticOrderSelector { get; }

        public int ItemCount { get; private set; } = 100;
        public int ItemGroup { get; private set; } = 1;
        public bool IsAscending { get; private set; } = true;
        public SortingTypes SortingType { get; private set; } = SortingTypes.Original;

        private int OverallItemCount { get; set; } = 100;
        private bool IsUpdating { get; set; } = false;

        public enum SortingTypes
        {
            Original,
            Alphabetical,
        }
        public enum VisibleItemCount
        {
            Ten = 10,
            TwentyFive = 25,
            Fifty = 50,
            OneHundred = 100,
            TwoHundredAndFifty = 250,
            FiveHundred = 500,
        }

        public EcfSorter(string itemCountSelectorTooltip, string itemGroupSelectorTooltip,
            string directionToolTip, string originToolTip, string aplhabeticToolTip,
            VisibleItemCount count) : base()
        {
            ItemCountSelector = (EcfToolBarComboBox)Add(new EcfToolBarComboBox(itemCountSelectorTooltip));
            ItemGroupSelector = (EcfToolBarNumericUpDown)Add(new EcfToolBarNumericUpDown(itemGroupSelectorTooltip));
            DirectionSelector = (EcfToolBarCheckBox)Add(new EcfToolBarCheckBox(directionToolTip, IconRecources.Icon_ChangeSortDirection, null));
            OriginOrderSelector = (EcfToolBarRadioButton)Add(new EcfToolBarRadioButton(originToolTip, IconRecources.Icon_NumericSorting, null));
            AlphabeticOrderSelector = (EcfToolBarRadioButton)Add(new EcfToolBarRadioButton(aplhabeticToolTip, IconRecources.Icon_AlphabeticSorting, null));

            ItemCountSelector.Items.AddRange(Enum.GetValues(typeof(VisibleItemCount)).Cast<VisibleItemCount>().Select(value => Convert.ToInt32(value).ToString()).ToArray());
            ItemCount = Convert.ToInt32(count);
            ItemCountSelector.SelectedItem = ItemCount.ToString();

            ItemCountSelector.Width = ItemCountSelector.Items.Cast<string>().Max(x => TextRenderer.MeasureText(x, ItemCountSelector.Font).Width) + SystemInformation.VerticalScrollBarWidth;
            ItemGroupSelector.Minimum = ItemGroup;
            UpdateItemGroupSelector();
            OriginOrderSelector.Checked = true;

            ItemCountSelector.SelectionChangeCommitted += ItemCountSelector_SelectionChangeCommitted;
            ItemGroupSelector.ValueChanged += ItemGroupSelector_ValueChanged;
            DirectionSelector.Click += DirectionSelector_Click;
            OriginOrderSelector.CheckedChanged += OriginOrderSelector_CheckedChanged;
            AlphabeticOrderSelector.CheckedChanged += AlphabeticOrderSelector_CheckedChanged;
        }

        // events
        private void ItemCountSelector_SelectionChangeCommitted(object sender, EventArgs evt)
        {
            ItemCount = Convert.ToInt32(ItemCountSelector.SelectedItem);
            UpdateItemGroupSelector();
            if (!IsUpdating)
            {
                SortingUserChanged?.Invoke(sender, evt);
            }
        }
        private void ItemGroupSelector_ValueChanged(object sender, EventArgs evt)
        {
            ItemGroup = Convert.ToInt32(ItemGroupSelector.Value);
            if (!IsUpdating)
            {
                SortingUserChanged?.Invoke(sender, evt);
            }
        }
        private void DirectionSelector_Click(object sender, EventArgs evt)
        {
            IsAscending = !DirectionSelector.Checked;
            SortingUserChanged?.Invoke(sender, evt);
        }
        private void OriginOrderSelector_CheckedChanged(object sender, EventArgs evt)
        {
            if (!IsUpdating)
            {
                if (OriginOrderSelector.Checked)
                {
                    SortingType = SortingTypes.Original;
                    SortingUserChanged?.Invoke(sender, evt);
                }
            }
        }
        private void AlphabeticOrderSelector_CheckedChanged(object sender, EventArgs evt)
        {
            if (!IsUpdating)
            {
                if (AlphabeticOrderSelector.Checked)
                {
                    SortingType = SortingTypes.Alphabetical;
                    SortingUserChanged?.Invoke(sender, evt);
                }
            }
        }

        // public
        public void SetOverallItemCount(int overallItemCount)
        {
            OverallItemCount = overallItemCount;
            UpdateItemGroupSelector();
        }
        public void SetItemCount(VisibleItemCount count)
        {
            ItemCount = Convert.ToInt32(count);
            ItemCountSelector.SelectedItem = ItemCount.ToString();
            UpdateItemGroupSelector();
        }

        // privates
        private void UpdateItemGroupSelector()
        {
            IsUpdating = true;
            int selectedValue = (int)ItemGroupSelector.Value;
            ItemGroupSelector.Maximum = Math.Max((int)Math.Ceiling(OverallItemCount / (double)ItemCount), 1);
            int biggestGroup = OverallItemCount / Convert.ToInt32(ItemCountSelector.Items[0]);
            ItemGroupSelector.Width = TextRenderer.MeasureText(biggestGroup.ToString(), ItemGroupSelector.Font).Width + SystemInformation.VerticalScrollBarWidth;

            if (selectedValue < ItemGroupSelector.Minimum)
            {
                ItemGroupSelector.Value = ItemGroupSelector.Minimum;
            }
            else if (selectedValue > ItemGroupSelector.Maximum)
            {
                ItemGroupSelector.Value = ItemGroupSelector.Maximum;
            }
            else
            {
                ItemGroupSelector.Value = selectedValue;
            }
            IsUpdating = false;
        }
    }
}

    // generic tool controls
namespace EcfToolBarControls
{
    public class EcfToolBarCheckComboBox : Panel
    {
        public event EventHandler SelectionChangeCommitted;

        private bool IsResultBoxUnderCursor { get; set; } = false;

        private ToolTip BoxToolTip { get; } = new ToolTip();
        private TextBox ResultBox { get; } = new TextBox();
        private DropDownButton DropButton { get; }
        private DropDownList ItemList { get; }

        public string ToolTipText { get; }
        public string LocalisedOf { get; }
        public string LocalisedName { get; }
        public int MaxDropDownItems { get; set; } = 10;

        public EcfToolBarCheckComboBox(string TypeText, string toolTip)
        {
            ToolTipText = toolTip;
            LocalisedOf = TextRecources.Generic_Of;
            LocalisedName = TypeText;

            ResultBox.ReadOnly = true;
            ResultBox.Margin = new Padding(0);
            ResultBox.Location = new Point(0, 0);

            DropButton = new DropDownButton(ResultBox.Height);

            ItemList = new DropDownList(TextRecources.Generic_ChangeAll)
            {
                AnchorControl = this
            };

            ResultBox.MouseHover += ResultBox_MouseHover;
            ResultBox.Click += ResultBox_Click;
            ResultBox.MouseEnter += ResultBox_MouseEnter;
            ResultBox.MouseLeave += ResultBox_MouseLeave;
            DropButton.DropStateChanged += DropButton_DropStateChanged;
            ItemList.ItemStateChanged += ItemList_ItemStateChanged;
            ItemList.DropDownFocusLost += ItemList_DropDownFocusLost;

            Controls.Add(ResultBox);
            Controls.Add(DropButton);
        }

        // events
        private void ResultBox_MouseHover(object sender, EventArgs evt)
        {
            BoxToolTip.SetToolTip(ResultBox, ToolTipText);
        }
        private void ResultBox_Click(object sender, EventArgs evt)
        {
            DropButton.Switch();
        }
        private void ResultBox_MouseEnter(object sender, EventArgs evt)
        {
            IsResultBoxUnderCursor = true;
        }
        private void ResultBox_MouseLeave(object sender, EventArgs evt)
        {
            IsResultBoxUnderCursor = false;
        }
        private void ItemList_DropDownFocusLost(object sender, EventArgs evt)
        {
            if (!DropButton.IsUnderCursor && !IsResultBoxUnderCursor)
            {
                DropButton.Reset();
            }
        }
        private void DropButton_DropStateChanged(object sender, EventArgs evt)
        {
            if (DropButton.State == ComboBoxState.Pressed)
            {
                ItemList.ShowPopup(this);
            }
            else
            {
                ItemList.Hide();
                SelectionChangeCommitted?.Invoke(this, null);
            }
        }
        private void ItemList_ItemStateChanged(object sender, ItemCheckEventArgs evt)
        {
            UpdateResult();
        }

        // private
        private void UpdateResult()
        {
            ResultBox.Text = ToString();
            int width = TextRenderer.MeasureText(ResultBox.Text, ResultBox.Font).Width;
            if (width > ResultBox.Width)
            {
                ResultBox.Width = width;
                DropButton.Location = new Point(width, 0);
                Size = new Size(width + DropButton.Width, ResultBox.Height);
                MinimumSize = Size;
                MaximumSize = Size;
            }
        }

        // public
        public void Reset()
        {
            ItemList.Reset();
        }
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", GetCheckedItems().Count, LocalisedOf, GetItems().Count, LocalisedName);
        }
        public string GetResult()
        {
            return ResultBox.Text;
        }
        public List<CheckableItem> GetItems()
        {
            return ItemList.GetItems();
        }
        public List<CheckableItem> GetCheckedItems()
        {
            return ItemList.GetCheckedItems();
        }
        public List<CheckableItem> GetUncheckedItems()
        {
            return ItemList.GetUncheckedItems();
        }
        public void SetItems(List<CheckableItem> items)
        {
            if (items != null)
            {
                ItemList.SetItems(items);
                UpdateResult();
            }
        }
        public bool IsItemChecked(string itemId)
        {
            return ItemList.GetCheckedItems().Any(item => item.Id.Equals(itemId));
        }

        private class DropDownList : Form
        {
            public event ItemCheckEventHandler ItemStateChanged;
            public event EventHandler DropDownFocusLost;

            public Control AnchorControl { get; set; } = null;
            public string LocalisedChangeAll { get; }

            private CheckedListBox ItemList { get; } = new CheckedListBox();

            public DropDownList(string changAllText) : base()
            {
                AnchorControl = this;

                AutoSize = true;
                AutoSizeMode = AutoSizeMode.GrowAndShrink;
                FormBorderStyle = FormBorderStyle.None;
                LocalisedChangeAll = changAllText;
                ShowInTaskbar = false;
                StartPosition = FormStartPosition.Manual;

                ItemList.IntegralHeight = true;
                ItemList.CheckOnClick = true;
                ItemList.Margin = new Padding(0);
                ItemList.ItemCheck += ItemList_ItemCheck;
                ItemList.LostFocus += ItemList_LostFocus; ;

                Controls.Add(ItemList);
            }

            // events
            private void ItemList_LostFocus(object sender, EventArgs evt)
            {
                DropDownFocusLost?.Invoke(this, null);
            }
            private void ItemList_ItemCheck(object sender, ItemCheckEventArgs evt)
            {
                if (ItemList.Items[evt.Index] is CheckableItem item)
                {
                    item.State = (evt.NewValue == CheckState.Checked);
                    if (evt.Index == 0)
                    {
                        ChangeAllStates(item.State);
                    }
                    ItemStateChanged?.Invoke(item, evt);
                }
            }

            // publics
            public void Reset()
            {
                ItemList.SetItemChecked(0, true);
                ResetAllStates();
            }
            public void SetItems(List<CheckableItem> items)
            {
                ItemList.BeginUpdate();
                ItemList.Items.Clear();
                ItemList.Items.Add(new CheckableItem(string.Format("#{0}", LocalisedChangeAll), true));
                ItemList.Items.AddRange(items.ToArray());
                Reset();
                ResizeDropDown();
                ItemList.EndUpdate();
            }
            public void ShowPopup(IWin32Window parent)
            {
                if (AnchorControl is EcfToolBarCheckComboBox box)
                {
                    Location = box.PointToScreen(new Point(0, box.Height));
                    Width = Math.Max(Width, box.Width);
                }
                Show(parent);
            }
            public List<CheckableItem> GetItems()
            {
                return ItemList.Items.Cast<CheckableItem>().Skip(1).ToList();
            }
            public List<CheckableItem> GetCheckedItems()
            {
                return ItemList.Items.Cast<CheckableItem>().Skip(1).Where(item => item.State == true).ToList();
            }
            public List<CheckableItem> GetUncheckedItems()
            {
                return ItemList.Items.Cast<CheckableItem>().Skip(1).Where(item => item.State == false).ToList();
            }

            // privates
            private void ResetAllStates()
            {
                for (int i = 1; i < ItemList.Items.Count; i++)
                {
                    if (ItemList.Items[i] is CheckableItem item)
                    {
                        ItemList.SetItemChecked(i, item.InitState);
                    }
                }
            }
            private void ChangeAllStates(bool state)
            {
                for (int i = 1; i < ItemList.Items.Count; i++)
                {
                    ItemList.SetItemChecked(i, state);
                }
            }
            private void ResizeDropDown()
            {
                if (AnchorControl is EcfToolBarCheckComboBox box && ItemList.Items.Count > 0)
                {
                    int itemCount = Math.Min(ItemList.Items.Count, box.MaxDropDownItems);
                    int height = (ItemList.GetItemHeight(0) + 5) * itemCount;

                    int width = 0;
                    using (Graphics gfx = ItemList.CreateGraphics())
                    {
                        gfx.PageUnit = GraphicsUnit.Pixel;
                        foreach (CheckableItem item in ItemList.Items)
                        {
                            width = Math.Max(width, (int)gfx.MeasureString(item.Display, ItemList.Font).Width);
                        }
                        if (ItemList.Items.Count > box.MaxDropDownItems)
                        {
                            width += SystemInformation.VerticalScrollBarWidth;
                        }
                        width += CheckBoxRenderer.GetGlyphSize(gfx, CheckBoxState.CheckedNormal).Width + 5;
                        width = Math.Max(width, box.Width);
                    }

                    ItemList.Size = new Size(width, height);
                }
            }
        }
        public class CheckableItem
        {
            public string Id { get; }
            public string Display { get; }
            public bool InitState { get; }
            public bool State { get; set; }

            public CheckableItem(string id, string displayName, bool initState)
            {
                Id = id;
                Display = displayName;
                State = initState;
                InitState = initState;
            }
            public CheckableItem(string displayName, bool initState) : this(displayName, displayName, initState)
            {

            }

            public override string ToString()
            {
                return Display;
            }
        }
        private class DropDownButton : ButtonBase
        {
            public event EventHandler DropStateChanged;

            public bool IsUnderCursor { get; private set; } = false;
            public ComboBoxState State { get; private set; } = ComboBoxState.Normal;

            public DropDownButton(int height) : base()
            {
                Margin = new Padding(0);
                Size = new Size(height, height);
            }

            public void Reset()
            {
                if (!State.Equals(ComboBoxState.Normal))
                {
                    State = ComboBoxState.Normal;
                    DropStateChanged?.Invoke(this, null);
                    Invalidate();
                }
            }
            public void Drop()
            {
                if (!State.Equals(ComboBoxState.Pressed))
                {
                    State = ComboBoxState.Pressed;
                    DropStateChanged?.Invoke(this, null);
                    Invalidate();
                }
            }
            public void Switch()
            {
                State = State.Equals(ComboBoxState.Normal) ? ComboBoxState.Pressed : ComboBoxState.Normal;
                DropStateChanged?.Invoke(this, null);
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs evt)
            {
                ComboBoxRenderer.DrawDropDownButton(evt.Graphics, evt.ClipRectangle, State);
            }
            protected override void OnClick(EventArgs evt)
            {
                ComboBoxState oldState = State;
                switch (State)
                {
                    case ComboBoxState.Normal: State = ComboBoxState.Pressed; break;
                    default: State = ComboBoxState.Normal; break;
                }
                if (!State.Equals(oldState))
                {
                    DropStateChanged?.Invoke(this, null);
                    Invalidate();
                }
            }
            protected override void OnMouseEnter(EventArgs evt)
            {
                IsUnderCursor = true;
            }
            protected override void OnMouseLeave(EventArgs evt)
            {
                IsUnderCursor = false;
            }
        }
    }
    public class EcfToolBarTextBox : TextBox
    {
        private string ToolTipText { get; }
        private ToolTip Tip { get; } = new ToolTip();

        public EcfToolBarTextBox(string toolTip) : base()
        {
            ToolTipText = toolTip;

            MouseHover += ToolTipTextBox_MouseHover;
        }

        private void ToolTipTextBox_MouseHover(object sender, EventArgs evt)
        {
            Tip.SetToolTip(this, ToolTipText);
        }
    }
    public class EcfToolBarButton : Button
    {
        public EcfToolBarButton(string toolTip, Image image, string text) : base()
        {
            SetStyle(ControlStyles.Selectable, false);
            new ToolTip().SetToolTip(this, toolTip);
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            if (image != null)
            {
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                Image = image;
            }
            else if (text != null)
            {
                Text = text;
            }
        }
    }
    public class EcfToolBarRadioButton : RadioButton
    {
        public EcfToolBarRadioButton(string toolTip, Image image, string text) : base()
        {
            SetStyle(ControlStyles.Selectable, false);
            new ToolTip().SetToolTip(this, toolTip);

            Appearance = Appearance.Button;
            if (image != null)
            {
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                Image = image;
            }
            else if (text != null)
            {
                Text = text;
            }
        }
    }
    public class EcfToolBarCheckBox : CheckBox
    {
        public EcfToolBarCheckBox(string toolTip, Image image, string text) : base()
        {
            SetStyle(ControlStyles.Selectable, false);
            new ToolTip().SetToolTip(this, toolTip);

            Appearance = Appearance.Button;
            if (image != null)
            {
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                Image = image;
            }
            else if (text != null)
            {
                Text = text;
            }
        }
    }
    public class EcfToolBarTreeStateCheckBox : EcfToolBarCheckBox
    {
        private Image IndeterminateImage { get; }
        private Image CheckedImage { get; }
        private Image UncheckedImage { get; }

        public EcfToolBarTreeStateCheckBox(string toolTip, Image indeterminateImage, Image checkedImage, Image uncheckedImage)
            : base(toolTip, indeterminateImage, null)
        {
            IndeterminateImage = indeterminateImage;
            CheckedImage = checkedImage;
            UncheckedImage = uncheckedImage;

            ThreeState = true;
            AutoCheck = true;

            CheckStateChanged += ToolBarTreeStateCheckBox_CheckStateChanged;

            Reset();
        }

        private void ToolBarTreeStateCheckBox_CheckStateChanged(object sender, EventArgs evt)
        {
            switch (CheckState)
            {
                case CheckState.Checked: Image = CheckedImage; break;
                case CheckState.Unchecked: Image = UncheckedImage; break;
                default: Image = IndeterminateImage; break;
            }
        }

        public void Reset()
        {
            CheckState = CheckState.Indeterminate;
        }
    }
    public class EcfToolBarNumericUpDown : NumericUpDown
    {
        public EcfToolBarNumericUpDown(string toolTip) : base()
        {
            SetStyle(ControlStyles.Selectable, false);
            new ToolTip().SetToolTip(this, toolTip);
        }
    }
    public class EcfToolBarComboBox : ComboBox
    {
        public EcfToolBarComboBox(string toolTip) : base()
        {
            SetStyle(ControlStyles.Selectable, false);
            new ToolTip().SetToolTip(this, toolTip);

            DropDownStyle = ComboBoxStyle.DropDownList;
        }
    }
    public class EcfToolBarLabel : Label
{
    public bool IsForcingBoldStyle { get; }

    public EcfToolBarLabel(string text, bool forceBold) : base()
    {
        Text = text;
        IsForcingBoldStyle = forceBold;

        SetStyle(ControlStyles.Selectable, false);
        TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

        FontChanged += ToolBarLabel_FontChanged;

        OnFontChanged(null);
    }

    private void ToolBarLabel_FontChanged(object sender, EventArgs evt)
    {
        if (IsForcingBoldStyle)
        {
            Font = new Font(Font, FontStyle.Bold);
        }
    }
}
}

// helferlein
namespace Helpers
{
    public static class EnumLocalisation
    {
        public static string GetLocalizedEnum<E>(E enumMember) where E : Enum
        {
            string enumPath = string.Format("{0}.{1}", enumMember.GetType(), enumMember);
            string localizedEnum = EnumRecources.ResourceManager.GetString(enumPath);
            bool enumValid = !string.IsNullOrEmpty(localizedEnum);
            if (Debugger.IsAttached) { Debug.Assert(enumValid, string.Format("Enum {0} has no localisation!", enumPath)); }
            return enumValid ? localizedEnum : enumPath;
        }
    }
    public static class FileHandling
    {
        public static string ParseSteamConfigFileValue(string filePathAndName, string key)
        {
            if (!string.IsNullOrEmpty(filePathAndName) && !string.IsNullOrEmpty(key))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(File.Open(filePathAndName, FileMode.Open, FileAccess.Read), Encoding.UTF8))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine().Replace("\t", "");
                            string[] lineElements = line.Split("\"".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                            if (lineElements.Length >= 2 && lineElements[0].Equals(key))
                            {
                                return lineElements[1].Replace("\\\\", "\\");
                            }
                        }
                    }
                }
                catch (Exception) { }
            }
            return null;
        }
        public static string FindSteamAppPath(string appId)
        {
            string appPath = null;
            string steamInstallPath = Registry.GetValue(
                InternalSettings.Default.EgsEcfEditorApp_SteamRegistryKey64,
                InternalSettings.Default.EgsEcfEditorApp_InstallPathRegistryValue, null)?.ToString();
            if (string.IsNullOrEmpty(steamInstallPath))
            {
                steamInstallPath = Registry.GetValue(
                    InternalSettings.Default.EgsEcfEditorApp_SteamRegistryKey32,
                    InternalSettings.Default.EgsEcfEditorApp_InstallPathRegistryValue, null)?.ToString();
            }
            if (!string.IsNullOrEmpty(steamInstallPath))
            {
                string steamManifestPath = Path.Combine(steamInstallPath, InternalSettings.Default.EgsEcfEditorApp_SteamAppsFolderName);
                string[] manifestFiles = Directory.GetFiles(steamManifestPath, string.Format("*.{0}", InternalSettings.Default.EgsEcfEditorApp_SteamAppManifestFileExtension));

                string steamLibraryFilePath = Path.Combine(steamManifestPath, InternalSettings.Default.EgsEcfEditorApp_SteamLibraryConfigFileName);
                string steamAppRootPath = ParseSteamConfigFileValue(steamLibraryFilePath, InternalSettings.Default.EgsEcfEditorApp_AppRootPathSteamConfigKey);

                string manifestFile = manifestFiles.FirstOrDefault(file => Path.GetFileNameWithoutExtension(file).Contains(appId));
                string appBasePath = ParseSteamConfigFileValue(manifestFile, InternalSettings.Default.EgsEcfEditorApp_AppPathSteamConfigKey);

                if (!string.IsNullOrEmpty(steamAppRootPath) && !string.IsNullOrEmpty(appBasePath))
                {
                    appPath = Path.Combine(steamAppRootPath, InternalSettings.Default.EgsEcfEditorApp_SteamAppsFolderName, 
                        InternalSettings.Default.EgsEcfEditorApp_SteamCommonFolderName, appBasePath);
                }
            }
            return appPath;
        }
        public static string FindFileDialogInitDirectory()
        {
            StringBuilder directory = new StringBuilder();
            directory.Append(AppSettings.Default.EgsEcfEditorApp_LastVisitedDirectory);
            if (directory.Length == 0)
            {
                string gamePath = FindSteamAppPath(InternalSettings.Default.EgsEcfEditorApp_EgsSteamAppId);
                if (gamePath != null)
                {
                    directory.Append(Path.Combine(gamePath, InternalSettings.Default.EgsEcfEditorApp_EgsConfigDirectory));
                }
            }
            if (directory.Length == 0)
            {
                directory.Append(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            }
            return directory.ToString();
        }
        [DllImport("shell32.dll", EntryPoint = "FindExecutable")]
        public static extern long FindExecutable(string lpFile, string lpDirectory, StringBuilder lpResult);
    }
}