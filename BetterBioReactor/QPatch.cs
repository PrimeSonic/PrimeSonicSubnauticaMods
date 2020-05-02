namespace BetterBioReactor
{
    using System.Reflection;
    using Common;
    using Harmony;
    using QModManager.API.ModLoading;

    /// <summary>
    /// For use by QModManager only.
    /// </summary>
    [QModCore]
    public static class QPatch
    {
        /// <summary>
        /// For use by QModManager only.
        /// </summary>
        [QModPatch]
        public static void Patch()
        {
            QuickLogger.Info("Start patching. Version: " + QuickLogger.GetAssemblyVersion());

            var harmony = HarmonyInstance.Create("com.betterbioreactor.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            QuickLogger.Info("Finished patching");
        }
    }
}
