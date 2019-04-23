using CyclopsNuclearReactor.Helpers;

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
        internal const float InitialReactorRodCharge = 10000f; // Half of what the Base Nuclear Reactor provides
        internal const float PowerMultiplier = 4.1f; // Rounded down from what the Base Nuclear Reactor provides

        internal const int MaxUpgradeLevel = 2;
        internal const int ContainerHeight = 5;
        internal const int ContainerWidth = 2;
        internal const int MaxSlots = ContainerHeight * ContainerWidth;

        internal int MaxActiveSlots => 2 + lastKnownUpgradeLevel * 4;

        private int lastKnownUpgradeLevel = 0;
        private const float TextDelayInterval = 1.4f;
        private float textDelay = TextDelayInterval;

        public SubRoot ParentCyclops = null;
        public CyNukeChargeManager Manager = null;
        internal ItemsContainer RodsContainer = null;
        private ChildObjectIdentifier _rodsRoot = null;
        private Constructable _buildable = null;

        private bool pdaIsOpen = false;
        private bool isLoadingSaveData = false;
        private bool isDepletingRod = false;
        private int activeRods = 0;

        private Dictionary<InventoryItem, uGUI_ItemIcon> _slotMapping;

        [ProtoMember(3, OverwriteList = true)]
        [NonSerialized]
        private CyNukeReactorSaveData _saveData;

        internal int TotalItemCount => reactorRodData.Count;

        internal int ActiveRodCount
        {
            get
            {
                int count = 0;
                foreach (SlotData slot in reactorRodData)
                {
                    if (slot.HasPower())
                        count++;
                }
                return count;
            }
        }

        internal readonly List<SlotData> reactorRodData = new List<SlotData>(MaxSlots);

        internal bool IsConstructed => _buildable != null && _buildable.constructed;

        internal string PowerIndicatorString()
        {
            if (reactorRodData.Count == 0)
                return CyNukReactorBuildable.NoPoweMessage();

            return NumberFormatter.FormatNumber(Mathf.CeilToInt(GetTotalAvailablePower()));
        }

        internal float GetTotalAvailablePower()
        {
            if (!this.IsConstructed || reactorRodData.Count == 0)
                return 0f;

            float totalPower = 0;
            for (int i = 0; i < this.MaxActiveSlots; i++)
            {
                SlotData slotData = reactorRodData[i];

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

            for (int i = 0; i < this.MaxActiveSlots; i++)
            {
                SlotData slotData = reactorRodData[i];

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

            SlotData depletedRod = null;
            for (int i = 0; i < this.MaxActiveSlots; i++)
            {
                SlotData slotData = reactorRodData[i];

                if (!slotData.HasPower())
                    continue;

                float powerProduced = Mathf.Min(PowerMultiplier * DayNightCycle.main.deltaTime, slotData.Charge);

                slotData.Charge -= powerProduced;
                totalPowerProduced += powerProduced;
                powerDeficit -= powerProduced;

                if (Mathf.Approximately(slotData.Charge, 0f))
                    depletedRod = slotData;

                UpdateGraphicalRod(slotData);
            }

            if (depletedRod != null)
            {
                isDepletingRod = true;

                RodsContainer.RemoveItem(depletedRod.Item, true);
                GameObject.Destroy(depletedRod.Item.gameObject);
                RodsContainer.AddItem(SpawnItem(TechType.DepletedReactorRod).item);
                activeRods--;

                ErrorMessage.AddMessage(CyNukReactorBuildable.DepletedMessage());

                isDepletingRod = false;
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
                string id = GetComponentInParent<PrefabIdentifier>().Id;
                _saveData = new CyNukeReactorSaveData(id, MaxSlots);
            }

            InitializeRodsContainer();
        }

        private void Start()
        {
            if (Manager != null)
            {
                ConnectToCyclops(Manager.Cyclops, Manager);
            }
            else if (ParentCyclops != null)
            {
                ConnectToCyclops(ParentCyclops);
            }
            else
            {
                SubRoot cyclops = GetComponentInParent<SubRoot>();

                if (cyclops == null)
                {
                    QuickLogger.Debug("Could not find Cyclops during Start. Attempting external synchronize.");
                    CyNukeChargeManager.SyncReactors();
                }
                else
                {
                    QuickLogger.Debug("Parent cyclops found directly!");
                    ConnectToCyclops(cyclops);
                }
            }
        }

        public void ConnectToCyclops(SubRoot cyclops, CyNukeChargeManager manager = null)
        {
            ParentCyclops = cyclops;
            this.transform.SetParent(cyclops.transform);

            Manager = manager ?? CyNukeChargeManager.GetManager(cyclops);
            Manager.AddReactor(this);

            QuickLogger.Debug("Cyclops Nuclear Reactor has been connected", true);
        }

        private void InitializeRodsContainer()
        {
            if (_rodsRoot == null)
            {
                QuickLogger.Debug("Initializing StorageRoot");
                var storageRoot = new GameObject("StorageRoot");
                storageRoot.transform.SetParent(this.transform, false);
                _rodsRoot = storageRoot.AddComponent<ChildObjectIdentifier>();
            }

            if (RodsContainer == null)
            {
                QuickLogger.Debug("Initializing RodsContainer");
                RodsContainer = new ItemsContainer(ContainerWidth, ContainerHeight, _rodsRoot.transform, CyNukReactorBuildable.StorageLabel(), null);
                RodsContainer.SetAllowedTechTypes(new[] { TechType.ReactorRod, TechType.DepletedReactorRod });

                RodsContainer.isAllowedToAdd += IsAllowedToAdd;
                RodsContainer.isAllowedToRemove += IsAllowedToRemove;

                RodsContainer.onAddItem += OnAddItem;
                RodsContainer.onRemoveItem += OnRemoveItem;
            }
        }

        private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
            TechType techType = pickupable.GetTechType();
            return techType == TechType.ReactorRod ||
                   (DepletedRodsAllowed() && techType == TechType.DepletedReactorRod);
        }

        private bool DepletedRodsAllowed()
        {
            return isLoadingSaveData || isDepletingRod;
        }

        private bool IsAllowedToRemove(Pickupable pickupable, bool verbose)
        {
            TechType techType = pickupable.GetTechType();
            return techType == TechType.DepletedReactorRod ||
                  (isDepletingRod && techType == TechType.ReactorRod);
        }

        #endregion

        #region Save Data

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            QuickLogger.Debug("Loading save data");

            if (_saveData.LoadData())
            {
                isLoadingSaveData = true;
                QuickLogger.Debug("Save data found");

                RodsContainer.Clear(false);
                reactorRodData.Clear();

                foreach (CyNukeRodSaveData rodData in _saveData.SlotData)
                {
                    TechType techTypeID = rodData.TechTypeID;

                    if (techTypeID != TechType.None)
                    {
                        InventoryItem spanwedItem = SpawnItem(techTypeID);

                        if (spanwedItem != null)
                        {
                            InventoryItem rod = RodsContainer.AddItem(spanwedItem.item);
                            reactorRodData.Add(new SlotData(rodData.RemainingCharge, rod.item));
                        }
                    }
                }

                QuickLogger.Debug($"Added {this.ActiveRodCount} items from save data");

                isLoadingSaveData = false;
            }
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            isLoadingSaveData = true;

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

            int currentPower = Mathf.CeilToInt(GetTotalAvailablePower());

            string text = currentPower > 0
                ? CyNukReactorBuildable.OnHoverPoweredText(currentPower, activeRods, this.MaxActiveSlots)
                : CyNukReactorBuildable.OnHoverNoPowerText();

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
            if (item.item.GetTechType() == TechType.ReactorRod)
                activeRods++;

            if (isLoadingSaveData)
                return;
            reactorRodData.Add(new SlotData(InitialReactorRodCharge, item.item));
        }

        private void OnRemoveItem(InventoryItem item)
        {
            SlotData slotData = reactorRodData.Find(rod => rod.Item == item.item);
            CyNukeRodHelper.EmptyRod(gameObject, reactorRodData.FindIndex(a => a == slotData));
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

            if (slotData != null)
                slotData.AddDisplayText(icon);
        }

        private void UpdateDisplayText()
        {
            if (Time.time < textDelay)
                return; // Slow down the text update

            textDelay = Time.time + TextDelayInterval;

            for (int i = 0; i < reactorRodData.Count; i++)
            {
                SlotData item = reactorRodData[i];
                if (item.InfoDisplay == null)
                    continue;

                if (item.HasPower())
                {
                    if (i < this.MaxActiveSlots)
                        item.InfoDisplay.text = NumberFormatter.FormatNumber(Mathf.FloorToInt(item.Charge));
                    else
                        item.InfoDisplay.text = CyNukReactorBuildable.InactiveRodMsg();
                }
                else
                {
                    item.InfoDisplay.text = CyNukReactorBuildable.NoPoweMessage();
                }
            }
        }

        #endregion

        #region Rod Updates
        private void UpdateGraphicalRod(SlotData slotData)
        {
            var graphicalRod = CyNukeRodHelper.Find(gameObject, reactorRodData.FindIndex(a => a == slotData));

            if (graphicalRod != null)
            {
                var uranium = graphicalRod.FindChild("PowerRod_Uranium")?.gameObject;

                if (uranium != null)
                {
                    uranium.transform.localPosition = NewPostion(uranium, slotData);
                }
                else
                {
                    QuickLogger.Error($"PowerRod_Uranium not found in GameObject {graphicalRod.name}");
                }
            }
            else
            {
                QuickLogger.Error($"GraphicalRod is null", true);
            }
        }

        private Vector3 NewPostion(GameObject uranium, SlotData slotData)
        {
            if (uranium == null) return Vector3.zero;
            var fuelPercentage = slotData.Charge / InitialReactorRodCharge;
            return new Vector3(uranium.transform.localPosition.x, fuelPercentage, uranium.transform.localPosition.z);
        }
        #endregion

        internal void UpdateUpgradeLevel(int upgradeLevel)
        {
            if (upgradeLevel < 0 || upgradeLevel > MaxUpgradeLevel)
                return;

            if (upgradeLevel == lastKnownUpgradeLevel)
                return;

            lastKnownUpgradeLevel = upgradeLevel;
            ErrorMessage.AddMessage(CyNukReactorBuildable.UpgradedMsg());
        }

        private void OnDestroy()
        {
            Manager.CyNukeReactors.Remove(this);
            ParentCyclops = null;
            Manager = null;
        }

        private static InventoryItem SpawnItem(TechType techTypeID)
        {
            var gameObject = GameObject.Instantiate(CraftData.GetPrefabForTechType(techTypeID));

            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
            return new InventoryItem(pickupable);
        }
    }
}
