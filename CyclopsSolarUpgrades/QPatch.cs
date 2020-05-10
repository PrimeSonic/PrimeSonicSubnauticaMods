namespace CyclopsSolarUpgrades
{
    using System;
    using Common;
    using CyclopsSolarUpgrades.Craftables;
    using CyclopsSolarUpgrades.Management;
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

                var solar1 = new CyclopsSolarCharger();
                var solar2 = new CyclopsSolarChargerMk2(solar1);

                solar1.Patch();
                solar2.Patch();

                MCUServices.Register.CyclopsCharger<SolarCharger>((SubRoot cyclops) =>
                {
                    return new SolarCharger(solar1.TechType, solar2.TechType, cyclops);
                });

                MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
                {
                    return new SolarUpgradeHandler(solar1.TechType, solar2.TechType, cyclops);
                });

                MCUServices.Register.PdaIconOverlay(solar1.TechType, CreateIconOverlay);

                MCUServices.Register.PdaIconOverlay(solar2.TechType, CreateIconOverlay);

                QuickLogger.Info($"Finished patching.");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }

        internal static IconOverlay CreateIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
        {
            return new SolarIconOverlay(icon, upgradeModule);
        }
    }
}
