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

            var speedUpgrade = new CyclopsSpeedModule();
            var engineMk2Upgrade = new PowerUpgradeModuleMk2();
            var engineMk3Upgrade = new PowerUpgradeModuleMk3(engineMk2Upgrade);

            speedUpgrade.Patch();
            engineMk2Upgrade.Patch();
            engineMk3Upgrade.Patch();


            MCUServices.Register.CyclopsUpgradeHandler(speedUpgrade.CreateSpeedUpgradeHandler);
            MCUServices.Register.CyclopsUpgradeHandler(engineMk3Upgrade.CreateEngineHandler);
            MCUServices.Register.AuxCyclopsManager((SubRoot cyclops) =>
            {
                return new EngineManager(cyclops);
            });

            QuickLogger.Info($"Finished patching.");
        }
    }
}
