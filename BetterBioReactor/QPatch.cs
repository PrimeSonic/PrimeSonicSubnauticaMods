namespace BetterBioReactor
{
    using System.Reflection;
    using Common;
    using HarmonyLib;
    using QModManager.API.ModLoading;

    [QModCore]
    public static class QPatch
    {
        [QModPatch]
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
