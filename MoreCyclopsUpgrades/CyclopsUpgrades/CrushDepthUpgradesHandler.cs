namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using System.Collections.Generic;

    internal class CrushDepthUpgradesHandler : TieredUpgradesHandlerCollection<float>
    {
        private const float NoBonusCrushDepth = 0f;

        public CrushDepthUpgradesHandler() : base(NoBonusCrushDepth)
        {
            this.LoggingName = "Depth Upgrades Collection";

            OnFinishedUpgrades += (SubRoot cyclops) =>
            {
                CrushDamage crushDmg = cyclops.gameObject.GetComponent<CrushDamage>();

                crushDmg.SetExtraCrushDepth(this.HighestValue);
            };

            foreach (KeyValuePair<TechType, float> upgrade in SubRoot.hullReinforcement)
            {
                TieredUpgradeHandler<float> tier = CreateTier(upgrade.Key, upgrade.Value);
                tier.LoggingName = $"Depth Upgrade {tier.TieredValue}";
            }
        }
    }
}
