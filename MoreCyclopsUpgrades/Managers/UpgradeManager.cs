namespace MoreCyclopsUpgrades.Managers
{
    using Common;
    using CyclopsUpgrades;
    using Modules;
    using Modules.Enhancement;
    using Monobehaviors;
    using SaveData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

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

        private IEnumerable<UpgradeSlot> UpgradeSlots
        {
            get
            {
                if (Cyclops.upgradeConsole != null)
                    foreach (string slot in SlotHelper.SlotNames)
                        yield return new UpgradeSlot(Cyclops.upgradeConsole.modules, slot);

                foreach (CyUpgradeConsoleMono aux in this.AuxUpgradeConsoles)
                    foreach (string slot in SlotHelper.SlotNames)
                        yield return new UpgradeSlot(aux.Modules, slot);
            }
        }

        internal CyclopsManager Manager { get; private set; }

        internal readonly SubRoot Cyclops;

        internal List<CyUpgradeConsoleMono> AuxUpgradeConsoles { get; } = new List<CyUpgradeConsoleMono>();

        private readonly Dictionary<TechType, UpgradeHandler> KnownsUpgradeModules = new Dictionary<TechType, UpgradeHandler>();

        internal UpgradeManager(SubRoot cyclops)
        {
            Cyclops = cyclops;
        }

        internal bool Initialize(CyclopsManager manager)
        {
            if (this.Manager != null)
                return false; // Already initialized

            this.Manager = manager;

            UpgradeManagerInitializing?.Invoke();
            
            PowerManager powerManager = this.Manager.PowerManager;
            powerManager.MaxSpeedModules = ModConfig.Settings.MaxSpeedModules();

            SetupPowerManagerUpgrades(powerManager, ModConfig.Settings.MaxChargingModules());
            this.Manager.ChargeManager.SetupChargingUpgrades(ModConfig.Settings.MaxChargingModules());

            RegisterUpgradeHandlers();

            this.Manager.ChargeManager.RegisterPowerChargers();

            Equipment cyclopsConsole = Cyclops.upgradeConsole.modules;
            AttachEquipmentEvents(ref cyclopsConsole);

            SyncUpgradeConsoles();

            return true;
        }

        private void SetupPowerManagerUpgrades(PowerManager powerManager, int maxModules)
        {
            RegisterOneTimeUseHandlerCreator(() =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: Engine Upgrades Collection");
                var efficiencyUpgrades = new TieredUpgradesHandlerCollection<int>(0);

                QuickLogger.Debug("UpgradeHandler Registered: Engine Upgrade Mk1");
                TieredUpgradeHandler<int> engine1 = efficiencyUpgrades.CreateTier(TechType.PowerUpgradeModule, 1);

                QuickLogger.Debug("UpgradeHandler Registered: Engine Upgrade Mk2");
                TieredUpgradeHandler<int> engine2 = efficiencyUpgrades.CreateTier(CyclopsModule.PowerUpgradeMk2ID, 2);

                QuickLogger.Debug("UpgradeHandler Registered: Engine Upgrade Mk3");
                TieredUpgradeHandler<int> engine3 = efficiencyUpgrades.CreateTier(CyclopsModule.PowerUpgradeMk3ID, 3);

                powerManager.EngineEfficientyUpgrades = efficiencyUpgrades;
                return efficiencyUpgrades;
            });

            RegisterOneTimeUseHandlerCreator(() =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: SpeedBooster Upgrade");
                var speed = new UpgradeHandler(CyclopsModule.SpeedBoosterModuleID)
                {
                    MaxCount = maxModules,
                    OnFirstTimeMaxCountReached = () =>
                    {
                        ErrorMessage.AddMessage(CyclopsSpeedBooster.MaxRatingAchived);
                    }
                };
                powerManager.SpeedBoosters = speed;
                return speed;
            });

        }

        private void RegisterUpgradeHandlers()
        {
            // Register upgrades from other mods
            foreach (HandlerCreator upgradeHandlerCreator in ReusableUpgradeHandlers)
            {
                UpgradeHandler upgrade = upgradeHandlerCreator.Invoke();
                upgrade.RegisterSelf(KnownsUpgradeModules);
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

            CyUpgradeConsoleMono[] auxUpgradeConsoles = Cyclops.GetAllComponentsInChildren<CyUpgradeConsoleMono>();

            foreach (CyUpgradeConsoleMono auxConsole in auxUpgradeConsoles)
            {
                if (TempCache.Contains(auxConsole))
                    continue; // This is a workaround because of the object references being returned twice in this array.

                TempCache.Add(auxConsole);

                if (auxConsole.ParentCyclops == null)
                {
                    QuickLogger.Debug("CyUpgradeConsoleMono synced externally");
                    // This is a workaround to get a reference to the Cyclops into the AuxUpgradeConsole
                    auxConsole.ConnectToCyclops(Cyclops, this.Manager);
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
            if (Cyclops == null)
            {
                ErrorMessage.AddError("ClearAllUpgrades: Cyclops ref is null - Upgrade handling cancled");
                return;
            }

            // get the handler from this cyclops
            UpgradeModuleEventHandler upgradeModuleEventHandler = Cyclops.gameObject.GetComponentInChildren<UpgradeModuleEventHandler>();
            
            if (upgradeModuleEventHandler == null)
            {
                QuickLogger.Error("UpgradeModuleEventHandler is null!");
            }
            
            foreach (UpgradeHandler upgradeType in KnownsUpgradeModules.Values)
                upgradeType.UpgradesCleared(Cyclops);

            // triggering the remove event on this cyclops
            upgradeModuleEventHandler.onUpgradeModuleRemove.Trigger(Cyclops);
            
            var foundUpgrades = new List<TechType>();

            foreach (UpgradeSlot upgradeSlot in this.UpgradeSlots)
            {
                Equipment modules = upgradeSlot.Modules;
                string slot = upgradeSlot.Slot;

                TechType techTypeInSlot = modules.GetTechTypeInSlot(slot);

                if (techTypeInSlot == TechType.None)
                    continue;

                // triggering the add event on this cyclops
                upgradeModuleEventHandler.onUpgradeModuleAdd.Trigger(new UpgradeInfo(Cyclops, techTypeInSlot));
                
                foundUpgrades.Add(techTypeInSlot);

                if (KnownsUpgradeModules.TryGetValue(techTypeInSlot, out UpgradeHandler handler))
                {
                    handler.UpgradeCounted(Cyclops, modules, slot);
                }
            }

            if (foundUpgrades.Count > 0)
            {
                Cyclops.slotModSFX?.Play();
                Cyclops.BroadcastMessage("RefreshUpgradeConsoleIcons", foundUpgrades.ToArray(), SendMessageOptions.RequireReceiver);

                foreach (UpgradeHandler upgradeType in KnownsUpgradeModules.Values)
                    upgradeType.UpgradesFinished(Cyclops);
            }
        }

        private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
            if (KnownsUpgradeModules.TryGetValue(pickupable.GetTechType(), out UpgradeHandler handler))
            {
                return handler.CanUpgradeBeAdded(Cyclops, pickupable, verbose);
            }

            return true;
        }

        private bool IsAllowedToRemove(Pickupable pickupable, bool verbose)
        {
            if (KnownsUpgradeModules.TryGetValue(pickupable.GetTechType(), out UpgradeHandler handler))
            {
                return handler.CanUpgradeBeRemoved(Cyclops, pickupable, verbose);
            }

            return true;
        }
    }
}
