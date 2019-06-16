namespace CyclopsSolarUpgrades
{
    using Common;
    using CyclopsSolarUpgrades.Craftables;
    using CyclopsSolarUpgrades.Management;
    using MoreCyclopsUpgrades.API;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            var solar1 = new CyclopsSolarCharger();
            var solar2 = new CyclopsSolarChargerMk2(solar1);

            solar1.Patch();
            solar2.Patch();

            MCUServices.Client.RegisterAuxManagerCreators(Solar.CreateHandler);
            MCUServices.Client.RegisterChargerCreator(Solar.FindHandler);
            MCUServices.Client.RegisterHandlerCreator(Solar.FindHandler);

            QuickLogger.Info($"Finished patching.");
        }
    }
}
