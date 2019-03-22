namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using Common;
    using System.Collections.Generic;

    internal class CrushDepthUpgradesHandler : TieredUpgradesHandlerCollection<float>
    {
        private const float NoBonusCrushDepth = 0f;

        public CrushDepthUpgradesHandler() : base(NoBonusCrushDepth)
        {
            OnFinishedUpgrades += (SubRoot cyclops) =>
            {
                CrushDamage crushDmg = cyclops.gameObject.GetComponent<CrushDamage>();

                crushDmg.SetExtraCrushDepth(this.HighestValue);
            };

            foreach (KeyValuePair<TechType, float> upgrade in SubRoot.hullReinforcement)
                CreateTier(upgrade.Key, upgrade.Value);
        }
    }
}
