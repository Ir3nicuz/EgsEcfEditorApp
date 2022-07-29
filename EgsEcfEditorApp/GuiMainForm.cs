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
using EgsEcfControls;
using static EgsEcfControls.EcfTabPage;
using System.Collections.Generic;
using static EgsEcfControls.EcfTabPage.CopyPasteClickedEventArgs;

namespace EgsEcfEditorApp
{
    public partial class GuiMainForm : Form
    {
        private EcfToolContainer FileOperationContainer { get; } = new EcfToolContainer();
        private EcfBasicFileOperations BasicFileOperations { get; } = new EcfBasicFileOperations();
        private EcfExtendedFileOperations ExtendedFileOperations { get; } = new EcfExtendedFileOperations();
        private TabControl EcfFileViewPanel { get; } = new TabControl();
        private EcfFileOpenDialog OpenDialog { get; } = new EcfFileOpenDialog(
            Properties.icons.AppIcon, Properties.Settings.Default.Filter_EgsEcfFile, Properties.Settings.Default.Extension_EgsConfigFile);
        private EcfFileSaveDialog SaveDialog { get; } = new EcfFileSaveDialog(
            Properties.icons.AppIcon, Properties.Settings.Default.Filter_EgsEcfFile, Properties.Settings.Default.Extension_EgsConfigFile);
        private DeprecatedDefinitionsDialog DeprecatedDefinitions { get; } = new DeprecatedDefinitionsDialog(Properties.icons.AppIcon);

        private List<EcfStructureItem> CopyClipboard { get; set; } = null;

        private EcfFileLoaderDialog FileLoader { get; } = new EcfFileLoaderDialog(Properties.icons.AppIcon);

        public GuiMainForm()
        {
            InitializeComponent();
            InitForm();
        }

