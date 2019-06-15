namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using System.Collections.Generic;

    internal class CrushDepthUpgradesHandler : TieredUpgradesHandlerCollection<float>
    {
        private const float NoBonusCrushDepth = 0f;

        public CrushDepthUpgradesHandler(SubRoot cyclops) : base(NoBonusCrushDepth, cyclops)
        {
            OnFinishedUpgrades += () =>
            {
                CrushDamage crushDmg = cyclops.gameObject.GetComponent<CrushDamage>();

                crushDmg.SetExtraCrushDepth(this.HighestValue);
            };

            foreach (KeyValuePair<TechType, float> upgrade in SubRoot.hullReinforcement)
            {
                TieredUpgradeHandler<float> tier = CreateTier(upgrade.Key, upgrade.Value);
            }
        }
    }
}
