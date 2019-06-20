namespace MoreCyclopsUpgrades.StandardUpgrades
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API.Upgrades;

    internal class CrushDepthUpgradesHandler : TieredGroupHandler<float>
    {
        private const float NoBonusCrushDepth = 0f;

        public CrushDepthUpgradesHandler(SubRoot cyclops) : base(NoBonusCrushDepth, cyclops)
        {
            OnFinishedWithUpgrades += () =>
            {
                CrushDamage crushDmg = cyclops.gameObject.GetComponent<CrushDamage>();

                crushDmg.SetExtraCrushDepth(this.HighestValue);
            };

            foreach (KeyValuePair<TechType, float> upgrade in SubRoot.hullReinforcement)
            {
                TieredUpgradeHandler<float> tier = CreateTier(upgrade.Key, upgrade.Value);
                tier.MaxCount = 1;
            }
        }
    }
}
