namespace CyclopsBioReactor
{
    using System;
    using System.Reflection;
    using Common;
    using CyclopsBioReactor.Items;
    using CyclopsBioReactor.Management;
    using Harmony;
    using MoreCyclopsUpgrades.API;

    public static class QPatch
    {
        public static void Patch()
        {
            try
            {
                QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

                var booster = new BioReactorBooster();
                booster.Patch();

                var reactor = new CyBioReactor();
                reactor.Patch();

                MCUServices.Client.RegisterAuxManagerCreators((SubRoot cyclops) =>
                {
                    return new BioManager(cyclops);
                });

                MCUServices.Client.RegisterChargerCreator((SubRoot cyclops) =>
                {
                    return MCUServices.Client.GetManager<BioManager>(cyclops, BioManager.ManagerName);
                });

                MCUServices.Client.RegisterHandlerCreator((SubRoot cyclops) =>
                {
                    return MCUServices.Client.GetManager<BioManager>(cyclops, BioManager.ManagerName);
                });

                var harmony = HarmonyInstance.Create("com.morecyclopsupgrades.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                QuickLogger.Info("Finished Patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }
    }
}
