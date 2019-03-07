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
        public static void Patch()
        {
            QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            try
            {
                CustomCraft2Config.CheckLogLevel();

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

        internal static void RestoreAssets()
        {
            string prefix = "CustomCraft2SML.Assets.";

            var ass = Assembly.GetExecutingAssembly();
            System.Collections.Generic.IEnumerable<string> resources = ass.GetManifestResourceNames().Where(name => name.StartsWith(prefix));

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
