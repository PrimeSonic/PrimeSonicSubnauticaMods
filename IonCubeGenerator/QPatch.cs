namespace IonCubeGenerator
{
    using Common;
    using HarmonyLib;
    using IonCubeGenerator.Buildable;
    using IonCubeGenerator.Configuration;    
    using QModManager.API.ModLoading;
    using System.Reflection;
    // using Logger = QModManager.Utility.Logger;

    [QModCore]
    public static class QPatch
    {
        [QModPrePatch]
        public static void SetupConfig()
        {
            //QuickLogger.DebugLogsEnabled = QModManager.Utility.Logger.DebugLogsEnabled;
            // Logger.Log(Logger.Level.Debug, "Debug logs enabled");

            QuickLogger.Info("Loading config.json settings");
            ModConfiguration.Initialize();
        }

        [QModPatch]
        public static void Patch()
        {
            QuickLogger.Info("Started patching. Version: " + QuickLogger.GetAssemblyVersion());

            CubeGeneratorBuildable.PatchSMLHelper();

            var harmony = new Harmony("com.ioncubegenerator.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            QuickLogger.Info("Finished patching");
        }
    }
}
