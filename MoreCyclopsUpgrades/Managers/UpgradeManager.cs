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
        public static UpgradeHandlerCreateEvent ExternalUpgradeHandlerCreator;

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

        private readonly Dictionary<TechType, UpgradeHandler> KnownsUpgradeModules = new Dictionary<TechType, UpgradeHandler>();

        internal bool Initialize(CyclopsManager manager)
        {
            if (this.Manager != null)
                return false; // Already initialized

            this.Manager = manager;

            SetupPowerManagerUpgrades();

            RegisterUpgradeHandlers();

            SyncUpgradeConsoles();

            return true;
        }

        private void SetupPowerManagerUpgrades()
        {
            PowerManager powerManager = this.Manager.PowerManager;

            ExternalUpgradeHandlerCreator += () =>
            {
                var efficiencyUpgrades = new TieredUpgradeHandlerCollection<int>(0);
                efficiencyUpgrades.CreateTier(TechType.PowerUpgradeModule, 1);
                efficiencyUpgrades.CreateTier(CyclopsModule.PowerUpgradeMk2ID, 2);
                efficiencyUpgrades.CreateTier(CyclopsModule.PowerUpgradeMk3ID, 3);

                powerManager.EngineEfficientyUpgrades = efficiencyUpgrades;
                return efficiencyUpgrades;
            };

            ExternalUpgradeHandlerCreator += () =>
            {
                var speed = new UpgradeHandler(CyclopsModule.SpeedBoosterModuleID)
                {
                    MaxCount = 6,
                };
                powerManager.SpeedBoosters = speed;
                return speed;
            };

            ExternalUpgradeHandlerCreator += () =>
            {
                var solarMk1 = new ChargingUpgradeHandler(CyclopsModule.SolarChargerID);
                powerManager.SolarCharger = solarMk1;
                return solarMk1;
            };

            ExternalUpgradeHandlerCreator += () =>
            {
                var solarMk2 = new BatteryCyclopsUpgradeHandler(CyclopsModule.SolarChargerMk2ID, canRecharge: true);
                powerManager.SolarChargerMk2 = solarMk2;
                return solarMk2;
            };

            ExternalUpgradeHandlerCreator += () =>
            {
                var thermalMk1 = new ChargingUpgradeHandler(TechType.CyclopsThermalReactorModule);
                powerManager.ThermalCharger = thermalMk1;
                return thermalMk1;
            };

            ExternalUpgradeHandlerCreator += () =>
            {
                var thermalMk2 = new BatteryCyclopsUpgradeHandler(CyclopsModule.ThermalChargerMk2ID, canRecharge: true);
                powerManager.ThermalChargerMk2 = thermalMk2;
                return thermalMk2;
            };

            ExternalUpgradeHandlerCreator += () =>
            {
                var nuclear = new NuclearUpgradeHandler();
                powerManager.NuclearCharger = nuclear;
                return nuclear;
            };

            ExternalUpgradeHandlerCreator += () =>
            {
                var bioBoost = new BioBoosterUpgradeHandler();
                powerManager.BioBoosters = bioBoost;
                return bioBoost;
            };
        }


        private void RegisterUpgradeHandlers()
        {
            // Register upgrades from other mods
            foreach (Delegate externalMethod in ExternalUpgradeHandlerCreator.GetInvocationList())
            {
                if (externalMethod is UpgradeHandlerCreateEvent upgradeHandlerCreator)
                {
                    UpgradeHandler upgrade = upgradeHandlerCreator.Invoke();
                    upgrade.RegisterSelf(KnownsUpgradeModules);
                }
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

        internal void HandleUpgrades()
        {
            // Turn off all upgrades and clear all values
            if (this.Cyclops == null)
            {
                ErrorMessage.AddError("ClearAllUpgrades: Cyclops ref is null - Upgrade handling cancled");
                return;
            }

            foreach (UpgradeHandler upgradeType in KnownsUpgradeModules.Values)
                upgradeType.UpgradesCleared(this.Cyclops);

            this.HasChargingModules = false;

            var foundUpgrades = new List<TechType>();

            foreach (UpgradeSlot upgradeSlot in this.UpgradeSlots)
            {
                Equipment modules = upgradeSlot.Modules;
                string slot = upgradeSlot.Slot;

                TechType techTypeInSlot = modules.GetTechTypeInSlot(slot);

                if (techTypeInSlot == TechType.None)
                    continue;

                foundUpgrades.Add(techTypeInSlot);

                if (KnownsUpgradeModules.TryGetValue(techTypeInSlot, out UpgradeHandler upgrade))
                {
                    upgrade.UpgradeCounted(this.Cyclops, modules, slot);

                    if (upgrade.IsPowerProducer)
                        this.HasChargingModules = true;
                }
            }

            if (foundUpgrades.Count > 0)
            {
                this.Cyclops.slotModSFX?.Play();
                this.Cyclops.BroadcastMessage("RefreshUpgradeConsoleIcons", foundUpgrades.ToArray(), SendMessageOptions.RequireReceiver);

                foreach (UpgradeHandler upgradeType in KnownsUpgradeModules.Values)
                    upgradeType.UpgradesFinished(this.Cyclops);
            }
        }
    }
}
