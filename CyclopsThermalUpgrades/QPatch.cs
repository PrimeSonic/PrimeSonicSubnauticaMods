namespace CyclopsThermalUpgrades
{
    using System;
    using Common;
    using CyclopsThermalUpgrades.Craftables;
    using CyclopsThermalUpgrades.Management;
    using MoreCyclopsUpgrades.API;

    public static class QPatch
    {
        public static void Patch()
        {
            try
            {
                QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

                var thermalMk2 = new CyclopsThermalChargerMk2();
                thermalMk2.Patch();

                MCUServices.Register.CyclopsUpgradeHandler(thermalMk2);
                MCUServices.Register.RenewableCyclopsCharger<ThermalCharger>(thermalMk2);
                MCUServices.Register.PdaIconOverlay(TechType.CyclopsThermalReactorModule, thermalMk2);
                MCUServices.Register.PdaIconOverlay(thermalMk2.TechType, thermalMk2);

                QuickLogger.Info($"Finished patching.");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }
    }
}
