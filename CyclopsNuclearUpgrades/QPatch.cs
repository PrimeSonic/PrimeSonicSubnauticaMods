namespace CyclopsNuclearUpgrades
{
    using System;
    using Common;
    using CyclopsNuclearUpgrades.Management;
    using MoreCyclopsUpgrades.API;

    public static class QPatch
    {

        public static void Patch()
        {
            try
            {
                var nuclearModule = new CyclopsNuclearModule();
                var depletedModule = new DepletedNuclearModule(nuclearModule);
                var nuclearFabricator = new NuclearFabricator(nuclearModule);

                nuclearModule.Patch();
                depletedModule.Patch();
                nuclearFabricator.Patch();

                MCUServices.Register.CyclopsUpgradeHandler(depletedModule);
                MCUServices.Register.CyclopsCharger<NuclearChargeHandler>(depletedModule);
                MCUServices.Register.PdaIconOverlay(nuclearModule.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
                {
                    return new NuclearIconOverlay(icon, upgradeModule);
                });
            }
            catch(Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }
    }
}
