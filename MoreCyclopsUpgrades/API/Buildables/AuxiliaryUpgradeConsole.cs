namespace MoreCyclopsUpgrades.API.Buildables
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Common;
    using MoreCyclopsUpgrades.API.Upgrades;
    using MoreCyclopsUpgrades.AuxConsole;
    using MoreCyclopsUpgrades.Managers;
    using ProtoBuf;
    using UnityEngine;

    /// <summary>
    /// The core functionality of an Auxiliary Upgrade Console.<para/>
    /// Handles basic player interaction, save data, and connecting with the Cyclops sub.
    /// </summary>
    /// <seealso cref="HandTarget" />
    /// <seealso cref="IHandTarget" />
    /// <seealso cref="IProtoEventListener" />
    /// <seealso cref="ICyclopsBuildable" />
    [ProtoContract]
    public abstract class AuxiliaryUpgradeConsole : HandTarget, IHandTarget, IProtoEventListener, ICyclopsBuildable, IUpgradeSlots
    {
        /// <summary>
        /// The total number of upgrade slots. This value is constant.
        /// </summary>
        public const int TotalSlots = 6;

        /// <summary>
        /// A read-only collection of the upgrade slot names. These will be used to up upgrade slots in <see cref="Modules"/>.
        /// </summary>
        public static readonly IEnumerable<string> SlotNames = new string[TotalSlots]
        {
            "Module1",
            "Module2",
            "Module3",
            "Module4",
            "Module5",
            "Module6"
        };

        /// <summary>
        /// Invoked after <see cref="OnEquip(string, InventoryItem)"/> has finished handling the added item.
        /// </summary>
        public virtual void OnSlotEquipped(string slot, InventoryItem item) { }

        /// <summary>
        /// Invoked after <see cref="OnUnequip(string, InventoryItem)"/> has finished handling the removed item.
        /// </summary>
        public virtual void OnSlotUnequipped(string slot, InventoryItem item) { }

        private AuxCyUpgradeConsoleSaveData saveData;
        private string prefabId = null;
        private SubRoot ParentCyclops;
        private UpgradeManager UpgradeManager;

        /// <summary>
        /// The root object container for the <see cref="Equipment"/> modules.
        /// </summary>
        public ChildObjectIdentifier ModulesRoot;

        /// <summary>
        /// Gets the equipment modules.
        /// </summary>
        public Equipment Modules { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this buildable is connected to the Cyclops.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this buildable is connected to cyclops; otherwise, <c>false</c>.
        /// </value>
        /// <see cref="ICyclopsBuildable"/>
        /// <seealso cref="BuildableManager{BuildableMono}.ConnectWithManager(BuildableMono)" />
        public bool IsConnectedToCyclops => ParentCyclops != null && UpgradeManager != null;

        internal UpgradeSlot[] UpgradeSlotArray;

        /// <summary>
        /// Gets the upgrade slots for this upgrade console.
        /// </summary>
        /// <remarks>Value not initialized until after <see cref="Awake"/> is run.</remarks>
        public IEnumerable<UpgradeSlot> UpgradeSlots => UpgradeSlotArray;

        private Constructable _buildable = null;
        internal Constructable Buildable => _buildable ?? (_buildable = GetComponentInParent<Constructable>() ?? GetComponent<Constructable>());

        #region // Initialization

        /// <summary>
        ///  Unity Awake event.
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            if (saveData == null)
                PrepareSaveData();

            if (this.Modules == null)
                InitializeModules();
        }

        private void Start()
        {
            SubRoot cyclops = base.GetComponentInParent<SubRoot>();

            if (cyclops == null)
            {
                QuickLogger.Debug("CyUpgradeConsoleMono: Could not find Cyclops during Start. Attempting external syncronize.");
                for (int i = 0; i < CyclopsManager.Managers.Count; i++)
                    CyclopsManager.Managers[i].Upgrade.SyncBuildables();
            }
            else
            {
                QuickLogger.Debug("CyUpgradeConsoleMono: Parent cyclops found!");
                ConnectToCyclops(cyclops);
            }
        }

        internal void ConnectToCyclops(SubRoot parentCyclops, UpgradeManager manager = null)
        {
            ParentCyclops = parentCyclops;
            this.transform.SetParent(parentCyclops.transform);
            UpgradeManager = manager ?? CyclopsManager.GetManager(ref parentCyclops).Upgrade;

            if (UpgradeManager != null)
            {
                UpgradeManager.AddBuildable(this);

                Equipment console = this.Modules;
                UpgradeManager.AttachEquipmentEvents(ref console);
                QuickLogger.Debug("Auxiliary Upgrade Console has been connected", true);
            }
        }

        private void InitializeModules()
        {
            QuickLogger.Debug("Initializing Equipment");
            if (ModulesRoot == null)
            {
                var equipmentRoot = new GameObject("EquipmentRoot");
                equipmentRoot.transform.SetParent(this.transform, false);
                ModulesRoot = equipmentRoot.AddComponent<ChildObjectIdentifier>();
            }

            this.Modules = new Equipment(base.gameObject, ModulesRoot.transform);
            this.Modules.SetLabel("CyclopsUpgradesStorageLabel");

            EventInfo onEquipInfo = typeof(Equipment).GetEvent("onEquip", BindingFlags.Public | BindingFlags.Instance);
            EventInfo onUnequipInfo = typeof(Equipment).GetEvent("onUnequip", BindingFlags.Public | BindingFlags.Instance);

            MethodInfo myOnEquipMethod = typeof(AuxiliaryUpgradeConsole).GetMethod(nameof(AuxiliaryUpgradeConsole.OnEquip), BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo myOnUnequipMethod = typeof(AuxiliaryUpgradeConsole).GetMethod(nameof(AuxiliaryUpgradeConsole.OnUnequip), BindingFlags.Instance | BindingFlags.NonPublic);

            var onEquipDelegate = Delegate.CreateDelegate(typeof(Equipment.OnEquip), this, myOnEquipMethod);
            var onUnequipDelegate = Delegate.CreateDelegate(typeof(Equipment.OnUnequip), this, myOnUnequipMethod);

            onEquipInfo.AddEventHandler(this.Modules, onEquipDelegate);
            onUnequipInfo.AddEventHandler(this.Modules, onUnequipDelegate);

            this.Modules.AddSlots(SlotNames);

            UpgradeSlotArray = new UpgradeSlot[TotalSlots]
            {
                new UpgradeSlot(this.Modules, "Module1"),
                new UpgradeSlot(this.Modules, "Module2"),
                new UpgradeSlot(this.Modules, "Module3"),
                new UpgradeSlot(this.Modules, "Module4"),
                new UpgradeSlot(this.Modules, "Module5"),
                new UpgradeSlot(this.Modules, "Module6")
            };
        }

        #endregion

        #region // Player Interaction

        /// <summary>
        /// Gets the text to display when the player's cursor hovers over this upgrade console.<para/>
        /// By default, this will display the same text as the original AuxUpgradeConsole.
        /// </summary>
        /// <value>
        /// The on hover text to display.
        /// </value>
        protected virtual string OnHoverText => AuxCyUpgradeConsole.OnHoverText;

        /// <summary>
        /// Called when the player hovers over the upgrade console.
        /// </summary>
        /// <param name="guiHand">The GUI hand.</param>
        /// <see cref="IHandTarget"/>
        public void OnHandHover(GUIHand guiHand)
        {
            if (!this.Buildable.constructed)
                return;

            HandReticle main = HandReticle.main;
            main.SetInteractText(this.OnHoverText);
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        /// <summary>
        /// Called when the player clicks the upgrade console.
        /// </summary>
        /// <param name="guiHand">The GUI hand.</param>
        /// <see cref="IHandTarget"/>
        public void OnHandClick(GUIHand guiHand)
        {
            if (!this.Buildable.constructed)
                return;

            PdaOverlayManager.StartConnectingToPda(this.Modules);

            Player main = Player.main;
            global::PDA pda = main.GetPDA();
            Inventory.main.SetUsedStorage(this.Modules, false);
            pda.Open(PDATab.Inventory, null, new global::PDA.OnClose((closingPdaEvent) => PdaOverlayManager.DisconnectFromPda()), -1f);
        }

        private void OnEquip(string slot, InventoryItem item)
        {
            CyclopsUpgradeChange();

            // Disallow deconstruction while there are modules in here
            if (this.Buildable != null)
                this.Buildable.deconstructionAllowed = false;

            OnSlotEquipped(slot, item);
        }

        private void OnUnequip(string slot, InventoryItem item)
        {
            CyclopsUpgradeChange();

            bool allEmpty = true;

            for (int s = 0; s < TotalSlots; s++)
            {
                if (UpgradeSlotArray[s].HasItemInSlot())
                {
                    allEmpty = false;
                    break;
                }
            }

            // Deconstruction only allowed if all slots are empty
            if (this.Buildable != null)
                this.Buildable.deconstructionAllowed = allEmpty;

            OnSlotUnequipped(slot, item);
        }

        private void CyclopsUpgradeChange()
        {
            if (ParentCyclops == null)
                return;

            ParentCyclops.subModulesDirty = true;
        }

        private void OnDestroy()
        {
            if (UpgradeManager != null)
                UpgradeManager.RemoveBuildable(this);

            ParentCyclops = null;
            UpgradeManager = null;
        }

        #endregion

        #region // Save Data Handling

        /// <summary>
        /// Called when the game is being saved.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <see cref="IProtoEventListener"/>
        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            if (saveData == null)
                PrepareSaveData();

            for (int s = 0; s < TotalSlots; s++)
            {
                UpgradeSlot upgradeSlot = UpgradeSlotArray[s];

                EmModuleSaveData savedModule = saveData.GetModuleInSlot(upgradeSlot.slotName);
                InventoryItem item = upgradeSlot.GetItemInSlot();

                if (item == null)
                {
                    savedModule.ItemID = (int)TechType.None;
                    savedModule.RemainingCharge = -1f;
                }
                else
                {
                    savedModule.ItemID = (int)item.item.GetTechType();

                    Battery battery = item.item.GetComponent<Battery>();

                    if (battery == null)
                    {
                        savedModule.RemainingCharge = -1f;
                    }
                    else
                    {
                        savedModule.RemainingCharge = battery._charge;
                    }
                }

            }

            saveData.Save();
        }

        /// <summary>
        /// Called when loading the game from a save file.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <see cref="IProtoEventListener"/>
        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            if (saveData == null)
                PrepareSaveData();

            if (this.Modules == null)
                InitializeModules();

            this.Modules.Clear();

            QuickLogger.Debug("Checking save data");

            if (saveData != null && saveData.Load())
            {
                // Because the items here aren't being serialized with everything else normally,
                // I've used custom save data to handle whatever gets left in these slots.

                QuickLogger.Debug("Loading save data");
                // The following is a recreation of the essential parts of the Equipment.ResponseEquipment method.
                for (int s = 0; s < TotalSlots; s++)
                {
                    string slot = UpgradeSlotArray[s].slotName;
                    // These slots need to be added before we can add items to them
                    this.Modules.AddSlot(slot);

                    EmModuleSaveData savedModule = saveData.GetModuleInSlot(slot);

                    if (savedModule.ItemID == 0) // (int)TechType.None
                        continue; // Nothing here

                    var techtype = (TechType)savedModule.ItemID;
                    string itemName = techtype.AsString();

                    QuickLogger.Debug($"Spawning '{itemName}' from save data");
                    InventoryItem spanwedItem = CyclopsUpgrade.SpawnCyclopsModule(techtype);

                    if (spanwedItem == null)
                    {
                        QuickLogger.Warning($"Unknown upgrade module '{itemName}' could not be spamned in from save data");
                        continue;
                    }

                    QuickLogger.Debug($"Spawned in {itemName} from save data");

                    if (savedModule.RemainingCharge > 0f) // Modules without batteries are stored with a -1 value for charge
                        spanwedItem.item.GetComponent<Battery>().charge = savedModule.RemainingCharge;

                    this.Modules.AddItem(slot, spanwedItem, true);
                    OnSlotEquipped(slot, spanwedItem);
                }
            }
            else
            {
                QuickLogger.Debug("No save data found.");
                this.Modules.AddSlots(SlotNames);
            }
        }

        internal void DeleteSaveData()
        {
            saveData?.Delete();
        }

        private void PrepareSaveData()
        {
            if (prefabId == null)
            {
                PrefabIdentifier prefabIdentifier = GetComponentInParent<PrefabIdentifier>() ?? GetComponent<PrefabIdentifier>();
                prefabId = prefabIdentifier.id;
            }

            if (prefabId != null && saveData == null)
            {
                QuickLogger.Debug($"AuxCyUpgradeConsole PrefabIdentifier {prefabId}");
                saveData = new AuxCyUpgradeConsoleSaveData(prefabId);
            }
        }

        #endregion
    }
}
