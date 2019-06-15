namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using System;

    /// <summary>
    /// Represents a specialized type of upgrade module that is intended to be non-stacking, where only the best version applies.
    /// This is always created through <see cref="TieredUpgradesHandlerCollection{T}.CreateTier(TechType, T)"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="UpgradeHandler" />
    public class TieredUpgradeHandler<T> : UpgradeHandler
         where T : IComparable<T>
    {
        /// <summary>
        /// The value of this upgrade's tier. Higher values are always used before lower values.
        /// </summary>
        public readonly T TieredValue;

        /// <summary>
        /// The parent collection. Create this first.
        /// </summary>
        public readonly TieredUpgradesHandlerCollection<T> ParentCollection;

        internal TieredUpgradeHandler(TechType techType, T tieredValue, TieredUpgradesHandlerCollection<T> parentCollection) : base(techType, parentCollection.cyclops)
        {
            TieredValue = tieredValue;
            ParentCollection = parentCollection;
        }

        internal override void UpgradesCleared()
        {            
            ParentCollection.UpgradesCleared();
        }

        internal override void UpgradeCounted(Equipment modules, string slot)
        {
            ParentCollection.TierCounted(TieredValue, modules, slot);
        }

        internal override void UpgradesFinished()
        {
            ParentCollection.UpgradesFinished();
        }
    }
}
