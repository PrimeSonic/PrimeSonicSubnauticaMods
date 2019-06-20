namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;

    internal class OriginalUpgrades
    {
        internal void RegisterOriginalUpgrades()
        {
            foreach (CreateUpgradeHandler creator in this.UpgradeCreators)
                MCUServices.Register.CyclopsUpgradeHandler(creator);
        }

        private IEnumerable<CreateUpgradeHandler> UpgradeCreators
        {
            get
            {
                yield return (SubRoot cyclops) =>
                {
                    QuickLogger.Debug("UpgradeHandler Registered: Depth Upgrades Collection");
                    return new CrushDepthUpgradesHandler(cyclops);
                };

                yield return (SubRoot cyclops) =>
                {
                    QuickLogger.Debug("UpgradeHandler Registered: CyclopsShieldModule");
                    return new UpgradeHandler(TechType.CyclopsShieldModule, cyclops)
                    {
                        OnClearUpgrades = () => { cyclops.shieldUpgrade = false; },
                        OnUpgradeCounted = (Equipment modules, string slot) => { cyclops.shieldUpgrade = true; },
                    };
                };

                yield return (SubRoot cyclops) =>
                {
                    QuickLogger.Debug("UpgradeHandler Registered: CyclopsSonarModule");
                    return new UpgradeHandler(TechType.CyclopsSonarModule, cyclops)
                    {
                        OnClearUpgrades = () => { cyclops.sonarUpgrade = false; },
                        OnUpgradeCounted = (Equipment modules, string slot) => { cyclops.sonarUpgrade = true; },
                    };
                };

                yield return (SubRoot cyclops) =>
                {
                    QuickLogger.Debug("UpgradeHandler Registered: CyclopsSeamothRepairModule");
                    return new UpgradeHandler(TechType.CyclopsSeamothRepairModule, cyclops)
                    {
                        OnClearUpgrades = () => { cyclops.vehicleRepairUpgrade = false; },
                        OnUpgradeCounted = (Equipment modules, string slot) => { cyclops.vehicleRepairUpgrade = true; },
                    };
                };

                yield return (SubRoot cyclops) =>
                {
                    QuickLogger.Debug("UpgradeHandler Registered: CyclopsDecoyModule");
                    return new UpgradeHandler(TechType.CyclopsDecoyModule, cyclops)
                    {
                        OnClearUpgrades = () => { cyclops.decoyTubeSizeIncreaseUpgrade = false; },
                        OnUpgradeCounted = (Equipment modules, string slot) => { cyclops.decoyTubeSizeIncreaseUpgrade = true; },
                    };
                };

                yield return (SubRoot cyclops) =>
                {
                    QuickLogger.Debug("UpgradeHandler Registered: CyclopsFireSuppressionModule");
                    return new UpgradeHandler(TechType.CyclopsFireSuppressionModule, cyclops)
                    {
                        OnClearUpgrades = () =>
                        {
                            CyclopsHolographicHUD fss = cyclops.GetComponentInChildren<CyclopsHolographicHUD>();
                            if (fss != null)
                                fss.fireSuppressionSystem.SetActive(false);
                        },
                        OnUpgradeCounted = (Equipment modules, string slot) =>
                        {
                            CyclopsHolographicHUD fss = cyclops.GetComponentInChildren<CyclopsHolographicHUD>();
                            if (fss != null)
                                fss.fireSuppressionSystem.SetActive(true);
                        },
                    };
                };
            }
        }
    }
}
