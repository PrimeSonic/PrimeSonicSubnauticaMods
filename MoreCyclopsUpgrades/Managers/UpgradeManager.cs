namespace MoreCyclopsUpgrades.Managers
{
    using Common;
    using Modules;
    using Monobehaviors;
    using MoreCyclopsUpgrades.CyclopsUpgrades;
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

            var efficiencyUpgrades = new TieredCyclopsUpgradeCollection<int>(0);
            efficiencyUpgrades.CreateTier(TechType.PowerUpgradeModule, 1);
            efficiencyUpgrades.CreateTier(CyclopsModule.PowerUpgradeMk2ID, 2);
            efficiencyUpgrades.CreateTier(CyclopsModule.PowerUpgradeMk3ID, 3);
            efficiencyUpgrades.RegisterSelf(KnownsUpgradeModules);

            powerManager.EngineEfficientyUpgrades = efficiencyUpgrades;

            var speed = new CyclopsUpgrade(CyclopsModule.SpeedBoosterModuleID)
            {
                MaxCount = 6,
            };
            speed.RegisterSelf(KnownsUpgradeModules);
            powerManager.SpeedBoosters = speed;

            var solarMk1 = new ChargingCyclopsUpgrade(CyclopsModule.SolarChargerID);
            solarMk1.RegisterSelf(KnownsUpgradeModules);
            powerManager.SolarCharger = solarMk1;

            var solarMk2 = new BatteryCyclopsUpgrade(CyclopsModule.SolarChargerMk2ID, canRecharge: true);
            solarMk2.RegisterSelf(KnownsUpgradeModules);
            powerManager.SolarChargerMk2 = solarMk2;

            var thermalMk1 = new ChargingCyclopsUpgrade(TechType.CyclopsThermalReactorModule);
            thermalMk1.RegisterSelf(KnownsUpgradeModules);
            powerManager.ThermalCharger = thermalMk1;

            var thermalMk2 = new BatteryCyclopsUpgrade(CyclopsModule.ThermalChargerMk2ID, canRecharge: true);
            thermalMk2.RegisterSelf(KnownsUpgradeModules);
            powerManager.ThermalChargerMk2 = thermalMk2;

            var nuclear = new NuclearChargingModule();
            nuclear.RegisterSelf(KnownsUpgradeModules);
            powerManager.NuclearCharger = nuclear;

            var bioBoost = new BioBoosterUpgrade();
            bioBoost.RegisterSelf(KnownsUpgradeModules);
            powerManager.BioBoosters = bioBoost;

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

        internal void HandleUpgrades()
        {
            // Turn off all upgrades and clear all values
            if (this.Cyclops == null)
            {
                ErrorMessage.AddError("ClearAllUpgrades: Cyclops ref is null - Upgrade handling cancled");
                return;
            }

            foreach (CyclopsUpgrade upgradeType in KnownsUpgradeModules.Values)
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

                if (KnownsUpgradeModules.TryGetValue(techTypeInSlot, out CyclopsUpgrade upgrade))
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

                foreach (CyclopsUpgrade upgradeType in KnownsUpgradeModules.Values)
                    upgradeType.UpgradesFinished(this.Cyclops);
            }
        }
    }
}
