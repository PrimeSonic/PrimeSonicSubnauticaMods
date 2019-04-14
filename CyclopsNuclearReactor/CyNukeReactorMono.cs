namespace CyclopsNuclearReactor
{
    using Common;
    using MoreCyclopsUpgrades.Managers;
    using ProtoBuf;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ProtoContract]
    internal partial class CyNukeReactorMono : HandTarget, IHandTarget, IProtoEventListener, IProtoTreeEventListener
    {
        internal static readonly string[] SlotNames = { "CyNuk1", "CyNuk2", "CyNuk3", "CyNuk4" };
        internal const int MaxSlots = 4;
        internal const float InitialReactorRodCharge = 10000f; // Half of what the Base Nuclear Reactor provides
        internal const float PowerMultiplier = 4.1f; // Rounded down from what the Base Nuclear Reactor provides

        private const float TextDelayInterval = 2f;
        private float textDelay = TextDelayInterval;

        public SubRoot ParentCyclops;
        public CyNukeChargeManager Manager;

        internal Equipment RodSlots;
        private ChildObjectIdentifier _rodsRoot;
        private Constructable _buildable;

        private bool pdaIsOpen = false;

        private Dictionary<InventoryItem, uGUI_EquipmentSlot> _slotMapping;

        [ProtoMember(3, OverwriteList = true)]
        [NonSerialized]
        private CyNukeReactorSaveData _saveData;

        internal readonly SlotData[] slots = new SlotData[MaxSlots]
        {
            new SlotData(),
            new SlotData(),
            new SlotData(),
            new SlotData(),
        };

        internal bool IsConstructed => _buildable != null && _buildable.constructed;

        internal bool OverLimit = false;

        internal float GetTotalAvailablePower()
        {
            if (OverLimit)
                return 0f;

            float totalPower = 0;
            foreach (SlotData slotData in slots)
            {
                if (slotData.techType == TechType.None)
                    continue;

                totalPower += slotData.charge;
            }

            return totalPower;
        }

        internal bool HasPower()
        {
            foreach (SlotData slotData in slots)
            {
                if (slotData.charge > PowerManager.MinimalPowerValue)
                    return true;
            }

            return false;
        }

        public float ProducePower(ref float powerDeficit)
        {
            if (Mathf.Approximately(powerDeficit, 0f))
                return 0f;

            if (OverLimit)
                return 0f;

            float totalPowerProduced = 0f;
            int slot = 0;
            while (slot < MaxSlots && powerDeficit < PowerManager.MinimalPowerValue)
            {
                SlotData slotData = slots[slot];

                if (slotData.techType != TechType.ReactorRod)
                    continue;

                if (slotData.charge < PowerManager.MinimalPowerValue)
                    continue;

                float powerProduced = Mathf.Min(PowerMultiplier * DayNightCycle.main.deltaTime, slotData.charge);

                slotData.charge -= powerProduced;
                totalPowerProduced += powerProduced;

                if (Mathf.Approximately(slotData.charge, 0f))
                {
                    // Deplete reactor rod
                    string slotName = SlotNames[slot];

                    InventoryItem inventoryItem = RodSlots.RemoveItem(slotName, true, false);
                    GameObject.Destroy(inventoryItem.item.gameObject);
                    RodSlots.AddItem(slotName, SpawnItem(TechType.DepletedReactorRod), true);

                    ErrorMessage.AddMessage(CyNukReactorSMLHelper.DepletedMessage());
                }
            }

            if (pdaIsOpen)
                UpdateDisplayText();

            return totalPowerProduced;
        }

        #region Initialization

        public override void Awake()
        {
            base.Awake();

            if (_buildable == null)
            {
                _buildable = GetComponentInChildren<Constructable>();
            }

            if (_saveData == null)
            {
                string id = GetComponentInParent<PrefabIdentifier>().Id;
                _saveData = new CyNukeReactorSaveData(id, MaxSlots);
            }

            if (RodSlots == null)
            {
                InitializeRodSlots();
            }
        }

        private void Start()
        {
            SubRoot cyclops = GetComponentInParent<SubRoot>();

            if (cyclops is null)
            {
                QuickLogger.Debug("CyNukeReactorMono: Could not find Cyclops during Start. Attempting external syncronize.");
                CyNukeChargeManager.SyncReactors();
            }
            else
            {
                QuickLogger.Debug("CyNukeReactorMono: Parent cyclops found directly!");
                ConnectToCyclops(cyclops);
            }
        }

        public void ConnectToCyclops(SubRoot cyclops, CyNukeChargeManager manager = null)
        {
            ParentCyclops = cyclops;
            this.transform.SetParent(cyclops.transform);

            Manager = manager ?? CyNukeChargeManager.GetManager(cyclops);

            QuickLogger.Debug("Cyclops Nuclear Reactor has been connected", true);
        }

        private void InitializeRodSlots()
        {
            QuickLogger.Debug("Initializing Equipment");
            if (_rodsRoot == null)
            {
                var equipmentRoot = new GameObject("EquipmentRoot");
                equipmentRoot.transform.SetParent(this.transform, false);
                _rodsRoot = equipmentRoot.AddComponent<ChildObjectIdentifier>();
            }

            RodSlots = new Equipment(base.gameObject, _rodsRoot.transform);
            RodSlots.SetLabel(CyNukReactorSMLHelper.EquipmentLabel());
            RodSlots.isAllowedToAdd += (Pickupable pickupable, bool verbose) => { return pickupable.GetTechType() == TechType.ReactorRod; };
            RodSlots.isAllowedToRemove += (Pickupable pickupable, bool verbose) => { return pickupable.GetTechType() == TechType.DepletedReactorRod; };
            RodSlots.onEquip += OnEquip;
            RodSlots.onUnequip += OnUnequip;

            UnlockDefaultRodSlots();
        }

        private void UnlockDefaultRodSlots()
        {
            RodSlots.AddSlots(SlotNames);
        }

        #endregion

        #region Save Data

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            if (_saveData.LoadData())
            {
                QuickLogger.Debug("Loading save data");

                int slotIndex = 0;
                foreach (CyNukeRodSaveData rodData in _saveData.Rods)
                {
                    // These slots need to be added before we can add items to them
                    string slotName = SlotNames[slotIndex];
                    RodSlots.AddSlot(slotName);

                    TechType techTypeID = rodData.TechTypeID;

                    if (techTypeID != TechType.None)
                    {
                        InventoryItem spanwedItem = SpawnItem(techTypeID);

                        if (spanwedItem != null)
                        {
                            RodSlots.AddItem(slotName, spanwedItem, true);
                            QuickLogger.Debug($"Spawned '{techTypeID.AsString()}' into slot '{slotName}' from save data");
                        }
                    }

                    slotIndex++;
                }
            }
            else
            {
                UnlockDefaultRodSlots();
            }
        }

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            if (RodSlots == null)
                InitializeRodSlots();

            RodSlots.Clear();
        }

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            _saveData.ClearOldData();

            foreach (SlotData slotData in slots)
            {
                if (slotData.techType == TechType.None)
                {
                    _saveData.AddEmptySlot();
                }
                else
                {
                    _saveData.AddRodData(slotData.techType, slotData.charge);
                }
            }

            _saveData.SaveData();
        }

        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
            // Intentionally empty
        }

        #endregion

        #region Player Interaction

        public void OnHandClick(GUIHand hand)
        {
            if (!_buildable.constructed)
                return;

            if (OverLimit)
                return;

            Player main = Player.main;
            PDA pda = main.GetPDA();
            Inventory.main.SetUsedStorage(RodSlots, false);
            pda.Open(PDATab.Inventory, null, new PDA.OnClose(CyOnPdaClose), 4f);

            pdaIsOpen = true;
        }

        public void OnHandHover(GUIHand hand)
        {
            if (!_buildable.constructed)
                return;

            HandReticle main = HandReticle.main;

            if (OverLimit)
            {
                main.SetInteractText(CyNukReactorSMLHelper.OverLimit());
            }
            else
            {
                int currentPower = Mathf.FloorToInt(GetTotalAvailablePower());
                main.SetInteractText(CyNukReactorSMLHelper.OnHoverText(currentPower));
                main.SetIcon(HandReticle.IconType.Hand, 1f);
            }
        }

        internal void CyOnPdaClose(PDA pda)
        {
            _slotMapping = null;

            foreach (SlotData data in slots)
                data.text = null;

            pdaIsOpen = false;

            RodSlots.onEquip -= OnEquipLate;
        }

        private void OnEquip(string slot, InventoryItem item)
        {
            int slotIndex = FindSlotIndex(slot);

            if (slotIndex > MaxSlots)
            {
                QuickLogger.Error($"Attempting to equip item to unknown slot '{slot}'");
                return;
            }

            slots[slotIndex] = new SlotData(InitialReactorRodCharge, item.item);
        }

        private void OnUnequip(string slot, InventoryItem item)
        {
            int slotIndex = FindSlotIndex(slot);

            if (slotIndex > MaxSlots)
            {
                QuickLogger.Error($"Attempting to remove item to unknown slot '{slot}'");
                return;
            }

            slots[slotIndex] = new SlotData();
        }

        private void OnEquipLate(string slot, InventoryItem item)
        {
            if (_slotMapping == null)
                return; // Safety check

            if (_slotMapping.TryGetValue(item, out uGUI_EquipmentSlot icon))
            {
                int slotIndex = FindSlotIndex(slot);

                if (slotIndex > MaxSlots)
                {
                    QuickLogger.Error($"Attempting to equip item to unknown slot '{slot}'");
                    return;
                }

                slots[slotIndex].AddDisplayText(icon);
            }
        }

        internal void ConnectToEquipment(Dictionary<InventoryItem, uGUI_EquipmentSlot> lookup)
        {
            _slotMapping = lookup;

            RodSlots.onEquip += OnEquipLate;

            foreach (KeyValuePair<InventoryItem, uGUI_EquipmentSlot> pair in _slotMapping)
            {
                InventoryItem item = pair.Key;
                uGUI_EquipmentSlot icon = pair.Value;

                int slotIndex = FindSlotIndex(item);

                slots[slotIndex].AddDisplayText(icon);
            }
        }

        private void UpdateDisplayText()
        {
            if (Time.time < textDelay)
                return; // Slow down the text update

            textDelay = Time.time + TextDelayInterval;

            foreach (SlotData item in slots)
            {
                if (item.techType != TechType.ReactorRod || item.text == null)
                    continue;

                item.text.text = NumberFormatter.FormatNumber(Mathf.FloorToInt(item.charge));
            }
        }

        #endregion

        private int FindSlotIndex(InventoryItem item)
        {
            int slotIndex = 0;
            while (slotIndex < MaxSlots && slots[slotIndex].pickupable != item.item)
                slotIndex++;

            return slotIndex;
        }

        private static int FindSlotIndex(string slot)
        {
            int slotIndex = 0;
            while (slotIndex < MaxSlots && slot != SlotNames[slotIndex])
                slotIndex++;
            return slotIndex;
        }

        private static InventoryItem SpawnItem(TechType techTypeID)
        {
            var gameObject = GameObject.Instantiate(CraftData.GetPrefabForTechType(techTypeID));

            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
            return new InventoryItem(pickupable);
        }
    }
}
