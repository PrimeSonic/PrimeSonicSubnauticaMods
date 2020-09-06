namespace CyclopsNuclearReactor
{
    using System.Reflection;
    using Common;
    using HarmonyLib;
    using MoreCyclopsUpgrades.API;
    using QModManager.API.ModLoading;

    [QModCore]
    public static class QPatch
    {
        [QModPatch]
        public static void Patch()
        {
            MCUServices.Logger.Info("Started patching. Version: " + QuickLogger.GetAssemblyVersion());

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

            var harmony = new Harmony("com.cyclopsnuclearreactor.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            MCUServices.Logger.Info("Finished patching");
        }
    }
}
