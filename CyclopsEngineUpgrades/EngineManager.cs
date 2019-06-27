namespace CyclopsEngineUpgrades
{
    using CyclopsEngineUpgrades.Craftables;
    using CyclopsEngineUpgrades.Handlers;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.General;
    using UnityEngine;

    internal class EngineManager : IAuxCyclopsManager
    {
        internal const string ManagerName = "CyPowMgr";
        private const float EnginePowerPenalty = 0.7f;
        internal const int MaxSpeedBoosters = 6;
        private const int PowerIndexCount = 4;

        /// <summary>
        /// "Practically zero" for all intents and purposes. Any energy value lower than this should be considered zero.
        /// </summary>
        public const float MinimalPowerValue = MCUServices.MinimalPowerValue;

        private static readonly float[] SlowSpeedBonuses = new float[MaxSpeedBoosters]
        {
            0.25f, 0.15f, 0.10f, 0.10f, 0.05f, 0.05f // Diminishing returns on speed modules
            // Max +70%
        };

        private static readonly float[] StandardSpeedBonuses = new float[MaxSpeedBoosters]
        {
            0.40f, 0.30f, 0.20f, 0.15f, 0.10f, 0.05f // Diminishing returns on speed modules
            // Max +120%
        };

        private static readonly float[] FlankSpeedBonuses = new float[MaxSpeedBoosters]
        {
            0.45f, 0.20f, 0.10f, 0.10f, 0.05f, 0.05f // Diminishing returns on speed modules
            // Max +95%
        };

        private static readonly float[] EnginePowerRatings = new float[PowerIndexCount]
        {
            1f, 3f, 5f, 6f
        };

        private static readonly float[] SilentRunningPowerCosts = new float[PowerIndexCount]
        {
            5f, 5f, 4f, 3f // Lower costs here don't show up until the Mk2
        };

        private static readonly float[] SonarPowerCosts = new float[PowerIndexCount]
        {
            10f, 10f, 8f, 7f // Lower costs here don't show up until the Mk2
        };

        private static readonly float[] ShieldPowerCosts = new float[PowerIndexCount]
        {
            50f, 50f, 45f, 35f // Lower costs here don't show up until the Mk2
        };

        private float lastKnownPowerRating = -1f;
        private int lastKnownSpeedBoosters = -1;
        private int lastKnownPowerIndex = -1;

        private readonly float[] originalSpeeds = new float[3];
        private readonly float[] originalNoise = new float[3];

        internal SpeedHandler SpeedBoosters;
        internal EngineHandler EngineEfficientyUpgrades;

        internal readonly SubRoot Cyclops;

        private CyclopsMotorMode motorMode;
        private CyclopsMotorMode MotorMode => motorMode ?? (motorMode = Cyclops.GetComponentInChildren<CyclopsMotorMode>());

        private SubControl subControl;
        private SubControl SubControl => subControl ?? (subControl = Cyclops.GetComponentInChildren<SubControl>());

        public string Name { get; } = ManagerName;

        internal EngineManager(SubRoot cyclops)
        {
            Cyclops = cyclops;
        }

        public bool Initialize(SubRoot cyclops)
        {
            // Store the original values before we start to change them
            originalSpeeds[0] = this.MotorMode.motorModeSpeeds[0];
            originalSpeeds[1] = this.MotorMode.motorModeSpeeds[1];
            originalSpeeds[2] = this.MotorMode.motorModeSpeeds[2];

            originalNoise[0] = this.MotorMode.motorModeNoiseValues[0];
            originalNoise[1] = this.MotorMode.motorModeNoiseValues[1];
            originalNoise[2] = this.MotorMode.motorModeNoiseValues[2];

            return Cyclops == cyclops;
        }

        /// <summary>
        /// Updates the Cyclops power and speed rating.
        /// Power Rating manages engine efficiency as well as the power cost of using Silent Running, Sonar, and Defense Shield.
        /// Speed rating manages bonus speed across all motor modes.
        /// </summary>
        internal void UpdatePowerSpeedRating()
        {
            int powerIndex = EngineEfficientyUpgrades.HighestValue;
            int speedBoosters = SpeedBoosters.Count;

            if (lastKnownPowerIndex != powerIndex)
            {
                lastKnownPowerIndex = powerIndex;

                Cyclops.silentRunningPowerCost = SilentRunningPowerCosts[powerIndex];
                Cyclops.sonarPowerCost = SonarPowerCosts[powerIndex];
                Cyclops.shieldPowerCost = ShieldPowerCosts[powerIndex];
            }

            // Speed modules can affect power rating too
            float efficiencyBonus = EnginePowerRatings[powerIndex];

            for (int i = 0; i < speedBoosters; i++)
            {
                efficiencyBonus *= EnginePowerPenalty;
            }

            int cleanRating = Mathf.CeilToInt(100f * efficiencyBonus);

            while (cleanRating % 5 != 0)
                cleanRating--;

            float powerRating = cleanRating / 100f;

            if (lastKnownPowerRating != powerRating)
            {
                lastKnownPowerRating = powerRating;
                MCUServices.CrossMod.ChangePowerRatingWithPenalty(Cyclops, powerRating);

                // Inform the new power rating just like the original method would.
                ErrorMessage.AddMessage(Language.main.GetFormat("PowerRatingNowFormat", powerRating));
            }

            if (speedBoosters > MaxSpeedBoosters)
                return; // Exit here

            if (lastKnownSpeedBoosters != speedBoosters)
            {
                lastKnownSpeedBoosters = speedBoosters;

                float slowMultiplier = 1f;
                float standardMultiplier = 1f;
                float slankMultiplier = 1f;
                float noiseMultiplier = 1f;

                // Calculate the speed multiplier with diminishing returns
                while (--speedBoosters > -1)
                {
                    slowMultiplier += SlowSpeedBonuses[speedBoosters];
                    standardMultiplier += StandardSpeedBonuses[speedBoosters];
                    slankMultiplier += FlankSpeedBonuses[speedBoosters];
                    noiseMultiplier += 0.1f;
                }

                // These will apply when changing speed modes
                this.MotorMode.motorModeSpeeds[0] = originalSpeeds[0] * slowMultiplier;
                this.MotorMode.motorModeSpeeds[1] = originalSpeeds[1] * standardMultiplier;
                this.MotorMode.motorModeSpeeds[2] = originalSpeeds[2] * slankMultiplier;

                this.MotorMode.motorModeNoiseValues[0] = originalNoise[0] * noiseMultiplier;
                this.MotorMode.motorModeNoiseValues[1] = originalNoise[1] * noiseMultiplier;
                this.MotorMode.motorModeNoiseValues[2] = originalNoise[2] * noiseMultiplier;

                // These will apply immediately
                CyclopsMotorMode.CyclopsMotorModes currentMode = this.MotorMode.cyclopsMotorMode;
                this.SubControl.BaseForwardAccel = this.MotorMode.motorModeSpeeds[(int)currentMode];

                ErrorMessage.AddMessage(CyclopsSpeedModule.SpeedRatingText(lastKnownSpeedBoosters, Mathf.RoundToInt(standardMultiplier * 100)));
            }
        }
    }
}
