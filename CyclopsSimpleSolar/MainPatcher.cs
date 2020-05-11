namespace CyclopsSimpleSolar
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using QModManager.API.ModLoading;

    [QModCore]
    public class MainPatcher
    {
        [QModPatch]
        public static void Patch()
        {
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
        }
    }
}
