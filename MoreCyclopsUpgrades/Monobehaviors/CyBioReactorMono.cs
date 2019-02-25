namespace MoreCyclopsUpgrades.Monobehaviors
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using Common;
    using ProtoBuf;
    using UnityEngine;
    using UWE;

    [ProtoContract]
    internal class CyBioReactorMono : HandTarget, IHandTarget, IProtoEventListener, IProtoTreeEventListener, ISubRootConnection
    {
        internal const int StorageWidth = 3;
        internal const int StorageHeight = 3;
        internal const float ChargeRatio = 0.80f;
        internal const float MaxPower = 400;

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

        public bool ProducingPower => _constructed >= 1f && this.Container.count > 0;

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
                    float chargeOverTime = ChargeRatio * DayNightCycle.main.deltaTime;

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
            if (BioReactorCharges.ContainsKey(item.item.GetTechType()))
            {
                item.isEnabled = false;
            }
        }

        private void OnRemoveItem(InventoryItem item)
        {

        }

        private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
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

        private float ProducePower(float requested)
        {
            if (requested > 0f && this.Container.count > 0)
            {
                _toConsume += requested;
                foreach (InventoryItem inventoryItem in this.Container)
                {
                    Pickupable item = inventoryItem.item;
                    TechType techType = item.GetTechType();
                    if (BioReactorCharges.TryGetValue(techType, out float bioEnergyValue) && _toConsume >= bioEnergyValue)
                    {
                        _toConsume -= bioEnergyValue;
                        toRemove.Add(item);
                    }
                }
                for (int i = toRemove.Count - 1; i >= 0; i--)
                {
                    Pickupable pickupable = toRemove[i];
                    this.Container.RemoveItem(pickupable, true);
                    Destroy(pickupable.gameObject);
                }
                toRemove.Clear();
                if (this.Container.count == 0)
                {
                    requested -= _toConsume;
                    _toConsume = 0f;
                }
            }

            return requested;
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
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            this.Container.Clear(false);
            if (_protoVersion < 2)
            {
                //if (this._serializedStorage != null)
                //{
                //    StorageHelper.RestoreItems(serializer, this._serializedStorage, this.container);
                //    this._serializedStorage = null;
                //}

                _protoVersion = 2;
            }
        }

        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
        }

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            StorageHelper.TransferItems(storageRoot.gameObject, this.Container);
            if (_protoVersion < 3)
            {
                CoroutineHost.StartCoroutine(CleanUpDuplicatedStorage());
            }
        }

        private IEnumerator CleanUpDuplicatedStorage()
        {
            yield return StorageHelper.DestroyDuplicatedItems(storageRoot.transform.parent.gameObject);
            _protoVersion = Mathf.Max(_protoVersion, 3);
            yield break;
        }

        public void ConnectToCyclops(SubRoot parentCyclops)
        {
            QuickLogger.Debug("BioReactor has been connected to Cyclops", true);
            this.ParentCyclops = parentCyclops;
            this.transform.SetParent(parentCyclops.transform);
        }

        private const float powerPerSecond = ChargeRatio;

        private const float chargeMultiplier = 7f;

        private const float storageMaxDistance = 4f;

        private const int _currentVersion = 3;

        [ProtoMember(1)]
        [NonSerialized]
        public int _protoVersion = 3;

        [ProtoMember(2)]
        [NonSerialized]
        public Base.Face _moduleFace;

        [ProtoMember(3)]
        [NonSerialized]
        public float _constructed = 1f;

        [ProtoMember(5)]
        [NonSerialized]
        public float _toConsume;

        [AssertNotNull]
        public ChildObjectIdentifier storageRoot;

        private ItemsContainer _container;


        private List<Pickupable> toRemove = new List<Pickupable>();
    }
}