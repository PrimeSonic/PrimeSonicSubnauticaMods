namespace CyclopsSpeedUpgrades
{
    using Common;
    using MoreCyclopsUpgrades.API;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            var speedUpgrade = new CyclopsSpeedModule();
            speedUpgrade.Patch();

            MCUServices.Register.CyclopsUpgradeHandler(speedUpgrade.CreateSpeedUpgradeHandler);
            MCUServices.Register.PdaIconOverlay(speedUpgrade.TechType, speedUpgrade.CreateSpeedIconOverlay);

            QuickLogger.Info($"Finished patching.");
        }
    }
}
