namespace CyclopsSpeedUpgrades
{
    using Common;
    using CyclopsSpeedUpgrades.Craftables;
    using MoreCyclopsUpgrades.API;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            var speedUpgrade = new CyclopsSpeedModule();
            speedUpgrade.Patch();

            MCUServices.Register.CyclopsUpgradeHandler(speedUpgrade.CreateSpeedUpgradeHandler);

            QuickLogger.Info($"Finished patching.");
        }
    }
}
