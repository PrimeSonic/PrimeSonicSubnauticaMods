namespace CyclopsBioReactor
{
    using System.Collections.Generic;
    using CyclopsBioReactor.Items;
    using CyclopsBioReactor.Management;
    using CyclopsBioReactor.SaveData;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Buildables;
    using ProtoBuf;
    using UnityEngine;

    [ProtoContract]
    internal class CyBioReactorMono : HandTarget, IHandTarget, IProtoEventListener, ICyclopsBuildable
    {
        internal static bool PdaIsOpen = false;
        internal static CyBioReactorMono OpenInPda = null;

        public const int MaxBoosters = 3;
        private const float MaxPowerBaseline = 200;
        private const float TextDelayInterval = 1.5f;

        private const float baselineChargeRate = 0.765f;

        private const int StorageWidth = 4;
        private const int StorageHeight = 4;
        private const float TotalContainerSpaces = StorageHeight * StorageWidth;

        // Because now each item produces charge in parallel, the charge rate will be variable.
        // At half-full, we get close to original charging rates.
        // When at full capacity, charging rates will nearly double.
        private float chargeRate = baselineChargeRate / 6f;

        private BioAuxCyclopsManager _manager = null;
        private BioAuxCyclopsManager Manager
        {
            get => _manager ?? (_manager = MCUServices.Find.AuxCyclopsManager<BioAuxCyclopsManager>(Cyclops));
            set => _manager = value;
        }

        private BioBoosterUpgradeHandler upgradeHandler;
        private BioBoosterUpgradeHandler UpgradeHandler => upgradeHandler ?? (upgradeHandler = MCUServices.Find.CyclopsUpgradeHandler<BioBoosterUpgradeHandler>(Cyclops, BioReactorBooster.BoosterTechType));

        [AssertNotNull]
        private ChildObjectIdentifier storageRoot;
        private ItemsContainer container;
        private Constructable _buildable;

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

        private string prefabId;
        private float textDelay = TextDelayInterval;
        private bool isLoadingSaveData = false;
        private bool isDrainingEnergy = false;
        private CyBioReactorSaveData _saveData;
        private SubRoot Cyclops;
        private int lastKnownBioBooster = 0;
        private CyBioReactorDisplayHandler displayHandler;
        private CyBioReactorAudioHandler audioHandler;

        private readonly BioEnergyCollection bioMaterialsProcessing = new BioEnergyCollection();

        // Careful, this map only exists while the PDA screen is open
        private Dictionary<InventoryItem, uGUI_ItemIcon> pdaInventoryMapping = null;

        public bool IsConnectedToCyclops => Cyclops != null && _manager != null;
        public bool ProducingPower => this.IsContructed && bioMaterialsProcessing.Count > 0;
        public bool HasPower => this.IsContructed && this.Charge > 0f;
        public bool IsContructed => (this.Buildable != null) && this.Buildable.constructed;

        public float Charge { get; private set; }
        public float Capacity { get; private set; } = MaxPowerBaseline;
        public int ProcessingCapacity { get; private set; } = 4;

        #region Initialization

        private void Start()
        {
            SubRoot cyclops = GetComponentInParent<SubRoot>();

            if (cyclops != null)
            {
                MCUServices.Logger.Debug("CyBioReactorMono: Parent cyclops found!");
                ConnectToCyclops(cyclops);
            }
            else
            {
                InvokeRepeating(nameof(GetMCUHandler), 3f, 1f);
            }
        }

        internal int MixingStateHash { get; private set; }

        internal int DoorStateHash { get; private set; }

        internal int ScreenStateHash { get; private set; }

        internal CyBioReactorAnimationHandler AnimationHandler { get; private set; }

        public override void Awake()
        {
            base.Awake();

            if (_saveData == null)
                ReadySaveData();

            InitializeStorageRoot();
            InitializeContainer();
            InitializeAnimations();
        }

        private void InitializeAnimations()
        {
            this.ScreenStateHash = Animator.StringToHash("ScreenState");
            this.DoorStateHash = Animator.StringToHash("DoorState");
            this.MixingStateHash = Animator.StringToHash("MixingState");

            this.AnimationHandler = new CyBioReactorAnimationHandler(this);
            displayHandler = new CyBioReactorDisplayHandler(this);

            CyBioreactorTrigger trigger = this.gameObject.FindChild("Trigger").AddComponent<CyBioreactorTrigger>();
            trigger.OnPlayerEnter += () =>
            {
                this.AnimationHandler?.SetIntHash(this.DoorStateHash, 1);
                audioHandler.PlayDoorSoundClip(true);
            };
            trigger.OnPlayerExit += () =>
            {
                this.AnimationHandler?.SetIntHash(this.DoorStateHash, 2);
                audioHandler.PlayDoorSoundClip(true);
            };

            audioHandler = new CyBioReactorAudioHandler(this.transform);
            audioHandler.SetSoundActive(true);

            displayHandler.TurnOnDisplay();
        }

        private void InitializeContainer()
        {
            if (container != null)
                return;

            container = new ItemsContainer(StorageWidth, StorageHeight, storageRoot.transform, CyBioReactor.StorageLabel, null);

            container.isAllowedToAdd += (Pickupable pickupable, bool verbose) =>
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
            };

            container.isAllowedToRemove += (Pickupable pickupable, bool verbose) => false;

            (container as IItemsContainer).onAddItem += (InventoryItem item) =>
            {
                item.isEnabled = false;

                if (isLoadingSaveData)
                    return;

                if (BaseBioReactor.charge.TryGetValue(item.item.GetTechType(), out float bioEnergyValue) && bioEnergyValue > 0f)
                {
                    var bioenergy = new BioEnergy(item.item, bioEnergyValue, bioEnergyValue)
                    {
                        Size = item.width * item.height
                    };

                    bioMaterialsProcessing.Add(bioenergy);
                }
                else
                {
                    Destroy(item.item.gameObject); // Failsafe
                }
            };
        }

        private void ReadySaveData()
        {
            if (prefabId == null)
            {
                PrefabIdentifier prefabIdentifier = GetComponentInParent<PrefabIdentifier>() ?? GetComponent<PrefabIdentifier>();
                prefabId = prefabIdentifier.id;
            }

            if (prefabId != null && _saveData == null)
            {
                MCUServices.Logger.Debug($"CyBioReactorMono PrefabIdentifier {prefabId}");
                _saveData = new CyBioReactorSaveData(prefabId);
            }
        }

        private void InitializeStorageRoot()
        {
            if (storageRoot != null)
                return;

            var storeRoot = new GameObject("StorageRoot");
            storeRoot.transform.SetParent(this.transform, false);
            storageRoot = storeRoot.AddComponent<ChildObjectIdentifier>();
        }

        private void GetMCUHandler()
        {
            Cyclops = Cyclops ?? GetComponentInParent<SubRoot>();

            if (Cyclops == null)
            {
                MCUServices.Logger.Debug("Could not find Cyclops during Start. Attempting external synchronize.");
                BioAuxCyclopsManager.SyncAllBioReactors();
            }
            else if (this.Manager == null)
            {
                MCUServices.Logger.Debug("Looking for BioAuxCyclopsManager.");
                this.Manager = MCUServices.Find.AuxCyclopsManager<BioAuxCyclopsManager>(Cyclops);
            }

            if (this.Manager != null)
                CancelInvoke(nameof(GetMCUHandler));
        }

        #endregion

        private void Update() // The all important Update method
        {
            if (this.ProducingPower)
            {
                if (this.Charge > this.Capacity)
                {
                    // Overcharged
                    this.Charge = Mathf.Min(this.Charge - chargeRate * DayNightCycle.main.deltaTime, this.Charge);
                }
                else // Normal operation
                {
                    float powerDeficit = this.Capacity - this.Charge;

                    if (powerDeficit > MCUServices.MinimalPowerValue)
                    {
                        float chargeOverTime = chargeRate * DayNightCycle.main.deltaTime;
                        float powerDrawnPerItem = Mathf.Min(powerDeficit, chargeOverTime);

                        float powerProduced = ProducePower(powerDrawnPerItem);

                        this.Charge = Mathf.Min(this.Charge + powerProduced, this.Capacity);
                    }
                }
            }

            if (PdaIsOpen)
                UpdateDisplayText();

            UpdateReactorSystems();
        }

        private void UpdateReactorSystems()
        {
            if (this.AnimationHandler == null || displayHandler == null)
                return;

            if (this.ProducingPower)
                displayHandler.SetActive(Mathf.RoundToInt(this.Charge), Mathf.CeilToInt(this.Capacity), isDrainingEnergy);
            else if (this.HasPower)
                displayHandler.SetInActivating(Mathf.RoundToInt(this.Charge), Mathf.CeilToInt(this.Capacity), isDrainingEnergy);
            else
                displayHandler.SetInactive();

            isDrainingEnergy = false; // Reset this after using it

            bool isOperating = this.ProducingPower && this.HasPower;

            if (this.AnimationHandler.GetBoolHash(this.MixingStateHash) != isOperating)
                this.AnimationHandler.SetBoolHash(this.MixingStateHash, isOperating);
        }

        #region Player interaction

        public void OnHandHover(GUIHand guiHand)
        {
            if (!this.IsContructed)
                return;

            HandReticle main = HandReticle.main;

            main.SetInteractText($"{Language.main.GetFormat("UseBaseBioReactor", Mathf.RoundToInt(this.Charge), Mathf.RoundToInt(this.Capacity))}{(bioMaterialsProcessing.Count > 0 ? "+" : "")}");
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        public void OnHandClick(GUIHand guiHand)
        {
            if (!this.IsContructed)
                return;

            PdaIsOpen = true;
            OpenInPda = this;

            PDA pda = Player.main.GetPDA();
            Inventory.main.SetUsedStorage(container);
            pda.Open(PDATab.Inventory, null, new PDA.OnClose(CyOnPdaClose), 4f);
        }

        internal void CyOnPdaClose(PDA pda)
        {
            pdaInventoryMapping = null;

            for (int m = 0; m < bioMaterialsProcessing.Count; m++)
                bioMaterialsProcessing[m].DisplayText = null;

            PdaIsOpen = false;
            OpenInPda = null;

            (container as IItemsContainer).onAddItem -= OnAddItemLate;
        }

        private void OnAddItemLate(InventoryItem item)
        {
            if (pdaInventoryMapping == null)
                return;

            if (pdaInventoryMapping.TryGetValue(item, out uGUI_ItemIcon icon))
            {
                BioEnergy bioEnergy = bioMaterialsProcessing.Find(item.item);

                if (bioEnergy == null)
                {
                    MCUServices.Logger.Debug("Matching pickable in bioreactor not found", true);
                    return;
                }

                bioEnergy.AddDisplayText(icon);
            }
        }

        #endregion

        private float ProducePower(float powerDrawnPerItem)
        {
            float powerProduced = 0f;

            if (powerDrawnPerItem > 0f && // More than zero energy being produced per item per time delta
                bioMaterialsProcessing.Count > 0) // There should be materials in the reactor to process
            {
                int currentProcessingCapacity = this.ProcessingCapacity;
                int m = 0;
                while (m < bioMaterialsProcessing.Count && currentProcessingCapacity > 0)
                {
                    BioEnergy material = bioMaterialsProcessing[m];
                    m++;

                    if (material.Size > currentProcessingCapacity)
                        continue;

                    currentProcessingCapacity -= material.Size;
                    float availablePowerPerItem = Mathf.Min(material.RemainingEnergy, material.Size * powerDrawnPerItem);

                    material.RemainingEnergy -= availablePowerPerItem;
                    powerProduced += availablePowerPerItem;

                    if (material.FullyConsumed)
                        bioMaterialsProcessing.StageForRemoval(material);
                }
            }

            bioMaterialsProcessing.ClearAllStagedForRemoval(container);

            return powerProduced;
        }

        public float GetBatteryPower(float drainingRate, float requestedAmount)
        {
            if (requestedAmount < MCUServices.MinimalPowerValue || !this.HasPower)
                return 0f;

            // Mathf.Min is to prevent accidentally taking too much power from the battery
            float amtToDrain = Mathf.Min(requestedAmount, drainingRate * DayNightCycle.main.deltaTime);

            if (this.Charge > amtToDrain)
            {
                this.Charge -= amtToDrain;
            }
            else // Battery about to be fully drained
            {
                amtToDrain = this.Charge; // Take what's left
                this.Charge = 0f; // Set battery to empty
            }

            isDrainingEnergy = true;
            return amtToDrain;
        }

        private void UpdateDisplayText()
        {
            if (Time.time < textDelay)
                return; // Slow down the text update

            textDelay = Time.time + TextDelayInterval;

            for (int m = 0; m < bioMaterialsProcessing.Count; m++)
                bioMaterialsProcessing[m].UpdateInventoryText();
        }

        #region Save data handling

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            if (_saveData == null)
                ReadySaveData();

            _saveData.ReactorBatterCharge = this.Charge;
            _saveData.SaveMaterialsProcessing(bioMaterialsProcessing);
            _saveData.BoosterCount = lastKnownBioBooster;

            _saveData.Save();
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            if (_saveData == null)
                ReadySaveData();

            InitializeStorageRoot();

            InitializeContainer();

            MCUServices.Logger.Debug("Checking save data");

            isLoadingSaveData = true;

            if (_saveData != null && _saveData.Load())
            {
                MCUServices.Logger.Debug("Save data found");

                container.Clear(false);
                bioMaterialsProcessing.Clear();

                MCUServices.Logger.Debug($"Setting up Boosters at {_saveData.BoosterCount} from save data");
                UpdateBoosterCount(_saveData.BoosterCount);

                MCUServices.Logger.Debug($"Restoring {_saveData.ReactorBatterCharge} energy from save data");
                this.Charge = Mathf.Min(this.Capacity, _saveData.ReactorBatterCharge);

                List<BioEnergy> savedMaterials = _saveData.GetMaterialsInProcessing();
                MCUServices.Logger.Debug($"Found {savedMaterials.Count} materials in save data");

                for (int i = 0; i < savedMaterials.Count; i++)
                {
                    BioEnergy material = savedMaterials[i];
                    MCUServices.Logger.Debug($"Adding {material.Pickupable.GetTechName()} to container from save data");
                    bioMaterialsProcessing.Add(material, container);
                }

                MCUServices.Logger.Debug($"Added {savedMaterials.Count} items from save data");
            }
            else
            {
                MCUServices.Logger.Debug("No save data found");
            }

            isLoadingSaveData = false;
        }

        #endregion 

        public void ConnectToCyclops(SubRoot parentCyclops, BioAuxCyclopsManager manager = null)
        {
            if (this.IsConnectedToCyclops)
                return;

            Cyclops = parentCyclops;
            this.transform.SetParent(parentCyclops.transform);

            if (this.Manager == null && manager != null)
                this.Manager = manager;

            if (this.Manager != null)
                this.Manager.AddBuildable(this);

            if (this.UpgradeHandler != null)
                UpdateBoosterCount(this.UpgradeHandler.Count);
        }

        public void ConnectToContainer(Dictionary<InventoryItem, uGUI_ItemIcon> lookup)
        {
            pdaInventoryMapping = lookup;

            (container as IItemsContainer).onAddItem += OnAddItemLate;

            if (bioMaterialsProcessing.Count == 0)
                return;

            foreach (KeyValuePair<InventoryItem, uGUI_ItemIcon> pair in lookup)
            {
                InventoryItem item = pair.Key;
                uGUI_ItemIcon icon = pair.Value;

                BioEnergy bioEnergy = bioMaterialsProcessing.Find(item.item);

                if (bioEnergy == null)
                {
                    MCUServices.Logger.Debug("Matching pickable in bioreactor not found", true);
                    continue;
                }

                bioEnergy.AddDisplayText(icon);
            }
        }

        public void UpdateBoosterCount(int boosterCount)
        {
            if (boosterCount > MaxBoosters)
                return;

            if (lastKnownBioBooster == boosterCount)
                return;

            var nextStats = ReactorStats.GetStatsForBoosterCount(boosterCount);

            this.Capacity = nextStats.Capacity;
            this.ProcessingCapacity = nextStats.ProcessingCapacity;

            chargeRate = baselineChargeRate / nextStats.ProcessingCapacity * 2f;

            lastKnownBioBooster = boosterCount;
        }

        private void OnDestroy()
        {
            if (_manager != null)
                _manager.RemoveBuildable(this);
            else
                BioAuxCyclopsManager.RemoveReactor(this);

            Cyclops = null;
            this.Manager = null;
        }

        private class ReactorStats
        {
            private static readonly ReactorStats boost0 = new ReactorStats(4, MaxPowerBaseline);
            private static readonly ReactorStats boost1 = new ReactorStats(7, MaxPowerBaseline + 100f);
            private static readonly ReactorStats boost2 = new ReactorStats(10, MaxPowerBaseline + 175f);
            private static readonly ReactorStats boost3 = new ReactorStats(13, MaxPowerBaseline + 250f);

            internal readonly int ProcessingCapacity;
            internal readonly float Capacity;

            private ReactorStats(int processingCapacity, float capacity)
            {
                Capacity = capacity;
                ProcessingCapacity = processingCapacity;
            }

            internal static ReactorStats GetStatsForBoosterCount(int boosterCount)
            {
                switch (boosterCount)
                {
                    default:
                        return boost0; // 4 slots
                    case 1:
                        return boost1; // 6 slots
                    case 2:
                        return boost2; // 9 slots
                    case 3: // MaxBoosters
                        return boost3; // 12 slots
                }
            }
        }
    }
}
