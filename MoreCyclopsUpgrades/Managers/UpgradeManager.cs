namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Buildables;
    using MoreCyclopsUpgrades.API.Upgrades;
    using MoreCyclopsUpgrades.VanillaModules;

    /// <summary>
    /// The manager class that handles all upgrade events for a given Cyclops <see cref="SubRoot"/> instance.
    /// </summary>
    internal class UpgradeManager : BuildableManager<AuxiliaryUpgradeConsole>
    {
        internal class UpgradeCollection : Dictionary<TechType, UpgradeHandler>, IMCUUpgradeCollection
        {
        }

        internal static bool TooLateToRegister { get; private set; }

        private static readonly IDictionary<CreateUpgradeHandler, string> HandlerCreators = new Dictionary<CreateUpgradeHandler, string>();

        internal static void RegisterHandlerCreator(CreateUpgradeHandler createEvent, string assemblyName)
        {
            if (HandlerCreators.ContainsKey(createEvent))
            {
                QuickLogger.Warning($"Duplicate UpgradeHandlerCreator blocked from {assemblyName}");
                return;
            }

            QuickLogger.Info($"Received UpgradeHandlerCreator from {assemblyName}");
            HandlerCreators.Add(createEvent, assemblyName);
        }

        private UpgradeSlot[] engineRoomUpgradeSlots;

        internal IEnumerable<UpgradeSlot> UpgradeSlots
        {
            get
            {
                for (int s = 0; s < AuxiliaryUpgradeConsole.TotalSlots; s++)
                    yield return engineRoomUpgradeSlots[s];

                for (int a = 0; a < base.TrackedBuildables.Count; a++)
                {
                    UpgradeSlot[] auxSlots = base.TrackedBuildables[a].UpgradeSlotArray;

                    for (int s = 0; s < AuxiliaryUpgradeConsole.TotalSlots; s++)
                        yield return auxSlots[s];
                }
            }
        }

        public bool Initialized { get; private set; } = false;

        private Equipment engineRoomUpgradeConsole;

        private readonly IVanillaUpgrades vanillaUpgrades = new VanillaUpgrades();
        private UpgradeHandler[] upgradeHandlers;

        internal IVanillaUpgrades VanillaUpgrades
        {
            get
            {
                if (!Initialized)
                    InitializeUpgradeHandlers();

                return vanillaUpgrades;
            }
        }

        internal readonly UpgradeCollection KnownsUpgradeModules = new UpgradeCollection();

        internal T GetUpgradeHandler<T>(TechType upgradeId) where T : UpgradeHandler
        {
            if (!Initialized)
                InitializeUpgradeHandlers();

            if (KnownsUpgradeModules.TryGetValue(upgradeId, out UpgradeHandler upgradeHandler))
            {
                if (upgradeHandler is IGroupedUpgradeHandler groupMember)
                    return (T)groupMember.GroupHandler;

                return (T)upgradeHandler;
            }

            return null;
        }

        internal UpgradeHandler GetUpgradeHandler(TechType upgradeId)
        {
            if (!Initialized)
                InitializeUpgradeHandlers();

            if (KnownsUpgradeModules.TryGetValue(upgradeId, out UpgradeHandler upgradeHandler))
            {
                if (upgradeHandler is IGroupedUpgradeHandler groupMember)
                    return (UpgradeHandler)groupMember.GroupHandler;

                return upgradeHandler;
            }

            return null;
        }

        internal T GetGroupHandler<T>(TechType upgradeId, params TechType[] additionalIds) where T : UpgradeHandler, IGroupHandler
        {
            if (!Initialized)
                InitializeUpgradeHandlers();

            if (!KnownsUpgradeModules.TryGetValue(upgradeId, out UpgradeHandler upgradeHandler))
                return null;

            if (upgradeHandler is IGroupedUpgradeHandler groupMember)
            {
                if (additionalIds.Length > 0)
                {
                    IGroupHandler groupHandler = groupMember.GroupHandler;
                    for (int i = 0; i < additionalIds.Length; i++)
                    {
                        if (!groupHandler.IsManaging(additionalIds[i]))
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

        internal UpgradeManager(SubRoot cyclops) : base(cyclops)
        {
            engineRoomUpgradeConsole = Cyclops.upgradeConsole.modules;
        }

        private void InitializeUpgradeHandlers()
        {
            if (Initialized)
                return;

            QuickLogger.Debug($"UpgradeManager adding new UpgradeHandlers from external mods");
            // First, register upgrades from other mods.
            foreach (KeyValuePair<CreateUpgradeHandler, string> pair in HandlerCreators)
            {
                CreateUpgradeHandler upgradeHandlerCreator = pair.Key;
                string assemblyName = pair.Value;
                UpgradeHandler upgrade = upgradeHandlerCreator.Invoke(Cyclops);

                if (upgrade == null)
                {
                    QuickLogger.Warning($"UpgradeHandler from '{assemblyName}' was null");
                }
                else if (!KnownsUpgradeModules.ContainsKey(upgrade.TechType))
                {
                    upgrade.RegisterSelf(KnownsUpgradeModules);
                    upgrade.SourceMod = assemblyName;
                }
                else
                {
                    QuickLogger.Warning($"Duplicate UpgradeHandler for '{upgrade.TechType.AsString()}' from '{assemblyName}' was blocked");
                }
            }

            // Next, if no external mod has provided an UpgradeHandler for the vanilla upgrades, they will be added here.
            // This is to allow other mods to provide new functionality to the original upgrades.

            QuickLogger.Debug($"UpgradeManager adding default UpgradeHandlers for unmanaged vanilla upgrades");

            string mcuAssemblyName = QuickLogger.GetAssemblyName();
            for (int i = 0; i < vanillaUpgrades.OriginalUpgradeIDs.Count; i++)
            {
                TechType upgradeID = vanillaUpgrades.OriginalUpgradeIDs[i];
                if (!KnownsUpgradeModules.ContainsKey(upgradeID))
                {
                    UpgradeHandler vanillaUpgrade = vanillaUpgrades.CreateUpgradeHandler(upgradeID, Cyclops);
                    vanillaUpgrade.RegisterSelf(KnownsUpgradeModules);
                    vanillaUpgrade.SourceMod = mcuAssemblyName;
                }
            }

            upgradeHandlers = new UpgradeHandler[KnownsUpgradeModules.Count];

            int u = 0;
            foreach (UpgradeHandler upgrade in KnownsUpgradeModules.Values)
                upgradeHandlers[u++] = upgrade;

            QuickLogger.Debug("Attaching events to Engine Room Upgrade Console");

            if (engineRoomUpgradeConsole == null)
                engineRoomUpgradeConsole = Cyclops.upgradeConsole.modules;

            AttachEquipmentEvents(ref engineRoomUpgradeConsole);

            engineRoomUpgradeSlots = new UpgradeSlot[AuxiliaryUpgradeConsole.TotalSlots]
            {
                new UpgradeSlot(engineRoomUpgradeConsole, "Module1"),
                new UpgradeSlot(engineRoomUpgradeConsole, "Module2"),
                new UpgradeSlot(engineRoomUpgradeConsole, "Module3"),
                new UpgradeSlot(engineRoomUpgradeConsole, "Module4"),
                new UpgradeSlot(engineRoomUpgradeConsole, "Module5"),
                new UpgradeSlot(engineRoomUpgradeConsole, "Module6")
            };

            Initialized = true;
            TooLateToRegister = true;
        }

        public void AttachEquipmentEvents(ref Equipment upgradeConsoleEquipment)
        {
            if (upgradeConsoleEquipment == null)
            {
                QuickLogger.Error("Engine room upgrade console in Cyclops was null");
                return;
            }

            upgradeConsoleEquipment.isAllowedToAdd += (Pickupable pickupable, bool verbose) =>
            {
                if (KnownsUpgradeModules.TryGetValue(pickupable.GetTechType(), out UpgradeHandler handler))
                {
                    return handler.CanUpgradeBeAdded(pickupable, verbose);
                }

                return true;
            };

            upgradeConsoleEquipment.isAllowedToRemove += (Pickupable pickupable, bool verbose) =>
            {
                if (KnownsUpgradeModules.TryGetValue(pickupable.GetTechType(), out UpgradeHandler handler))
                {
                    return handler.CanUpgradeBeRemoved(pickupable, verbose);
                }

                return true;
            };
        }

        public void HandleUpgrades()
        {
            if (!Initialized)
                InitializeUpgradeHandlers();

            QuickLogger.Debug($"UpgradeManager clearing cyclops upgrades");

            // Turn off all upgrades and clear all values
            for (int i = 0; i < upgradeHandlers.Length; i++)
            {
                UpgradeHandler upgradeType = upgradeHandlers[i];

                if (upgradeType.HasUpgrade)
                    QuickLogger.Debug($"UpgradeManager clearing {upgradeType.TechType.AsString()}");

                upgradeType.UpgradesCleared(); // UpgradeHandler event
            }

            bool foundUpgrades = false;

            // Go through all slots and check what upgrades are available
            QuickLogger.Debug($"UpgradeManager checking upgrade slots");
            foreach (UpgradeSlot upgradeSlot in this.UpgradeSlots)
            {
                TechType techTypeInSlot = upgradeSlot.GetTechTypeInSlot();

                if (techTypeInSlot == TechType.None)
                    continue;

                foundUpgrades = true;

                if (KnownsUpgradeModules.TryGetValue(techTypeInSlot, out UpgradeHandler handler))
                {
                    QuickLogger.Debug($"UpgradeManager counting cyclops upgrade '{techTypeInSlot.AsString()}'");
                    handler.UpgradeCounted(upgradeSlot); // UpgradeHandler event
                }
                else
                {
                    QuickLogger.Warning($"UpgradeManager encountered unmanaged cyclops upgrade '{techTypeInSlot.AsString()}'");
                }
            }

            for (int i = 0; i < upgradeHandlers.Length; i++)
                upgradeHandlers[i].UpgradesFinished(); // UpgradeHandler event            

            // If any upgrades were found, play the sound to alert the player
            if (foundUpgrades)
            {
                Cyclops.slotModSFX?.Play();
                PdaOverlayManager.RemapItems();
            }
        }

        public override bool Initialize(SubRoot cyclops)
        {
            if (!Initialized)
                InitializeUpgradeHandlers();

            return cyclops == Cyclops;
        }

        protected override void ConnectWithManager(AuxiliaryUpgradeConsole buildable)
        {
            buildable.ConnectToCyclops(base.Cyclops, this);
        }
    }
}
