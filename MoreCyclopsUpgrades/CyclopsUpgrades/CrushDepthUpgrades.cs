namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using System.Collections.Generic;

    internal class CrushDepthUpgrades : TieredCyclopsUpgradeCollection<float>
    {
        public CrushDepthUpgrades() : base(0f)
        {
            OnFinishedUpgrades += (SubRoot cyclops) =>
            {
                CrushDamage crushDmg = cyclops.gameObject.GetComponent<CrushDamage>();

                if (crushDmg == null)
                    return;

                float orignialCrushDepth = crushDmg.crushDepth;

                crushDmg.SetExtraCrushDepth(this.HighestValue);

                if (orignialCrushDepth != crushDmg.crushDepth)
                    ErrorMessage.AddMessage(Language.main.GetFormat("CrushDepthNow", crushDmg.crushDepth));
            };

            foreach (KeyValuePair<TechType, float> upgrade in SubRoot.hullReinforcement)
                CreateTier(upgrade.Key, upgrade.Value);
        }
    }
}
