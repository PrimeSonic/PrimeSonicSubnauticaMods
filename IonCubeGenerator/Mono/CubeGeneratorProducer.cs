namespace IonCubeGenerator.Mono
{
    using Common;
    using IonCubeGenerator.Enums;
    using IonCubeGenerator.Interfaces;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class CubeGeneratorMono : MonoBehaviour, ICubeGeneratorSaveData, IProtoTreeEventListener, ICubeContainer, ICubeProduction
    {
        private static readonly GameObject CubePrefab = CraftData.GetPrefabForTechType(TechType.PrecursorIonCrystal);

        internal const float ProgressComplete = 100f;
        internal const SpeedModes StartingMode = SpeedModes.Off;
        internal const float StartUpComplete = 10f;
        internal const float CooldownComplete = 20f;

        private const float DelayedStartTime = 0.5f;
        private const float CubeEnergyCost = 1200f;

        private const int MaxContainerSpaces = CubeGeneratorContainer.MaxAvailableSpaces;
        private SpeedModes _currentMode = StartingMode;
        private PowerRelay _connectedRelay = null;
        private Constructable _buildable = null;
        private ICubeContainer _cubeContainer;
        private ICubeGeneratorSaveHandler _saveData;

        private float CubeCreationTime = 0f;
        private float EnergyConsumptionPerSecond = 0f;
        private readonly IList<float> _progress = new List<float>(new[] { -1f, -1f, -1f });

        private PowerRelay ConnectedRelay
        {
            get
            {
                while (_connectedRelay == null)
                    UpdatePowerRelay();

                return _connectedRelay;
            }
        }

        internal bool IsConstructed => _buildable != null && _buildable.constructed;

        public bool IsFull => _cubeContainer.IsFull;

        private float AvailablePower => this.ConnectedRelay.GetPower();

        public bool IsLoadingSaveData { get; set; } = false;

        public SpeedModes CurrentSpeedMode
        {
            get => _currentMode;
            set
            {
                SpeedModes previousState = _currentMode;
                _currentMode = value;
                if (value != SpeedModes.Off)
                {
                    CubeCreationTime = Convert.ToSingle(_currentMode);
                    EnergyConsumptionPerSecond = CubeEnergyCost / CubeCreationTime;

                    if (previousState == SpeedModes.Off)
                        TryStartingNextCube();
                }
                else // Off State
                {
                    CubeCreationTime = -1f;
                    EnergyConsumptionPerSecond = -1f;
                }
            }
        }

        private float StartUpProgress
        {
            get => this.Progress[(int)CubePhases.StartUp];
            set => this.Progress[(int)CubePhases.StartUp] = value;
        }

        private float GenerationProgress
        {
            get => this.Progress[(int)CubePhases.Generating];
            set => this.Progress[(int)CubePhases.Generating] = value;
        }

        private float CoolDownProgress
        {
            get => this.Progress[(int)CubePhases.CoolDown];
            set => this.Progress[(int)CubePhases.CoolDown] = value;
        }

        public float StartUpPercent => Mathf.Max(0f, this.StartUpProgress / StartUpComplete);
        public float GenerationPercent => Mathf.Max(0f, this.GenerationProgress / CubeEnergyCost);
        public float CoolDownPercent => Mathf.Max(0f, this.CoolDownProgress / CooldownComplete);

        public int NumberOfCubes
        {
            get => _cubeContainer.NumberOfCubes;
            set => _cubeContainer.NumberOfCubes = value;
        }

        public bool NotAllowToGenerate => this.IsLoadingSaveData || !this.IsConstructed || this.CurrentSpeedMode == SpeedModes.Off;

        public IList<float> Progress
        {
            get => _progress;
            set
            {
                _progress[(int)CubePhases.StartUp] = value[(int)CubePhases.StartUp];
                _progress[(int)CubePhases.Generating] = value[(int)CubePhases.Generating];
                _progress[(int)CubePhases.CoolDown] = value[(int)CubePhases.CoolDown];
            }
        }

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
                _saveData = new CubeGeneratorSaveData(id, MaxContainerSpaces);
            }

            _cubeContainer = new CubeGeneratorContainer(this);
        }

        private void Start()
        {
            UpdatePowerRelay();

            base.Invoke(nameof(TryStartingNextCube), DelayedStartTime);
        }

        private void Update()
        {
            if (this.NotAllowToGenerate)
                return;

            float energyToConsume = EnergyConsumptionPerSecond * DayNightCycle.main.dayNightSpeed;
            bool requiresEnergy = GameModeUtils.RequiresPower();
            bool hasPowerToConsume = !requiresEnergy || (this.AvailablePower >= energyToConsume);

            if (!hasPowerToConsume)
                return;

            if (this.CoolDownProgress == CooldownComplete)
            {
                // Finished cool down - See if the next cube can be started
                _cubeContainer.NumberOfCubes++;
                TryStartingNextCube();
            }
            else if (this.CoolDownProgress >= 0f)
            {
                // Is currently cooling down
                this.CoolDownProgress = Mathf.Min(CooldownComplete, this.CoolDownProgress + DayNightCycle.main.dayNightSpeed);
            }
            else if (this.GenerationProgress == CubeEnergyCost)
            {
                // Finished generating cube - Start cool down
                this.CoolDownProgress = 0f;
            }
            else if (this.GenerationProgress >= 0f)
            {                
                if (requiresEnergy)
                    this.ConnectedRelay.ConsumeEnergy(energyToConsume, out float amountConsumed);

                // Is currently generating cube
                this.GenerationProgress = Mathf.Min(CubeEnergyCost, this.GenerationProgress + energyToConsume);
            }
            else if (this.StartUpProgress == StartUpComplete)
            {
                // Finished start up - Start generating cube
                this.GenerationProgress = 0f;
            }
            else if (this.StartUpProgress >= 0f)
            {
                // Is currently in start up routine
                this.StartUpProgress = Mathf.Min(StartUpComplete, this.StartUpProgress + DayNightCycle.main.dayNightSpeed);
            }
        }

        #endregion

        public void OpenStorage()
        {
            _cubeContainer.OpenStorage();
        }

        private void TryStartingNextCube()
        {
            if (this.CurrentSpeedMode == SpeedModes.Off)
                return;// Powered off, can't start a new cube

            if (this.Progress[(int)CubePhases.StartUp] <= 0f || // Has not started a cube yet
                this.Progress[(int)CubePhases.CoolDown] == CooldownComplete) // Has finished a cube
            {
                if (!_cubeContainer.IsFull)
                    StartNextCube();
            }
        }

        private void StartNextCube()
        {
            this.Progress[(int)CubePhases.StartUp] = 0f;
            this.Progress[(int)CubePhases.Generating] = -1f;
            this.Progress[(int)CubePhases.CoolDown] = -1f;
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
            _buildable.deconstructionAllowed = _cubeContainer.NumberOfCubes == 0;
            TryStartingNextCube();
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
