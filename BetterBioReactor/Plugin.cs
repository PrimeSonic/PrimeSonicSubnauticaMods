namespace BetterBioReactor
{
    using System.Reflection;
    using BepInEx;
    using Common;
    using HarmonyLib;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        private const string
            MODNAME = "Better Bio-Reactor",
            AUTHOR = "PrimeSonic",
            GUID = "com.betterbioreactor.psmod",
            VERSION = "1.0.0.0";
        #endregion
        public static void Patch()
        {
            QuickLogger.Info("Start patching. Version: " + QuickLogger.GetAssemblyVersion());

#if DEBUG
            QuickLogger.DebugLogsEnabled = true;
            QuickLogger.Debug("Debug logs enabled");
#endif
            var harmony = new Harmony("com.betterbioreactor.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            QuickLogger.Info("Finished patching");
        }
    }
}
