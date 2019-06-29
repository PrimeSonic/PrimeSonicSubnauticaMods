namespace MoreCyclopsUpgrades.OriginalUpgrades
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;

    internal class OriginalUpgrades
    {
        internal IEnumerable<TechType> OriginalUpgradeIDs => originalUpgrades.Keys;

        internal UpgradeHandler CreateUpgradeHandler(TechType upgradeID, SubRoot cyclops)
        {
            return originalUpgrades[upgradeID].Invoke(cyclops);
        }

        private readonly Dictionary<TechType, CreateUpgradeHandler> originalUpgrades = new Dictionary<TechType, CreateUpgradeHandler>()
        {
            {
                TechType.CyclopsHullModule1, (SubRoot cyclops) =>
                {
                    var chm = new TieredGroupHandler<float>(0f, cyclops);
                    chm.OnFinishedUpgrades = () =>
                    {
                        cyclops.gameObject.GetComponent<CrushDamage>().SetExtraCrushDepth(chm.HighestValue);
                    };
                    chm.CreateTier(TechType.CyclopsHullModule1, 400f);
                    chm.CreateTier(TechType.CyclopsHullModule2, 800f);
                    chm.CreateTier(TechType.CyclopsHullModule3, 1200f);

                    return chm;
                }
            },
            {
                TechType.PowerUpgradeModule, (SubRoot cyclops) =>
                {
                    float lastKnownRating = -1f;
                    var pum = new UpgradeHandler(TechType.PowerUpgradeModule, cyclops)
                    {
                        OnClearUpgrades = () =>
                        {
                            lastKnownRating = cyclops.currPowerRating;
                            MCUServices.CrossMod.ApplyPowerRatingModifier(cyclops, TechType.PowerUpgradeModule, 1f);
                        },
                        OnUpgradeCountedDetailed = (Equipment modules, string slot, InventoryItem inventoryItem) =>
                        {
                            MCUServices.CrossMod.ApplyPowerRatingModifier(cyclops, TechType.PowerUpgradeModule, 3f);
                        },
                        OnFinishedUpgrades = () =>
                        {
                            if (lastKnownRating != cyclops.currPowerRating)
                            {
                                // Inform the new power rating just like the original method would.
                                ErrorMessage.AddMessage(Language.main.GetFormat("PowerRatingNowFormat", cyclops.currPowerRating));
                            }
                        }
                    };

                    return pum;
                }
            },
            {
                TechType.CyclopsShieldModule, (SubRoot cyclops) =>
                {
                    var csm = new UpgradeHandler(TechType.CyclopsShieldModule, cyclops);
                    csm.OnFinishedUpgrades = () => 
                    {
                        cyclops.shieldUpgrade = csm.HasUpgrade;
                    };
                    return csm;
                }
            },
            {
                TechType.CyclopsSonarModule, (SubRoot cyclops) =>
                {
                    var csm = new UpgradeHandler(TechType.CyclopsSonarModule, cyclops);
                    csm.OnFinishedUpgrades = () =>
                    {
                        cyclops.sonarUpgrade = csm.HasUpgrade;
                    };
                    return csm;
                }
            },
            {
                TechType.CyclopsSeamothRepairModule, (SubRoot cyclops) =>
                {
                    var csrm = new UpgradeHandler(TechType.CyclopsSeamothRepairModule, cyclops);
                    csrm.OnFinishedUpgrades = () =>
                    {
                        cyclops.vehicleRepairUpgrade = csrm.HasUpgrade;
                    };
                    return csrm;
                }
            },
            {
                TechType.CyclopsDecoyModule, (SubRoot cyclops) =>
                {
                    var cdm = new UpgradeHandler(TechType.CyclopsDecoyModule, cyclops);
                    cdm.OnFinishedUpgrades = () =>
                    {
                        cyclops.decoyTubeSizeIncreaseUpgrade = cdm.HasUpgrade;
                    };
                    return cdm;
                }
            },
            {
                TechType.CyclopsFireSuppressionModule, (SubRoot cyclops) =>
                {
                    var fsm = new UpgradeHandler(TechType.CyclopsFireSuppressionModule, cyclops);
                    fsm.OnFinishedUpgrades = () =>
                    {
                        cyclops.GetComponentInChildren<CyclopsHolographicHUD>()?.fireSuppressionSystem.SetActive(fsm.HasUpgrade);
                    };
                    return fsm;
                }
            }
        };
    }
}
