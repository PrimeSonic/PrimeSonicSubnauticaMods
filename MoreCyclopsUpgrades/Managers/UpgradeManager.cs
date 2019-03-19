namespace MoreCyclopsUpgrades.Managers
{
    using Common;
    using Modules;
    using Monobehaviors;
    using MoreCyclopsUpgrades.CyclopsUpgrades;
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

        internal TieredCyclopsUpgradeCollection<float> CrushDepthUpgrades { get; private set; }
        internal TieredCyclopsUpgradeCollection<int> EngineEfficientyUpgrades { get; private set; }

        internal bool HasChargingModules { get; private set; } = false;
        internal bool HasSolarModules => this.SolarModuleCount > 0 || this.SolarChargerMk2.Count > 0;
        internal bool HasThermalModules => this.ThermalModuleCount > 0 || this.ThermalChargerMk2.Count > 0;
        internal bool HasNuclearModules => this.NuclearCharger.Count > 0;

        internal int SpeedBoosters => KnownsUpgradeModules[CyclopsModule.SpeedBoosterModuleID].Count;
        internal int SolarModuleCount => KnownsUpgradeModules[CyclopsModule.SolarChargerID].Count;
        internal int ThermalModuleCount => KnownsUpgradeModules[TechType.CyclopsThermalReactorModule].Count;
        internal int BioBoosterCount => KnownsUpgradeModules[CyclopsModule.BioReactorBoosterID].Count;

        internal ChargingCyclopsUpgrade SolarCharger { get; private set; }
        internal ChargingCyclopsUpgrade ThermalCharger { get; private set; }
        internal BatteryCyclopsUpgrade SolarChargerMk2 { get; private set; }
        internal BatteryCyclopsUpgrade ThermalChargerMk2 { get; private set; }
        internal BatteryCyclopsUpgrade NuclearCharger { get; private set; }

        internal IEnumerable<Battery> ReserveBatteries()
        {
            foreach (BatteryDetails details in this.SolarChargerMk2.Batteries)
                yield return details.BatteryRef;

            foreach (BatteryDetails details in this.ThermalChargerMk2.Batteries)
                yield return details.BatteryRef;

            foreach (BatteryDetails details in this.NuclearCharger.Batteries)
                yield return details.BatteryRef;
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

        public T RegisterKnownUpgrade<T>(T upgrade) where T : CyclopsUpgrade
        {
            KnownsUpgradeModules.Add(upgrade.techType, upgrade);
            return upgrade;
        }

        private void RegisterUpgrades()
        {
            this.CrushDepthUpgrades = new TieredCyclopsUpgradeCollection<float>(0f);
            foreach (KeyValuePair<TechType, float> depthValue in CyclopsCrushDepths)
            {
                RegisterKnownUpgrade(this.CrushDepthUpgrades.Create(depthValue.Key, depthValue.Value));
            }

            this.EngineEfficientyUpgrades = new TieredCyclopsUpgradeCollection<int>(0);
            RegisterKnownUpgrade(this.EngineEfficientyUpgrades.Create(TechType.PowerUpgradeModule, 1));
            RegisterKnownUpgrade(this.EngineEfficientyUpgrades.Create(CyclopsModule.PowerUpgradeMk2ID, 2));
            RegisterKnownUpgrade(this.EngineEfficientyUpgrades.Create(CyclopsModule.PowerUpgradeMk3ID, 3));

            RegisterKnownUpgrade(new CyclopsUpgrade(TechType.CyclopsShieldModule)
            {
                OnClearUpgrades = (SubRoot cyclops) => { cyclops.shieldUpgrade = false; },
                OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) => { cyclops.shieldUpgrade = true; },
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(TechType.CyclopsSonarModule)
            {
                OnClearUpgrades = (SubRoot cyclops) => { cyclops.sonarUpgrade = false; },
                OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) => { cyclops.sonarUpgrade = true; },
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(TechType.CyclopsSeamothRepairModule)
            {
                OnClearUpgrades = (SubRoot cyclops) => { cyclops.vehicleRepairUpgrade = false; },
                OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) => { cyclops.vehicleRepairUpgrade = true; },
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(TechType.CyclopsDecoyModule)
            {
                OnClearUpgrades = (SubRoot cyclops) => { cyclops.decoyTubeSizeIncreaseUpgrade = false; },
                OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) => { cyclops.decoyTubeSizeIncreaseUpgrade = true; },
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(TechType.CyclopsFireSuppressionModule)
            {
                OnClearUpgrades = (SubRoot cyclops) =>
                {
                    if (this.HolographicHUD != null)
                        this.HolographicHUD.fireSuppressionSystem.SetActive(false);
                },
                OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) =>
                {
                    if (this.HolographicHUD != null)
                        this.HolographicHUD.fireSuppressionSystem.SetActive(true);
                },
            });

            this.SolarCharger = RegisterKnownUpgrade(new ChargingCyclopsUpgrade(CyclopsModule.SolarChargerID));

            this.SolarChargerMk2 = RegisterKnownUpgrade(new BatteryCyclopsUpgrade(CyclopsModule.SolarChargerMk2ID, true));

            this.ThermalCharger = RegisterKnownUpgrade(new ChargingCyclopsUpgrade(TechType.CyclopsThermalReactorModule));

            this.ThermalChargerMk2 = RegisterKnownUpgrade(new BatteryCyclopsUpgrade(CyclopsModule.ThermalChargerMk2ID, true));

            this.NuclearCharger = RegisterKnownUpgrade(new BatteryCyclopsUpgrade(CyclopsModule.NuclearChargerID, false)
            {
                OnBatteryDrained = (BatteryDetails details) =>
                {
                    Equipment modules = details.ParentEquipment;
                    string slotName = details.SlotName;
                    // Drained nuclear batteries are handled just like how the Nuclear Reactor handles depleated reactor rods
                    InventoryItem inventoryItem = modules.RemoveItem(slotName, true, false);
                    GameObject.Destroy(inventoryItem.item.gameObject);
                    modules.AddItem(slotName, CyclopsModule.SpawnCyclopsModule(CyclopsModule.DepletedNuclearModuleID), true);
                    ErrorMessage.AddMessage("Nuclear Reactor Module depleted");
                }
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(CyclopsModule.SpeedBoosterModuleID)
            {
                MaxCount = 6
            });

            RegisterKnownUpgrade(new CyclopsUpgrade(CyclopsModule.BioReactorBoosterID)
            {
                OnFinishedUpgrades = (SubRoot cyclops) => { UpdateBioReactors(); }
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

            this.CrushDepthUpgrades.ResetValue();
            this.EngineEfficientyUpgrades.ResetValue();

            foreach (CyclopsUpgrade upgradeType in KnownsUpgradeModules.Values)
                upgradeType.UpgradesCleared(cyclops);

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

                if (KnownsUpgradeModules.TryGetValue(techTypeInSlot, out CyclopsUpgrade upgrade))
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
    }
}
