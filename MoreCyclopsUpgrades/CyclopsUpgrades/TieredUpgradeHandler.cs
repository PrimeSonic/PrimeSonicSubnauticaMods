namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using System;

    /// <summary>
    /// Represents a specialized type of upgrade module that is intended to be non-stacking, where only the best version applies.
    /// This is always created through <see cref="TieredUpgradeHandlerCollection.CreateTier(TechType, T)"/> or 
    /// <see cref="TieredUpgradeHandlerCollection.CreateTiers(IDictionary{TechType, T})"/>
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
        public readonly TieredUpgradeHandlerCollection<T> ParentCollection;

        internal TieredUpgradeHandler(TechType techType, T tieredValue, TieredUpgradeHandlerCollection<T> parentCollection) : base(techType)
        {
            TieredValue = tieredValue;
            ParentCollection = parentCollection;
        }

        internal override void UpgradesCleared(SubRoot cyclops)
        {            
            ParentCollection.UpgradesCleared(cyclops);
        }

        internal override void UpgradeCounted(SubRoot cyclops, Equipment modules, string slot)
        {
            ParentCollection.TierCounted(TieredValue, cyclops, modules, slot);
        }

        internal override void UpgradesFinished(SubRoot cyclops)
        {
            // Always report, even if count is 0
            ParentCollection.UpgradesFinished(cyclops);
        }
    }
}
