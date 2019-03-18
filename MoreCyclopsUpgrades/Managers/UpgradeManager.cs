namespace MoreCyclopsUpgrades.Managers
{
    using Caching;
    using Common;
    using Modules;
    using Monobehaviors;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class UpgradeManager
    {
        public static Dictionary<TechType, float> CyclopsCrushDepths => SubRoot.hullReinforcement;

        private class UpgradeSlot
        {
            internal Equipment Modules;
            internal string Slot;

            public UpgradeSlot(Equipment modules, string slot)
            {
                Modules = modules;
                Slot = slot;
            }
        }

        private readonly List<CyUpgradeConsoleMono> TempCache = new List<CyUpgradeConsoleMono>();

        internal float BonusCrushDepth { get; private set; } = 0f;

        internal bool HasChargingModules { get; private set; } = false;
        internal bool HasSolarModules => this.SolarModuleCount > 0 || this.SolarMk2Batteries.Count > 0;
        internal bool HasThermalModules => this.ThermalModuleCount > 0 || this.ThermalMk2Batteries.Count > 0;
        internal bool HasNuclearModules => this.NuclearModules.Count > 0;

        internal int PowerIndex { get; private set; } = 0;

        internal int SpeedBoosters => KnownsUpgradeModules[CyclopsModule.SpeedBoosterModuleID].Count;
        internal int SolarModuleCount => KnownsUpgradeModules[CyclopsModule.SolarChargerID].Count;
        internal int ThermalModuleCount => KnownsUpgradeModules[TechType.CyclopsThermalReactorModule].Count;
        internal int BioBoosterCount => KnownsUpgradeModules[CyclopsModule.BioReactorBoosterID].Count;

        internal IList<Battery> SolarMk2Batteries { get; } = new List<Battery>();
        internal IList<Battery> ThermalMk2Batteries { get; } = new List<Battery>();
        internal IList<NuclearModuleDetails> NuclearModules { get; } = new List<NuclearModuleDetails>();

        internal IEnumerable<Battery> ReserveBatteries
        {
            get
            {
                foreach (Battery battery in this.SolarMk2Batteries)
                    yield return battery;

                foreach (Battery battery in this.ThermalMk2Batteries)
                    yield return battery;

                foreach (NuclearModuleDetails module in this.NuclearModules)
                    yield return module.NuclearBattery;
            }
        }

        private IEnumerable<UpgradeSlot> UpgradeSlots
        {
            get
            {
                if (this.Cyclops.upgradeConsole != null)
                    foreach (string slot in SlotHelper.SlotNames)
                        yield return new UpgradeSlot(this.Cyclops.upgradeConsole.modules, slot);

                foreach (CyUpgradeConsoleMono aux in this.AuxUpgradeConsoles)
                    foreach (string slot in SlotHelper.SlotNames)
                        yield return new UpgradeSlot(aux.Modules, slot);
            }
        }

        internal CyclopsManager Manager { get; private set; }

        internal SubRoot Cyclops => this.Manager.Cyclops;

        internal List<CyUpgradeConsoleMono> AuxUpgradeConsoles { get; } = new List<CyUpgradeConsoleMono>();
        private CyclopsHolographicHUD holographicHUD = null;
        internal CyclopsHolographicHUD HolographicHUD => holographicHUD ?? (holographicHUD = this.Cyclops.GetComponentInChildren<CyclopsHolographicHUD>());

        private readonly Dictionary<TechType, CyclopsUpgrade> KnownsUpgradeModules = new Dictionary<TechType, CyclopsUpgrade>();

        internal bool Initialize(CyclopsManager manager)
        {
            if (this.Manager != null)
                return false; // Already initialized

            this.Manager = manager;

            RegisterUpgrades();

            holographicHUD = this.Cyclops.GetComponent<CyclopsHolographicHUD>();

            SyncUpgradeConsoles();

            return true;
        }

        public void RegisterKnownUpgrade(CyclopsUpgrade upgrade)
        {
            KnownsUpgradeModules.Add(upgrade.techType, upgrade);
        }

        private void RegisterUpgrades()
        {
            RegisterKnownUpgrade(new CyclopsUpgrade(TechType.CyclopsShieldModule)
            {
                OnClearUpgradesCyclops = (SubRoot cyclops) => { cyclops.shieldUpgrade = false; },
                OnUpgradeCountedByCyclops = (SubRoot cyclops) => { cyclops.shieldUpgrade = true; },
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(TechType.CyclopsSonarModule)
            {
                OnClearUpgradesCyclops = (SubRoot cyclops) => { cyclops.sonarUpgrade = false; },
                OnUpgradeCountedByCyclops = (SubRoot cyclops) => { cyclops.sonarUpgrade = true; },
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(TechType.CyclopsSeamothRepairModule)
            {
                OnClearUpgradesCyclops = (SubRoot cyclops) => { cyclops.vehicleRepairUpgrade = false; },
                OnUpgradeCountedByCyclops = (SubRoot cyclops) => { cyclops.vehicleRepairUpgrade = true; },
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(TechType.CyclopsDecoyModule)
            {
                OnClearUpgradesCyclops = (SubRoot cyclops) => { cyclops.decoyTubeSizeIncreaseUpgrade = false; },
                OnUpgradeCountedByCyclops = (SubRoot cyclops) => { cyclops.decoyTubeSizeIncreaseUpgrade = true; },
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(TechType.CyclopsFireSuppressionModule)
            {
                OnClearUpgrades = () =>
                {
                    if (this.HolographicHUD != null)
                        this.HolographicHUD.fireSuppressionSystem.SetActive(false);
                },
                OnUpgradeCounted = () =>
                {
                    if (this.HolographicHUD != null)
                        this.HolographicHUD.fireSuppressionSystem.SetActive(true);
                },
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(TechType.PowerUpgradeModule)
            {
                OnClearUpgrades = () =>
                {
                    this.PowerIndex = 0;
                },
                OnUpgradeCounted = () =>
                {
                    this.PowerIndex = Math.Max(this.PowerIndex, 1);
                },
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(CyclopsModule.PowerUpgradeMk2ID)
            {
                //OnClearUpgrades = () =>
                //{
                //    this.PowerIndex = 0; // Redundant with PowerUpgradeModule
                //},
                OnUpgradeCounted = () =>
                {
                    this.PowerIndex = Math.Max(this.PowerIndex, 2);
                },
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(CyclopsModule.PowerUpgradeMk3ID)
            {
                //OnClearUpgrades = () =>
                //{
                //    this.PowerIndex = 0; // Redundant with PowerUpgradeModule
                //},
                OnUpgradeCounted = () =>
                {
                    this.PowerIndex = Math.Max(this.PowerIndex, 3);
                },
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(TechType.CyclopsThermalReactorModule)
            {
                IsPowerProducer = true
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(CyclopsModule.SolarChargerID)
            {
                IsPowerProducer = true
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(CyclopsModule.SpeedBoosterModuleID));

            RegisterKnownUpgrade(new CyclopsUpgrade(CyclopsModule.BioReactorBoosterID)
            {
                OnFinishedUpgrades = UpdateBioReactors
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(CyclopsModule.SolarChargerMk2ID)
            {
                IsPowerProducer = true,
                OnClearUpgrades = () => { this.SolarMk2Batteries.Clear(); },
                OnUpgradeCountedBySlot = (Equipment modules, string slot) =>
                {
                    this.SolarMk2Batteries.Add(GetBatteryInSlot(modules, slot));
                }
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(CyclopsModule.ThermalChargerMk2ID)
            {
                IsPowerProducer = true,
                OnClearUpgrades = () => { this.ThermalMk2Batteries.Clear(); },
                OnUpgradeCountedBySlot = (Equipment modules, string slot) =>
                {
                    this.ThermalMk2Batteries.Add(GetBatteryInSlot(modules, slot));
                }
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(CyclopsModule.NuclearChargerID)
            {
                IsPowerProducer = true,
                OnClearUpgrades = () => { this.NuclearModules.Clear(); },
                OnUpgradeCountedBySlot = (Equipment modules, string slot) =>
                {
                    this.NuclearModules.Add(new NuclearModuleDetails(modules, slot, GetBatteryInSlot(modules, slot)));
                }
            });
        }

        internal void SyncUpgradeConsoles()
        {
            TempCache.Clear();

            CyUpgradeConsoleMono[] auxUpgradeConsoles = this.Cyclops.GetAllComponentsInChildren<CyUpgradeConsoleMono>();

            foreach (CyUpgradeConsoleMono auxConsole in auxUpgradeConsoles)
            {
                if (TempCache.Contains(auxConsole))
                    continue; // This is a workaround because of the object references being returned twice in this array.

                TempCache.Add(auxConsole);

                if (auxConsole.ParentCyclops == null)
                {
                    QuickLogger.Debug("CyUpgradeConsoleMono synced externally");
                    // This is a workaround to get a reference to the Cyclops into the AuxUpgradeConsole
                    auxConsole.ConnectToCyclops(this.Cyclops, this.Manager);
                }
            }

            if (TempCache.Count != this.AuxUpgradeConsoles.Count)
            {
                this.AuxUpgradeConsoles.Clear();
                this.AuxUpgradeConsoles.AddRange(TempCache);
            }

            HandleUpgrades();
        }

        private void ClearAllUpgrades()
        {
            if (this.Cyclops == null)
            {
                ErrorMessage.AddError("ClearAllUpgrades: Cyclops ref is null - Upgrade handling cancled");
                return;
            }

            SubRoot cyclops = this.Cyclops;

            foreach (CyclopsUpgrade upgradeType in KnownsUpgradeModules.Values)
                upgradeType.UpgradesCleared(cyclops);

            this.BonusCrushDepth = 0f;
            this.HasChargingModules = false;
        }

        private void UpdateBioReactors()
        {
            if (this.BioBoosterCount > CyBioReactorMono.MaxBoosters)
            {
                ErrorMessage.AddMessage("Cannot exceed maximum boost to bioreactors");
                return;
            }

            CyBioReactorMono lastRef = null;
            bool changedHappened = false;
            foreach (CyBioReactorMono reactor in this.Manager.BioReactors)
            {
                changedHappened |= (lastRef = reactor).UpdateBoosterCount(this.BioBoosterCount);
            }

            if (changedHappened && this.BioBoosterCount == CyBioReactorMono.MaxBoosters)
            {
                ErrorMessage.AddMessage("Maximum boost to bioreactors achieved");
            }
        }

        private void UpdateDepth(float bonusDepth)
        {
            this.BonusCrushDepth = Mathf.Max(this.BonusCrushDepth, bonusDepth);
        }

        internal void HandleUpgrades()
        {
            // Turn off all upgrades and clear all values
            ClearAllUpgrades();
            SubRoot cyclops = this.Cyclops;

            var foundUpgrades = new List<TechType>();

            foreach (UpgradeSlot upgradeSlot in this.UpgradeSlots)
            {
                Equipment modules = upgradeSlot.Modules;
                string slot = upgradeSlot.Slot;

                TechType techTypeInSlot = modules.GetTechTypeInSlot(slot);

                if (techTypeInSlot == TechType.None)
                    continue;

                foundUpgrades.Add(techTypeInSlot);

                if (CyclopsCrushDepths.TryGetValue(techTypeInSlot, out float bonusDepth))
                {
                    UpdateDepth(bonusDepth);
                    continue;
                }
                else if (KnownsUpgradeModules.TryGetValue(techTypeInSlot, out CyclopsUpgrade upgrade))
                {
                    upgrade.UpgradeCounted(cyclops, modules, slot);

                    if (upgrade.IsPowerProducer)
                        this.HasChargingModules = true;
                }
            }

            if (foundUpgrades.Count > 0)
            {
                this.Cyclops.slotModSFX?.Play();
                this.Cyclops.BroadcastMessage("RefreshUpgradeConsoleIcons", foundUpgrades.ToArray(), SendMessageOptions.RequireReceiver);

                foreach (CyclopsUpgrade upgradeType in KnownsUpgradeModules.Values)
                    upgradeType.UpgradesFinished(cyclops);
            }
        }

        /// <summary>
        /// Gets the battery of the upgrade module in the specified slot.
        /// </summary>
        /// <param name="modules">The equipment modules.</param>
        /// <param name="slotName">The slot name.</param>
        /// <returns>The <see cref="Battery"/> component from the upgrade module.</returns>
        internal static Battery GetBatteryInSlot(Equipment modules, string slotName)
        {
            // Get the battery component
            return modules.GetItemInSlot(slotName).item.GetComponent<Battery>();
        }
    }
}
