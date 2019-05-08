using IonCubeGenerator.Display;
using IonCubeGenerator.Enums;
using System;
using Serialization;

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
        private PowerState _powerState = PowerState.None;

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
            if (AvailablePower <= 0)
            {
                QuickLogger.Debug("No Power", true);
                if (_display != null)
                {
                    //There is no power so lets turn off the display
                    _display.ShutDownDisplay();
                }

                //Pause the animator
                PauseAnimation();

                //return to prevent any other code from running
                return;
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
            
            if (this.CurrentCubeCount == MaxAvailableSpaces || !_cubeContainer.HasRoomFor(CubeSize.x, CubeSize.y))
            {
                QuickLogger.Debug("Storage Full", true);
                //Pause the animator
                PauseAnimation();
                return;
            }

            //If we pass all these conditions show the screen
            ResumeAnimation();
            AnimationWorkingState();
            _display.PowerOnDisplay();
            
        }
        
        private void Start()
        {
            UpdatePowerRelay();

            _display = gameObject.GetComponent<IonGeneratorDisplay>();
            _display.Setup(ChangeStorageState, UpdateBreaker);
            
            if (_connectedRelay == null)
            {
                QuickLogger.Debug("Did not find PowerRelay in parent during Start. Trying again.");
                UpdatePowerRelay();
            }

            base.Invoke(nameof(TryStartingNextCube), DelayedStartTime);

            if (!coroutineStarted)
            {
                base.InvokeRepeating(nameof(UpdateCubeGeneration), DelayedStartTime * 3f, RepeatingUpdateInterval);
                base.InvokeRepeating(nameof(UpdateSystem), DelayedStartTime * 3f, RepeatingUpdateInterval);
            }
            else
                QuickLogger.Debug("Start attempted to invoke coroutine twice but was prevented");

        }

        private void UpdateBreaker(bool value)
        {
            QuickLogger.Debug($"Changed Breaker Value to {value}", true);
            HasBreakerTripped = value;
            QuickLogger.Debug($"Current Breaker Value {HasBreakerTripped}", true);

        }

        private void Update()
        {
            // Monobehavior Update method
        }

        private void UpdateCubeGeneration()
        {
            if (!HasBreakerTripped || !_AnimatorPausedState)
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
            if (this.CurrentCubeCount == MaxAvailableSpaces || !_cubeContainer.HasRoomFor(CubeSize.x, CubeSize.y)) return false;

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
