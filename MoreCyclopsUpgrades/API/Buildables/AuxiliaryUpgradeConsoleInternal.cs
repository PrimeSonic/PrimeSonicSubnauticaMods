namespace MoreCyclopsUpgrades.API.Buildables
{
    using System;
    using System.Reflection;
    using Common;
    using MoreCyclopsUpgrades.API.Upgrades;
    using MoreCyclopsUpgrades.AuxConsole;
    using MoreCyclopsUpgrades.Managers;
    using ProtoBuf;
    using UnityEngine;

    // This partial class file contains all members of AuxiliaryUpgradeConsole intended for internal use only
    [ProtoContract]
    public abstract partial class AuxiliaryUpgradeConsole : IProtoEventListener
    {
        #region // Private/Internal members

        private AuxCyUpgradeConsoleSaveData saveData;
        private Constructable _buildable;
        private string prefabId;
        private SubRoot ParentCyclops;
        private UpgradeManager UpgradeManager;

        internal UpgradeSlot[] UpgradeSlotArray;
        internal Constructable Buildable => _buildable ?? (_buildable = GetComponentInParent<Constructable>() ?? GetComponent<Constructable>());

        #endregion

        #region // Initialization (for internal use only)

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
            else if (!cyclops.isCyclops)
            {
                QuickLogger.Error("DEVELOPER WARNING: The AuxiliaryUpgradeConsole is inside a base and not a Cyclops sub. This should NOT be allowed.");
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
            UpgradeManager = manager ?? CyclopsManager.GetManager(ref parentCyclops)?.Upgrade;

            if (UpgradeManager != null)
            {
                UpgradeManager.AddBuildable(this);

                Equipment console = this.Modules;
                UpgradeManager.AttachEquipmentEvents(ref console);
                QuickLogger.Debug("Auxiliary Upgrade Console has been connected", true);
            }
            else
            {
                QuickLogger.Error("There was a problem connecting with the parent cyclops.");
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

        #region // Player Interaction (for internal use only)

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

        #region // Save Data Handling (for internal use only)

        /// <summary>
        /// Called when the game is being saved.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <see cref="IProtoEventListener"/>
        void IProtoEventListener.OnProtoSerialize(ProtobufSerializer serializer)
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
        void IProtoEventListener.OnProtoDeserialize(ProtobufSerializer serializer)
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
