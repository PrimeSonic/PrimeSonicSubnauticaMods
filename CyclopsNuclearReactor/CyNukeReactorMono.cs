namespace CyclopsNuclearReactor
{
    using Common;
    using MoreCyclopsUpgrades.Managers;
    using ProtoBuf;
    using System;
    using System.Reflection;
    using UnityEngine;

    [ProtoContract]
    internal class CyNukeReactorMono : HandTarget, IHandTarget, IProtoEventListener, IProtoTreeEventListener
    {
        internal static readonly string[] SlotNames = { "CyNuk1", "CyNuk2", "CyNuk3", "CyNuk4" };
        internal const int MaxSlots = 4;
        internal const float InitialReactorRodCharge = 10000f; // Half of what the Base Nuclear Reactor provides
        internal const float PowerMultiplier = 4.1f; // Rounded down from what the Base Nuclear Reactor provides

        public SubRoot ParentCyclops;
        public CyNukeChargeManager Manager;

        private Equipment _rodSlots;
        private ChildObjectIdentifier _rodsRoot;
        private Constructable _buildable;

        [ProtoMember(3, OverwriteList = true)]
        [NonSerialized]
        private CyNukeReactorSaveData _saveData;

        internal readonly TechType[] TechTypePerSlot = new TechType[MaxSlots] { TechType.None, TechType.None, TechType.None, TechType.None };
        internal readonly float[] ChargePerSlot = new float[MaxSlots] { -1f, -1f, -1f, -1f };

        internal float GetTotalAvailablePower()
        {
            float totalPower = 0;
            for (int slot = 0; slot < MaxSlots; slot++)
            {
                if (TechTypePerSlot[slot] == TechType.None)
                    continue;

                totalPower += ChargePerSlot[slot];
            }

            return totalPower;
        }

        internal bool HasPower()
        {
            for (int slot = 0; slot < MaxSlots; slot++)
            {
                if (TechTypePerSlot[slot] == TechType.None || TechTypePerSlot[slot] == TechType.DepletedReactorRod)
                    continue;

                if (ChargePerSlot[slot] > PowerManager.MinimalPowerValue)
                    return true;
            }

            return false;
        }

        public float ProducePower(ref float powerDeficit)
        {
            if (Mathf.Approximately(powerDeficit, 0f))
                return 0f;

            float totalPowerProduced = 0f;
            int slot = 0;
            while (slot < MaxSlots && powerDeficit < PowerManager.MinimalPowerValue)
            {
                if (TechTypePerSlot[slot] == TechType.None || TechTypePerSlot[slot] == TechType.DepletedReactorRod)
                    continue;

                if (ChargePerSlot[slot] <= PowerManager.MinimalPowerValue)
                    continue;

                float powerProduced = Mathf.Min(PowerMultiplier * DayNightCycle.main.deltaTime, ChargePerSlot[slot]);

                ChargePerSlot[slot] -= powerProduced;
                totalPowerProduced += powerProduced;
            }

            return totalPowerProduced;
        }

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
                _saveData = new CyNukeReactorSaveData(id);
            }

            if (_rodSlots == null)
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

        public void OnHandClick(GUIHand hand)
        {
            if (!_buildable.constructed)
                return;

            Player main = Player.main;
            PDA pda = main.GetPDA();
            Inventory.main.SetUsedStorage(_rodSlots, false);
            pda.Open(PDATab.Inventory, null, null, -1f);
        }

        public void OnHandHover(GUIHand hand)
        {
            if (!_buildable.constructed)
                return;

            HandReticle main = HandReticle.main;
            main.SetInteractText(CyNukReactorSMLHelper.OnHoverText()); // TODO - Provide some power info here
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

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
                    _rodSlots.AddSlot(slotName);

                    TechType techTypeID = rodData.TechTypeID;

                    if (techTypeID != TechType.None)
                    {
                        InventoryItem spanwedItem = SpawnItem(techTypeID);

                        if (spanwedItem != null)
                        {
                            _rodSlots.AddItem(slotName, spanwedItem, true);
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
            if (_rodSlots == null)
                InitializeRodSlots();

            _rodSlots.Clear();
        }

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            _saveData.ClearOldData();

            for (int i = 0; i < MaxSlots; i++)
            {
                if (TechTypePerSlot[i] == TechType.None)
                {
                    _saveData.AddEmptySlot();
                }
                else
                {
                    _saveData.AddRodData(TechTypePerSlot[i], ChargePerSlot[i]);
                }
            }

            _saveData.SaveData();
        }

        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
            // Intentionally empty
        }

        private void OnEquip(string slot, InventoryItem item)
        {
            int slotIndex = 0;
            while (slotIndex < MaxSlots && slot != SlotNames[slotIndex])
                slotIndex++;

            if (slotIndex > MaxSlots)
            {
                QuickLogger.Error($"Attempting to equip item to unknown slot '{slot}'");
                return;
            }

            TechTypePerSlot[slotIndex] = item.item.GetTechType();
            ChargePerSlot[slotIndex] = InitialReactorRodCharge;
        }

        private void OnUnequip(string slot, InventoryItem item)
        {
            int slotIndex = 0;
            while (slotIndex < MaxSlots && slot != SlotNames[slotIndex])
                slotIndex++;

            if (slotIndex > MaxSlots)
            {
                QuickLogger.Error($"Attempting to remove item to unknown slot '{slot}'");
                return;
            }

            TechTypePerSlot[slotIndex] = TechType.None;
            ChargePerSlot[slotIndex] = -1f;
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

            _rodSlots = new Equipment(base.gameObject, _rodsRoot.transform);
            _rodSlots.SetLabel(CyNukReactorSMLHelper.EquipmentLabel());
            _rodSlots.isAllowedToAdd += (Pickupable pickupable, bool verbose) => { return pickupable.GetTechType() == TechType.ReactorRod; };
            _rodSlots.isAllowedToRemove += (Pickupable pickupable, bool verbose) => { return pickupable.GetTechType() == TechType.DepletedReactorRod; };

            Type equipmentType = typeof(Equipment);
            EventInfo onEquipInfo = equipmentType.GetEvent("onEquip", BindingFlags.Public | BindingFlags.Instance);
            EventInfo onUnequipInfo = equipmentType.GetEvent("onUnequip", BindingFlags.Public | BindingFlags.Instance);

            Type cyConsoleType = typeof(CyNukeReactorMono);
            MethodInfo myOnEquipMethod = cyConsoleType.GetMethod("OnEquip", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo myOnUnequipMethod = cyConsoleType.GetMethod("OnUnequip", BindingFlags.Instance | BindingFlags.NonPublic);

            var onEquipDelegate = Delegate.CreateDelegate(typeof(Equipment.OnEquip), this, myOnEquipMethod);
            var onUnequipDelegate = Delegate.CreateDelegate(typeof(Equipment.OnUnequip), this, myOnUnequipMethod);

            onEquipInfo.AddEventHandler(_rodSlots, onEquipDelegate);
            onUnequipInfo.AddEventHandler(_rodSlots, onUnequipDelegate);

            UnlockDefaultRodSlots();
        }

        private void UnlockDefaultRodSlots()
        {
            _rodSlots.AddSlots(SlotNames);
        }

        private static InventoryItem SpawnItem(TechType techTypeID)
        {
            var gameObject = GameObject.Instantiate(CraftData.GetPrefabForTechType(techTypeID));

            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
            return new InventoryItem(pickupable);
        }
    }
}
