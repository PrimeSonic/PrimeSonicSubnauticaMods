namespace CyclopsEngineUpgrades
{
    using Common;
    using CyclopsEngineUpgrades.Craftables;
    using CyclopsEngineUpgrades.Handlers;
    using MoreCyclopsUpgrades.API;
    using QModManager.API.ModLoading;
    using SMLHelper.V2.Handlers;

    [QModCore]
    public static class QPatch
    {
        [QModPatch]
        public static void Patch()
        {
            MCUServices.Logger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            var engineMk2Upgrade = new PowerUpgradeModuleMk2();
            var engineMk3Upgrade = new PowerUpgradeModuleMk3(engineMk2Upgrade);

            engineMk2Upgrade.Patch();
            engineMk3Upgrade.Patch();

            LanguageHandler.SetLanguageLine(EngineOverlay.BonusKey, "[Bonus Efficiency]");
            LanguageHandler.SetLanguageLine(EngineOverlay.TotalKey, "[Total Efficiency]");

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                return new EngineHandler(engineMk2Upgrade, engineMk3Upgrade, cyclops);
            });

            MCUServices.Register.PdaIconOverlay(TechType.PowerUpgradeModule, CreateEngineOverlay);
            MCUServices.Register.PdaIconOverlay(engineMk2Upgrade.TechType, CreateEngineOverlay);
            MCUServices.Register.PdaIconOverlay(engineMk3Upgrade.TechType, CreateEngineOverlay);

            MCUServices.Logger.Info($"Finished patching.");
        }

        internal static EngineOverlay CreateEngineOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
        {
            return new EngineOverlay(icon, upgradeModule);
        }
    }
}
