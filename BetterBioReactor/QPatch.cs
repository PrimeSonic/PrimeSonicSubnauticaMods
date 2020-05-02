namespace BetterBioReactor
{
    using Common;

    using Harmony;

    using System.Reflection;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Info("Start patching. Version: " + QuickLogger.GetAssemblyVersion());

#if DEBUG
            QuickLogger.DebugLogsEnabled = true;
            QuickLogger.Debug("Debug logs enabled");
#endif
            var harmony = HarmonyInstance.Create("com.betterbioreactor.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            QuickLogger.Info("Finished patching");
        }
    }
}
