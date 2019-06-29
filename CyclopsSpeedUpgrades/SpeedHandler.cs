namespace CyclopsSpeedUpgrades.Handlers
{
    using CyclopsSpeedUpgrades.Craftables;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.General;
    using MoreCyclopsUpgrades.API.Upgrades;
    using UnityEngine;

    internal class SpeedHandler : UpgradeHandler
    {
        internal const int MaxSpeedBoosters = 6;
        private const float EnginePowerPenalty = 0.75f;

        private readonly CyclopsSpeedModule speedModule;

        private static readonly float[] SlowSpeedBonuses = new float[MaxSpeedBoosters] // Max +70%
        {
            0.25f, 0.15f, 0.10f, 0.10f, 0.05f, 0.05f // Diminishing returns on speed modules            
        };

        private static readonly float[] StandardSpeedBonuses = new float[MaxSpeedBoosters] // Max +120%
        {
            0.40f, 0.30f, 0.20f, 0.15f, 0.10f, 0.05f // Diminishing returns on speed modules            
        };

        private static readonly float[] FlankSpeedBonuses = new float[MaxSpeedBoosters] // Max +95%
        {
            0.45f, 0.20f, 0.10f, 0.10f, 0.05f, 0.05f // Diminishing returns on speed modules            
        };

        private CyclopsMotorMode motorMode;
        private CyclopsMotorMode MotorMode => motorMode ?? (motorMode = base.cyclops.GetComponentInChildren<CyclopsMotorMode>());

        private SubControl subControl;
        private SubControl SubControl => subControl ?? (subControl = base.cyclops.GetComponentInChildren<SubControl>());

        private IPowerRatingManager ratingManager;
        private IPowerRatingManager RatingManager => ratingManager ?? (ratingManager = MCUServices.CrossMod.GetPowerRatingManager(cyclops));

        private readonly float[] originalSpeeds = new float[3];
        private readonly float[] originalNoise = new float[3];

        private int lastKnownSpeedBoosters = -1;

        public SpeedHandler(CyclopsSpeedModule cyclopsSpeedModule, SubRoot cyclops) : base(cyclopsSpeedModule.TechType, cyclops)
        {
            speedModule = cyclopsSpeedModule;
            this.MaxCount = MaxSpeedBoosters;

            // Store the original values before we start to change them
            originalSpeeds[0] = this.MotorMode.motorModeSpeeds[0];
            originalSpeeds[1] = this.MotorMode.motorModeSpeeds[1];
            originalSpeeds[2] = this.MotorMode.motorModeSpeeds[2];

            originalNoise[0] = this.MotorMode.motorModeNoiseValues[0];
            originalNoise[1] = this.MotorMode.motorModeNoiseValues[1];
            originalNoise[2] = this.MotorMode.motorModeNoiseValues[2];

            OnFirstTimeMaxCountReached = () =>
            {
                ErrorMessage.AddMessage(CyclopsSpeedModule.MaxRatingAchived);
            };

            OnFinishedUpgrades = UpdatePowerSpeedRating;
        }

        internal void UpdatePowerSpeedRating()
        {
            // Speed modules can affect power rating too
            float efficiencyPenalty = Mathf.Pow(EnginePowerPenalty, this.Count);

            this.RatingManager.ApplyPowerRatingModifier(techType, efficiencyPenalty);

            int speedBoosters = this.Count;
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
