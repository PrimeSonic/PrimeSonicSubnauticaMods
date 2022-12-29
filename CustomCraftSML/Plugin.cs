namespace CustomCraft2SML
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using BepInEx;
    using Common;
    using CustomCraft2SML.Serialization;
    using HarmonyLib;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        private const string
            MODNAME = "Custom Craft 2",
            AUTHOR = "PrimeSonic",
            GUID = "com.customcraft2sml.psmod",
            VERSION = "1.0.0.0";
        #endregion

        private static readonly string version = QuickLogger.GetAssemblyVersion();

        static Plugin()
        {
            QuickLogger.Info($"Setting up logging. Version {version}");

            if (!Directory.Exists(FileLocations.AssetsFolder))
                Directory.CreateDirectory(FileLocations.AssetsFolder);

            if (!Directory.Exists(FileLocations.WorkingFolder))
                Directory.CreateDirectory(FileLocations.WorkingFolder);

            CustomCraft2Config.CheckLogLevel();
        }

        public void Awake()
        {
            Harmony.CreateAndPatchAll(this.GetType(), GUID);
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

        // Using patch to ensure that CC2 patches all these requests after all other SMLHelper mods
        [HarmonyPatch(typeof(PlatformUtils), nameof(PlatformUtils.PlatformInitAsync))]
        [HarmonyPostfix]
        public static IEnumerator PatchRequestsFromFiles(IEnumerator result)
        {
            yield return result;


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
            Harmony.UnpatchID(GUID);
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
