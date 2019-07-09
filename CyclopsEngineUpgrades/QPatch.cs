namespace CyclopsEngineUpgrades
{
    using Common;
    using CyclopsEngineUpgrades.Craftables;
    using MoreCyclopsUpgrades.API;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            var engineMk2Upgrade = new PowerUpgradeModuleMk2();
            var engineMk3Upgrade = new PowerUpgradeModuleMk3(engineMk2Upgrade);

            engineMk2Upgrade.Patch();
            engineMk3Upgrade.Patch();

            MCUServices.Register.CyclopsUpgradeHandler(engineMk3Upgrade.CreateEngineHandler);
            MCUServices.Register.PdaIconOverlay(TechType.PowerUpgradeModule, engineMk3Upgrade.CreateEngineOverlay);
            MCUServices.Register.PdaIconOverlay(engineMk2Upgrade.TechType, engineMk3Upgrade.CreateEngineOverlay);
            MCUServices.Register.PdaIconOverlay(engineMk3Upgrade.TechType, engineMk3Upgrade.CreateEngineOverlay);

            QuickLogger.Info($"Finished patching.");
        }
    }
}
