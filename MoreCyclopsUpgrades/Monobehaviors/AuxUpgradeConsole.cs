namespace MoreCyclopsUpgrades
{
    using System;
    using ProtoBuf;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    [ProtoContract]
    public class AuxUpgradeConsole : HandTarget, IHandTarget, IProtoEventListener, IProtoTreeEventListener
    {
        // This will be set externally
        public SubRoot ParentCyclops = null;

        public Equipment Modules { get; private set; }

        private Constructable Buildable = null;

        public override void Awake()
        {
            base.Awake();

            if (Buildable == null)
            {
                Buildable = GetComponentInChildren<Constructable>();
            }

            if (SaveData == null)
            {
                string id = GetComponentInParent<PrefabIdentifier>().Id;
                SaveData = new AuxUpgradeConsoleSaveData(id);
            }

            if (this.Modules == null)
            {
                this.InitializeModules();
            }
        }

        private void InitializeModules()
        {
            if (this.ModulesRoot == null)
            {
                var equipmentRoot = new GameObject("EquipmentRoot");
                equipmentRoot.transform.SetParent(this.transform, false);
                this.ModulesRoot = equipmentRoot.AddComponent<ChildObjectIdentifier>();
            }

            this.Modules = new Equipment(base.gameObject, this.ModulesRoot.transform);
            this.Modules.SetLabel("CyclopsUpgradesStorageLabel");
            //this.UpdateVisuals();
            this.Modules.onEquip += this.OnEquip;
            this.Modules.onUnequip += this.OnUnequip;
            this.UnlockDefaultModuleSlots();
        }

        private void UnlockDefaultModuleSlots()
        {
            this.Modules.AddSlots(SlotHelper.SlotNames);
        }

        public void OnHandClick(GUIHand guiHand)
        {
            if (!Buildable.constructed)
                return;

            Player main = Player.main;
            PDA pda = main.GetPDA();
            Inventory.main.SetUsedStorage(this.Modules, false);
            pda.Open(PDATab.Inventory, null, null, -1f);
        }

        public void OnHandHover(GUIHand guiHand)
        {
            if (!Buildable.constructed)
                return;

            HandReticle main = HandReticle.main;
            main.SetInteractText("UseAuxConsole");
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        private void OnEquip(string slot, InventoryItem item)
        {
            CyclopsUpgradeChange();
            //this.UpdateVisuals();
            
            // Disallow deconstruction while there are modules in here
            Buildable.deconstructionAllowed = false;
        }

        private void OnUnequip(string slot, InventoryItem item)
        {
            CyclopsUpgradeChange();
            //this.UpdateVisuals();

            bool allEmpty = true;

            foreach (string slotName in SlotHelper.SlotNames)
                allEmpty &= Modules.GetTechTypeInSlot(slotName) == TechType.None;

            // Deconstruction only allowed if all slots are empty
            Buildable.deconstructionAllowed = allEmpty;
        }

        internal void CyclopsUpgradeChange()
        {
            ParentCyclops?.SetInstanceField("subModulesDirty", true);
        }

        //private void UpdateVisuals()
        //{
        //    this.SetModuleVisibility("Module1", this.Module1);
        //    this.SetModuleVisibility("Module2", this.Module2);
        //    this.SetModuleVisibility("Module3", this.Module3);
        //    this.SetModuleVisibility("Module4", this.Module4);
        //    this.SetModuleVisibility("Module5", this.Module5);
        //    this.SetModuleVisibility("Module6", this.Module6);
        //}

        //private bool SetModuleVisibility(string slot, GameObject module)
        //{
        //    if (module == null)
        //    {
        //        ErrorMessage.AddMessage($"SetModuleVisibility in slot {slot} module was null");
        //        return false;
        //    }

        //    bool hasItem = this.Modules.GetTechTypeInSlot(slot) != TechType.None;

        //    module.SetActive(hasItem);

        //    return hasItem;
        //}

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            this.version = 2;

            foreach (var slot in SlotHelper.SlotNames)
            {
                EmModuleSaveData savedModule = SaveData.GetModuleInSlot(slot);
                InventoryItem item = Modules.GetItemInSlot(slot);

                if (item == null)
                {
                    savedModule.ItemID = (int)TechType.None;
                    savedModule.BatteryCharge = -1f;
                }
                else
                {
                    savedModule.ItemID = (int)item.item.GetTechType();

                    var battery = item.item.GetComponent<Battery>();

                    if (battery == null)
                    {
                        savedModule.BatteryCharge = -1f;
                    }
                    else
                    {
                        savedModule.BatteryCharge = battery.charge;
                    }
                }

            }

            SaveData.Save();
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            if (this.Modules == null)
                this.InitializeModules();

            this.Modules.Clear();
        }

        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
        }

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            bool hasSaveData = this.SaveData.Load();
            if (hasSaveData)
            {
                // Because the items here aren't being serialized with everything else normally,
                // I've used custom save data to handle whatever gets left in these slots.

                // The following is a recreation of the essential parts of the Equipment.ResponseEquipment method.
                foreach (string slot in SlotHelper.SlotNames)
                {
                    // These slots need to be added before we can add items to them
                    this.Modules.AddSlot(slot);

                    EmModuleSaveData savedModule = SaveData.GetModuleInSlot(slot);

                    if (savedModule.ItemID == (int)TechType.None)
                        continue;

                    InventoryItem spanwedItem = CyclopsModule.SpawnCyclopsModule((TechType)savedModule.ItemID);

                    if (spanwedItem is null)
                        continue;

                    if (savedModule.BatteryCharge > 0f) // Modules without batteries are stored with a -1 value for charge
                        spanwedItem.item.GetComponent<Battery>().charge = savedModule.BatteryCharge;

                    this.Modules.AddItem(slot, spanwedItem, true);
                }
            }
            else
            {
                this.UnlockDefaultModuleSlots();
            }
        }

        //public GameObject Module1;

        //public GameObject Module2;

        //public GameObject Module3;

        //public GameObject Module4;

        //public GameObject Module5;

        //public GameObject Module6;

        public ChildObjectIdentifier ModulesRoot;

        private const int currentVersion = 2;

        [ProtoMember(1)]
        [NonSerialized]
        public int version;

        [ProtoMember(3, OverwriteList = true)]
        [NonSerialized]
        public AuxUpgradeConsoleSaveData SaveData;
    }
}

