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
            MCUServices.Logger.Info("Started patching. Version: " + QuickLogger.GetAssemblyVersion());

            try
            {
                CyNukReactorBuildable.PatchSMLHelper();
                CyNukeEnhancerMk1.PatchSMLHelper();
                CyNukeEnhancerMk2.PatchSMLHelper();

                MCUServices.Logger.Debug("Registering with MoreCyclopsUpgrades");

                MCUServices.Register.CyclopsCharger<CyNukeChargeManager>((SubRoot cyclops) =>
                {
                    return new CyNukeChargeManager(cyclops);
                });

                MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
                {
                    return new CyNukeEnhancerHandler(cyclops);
                });

                MCUServices.Register.AuxCyclopsManager<CyNukeManager>((SubRoot cyclops) =>
                {
                    return new CyNukeManager(cyclops);
                });

                var harmony = HarmonyInstance.Create("com.cyclopsnuclearreactor.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                MCUServices.Logger.Info("Finished patching");
            }
            catch (Exception ex)
            {
                MCUServices.Logger.Error(ex);
            }
        }
    }
}
