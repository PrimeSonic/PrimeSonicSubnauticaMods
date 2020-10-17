namespace CustomBatteries.PackReading
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Common;
    using CustomBatteries.API;
    using CustomBatteries.Items;

    internal static class PackReader
    {
        private const string PluginFileName = EmTextPluginPack.MainKey + ".txt";

        public static void PatchTextPacks()
        {
            string pluginPacksFolder = Path.Combine(CbDatabase.ExecutingFolder, "Packs");

            if (!Directory.Exists(pluginPacksFolder))
            {
                QuickLogger.Warning("'Packs' folder was not found. Folder will be created. No text plugins were patched.");
                Directory.CreateDirectory(pluginPacksFolder);
                return;
            }

            var customPacks = new List<TextPluginPack>();

            foreach (IParsedPluginPack pluginPack in GetAllPacks(pluginPacksFolder))
            {
                customPacks.Add(new TextPluginPack(pluginPack));
            }

            if (customPacks.Count == 0)
            {
                QuickLogger.Info("No plugin files were found in the 'Packs' folder. No text plugins were patched.");
                return;
            }

            foreach (TextPluginPack customPack in customPacks)
            {
                customPack.Patch();
            }
        }

        private static IEnumerable<IParsedPluginPack> GetAllPacks(string folderLocation)
        {
            // Check all folders
            foreach (string pluginFolder in Directory.GetDirectories(folderLocation))
            {
                // Find the CustomBatteriesPack.txt file
                string pluginDataFilePath = Path.Combine(pluginFolder, PluginFileName);
                string plugingFolderName = new DirectoryInfo(Path.GetDirectoryName(pluginDataFilePath)).Name;

                if (!File.Exists(pluginDataFilePath))
                {
                    QuickLogger.Warning($"Plugin packs folder '{plugingFolderName}' did not contain a file named '{PluginFileName}'");
                    continue;
                }

                QuickLogger.Info($"Reading plugin pack '{plugingFolderName}'");

                EmTextPluginPack plugin = LoadFromFile(pluginDataFilePath);

                if (plugin == null)
                {
                    QuickLogger.Warning($"Pack file in '{pluginFolder}' contained errors and could not be read");
                    continue;
                }

                plugin.PluginPackFolder = pluginFolder;

                yield return plugin;
            }
        }

        private static EmTextPluginPack LoadFromFile(string file)
        {
            string text = File.ReadAllText(file, Encoding.UTF8);

            var pluginPack = new EmTextPluginPack();

            try
            {
                bool readCorrectly = pluginPack.FromString(text);

                if (readCorrectly)
                {
                    return pluginPack;
                }

                return null;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Error reading file '{file}'", ex);
                return null;
            }
        }
    }
}
