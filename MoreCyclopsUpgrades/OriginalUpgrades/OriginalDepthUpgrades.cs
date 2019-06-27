namespace MoreCyclopsUpgrades.OriginalUpgrades
{
    using MoreCyclopsUpgrades.API.Upgrades;

    internal class OriginalDepthUpgrades : TieredGroupHandler<float>
    {
        private const float NoBonusCrushDepth = 0f;
        private readonly CrushDamage crushDmg;

        public OriginalDepthUpgrades(SubRoot cyclops) : base(NoBonusCrushDepth, cyclops)
        {
            crushDmg = cyclops.gameObject.GetComponent<CrushDamage>();

            OnFinishedWithUpgrades += () =>
            {
                crushDmg.SetExtraCrushDepth(this.HighestValue);
            };

            TieredUpgradeHandler<float> tier1 = CreateTier(TechType.CyclopsHullModule1, 400f);
            tier1.MaxCount = 1;

            TieredUpgradeHandler<float> tier2 = CreateTier(TechType.CyclopsHullModule2, 800f);
            tier2.MaxCount = 1;

            TieredUpgradeHandler<float> tier3 = CreateTier(TechType.CyclopsHullModule3, 1200f);
            tier3.MaxCount = 1;
        }
    }
}
