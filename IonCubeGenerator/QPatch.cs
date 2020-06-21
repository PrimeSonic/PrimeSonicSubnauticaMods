using IonCubeGenerator.Configuration;

namespace IonCubeGenerator
{
    using Common;
    using Harmony;
    using IonCubeGenerator.Buildable;
    using System;
    using System.Reflection;

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
                ModConfiguration.Initialize();

                CubeGeneratorBuildable.PatchSMLHelper();

                var harmony = HarmonyInstance.Create("com.ioncubegenerator.psmod");
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
