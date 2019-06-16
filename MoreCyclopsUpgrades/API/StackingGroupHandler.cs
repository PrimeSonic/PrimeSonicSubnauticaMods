namespace MoreCyclopsUpgrades.API
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the complete collection of <see cref="StackingUpgradeHandler"/> instances.<para/>
    /// The events for this collection will be invoked only as few times as needed.
    /// </summary>
    /// <seealso cref="UpgradeHandler" />
    public class StackingGroupHandler : UpgradeHandler
    {
        private readonly ICollection<StackingUpgradeHandler> collection = new List<StackingUpgradeHandler>(3);
        private readonly IDictionary<TechType, int> counted = new Dictionary<TechType, int>(3);

        private bool cleared = false;
        private bool finished = false;

        /// <summary>
        /// Gets the total count of all stacking tiers of upgrades.
        /// </summary>
        /// <value>
        /// The total count of all upgrades managed by this collection.
        /// </value>
        public int TotalCount
        {
            get
            {
                int total = 0;

                foreach (int count in counted.Values)
                    total += count;

                return total;
            }
        }

        /// <summary>
        /// Returns how many upgrades of a specific tier were counted.
        /// </summary>
        /// <param name="tier">The tier to check.</param>
        /// <returns>The total number of upgrades when the <see cref="TechType"/> is managed by this collection; Otherwise returns -1.</returns>
        public int TierCount(TechType tier)
        {
            if (counted.TryGetValue(techType, out int count))
            {
                return count;
            }

            return -1;
        }

        /// <summary>
        /// Gets the total counts of the specified upgrade tier.<para/>
        /// WARNING: This method provides no error checking.
        /// </summary>
        /// <value>
        /// The total count of this tier of upgrade.
        /// </value>
        /// <param name="tier">The tier to check.</param>
        /// <returns>The total number of upgrades when the <see cref="TechType"/> is managed by this collection.</returns>
        /// <seealso cref="TierCount(TechType)"/>
        /// <exception cref="KeyNotFoundException"/>
        public int this[TechType tier]
        {
            get => counted[tier];
        }

        public StackingGroupHandler(SubRoot cyclops)
            : base(TechType.None, cyclops)
        {
        }

        /// <summary>
        /// Adds a new <see cref="StackingUpgradeHandler" /> to the collection, with all necessary default events created.
        /// </summary>
        /// <param name="techType">The TechType of the upgrade module.</param>
        /// <returns>The newly created <see cref="StackingUpgradeHandler" /> intance.</returns>
        public StackingUpgradeHandler CreateStackingTier(TechType techType)
        {
            var stackingUpgrade = new StackingUpgradeHandler(techType, this);
            collection.Add(stackingUpgrade);
            counted.Add(techType, 0);

            return stackingUpgrade;
        }

        internal override void UpgradesCleared()
        {
            if (cleared) // Because this might be called multiple times by the various members of the tier,
                return; // exit if we've already made this call.

            cleared = true;
            finished = false;

            foreach (TechType tier in counted.Keys)
                counted[tier] = 0;

            OnClearUpgrades?.Invoke();
        }

        internal void TierCounted(TechType countedTier, Equipment modules, string slot)
        {
            counted[countedTier]++;
            this.Count++;

            OnUpgradeCounted?.Invoke(modules, slot);
        }

        internal override void UpgradesFinished()
        {
            if (finished) // Because this might be called multiple times by the various members of the tier,
                return; // exit if we've already made this call.

            finished = true;
            cleared = false;

            OnFinishedWithUpgrades?.Invoke();
        }

        internal override void RegisterSelf(IDictionary<TechType, UpgradeHandler> dictionary)
        {
            foreach (UpgradeHandler upgrade in collection)
                upgrade.RegisterSelf(dictionary);
        }
    }
}
