namespace IonCubeGenerator.Mono
{
    using UnityEngine;

    internal partial class CubeGeneratorMono
    {
        private static readonly Vector2int CubeSize = CraftData.GetItemSize(TechType.PrecursorIonCrystal);
        private static readonly GameObject CubePrefab = CraftData.GetPrefabForTechType(TechType.PrecursorIonCrystal);

        private const float EnergyConsumptionMultiplier = 1.25f;
        private const float CubeCreationTime = 1000f;

        private bool isGenerating = false;
        private float timeToNextCube = -1f;
        private PowerRelay powerRelay;


        private void Update()
        {
            // Monobehavior Update method
        }


        private void UpdateCubeGeneration()
        {
            bool isCurrentlyGenerating = false;
            if (timeToNextCube > 0f)
            {
                float energyToConsume = EnergyConsumptionMultiplier * DayNightCycle.main.dayNightSpeed;

                bool requiresEnergy = GameModeUtils.RequiresPower();

                bool hasPowerToConsume = !requiresEnergy || (powerRelay != null && powerRelay.GetPower() >= energyToConsume); // Has enough power

                if (hasPowerToConsume)
                {
                    if (_AnimatorPausedState)
                    {
                        ResumeAnimation();
                    }

                    if (requiresEnergy)
                    {
                        powerRelay.ConsumeEnergy(energyToConsume, out float amountConsumed);
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
                PlaySounds();
            }
            else if (!isGenerating && wasPreviouslyGenerating)
            {
                AnimationIdleState();
                StopSounds();
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

            int currentCubeCount = _cubeContainer.GetCount(TechType.PrecursorIonCrystal);
            if (currentCubeCount < MaxAvailableSpaces)                
            {
                timeToNextCube = CubeCreationTime;
            }
        }
    }
}
