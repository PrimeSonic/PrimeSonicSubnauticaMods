namespace CustomCraft2SML
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Common;
    using CustomCraft2SML.Serialization;
    using QModManager.API.ModLoading;

    [QModCore]
    public static class QPatch
    {
        private static readonly string version = QuickLogger.GetAssemblyVersion();

        [QModPrePatch]
        public static void SetUpLogging()
        {
            QuickLogger.Info($"Setting up logging. Version {version}");

            if (!Directory.Exists(FileLocations.AssetsFolder))
                Directory.CreateDirectory(FileLocations.AssetsFolder);

            if (!Directory.Exists(FileLocations.WorkingFolder))
                Directory.CreateDirectory(FileLocations.WorkingFolder);

            CustomCraft2Config.CheckLogLevel();
        }

        [QModPatch]
        public static void RestoreFiles()
        {
            QuickLogger.Info($"Restoring files. Version {version}");

            try
            {
                RestoreAssets();
                HelpFilesWriter.HandleHelpFiles();
            }
            catch
            {
                QuickLogger.Error($"Critical error during file restoration");
                throw;
            }
        }

        // Using secret key to ensure that CC2 patches all these requests after all other SMLHelper mods
        [QModPostPatch("594BAD715C15AC3ABFDA0B394DFF273F")]
        public static void PatchRequestsFromFiles()
        {
            QuickLogger.Info($"Started patching. Version {version}");

            try
            {
                WorkingFileParser.HandleWorkingFiles();

                QuickLogger.Info("Finished patching.");
            }
            catch
            {
                QuickLogger.Error($"Critical error during file patching");
                throw;
            }
        }

        internal static void RestoreAssets()
        {
            string prefix = "CustomCraft2SML.Assets.";

            var ass = Assembly.GetExecutingAssembly();
            IEnumerable<string> resources = ass.GetManifestResourceNames().Where(name => name.StartsWith(prefix));

            foreach (string resource in resources)
            {
                string file = resource.Substring(resource.Substring(0, resource.LastIndexOf(".")).LastIndexOf(".") + 1);

                if (!Directory.Exists(FileLocations.AssetsFolder))
                    Directory.CreateDirectory(FileLocations.AssetsFolder);

                string outFile = Path.Combine(FileLocations.AssetsFolder, file);
                if (!File.Exists(outFile))
                {
                    QuickLogger.Debug($"Restoring asset: {file}");

                    Stream s = ass.GetManifestResourceStream(resource);
                    var r = new BinaryReader(s);
                    File.WriteAllBytes(outFile, r.ReadBytes((int)s.Length));
                }
            }
        }
    }
}
