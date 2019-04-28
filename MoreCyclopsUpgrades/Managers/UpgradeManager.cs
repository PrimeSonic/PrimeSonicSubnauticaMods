namespace MoreCyclopsUpgrades.Managers
{
    using Common;
    using CyclopsUpgrades;
    using Monobehaviors;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
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
            QuickLogger.Info($"Received OneTimeUse HandlerCreator from {Assembly.GetCallingAssembly().GetName().Name}");
            OneTimeUseUpgradeHandlers.Add(createEvent);
        }

        /// <summary>
        /// Registers a <see cref="HandlerCreator"/> method that creates returns a new <see cref="UpgradeHandler"/> on demand that can be reused for each new Cyclops.
        /// </summary>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="UpgradeHandler"/>.</param>
        public static void RegisterReusableHandlerCreator(HandlerCreator createEvent)
        {
            QuickLogger.Info($"Received Reusable HandlerCreator from {Assembly.GetCallingAssembly().GetName().Name}");
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

            RegisterUpgradeHandlers();

            Equipment cyclopsConsole = Cyclops.upgradeConsole.modules;
            AttachEquipmentEvents(ref cyclopsConsole);

            SyncUpgradeConsoles();

            return true;
        }

        private void RegisterUpgradeHandlers()
        {
            QuickLogger.Debug("UpgradeManager RegisterUpgradeHandlers");
            UpgradeManagerInitializing?.Invoke();
            QuickLogger.Debug("External UpgradeManagerInitializing methods invoked");

            // Register upgrades from other mods
            foreach (HandlerCreator upgradeHandlerCreator in ReusableUpgradeHandlers)
            {
                UpgradeHandler upgrade = upgradeHandlerCreator.Invoke();

                if (!KnownsUpgradeModules.ContainsKey(upgrade.techType))
                    upgrade.RegisterSelf(KnownsUpgradeModules);
                else
                    QuickLogger.Warning($"Duplicate Reusable UpgradeHandler for '{upgrade.techType}' was blocked");
            }

            foreach (HandlerCreator upgradeHandlerCreator in OneTimeUseUpgradeHandlers)
            {
                UpgradeHandler upgrade = upgradeHandlerCreator.Invoke();

                if (!KnownsUpgradeModules.ContainsKey(upgrade.techType))
                    upgrade.RegisterSelf(KnownsUpgradeModules);
                else
                    QuickLogger.Warning($"Duplicate OneTimeUse UpgradeHandler for '{upgrade.techType}' was blocked");
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
            foreach (UpgradeHandler upgradeType in KnownsUpgradeModules.Values)
                upgradeType.UpgradesCleared(Cyclops); // UpgradeHandler event

            var foundUpgrades = new List<TechType>();

            // Go through all slots and check what upgrades are available
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
                    handler.UpgradeCounted(Cyclops, modules, slot); // UpgradeHandler event
                }
            }

            // If any upgrades were found, play the sound to alert the player
            if (foundUpgrades.Count > 0)
            {
                Cyclops.slotModSFX?.Play();

                foreach (UpgradeHandler upgradeType in KnownsUpgradeModules.Values)
                    upgradeType.UpgradesFinished(Cyclops); // UpgradeHandler event
            }

            Cyclops.BroadcastMessage("RefreshUpgradeConsoleIcons", foundUpgrades.ToArray(), SendMessageOptions.RequireReceiver);
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
