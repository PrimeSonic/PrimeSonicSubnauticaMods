namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using System;

    /// <summary>
    /// Represents a specialized type of upgrade module that is intended to be non-stacking, where only the best version applies.
    /// This is always created through <see cref="TieredCyclopsUpgradeCollection.CreateTier(TechType, T)"/> or 
    /// <see cref="TieredCyclopsUpgradeCollection.CreateTiers(IDictionary{TechType, T})"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="CyclopsUpgrade" />
    public class TieredCyclopsUpgrade<T> : CyclopsUpgrade
         where T : IComparable<T>
    {
        /// <summary>
        /// The value of this upgrade's tier. Higher values are always used before lower values.
        /// </summary>
        public readonly T TieredValue;

        /// <summary>
        /// The parent collection. Create this first.
        /// </summary>
        public readonly TieredCyclopsUpgradeCollection<T> ParentCollection;

        internal TieredCyclopsUpgrade(TechType techType, T tieredValue, TieredCyclopsUpgradeCollection<T> parentCollection) : base(techType)
        {
            TieredValue = tieredValue;
            ParentCollection = parentCollection;

            OnClearUpgrades += (SubRoot cyclops) =>
            {
                ParentCollection.UpgradesCleared(cyclops);
            };

            OnUpgradeCounted += (SubRoot cyclops, Equipment modules, string slot) =>
            {
                ParentCollection.TierCounted(TieredValue);
            };

            OnFinishedUpgrades += (SubRoot cyclops) =>
            {
                ParentCollection.UpgradesFinished(cyclops);
            };
        }
    }
}
