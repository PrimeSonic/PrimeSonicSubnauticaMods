namespace CyclopsNuclearReactor
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Common;
    using CyclopsNuclearReactor.Helpers;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Buildables;
    using ProtoBuf;
    using UnityEngine;

    [ProtoContract]
    internal partial class CyNukeReactorMono : HandTarget, IHandTarget, IProtoEventListener, ICyclopsBuildable
    {
        internal static bool PdaIsOpen = false;
        internal static CyNukeReactorMono OpenInPda = null;

        internal const float InitialReactorRodCharge = 10000f; // Half of what the Base Nuclear Reactor provides
        internal const float PowerMultiplier = 4.05f; // Rounded down and slightly reduced from what the Base Nuclear Reactor provides

        internal const int MaxUpgradeLevel = 2;
        internal const int ContainerHeight = 5;
        internal const int ContainerWidth = 2;
        internal const int MaxSlots = ContainerHeight * ContainerWidth;

        internal int MaxActiveSlots => 2 + lastKnownUpgradeLevel * 4;

        private int lastKnownUpgradeLevel = 0;
        private const float TextDelayInterval = 1.5f;
        private float textDelay = TextDelayInterval;

        public SubRoot Cyclops = null;
        private CyNukeManager _manager = null;
        public CyNukeManager Manager
        {
            get => _manager ?? (_manager = MCUServices.Find.AuxCyclopsManager<CyNukeManager>(Cyclops));
            set => _manager = value;
        }

        private ItemsContainer container = null;
        private ChildObjectIdentifier _rodsRoot = null;
        private string prefabId = null;

        private Constructable _buildable = null;
        internal Constructable Buildable
        {
            get
            {
                if (_buildable == null)
                {
                    _buildable = GetComponentInParent<Constructable>() ?? GetComponent<Constructable>();
                }

                return _buildable;
            }
        }

        private CyNukeEnhancerHandler upgradeHandler;
        private CyNukeEnhancerHandler UpgradeHandler => upgradeHandler ??
            (upgradeHandler = MCUServices.Find.CyclopsGroupUpgradeHandler<CyNukeEnhancerHandler>(Cyclops, CyNukeEnhancerMk1.TechTypeID, CyNukeEnhancerMk2.TechTypeID));

        private bool isLoadingSaveData = false;
        private bool isDepletingRod = false;

        private Dictionary<InventoryItem, uGUI_ItemIcon> pdaInventoryMapping;

        [ProtoMember(3, OverwriteList = true)]
        [NonSerialized]
        private CyNukeReactorSaveData _saveData;

        internal int TotalItemCount => reactorRodData.Count;

        internal int ActiveRodCount
        {
            get
            {
                int count = 0;
                for (int r = 0; r < reactorRodData.Count; r++)
                {
                    if (reactorRodData[r].HasPower())
                        count++;
                }

                return Math.Min(count, this.MaxActiveSlots);
            }
        }

        internal readonly List<SlotData> reactorRodData = new List<SlotData>(MaxSlots);

        internal bool IsConstructed => this.Buildable != null && this.Buildable.constructed;

        public bool IsConnectedToCyclops => Cyclops != null && this.Manager != null;

        internal string PowerIndicatorString()
        {
            if (reactorRodData.Count == 0)
                return CyNukReactorBuildable.NoPoweMessage();

            return NumberFormatter.FormatValue(GetTotalAvailablePower());
        }

        internal float GetTotalAvailablePower()
        {
            if (!this.IsConstructed || reactorRodData.Count == 0)
                return 0f;

            float totalPower = 0;
            int max = Math.Min(this.MaxActiveSlots, this.TotalItemCount);
            for (int i = 0; i < max; i++)
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

            int max = Math.Min(this.MaxActiveSlots, this.TotalItemCount);
            for (int i = 0; i < max; i++)
            {
                SlotData slotData = reactorRodData[i];

                if (slotData.HasPower())
                    return true;
            }

            return false;
        }

        public float GetPower(ref float powerDeficit)
        {
            if (isDepletingRod)
                return 0f;

            if (powerDeficit <= MCUServices.MinimalPowerValue)
                return 0f;

            if (reactorRodData.Count == 0)
                return 0f;

            float totalPowerProduced = 0f;

            int max = Math.Min(this.MaxActiveSlots, this.TotalItemCount);
            for (int i = 0; i < max; i++)
            {
                if (powerDeficit <= MCUServices.MinimalPowerValue)
                    break;

                SlotData slotData = reactorRodData[i];

                if (!slotData.HasPower())
                    continue;

                float powerProduced = Mathf.Min(PowerMultiplier * DayNightCycle.main.deltaTime, slotData.Charge);
                powerProduced = Mathf.Min(powerDeficit, powerProduced);

                slotData.Charge -= powerProduced;
                totalPowerProduced += powerProduced;
                powerDeficit -= powerProduced;

                UpdateGraphicalRod(slotData);
            }

            return totalPowerProduced;
        }

        #region Initialization

        public override void Awake()
        {
            base.Awake();

            if (_saveData == null)
                ReadySaveData();

            InitializeRodsContainer();
        }

        private void Start()
        {
            SubRoot cyclops = GetComponentInParent<SubRoot>();

            if (cyclops != null)
            {
                MCUServices.Logger.Debug("Parent cyclops found directly!");
                ConnectToCyclops(cyclops);
            }
            else
            {
                InvokeRepeating(nameof(GetMCUHandler), 3f, 1f);
            }
        }

        private void ReadySaveData()
        {
            if (prefabId == null)
            {
                PrefabIdentifier prefabIdentifier = GetComponentInParent<PrefabIdentifier>() ?? GetComponent<PrefabIdentifier>();
                prefabId = prefabIdentifier.Id;
            }

            if (prefabId != null && _saveData == null)
            {
                MCUServices.Logger.Debug($"CyNukeReactorMono PrefabIdentifier {prefabId}");
                _saveData = new CyNukeReactorSaveData(prefabId, MaxSlots);
            }
        }

        private void GetMCUHandler()
        {
            Cyclops = Cyclops ?? GetComponentInParent<SubRoot>();

            if (Cyclops == null)
            {
                MCUServices.Logger.Debug("Could not find Cyclops during Start. Attempting external synchronize.");
                CyNukeManager.SyncAllReactors();
            }
            else if (this.Manager == null)
            {
                MCUServices.Logger.Debug("Looking for CyNukeChargeManager.");
                this.Manager = MCUServices.Find.AuxCyclopsManager<CyNukeManager>(Cyclops);
            }

            if (this.Manager != null)
                CancelInvoke(nameof(GetMCUHandler));
        }

        public void ConnectToCyclops(SubRoot parentCyclops, CyNukeManager manager = null)
        {
            if (this.IsConnectedToCyclops)
                return;

            Cyclops = parentCyclops;
            this.transform.SetParent(parentCyclops.transform);

            if (this.Manager != null)
            {
                this.Manager.AddBuildable(this);
            }

            if (this.UpgradeHandler != null)
            {
                UpdateUpgradeLevel(this.UpgradeHandler.HighestValue);
            }
        }

        private void InitializeRodsContainer()
        {
            if (_rodsRoot == null)
            {
                MCUServices.Logger.Debug("Initializing StorageRoot");

                var storageRoot = new GameObject("StorageRoot");
                storageRoot.transform.SetParent(this.transform, false);
                _rodsRoot = storageRoot.AddComponent<ChildObjectIdentifier>();
            }

            if (container == null)
            {
                MCUServices.Logger.Debug("Initializing RodsContainer");

                container = new ItemsContainer(ContainerWidth, ContainerHeight, _rodsRoot.transform, CyNukReactorBuildable.StorageLabel(), null);
                container.SetAllowedTechTypes(new[] { TechType.ReactorRod, TechType.DepletedReactorRod });

                container.isAllowedToAdd += IsAllowedToAdd;
                container.isAllowedToRemove += IsAllowedToRemove;

                (container as IItemsContainer).onAddItem += OnAddItem;
                (container as IItemsContainer).onRemoveItem += OnRemoveItem;

                EventInfo onChangeItemPositionInfo = typeof(ItemsContainer).GetEvent("onChangeItemPosition", BindingFlags.Public | BindingFlags.Instance);
                MethodInfo myChangeItemPositionMethod = typeof(CyNukeReactorMono).GetMethod(nameof(CyNukeReactorMono.RodsContainer_onChangeItemPosition), BindingFlags.NonPublic | BindingFlags.Instance);

                var onChangeDelegate = Delegate.CreateDelegate(typeof(ItemsContainer.OnChangeItemPosition), this, myChangeItemPositionMethod);
                onChangeItemPositionInfo.AddEventHandler(container, onChangeDelegate);
            }
        }

        private void RodsContainer_onChangeItemPosition(InventoryItem item)
        {
            RefreshAllRods();
        }

        private void RefreshAllRods()
        {
            for (int i = 0; i < MaxSlots; i++)
            {
                if (i <= reactorRodData.Count - 1)
                {
                    SlotData slotData = reactorRodData[i];
                    UpdateGraphicalRod(slotData);
                }
                else
                {
                    CyNukeRodHelper.EmptyRod(this.gameObject, i);
                }
            }
        }

        private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
            TechType techType = pickupable.GetTechType();
            return techType == TechType.ReactorRod || // Normal case
                   ((isLoadingSaveData || isDepletingRod) && // When depleted rods are allowed
                   techType == TechType.DepletedReactorRod);
        }

        private bool IsAllowedToRemove(Pickupable pickupable, bool verbose)
        {
            TechType techType = pickupable.GetTechType();
            return techType == TechType.DepletedReactorRod ||
                  (isDepletingRod && techType == TechType.ReactorRod);
        }

        #endregion

        private void Update() // The all important Update method
        {
            if (PdaIsOpen)
            {
                UpdateDisplayText();
                int max = Math.Min(this.MaxActiveSlots, this.TotalItemCount);
                for (int i = 0; i < max; i++)
                {
                    SlotData slotData = reactorRodData[i];

                    if (slotData.TechTypeID == TechType.DepletedReactorRod || slotData.HasPower())
                        continue;

                    isDepletingRod = true;

                    SlotData depletedRod = slotData;
                    container.RemoveItem(depletedRod.Item, true);
                    GameObject.Destroy(depletedRod.Item.gameObject);
                    container.AddItem(SpawnItem(TechType.DepletedReactorRod).item);

                    ErrorMessage.AddMessage(CyNukReactorBuildable.DepletedMessage());

                    isDepletingRod = false;
                    break;
                }
            }
        }

        #region Save Data

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            if (_saveData == null)
                ReadySaveData();

            InitializeRodsContainer();

            MCUServices.Logger.Debug("Loading save data");

            isLoadingSaveData = true;

            if (_saveData != null && _saveData.LoadData())
            {
                MCUServices.Logger.Debug("Save data found");

                container.Clear(false);
                reactorRodData.Clear();

                int nonEmptySlots = 0;
                for (int r = 0; r < _saveData.SlotData.Count; r++)
                {
                    CyNukeRodSaveData rodData = _saveData.SlotData[r];
                    TechType techTypeID = rodData.TechTypeID;

                    if (techTypeID != TechType.None)
                    {
                        InventoryItem spanwedItem = SpawnItem(techTypeID);

                        if (spanwedItem != null)
                        {
                            MCUServices.Logger.Debug($"Adding {techTypeID.AsString()} with {rodData.RemainingCharge} charge from save data");
                            InventoryItem rod = container.AddItem(spanwedItem.item);
                            AddNewRod(rodData.RemainingCharge, rod.item);
                            nonEmptySlots++;
                        }
                    }
                }

                MCUServices.Logger.Debug($"Added {nonEmptySlots} items from save data");
            }
            else
            {
                MCUServices.Logger.Debug("No save data found");
            }

            isLoadingSaveData = false;
        }

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            if (_saveData == null)
                ReadySaveData();

            _saveData.ClearOldData();
            _saveData.AddSlotData(reactorRodData);
            _saveData.SaveData();
        }


        #endregion

        #region Player Interaction

        public void OnHandClick(GUIHand hand)
        {
            if (!this.IsConstructed)
                return;

            PdaIsOpen = true;
            OpenInPda = this;

            PDA pda = Player.main.GetPDA();
            Inventory.main.SetUsedStorage(container, false);
            pda.Open(PDATab.Inventory, null, new PDA.OnClose(CyOnPdaClose), 4f);
        }

        public void OnHandHover(GUIHand hand)
        {
            if (!this.Buildable.constructed)
                return;

            HandReticle main = HandReticle.main;

            int currentPower = Mathf.CeilToInt(GetTotalAvailablePower());

            string text = currentPower > 0
                ? CyNukReactorBuildable.OnHoverPoweredText(NumberFormatter.FormatValue(currentPower), this.ActiveRodCount, this.MaxActiveSlots)
                : CyNukReactorBuildable.OnHoverNoPowerText();

            main.SetInteractText(text);
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        internal void CyOnPdaClose(PDA pda)
        {
            pdaInventoryMapping = null;

            for (int r = 0; r < reactorRodData.Count; r++)
                reactorRodData[r].InfoDisplay = null;

            PdaIsOpen = false;
            OpenInPda = null;

            (container as IItemsContainer).onAddItem -= OnAddItemLate;
        }

        private void OnAddItem(InventoryItem item)
        {
            if (isLoadingSaveData)
                return;

            AddNewRod(item.item.GetTechType() == TechType.DepletedReactorRod ? 0 : InitialReactorRodCharge, item.item);
        }

        private void OnRemoveItem(InventoryItem item)
        {
            SlotData slotData = reactorRodData.Find(rod => rod.Item == item.item);
            CyNukeRodHelper.EmptyRod(this.gameObject, reactorRodData.FindIndex(a => a == slotData));
            reactorRodData.Remove(slotData);
        }

        private void OnAddItemLate(InventoryItem item)
        {
            if (pdaInventoryMapping == null)
                return; // Safety check

            if (pdaInventoryMapping.TryGetValue(item, out uGUI_ItemIcon icon))
            {
                AddDisplayText(item, icon);
            }
        }

        internal void ConnectToContainer(Dictionary<InventoryItem, uGUI_ItemIcon> lookup)
        {
            pdaInventoryMapping = lookup;

            (container as IItemsContainer).onAddItem += OnAddItemLate;

            if (this.TotalItemCount == 0)
                return;

            foreach (KeyValuePair<InventoryItem, uGUI_ItemIcon> pair in lookup)
            {
                InventoryItem item = pair.Key;
                uGUI_ItemIcon icon = pair.Value;

                AddDisplayText(item, icon);
            }

            UpdateDisplayText(true);
        }

        private void AddDisplayText(InventoryItem item, uGUI_ItemIcon icon)
        {
            SlotData slotData = reactorRodData.Find(rod => rod.Item == item.item);

            if (slotData != null)
                slotData.AddDisplayText(icon);
        }

        private void UpdateDisplayText(bool force = false)
        {
            if (!force)
            {
                if (Time.time < textDelay)
                    return; // Slow down the text update

                textDelay = Time.time + TextDelayInterval;
            }

            for (int i = 0; i < reactorRodData.Count; i++)
            {
                SlotData item = reactorRodData[i];

                if (item.InfoDisplay == null)
                    continue;

                if (item.HasPower())
                {
                    if (i < this.MaxActiveSlots)
                    {
                        item.InfoDisplay.text = NumberFormatter.FormatValue(item.Charge);
                        item.InfoDisplay.color = Color.white;
                    }
                    else
                    {
                        item.InfoDisplay.text = CyNukReactorBuildable.InactiveRodMsg();
                        item.InfoDisplay.color = Color.yellow;
                    }
                }
                else
                {
                    item.InfoDisplay.text = CyNukReactorBuildable.NoPoweMessage();
                    item.InfoDisplay.color = Color.red;
                }
            }
        }

        #endregion

        #region Rod Updates

        private void UpdateGraphicalRod(SlotData slotData)
        {
            GameObject graphicalRod = CyNukeRodHelper.Find(this.gameObject, reactorRodData.FindIndex(a => a == slotData));

            if (graphicalRod != null)
            {
                GameObject uranium = graphicalRod.FindChild("PowerRod_Uranium")?.gameObject;

                if (uranium != null)
                {
                    uranium.transform.localPosition = NewPostion(uranium, slotData);
                }
                else
                {
                    MCUServices.Logger.Error($"PowerRod_Uranium not found in GameObject {graphicalRod.name}");
                }
            }
            else
            {
                MCUServices.Logger.Error($"GraphicalRod is null", true);
            }
        }

        private Vector3 NewPostion(GameObject uranium, SlotData slotData)
        {
            if (uranium == null)
                return Vector3.zero;
            float fuelPercentage = slotData.Charge / InitialReactorRodCharge;

            var positon = new Vector3(uranium.transform.localPosition.x, fuelPercentage,
                uranium.transform.localPosition.z);
            return positon;
        }

        private void AddNewRod(float chargeLevel, Pickupable item)
        {
            var slotData = new SlotData(chargeLevel, item);
            reactorRodData.Add(slotData);
            UpdateGraphicalRod(slotData);
            if (chargeLevel < 0 && item.GetTechType() == TechType.ReactorRod)
            {
                MCUServices.Logger.Warning("ReactorRod added with no power");
            }
            else if (chargeLevel > 0 && item.GetTechType() == TechType.DepletedReactorRod)
            {
                MCUServices.Logger.Warning("DepletedReactorRod added with power");
            }
        }

        #endregion

        internal void UpdateUpgradeLevel(int upgradeLevel)
        {
            if (upgradeLevel < 0 || upgradeLevel > MaxUpgradeLevel)
                return;

            if (upgradeLevel == lastKnownUpgradeLevel)
                return;

            if (upgradeLevel > 0)
                ErrorMessage.AddMessage(CyNukReactorBuildable.UpgradedMsg());

            lastKnownUpgradeLevel = upgradeLevel;
        }

        private void OnDestroy()
        {
            if (_manager != null)
                _manager.RemoveBuildable(this);
            else
                CyNukeManager.RemoveReactor(this);

            Cyclops = null;
            this.Manager = null;
        }

        private static InventoryItem SpawnItem(TechType techTypeID)
        {
            var gameObject = GameObject.Instantiate(CraftData.GetPrefabForTechType(techTypeID));

            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
            return new InventoryItem(pickupable);
        }
    }
}
