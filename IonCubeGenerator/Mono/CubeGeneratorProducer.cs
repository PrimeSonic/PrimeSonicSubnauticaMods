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
        internal const float StartUpComplete = 4f;
        internal const float CooldownComplete = 19f;

        private const float DelayedStartTime = 3f;
        private const float CubeEnergyCost = 1500f;

        private SpeedModes _currentMode = StartingMode;
        private PowerRelay _connectedRelay = null;
        private Constructable _buildable = null;
        private ICubeContainer _cubeContainer;
        private ICubeGeneratorSaveHandler _saveData;

        private float CubeCreationTime = CubeEnergyCost;
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

        public bool PauseUpdates { get; set; } = false;

        public SpeedModes CurrentSpeedMode
        {
            get => _currentMode;
            set
            {
                SpeedModes previousMode = _currentMode;
                _currentMode = value;
                if (_currentMode != SpeedModes.Off)
                {
                    CubeCreationTime = Convert.ToSingle(_currentMode);
                    EnergyConsumptionPerSecond = CubeEnergyCost / CubeCreationTime;

                    if (previousMode == SpeedModes.Off)
                        TryStartingNextCube();
                }
                else // Off State
                {
                    EnergyConsumptionPerSecond = 0f;
                }
            }
        }

        public float StartUpProgress
        {
            get => _progress[(int)CubePhases.StartUp];
            set => _progress[(int)CubePhases.StartUp] = value;
        }

        public float GenerationProgress
        {
            get => _progress[(int)CubePhases.Generating];
            set => _progress[(int)CubePhases.Generating] = value;
        }

        public float CoolDownProgress
        {
            get => _progress[(int)CubePhases.CoolDown];
            set => _progress[(int)CubePhases.CoolDown] = value;
        }

        public float StartUpPercent => Mathf.Max(0f, this.StartUpProgress / StartUpComplete);
        public float GenerationPercent => Mathf.Max(0f, this.GenerationProgress / CubeEnergyCost);
        public float CoolDownPercent => Mathf.Max(0f, this.CoolDownProgress / CooldownComplete);

        public int NumberOfCubes
        {
            get => _cubeContainer.NumberOfCubes;
            set => _cubeContainer.NumberOfCubes = value;
        }

        public bool NotAllowToGenerate => this.PauseUpdates || !this.IsConstructed || this.CurrentSpeedMode == SpeedModes.Off;

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
                _saveData = new CubeGeneratorSaveData(id);
            }

            _cubeContainer = new CubeGeneratorContainer(this);

            UpdatePowerRelay();
        }

        private void Start()
        {
            UpdatePowerRelay();
        }

        private void Update()
        {
            if (this.NotAllowToGenerate)
                return;

            float energyToConsume = EnergyConsumptionPerSecond * DayNightCycle.main.deltaTime;
            bool requiresEnergy = GameModeUtils.RequiresPower();
            bool hasPowerToConsume = !requiresEnergy || (this.AvailablePower >= energyToConsume);

            if (!hasPowerToConsume)
                return;

            if (this.CoolDownProgress >= CooldownComplete)
            {
                QuickLogger.Debug("IonCube Generator - Finished Cooldown", true);

                this.PauseUpdates = true;
                _cubeContainer.NumberOfCubes++;
                // Finished cool down - See if the next cube can be started                
                TryStartingNextCube();

                this.PauseUpdates = false;
            }
            else if (this.CoolDownProgress >= 0f)
            {
                // Is currently cooling down
                this.CoolDownProgress = Mathf.Min(CooldownComplete, this.CoolDownProgress + DayNightCycle.main.deltaTime);
            }
            else if (this.GenerationProgress >= CubeEnergyCost)
            {
                QuickLogger.Debug("IonCube Generator - Cooldown", true);
                
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
            else if (this.StartUpProgress >= StartUpComplete)
            {
                QuickLogger.Debug("IonCube Generator - Generating", true);
                // Finished start up - Start generating cube
                this.GenerationProgress = 0f;
            }
            else if (this.StartUpProgress >= 0f)
            {
                // Is currently in start up routine
                this.StartUpProgress = Mathf.Min(StartUpComplete, this.StartUpProgress + DayNightCycle.main.deltaTime);
            }
        }

        #endregion

        public void OpenStorage()
        {
            _cubeContainer.OpenStorage();
        }

        private void TryStartingNextCube()
        {
            QuickLogger.Debug("Trying to start another cube", true);

            if (this.CurrentSpeedMode == SpeedModes.Off)
                return;// Powered off, can't start a new cube

            if (this.StartUpProgress < 0f || // Has not started a cube yet
                this.CoolDownProgress == CooldownComplete) // Has finished a cube
            {
                if (!_cubeContainer.IsFull)
                {
                    QuickLogger.Debug("IonCube Generator - Start up", true);
                    this.CoolDownProgress = -1f;
                    this.GenerationProgress = -1f;
                    this.StartUpProgress = 0f;
                }
                else
                {
                    QuickLogger.Debug("Cannot start another cube, container is full", true);
                }
            }
            else
            {
                QuickLogger.Debug("Cannot start another cube, another cube is currently in progress", true);
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
            _buildable.deconstructionAllowed = _cubeContainer.NumberOfCubes == 0;
            TryStartingNextCube();
        }

        #region ProtoTree methods

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            this.PauseUpdates = true;
            _saveData.LoadData(this);
            this.PauseUpdates = false;
        }

        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
            this.PauseUpdates = true;
            _saveData.SaveData(this);
            this.PauseUpdates = false;
        }

        #endregion
    }
}
