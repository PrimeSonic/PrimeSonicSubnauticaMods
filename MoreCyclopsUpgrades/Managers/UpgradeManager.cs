namespace MoreCyclopsUpgrades.Managers
{
    using Common;
    using Modules;
    using Monobehaviors;
    using MoreCyclopsUpgrades.CyclopsUpgrades;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The manager class that handles all upgrade events for a given Cyclops <see cref="SubRoot"/> instance.
    /// </summary>
    public class UpgradeManager
    {
        private static readonly List<CyclopsUpgrade> UpgradesToRegister = new List<CyclopsUpgrade>();

        /// <summary>
        /// Pre-Registered a new cyclops upgrade.
        /// </summary>
        /// <typeparam name="T">The <see cref="CyclopsUpgrade"/> type.</typeparam>
        /// <param name="upgrade">The cyclops upgrade instance.</param>
        public static void PreRegisterCyclopsUpgrade<T>(T upgrade) where T : CyclopsUpgrade
        {
            UpgradesToRegister.Add(upgrade);
        }

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

        internal bool HasChargingModules { get; private set; } = false;

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

        private readonly Dictionary<TechType, CyclopsUpgrade> KnownsUpgradeModules = new Dictionary<TechType, CyclopsUpgrade>();

        internal bool Initialize(CyclopsManager manager)
        {
            if (this.Manager != null)
                return false; // Already initialized

            this.Manager = manager;

            RegisterUpgrades();

            SyncUpgradeConsoles();

            return true;
        }

        private void RegisterUpgrades()
        {
            PowerManager powerManager = this.Manager.PowerManager;
            var crushDepthUpgrades = new CrushDepthUpgrades();
            crushDepthUpgrades.RegisterSelf(KnownsUpgradeModules);

            powerManager.EngineEfficientyUpgrades = new TieredCyclopsUpgradeCollection<int>(0);
            powerManager.EngineEfficientyUpgrades.CreateTier(TechType.PowerUpgradeModule, 1);
            powerManager.EngineEfficientyUpgrades.CreateTier(CyclopsModule.PowerUpgradeMk2ID, 2);
            powerManager.EngineEfficientyUpgrades.CreateTier(CyclopsModule.PowerUpgradeMk3ID, 3);

            powerManager.EngineEfficientyUpgrades.RegisterSelf(KnownsUpgradeModules);

            var shield = new CyclopsUpgrade(TechType.CyclopsShieldModule)
            {
                OnClearUpgrades = (SubRoot cyclops) => { cyclops.shieldUpgrade = false; },
                OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) => { cyclops.shieldUpgrade = true; },
            };
            shield.RegisterSelf(KnownsUpgradeModules);

            var sonar = new CyclopsUpgrade(TechType.CyclopsSonarModule)
            {
                OnClearUpgrades = (SubRoot cyclops) => { cyclops.sonarUpgrade = false; },
                OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) => { cyclops.sonarUpgrade = true; },
            };
            sonar.RegisterSelf(KnownsUpgradeModules);

            var repair = new CyclopsUpgrade(TechType.CyclopsSeamothRepairModule)
            {
                OnClearUpgrades = (SubRoot cyclops) => { cyclops.vehicleRepairUpgrade = false; },
                OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) => { cyclops.vehicleRepairUpgrade = true; },
            };
            repair.RegisterSelf(KnownsUpgradeModules);

            var decoy = new CyclopsUpgrade(TechType.CyclopsDecoyModule)
            {
                OnClearUpgrades = (SubRoot cyclops) => { cyclops.decoyTubeSizeIncreaseUpgrade = false; },
                OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) => { cyclops.decoyTubeSizeIncreaseUpgrade = true; },
            };
            decoy.RegisterSelf(KnownsUpgradeModules);

            var fire = new CyclopsUpgrade(TechType.CyclopsFireSuppressionModule)
            {
                OnClearUpgrades = (SubRoot cyclops) =>
                {
                    CyclopsHolographicHUD fss = cyclops.GetComponentInChildren<CyclopsHolographicHUD>();
                    if (fss != null)
                        fss.fireSuppressionSystem.SetActive(false);
                },
                OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) =>
                {
                    CyclopsHolographicHUD fss = cyclops.GetComponentInChildren<CyclopsHolographicHUD>();
                    if (fss != null)
                        fss.fireSuppressionSystem.SetActive(true);
                },
            };
            fire.RegisterSelf(KnownsUpgradeModules);

            powerManager.SolarCharger = new ChargingCyclopsUpgrade(CyclopsModule.SolarChargerID);
            powerManager.SolarCharger.RegisterSelf(KnownsUpgradeModules);

            powerManager.SolarChargerMk2 = new BatteryCyclopsUpgrade(CyclopsModule.SolarChargerMk2ID, true);
            powerManager.SolarChargerMk2.RegisterSelf(KnownsUpgradeModules);

            powerManager.ThermalCharger = new ChargingCyclopsUpgrade(TechType.CyclopsThermalReactorModule);
            powerManager.ThermalCharger.RegisterSelf(KnownsUpgradeModules);

            powerManager.ThermalChargerMk2 = new BatteryCyclopsUpgrade(CyclopsModule.ThermalChargerMk2ID, true);
            powerManager.ThermalChargerMk2.RegisterSelf(KnownsUpgradeModules);

            powerManager.NuclearCharger = new BatteryCyclopsUpgrade(CyclopsModule.NuclearChargerID, false)
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
            };
            powerManager.NuclearCharger.RegisterSelf(KnownsUpgradeModules);

            var speed = new CyclopsUpgrade(CyclopsModule.SpeedBoosterModuleID)
            {
                MaxCount = 6
            };
            speed.RegisterSelf(KnownsUpgradeModules);

            powerManager.BioBoosters = new BioBoosterUpgrade();
            powerManager.BioBoosters.RegisterSelf(KnownsUpgradeModules);

            // Register upgrades from other mods
            foreach (CyclopsUpgrade externalUpgrade in UpgradesToRegister)
            {
                if (externalUpgrade.techType != TechType.None)
                    externalUpgrade.RegisterSelf(KnownsUpgradeModules);
            }
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

            this.HasChargingModules = false;
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
