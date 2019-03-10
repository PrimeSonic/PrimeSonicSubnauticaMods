namespace MoreCyclopsUpgrades.Managers
{
    using System;
    using System.Collections.Generic;
    using Caching;
    using Common;
    using Modules;
    using Monobehaviors;
    using UnityEngine;

    internal class UpgradeManager
    {
        // This is a straight copy of the values in the original
        private static readonly Dictionary<TechType, float> ExtraCrushDepths = new Dictionary<TechType, float>
        {
            { TechType.HullReinforcementModule, 800f },
            { TechType.HullReinforcementModule2, 1600f },
            { TechType.HullReinforcementModule3, 2800f },
            { TechType.CyclopsHullModule1, 400f },
            { TechType.CyclopsHullModule2, 800f },
            { TechType.CyclopsHullModule3, 1200f }
        };

        private struct UpgradeSlot
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

        internal int SpeedBoosters { get; private set; } = 0;

        internal int SolarModuleCount { get; private set; } = 0;
        internal int ThermalModuleCount { get; private set; } = 0;

        internal int BioBoosterCount { get; private set; } = 0;

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

        public CyclopsManager Manager { get; private set; }

        public SubRoot Cyclops => this.Manager.Cyclops;

        internal List<CyUpgradeConsoleMono> AuxUpgradeConsoles { get; } = new List<CyUpgradeConsoleMono>();
        private CyclopsHolographicHUD holographicHUD = null;
        internal CyclopsHolographicHUD HolographicHUD => holographicHUD ?? (holographicHUD = this.Cyclops.GetComponentInChildren<CyclopsHolographicHUD>());

        private readonly Dictionary<TechType, Action> SimpleUpgradeActions = new Dictionary<TechType, Action>(12);
        private readonly Dictionary<TechType, Action<Equipment, string>> SlotBoundUpgradeActions = new Dictionary<TechType, Action<Equipment, string>>(12);
        private readonly HashSet<TechType> ChargingModules = new HashSet<TechType>();

        internal bool Initialize(CyclopsManager manager)
        {
            if (this.Manager != null)
                return false; // Already initialized

            this.Manager = manager;

            SimpleUpgradeActions.Add(TechType.CyclopsShieldModule, EnabledShield);
            SimpleUpgradeActions.Add(TechType.CyclopsSonarModule, EnableSonar);
            SimpleUpgradeActions.Add(TechType.CyclopsSeamothRepairModule, EnableRepairDock);
            SimpleUpgradeActions.Add(TechType.CyclopsDecoyModule, EnableExtraDecoySlots);
            SimpleUpgradeActions.Add(TechType.CyclopsFireSuppressionModule, EnableFireSuppressionSystem);
            SimpleUpgradeActions.Add(TechType.CyclopsThermalReactorModule, AddThermalModule);
            SimpleUpgradeActions.Add(TechType.PowerUpgradeModule, AddPowerMk1Module);

            SimpleUpgradeActions.Add(CyclopsModule.SolarChargerID, AddSolarModule);
            SimpleUpgradeActions.Add(CyclopsModule.SpeedBoosterModuleID, AddSpeedModule);
            SimpleUpgradeActions.Add(CyclopsModule.PowerUpgradeMk2ID, AddPowerMk2Module);
            SimpleUpgradeActions.Add(CyclopsModule.PowerUpgradeMk3ID, AddPowerMk3Module);
            SimpleUpgradeActions.Add(CyclopsModule.BioReactorBoosterID, AddBioBooster);

            SlotBoundUpgradeActions.Add(CyclopsModule.SolarChargerMk2ID, AddSolarMk2Module);
            SlotBoundUpgradeActions.Add(CyclopsModule.ThermalChargerMk2ID, AddThermalMk2Module);
            SlotBoundUpgradeActions.Add(CyclopsModule.NuclearChargerID, AddNuclearModule);

            ChargingModules.Add(CyclopsModule.SolarChargerID);
            ChargingModules.Add(CyclopsModule.SolarChargerMk2ID);
            ChargingModules.Add(TechType.CyclopsThermalReactorModule);
            ChargingModules.Add(CyclopsModule.ThermalChargerMk2ID);
            ChargingModules.Add(CyclopsModule.NuclearChargerID);

            SyncUpgradeConsoles();

            return true;
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
                    auxConsole.ConnectToCyclops(this.Cyclops);
                }
            }

            if (TempCache.Count != this.AuxUpgradeConsoles.Count)
            {
                this.AuxUpgradeConsoles.Clear();
                this.AuxUpgradeConsoles.AddRange(TempCache);
            }
        }

        private void ClearAllUpgrades()
        {
            if (this.Cyclops == null)
                QuickLogger.Error("ClearAllUpgrades: Cyclops ref is null", true);

            // Turn off all toggleable upgrades first
            this.Cyclops.shieldUpgrade = false;
            this.Cyclops.sonarUpgrade = false;
            this.Cyclops.vehicleRepairUpgrade = false;
            this.Cyclops.decoyTubeSizeIncreaseUpgrade = false;
            this.Cyclops.thermalReactorUpgrade = false;

            if (this.HolographicHUD == null)
                QuickLogger.Error("ClearAllUpgrades: HolographicHUD ref is null", true);

            // The fire suppression system is toggleable but isn't a field on the SubRoot class
            this.HolographicHUD.fireSuppressionSystem.SetActive(false);

            this.BonusCrushDepth = 0f;

            this.PowerIndex = 0;
            this.SpeedBoosters = 0;

            this.SolarModuleCount = 0;
            this.ThermalModuleCount = 0;
            this.BioBoosterCount = 0;

            this.SolarMk2Batteries.Clear();
            this.ThermalMk2Batteries.Clear();
            this.NuclearModules.Clear();

            this.HasChargingModules = false;
        }

        private void AddSpeedModule()
        {
            ++this.SpeedBoosters;
        }

        private void AddPowerMk1Module()
        {
            this.PowerIndex = Math.Max(this.PowerIndex, 1);
        }

        private void AddPowerMk2Module()
        {
            this.PowerIndex = Math.Max(this.PowerIndex, 2);
        }

        private void AddPowerMk3Module()
        {
            this.PowerIndex = Math.Max(this.PowerIndex, 3);
        }

        private void AddSolarModule()
        {
            ++this.SolarModuleCount;
        }

        private void AddThermalModule()
        {
            ++this.ThermalModuleCount;
        }

        private void AddBioBooster()
        {
            ++this.BioBoosterCount;            
        }

        private void UpdateBioReactors()
        {
            if (this.BioBoosterCount > CyBioReactorMono.MaxBoosters)
            {
                ErrorMessage.AddMessage("Cannot exceed maximum boost to bioreactors");
                return;
            }

            CyBioReactorMono lastRef = null;
            foreach (CyBioReactorMono reactor in this.Manager.BioReactors)
            {
                (lastRef = reactor).UpdateBoosterCount(this.BioBoosterCount);
            }

            if (this.BioBoosterCount == CyBioReactorMono.MaxBoosters)
            {
                ErrorMessage.AddMessage("Maximum boost to bioreactors achieved");
            }
        }

        private void AddSolarMk2Module(Equipment modules, string slot)
        {
            this.SolarMk2Batteries.Add(GetBatteryInSlot(modules, slot));
        }

        private void AddThermalMk2Module(Equipment modules, string slot)
        {
            this.ThermalMk2Batteries.Add(GetBatteryInSlot(modules, slot));
        }

        private void AddNuclearModule(Equipment modules, string slot)
        {
            this.NuclearModules.Add(new NuclearModuleDetails(modules, slot, GetBatteryInSlot(modules, slot)));
        }

        private void AddDepthModule(TechType depthModule)
        {
            this.BonusCrushDepth = Mathf.Max(this.BonusCrushDepth, ExtraCrushDepths[depthModule]);
        }

        private void EnableFireSuppressionSystem()
        {
            this.HolographicHUD.fireSuppressionSystem.SetActive(true);
        }

        private void EnableExtraDecoySlots()
        {
            this.Cyclops.decoyTubeSizeIncreaseUpgrade = true;
        }

        private void EnableRepairDock()
        {
            this.Cyclops.vehicleRepairUpgrade = true;
        }

        private void EnableSonar()
        {
            this.Cyclops.sonarUpgrade = true;
        }

        private void EnabledShield()
        {
            this.Cyclops.shieldUpgrade = true;
        }

        internal void HandleUpgrades()
        {
            // Turn off all upgrades and clear all values
            ClearAllUpgrades();

            var foundUpgrades = new List<TechType>();

            foreach (UpgradeSlot upgradeSlot in this.UpgradeSlots)
            {
                Equipment modules = upgradeSlot.Modules;
                string slot = upgradeSlot.Slot;

                TechType techTypeInSlot = modules.GetTechTypeInSlot(slot);

                if (techTypeInSlot == TechType.None)
                    continue;

                foundUpgrades.Add(techTypeInSlot);

                this.HasChargingModules |= ChargingModules.Contains(techTypeInSlot);

                if (SimpleUpgradeActions.TryGetValue(techTypeInSlot, out Action simpleUpgrade))
                {
                    simpleUpgrade.Invoke();
                    continue;
                }

                switch (techTypeInSlot)
                {
                    case TechType.HullReinforcementModule:
                    case TechType.HullReinforcementModule2:
                    case TechType.HullReinforcementModule3:
                    case TechType.CyclopsHullModule1:
                    case TechType.CyclopsHullModule2:
                    case TechType.CyclopsHullModule3:
                        AddDepthModule(techTypeInSlot);
                        continue;
                }

                if (SlotBoundUpgradeActions.TryGetValue(techTypeInSlot, out Action<Equipment, string> batteryUpgrade))
                {
                    batteryUpgrade.Invoke(modules, slot);
                    continue;
                }
            }

            if (foundUpgrades.Count > 0)
            {
                this.Cyclops.slotModSFX?.Play();
                this.Cyclops.BroadcastMessage("RefreshUpgradeConsoleIcons", foundUpgrades.ToArray(), SendMessageOptions.RequireReceiver);
                UpdateBioReactors();
            }
        }

        /// <summary>
        /// Gets the battery of the upgrade module in the specified slot.
        /// </summary>
        /// <param name="modules">The equipment modules.</param>
        /// <param name="slotName">The slot name.</param>
        /// <returns>The <see cref="Battery"/> component from the upgrade module.</returns>
        private static Battery GetBatteryInSlot(Equipment modules, string slotName)
        {
            // Get the battery component
            return modules.GetItemInSlot(slotName).item.GetComponent<Battery>();
        }
    }
}
