namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.API.Upgrades;
    using MoreCyclopsUpgrades.Items.AuxConsole;
    using MoreCyclopsUpgrades.OriginalUpgrades;
    using UnityEngine;

    /// <summary>
    /// The manager class that handles all upgrade events for a given Cyclops <see cref="SubRoot"/> instance.
    /// </summary>
    internal class UpgradeManager
    {
        internal static bool Initialized { get; private set; }

        internal const string ManagerName = "McuUpgrdMgr";
        private static readonly IDictionary<CreateUpgradeHandler, string> HandlerCreators = new Dictionary<CreateUpgradeHandler, string>();

        internal static void RegisterHandlerCreator(CreateUpgradeHandler createEvent, string assemblyName)
        {
            if (HandlerCreators.ContainsKey(createEvent))
            {
                QuickLogger.Warning($"Duplicate HandlerCreator blocked from {assemblyName}");
                return;
            }

            QuickLogger.Info($"Received HandlerCreator from {assemblyName}");
            HandlerCreators.Add(createEvent, assemblyName);
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

        private readonly List<AuxCyUpgradeConsoleMono> TempCache = new List<AuxCyUpgradeConsoleMono>();

        private IEnumerable<UpgradeSlot> UpgradeSlots
        {
            get
            {
                if (Cyclops.upgradeConsole != null)
                    foreach (string slot in SlotHelper.SlotNames)
                        yield return new UpgradeSlot(Cyclops.upgradeConsole.modules, slot);

                foreach (AuxCyUpgradeConsoleMono aux in this.AuxUpgradeConsoles)
                    foreach (string slot in SlotHelper.SlotNames)
                        yield return new UpgradeSlot(aux.Modules, slot);
            }
        }

        internal readonly SubRoot Cyclops;

        internal List<AuxCyUpgradeConsoleMono> AuxUpgradeConsoles { get; } = new List<AuxCyUpgradeConsoleMono>();

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

        public bool InitializeUpgradeHandlers()
        {
            RegisterUpgradeHandlers();

            Equipment cyclopsConsole = Cyclops.upgradeConsole.modules;
            AttachEquipmentEvents(ref cyclopsConsole);

            return Initialized = true;
        }

        private void RegisterUpgradeHandlers()
        {
            QuickLogger.Debug("UpgradeManager RegisterUpgradeHandlers");

            // Register upgrades from other mods
            foreach (KeyValuePair<CreateUpgradeHandler, string> pair in HandlerCreators)
            {
                CreateUpgradeHandler upgradeHandlerCreator = pair.Key;
                string assemblyName = pair.Value;
                UpgradeHandler upgrade = upgradeHandlerCreator.Invoke(Cyclops);

                if (upgrade == null)
                {
                    QuickLogger.Warning($"UpgradeHandler from '{assemblyName}' was null");
                }
                else if (!KnownsUpgradeModules.ContainsKey(upgrade.techType))
                {
                    upgrade.RegisterSelf(KnownsUpgradeModules);
                    QuickLogger.Debug($"Added UpgradeHandler for {upgrade.techType} from '{assemblyName}'");
                }
                else
                {
                    QuickLogger.Warning($"Duplicate UpgradeHandler for '{upgrade.techType}' from '{assemblyName}' was blocked");
                }
            }

            if (!KnownsUpgradeModules.ContainsKey(TechType.PowerUpgradeModule))
            {
                QuickLogger.Debug("No UpgradeHandler for the original Engine Efficiency module detected. Adding it internally.");
                KnownsUpgradeModules.Add(TechType.PowerUpgradeModule, new OriginalEngineUpgrade(Cyclops));
            }
        }

        internal void SyncUpgradeConsoles()
        {
            TempCache.Clear();

            AuxCyUpgradeConsoleMono[] auxUpgradeConsoles = Cyclops.GetAllComponentsInChildren<AuxCyUpgradeConsoleMono>();

            foreach (AuxCyUpgradeConsoleMono auxConsole in auxUpgradeConsoles)
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
            QuickLogger.Debug($"UpgradeManager clearing cyclops upgrades");

            // Turn off all upgrades and clear all values
            foreach (UpgradeHandler upgradeType in KnownsUpgradeModules.Values)
            {
                QuickLogger.Debug($"UpgradeManager clearing {upgradeType.techType}");
                upgradeType.UpgradesCleared(); // UpgradeHandler event
            }

            var foundUpgrades = new List<TechType>();

            // Go through all slots and check what upgrades are available
            QuickLogger.Debug($"UpgradeManager checking upgrade slots");
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
                    QuickLogger.Debug($"UpgradeManager counting cyclops upgrade: {techTypeInSlot}");
                    handler.UpgradeCounted(modules, slot); // UpgradeHandler event
                }
                else
                {
                    QuickLogger.Warning($"UpgradeManager encountered unmanaged cyclops upgrade: {techTypeInSlot}");
                }
            }

            // If any upgrades were found, play the sound to alert the player
            if (foundUpgrades.Count > 0)
            {
                Cyclops.slotModSFX?.Play();

                foreach (UpgradeHandler upgradeType in KnownsUpgradeModules.Values)
                    upgradeType.UpgradesFinished(); // UpgradeHandler event

                PdaOverlayManager.RemapItems();
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
    }
}
