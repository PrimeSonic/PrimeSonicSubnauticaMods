namespace CyclopsNuclearReactor
{
    using Common;
    using Harmony;
    using MoreCyclopsUpgrades.API;
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
            QuickLogger.Debug("Registering with MoreCyclopsUpgrades");
            MCUServices.Client.RegisterChargerCreator((SubRoot cyclops) =>
            {                
                return CyNukeChargeManager.GetManager(cyclops);
            });

            MCUServices.Client.RegisterHandlerCreator((SubRoot cyclops) =>
            {
                return new CyNukeEnhancerHandler(cyclops);
            });
        }
    }
}
