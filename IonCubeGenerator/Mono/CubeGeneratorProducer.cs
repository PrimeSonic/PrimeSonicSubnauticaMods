namespace IonCubeGenerator.Mono
{
    using Common;
    using UnityEngine;

    internal partial class CubeGeneratorMono
    {
        private static readonly Vector2int CubeSize = CraftData.GetItemSize(TechType.PrecursorIonCrystal);
        private static readonly GameObject CubePrefab = CraftData.GetPrefabForTechType(TechType.PrecursorIonCrystal);

        private const float DelayedStartTime = 0.5f;
        private const float RepeatingUpdateInterval = 1f;
        private const float CubeCreationTimeInterval = 10f;
        private const float CubeEnergyCost = 1200f;

        private float CubeCreationTime = 0f;
        private float EnergyConsumptionPerSecond = 0f;

        internal bool IsGenerating { get; private set; } = false;
        internal float TimeToNextCube = -1f;
        private SpeedModes currentMode = SpeedModes.High;

        internal SpeedModes CurrentSpeedMode
        {
            get => currentMode;
            set
            {
                currentMode = value;
                CubeCreationTime = CubeCreationTimeInterval * (int)currentMode;
                EnergyConsumptionPerSecond = currentMode != SpeedModes.Off
                                            ? CubeEnergyCost / CubeCreationTime
                                            : 0f;
            }
        }

        private PowerRelay _connectedRelay = null;

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

        private int NextCubePercentage => TimeToNextCube > 0f
                                                ? Mathf.RoundToInt((1f - TimeToNextCube / CubeEnergyCost) * 100)
                                                : 0; // default to zero when not generating

        private bool coroutineStarted = false;

        private void Start()
        {
            UpdatePowerRelay();

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

        private void Update()
        {
            // Monobehavior Update method
        }

        private void UpdateCubeGeneration()
        {
            coroutineStarted = true;

            if (_isLoadingSaveData)
                return;

            bool isCurrentlyGenerating = false;

            if (this.IsConstructed && currentMode > SpeedModes.Off && TimeToNextCube > 0f)
            {
                float energyToConsume = EnergyConsumptionPerSecond * DayNightCycle.main.dayNightSpeed;

                bool requiresEnergy = GameModeUtils.RequiresPower();

                bool hasPowerToConsume = !requiresEnergy || (this.AvailablePower >= energyToConsume); // Has enough power

                if (hasPowerToConsume)
                {
                    if (_AnimatorPausedState)
                    {
                        ResumeAnimation();
                    }

                    if (requiresEnergy && _connectedRelay != null)
                    {
                        _connectedRelay.ConsumeEnergy(energyToConsume, out float amountConsumed);
                    }

                    if (TimeToNextCube > 0f)
                    {
                        TimeToNextCube = Mathf.Max(0f, TimeToNextCube - energyToConsume);
                        isCurrentlyGenerating = true;
                    }

                    if (TimeToNextCube == 0f)
                    {
                        TimeToNextCube = -1f;
                        bool successfullySpawnedCube = SpawnCube();

                        if (successfullySpawnedCube)
                            TryStartingNextCube();
                    }
                }
                else
                {
                    PauseAnimation();
                }

                if (TimeToNextCube == -1f)
                {
                    isCurrentlyGenerating = false;
                }
            }

            bool wasPreviouslyGenerating = this.IsGenerating;
            this.IsGenerating = isCurrentlyGenerating;

            if (this.IsGenerating && !wasPreviouslyGenerating)
            {
                AnimationWorkingState();
            }
            else if (!this.IsGenerating && wasPreviouslyGenerating)
            {
                AnimationIdleState();
            }
        }

        internal bool SpawnCube()
        {
            if (this.CurrentCubeCount == MaxAvailableSpaces || !_cubeContainer.HasRoomFor(CubeSize.x, CubeSize.y))
            {
                AnimationIdleState();
                return false;
            }

            var gameObject = GameObject.Instantiate<GameObject>(CubePrefab);

            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
            var item = new InventoryItem(pickupable);

            _cubeContainer.UnsafeAdd(item);
            return true;
        }

        private void TryStartingNextCube()
        {
            if (TimeToNextCube > 0f || CurrentSpeedMode == SpeedModes.Off)
                return;

            if (this.CurrentCubeCount < MaxAvailableSpaces)
            {
                TimeToNextCube = CubeEnergyCost;
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
    }
}
