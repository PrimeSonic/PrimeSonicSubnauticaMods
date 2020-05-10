namespace CyclopsEngineUpgrades.Handlers
{
    using CyclopsEngineUpgrades.Craftables;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.General;
    using MoreCyclopsUpgrades.API.Upgrades;

    internal class EngineHandler : TieredGroupHandler<int>
    {
        private const int MaxAllowedPerTier = 1;
        private const int BaseValue = 0;
        private const int PowerIndexCount = 4;

        private static readonly float[] EnginePowerRatings = new float[PowerIndexCount]
        {
            1f, 3f, 4.5f, 6f
        };

        private static readonly float[] SilentRunningPowerCosts = new float[PowerIndexCount]
        {
            5f, 4.6f, 4.2f, 3.8f
        };

        private static readonly float[] SonarPowerCosts = new float[PowerIndexCount]
        {
            10f, 9.5f, 9f, 8.5f
        };

        private static readonly float[] ShieldPowerCosts = new float[PowerIndexCount]
        {
            50f, 46f, 42f, 38f
        };

        private int lastKnownPowerIndex = -1;

        private IPowerRatingManager ratingManager;
        private IPowerRatingManager RatingManager => ratingManager ?? (ratingManager = MCUServices.CrossMod.GetPowerRatingManager(base.Cyclops));

        public float CurrentRatingBonus => EnginePowerRatings[this.HighestValue] * 100f;

        public float EngineRating(TechType tier)
        {
            int index = base.TierValue(tier);
            return EnginePowerRatings[index];
        }

        public EngineHandler(PowerUpgradeModuleMk2 upgradeMk2, PowerUpgradeModuleMk3 upgradeMk3, SubRoot cyclops)
            : base(BaseValue, cyclops)
        {
            TieredUpgradeHandler<int> tier1 = CreateTier(TechType.PowerUpgradeModule, 1);
            tier1.MaxCount = MaxAllowedPerTier;

            TieredUpgradeHandler<int> tier2 = CreateTier(upgradeMk2.TechType, 2);
            tier2.MaxCount = MaxAllowedPerTier;

            TieredUpgradeHandler<int> tier3 = CreateTier(upgradeMk3.TechType, 3);
            tier3.MaxCount = MaxAllowedPerTier;

            OnFinishedUpgrades = () =>
            {
                int powerIndex = this.HighestValue;

                if (lastKnownPowerIndex != powerIndex)
                {
                    lastKnownPowerIndex = powerIndex;

                    base.Cyclops.silentRunningPowerCost = SilentRunningPowerCosts[powerIndex];
                    base.Cyclops.sonarPowerCost = SonarPowerCosts[powerIndex];
                    base.Cyclops.shieldPowerCost = ShieldPowerCosts[powerIndex];
                    this.RatingManager.ApplyPowerRatingModifier(TechType.PowerUpgradeModule, EnginePowerRatings[powerIndex]);
                }
            };
        }
    }
}
