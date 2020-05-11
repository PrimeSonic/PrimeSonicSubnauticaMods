namespace CyclopsSimpleSolar
{
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using QModManager.API;
    using QModManager.API.ModLoading;
    using SMLHelper.V2.Handlers;

    [QModCore]
    public static class MainPatcher
    {
        private static TechType solarChargerMk1 = TechType.None;
        private static TechType solarChargerMk2 = TechType.None;
        internal const string CrossModKey = "CySolCross";

        [QModPrePatch]
        public static void PrePatch()
        {
            if (QModServices.Main.ModPresent("CyclopsSolarUpgrades"))
            {
                if (TechTypeHandler.TryGetModdedTechType("CyclopsSolarCharger", out solarChargerMk1) &&
                    TechTypeHandler.TryGetModdedTechType("CyclopsSolarChargerMk2", out solarChargerMk2))
                {
                    MCUServices.Logger.Info("CyclopsSolarUpgrades mod is present. Solar charging will not stack with this mod.");
                    MCUServices.Logger.Debug("TechTypes for other Cyclops solar chargers detected.");

                    LanguageHandler.SetLanguageLine(CrossModKey, "DISABLED");
                }
            }
        }

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
                return new CySolarChargeManager(solarChargerItem, cyclops)
                {
                    OtherCySolarModsPresent = solarChargerMk2 > TechType.None,
                    CrossModSolarCharger1 = solarChargerMk1,
                    CrossModSolarCharger2 = solarChargerMk2
                };
            });

            MCUServices.Register.PdaIconOverlay(solarChargerItem.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
            {
                return new SolarPdaOverlay(icon, upgradeModule);
            });

            MCUServices.Logger.Info("Finished patching");
        }
    }
}
