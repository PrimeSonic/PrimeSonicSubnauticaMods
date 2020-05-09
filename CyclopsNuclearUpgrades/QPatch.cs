namespace CyclopsNuclearUpgrades
{
    using Common;
    using CyclopsNuclearUpgrades.Management;
    using MoreCyclopsUpgrades.API;
    using QModManager.API.ModLoading;

    [QModCore]
    public static class QPatch
    {
        [QModPatch]
        public static void Patch()
        {
            MCUServices.Logger.Info("Started patching v" + QuickLogger.GetAssemblyVersion());

            var nuclearModule = new CyclopsNuclearModule();
            var depletedModule = new DepletedNuclearModule(nuclearModule);
            var nuclearFabricator = new NuclearFabricator(nuclearModule);

            nuclearModule.Patch();
            depletedModule.Patch();
            nuclearFabricator.Patch();

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                return new NuclearUpgradeHandler(nuclearModule.TechType, cyclops);
            });

            MCUServices.Register.CyclopsCharger<NuclearChargeHandler>((SubRoot cyclops) =>
            {
                return new NuclearChargeHandler(cyclops, nuclearModule.TechType);
            });

            MCUServices.Register.PdaIconOverlay(nuclearModule.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
            {
                return new NuclearIconOverlay(icon, upgradeModule);
            });

            MCUServices.Logger.Info("Finished patching");
        }
    }
}
