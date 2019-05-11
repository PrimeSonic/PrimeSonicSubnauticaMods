using Steamworks;

namespace IonCubeGenerator.Mono
{
    using Common;
    using IonCubeGenerator.Enums;
    using IonCubeGenerator.Interfaces;
    using System;
    using UnityEngine;

    internal class CubeGeneratorMono : MonoBehaviour, ICubeGeneratorSaveData, IProtoTreeEventListener, ICubeContainer, ICubeProduction
    {
        private static readonly GameObject CubePrefab = CraftData.GetPrefabForTechType(TechType.PrecursorIonCrystal);

        internal const float ProgressComplete = 100f;

        private const float DelayedStartTime = 0.5f;
        private const float RepeatingUpdateInterval = 1f;
        private const float CubeEnergyCost = 1200f;

        private SpeedModes _currentMode = SpeedModes.High;
        private PowerRelay _connectedRelay = null;
        private Constructable _buildable = null;
        private ICubeContainer _cubeContainer;
        private ICubeGeneratorSaveHandler _saveData;

        private float CubeCreationTime = 0f;
        private float EnergyConsumptionPerSecond = 0f;

        internal bool IsGenerating { get; private set; } = false;
        public bool IsLoadingSaveData { get; set; } = false;

        public float RemainingTimeToNextCube { get; set; } = -1f;

        public SpeedModes CurrentSpeedMode
        {
            get => _currentMode;
            set
            {
                _currentMode = value;
                if (_currentMode != SpeedModes.Off)
                {
                    CubeCreationTime = Convert.ToSingle(_currentMode);
                    EnergyConsumptionPerSecond = CubeEnergyCost / CubeCreationTime;
                }
                else
                {
                    CubeCreationTime = -1f;
                    EnergyConsumptionPerSecond = -1f;
                }
            }
        }

        internal bool IsConstructed => _buildable != null && _buildable.constructed;
        public bool IsFull => _cubeContainer.IsFull;

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

        public float CubeProgress
        {
            get
            {
                return this.RemainingTimeToNextCube > 0f
                        ? ((1f - (this.RemainingTimeToNextCube / CubeEnergyCost)) * ProgressComplete)
                        : 0f; // default to zero when not generating
            }
        }

        public int NumberOfCubes
        {
            get => _cubeContainer.NumberOfCubes;
            set => _cubeContainer.NumberOfCubes = value;
        }

        public bool IsProductionStopped => this.IsLoadingSaveData || !this.IsConstructed;

        private CubeGeneratorAnimator _animator;

        #region Unity methods

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
            base.InvokeRepeating(nameof(UpdateCubeGeneration), DelayedStartTime * 3f, RepeatingUpdateInterval);
        }

        #endregion

        public void OpenStorage()
        {
            _cubeContainer.OpenStorage();
        }

        private void UpdateCubeGeneration()
        {
            if (this.IsProductionStopped || _animator.InCoolDown)
                return;

            bool isCurrentlyGenerating = false;

            if (this.CurrentSpeedMode > SpeedModes.Off && this.RemainingTimeToNextCube > 0f)
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

        #region ProtoTree methods

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            _saveData.LoadData(this);
        }

        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
            _saveData.SaveData(this);
        }

        #endregion
    }
}
