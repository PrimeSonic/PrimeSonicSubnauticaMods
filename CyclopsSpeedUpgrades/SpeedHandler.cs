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

        private static readonly float[] SlowSpeedModifiers = new float[MaxSpeedBoosters + 1]
        {
            1.00f, 1.25f, 1.40f, 1.50f, 1.60f, 1.65f, 1.70f
        };

        private static readonly float[] StandardSpeedModifiers = new float[MaxSpeedBoosters + 1]
        {
            1.00f, 1.40f, 1.70f, 1.90f, 2.05f, 2.15f, 2.20f
        };

        private static readonly float[] FlankSpeedModifiers = new float[MaxSpeedBoosters + 1]
        {
            1.00f, 1.50f, 1.70f, 1.80f, 1.90f, 1.95f, 2.00f
        };

        private CyclopsMotorMode motorMode;
        private CyclopsMotorMode MotorMode => motorMode ?? (motorMode = base.cyclops.GetComponentInChildren<CyclopsMotorMode>());

        private SubControl subControl;
        private SubControl SubControl => subControl ?? (subControl = base.cyclops.GetComponentInChildren<SubControl>());

        private IPowerRatingManager ratingManager;
        private IPowerRatingManager RatingManager => ratingManager ?? (ratingManager = MCUServices.CrossMod.GetPowerRatingManager(cyclops));

        private readonly float[] originalSpeeds = new float[3];
        private readonly float[] originalNoise = new float[3];

        private int lastKnownSpeedIndex = -1;

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
                float efficiencyPenalty = Mathf.Pow(EnginePowerPenalty, this.Count);

                this.RatingManager.ApplyPowerRatingModifier(techType, efficiencyPenalty);

                int speedIndex = this.Count;
                if (lastKnownSpeedIndex == speedIndex)
                    return;

                lastKnownSpeedIndex = speedIndex;

                float slowMultiplier = SlowSpeedModifiers[speedIndex];
                float standardMultiplier = StandardSpeedModifiers[speedIndex];
                float flankMultiplier = FlankSpeedModifiers[speedIndex];
                float noiseMultiplier = 1f + 0.1f * speedIndex;

                // These will apply when changing speed modes
                this.MotorMode.motorModeSpeeds[0] = originalSpeeds[0] * slowMultiplier;
                this.MotorMode.motorModeSpeeds[1] = originalSpeeds[1] * standardMultiplier;
                this.MotorMode.motorModeSpeeds[2] = originalSpeeds[2] * flankMultiplier;

                this.MotorMode.motorModeNoiseValues[0] = originalNoise[0] * noiseMultiplier;
                this.MotorMode.motorModeNoiseValues[1] = originalNoise[1] * noiseMultiplier;
                this.MotorMode.motorModeNoiseValues[2] = originalNoise[2] * noiseMultiplier;

                // These will apply immediately
                CyclopsMotorMode.CyclopsMotorModes currentMode = this.MotorMode.cyclopsMotorMode;
                this.SubControl.BaseForwardAccel = this.MotorMode.motorModeSpeeds[(int)currentMode];

                ErrorMessage.AddMessage(CyclopsSpeedModule.SpeedRatingText(lastKnownSpeedIndex, standardMultiplier));
            };
        }
    }
}
