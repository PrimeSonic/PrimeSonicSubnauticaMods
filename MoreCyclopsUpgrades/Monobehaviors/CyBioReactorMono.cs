namespace MoreCyclopsUpgrades.Monobehaviors
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Common;
    using MoreCyclopsUpgrades.SaveData;
    using ProtoBuf;
    using UnityEngine;

    [ProtoContract]
    internal class CyBioReactorMono : HandTarget, IHandTarget, IProtoEventListener, IProtoTreeEventListener, ISubRootConnection
    {
        internal const int StorageWidth = 2;
        internal const int StorageHeight = 2;
        internal const int TotalContainerSpaces = StorageHeight * StorageWidth;
        internal const float ChargePerSecondPerItem = 0.82f / TotalContainerSpaces * 2;
        internal const float MaxPower = 200;

        // This will be set externally
        public SubRoot ParentCyclops { get; private set; }

        private Constructable _buildable;
        public Constructable Buildable
        {
            get
            {
                if (_buildable is null)
                    _buildable = this.gameObject.GetComponent<Constructable>();

                return _buildable;
            }
        }

        private Battery _battery;
        public Battery Battery
        {
            get
            {
                if (_battery is null)
                {
                    _battery = this.gameObject.AddComponent<Battery>();
                    _battery.transform.SetParent(this.transform, false);
                    _battery._capacity = MaxPower;
                }

                return _battery;
            }
        }

        private static Dictionary<TechType, float> _bioReactorCharges;
        internal static readonly FieldInfo BioEnergyLookupInfo = typeof(BaseBioReactor).GetField("charge", BindingFlags.Static | BindingFlags.NonPublic);
        internal static Dictionary<TechType, float> BioReactorCharges
        {
            get
            {
                if (_bioReactorCharges is null)
                {
                    _bioReactorCharges = (Dictionary<TechType, float>)BioEnergyLookupInfo.GetValue(null);
                }

                return _bioReactorCharges;
            }
        }

        public bool ProducingPower => _constructed >= 1f && this.MaterialsProcessing.Count > 0;

        public int CurrentPower => Mathf.RoundToInt(this.Battery.charge);

        internal ItemsContainer Container
        {
            get
            {
                if (_container == null)
                {
                    _container = new ItemsContainer(StorageWidth, StorageHeight, storageRoot.transform, "CyBioReactorStorageLabel", null);

                    _container.isAllowedToAdd += IsAllowedToAdd;
                    _container.isAllowedToRemove += IsAllowedToRemove;

                    _container.onAddItem += OnAddItem;
                    _container.onRemoveItem += OnRemoveItem;
                }

                return _container;
            }
        }

        public override void Awake()
        {
            base.Awake();

            InitializeStorage();

            if (SaveData == null)
            {
                string id = GetComponentInParent<PrefabIdentifier>().Id;
                SaveData = new CyBioReactorSaveData(id);
            }
        }

        private void InitializeStorage()
        {
            if (storageRoot is null)
            {
                var storeRoot = new GameObject("StorageRoot");
                storeRoot.transform.SetParent(this.transform, false);
                storageRoot = storeRoot.AddComponent<ChildObjectIdentifier>();
            }
        }

        private void Update()
        {
            if (this.ProducingPower)
            {
                float powerDeficit = this.Battery.capacity - this.Battery.charge;

                if (powerDeficit > 0f)
                {
                    float chargeOverTime = ChargePerSecondPerItem * DayNightCycle.main.deltaTime;

                    chargeOverTime = Mathf.Min(chargeOverTime, powerDeficit);

                    float powerProduced = ProducePower(chargeOverTime);

                    this.Battery.charge += powerProduced;
                }
            }
        }

        public void OnHandHover(GUIHand guiHand)
        {
            if (!this.Buildable.constructed)
                return;

            HandReticle main = HandReticle.main;
            main.SetInteractText($"Use Cyclops BioReacactor {this.CurrentPower}/{MaxPower}");
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        public void OnHandClick(GUIHand guiHand)
        {
            PDA pda = Player.main.GetPDA();
            Inventory.main.SetUsedStorage(this.Container, false);
            pda.Open(PDATab.Inventory, null, null, 4f);
        }

        public static float GetChargeValue(TechType techType) => BioReactorCharges.GetOrDefault(techType, -1f);

        private void OnAddItem(InventoryItem item)
        {
            if (isLoadingSaveData)
            {
                item.isEnabled = false;
                return;
            }

            if (BioReactorCharges.TryGetValue(item.item.GetTechType(), out float bioEnergyValue) && bioEnergyValue > 0f)
            {
                item.isEnabled = false;
                this.MaterialsProcessing.Add(new BioEnergy(item.item, bioEnergyValue, bioEnergyValue));
            }
            else
            {
                Destroy(item.item.gameObject); // Failsafe
            }

        }

        private void OnRemoveItem(InventoryItem item)
        {

        }

        private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
            if (isLoadingSaveData)
                return true;

            bool canAdd = false;
            if (pickupable != null)
            {
                TechType techType = pickupable.GetTechType();

                if (BioReactorCharges.ContainsKey(techType))
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

        private bool IsAllowedToRemove(Pickupable pickupable, bool verbose) => false;

        private float ProducePower(float powerDrawnPerItem)
        {
            float powerProduced = 0f;

            if (powerDrawnPerItem > 0f && // More than zero energy being produced per item per time delta
                this.MaterialsProcessing.Count > 0 && // There should be materials in the reactor to process
                this.Battery.charge < this.Battery.capacity) // Stop producing power if the battery is full
            {
                float availablePowerPerItem;

                foreach (BioEnergy material in this.MaterialsProcessing)
                {
                    availablePowerPerItem = Mathf.Max(0f, material.Energy - powerDrawnPerItem);
                    powerProduced += availablePowerPerItem;
                    material.Energy -= availablePowerPerItem;

                    if (material.FullyConsumed)
                        this.FullyConsumed.Add(material);
                }
            }

            if (this.FullyConsumed.Count > 1)
            {
                foreach (BioEnergy material in this.FullyConsumed)
                {
                    this.MaterialsProcessing.Remove(material);
                    this.Container.RemoveItem(material.Item, true);
                    Destroy(material.Item.gameObject);
                }

                this.FullyConsumed.Clear();
            }

            return powerProduced;
        }

        public float Constructed
        {
            get => _constructed;
            set
            {
                value = Mathf.Clamp01(value);
                if (_constructed != value)
                {
                    _constructed = value;
                    if (_constructed < 1f)
                    {
                        if (_constructed <= 0f)
                        {
                            Destroy(this.gameObject);
                        }
                    }
                }
            }
        }

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            SaveData.ReactorBatterCharge = this.Battery.charge;
            SaveData.SaveMaterialsProcessing(this.MaterialsProcessing);

            SaveData.Save();
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            isLoadingSaveData = true;

            InitializeStorage();

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

                this.Battery.charge = SaveData.ReactorBatterCharge;

                List<BioEnergy> materials = SaveData.GetMaterialsInProcessing();

                foreach (BioEnergy item in materials)
                {
                    this.Container.AddItem(item.Item);
                }

                this.MaterialsProcessing.AddRange(materials);
            }

            isLoadingSaveData = false;
        }

        public void ConnectToCyclops(SubRoot parentCyclops)
        {
            QuickLogger.Debug("BioReactor has been connected to Cyclops", true);
            this.ParentCyclops = parentCyclops;
            this.transform.SetParent(parentCyclops.transform);
        }

        [ProtoMember(3)]
        [NonSerialized]
        public float _constructed = 1f;

        [AssertNotNull]
        public ChildObjectIdentifier storageRoot;

        private ItemsContainer _container;

        internal List<BioEnergy> MaterialsProcessing { get; } = new List<BioEnergy>();
        private List<BioEnergy> FullyConsumed { get; } = new List<BioEnergy>(TotalContainerSpaces);

        private bool isLoadingSaveData = false;
        private CyBioReactorSaveData SaveData;
    }
}