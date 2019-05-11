namespace IonCubeGenerator.Mono
{
    using Common;
    using IonCubeGenerator.Enums;
    using System;
    using UnityEngine;

    internal partial class CubeGeneratorMono : MonoBehaviour, ICubeGeneratorSaveData, IProtoTreeEventListener
    {
        private static readonly GameObject CubePrefab = CraftData.GetPrefabForTechType(TechType.PrecursorIonCrystal);

        private const float DelayedStartTime = 0.5f;
        private const float RepeatingUpdateInterval = 1f;
        private const float CubeEnergyCost = 1200f;

        private float CubeCreationTime = 0f;
        private float EnergyConsumptionPerSecond = 0f;

        internal bool IsGenerating { get; private set; } = false;
        internal bool IsLoadingSaveData { get; private set; } = false;

        public float RemainingTimeToNextCube { get; set; } = -1f;

        private SpeedModes currentMode = SpeedModes.High;

        public SpeedModes CurrentSpeedMode
        {
            get => currentMode;
            set
            {
                currentMode = value;
                CubeCreationTime = Convert.ToSingle(currentMode);
                EnergyConsumptionPerSecond = currentMode != SpeedModes.Off
                                            ? CubeEnergyCost / CubeCreationTime
                                            : 0f;
            }
        }

        private PowerRelay _connectedRelay = null;
        private Constructable _buildable = null;
        private CubeGeneratorContainer _cubeContainer;

        private CubeGeneratorSaveData _saveData;

        internal bool IsConstructed => _buildable != null && _buildable.constructed;
        internal bool IsContainerFull => _cubeContainer.IsFull;

        private float AvailablePower
        {
            get
            {
                if (_connectedRelay == null)
                    UpdatePowerRelay();

                if (_connectedRelay == null)
                {
                    QuickLogger.Debug("Late call to find PowerRelay in parent failed.");
                    return 0f;
                }

                return _connectedRelay.GetPower();
            }
        }

        internal int NextCubePercentage => this.RemainingTimeToNextCube > 0f
                    ? Mathf.RoundToInt((1f - this.RemainingTimeToNextCube / CubeEnergyCost) * 100)
                    : 0; // default to zero when not generating

        public int NumberOfCubes
        {
            get => _cubeContainer.NumberOfCubes;
            set => _cubeContainer.NumberOfCubes = value;
        }

        private bool coroutineStarted = false;

        public bool HasBreakerTripped;
        private CubeGeneratorAnimator _animator;

        public void Awake()
        {
            if (_buildable == null)
            {
                _buildable = GetComponentInParent<Constructable>();
            }

            if (_saveData == null)
            {
                string id = GetComponentInParent<PrefabIdentifier>().Id;
                _saveData = new CubeGeneratorSaveData(id, CubeGeneratorContainer.MaxAvailableSpaces);
            }

            _cubeContainer = new CubeGeneratorContainer(this);
        }

        private void Start()
        {

            UpdatePowerRelay();

            _animator = this.gameObject.GetComponent<CubeGeneratorAnimator>();

            if (_animator == null)
            {
                QuickLogger.Error("Did not find Animator in parent during Start.");
            }


            if (_connectedRelay == null)
            {
                QuickLogger.Debug("Did not find PowerRelay in parent during Start. Trying again.");
                UpdatePowerRelay();
            }

            base.Invoke(nameof(TryStartingNextCube), DelayedStartTime);

            if (!coroutineStarted)
                base.InvokeRepeating(nameof(UpdateCubeGeneration), DelayedStartTime * 3f, RepeatingUpdateInterval);
            else
                QuickLogger.Debug("Start attempted to invoke coroutine twice but was prevented");

        }

        internal void OpenStorageState()
        {
            _cubeContainer.OpenStorageState();
        }

        internal void UpdateBreaker(bool value)
        {
            HasBreakerTripped = value;
            QuickLogger.Debug($"Current Breaker Value {HasBreakerTripped}", true);
        }

        private void Update()
        {
            // Monobehavior Update method
        }

        private void UpdateCubeGeneration()
        {
            coroutineStarted = true;

            if (this.IsLoadingSaveData || _animator.InCoolDown)
                return;

            bool isCurrentlyGenerating = false;

            if (this.IsConstructed && currentMode > SpeedModes.Off && this.RemainingTimeToNextCube > 0f)
            {
                float energyToConsume = EnergyConsumptionPerSecond * DayNightCycle.main.dayNightSpeed;

                bool requiresEnergy = GameModeUtils.RequiresPower();

                bool _hasPowerToConsume = !requiresEnergy || (this.AvailablePower >= energyToConsume); // Has enough power

                if (_hasPowerToConsume)
                {
                    if (requiresEnergy && _connectedRelay != null)
                    {
                        _connectedRelay.ConsumeEnergy(energyToConsume, out float amountConsumed);
                    }

                    if (this.RemainingTimeToNextCube > 0f)
                    {
                        this.RemainingTimeToNextCube = Mathf.Max(0f, this.RemainingTimeToNextCube - energyToConsume);
                        isCurrentlyGenerating = true;
                    }

                    if (this.RemainingTimeToNextCube == 0f)
                    {
                        this.RemainingTimeToNextCube = -1f;
                        _cubeContainer.NumberOfCubes++;

                        TryStartingNextCube();
                    }
                }

                if (this.RemainingTimeToNextCube == -1f)
                {
                    isCurrentlyGenerating = false;
                }

            }

            this.IsGenerating = isCurrentlyGenerating;
        }



        private void TryStartingNextCube()
        {
            if (this.RemainingTimeToNextCube > 0f || this.CurrentSpeedMode == SpeedModes.Off)
                return;

            if (!_cubeContainer.IsFull)
            {
                this.RemainingTimeToNextCube = CubeEnergyCost;
            }
        }

        private void UpdatePowerRelay()
        {
            PowerRelay relay = PowerSource.FindRelay(this.transform);
            if (relay != null && relay != _connectedRelay)
            {
                _connectedRelay = relay;
                QuickLogger.Debug("PowerRelay found at last!");
            }
            else
            {
                _connectedRelay = null;
            }
        }

        internal void OnAddItemEvent(InventoryItem item)
        {
            _buildable.deconstructionAllowed = false;
        }

        internal void OnRemoveItemEvent(InventoryItem item)
        {
            TryStartingNextCube();
            _buildable.deconstructionAllowed = _cubeContainer.NumberOfCubes == 0;
        }

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            this.IsLoadingSaveData = true;

            _saveData.LoadData(this);

            this.IsLoadingSaveData = false;

        }

        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
            _saveData.SaveData(this);
        }
    }
}
