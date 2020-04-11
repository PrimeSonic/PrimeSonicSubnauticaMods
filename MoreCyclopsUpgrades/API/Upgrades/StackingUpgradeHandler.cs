using MoreCyclopsUpgrades.API.Buildables;

namespace MoreCyclopsUpgrades.API.Upgrades
{
    /// <summary>
    /// Represents a specialized type of upgrade module that is intended to stack its effect with other similar upgrades.<para/>
    /// This is always created through <see cref="StackingGroupHandler.CreateStackingTier(TechType)"/>.
    /// </summary>
    /// <seealso cref="UpgradeHandler" />
    public class StackingUpgradeHandler : UpgradeHandler, IGroupedUpgradeHandler
    {
        /// <summary>
        /// The parent <see cref="StackingGroupHandler"/> that manages the collection as a group.
        /// </summary>
        public readonly StackingGroupHandler ParentCollection;

        /// <summary>
        /// The parent <see cref="UpgradeHandler"/> that manages the collection as a group.
        /// </summary>
        public IGroupHandler GroupHandler => ParentCollection;

        internal StackingUpgradeHandler(TechType techType, StackingGroupHandler parentCollection)
            : base(techType, parentCollection.Cyclops)
        {
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
            ParentCollection.TierCounted(TechType, upgradeSlot.equipment, upgradeSlot.slotName, upgradeSlot.GetItemInSlot());
        }

        internal override void UpgradesFinished()
        {
            ParentCollection.UpgradesFinished();
        }
    }
}
