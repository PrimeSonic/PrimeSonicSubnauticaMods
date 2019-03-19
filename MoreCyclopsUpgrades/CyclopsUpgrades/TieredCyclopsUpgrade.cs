namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using System;

    public class TieredCyclopsUpgrade<T> : CyclopsUpgrade
         where T : IComparable<T>
    {
        public readonly T TieredValue;
        public readonly TieredCyclopsUpgradeCollection<T> ParentCollection;

        public TieredCyclopsUpgrade(TechType techType, T tieredValue, TieredCyclopsUpgradeCollection<T> parentCollection) : base(techType)
        {
            TieredValue = tieredValue;
            ParentCollection = parentCollection;

            OnClearUpgrades = (SubRoot cyclops) =>
            {
                ParentCollection.UpgradesCleared(cyclops);
            };

            OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) =>
            {
                ParentCollection.TierCounted(this);
            };

            OnFinishedUpgrades = (SubRoot cyclops) =>
            {
                ParentCollection.UpgradesFinished(cyclops);
            };
        }
    }
}
