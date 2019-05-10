using System.Text;

namespace IonCubeGenerator.Mono
{
    using Common;
    using IonCubeGenerator.Display;
    using IonCubeGenerator.Enums;
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
        private IonGeneratorDisplay _display;
        public bool HasBreakerTripped;

        private void UpdateSystem()
        {
            if (this.AvailablePower <= 0)
            {
                QuickLogger.Debug("No Power", true);
                if (_display != null)
                {
                    //There is no power so lets turn off the display
                    _display.ShutDownDisplay();
                }

                //Pause the animator
                PauseAnimation();

                return;
            }

            if (_display.HasBeenShutDown)
            {
                _display.TurnOnDisplay();
            }

            if (HasBreakerTripped)
            {
                QuickLogger.Debug("Breaker Tripped", true);
                if (_display != null)
                {
                    //There is no power so lets turn off the display
                    _display.PowerOffDisplay();
                }

                //Pause the animator
                PauseAnimation();

                return;
            }


            //If we pass all these conditions show the screen
            if ((this.CurrentCubeCount == MaxAvailableSpaces ||
                 !_cubeContainer.HasRoomFor(CubeSize.x, CubeSize.y))) return;
            ResumeAnimation();
            AnimationWorkingState();
            _display.PowerOnDisplay();
        }

        private void Start()
        {
            RetrieveAnimator();
            UpdatePowerRelay();

            _display = this.gameObject.GetComponent<IonGeneratorDisplay>();
            _display.Setup(this, ChangeStorageState, UpdateBreaker);

            if (_connectedRelay == null)
            {
                QuickLogger.Debug("Did not find PowerRelay in parent during Start. Trying again.");
                UpdatePowerRelay();
            }

            base.Invoke(nameof(TryStartingNextCube), DelayedStartTime);

            if (!coroutineStarted)
            {
                base.InvokeRepeating(nameof(UpdateCubeGeneration), DelayedStartTime * 3f, RepeatingUpdateInterval);
                //base.InvokeRepeating(nameof(UpdateSystem), DelayedStartTime * 3f, RepeatingUpdateInterval);
            }
            else
                QuickLogger.Debug("Start attempted to invoke coroutine twice but was prevented");

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
            UpdateSystem();

            if (!HasBreakerTripped && !_animatorPausedState && !_coolDownPeriod)
            {
                coroutineStarted = true;

                if (_isLoadingSaveData)
                    return;

                bool isCurrentlyGenerating = false;

                if (this.IsConstructed && currentMode > SpeedModes.Off && TimeToNextCube > 0f)
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

                    if (TimeToNextCube == -1f)
                    {
                        isCurrentlyGenerating = false;
                    }

                }

                this.IsGenerating = isCurrentlyGenerating;
            }
        }

        internal bool SpawnCube()
        {
            if (this.CurrentCubeCount == MaxAvailableSpaces || !_cubeContainer.HasRoomFor(CubeSize.x, CubeSize.y))
                return false;

            var gameObject = GameObject.Instantiate<GameObject>(CubePrefab);

            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
            var item = new InventoryItem(pickupable);

            _cubeContainer.UnsafeAdd(item);
            return true;
        }

        private void TryStartingNextCube()
        {
            if (TimeToNextCube > 0f || this.CurrentSpeedMode == SpeedModes.Off)
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

        private void CreateDisplayedIonCube()
        {
            GameObject ionSlot = gameObject.FindChild("model").FindChild("Platform_Lifter").FindChild("Ion_Lifter")
                .FindChild("IonCube").FindChild("precursor_crystal")?.gameObject;


            if (ionSlot != null)
            {
                QuickLogger.Debug("Ion Cube Display Object Created", true);
                GameObject displayedIonCube = GameObject.Instantiate<GameObject>(CubePrefab);
                displayedIonCube.transform.SetParent(ionSlot.transform);
                displayedIonCube.transform.localPosition =
                    new Vector3(-0.1152f, 0.05f, 0f); // Is to high maybe the axis is flipped
                displayedIonCube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                displayedIonCube.transform.Rotate(new Vector3(0, 0, 90));
            }

            else
            {
                QuickLogger.Error("Cannot Find IonCube in the prefab");
            }
        }

    }
}
