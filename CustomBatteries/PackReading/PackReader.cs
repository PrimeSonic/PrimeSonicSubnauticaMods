﻿namespace CustomBatteries.PackReading
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Common;
    using CustomBatteries.API;
    using CustomBatteries.Items;

    internal static class PackReader
    {
        public static void PatchTextPacks()
        {
            QuickLogger.Info("Reading pluging packs");
            string pluginPacksFolder = Path.Combine(CbCore.ExecutingFolder, "Packs");

            if (!Directory.Exists(pluginPacksFolder))
            {
                QuickLogger.Warning("'Packs' folder was not found. Folder will be created. No text plugins were patched.");
                Directory.CreateDirectory(pluginPacksFolder);
                return;
            }

            var customPacks = new List<TextPluginPack>();

            foreach (IParsedPluginPack pluginPack in GetAllPacks(pluginPacksFolder))
            {
                QuickLogger.Info($"Found CustomBatteriesPack '{pluginPack.PluginPackName}'");
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

        private static EmTextPluginPack LoadFromFile(string file)
        {
            string text = File.ReadAllText(file, Encoding.UTF8);

            var pluginPack = new EmTextPluginPack();

            bool readCorrectly = pluginPack.FromString(text);

            if (readCorrectly)
            {
                return pluginPack;
            }
            
            return null;
        }

        private static IEnumerable<IParsedPluginPack> GetAllPacks(string folderLocation)
        {
            // Check all folders
            foreach (string pluginFolder in Directory.GetDirectories(folderLocation))
            {
                // Find the CustomBatteriesPack.txt file
                string pluginDataFile = Path.Combine(pluginFolder, "CustomBatteriesPack.txt");

                if (!File.Exists(pluginFolder))
                {
                    QuickLogger.Warning($"Packs folder '{pluginFolder}' did not contain a file named 'CustomBatteriesPack.txt'");
                    continue;
                }

                EmTextPluginPack plugin = LoadFromFile(pluginDataFile);

                if (plugin == null)
                {
                    QuickLogger.Warning($"Pack file in '{pluginFolder}' contained errors and could not be read");
                    continue;
                }

                plugin.PluginPackFolder = pluginFolder;

                yield return plugin;
            }
        }
    }
}