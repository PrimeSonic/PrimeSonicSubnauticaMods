namespace MoreCyclopsUpgrades.CyclopsUpgrades
{

    internal class DepthCyclopsUpgrade : CyclopsUpgrade
    {
        internal readonly float BonusDepth;

        public DepthCyclopsUpgrade(TechType techType, float bonusDepth) : base(techType)
        {
            BonusDepth = bonusDepth;
        }
    }
}
