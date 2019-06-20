namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;
    using Common;
    using Monobehaviors;
    using MoreCyclopsUpgrades.API.General;
    using MoreCyclopsUpgrades.API.Upgrades;
    using MoreCyclopsUpgrades.CyclopsUpgrades;
    using UnityEngine;

    /// <summary>
    /// The manager class that handles all upgrade events for a given Cyclops <see cref="SubRoot"/> instance.
    /// </summary>
    internal class UpgradeManager : IAuxCyclopsManager
    {
        internal static bool Initialized { get; private set; }

        internal const string ManagerName = "McuUpgrdMgr";
        private static readonly ICollection<CreateUpgradeHandler> HandlerCreators = new List<CreateUpgradeHandler>();

        /// <summary>
        /// Registers a <see cref="CreateUpgradeHandler"/> method that creates returns a new <see cref="UpgradeHandler"/> on demand and is only used once.
        /// </summary>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="UpgradeHandler"/>.</param>
        internal static void RegisterHandlerCreator(CreateUpgradeHandler createEvent, string assemblyName)
        {
            if (HandlerCreators.Contains(createEvent))
            {
                QuickLogger.Warning($"Duplicate HandlerCreator blocked from {assemblyName}");
                return;
            }

            QuickLogger.Info($"Received HandlerCreator from {assemblyName}");
            HandlerCreators.Add(createEvent);
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

        internal readonly SubRoot Cyclops;

        internal List<CyUpgradeConsoleMono> AuxUpgradeConsoles { get; } = new List<CyUpgradeConsoleMono>();

        public string Name { get; } = ManagerName;

        internal readonly Dictionary<TechType, UpgradeHandler> KnownsUpgradeModules = new Dictionary<TechType, UpgradeHandler>();

        internal T GetUpgradeHandler<T>(TechType upgradeId) where T : UpgradeHandler
        {
            if (KnownsUpgradeModules.TryGetValue(upgradeId, out UpgradeHandler upgradeHandler))
            {
                if (upgradeHandler is IGroupedUpgradeHandler groupMember)
                    return (T)groupMember.GroupHandler;

                return (T)upgradeHandler;
            }

            return null;
        }

        internal T GetGroupHandler<T>(TechType upgradeId, params TechType[] additionalIds) where T : UpgradeHandler, IGroupHandler
        {
            if (!KnownsUpgradeModules.TryGetValue(upgradeId, out UpgradeHandler upgradeHandler))
                return null;

            if (upgradeHandler is IGroupedUpgradeHandler groupMember)
            {
                if (additionalIds.Length > 0)
                {
                    IGroupHandler groupHandler = groupMember.GroupHandler;
                    foreach (TechType techType in additionalIds)
                    {
                        if (!groupHandler.IsManaging(techType))
                            return null;
                    }
                    return (T)groupHandler;
                }
                else
                {
                    return (T)groupMember.GroupHandler;
                }
            }

            return (T)upgradeHandler;
        }

        internal UpgradeManager(SubRoot cyclops)
        {
            Cyclops = cyclops;
        }

        internal bool Initialize(CyclopsManager manager)
        {
            RegisterUpgradeHandlers();

            Equipment cyclopsConsole = Cyclops.upgradeConsole.modules;
            AttachEquipmentEvents(ref cyclopsConsole);

            SyncUpgradeConsoles();

            return Initialized = true;
        }

        private void RegisterUpgradeHandlers()
        {
            QuickLogger.Debug("UpgradeManager RegisterUpgradeHandlers");

            // Register upgrades from other mods
            foreach (CreateUpgradeHandler upgradeHandlerCreator in HandlerCreators)
            {
                UpgradeHandler upgrade = upgradeHandlerCreator.Invoke(Cyclops);

                if (!KnownsUpgradeModules.ContainsKey(upgrade.techType))
                    upgrade.RegisterSelf(KnownsUpgradeModules);
                else
                    QuickLogger.Warning($"Duplicate Reusable UpgradeHandler for '{upgrade.techType}' was blocked");
            }

            if (!KnownsUpgradeModules.ContainsKey(TechType.PowerUpgradeModule))
            {
                QuickLogger.Debug("No UpgradeHandler for the original Engine Efficiency module detected. Adding it internally.");
                KnownsUpgradeModules.Add(TechType.PowerUpgradeModule, new DefaultEngineUpgrade(Cyclops));
            }
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
                    auxConsole.ConnectToCyclops(Cyclops, this);
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
                upgradeType.UpgradesCleared(); // UpgradeHandler event

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
                    handler.UpgradeCounted(modules, slot); // UpgradeHandler event
                }
            }

            // If any upgrades were found, play the sound to alert the player
            if (foundUpgrades.Count > 0)
            {
                Cyclops.slotModSFX?.Play();

                foreach (UpgradeHandler upgradeType in KnownsUpgradeModules.Values)
                    upgradeType.UpgradesFinished(); // UpgradeHandler event
            }

            Cyclops.BroadcastMessage("RefreshUpgradeConsoleIcons", foundUpgrades.ToArray(), SendMessageOptions.RequireReceiver);
        }

        private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
            if (KnownsUpgradeModules.TryGetValue(pickupable.GetTechType(), out UpgradeHandler handler))
            {
                return handler.CanUpgradeBeAdded(pickupable, verbose);
            }

            return true;
        }

        private bool IsAllowedToRemove(Pickupable pickupable, bool verbose)
        {
            if (KnownsUpgradeModules.TryGetValue(pickupable.GetTechType(), out UpgradeHandler handler))
            {
                return handler.CanUpgradeBeRemoved(pickupable, verbose);
            }

            return true;
        }

        public bool Initialize(SubRoot cyclops)
        {
            throw new System.NotImplementedException();
        }
    }
}
