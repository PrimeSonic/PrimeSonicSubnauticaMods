namespace MoreCyclopsUpgrades.Managers
{
    using Common;
    using Modules;
    using Monobehaviors;
    using CyclopsUpgrades;
    using Modules.Enhancement;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using MoreCyclopsUpgrades.SaveData;

    /// <summary>
    /// The manager class that handles all upgrade events for a given Cyclops <see cref="SubRoot"/> instance.
    /// </summary>
    public class UpgradeManager
    {
        private static readonly ICollection<HandlerCreator> ReusableUpgradeHandlers = new List<HandlerCreator>();
        private static readonly ICollection<HandlerCreator> OneTimeUseUpgradeHandlers = new List<HandlerCreator>();

        /// <summary>
        /// <para>This event happens whenever a new UpgradeManager is initialized, before <see cref="HandlerCreator"/>s are registered.</para>
        /// <para>Use this if you need a way to know when you should call <see cref="RegisterOneTimeUseHandlerCreator"/> for <see cref="HandlerCreator"/>s that cannot be created from a static context.</para>
        /// </summary>
        public static Action UpgradeManagerInitializing;

        /// <summary>
        /// Registers a <see cref="HandlerCreator"/> method that creates returns a new <see cref="UpgradeHandler"/> on demand and is only used once.
        /// </summary>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="UpgradeHandler"/>.</param>
        public static void RegisterOneTimeUseHandlerCreator(HandlerCreator createEvent)
        {
            OneTimeUseUpgradeHandlers.Add(createEvent);
        }

        /// <summary>
        /// Registers a <see cref="HandlerCreator"/> method that creates returns a new <see cref="UpgradeHandler"/> on demand that can be reused for each new Cyclops.
        /// </summary>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="UpgradeHandler"/>.</param>
        public static void RegisterReusableHandlerCreator(HandlerCreator createEvent)
        {
            ReusableUpgradeHandlers.Add(createEvent);
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

        private readonly Dictionary<TechType, UpgradeHandler> KnownsUpgradeModules = new Dictionary<TechType, UpgradeHandler>();

        internal bool Initialize(CyclopsManager manager)
        {
            if (this.Manager != null)
                return false; // Already initialized

            this.Manager = manager;

            SetupPowerManagerUpgrades();

            UpgradeManagerInitializing?.Invoke();

            RegisterUpgradeHandlers();

            Equipment cyclopsConsole = this.Cyclops.upgradeConsole.modules;
            AttachEquipmentEvents(ref cyclopsConsole);

            SyncUpgradeConsoles();

            return true;
        }

        private void SetupPowerManagerUpgrades()
        {
            PowerManager powerManager = this.Manager.PowerManager;

            int maxChargingModules = MaxChargingModules(EmModPatchConfig.Settings.PowerLevel);
            powerManager.MaxModules = maxChargingModules;

            RegisterOneTimeUseHandlerCreator(() =>
            {
                var efficiencyUpgrades = new TieredUpgradesHandlerCollection<int>(0)
                {
                    LoggingName = "Engine Upgrades Collection"
                };
                TieredUpgradeHandler<int> engine1 = efficiencyUpgrades.CreateTier(TechType.PowerUpgradeModule, 1);
                engine1.LoggingName = "Engine Upgrade Mk1";
                TieredUpgradeHandler<int> engine2 = efficiencyUpgrades.CreateTier(CyclopsModule.PowerUpgradeMk2ID, 2);
                engine2.LoggingName = "Engine Upgrade Mk2";
                TieredUpgradeHandler<int> engine3 = efficiencyUpgrades.CreateTier(CyclopsModule.PowerUpgradeMk3ID, 3);
                engine3.LoggingName = "Engine Upgrade Mk3";

                powerManager.EngineEfficientyUpgrades = efficiencyUpgrades;
                return efficiencyUpgrades;
            });

            RegisterOneTimeUseHandlerCreator(() =>
            {
                var speed = new UpgradeHandler(CyclopsModule.SpeedBoosterModuleID)
                {
                    MaxCount = maxChargingModules,
                    LoggingName = "SpeedBooster",
                    OnFirstTimeMaxCountReached = () =>
                    {
                        ErrorMessage.AddMessage(CyclopsSpeedBooster.MaxRatingAchived);
                    }
                };
                powerManager.SpeedBoosters = speed;
                return speed;
            });

            RegisterOneTimeUseHandlerCreator(() =>
            {
                var solarMk1 = new ChargingUpgradeHandler(CyclopsModule.SolarChargerID)
                {
                    LoggingName = "SolarCharger",
                    MaxCount = maxChargingModules
                };
                powerManager.SolarCharger = solarMk1;
                return solarMk1;
            });

            RegisterOneTimeUseHandlerCreator(() =>
            {
                var solarMk2 = new BatteryUpgradeHandler(CyclopsModule.SolarChargerMk2ID, canRecharge: true)
                {
                    LoggingName = "SolarChargerMk2",
                    MaxCount = maxChargingModules
                };
                powerManager.SolarChargerMk2 = solarMk2;
                return solarMk2;
            });

            powerManager.SolarCharger.SiblingUpgrade = powerManager.SolarChargerMk2;
            powerManager.SolarChargerMk2.SiblingUpgrade = powerManager.SolarCharger;

            RegisterOneTimeUseHandlerCreator(() =>
            {
                var thermalMk1 = new ChargingUpgradeHandler(TechType.CyclopsThermalReactorModule)
                {
                    LoggingName = "ThermalCharger",
                    MaxCount = maxChargingModules
                };
                powerManager.ThermalCharger = thermalMk1;
                return thermalMk1;
            });

            RegisterOneTimeUseHandlerCreator(() =>
            {
                var thermalMk2 = new BatteryUpgradeHandler(CyclopsModule.ThermalChargerMk2ID, canRecharge: true)
                {
                    LoggingName = "ThermalChargerMk2",
                    MaxCount = maxChargingModules
                };
                powerManager.ThermalChargerMk2 = thermalMk2;
                return thermalMk2;
            });

            powerManager.ThermalCharger.SiblingUpgrade = powerManager.ThermalChargerMk2;
            powerManager.ThermalChargerMk2.SiblingUpgrade = powerManager.ThermalCharger;

            RegisterOneTimeUseHandlerCreator(() =>
            {
                var nuclear = new NuclearUpgradeHandler()
                {
                    MaxCount = maxChargingModules
                };
                powerManager.NuclearCharger = nuclear;
                return nuclear;
            });

            RegisterOneTimeUseHandlerCreator(() =>
            {
                var bioBoost = new BioBoosterUpgradeHandler(maxChargingModules);
                powerManager.BioBoosters = bioBoost;
                return bioBoost;
            });
        }

        private void RegisterUpgradeHandlers()
        {
            // Register upgrades from other mods
            foreach (Delegate externalMethod in ReusableUpgradeHandlers)
            {
                if (externalMethod is HandlerCreator upgradeHandlerCreator)
                {
                    UpgradeHandler upgrade = upgradeHandlerCreator.Invoke();
                    upgrade.RegisterSelf(KnownsUpgradeModules);
                }
            }

            foreach (HandlerCreator upgradeHandlerCreator in OneTimeUseUpgradeHandlers)
            {
                UpgradeHandler upgrade = upgradeHandlerCreator.Invoke();
                upgrade.RegisterSelf(KnownsUpgradeModules);
            }

            OneTimeUseUpgradeHandlers.Clear();
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

        internal void AttachEquipmentEvents(ref Equipment upgradeConsoleEquipment)
        {
            if (upgradeConsoleEquipment == null)
                return;

            upgradeConsoleEquipment.isAllowedToAdd += IsAllowedToAdd;
            upgradeConsoleEquipment.isAllowedToRemove += IsAllowedToRemove;
        }

        internal void HandleUpgrades()
        {
            // Turn off all upgrades and clear all values
            if (this.Cyclops == null)
            {
                ErrorMessage.AddError("ClearAllUpgrades: Cyclops ref is null - Upgrade handling cancled");
                return;
            }

            this.Manager.PowerManager.PowerIcons.DisableAll();

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

                if (KnownsUpgradeModules.TryGetValue(techTypeInSlot, out UpgradeHandler handler))
                {
                    handler.UpgradeCounted(this.Cyclops, modules, slot);

                    if (handler.IsPowerProducer)
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

        private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
            if (KnownsUpgradeModules.TryGetValue(pickupable.GetTechType(), out UpgradeHandler handler))
            {
                return handler.CanUpgradeBeAdded(this.Cyclops, pickupable, verbose);
            }

            return true;
        }

        private bool IsAllowedToRemove(Pickupable pickupable, bool verbose)
        {
            if (KnownsUpgradeModules.TryGetValue(pickupable.GetTechType(), out UpgradeHandler handler))
            {
                return handler.CanUpgradeBeRemoved(this.Cyclops, pickupable, verbose);
            }

            return true;
        }

        private static int MaxChargingModules(CyclopsPowerLevels setting)
        {
            switch (setting)
            {
                case CyclopsPowerLevels.Leviathan: return 6;
                case CyclopsPowerLevels.Crabsnake: return 3;
                case CyclopsPowerLevels.Peeper: return 1;
                default: return 12;
            }
        }        
    }
}
