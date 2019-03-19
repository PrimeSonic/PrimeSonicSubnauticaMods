namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using System;

    internal class TieredCyclopsUpgrade<T> : CyclopsUpgrade
         where T : IComparable<T>
    {
        internal readonly T TieredValue;
        internal readonly TieredCyclopsUpgradeCollection<T> ParentCollection;

        public TieredCyclopsUpgrade(TechType techType, T tieredValue, TieredCyclopsUpgradeCollection<T> parentCollection) : base(techType)
        {
            TieredValue = tieredValue;
            ParentCollection = parentCollection;

            OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) =>
            {
                ParentCollection.TierCounted(this);
            };
        }
    }
}
