namespace CyclopsSimpleSolar
{
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using QModManager.API.ModLoading;

    [QModCore]
    public class MainPatcher
    {
        [QModPatch]
        public static void Patch()
        {
            MCUServices.Logger.Info("Started patching v" + QuickLogger.GetAssemblyVersion());

            var solarChargerItem = new CySolarModule();
            solarChargerItem.Patch();

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                return new UpgradeHandler(solarChargerItem.TechType, cyclops)
                {
                    MaxCount = 1
                };
            });

            MCUServices.Register.CyclopsCharger<CySolarChargeManager>((SubRoot cyclops) =>
            {
                return new CySolarChargeManager(solarChargerItem, cyclops);
            });

            MCUServices.Register.PdaIconOverlay(solarChargerItem.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
            {
                return new SolarPdaOverlay(icon, upgradeModule);
            });

            MCUServices.Logger.Info("Finished patching");
        }
    }
}
