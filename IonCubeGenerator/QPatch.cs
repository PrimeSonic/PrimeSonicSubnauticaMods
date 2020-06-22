namespace IonCubeGenerator
{
    using Common;
    using Harmony;
    using IonCubeGenerator.Buildable;
    using IonCubeGenerator.Configuration;
    using QModManager.API.ModLoading;
    using System.Reflection;

    [QModCore]
    public static class QPatch
    {
        [QModPatch]
        public static void Patch()
        {
            QuickLogger.Info("Started patching. Version: " + QuickLogger.GetAssemblyVersion());

#if DEBUG
            QuickLogger.DebugLogsEnabled = true;
            QuickLogger.Debug("Debug logs enabled");
#endif

            ModConfiguration.Initialize();

            CubeGeneratorBuildable.PatchSMLHelper();

            var harmony = HarmonyInstance.Create("com.ioncubegenerator.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            QuickLogger.Info("Finished patching");
        }
    }
}
