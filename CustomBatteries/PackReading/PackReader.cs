namespace CustomBatteries.PackReading
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Common;

    internal static class PackReader
    {
        public static IEnumerable<IPluginPack> GetAllPacks(string folderLocation)
        {
            foreach (string pluginFolder in Directory.GetDirectories(folderLocation))
            {
                string pluginDataFile = Path.Combine(pluginFolder, "Plugin.txt");

                if (!File.Exists(pluginFolder))
                {
                    QuickLogger.Warning($"Plugin folder '{pluginFolder}' did not contain a 'Plugin.txt' file");
                    continue;
                }

                PluginPack plugin = LoadFromFile(pluginDataFile);

                if (plugin == null)
                {
                    QuickLogger.Warning($"Plugin file contained errors and could not be read");
                    continue;
                }

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
