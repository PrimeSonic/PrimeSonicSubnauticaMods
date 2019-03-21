namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using Common;
    using System.Collections.Generic;

    internal class CrushDepthUpgrades : TieredCyclopsUpgradeCollection<float>
    {
        private const float NoBonusCrushDepth = 0f;

        public CrushDepthUpgrades() : base(NoBonusCrushDepth)
        {
            OnClearUpgrades += (SubRoot cyclops) =>
            {
                CrushDamage crushDmg = cyclops.gameObject.GetComponent<CrushDamage>();

                if (crushDmg == null)
                {
                    QuickLogger.Debug("Unable to locate CrushDamage in Cyclops");
                    return;
                }

                crushDmg.SetExtraCrushDepth(NoBonusCrushDepth);
            };

            OnFinishedUpgrades += (SubRoot cyclops) =>
            {
                CrushDamage crushDmg = cyclops.gameObject.GetComponent<CrushDamage>();

                if (crushDmg == null)
                {
                    QuickLogger.Debug("Unable to locate CrushDamage in Cyclops");
                    return;
                }

                float orignialCrushDepth = crushDmg.crushDepth;

                crushDmg.SetExtraCrushDepth(this.HighestValue);

                if (orignialCrushDepth != crushDmg.crushDepth)
                    ErrorMessage.AddMessage(Language.main.GetFormat("CrushDepthNow", crushDmg.crushDepth));
            };

            foreach (KeyValuePair<TechType, float> upgrade in SubRoot.hullReinforcement)
            {
                TieredCyclopsUpgrade<float> tier = CreateTier(upgrade.Key, upgrade.Value);

                tier.OnUpgradeCounted += (SubRoot cyclops, Equipment modules, string slot) =>
                {
                    QuickLogger.Debug($"Crush depth upgrade counted: {tier.TieredValue}");
                };
            }
        }
    }
}
