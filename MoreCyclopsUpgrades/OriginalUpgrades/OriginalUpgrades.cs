namespace MoreCyclopsUpgrades.OriginalUpgrades
{
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;

    internal class OriginalUpgrades
    {
        internal void RegisterOriginalUpgrades()
        {
            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: Depth Upgrades Collection");
                return new OriginalDepthUpgrades(cyclops);
            });

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: CyclopsShieldModule");
                return new UpgradeHandler(TechType.CyclopsShieldModule, cyclops)
                {
                    OnClearUpgrades = () => { cyclops.shieldUpgrade = false; },
                    OnUpgradeCounted = (Equipment modules, string slot) => { cyclops.shieldUpgrade = true; },
                };
            });

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: CyclopsSonarModule");
                return new UpgradeHandler(TechType.CyclopsSonarModule, cyclops)
                {
                    OnClearUpgrades = () => { cyclops.sonarUpgrade = false; },
                    OnUpgradeCounted = (Equipment modules, string slot) => { cyclops.sonarUpgrade = true; },
                };
            });

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: CyclopsSeamothRepairModule");
                return new UpgradeHandler(TechType.CyclopsSeamothRepairModule, cyclops)
                {
                    OnClearUpgrades = () => { cyclops.vehicleRepairUpgrade = false; },
                    OnUpgradeCounted = (Equipment modules, string slot) => { cyclops.vehicleRepairUpgrade = true; },
                };
            });

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: CyclopsDecoyModule");
                return new UpgradeHandler(TechType.CyclopsDecoyModule, cyclops)
                {
                    OnClearUpgrades = () => { cyclops.decoyTubeSizeIncreaseUpgrade = false; },
                    OnUpgradeCounted = (Equipment modules, string slot) => { cyclops.decoyTubeSizeIncreaseUpgrade = true; },
                };
            });

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: CyclopsFireSuppressionModule");
                return new OriginalFireSuppressionUpgrade(cyclops);
            });
        }
    }
}
