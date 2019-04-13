namespace CyclopsNuclearReactor
{
    using Common;
    using MoreCyclopsUpgrades.Managers;
    using System;

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
                    return CyNukeChargeManager.GetManager(cyclops);
                });

                // There aren't any Harmony patches in this mod (yet).
                //var harmony = HarmonyInstance.Create("com.cyclopsnuclearreactor.psmod");
                //harmony.PatchAll(Assembly.GetExecutingAssembly());
                QuickLogger.Info("Finished patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }
    }
}
