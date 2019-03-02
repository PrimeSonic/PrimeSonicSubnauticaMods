namespace BetterBioReactor
{
    using System;
    using System.Reflection;
    using Common;
    using Harmony;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Info("Start patching. Version: " + QuickLogger.GetAssemblyVersion());

            try
            {
                var harmony = HarmonyInstance.Create("com.betterbioreactor.psmod");
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
