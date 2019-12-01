namespace CustomBatteries.PackReading
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Common;

    internal static class PackReader
    {
        public static IEnumerable<IPluginDetails> GetAllPacks(string folderLocation)
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

                PluginPack plugin = LoadFromFile(pluginDataFile);

                if (plugin == null)
                {
                    QuickLogger.Warning($"Pack file in '{pluginFolder}' contained errors and could not be read");
                    continue;
                }

                plugin.PluginPackFolder = pluginFolder;

                yield return plugin;
            }
        }

        private static PluginPack LoadFromFile(string file)
        {
            string text = File.ReadAllText(file, Encoding.UTF8);

            var pluginPack = new PluginPack();

            bool readCorrectly = pluginPack.FromString(text);

            if (readCorrectly)
            {
                return pluginPack;
            }
            
            return null;
        }
    }
}
