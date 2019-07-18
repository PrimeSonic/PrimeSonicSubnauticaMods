namespace CyclopsBioReactor
{
    using System.Collections.Generic;
    using Common;
    using CommonCyclopsBuildables;
    using CyclopsBioReactor.Items;
    using CyclopsBioReactor.Management;
    using CyclopsBioReactor.SaveData;
    using MoreCyclopsUpgrades.API;
    using ProtoBuf;
    using UnityEngine;

    [ProtoContract]
    internal class CyBioReactorMono : HandTarget, IHandTarget, IProtoEventListener, IProtoTreeEventListener, IConstructable, ICyclopsBuildable
    {
        internal static bool PdaIsOpen = false;
        internal static CyBioReactorMono OpenInPda = null;

        public const int MaxBoosters = 3;
        private const float MaxPowerBaseline = 200;
        private const float TextDelayInterval = 2f;

        private const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        private const float baselineChargeRate = 0.75f;

        private int storageWidth = 2;
        private int storageHeight = 2;
        private int TotalContainerSpaces => storageHeight * storageWidth;

        // Because now each item produces charge in parallel, the charge rate will be variable.
        // At half-full, we get close to original charging rates.
        // When at full capacity, charging rates will nearly double.
        private float chargeRate = baselineChargeRate;

        private BioAuxCyclopsManager manager;
        private BioAuxCyclopsManager Manager
        {
            get => manager ?? (manager ?? MCUServices.Find.AuxCyclopsManager<BioAuxCyclopsManager>(Cyclops));
            set => manager = value;
        }

        [AssertNotNull]
        private ChildObjectIdentifier storageRoot;
        private ItemsContainer container;
        private Constructable buildable;
        private string prefabID;
        private float textDelay = TextDelayInterval;
        private bool isLoadingSaveData = false;
        private CyBioReactorSaveData saveData;
        private SubRoot Cyclops;
        private int lastKnownBioBooster = 0;
        private CyBioReactorDisplayHandler displayHandler;
        private CyBioReactorAudioHandler audioHandler;

        private readonly BioEnergyCollection bioMaterialsProcessing = new BioEnergyCollection();

        // Careful, this map only exists while the PDA screen is open
        private Dictionary<InventoryItem, uGUI_ItemIcon> pdaInventoryMapping = null;

        public bool IsConnectedToCyclops => Cyclops != null && manager != null;
        public bool ProducingPower => this.IsContructed && bioMaterialsProcessing.Count > 0;
        public bool HasPower => this.IsContructed && this.Charge > 0f;
        public bool IsContructed => (buildable != null) && buildable.constructed;

        public float Charge { get; private set; }
        public float Capacity { get; private set; } = MaxPowerBaseline;

        #region Initialization

        private void Start()
        {
            chargeRate = baselineChargeRate / this.TotalContainerSpaces * 2;

            SubRoot cyclops = GetComponentInParent<SubRoot>();

            if (cyclops == null)
            {
                QuickLogger.Debug("CyBioReactorMono: Could not find Cyclops during Start. Attempting external syncronize.");
                BioAuxCyclopsManager.SyncAllBioReactors();
            }
            else
            {
                QuickLogger.Debug("CyBioReactorMono: Parent cyclops found!");
                ConnectToCyclops(cyclops);
            }
        }

        internal int MixingStateHash { get; private set; }

        internal int DoorStateHash { get; private set; }

        internal int ScreenStateHash { get; private set; }

        internal CyBioReactorAnimationHandler AnimationHandler { get; private set; }

        public override void Awake()
        {
            base.Awake();

            InitializeConstructible();
            InitializeSaveData();
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
        }

        private void InitializeContainer()
        {
            if (container != null)
                return;

            container = new ItemsContainer(storageWidth, storageHeight, storageRoot.transform, CyBioReactor.StorageLabel, null);

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

        private void InitializeSaveData()
        {
            if (saveData != null)
                return;

            prefabID = GetComponentInParent<PrefabIdentifier>().Id;
            saveData = new CyBioReactorSaveData(prefabID);
        }

        private void InitializeStorageRoot()
        {
            if (storageRoot != null)
                return;

            var storeRoot = new GameObject("StorageRoot");
            storeRoot.transform.SetParent(this.transform, false);
            storageRoot = storeRoot.AddComponent<ChildObjectIdentifier>();
        }

        private void InitializeConstructible()
        {
            if (buildable != null)
                return;

            buildable = this.gameObject.GetComponent<Constructable>();
        }

        #endregion

        private void Update() // The all important Update method
        {
            if (this.ProducingPower)
            {
                float powerDeficit = this.Capacity - this.Charge;

                if (powerDeficit > MinimalPowerValue)
                {
                    float chargeOverTime = chargeRate * DayNightCycle.main.deltaTime;
                    float powerDrawnPerItem = Mathf.Min(powerDeficit, chargeOverTime);

                    float powerProduced = ProducePower(powerDrawnPerItem);

                    this.Charge = Mathf.Min(this.Charge + powerProduced, this.Capacity);
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
                displayHandler.SetActive(Mathf.RoundToInt(this.Charge), Mathf.CeilToInt(this.Capacity));
            else if (this.HasPower)
                displayHandler.SetInActivating(Mathf.RoundToInt(this.Charge), Mathf.CeilToInt(this.Capacity));
            else
                displayHandler.SetInactive();

            bool isOperating = this.ProducingPower && this.HasPower;

            if (this.AnimationHandler.GetBoolHash(this.MixingStateHash) == isOperating)
                return;

            this.AnimationHandler.SetBoolHash(this.MixingStateHash, isOperating);
        }

        #region Player interaction

        public void OnHandHover(GUIHand guiHand)
        {
            if (!buildable.constructed)
                return;

            HandReticle main = HandReticle.main;

            main.SetInteractText($"{Language.main.GetFormat("UseBaseBioReactor", Mathf.RoundToInt(this.Charge), Mathf.RoundToInt(this.Capacity))}{(bioMaterialsProcessing.Count > 0 ? "+" : "")}");
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        public void OnHandClick(GUIHand guiHand)
        {
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

                if (bioEnergy is null)
                {
                    QuickLogger.Debug("Matching pickable in bioreactor not found", true);
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
                for (int m = 0; m < bioMaterialsProcessing.Count; m++)
                {
                    BioEnergy material = bioMaterialsProcessing[m];
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
            if (requestedAmount < MinimalPowerValue || !this.HasPower)
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
            saveData.ReactorBatterCharge = this.Charge;
            saveData.SaveMaterialsProcessing(bioMaterialsProcessing);
            saveData.BoosterCount = lastKnownBioBooster;

            saveData.Save();
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            isLoadingSaveData = true;

            InitializeStorageRoot();

            container.Clear(false);

            isLoadingSaveData = false;
        }

        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
        }

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            isLoadingSaveData = true;

            bool hasSaveData = saveData.Load();

            if (hasSaveData)
            {
                container.Clear(false);

                UpdateBoosterCount(saveData.BoosterCount);

                this.Charge = Mathf.Min(this.Capacity, saveData.ReactorBatterCharge);

                List<BioEnergy> savedMaterials = saveData.GetMaterialsInProcessing();
                QuickLogger.Debug($"Found {savedMaterials.Count} materials in save data");

                for (int i = 0; i < savedMaterials.Count; i++)
                {
                    BioEnergy material = savedMaterials[i];
                    QuickLogger.Debug($"Adding {material.Pickupable.GetTechName()} to container from save data");
                    bioMaterialsProcessing.Add(material, container);
                }
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
            this.Manager = manager ?? MCUServices.Find.AuxCyclopsManager<BioAuxCyclopsManager>(parentCyclops);

            if (this.Manager != null)
            {
                this.Manager.AddBuildable(this);
            }
        }

        public void ConnectToInventory(Dictionary<InventoryItem, uGUI_ItemIcon> lookup)
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
                    QuickLogger.Debug("Matching pickable in bioreactor not found", true);
                    continue;
                }

                bioEnergy.AddDisplayText(icon);
            }
        }

        public bool HasRoomToShrink()
        {
            var nextStats = ReactorStats.GetStatsForBoosterCount(lastKnownBioBooster - 1);

            return nextStats.TotalSpaces >= bioMaterialsProcessing.SpacesOccupied;
        }

        public bool UpdateBoosterCount(int boosterCount)
        {
            if (boosterCount > MaxBoosters)
                return false;

            if (lastKnownBioBooster == boosterCount)
                return false;

            var nextStats = ReactorStats.GetStatsForBoosterCount(boosterCount);

            this.Capacity = nextStats.Capacity;

            if (!isLoadingSaveData)
            {
                this.Charge = Mathf.Min(this.Charge, this.Capacity);

                if (lastKnownBioBooster > boosterCount) // Getting smaller
                {
                    int nextAvailableSpace = nextStats.TotalSpaces;
                    while (bioMaterialsProcessing.SpacesOccupied > nextAvailableSpace)
                    {
                        BioEnergy material = bioMaterialsProcessing.GetCandidateForRemoval();

                        if (material == null)
                            break;

                        QuickLogger.Debug($"Removing material of size {material.Size}", true);
                        bioMaterialsProcessing.Remove(material, container);
                    }
                }
            }

            container.Resize(storageWidth = nextStats.Width, storageHeight = nextStats.Height);
            container.Sort();

            chargeRate = baselineChargeRate / this.TotalContainerSpaces * 2f;

            lastKnownBioBooster = boosterCount;

            return true;
        }

        private void OnDestroy()
        {
            if (manager != null)
                manager.RemoveBuildable(this);
            else
                BioAuxCyclopsManager.RemoveReactor(this);

            Cyclops = null;
            manager = null;
        }

        private class ReactorStats
        {
            private static readonly ReactorStats boost0 = new ReactorStats(2, 2, MaxPowerBaseline);
            private static readonly ReactorStats boost1 = new ReactorStats(3, 2, MaxPowerBaseline + 50f);
            private static readonly ReactorStats boost2 = new ReactorStats(3, 3, MaxPowerBaseline + 100f);
            private static readonly ReactorStats boost3 = new ReactorStats(6, 2, MaxPowerBaseline + 150f);

            internal readonly int Width;
            internal readonly int Height;
            internal readonly float Capacity;

            internal readonly int TotalSpaces;

            private ReactorStats(int width, int height, float capacity)
            {
                Width = width;
                Height = height;
                Capacity = capacity;
                TotalSpaces = Width * Height;
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

        public bool CanDeconstruct(out string reason)
        {
            bool flag = buildable.CanDeconstruct(out string result);
            reason = result;
            return flag;
        }

        public void OnConstructedChanged(bool constructed)
        {
            if (constructed)
            {
                displayHandler.TurnOnDisplay();
            }
        }
    }
}
