namespace CyclopsThermalUpgrades
{
    using System;
    using Common;
    using CyclopsThermalUpgrades.Craftables;
    using CyclopsThermalUpgrades.Management;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;
    using QModManager.API.ModLoading;

    [QModCore]
    public static class QPatch
    {
        [QModPatch]
        public static void Patch()
        {
            try
            {
                QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

                var thermalMk2 = new CyclopsThermalChargerMk2();
                thermalMk2.Patch();

                MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
                {
                    return new ThermalUpgradeHandler(TechType.CyclopsThermalReactorModule, thermalMk2.TechType, cyclops);
                });

                MCUServices.Register.CyclopsCharger<ThermalCharger>((SubRoot cyclops) =>
                {
                    return new ThermalCharger(thermalMk2.TechType, cyclops);
                });

                MCUServices.Register.PdaIconOverlay(TechType.CyclopsThermalReactorModule, CreateIconOverlay);
                MCUServices.Register.PdaIconOverlay(thermalMk2.TechType, CreateIconOverlay);

                QuickLogger.Info($"Finished patching.");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }

        internal static IconOverlay CreateIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
        {
            return new ThermalIconOverlay(icon, upgradeModule);
        }
    }
}
