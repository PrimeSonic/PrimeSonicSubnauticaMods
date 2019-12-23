namespace CustomCraft2SML
{
    using Common;
    using CustomCraft2SML.Serialization;
    using QModManager.API.ModLoading;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    [QModCore]
    public static class QPatch
    {
        private static readonly string version = QuickLogger.GetAssemblyVersion();

        [QModPrePatch("9EE41CD5C763AFFA8E50955620BFC831")]
        public static void SetUpLogging()
        {
            QuickLogger.Info($"Setting up logging. Version {version}");

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
            catch (Exception ex)
            {
                QuickLogger.Error($"Critical error during file restoration", ex);
            }
        }

        [QModPostPatch("594BAD715C15AC3ABFDA0B394DFF273F")]
        public static void PatchRequestsFromFiles()
        {
            QuickLogger.Info($"Started patching. Version {version}");

            try
            {
                WorkingFileParser.HandleWorkingFiles();

                QuickLogger.Info("Finished patching.");
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Critical error during file patching", ex);
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
