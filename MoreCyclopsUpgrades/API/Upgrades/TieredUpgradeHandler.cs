namespace MoreCyclopsUpgrades.API.Upgrades
{
    using System;
    using MoreCyclopsUpgrades.API.Buildables;

    /// <summary>
    /// Represents a specialized type of upgrade module that is intended to be non-stacking, where only the best version applies.<para/>
    /// This is always created through <see cref="TieredGroupHandler{T}.CreateTier(TechType, T)"/>.
    /// </summary>
    /// <typeparam name="T">The data type used to sort the tiers.</typeparam>
    /// <seealso cref="UpgradeHandler" />
    public class TieredUpgradeHandler<T> : UpgradeHandler, IGroupedUpgradeHandler where T : IComparable<T>
    {
        /// <summary>
        /// The value of this upgrade's tier. Higher values are always used before lower values.
        /// </summary>
        public readonly T TieredValue;

        /// <summary>
        /// The parent <see cref="TieredGroupHandler{T}"/> that manages the collection as a group.
        /// </summary>
        public readonly TieredGroupHandler<T> ParentCollection;

        /// <summary>
        /// The parent <see cref="UpgradeHandler"/> that manages the collection as a group.
        /// </summary>
        public IGroupHandler GroupHandler => ParentCollection;

        internal TieredUpgradeHandler(TechType techType, T tieredValue, TieredGroupHandler<T> parentCollection) : base(techType, parentCollection.Cyclops)
        {
            TieredValue = tieredValue;
            ParentCollection = parentCollection;
        }

        internal override void UpgradesCleared()
        {
            this.Count = 0;
            ParentCollection.UpgradesCleared();
        }

        internal override void UpgradeCounted(UpgradeSlot upgradeSlot)
        {
            this.Count++;
            ParentCollection.TierCounted(TieredValue, upgradeSlot.equipment, upgradeSlot.slotName, upgradeSlot.GetItemInSlot());
        }

        internal override void UpgradesFinished()
        {
            ParentCollection.UpgradesFinished();
        }
    }
}
