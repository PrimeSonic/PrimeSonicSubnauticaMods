namespace CyclopsSpeedUpgrades
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.General;
    using MoreCyclopsUpgrades.API.Upgrades;
    using UnityEngine;

    internal class SpeedHandler : UpgradeHandler
    {
        internal const int MaxSpeedBoosters = 6;
        private const float EnginePowerPenalty = 0.75f;

        private readonly CyclopsSpeedModule speedModule;

        private static readonly float[] SpeedModifiers = new float[MaxSpeedBoosters + 1]
        {
            1.00f, 1.35f, 1.65f, 1.90f, 2.10f, 2.25f, 2.35f
        };

        private CyclopsMotorMode motorMode;
        private CyclopsMotorMode MotorMode => motorMode ?? (motorMode = base.Cyclops.GetComponentInChildren<CyclopsMotorMode>());

        private SubControl subControl;
        private SubControl SubControl => subControl ?? (subControl = base.Cyclops.GetComponentInChildren<SubControl>());

        private IPowerRatingManager ratingManager;
        private IPowerRatingManager RatingManager => ratingManager ?? (ratingManager = MCUServices.CrossMod.GetPowerRatingManager(base.Cyclops));

        private readonly float[] originalSpeeds = new float[3];
        private readonly float[] originalNoise = new float[3];

        private int lastKnownSpeedIndex = -1;

        public float SpeedMultiplier { get; private set; } = 1f;
        public float EfficiencyPenalty { get; private set; } = 1f;
        public float NoisePenalty { get; private set; } = 1f;

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

            OnFinishedUpgrades = () =>
            {
                this.EfficiencyPenalty = Mathf.Pow(EnginePowerPenalty, this.Count);

                this.RatingManager.ApplyPowerRatingModifier(TechType, this.EfficiencyPenalty);

                int speedIndex = this.Count;
                if (lastKnownSpeedIndex == speedIndex)
                    return;

                lastKnownSpeedIndex = speedIndex;

                float speedMultiplier = this.SpeedMultiplier = SpeedModifiers[speedIndex];
                float noiseMultiplier = this.NoisePenalty = 1f + 0.05f * speedIndex;

                // These will apply when changing speed modes
                this.MotorMode.motorModeSpeeds[0] = originalSpeeds[0] * speedMultiplier;
                this.MotorMode.motorModeSpeeds[1] = originalSpeeds[1] * speedMultiplier;
                this.MotorMode.motorModeSpeeds[2] = originalSpeeds[2] * speedMultiplier;

                this.MotorMode.motorModeNoiseValues[0] = originalNoise[0] * noiseMultiplier;
                this.MotorMode.motorModeNoiseValues[1] = originalNoise[1] * noiseMultiplier;
                this.MotorMode.motorModeNoiseValues[2] = originalNoise[2] * noiseMultiplier;

                // These will apply immediately
                CyclopsMotorMode.CyclopsMotorModes currentMode = this.MotorMode.cyclopsMotorMode;
                this.SubControl.BaseForwardAccel = this.MotorMode.motorModeSpeeds[(int)currentMode];

                ErrorMessage.AddMessage(CyclopsSpeedModule.SpeedRatingText(lastKnownSpeedIndex, speedMultiplier));
            };
        }
    }
}
