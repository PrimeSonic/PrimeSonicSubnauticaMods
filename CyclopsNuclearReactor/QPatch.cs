namespace CyclopsNuclearReactor
{
    using System;
    using System.Reflection;
    using Common;
    using Harmony;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Info("Started patching. Version: " + QuickLogger.GetAssemblyVersion());

#if DEBUG
            QuickLogger.DebugLogsEnabled = true;
            QuickLogger.Debug("Debug logs enabled");
#endif

            try
            {
                var harmony = HarmonyInstance.Create("com.cyclopsnuclearreactor.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                QuickLogger.Info("Finished patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }
    }
}
