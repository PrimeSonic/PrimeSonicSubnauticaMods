namespace CustomCraft2SML
{
    using Common;
    using CustomCraft2SML.Serialization;
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public static class QPatch
    {
        private static readonly CustomCraft2Config Config = new CustomCraft2Config();

        public static void Patch()
        {
            QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            try
            {
                CheckLogLevel();

                RestoreAssets();

                HelpFilesWriter.HandleHelpFiles();

                WorkingFileParser.HandleWorkingFiles();

                QuickLogger.Info("Finished patching.");
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Critical error during patching{Environment.NewLine}{ex}");
            }
        }

        internal static void CheckLogLevel()
        {
            if (!File.Exists(FileLocations.ConfigFile))
            {
                File.WriteAllText(FileLocations.ConfigFile, Config.PrettyPrint());
                QuickLogger.DebugLogsEnabled = false;
                QuickLogger.Info("CustomCraft2Config file was not found. Default file written.");
            }
            else
            {
                string configText = File.ReadAllText(FileLocations.ConfigFile);

                if (Config.FromString(configText))
                {
                    QuickLogger.DebugLogsEnabled = Config.EnabledDebugLogs;
                }
            }

            if (QuickLogger.DebugLogsEnabled)
                QuickLogger.Debug("Debug logging is enable");
            else
                QuickLogger.Info("To enable Debug logging, change the \"DebugLogsEnabled\" attribute in the mod.json file to true");
        }

        internal static void RestoreAssets()
        {
            string prefix = "CustomCraft2SML.SpriteAssets.";

            var ass = Assembly.GetExecutingAssembly();
            System.Collections.Generic.IEnumerable<string> resources = ass.GetManifestResourceNames().Where(name => name.StartsWith(prefix));

            foreach (string resource in resources)
            {
                string file = resource.Substring(resource.Substring(0, resource.LastIndexOf(".")).LastIndexOf(".") + 1);
                //Console.WriteLine(file);
                string outFile = System.IO.Path.Combine(FileLocations.AssetsFolder, file);
                if (!File.Exists(outFile))
                {
                    Stream s = ass.GetManifestResourceStream(resource);
                    var r = new BinaryReader(s);
                    File.WriteAllBytes(outFile, r.ReadBytes((int)s.Length));
                }
            }
        }
    }
}
