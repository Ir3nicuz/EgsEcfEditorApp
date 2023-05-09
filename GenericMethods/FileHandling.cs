using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace GenericMethods
{
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
                InternalSettings.Default.EgsEcfEditorApp_FileHandling_SteamRegistryKey64,
                InternalSettings.Default.EgsEcfEditorApp_FileHandling_InstallPathRegistryValue, null)?.ToString();
            if (string.IsNullOrEmpty(steamInstallPath))
            {
                steamInstallPath = Registry.GetValue(
                    InternalSettings.Default.EgsEcfEditorApp_FileHandling_SteamRegistryKey32,
                    InternalSettings.Default.EgsEcfEditorApp_FileHandling_InstallPathRegistryValue, null)?.ToString();
            }
            if (!string.IsNullOrEmpty(steamInstallPath))
            {
                string steamManifestPath = Path.Combine(steamInstallPath, InternalSettings.Default.EgsEcfEditorApp_FileHandling_SteamAppsFolderName);
                string[] manifestFiles = Directory.GetFiles(steamManifestPath, string.Format("*.{0}", InternalSettings.Default.EgsEcfEditorApp_FileHandling_SteamAppManifestFileExtension));

                string steamLibraryFilePath = Path.Combine(steamManifestPath, InternalSettings.Default.EgsEcfEditorApp_FileHandling_SteamLibraryConfigFileName);
                string steamAppRootPath = ParseSteamConfigFileValue(steamLibraryFilePath, InternalSettings.Default.EgsEcfEditorApp_FileHandling_AppRootPathSteamConfigKey);

                string manifestFile = manifestFiles.FirstOrDefault(file => Path.GetFileNameWithoutExtension(file).Contains(appId));
                string appBasePath = ParseSteamConfigFileValue(manifestFile, InternalSettings.Default.EgsEcfEditorApp_FileHandling_AppPathSteamConfigKey);

                if (!string.IsNullOrEmpty(steamAppRootPath) && !string.IsNullOrEmpty(appBasePath))
                {
                    appPath = Path.Combine(steamAppRootPath, InternalSettings.Default.EgsEcfEditorApp_FileHandling_SteamAppsFolderName,
                        InternalSettings.Default.EgsEcfEditorApp_FileHandling_SteamCommonFolderName, appBasePath);
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
                string gamePath = FindSteamAppPath(InternalSettings.Default.EgsEcfEditorApp_EGSSteamAppId);
                if (gamePath != null)
                {
                    directory.Append(Path.Combine(gamePath, InternalSettings.Default.EgsEcfEditorApp_FileHandling_EGSConfigDirectory));
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
