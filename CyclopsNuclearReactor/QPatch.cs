namespace CyclopsNuclearReactor
{
    using Common;
    using Harmony;
    using MoreCyclopsUpgrades.Managers;
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
                CyNukReactorSMLHelper.PatchSMLHelper();

                // Register Charge Manager with MoreCyclopsUpgrades
                PowerManager.RegisterReusableChargerCreator((SubRoot cyclops) =>
                {
                    QuickLogger.Debug("Registering CyclopsNuclearReactor");
                    return CyNukeChargeManager.GetManager(cyclops);
                });

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
