namespace CyclopsAutoZapper
{
    using System.Reflection;
    using BepInEx;
    using BepInEx.Logging;
    using Common;
    using HarmonyLib;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.morecyclopsupgrades.psmod", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        private const string
            MODNAME = "Cyclops Auto-Zappers",
            AUTHOR = "PrimeSonic",
            GUID = "com.cyclopsautozapper.psmod",
            VERSION = "1.0.0.0";
        internal static ManualLogSource logSource;
        #endregion

        public void Awake()
        {
            logSource = Logger;
            QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());
            QuickLogger.DebugLogsEnabled = false;

            var defenseSystem = new CyclopsAutoDefense();
            defenseSystem.Patch();

            var antiParasites = new CyclopsParasiteRemover();
            antiParasites.Patch();

            var defenseSystemMk2 = new CyclopsAutoDefenseMk2(defenseSystem);
            defenseSystemMk2.Patch();

            DisplayTexts.Main.Patch();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);

            QuickLogger.Info("Finished Patching");
        }
    }
}