        // Events
        private void InitForm()
        {
            // Settings cleanup
            if (Debugger.IsAttached) { Properties.Settings.Default.Reset(); }

            // MainForm settings
            Text = string.Format("{0} - {1}", Properties.names.AppName, Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Icon = Properties.icons.AppIcon;
            RestoreWindowSettings();

            InitGuiControls();
        }
        private void GuiMainForm_FormClosing(object sender, FormClosingEventArgs evt)
        {
            evt.Cancel = AppClosing();
        }
        private void Tab_AnyViewResized(object sender, EventArgs evt)
        {
            StoreViewSizes(sender);
        }
        private void Tab_CopyClicked(object sender, CopyPasteClickedEventArgs evt)
        {
            CopyElements(sender, evt);
        }
        private void Tab_PasteClicked(object sender, CopyPasteClickedEventArgs evt)
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

        // App Handling
        private void InitGuiControls()
        {
            EcfFileViewPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            EcfFileViewPanel.Dock = DockStyle.Fill;
            EcfFileViewPanel.ShowToolTips = true;
            EcfFileViewPanel.TabPages.Clear();

            FileOperationContainer.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            FileOperationContainer.Dock = DockStyle.Top;

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

            FileOperationContainer.Add(BasicFileOperations);
            FileOperationContainer.Add(ExtendedFileOperations);
            Controls.Add(EcfFileViewPanel);
            Controls.Add(FileOperationContainer);
        }
        private bool AppClosing()
        {
            bool cancelClosing = false;
            if (EcfFileViewPanel.TabPages.Cast<EcfTabPage>().Any(tab => tab.File.HasUnsavedData))
            {
                cancelClosing = MessageBox.Show(this, Properties.texts.CloseAppWithUnsaved, Properties.titles.Generic_Attention, 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes;
            }
            if (!cancelClosing)
            {
                StoreWindowSettings();
                Properties.Settings.Default.Save();
            }
            return cancelClosing;
        }
        private bool TryGetSelectedTab(out EcfTabPage ecfTab)
        {
            ecfTab = null;
            TabPage tab = EcfFileViewPanel.SelectedTab;
            if (tab != null)
            {
                if (tab is EcfTabPage ecf)
                {
                    ecfTab = ecf;
                    return true;
                }
            }
            MessageBox.Show(this, Properties.texts.NoTabSelected, Properties.titles.Generic_Warning, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
        }
        private void RestoreWindowSettings()
        {
            Rectangle screen = Screen.FromControl(this).Bounds;

            Rectangle window = new Rectangle(
                Properties.Settings.Default.Location_App_X,
                Properties.Settings.Default.Location_App_Y,
                Properties.Settings.Default.Size_App_Width,
                Properties.Settings.Default.Size_App_Height);

            if (screen.Contains(window))
            {
                Location = new Point(window.X, window.Y);
                Size = new Size(window.Width, window.Height);
            }
            else
            {
                object xPos = Properties.Settings.Default.Properties["Location_App_X"].DefaultValue;
                object yPos = Properties.Settings.Default.Properties["Location_App_Y"].DefaultValue;
                object width = Properties.Settings.Default.Properties["Size_App_Width"].DefaultValue;
                object height = Properties.Settings.Default.Properties["Size_App_Height"].DefaultValue;
                Location = new Point(Convert.ToInt32(xPos), Convert.ToInt32(yPos));
                Size = new Size(Convert.ToInt32(width), Convert.ToInt32(height));
            }
        }
        private void StoreWindowSettings()
        {
            Properties.Settings.Default.Size_App_Width = Width;
            Properties.Settings.Default.Size_App_Height = Height;
            Properties.Settings.Default.Location_App_X = Location.X;
            Properties.Settings.Default.Location_App_Y = Location.Y;
        }
        private void StoreViewSizes(object sender)
        {
            if (sender is EcfTreeView treeView)
            {
                Properties.Settings.Default.Size_View_Tree_Width = treeView.Width;
            }
            else if (sender is EcfErrorView errorView)
            {
                Properties.Settings.Default.Size_View_Error_Height = errorView.Height;
            }
            else if (sender is EcfInfoView infoView)
            {
                Properties.Settings.Default.Size_View_Info_Width = infoView.Width;
            }
        }

        // FileHandling
        private static class FileHandling
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
                string steamInstallPath = Registry.GetValue(Properties.Settings.Default.RegistryKey64_Steam, Properties.Settings.Default.RegistryValue_InstallPath, null)?.ToString();
                if (string.IsNullOrEmpty(steamInstallPath))
                {
                    steamInstallPath = Registry.GetValue(Properties.Settings.Default.RegistryKey32_Steam, Properties.Settings.Default.RegistryValue_InstallPath, null)?.ToString();
                }
                if (!string.IsNullOrEmpty(steamInstallPath))
                {
                    string steamManifestPath = Path.Combine(steamInstallPath, Properties.Settings.Default.FolderName_SteamApps);
                    string[] manifestFiles = Directory.GetFiles(steamManifestPath, string.Format("*.{0}", Properties.Settings.Default.Extension_SteamAppManifestFile));

                    string steamLibraryFilePath = Path.Combine(steamManifestPath, Properties.Settings.Default.FileName_SteamLibraryConfig);
                    string steamAppRootPath = ParseSteamConfigFileValue(steamLibraryFilePath, Properties.Settings.Default.SteamConfigKey_AppRootPath);

                    string manifestFile = manifestFiles.FirstOrDefault(file => Path.GetFileNameWithoutExtension(file).Contains(appId));
                    string appBasePath = ParseSteamConfigFileValue(manifestFile, Properties.Settings.Default.SteamConfigKey_AppPath);

                    if (!string.IsNullOrEmpty(steamAppRootPath) && !string.IsNullOrEmpty(appBasePath))
                    {
                        appPath = Path.Combine(steamAppRootPath, Properties.Settings.Default.FolderName_SteamApps, Properties.Settings.Default.FolderName_SteamCommon, appBasePath);
                    }
                }
                return appPath;
            }
            public static string FindFileDialogInitDirectory()
            {
                StringBuilder directory = new StringBuilder();
                directory.Append(Properties.Settings.Default.Directory_FileDialog_Last);
                if (directory.Length == 0)
                {
                    string gamePath = FindSteamAppPath(Properties.Settings.Default.SteamAppId_Egs);
                    if (gamePath != null)
                    {
                        directory.Append(Path.Combine(gamePath, Properties.Settings.Default.Directory_EgsConfig));
                    }
                }
                if (directory.Length == 0)
                {
                    directory.Append(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                }
                return directory.ToString();
            }
        }
        private void NewEcfFile()
        {
            try
            {
                OpenDialog.SetInitDirectory(FileHandling.FindFileDialogInitDirectory());
                if (OpenDialog.ShowDialogNewFile(this, EcfFormatting.GetSupportedFileTypes()) == DialogResult.OK)
                {
                    Properties.Settings.Default.Directory_FileDialog_Last = Path.GetDirectoryName(OpenDialog.FilePathAndName);

                    EgsEcfFile file = new EgsEcfFile(OpenDialog.FilePathAndName, OpenDialog.FileDefinition, OpenDialog.FileEncoding, OpenDialog.FileNewLineSymbol);
                    
                    EcfTabPage tab = new EcfTabPage(Properties.icons.AppIcon, file);
                    tab.InitViewSizes(Properties.Settings.Default.Size_View_Info_Width,
                        Properties.Settings.Default.Size_View_Error_Height,
                        Properties.Settings.Default.Size_View_Tree_Width);
                    tab.AnyViewResized += Tab_AnyViewResized;
                    tab.CopyClicked += Tab_CopyClicked;
                    tab.PasteClicked += Tab_PasteClicked;
                    EcfFileViewPanel.TabPages.Add(tab);
                    EcfFileViewPanel.SelectedTab = tab;
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex, Properties.texts.CreateEcfFileFailed);
            }
        }
        private void OpenEcfFile()
        {
            try
            {
                OpenDialog.SetInitDirectory(FileHandling.FindFileDialogInitDirectory());
                if (OpenDialog.ShowDialogOpenFile(this, EcfFormatting.GetSupportedFileTypes()) != DialogResult.OK) { return; }
                Properties.Settings.Default.Directory_FileDialog_Last = Path.GetDirectoryName(OpenDialog.FilePathAndName);

                EgsEcfFile file = new EgsEcfFile(OpenDialog.FilePathAndName, OpenDialog.FileDefinition);
                if (FileLoader.ShowDialog(this, file) != DialogResult.OK) { return; }
                
                EcfTabPage tab = new EcfTabPage(Properties.icons.AppIcon, file);
                tab.InitViewSizes(Properties.Settings.Default.Size_View_Info_Width,
                    Properties.Settings.Default.Size_View_Error_Height,
                    Properties.Settings.Default.Size_View_Tree_Width);
                tab.AnyViewResized += Tab_AnyViewResized;
                tab.CopyClicked += Tab_CopyClicked;
                tab.PasteClicked += Tab_PasteClicked;
                EcfFileViewPanel.TabPages.Add(tab);
                EcfFileViewPanel.SelectedTab = tab;
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex, Properties.texts.OpenEcfFileFailed);
            }
        }
        private void ReloadEcfFile()
        {
            if (TryGetSelectedTab(out EcfTabPage tab))
            {
                bool cancelOverride = false;
                if (tab.File.HasUnsavedData)
                {
                    cancelOverride = MessageBox.Show(this, Properties.texts.OverrideTabWithUnsaved, Properties.titles.Generic_Attention,
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
                        ShowExceptionMessage(ex, Properties.texts.ReloadEcfFileFailed);
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
                    ecf.File.Save();
                    ecf.UpdateTabDescription();
                    ecf.UpdateErrorView();
                }
                catch (Exception ex)
                {
                    ShowExceptionMessage(ex, Properties.texts.SaveEcfFileFailed);
                }
            }
        }
        private void SaveAsEcfFile()
        {
            if (TryGetSelectedTab(out EcfTabPage ecf))
            {
                SaveDialog.SetInitDirectory(FileHandling.FindFileDialogInitDirectory());
                if (SaveDialog.ShowDialogSaveAs(this) == DialogResult.OK)
                {
                    Properties.Settings.Default.Directory_FileDialog_Last = Path.GetDirectoryName(SaveDialog.FilePathAndName);
                    try
                    {
                        ecf.File.Save(SaveDialog.FilePathAndName);
                        ecf.UpdateTabDescription();
                        ecf.UpdateErrorView();
                    }
                    catch (Exception ex)
                    {
                        ShowExceptionMessage(ex, Properties.texts.SaveEcfFileFailed);
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
                foreach (TabPage tab in EcfFileViewPanel.TabPages)
                {
                    if (tab is EcfTabPage ecf && ecf.File.HasUnsavedData)
                    {
                        ecf.File.Save();
                        ecf.UpdateTabDescription();
                        ecf.UpdateErrorView();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex, Properties.texts.SaveEcfFileFailed);
            }
        }
        private void CloseEcfFile()
        {
            if (TryGetSelectedTab(out EcfTabPage tab))
            {
                bool cancelClosing = false;
                if (tab.File.HasUnsavedData)
                {
                    cancelClosing = MessageBox.Show(this, Properties.texts.CloseTabWithUnsaved, Properties.titles.Generic_Attention,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes;
                }
                if (!cancelClosing)
                {
                    EcfFileViewPanel.TabPages.Remove(tab);
                }
            }
        }
        private void CloseAllEcfFiles()
        {
            bool cancelClosing = false;
            if (EcfFileViewPanel.TabPages.Cast<EcfTabPage>().Any(tab => tab.File.HasUnsavedData))
            {
                cancelClosing = MessageBox.Show(this, Properties.texts.CloseAllTabsWithUnsaved, Properties.titles.Generic_Attention,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes;
            }
            if (!cancelClosing)
            {
                EcfFileViewPanel.TabPages.Clear();
            }
        }
        private void ShowExceptionMessage(Exception ex, string header)
        {
            MessageBox.Show(this, string.Format("{0}{1}{1}{2}", header, Environment.NewLine, ex.Message),
                    Properties.titles.Generic_Warning, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        // definition handling
        private void ReloadDefinitions()
        {
            try
            {
                EcfFormatting.ReloadDefinitions();
                MessageBox.Show(this, Properties.texts.ReloadFileDefinitionsSuccess, 
                    Properties.titles.Generic_Info, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("{0}{1}{1}{2}", Properties.texts.ReloadFileDefinitionsFailed, Environment.NewLine, ex.Message), 
                    Properties.titles.Generic_Warning, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
