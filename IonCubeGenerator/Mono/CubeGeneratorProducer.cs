namespace IonCubeGenerator.Mono
{
    using Common;
    using ProtoBuf;
    using System;
    using UnityEngine;

    internal partial class CubeGeneratorMono
    {
        private static readonly Vector2int CubeSize = CraftData.GetItemSize(TechType.PrecursorIonCrystal);
        private static readonly GameObject CubePrefab = CraftData.GetPrefabForTechType(TechType.PrecursorIonCrystal);

        private const float EnergyConsumptionPerSecond = 1.35f;
        private const float CubeCreationTime = 60f;
        private const float CubeCreationCost = CubeCreationTime * EnergyConsumptionPerSecond;

        [ProtoMember(1)]
        [NonSerialized]
        private bool isGenerating = false;

        [ProtoMember(2)]
        [NonSerialized]
        private float timeToNextCube = -1f;

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

        private float NextCubePercentage => timeToNextCube > 0f
                                                ? (1f - timeToNextCube / CubeCreationCost) * 100f
                                                : 0f; // default to zero when not generating

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
        }

        private void Update()
        {
            // Monobehavior Update method
        }

        private void UpdateCubeGeneration()
        {
            coroutineStarted = true;

            bool isCurrentlyGenerating = false;

            if (this.IsConstructed && timeToNextCube > 0f)
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

                    if (timeToNextCube > 0f)
                    {
                        timeToNextCube = Mathf.Max(0f, timeToNextCube - energyToConsume);
                        isCurrentlyGenerating = true;
                    }

                    if (timeToNextCube == 0f)
                    {
                        timeToNextCube = -1f;
                        bool successfullySpawnedCube = SpawnCube();

                        if (successfullySpawnedCube)
                            TryStartingNextCube();
                    }
                }
                else
                {
                    PauseAnimation();
                }

                if (timeToNextCube == -1f)
                {
                    isCurrentlyGenerating = false;
                }
            }

            bool wasPreviouslyGenerating = isGenerating;
            isGenerating = isCurrentlyGenerating;

            if (isGenerating && !wasPreviouslyGenerating)
            {
                AnimationWorkingState();
            }
            else if (!isGenerating && wasPreviouslyGenerating)
            {
                AnimationIdleState();
            }
        }

        private bool SpawnCube()
        {
            if (!_cubeContainer.HasRoomFor(CubeSize.x, CubeSize.y))
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
            if (timeToNextCube > 0f)
                return;

            if (_cubeContainer.count < MaxAvailableSpaces)
            {
                timeToNextCube = CubeCreationCost;
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
