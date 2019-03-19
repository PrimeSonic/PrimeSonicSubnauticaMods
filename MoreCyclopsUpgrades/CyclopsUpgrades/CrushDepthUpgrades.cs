namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    internal class CrushDepthUpgrades : TieredCyclopsUpgradeCollection<float>
    {
        public CrushDepthUpgrades() : base(0f)
        {
            OnFinishedUpgrades = (SubRoot cyclops) =>
            {
                CrushDamage crushDmg = cyclops.gameObject.GetComponent<CrushDamage>();

                if (crushDmg == null)
                    return;

                float orignialCrushDepth = crushDmg.crushDepth;

                crushDmg.SetExtraCrushDepth(this.BestValue);

                if (orignialCrushDepth != crushDmg.crushDepth)
                    ErrorMessage.AddMessage(Language.main.GetFormat("CrushDepthNow", crushDmg.crushDepth));
            };

            CreateTiers(SubRoot.hullReinforcement);
        }
    }
}
