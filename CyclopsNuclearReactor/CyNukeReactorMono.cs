namespace CyclopsNuclearReactor
{
    using Common;
    using ProtoBuf;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ProtoContract]
    internal partial class CyNukeReactorMono : HandTarget, IHandTarget, IProtoEventListener, IProtoTreeEventListener
    {
        internal const int MaxSlots = 4;
        internal const float InitialReactorRodCharge = 10000f; // Half of what the Base Nuclear Reactor provides
        internal const float PowerMultiplier = 4.1f; // Rounded down from what the Base Nuclear Reactor provides

        private const float TextDelayInterval = 1.4f;
        private float textDelay = TextDelayInterval;

        public SubRoot ParentCyclops;
        public CyNukeChargeManager Manager;

        internal ItemsContainer RodsContainer;
        private ChildObjectIdentifier _rodsRoot;
        private Constructable _buildable;

        private bool pdaIsOpen = false;
        private bool isLoadingSaveData = false;


        private Dictionary<InventoryItem, uGUI_ItemIcon> _slotMapping;

        [ProtoMember(3, OverwriteList = true)]
        [NonSerialized]
        private CyNukeReactorSaveData _saveData;

        internal readonly List<SlotData> reactorRodData = new List<SlotData>(4);

        internal bool IsConstructed => _buildable != null && _buildable.constructed;

        internal string PowerIndicatorString()
        {
            if (reactorRodData.Count == 0)
                return CyNukReactorSMLHelper.NoPoweMessage();

            return NumberFormatter.FormatNumber(Mathf.CeilToInt(GetTotalAvailablePower()));
        }

        internal float GetTotalAvailablePower()
        {
            if (!this.IsConstructed || reactorRodData.Count == 0)
                return 0f;

            float totalPower = 0;
            foreach (SlotData slotData in reactorRodData)
            {
                if (!slotData.HasPower())
                    continue;

                totalPower += slotData.Charge;
            }

            return totalPower;
        }

        internal bool HasPower()
        {
            if (!this.IsConstructed)
                return false;

            foreach (SlotData slotData in reactorRodData)
            {
                if (slotData.HasPower())
                    return true;
            }

            return false;
        }

        public float ProducePower(ref float powerDeficit)
        {
            if (Mathf.Approximately(powerDeficit, 0f))
                return 0f;

            if (reactorRodData.Count == 0)
                return 0f;

            float totalPowerProduced = 0f;

            foreach (SlotData slotData in reactorRodData)
            {
                if (!slotData.HasPower())
                    continue;

                float powerProduced = Mathf.Min(PowerMultiplier * DayNightCycle.main.deltaTime, slotData.Charge);

                slotData.Charge -= powerProduced;
                totalPowerProduced += powerProduced;
                powerDeficit -= powerProduced;

                if (Mathf.Approximately(slotData.Charge, 0f))
                {
                    RodsContainer.RemoveItem(slotData.Item, true);
                    GameObject.Destroy(slotData.Item.gameObject);
                    RodsContainer.AddItem(SpawnItem(TechType.DepletedReactorRod).item);

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
                _buildable = GetComponentInParent<Constructable>();
            }

            if (_saveData == null)
            {
                string id = GetComponentInParent<PrefabIdentifier>().ClassId; // Changed this to classId because Id was returning null
                _saveData = new CyNukeReactorSaveData(id, MaxSlots);
            }

            if (RodsContainer == null)
            {
                InitializeRodsContainer();
            }
        }

        private void Start()
        {
            SubRoot cyclops = GetComponentInParent<SubRoot>();

            if (cyclops is null)
            {
                QuickLogger.Debug("Could not find Cyclops during Start. Attempting external syncronize.");
                CyNukeChargeManager.SyncReactors();
            }
            else
            {
                QuickLogger.Debug("Parent cyclops found directly!");
                ConnectToCyclops(cyclops);
            }
        }

        public void ConnectToCyclops(SubRoot cyclops, CyNukeChargeManager manager = null)
        {
            ParentCyclops = cyclops;
            this.transform.SetParent(cyclops.transform);

            Manager = manager ?? CyNukeChargeManager.GetManager(cyclops);

            if (!Manager.CyNukeReactors.Contains(this))
                Manager.CyNukeReactors.Add(this);

            QuickLogger.Debug("Cyclops Nuclear Reactor has been connected", true);
        }

        private void InitializeRodsContainer()
        {
            QuickLogger.Debug("Initializing Storage");
            if (_rodsRoot == null)
            {
                var storageRoot = new GameObject("StorageRoot");
                storageRoot.transform.SetParent(this.transform, false);
                _rodsRoot = storageRoot.AddComponent<ChildObjectIdentifier>();
            }

            RodsContainer = new ItemsContainer(2, 2, _rodsRoot.transform, CyNukReactorSMLHelper.StorageLabel(), null);
            RodsContainer.SetAllowedTechTypes(new[] { TechType.ReactorRod, TechType.DepletedReactorRod });

            RodsContainer.isAllowedToAdd += (Pickupable pickupable, bool verbose) =>
            {
                TechType techType = pickupable.GetTechType();

                return techType == TechType.ReactorRod || (isLoadingSaveData && techType == TechType.DepletedReactorRod);
            };

            RodsContainer.isAllowedToRemove += (Pickupable pickupable, bool verbose) =>
            {
                return pickupable.GetTechType() == TechType.DepletedReactorRod;
            };

            RodsContainer.onAddItem += OnAddItem;
            RodsContainer.onRemoveItem += OnRemoveItem;
        }

        #endregion

        #region Save Data

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            if (_saveData.LoadData())
            {
                isLoadingSaveData = true;
                QuickLogger.Debug("Loading save data");

                foreach (CyNukeRodSaveData rodData in _saveData.SlotData)
                {
                    TechType techTypeID = rodData.TechTypeID;

                    if (techTypeID != TechType.None)
                    {
                        InventoryItem spanwedItem = SpawnItem(techTypeID);

                        if (spanwedItem != null)
                        {
                            RodsContainer.AddItem(spanwedItem.item);
                            reactorRodData.Add(new SlotData(rodData.RemainingCharge, spanwedItem.item));
                        }
                    }
                }

                isLoadingSaveData = false;
            }
        }

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            isLoadingSaveData = true;

            if (RodsContainer == null)
                InitializeRodsContainer();

            RodsContainer.Clear();

            isLoadingSaveData = false;
        }

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            _saveData.ClearOldData();

            foreach (SlotData slotData in reactorRodData)
            {
                if (slotData.TechTypeID == TechType.None)
                {
                    _saveData.AddEmptySlot();
                }
                else
                {
                    _saveData.AddRodData(slotData.TechTypeID, slotData.Charge);
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

            Player main = Player.main;
            PDA pda = main.GetPDA();
            Inventory.main.SetUsedStorage(RodsContainer, false);
            pda.Open(PDATab.Inventory, null, new PDA.OnClose(CyOnPdaClose), 4f);

            pdaIsOpen = true;
        }

        public void OnHandHover(GUIHand hand)
        {
            if (!_buildable.constructed)
                return;

            HandReticle main = HandReticle.main;

            int currentPower = Mathf.FloorToInt(GetTotalAvailablePower());
            string text = currentPower > 0
                ? CyNukReactorSMLHelper.OnHoverPoweredText(currentPower)
                : CyNukReactorSMLHelper.OnHoverNoPowerText();

            main.SetInteractText(text);
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        internal void CyOnPdaClose(PDA pda)
        {
            _slotMapping = null;

            foreach (SlotData data in reactorRodData)
                data.InfoDisplay = null;

            pdaIsOpen = false;

            RodsContainer.onAddItem -= OnAddItemLate;
        }

        private void OnAddItem(InventoryItem item)
        {
            if (isLoadingSaveData)
                return;

            reactorRodData.Add(new SlotData(InitialReactorRodCharge, item.item));
        }

        private void OnRemoveItem(InventoryItem item)
        {
            SlotData slotData = reactorRodData.Find(rod => rod.Item == item.item);
            reactorRodData.Remove(slotData);
        }

        private void OnAddItemLate(InventoryItem item)
        {
            if (_slotMapping == null)
                return; // Safety check

            if (_slotMapping.TryGetValue(item, out uGUI_ItemIcon icon))
            {
                AddDisplayText(item, icon);
            }
        }

        internal void ConnectToContainer(Dictionary<InventoryItem, uGUI_ItemIcon> lookup)
        {
            _slotMapping = lookup;

            RodsContainer.onAddItem += OnAddItemLate;

            foreach (KeyValuePair<InventoryItem, uGUI_ItemIcon> pair in lookup)
            {
                InventoryItem item = pair.Key;
                uGUI_ItemIcon icon = pair.Value;

                AddDisplayText(item, icon);
            }
        }

        private void AddDisplayText(InventoryItem item, uGUI_ItemIcon icon)
        {
            SlotData slotData = reactorRodData.Find(rod => rod.Item == item.item);

            if (slotData != null && slotData.HasPower())
                slotData.AddDisplayText(icon);
        }

        private void UpdateDisplayText()
        {
            if (Time.time < textDelay)
                return; // Slow down the text update

            textDelay = Time.time + TextDelayInterval;

            foreach (SlotData item in reactorRodData)
            {
                if (!item.HasPower() || item.InfoDisplay == null)
                    continue;

                item.InfoDisplay.text = NumberFormatter.FormatNumber(Mathf.FloorToInt(item.Charge));
            }
        }

        #endregion

        private static InventoryItem SpawnItem(TechType techTypeID)
        {
            var gameObject = GameObject.Instantiate(CraftData.GetPrefabForTechType(techTypeID));

            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
            return new InventoryItem(pickupable);
        }
    }
}
