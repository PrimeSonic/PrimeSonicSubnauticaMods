namespace MoreCyclopsUpgrades.Caching
{
    using System;
    using System.Collections.Generic;
    using Monobehaviors;
    using MoreCyclopsUpgrades.Modules;
    using UnityEngine;

    internal class UpgradeManager : MonoBehaviour
    {
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

        private readonly List<Battery> SolarBatteries = new List<Battery>();
        private readonly List<Battery> ThermalBatteries = new List<Battery>();
        private readonly List<NuclearModuleDetails> NuclearReactorModules = new List<NuclearModuleDetails>();

        private List<AuxUpgradeConsole> TempCache = new List<AuxUpgradeConsole>();

        internal float BonusCrushDepth { get; private set; } = 0f;

        internal bool HasChargingModules { get; private set; } = false;
        internal bool HasSolarModules { get; private set; } = false;
        internal bool HasThermalModules { get; private set; } = false;
        internal bool HasNuclearModules { get; private set; } = false;

        internal int PowerIndex { get; private set; } = 0;

        internal int SpeedIndex { get; private set; } = 0;

        internal int SolarModuleCount { get; private set; } = 0;
        internal int ThermalModuleCount { get; private set; } = 0;

        internal IEnumerable<Battery> SolarMk2Batteries => SolarBatteries;
        internal IEnumerable<Battery> ThermalMk2Batteries => ThermalBatteries;
        internal IEnumerable<NuclearModuleDetails> NuclearModules => NuclearReactorModules;

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

                foreach (AuxUpgradeConsole aux in this.AuxUpgradeConsoles)
                    foreach (string slot in SlotHelper.SlotNames)
                        yield return new UpgradeSlot(aux.Modules, slot);
            }
        }

        private List<AuxUpgradeConsole> AuxUpgradeConsoles { get; } = new List<AuxUpgradeConsole>();
        private SubRoot Cyclops { get; set; } = null;
        private CyclopsHolographicHUD HolographicHUD { get; set; } = null;

        internal void Initialize(SubRoot cyclops)
        {
            if (this.Cyclops != null)
                return; // Already initialized

            this.Cyclops = cyclops;
            this.HolographicHUD = cyclops.GetComponentInChildren<CyclopsHolographicHUD>();

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

            SlotBoundUpgradeActions.Add(CyclopsModule.SolarChargerMk2ID, AddSolarMk2Module);
            SlotBoundUpgradeActions.Add(CyclopsModule.ThermalChargerMk2ID, AddThermalMk2Module);
            SlotBoundUpgradeActions.Add(CyclopsModule.NuclearChargerID, AddNuclearModule);
        }

        internal void SyncUpgradeConsoles(SubRoot cyclops)
        {
            if (cyclops is null)
                return;

            TempCache.Clear();

            AuxUpgradeConsole[] auxUpgradeConsoles = this.Cyclops.GetAllComponentsInChildren<AuxUpgradeConsole>();

            foreach (AuxUpgradeConsole auxConsole in auxUpgradeConsoles)
            {
                if (TempCache.Contains(auxConsole))
                    continue; // This is a workaround because of the object references being returned twice in this array.

                TempCache.Add(auxConsole);

                if (auxConsole.ParentCyclops == null)
                {
                    // This is a workaround to get a reference to the Cyclops into the AuxUpgradeConsole
                    auxConsole.ParentCyclops = cyclops;
                    ErrorMessage.AddMessage("Auxiliary Upgrade Console has been connected");
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
            // Turn off all toggleable upgrades first
            this.Cyclops.shieldUpgrade = false;
            this.Cyclops.sonarUpgrade = false;
            this.Cyclops.vehicleRepairUpgrade = false;
            this.Cyclops.decoyTubeSizeIncreaseUpgrade = false;

            // The fire suppression system is toggleable but isn't a field on the SubRoot class
            this.HolographicHUD.fireSuppressionSystem.SetActive(false);

            this.BonusCrushDepth = 0f;

            this.PowerIndex = 0;
            this.SpeedIndex = 0;

            this.SolarModuleCount = 0;
            this.ThermalModuleCount = 0;

            SolarBatteries.Clear();
            ThermalBatteries.Clear();
            NuclearReactorModules.Clear();

            this.HasChargingModules = false;
            this.HasSolarModules = false;
            this.HasThermalModules = false;
            this.HasNuclearModules = false;
        }

        private void AddSpeedModule() => this.SpeedIndex++;

        private void AddPowerMk1Module() => this.PowerIndex = Math.Max(this.PowerIndex, 1);

        private void AddPowerMk2Module() => this.PowerIndex = Math.Max(this.PowerIndex, 2);

        private void AddPowerMk3Module() => this.PowerIndex = Math.Max(this.PowerIndex, 3);

        private void AddSolarModule()
        {
            this.SolarModuleCount++;
            this.HasChargingModules = true;
            this.HasSolarModules = true;
        }

        private void AddThermalModule()
        {
            this.ThermalModuleCount++;
            this.HasChargingModules = true;
            this.HasThermalModules = true;
        }

        private void AddSolarMk2Module(Equipment modules, string slot)
        {
            Battery battery = PowerManager.GetBatteryInSlot(modules, slot);

            SolarBatteries.Add(battery);
            this.HasChargingModules = true;
            this.HasSolarModules = true;
        }

        private void AddThermalMk2Module(Equipment modules, string slot)
        {
            Battery battery = PowerManager.GetBatteryInSlot(modules, slot);

            ThermalBatteries.Add(battery);
            this.HasChargingModules = true;
            this.HasThermalModules = true;
        }

        private void AddNuclearModule(Equipment modules, string slot)
        {
            Battery battery = PowerManager.GetBatteryInSlot(modules, slot);

            NuclearReactorModules.Add(new NuclearModuleDetails(modules, slot, battery));
            this.HasChargingModules = true;
            this.HasNuclearModules = true;
        }

        private void AddDepthModule(TechType depthModule) => this.BonusCrushDepth = Mathf.Max(this.BonusCrushDepth, ExtraCrushDepths[depthModule]);

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
            }
        }

        private readonly Dictionary<TechType, Action> SimpleUpgradeActions = new Dictionary<TechType, Action>(12);
        private readonly Dictionary<TechType, Action<Equipment, string>> SlotBoundUpgradeActions = new Dictionary<TechType, Action<Equipment, string>>(12);

        private void EnableFireSuppressionSystem() => this.HolographicHUD.fireSuppressionSystem.SetActive(true);
        private void EnableExtraDecoySlots() => this.Cyclops.decoyTubeSizeIncreaseUpgrade = true;
        private void EnableRepairDock() => this.Cyclops.vehicleRepairUpgrade = true;
        private void EnableSonar() => this.Cyclops.sonarUpgrade = true;
        private void EnabledShield() => this.Cyclops.shieldUpgrade = true;

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
    }
}
