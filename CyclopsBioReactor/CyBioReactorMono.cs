namespace CyclopsBioReactor
{
    using Common;
    using CyclopsBioReactor.Items;
    using CyclopsBioReactor.Management;
    using CyclopsBioReactor.SaveData;
    using MoreCyclopsUpgrades.API;
    using ProtoBuf;
    using System.Collections.Generic;
    using UnityEngine;

    [ProtoContract]
    internal class CyBioReactorMono : HandTarget, IHandTarget, IProtoEventListener, IProtoTreeEventListener
    {
        internal const float MinimalPowerValue = MCUServices.MinimalPowerValue;

        private const float baselineChargeRate = 0.80f;
        public const int MaxBoosters = 3;

        internal int StorageWidth { get; private set; } = 2;
        internal int StorageHeight { get; private set; } = 2;
        internal int TotalContainerSpaces => this.StorageHeight * this.StorageWidth;

        // Because now each item produces charge in parallel, the charge rate will be variable.
        // At half-full, we get close to original charging rates.
        // When at full capacity, charging rates will nearly double.
        internal float ChargePerSecondPerItem = baselineChargeRate;

        internal const float MaxPowerBaseline = 200;

        private const float TextDelayInterval = 2f;

        [AssertNotNull]
        public ChildObjectIdentifier storageRoot;

        private float textDelay = TextDelayInterval;
        private bool pdaIsOpen = false;
        private bool isLoadingSaveData = false;
        private CyBioReactorSaveData SaveData;

        public SubRoot ParentCyclops;
        internal BioManager Manager;
        public Constructable Buildable;
        public ItemsContainer Container;
        public Battery Battery;
        public string PrefabID;

        public bool IsContructed => (Buildable != null) && Buildable.constructed;

        private int lastKnownBioBooster = 0;

        private BioEnergyCollection MaterialsProcessing { get; } = new BioEnergyCollection();

        // Careful, this map only exists while the PDA screen is open
        public Dictionary<InventoryItem, uGUI_ItemIcon> InventoryMapping { get; private set; }

        public bool ProducingPower => this.IsContructed && this.MaterialsProcessing.Count > 0;
        public bool HasPower => this.IsContructed && Battery._charge > 0f;

        #region Initialization

        private void Start()
        {
            ChargePerSecondPerItem = baselineChargeRate / this.TotalContainerSpaces * 2;

            SubRoot cyclops = GetComponentInParent<SubRoot>();

            if (cyclops is null)
            {
                QuickLogger.Debug("CyBioReactorMono: Could not find Cyclops during Start. Attempting external syncronize.");
                BioManager.SyncAllBioReactors();
            }
            else
            {
                QuickLogger.Debug("CyBioReactorMono: Parent cyclops found!");
                ConnectToCyclops(cyclops);
            }
        }

        public override void Awake()
        {
            base.Awake();

            InitializeConstructible();
            InitializeSaveData();
            InitializeStorageRoot();
            InitializeContainer();
            InitializeBattery();
        }

        private void InitializeContainer()
        {
            if (Container is null)
            {
                Container = new ItemsContainer(this.StorageWidth, this.StorageHeight, storageRoot.transform, CyBioReactor.StorageLabel, null);

                Container.isAllowedToAdd += IsAllowedToAdd;
                Container.isAllowedToRemove += IsAllowedToRemove;

                (Container as IItemsContainer).onAddItem += OnAddItem;
                (Container as IItemsContainer).onRemoveItem += OnRemoveItem;
            }
        }

        private void InitializeSaveData()
        {
            if (SaveData is null)
            {
                PrefabID = GetComponentInParent<PrefabIdentifier>().Id;
                SaveData = new CyBioReactorSaveData(PrefabID);
            }
        }

        private void InitializeBattery()
        {
            if (Battery is null)
            {
                Battery = GetComponent<Battery>();

                if (Battery is null)
                {
                    QuickLogger.Debug("Initialized Battery component", true);
                    Battery = new Battery(); // Failsafe
                }
                else
                {
                    QuickLogger.Debug("Battery component ready", true);
                }

                Battery._capacity = MaxPowerBaseline;
                Battery._charge = 0; // Starts empty
            }
        }

        private void InitializeStorageRoot()
        {
            if (storageRoot is null)
            {
                var storeRoot = new GameObject("StorageRoot");
                storeRoot.transform.SetParent(this.transform, false);
                storageRoot = storeRoot.AddComponent<ChildObjectIdentifier>();
            }
        }

        private void InitializeConstructible()
        {
            if (Buildable is null)
            {
                Buildable = this.gameObject.GetComponent<Constructable>();
            }
        }

        #endregion

        private void Update() // The all important Update method
        {
            if (this.ProducingPower)
            {
                float powerDeficit = Battery._capacity - Battery._charge;

                if (powerDeficit > 0.001f)
                {
                    float chargeOverTime = ChargePerSecondPerItem * DayNightCycle.main.deltaTime;

                    float powerProduced = ProducePower(Mathf.Min(powerDeficit, chargeOverTime));

                    Battery._charge += powerProduced;
                }
            }

            if (pdaIsOpen)
                UpdateDisplayText();
        }

        #region Player interaction

        public void OnHandHover(GUIHand guiHand)
        {
            if (!Buildable.constructed)
                return;

            HandReticle main = HandReticle.main;
            main.SetInteractText(CyBioReactor.OnHoverFormatString(Mathf.FloorToInt(Battery._charge), Battery._capacity, (this.MaterialsProcessing.Count > 0 ? "+" : "")));
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        public void OnHandClick(GUIHand guiHand)
        {
            PDA pda = Player.main.GetPDA();
            Inventory.main.SetUsedStorage(Container);
            pda.Open(PDATab.Inventory, null, new PDA.OnClose(CyOnPdaClose), 4f);

            pdaIsOpen = true;
        }

        internal void CyOnPdaClose(PDA pda)
        {
            this.InventoryMapping = null;

            foreach (BioEnergy item in this.MaterialsProcessing)
            {
                item.DisplayText = null;
            }

            pdaIsOpen = false;

            (Container as IItemsContainer).onAddItem -= OnAddItemLate;
        }

        private void OnAddItem(InventoryItem item)
        {
            item.isEnabled = false;

            if (isLoadingSaveData)
            {
                return;
            }

            if (BaseBioReactor.charge.TryGetValue(item.item.GetTechType(), out float bioEnergyValue) && bioEnergyValue > 0f)
            {
                var bioenergy = new BioEnergy(item.item, bioEnergyValue, bioEnergyValue)
                {
                    Size = item.width * item.height
                };

                this.MaterialsProcessing.Add(bioenergy);
            }
            else
            {
                Destroy(item.item.gameObject); // Failsafe
            }
        }

        private void OnAddItemLate(InventoryItem item)
        {
            if (this.InventoryMapping is null)
                return;

            if (this.InventoryMapping.TryGetValue(item, out uGUI_ItemIcon icon))
            {
                BioEnergy bioEnergy = this.MaterialsProcessing.Find(item.item);

                if (bioEnergy is null)
                {
                    QuickLogger.Debug("Matching pickable in bioreactor not found", true);
                    return;
                }

                bioEnergy.AddDisplayText(icon);
            }
        }

        private void OnRemoveItem(InventoryItem item)
        {
            // Don't need to do anything
        }

        private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
            if (isLoadingSaveData)
                return true;

            if (pickupable != null)
            {
                TechType techType = pickupable.GetTechType();

                if (BaseBioReactor.charge.ContainsKey(techType))
                    return true;
            }

            if (verbose)
                ErrorMessage.AddMessage(Language.main.Get("BaseBioReactorCantAddItem"));

            return false;
        }

        private bool IsAllowedToRemove(Pickupable pickupable, bool verbose)
        {
            return false;
        }

        #endregion

        private float ProducePower(float powerDrawnPerItem)
        {
            float powerProduced = 0f;

            if (powerDrawnPerItem > 0f && // More than zero energy being produced per item per time delta
                this.MaterialsProcessing.Count > 0) // There should be materials in the reactor to process
            {
                foreach (BioEnergy material in this.MaterialsProcessing)
                {
                    float availablePowerPerItem = Mathf.Min(material.RemainingEnergy, material.Size * powerDrawnPerItem);

                    material.RemainingEnergy -= availablePowerPerItem;
                    powerProduced += availablePowerPerItem;

                    if (material.FullyConsumed)
                        this.MaterialsProcessing.StageForRemoval(material);
                }
            }

            this.MaterialsProcessing.ClearAllStagedForRemoval(Container);

            return powerProduced;
        }

        public float GetBatteryPower(float drainingRate, float requestedAmount)
        {
            if (requestedAmount < MinimalPowerValue) // No power deficit left to charge
                return 0f; // Exit

            if (!this.HasPower)
                return 0f;

            // Mathf.Min is to prevent accidentally taking too much power from the battery
            float chargeAmt = Mathf.Min(requestedAmount, drainingRate);

            if (Battery._charge > chargeAmt)
            {
                Battery._charge -= chargeAmt;
            }
            else // Battery about to be fully drained
            {
                chargeAmt = Battery._charge; // Take what's left
                Battery._charge = 0f; // Set battery to empty
            }

            return chargeAmt;
        }

        private void UpdateDisplayText()
        {
            if (Time.time < textDelay)
                return; // Slow down the text update

            textDelay = Time.time + TextDelayInterval;

            foreach (BioEnergy material in this.MaterialsProcessing)
                material.UpdateInventoryText();
        }

        #region Save data handling

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            SaveData.ReactorBatterCharge = Battery._charge;
            SaveData.SaveMaterialsProcessing(this.MaterialsProcessing);
            SaveData.BoosterCount = lastKnownBioBooster;

            SaveData.Save();
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            isLoadingSaveData = true;

            InitializeBattery();
            InitializeStorageRoot();

            Container.Clear(false);

            isLoadingSaveData = false;
        }

        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
        }

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            isLoadingSaveData = true;

            bool hasSaveData = SaveData.Load();

            if (hasSaveData)
            {
                Container.Clear(false);

                UpdateBoosterCount(SaveData.BoosterCount);

                Battery._charge = Mathf.Min(Battery._capacity, SaveData.ReactorBatterCharge);

                List<BioEnergy> savedMaterials = SaveData.GetMaterialsInProcessing();
                QuickLogger.Debug($"Found {savedMaterials.Count} materials in save data");

                foreach (BioEnergy material in savedMaterials)
                {
                    QuickLogger.Debug($"Adding {material.Pickupable.GetTechName()} to container from save data");
                    this.MaterialsProcessing.Add(material, Container);
                }
            }

            isLoadingSaveData = false;
        }

        #endregion 

        public void ConnectToCyclops(SubRoot parentCyclops, BioManager manager = null)
        {
            if (ParentCyclops != null)
                return;

            ParentCyclops = parentCyclops;
            this.transform.SetParent(parentCyclops.transform);
            Manager = manager ?? MCUServices.Client.GetManager<BioManager>(parentCyclops, BioManager.ManagerName);

            if (!Manager.CyBioReactors.Contains(this))
            {
                Manager.CyBioReactors.Add(this);
            }

            UpdateBoosterCount(Manager.TotalBoosters);
            QuickLogger.Debug("Bioreactor has been connected to Cyclops", true);
        }

        public void ConnectToInventory(Dictionary<InventoryItem, uGUI_ItemIcon> lookup)
        {
            this.InventoryMapping = lookup;

            (Container as IItemsContainer).onAddItem += OnAddItemLate;

            if (this.MaterialsProcessing.Count == 0)
                return;

            foreach (KeyValuePair<InventoryItem, uGUI_ItemIcon> pair in lookup)
            {
                InventoryItem item = pair.Key;
                uGUI_ItemIcon icon = pair.Value;

                BioEnergy bioEnergy = this.MaterialsProcessing.Find(item.item);

                if (bioEnergy is null)
                {
                    QuickLogger.Debug("Matching pickable in bioreactor not found", true);
                    continue;
                }

                bioEnergy.AddDisplayText(icon);
            }
        }

        public bool HasRoomToShrink()
        {
            var nextStats = ReactorStats.GetStatsForBoosterCount(lastKnownBioBooster - 1);

            return nextStats.TotalSpaces >= this.MaterialsProcessing.SpacesOccupied;
        }

        public bool UpdateBoosterCount(int boosterCount)
        {
            if (boosterCount > MaxBoosters)
                return false;

            if (lastKnownBioBooster == boosterCount)
                return false;

            var nextStats = ReactorStats.GetStatsForBoosterCount(boosterCount);

            Battery._capacity = nextStats.Capacity;

            if (!isLoadingSaveData)
            {
                Battery._charge = Mathf.Min(Battery._charge, Battery._capacity);

                if (lastKnownBioBooster > boosterCount) // Getting smaller
                {
                    int nextAvailableSpace = nextStats.TotalSpaces;
                    while (this.MaterialsProcessing.SpacesOccupied > nextAvailableSpace)
                    {
                        BioEnergy material = this.MaterialsProcessing.GetCandidateForRemoval();

                        if (material == null)
                            break;

                        QuickLogger.Debug($"Removing material of size {material.Size}", true);
                        this.MaterialsProcessing.Remove(material, Container);
                    }
                }
            }

            Container.Resize(this.StorageWidth = nextStats.Width, this.StorageHeight = nextStats.Height);
            Container.Sort();

            ChargePerSecondPerItem = baselineChargeRate / this.TotalContainerSpaces * 2;

            lastKnownBioBooster = boosterCount;

            return true;
        }

        private void OnDestroy()
        {
            if (Manager != null)
                Manager.CyBioReactors.Remove(this);
            else
                BioManager.RemoveReactor(this);

            ParentCyclops = null;
            Manager = null;
        }

        private class ReactorStats
        {
            internal readonly int Width;
            internal readonly int Height;
            internal readonly float Capacity;

            internal int TotalSpaces => Width * Height;

            private ReactorStats(int width, int height, float capacity)
            {
                Width = width;
                Height = height;
                Capacity = capacity;
            }

            internal static ReactorStats GetStatsForBoosterCount(int boosterCount)
            {
                switch (boosterCount)
                {
                    default:
                        return new ReactorStats(2, 2, MaxPowerBaseline); // 4 slots
                    case 1:
                        return new ReactorStats(3, 2, MaxPowerBaseline + 50f); // 6 slots
                    case 2:
                        return new ReactorStats(3, 3, MaxPowerBaseline + 100f); // 9 slots
                    case 3: // MaxBoosters
                        return new ReactorStats(6, 2, MaxPowerBaseline + 150f); // 12 slots
                }
            }
        }
    }
}
