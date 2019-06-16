namespace CyclopsEngineUpgrades
{
    using Common;
    using CyclopsEngineUpgrades.Craftables;

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

            QuickLogger.Info($"Finished patching.");
        }
    }
}
