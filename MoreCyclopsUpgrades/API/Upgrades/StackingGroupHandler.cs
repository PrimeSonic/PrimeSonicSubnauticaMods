﻿namespace MoreCyclopsUpgrades.API.Upgrades
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the complete collection of <see cref="StackingUpgradeHandler"/> instances.<para/>
    /// The events for this collection will be invoked only as needed.
    /// </summary>
    /// <seealso cref="UpgradeHandler" />
    public class StackingGroupHandler : UpgradeHandler, IGroupHandler
    {
        private bool cleared = false;
        private bool finished = false;
        private readonly ICollection<StackingUpgradeHandler> collection = new List<StackingUpgradeHandler>(3);
        private readonly IDictionary<TechType, int> counted = new Dictionary<TechType, int>(3);

        /// <summary>
        /// Gets a readonly list of the <see cref="TechType"/>s managed by this group handler.
        /// </summary>
        /// <value>
        /// The upgrade tiers managed by this group handler.
        /// </value>
        public IEnumerable<TechType> ManagedTiers => counted.Keys;

        /// <summary>
        /// Determines whether the specified tech type is managed by this group handler.
        /// </summary>
        /// <param name="techType">The TechTech to check.</param>
        /// <returns>
        ///   <c>true</c> if this group handler manages the specified TechTech; otherwise, <c>false</c>.
        /// </returns>
        public bool IsManaging(TechType techType)
        {
            return counted.ContainsKey(techType);
        }

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

        /// <summary>
        /// Initializes a new instance of the <see cref="StackingGroupHandler"/> class, with all necessary default events created.<para/>
        /// Use this for upgrades that stack similar effects while also allowing a mix of multiple tiers.
        /// </summary>
        /// <param name="cyclops">The cyclops.</param>
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

        internal void TierCounted(TechType countedTier, Equipment modules, string slot, InventoryItem inventoryItem)
        {
            counted[countedTier]++;
            this.Count++;

            OnUpgradeCounted?.Invoke();
            OnUpgradeCountedDetailed?.Invoke(modules, slot, inventoryItem);
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