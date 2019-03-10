namespace MoreCyclopsUpgrades.Monobehaviors
{
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.Caching;
    using MoreCyclopsUpgrades.Managers;
    using MoreCyclopsUpgrades.SaveData;
    using ProtoBuf;
    using UnityEngine;

    [ProtoContract]
    internal class CyBioReactorMono : HandTarget, IHandTarget, IProtoEventListener, IProtoTreeEventListener, ISubRootConnection
    {
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
        internal const string StorageLabel = "Cyclops Bioreactor Materials";

        private const int TextDelayTicks = 60;

        [AssertNotNull]
        public ChildObjectIdentifier storageRoot;

        private int textDelay = TextDelayTicks;
        private bool pdaIsOpen = false;
        private bool isLoadingSaveData = false;
        private CyBioReactorSaveData SaveData;

        public SubRoot ParentCyclops { get; private set; }
        internal CyclopsManager Manager { get; private set; }
        public Constructable Buildable { get; private set; }
        public ItemsContainer Container { get; private set; }
        public Battery Battery { get; internal set; }
        public string PrefabID { get; private set; }
        public bool IsContructed => (this.Buildable != null) && this.Buildable.constructed;

        private int lastKnownBioBooster = 0;

        private BioEnergyCollection MaterialsProcessing { get; } = new BioEnergyCollection();

        // Careful, this map only exists while the PDA screen is open
        public Dictionary<InventoryItem, uGUI_ItemIcon> InventoryMapping { get; private set; }

        public bool ProducingPower => this.IsContructed && this.MaterialsProcessing.Count > 0;
        public bool HasPower => this.IsContructed && this.Battery.charge > 0f;

        #region Initialization

        private void Start()
        {
            ChargePerSecondPerItem = baselineChargeRate / this.TotalContainerSpaces * 2;

            SubRoot cyclops = GetComponentInParent<SubRoot>();

            if (cyclops is null)
            {
                QuickLogger.Debug("CyBioReactorMono: Could not find Cyclops during Start. Attempting external syncronize.");
                CyclopsManager.SyncBioReactors();
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
            if (this.Container is null)
            {
                this.Container = new ItemsContainer(this.StorageWidth, this.StorageHeight, storageRoot.transform, StorageLabel, null);

                this.Container.isAllowedToAdd += IsAllowedToAdd;
                this.Container.isAllowedToRemove += IsAllowedToRemove;

                (this.Container as IItemsContainer).onAddItem += OnAddItem;
                (this.Container as IItemsContainer).onRemoveItem += OnRemoveItem;
            }
        }

        private void InitializeSaveData()
        {
            if (SaveData is null)
            {
                this.PrefabID = GetComponentInParent<PrefabIdentifier>().Id;
                SaveData = new CyBioReactorSaveData(this.PrefabID);
            }
        }

        private void InitializeBattery()
        {
            if (this.Battery is null)
            {
                this.Battery = GetComponent<Battery>();

                if (this.Battery is null)
                {
                    QuickLogger.Debug("Battery was still null", true);
                    this.Battery = new Battery(); // Failsafe
                }
                else
                {
                    QuickLogger.Debug("Battery was found", true);
                }

                this.Battery._capacity = MaxPowerBaseline;
                this.Battery._charge = 0; // Starts empty
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
            if (this.Buildable is null)
            {
                this.Buildable = this.gameObject.GetComponent<Constructable>();
            }
        }

        #endregion

        private void Update() // The all important Update method
        {
            if (this.ProducingPower)
            {
                float powerDeficit = this.Battery.capacity - this.Battery.charge;

                if (powerDeficit > 0.001f)
                {
                    float chargeOverTime = ChargePerSecondPerItem * DayNightCycle.main.deltaTime;

                    float powerProduced = ProducePower(Mathf.Min(powerDeficit, chargeOverTime));

                    this.Battery.charge += powerProduced;
                }
            }

            if (pdaIsOpen)
                UpdateDisplayText();
        }

        #region Player interaction

        public void OnHandHover(GUIHand guiHand)
        {
            if (!this.Buildable.constructed)
                return;

            HandReticle main = HandReticle.main;
            main.SetInteractText($"Use Cyclops Bioreactor {Mathf.FloorToInt(this.Battery.charge)}/{this.Battery.capacity}{(this.MaterialsProcessing.Count > 0 ? "+" : "")}");
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        public void OnHandClick(GUIHand guiHand)
        {
            PDA pda = Player.main.GetPDA();
            Inventory.main.SetUsedStorage(this.Container);
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

            (this.Container as IItemsContainer).onAddItem -= OnAddItemLate;
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

            bool canAdd = false;
            if (pickupable != null)
            {
                TechType techType = pickupable.GetTechType();

                if (BaseBioReactor.charge.ContainsKey(techType))
                {
                    canAdd = true;
                }
            }

            if (!canAdd && verbose)
            {
                ErrorMessage.AddMessage(Language.main.Get("BaseBioReactorCantAddItem"));
            }

            return canAdd;
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

            this.MaterialsProcessing.ClearAllStagedForRemoval(this.Container);

            return powerProduced;
        }

        private void UpdateDisplayText()
        {
            if (textDelay-- > 0)
                return; // Slow down the text update

            textDelay = TextDelayTicks;

            foreach (BioEnergy material in this.MaterialsProcessing)
                material.UpdateInventoryText();
        }

        #region Save data handling

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            SaveData.ReactorBatterCharge = this.Battery.charge;
            SaveData.SaveMaterialsProcessing(this.MaterialsProcessing);
            SaveData.BoosterCount = lastKnownBioBooster;

            SaveData.Save();
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            isLoadingSaveData = true;

            InitializeBattery();
            InitializeStorageRoot();

            this.Container.Clear(false);

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
                this.Container.Clear();

                UpdateBoosterCount(SaveData.BoosterCount);
                this.Battery.charge = SaveData.ReactorBatterCharge;

                List<BioEnergy> materials = SaveData.GetMaterialsInProcessing();

                foreach (BioEnergy material in materials)
                {
                    InventoryItem inventoryItem = this.Container.AddItem(material.Pickupable);
                    material.Size = inventoryItem.width * inventoryItem.height;
                    this.MaterialsProcessing.Add(material);
                }
            }

            isLoadingSaveData = false;
        }

        #endregion 

        public void ConnectToCyclops(SubRoot parentCyclops)
        {
            if (this.ParentCyclops != null)
                return;

            this.ParentCyclops = parentCyclops;
            this.transform.SetParent(parentCyclops.transform);
            this.Manager = CyclopsManager.GetAllManagers(parentCyclops);

            if (!this.Manager.PowerManager.CyBioReactors.Contains(this))
            {
                this.Manager.PowerManager.CyBioReactors.Add(this);
            }

            QuickLogger.Debug("Bioreactor has been connected to Cyclops", true);
        }

        public void ConnectToInventory(Dictionary<InventoryItem, uGUI_ItemIcon> lookup)
        {
            this.InventoryMapping = lookup;

            (this.Container as IItemsContainer).onAddItem += OnAddItemLate;

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

        public bool UpdateBoosterCount(int boosterCount)
        {
            if (boosterCount > MaxBoosters)
            {
                return false;
            }

            if (lastKnownBioBooster == boosterCount)
                return false;

            int nextWidth;
            int nextHeight;
            float nextCapacity;
            switch (boosterCount)
            {
                default:
                    nextWidth = 2;
                    nextHeight = 2;
                    nextCapacity = MaxPowerBaseline;
                    break;
                case 1:
                    nextWidth = 3;
                    nextHeight = 2;
                    nextCapacity = MaxPowerBaseline + 50;
                    break;
                case 2:
                    nextWidth = 3;
                    nextHeight = 3;
                    nextCapacity = MaxPowerBaseline + 100;
                    break;
                case 3: // MaxBoosters
                    nextWidth = 3;
                    nextHeight = 3;
                    nextCapacity = MaxPowerBaseline + 200;
                    break;
            }

            this.Battery._capacity = nextCapacity;
            this.Battery._charge = Mathf.Min(this.Battery.charge, this.Battery.capacity);

            if (lastKnownBioBooster > boosterCount) // Getting smaller
            {
                int nextAvailableSpace = nextWidth * nextHeight;
                while (this.MaterialsProcessing.SpacesOccupied > nextAvailableSpace)
                {
                    BioEnergy material = this.MaterialsProcessing.GetCandidateForRemoval();

                    if (material is null)
                        break;

                    QuickLogger.Debug($"Removing material of size {material.Size}", true);
                    this.MaterialsProcessing.Remove(material, this.Container);
                }
            }

            this.Container.Resize(this.StorageWidth = nextWidth, this.StorageHeight = nextHeight);
            this.Container.Sort();
            this.Container.unsorted = false;

            ChargePerSecondPerItem = baselineChargeRate / this.TotalContainerSpaces * 2;

            lastKnownBioBooster = boosterCount;

            return true;
        }
    }
}