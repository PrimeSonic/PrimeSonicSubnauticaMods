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
                CyNukReactorBuildable.PatchSMLHelper();
                CyNukeEnhancerMk1.PatchSMLHelper();
                CyNukeEnhancerMk2.PatchSMLHelper();

                RegisterWithMoreCyclopsUpgrades();

                var harmony = HarmonyInstance.Create("com.cyclopsnuclearreactor.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                QuickLogger.Info("Finished patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }

        private static void RegisterWithMoreCyclopsUpgrades()
        {
            PowerManager.RegisterChargerCreator((SubRoot cyclops) =>
            {
                QuickLogger.Debug("Registering CyclopsNuclearReactor ICyclopsCharger");
                return CyNukeChargeManager.GetManager(cyclops);
            });

            UpgradeManager.RegisterHandlerCreator((SubRoot cyclops) =>
            {
                QuickLogger.Debug("Registering CyNukeEnhancerHandler UpgradeHandler");
                return new CyNukeEnhancerHandler(cyclops);
            });
        }
    }
}
