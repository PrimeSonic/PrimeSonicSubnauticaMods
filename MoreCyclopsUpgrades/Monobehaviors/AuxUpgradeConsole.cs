namespace MoreCyclopsUpgrades
{
    using System;
    using System.Collections.Generic;
    using SMLHelper.V2.Utility;
    using ProtoBuf;
    using UnityEngine;

    [ProtoContract]
    public class AuxUpgradeConsole : HandTarget, IHandTarget, IProtoEventListener, IProtoTreeEventListener
    {
        internal static readonly IEnumerable<string> Slots = new string[]
        {
            "Module1",
            "Module2",
            "Module3",
            "Module4",
            "Module5",
            "Module6"
        };

        public Equipment Modules { get; private set; }

        public override void Awake()
        {
            base.Awake();

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
            this.Modules.AddSlots(Slots);
        }

        public void OnHandClick(HandTargetEventData eventData)
        {
            OnHandClick(eventData.guiHand);
        }

        public void OnHandClick(GUIHand guiHand)
        {
            Player main = Player.main;
            PDA pda = main.GetPDA();
            Inventory.main.SetUsedStorage(this.Modules, false);
            pda.Open(PDATab.Inventory, null, null, -1f);
        }

        public void OnHandHover(HandTargetEventData eventData)
        {
            OnHandHover(eventData.guiHand);
        }

        public void OnHandHover(GUIHand guiHand)
        {
            HandReticle main = HandReticle.main;
            main.SetInteractText("UseAuxConsole");
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        private void OnEquip(string slot, InventoryItem item)
        {
            //this.UpdateVisuals();                   
        }

        private void OnUnequip(string slot, InventoryItem item)
        {
            //this.UpdateVisuals();                   
        }

        //private void UpdateVisuals()
        //{
        //    this.SetModuleVisibility("Module1", this.module1);
        //    this.SetModuleVisibility("Module2", this.module2);
        //    this.SetModuleVisibility("Module3", this.module3);
        //    this.SetModuleVisibility("Module4", this.module4);
        //    this.SetModuleVisibility("Module5", this.module5);
        //    this.SetModuleVisibility("Module6", this.module6);
        //}

        //private void SetModuleVisibility(string slot, GameObject module)
        //{
        //    if (module == null)
        //    {
        //        return;
        //    }
        //    module.SetActive(this.modules.GetTechTypeInSlot(slot) != TechType.None);
        //}

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            this.version = 2;

            foreach (var slot in Slots)
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
                var prEquipment = (Dictionary<string, InventoryItem>)this.Modules.GetInstanceField("equipment");
                var prEquipmentCount = (Dictionary<TechType, int>)this.Modules.GetInstanceField("equippedCount");

                foreach (string slot in Slots)
                {
                    EmModuleSaveData savedModule = SaveData.GetModuleInSlot(slot);

                    if (savedModule.ItemID == (int)TechType.None)
                        continue;

                    InventoryItem spanwedItem = CyclopsModule.SpawnCyclopsModule((TechType)savedModule.ItemID);

                    if (spanwedItem is null)
                        continue;

                    if (savedModule.BatteryCharge > 0f)
                        spanwedItem.item.GetComponent<Battery>().charge = savedModule.BatteryCharge;

                    // The code below is mostly a copy of Equipment.AddItem
                    // AddItem couldn't be called directly due to 
                    spanwedItem.container = this.Modules;
                    spanwedItem.item.Reparent(this.Modules.tr);

                    prEquipment[slot] = spanwedItem;

                    TechType techType = spanwedItem.item.GetTechType();

                    if (prEquipmentCount.ContainsKey(techType))
                        prEquipmentCount[techType]++;
                    else
                        prEquipmentCount.Add(techType, 1);

                    Equipment.SendEquipmentEvent(spanwedItem.item, 0, this.Modules.owner, slot);
                }
            }

            this.UnlockDefaultModuleSlots();
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

