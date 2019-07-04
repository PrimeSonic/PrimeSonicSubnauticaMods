namespace CyclopsSolarUpgrades
{
    using System;
    using Common;
    using CyclopsSolarUpgrades.Craftables;
    using CyclopsSolarUpgrades.Management;
    using MoreCyclopsUpgrades.API;

    public static class QPatch
    {
        public static void Patch()
        {
            try
            {
                QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

                var solar1 = new CyclopsSolarCharger();
                var solar2 = new CyclopsSolarChargerMk2(solar1);

                solar1.Patch();
                solar2.Patch();

                MCUServices.Register.RenewableCyclopsCharger<SolarCharger>(solar2);
                MCUServices.Register.CyclopsUpgradeHandler(solar2);
                MCUServices.Register.PdaIconOverlay(solar1.TechType, solar2);
                MCUServices.Register.PdaIconOverlay(solar2.TechType, solar2);

                QuickLogger.Info($"Finished patching.");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }
    }
}
