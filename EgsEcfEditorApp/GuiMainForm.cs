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

namespace EgsEcfEditorApp
{
    public partial class GuiMainForm : Form
    {
        private OpenFileDialog OpenDialog { get; } = new OpenFileDialog();
        private SaveFileDialog SaveDialog { get; } = new SaveFileDialog();

        public GuiMainForm()
        {
            InitializeComponent();
            AppInitializing();
        }

        // Events
        private void GuiMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = AppClosing();
        }
        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            OpenEcfFile();
        }
        private void ReloadButton_Click(object sender, EventArgs e)
        {
            ReloadEcfFile();
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveEcfFile();
        }
        private void SaveAsButton_Click(object sender, EventArgs e)
        {
            SaveEcfFileAs();
        }
        private void SaveAllButton_Click(object sender, EventArgs e)
        {
            SaveAllEcfFiles();
        }
        private void CloseButton_Click(object sender, EventArgs e)
        {
            CloseEcfFile();
        }
        private void CloseAllButton_Click(object sender, EventArgs e)
        {
            CloseAllEcfFiles();
        }
        private void ViewResized(object sender, EventArgs e)
        {
            StoreViewSizes(sender);
        }

        // App Handling
        private void AppInitializing()
        {
            // Settings cleanup
            if (Debugger.IsAttached) { Properties.Settings.Default.Reset(); }

            // MainForm settings
            Text = string.Format("{0} - {1}", Properties.names.AppName, Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Icon = Properties.icons.AppIcon;
            RestoreWindowSettings();

            // ecf file view 
            EcfFileViewPanel.TabPages.Clear();

            // open file dialog
            OpenDialog.AddExtension = true;
            OpenDialog.DefaultExt = Properties.Settings.Default.Extension_EgsConfigFile;
            OpenDialog.Filter = Properties.Settings.Default.Filter_EgsEcfFile;
            OpenDialog.Title = Properties.titles.FileOpen;
            OpenDialog.Multiselect = false;
            OpenDialog.ShowHelp = false;

            // save file dialog
            SaveDialog.AddExtension = true;
            SaveDialog.DefaultExt = Properties.Settings.Default.Extension_EgsConfigFile;
            SaveDialog.Filter = Properties.Settings.Default.Filter_EgsEcfFile;
            SaveDialog.Title = Properties.titles.FileSaveAs;
            SaveDialog.ShowHelp = false;
        }
        private bool AppClosing()
        {
            bool cancelClosing = false;
            if (EcfFileViewPanel.TabPages.Cast<EcfTabPage>().Any(tab => tab.File.HasUnsavedData))
            {
                cancelClosing = MessageBox.Show(this, Properties.texts.CloseAppWithUnsaved, Properties.titles.GenericAttention, 
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
            MessageBox.Show(this, Properties.texts.NoTabSelected, Properties.titles.GenericWarning);
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
        private void OpenEcfFile()
        {
            OpenDialog.InitialDirectory = FileHandling.FindFileDialogInitDirectory();
            if (OpenDialog.ShowDialog(this) == DialogResult.OK)
            {
                Properties.Settings.Default.Directory_FileDialog_Last = Path.GetDirectoryName(OpenDialog.FileName);
                try
                {
                    EgsEcfFile file = new EgsEcfFile(OpenDialog.FileName);
                    EcfTabPage tab = new EcfTabPage(file);
                    tab.InitViewSizes(Properties.Settings.Default.Size_View_Info_Width,
                        Properties.Settings.Default.Size_View_Error_Height,
                        Properties.Settings.Default.Size_View_Tree_Width);
                    tab.AnyViewResized += ViewResized;
                    EcfFileViewPanel.TabPages.Add(tab);
                    EcfFileViewPanel.SelectedTab = tab;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, string.Format("{0}{1}{1}{2}", Properties.texts.OpenEcfFileFailed, Environment.NewLine, ex.Message), Properties.titles.GenericWarning);
                }
            }
        }
        private void ReloadEcfFile()
        {
            if (TryGetSelectedTab(out EcfTabPage tab))
            {
                bool cancelOverride = false;
                if (tab.File.HasUnsavedData)
                {
                    cancelOverride = MessageBox.Show(this, Properties.texts.OverrideTabWithUnsaved, Properties.titles.GenericAttention,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes;
                }
                if (!cancelOverride)
                {
                    try
                    {
                        tab.File.Reload();
                        tab.UpdateTabDescription();
                        tab.UpdateAllViewsAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, string.Format("{0}{1}{1}{2}", Properties.texts.ReloadEcfFileFailed, Environment.NewLine, ex.Message), Properties.titles.GenericWarning);
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
                    ecf.UpdateErrorViewAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, string.Format("{0}{1}{1}{2}", Properties.texts.SaveEcfFileFailed, Environment.NewLine, ex.Message), Properties.titles.GenericWarning);
                }
            }
        }
        private void SaveEcfFileAs()
        {
            if (TryGetSelectedTab(out EcfTabPage ecf))
            {
                SaveDialog.InitialDirectory = FileHandling.FindFileDialogInitDirectory();
                if (SaveDialog.ShowDialog(this) == DialogResult.OK)
                {
                    Properties.Settings.Default.Directory_FileDialog_Last = Path.GetDirectoryName(SaveDialog.FileName);
                    try
                    {
                        ecf.File.Save(SaveDialog.FileName);
                        ecf.UpdateTabDescription();
                        ecf.UpdateErrorViewAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, string.Format("{0}{1}{1}{2}", Properties.texts.SaveEcfFileFailed, Environment.NewLine, ex.Message), Properties.titles.GenericWarning);
                    }
                }
            }
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
                        ecf.UpdateErrorViewAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("{0}{1}{1}{2}", Properties.texts.SaveEcfFileFailed, Environment.NewLine, ex.Message), Properties.titles.GenericWarning);
            }
        }
        private void CloseEcfFile()
        {
            if (TryGetSelectedTab(out EcfTabPage tab))
            {
                bool cancelClosing = false;
                if (tab.File.HasUnsavedData)
                {
                    cancelClosing = MessageBox.Show(this, Properties.texts.CloseTabWithUnsaved, Properties.titles.GenericAttention,
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
                cancelClosing = MessageBox.Show(this, Properties.texts.CloseAllTabsWithUnsaved, Properties.titles.GenericAttention,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes;
            }
            if (!cancelClosing)
            {
                EcfFileViewPanel.TabPages.Clear();
            }
        }
    }
}
