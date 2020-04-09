namespace MoreCyclopsUpgrades.API.Upgrades
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the complete collection of <see cref="TieredUpgradeHandler{T}"/> instances.<para/>
    /// The events for this collection will be invoked only as needed.
    /// </summary>
    /// <typeparam name="T">The data type used to sort the tiers.</typeparam>
    /// <seealso cref="UpgradeHandler" />
    public class TieredGroupHandler<T> : UpgradeHandler, IGroupHandler where T : IComparable<T>
    {
        private bool cleared = false;
        private bool finished = false;
        private readonly List<TieredUpgradeHandler<T>> collection = new List<TieredUpgradeHandler<T>>();

        /// <summary>
        /// Gets a readonly list of the <see cref="TechType"/>s managed by this group handler.
        /// </summary>
        /// <value>
        /// The upgrade tiers managed by this group handler.
        /// </value>
        public IEnumerable<TechType> ManagedTiers
        {
            get
            {
                foreach (TieredUpgradeHandler<T> tier in collection)
                    yield return tier.TechType;
            }
        }

        /// <summary>
        /// Determines whether the specified tech type is managed by this group handler.
        /// </summary>
        /// <param name="techType">The TechTech to check.</param>
        /// <returns>
        ///   <c>true</c> if this group handler manages the specified TechTech; otherwise, <c>false</c>.
        /// </returns>
        public bool IsManaging(TechType techType)
        {
            return collection.Exists(t => t.TechType == techType);
        }

        /// <summary>
        /// Gets the highest value reported among the <see cref="TieredUpgradeHandler{T}" /> of this collection.
        /// </summary>
        /// <value>
        /// The highest value.
        /// </value>
        public T HighestValue { get; private set; }

        /// <summary>
        /// The default value to reset to during the <see cref="UpgradeHandler.OnClearUpgrades"/> event.
        /// </summary>
        public readonly T DefaultValue;

        /// <summary>
        /// Get the value assigned to the specified tier techtype.
        /// </summary>
        /// <param name="tierId">The TechTech to check.</param>
        /// <returns>The value associated to the tier if found; Otherwise returns <see cref="DefaultValue"/>.</returns>
        public T TierValue(TechType tierId)
        {
            TieredUpgradeHandler<T> tier = collection.Find(t => t.TechType == tierId);
            if (tier != null)
            {
                return tier.TieredValue;
            }

            return DefaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TieredGroupHandler{T}" /> class with the default tier value.
        /// </summary>
        /// <param name="defaultValue">The default value to use when upgrades are cleared.</param>
        /// <param name="cyclops">The cyclops where the handler is being registered.</param>
        public TieredGroupHandler(T defaultValue, SubRoot cyclops) : base(TechType.None, cyclops)
        {
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Adds a new <see cref="TieredUpgradeHandler{T}" /> to the collection, with all necessary default events created.<para/>
        /// Use this for upgrades where only the highest tier is counted, no matter how many different tiers are equipped.
        /// </summary>
        /// <param name="techType">The TechType of the upgrade module.</param>
        /// <param name="tieredValue">The tiered value this upgrade module represents.</param>
        /// <returns>The newly created <see cref="TieredUpgradeHandler{T}"/> instance.</returns>
        public TieredUpgradeHandler<T> CreateTier(TechType techType, T tieredValue)
        {
            var tieredUpgrade = new TieredUpgradeHandler<T>(techType, tieredValue, this)
            {
                MaxCount = this.MaxCount
            };

            collection.Add(tieredUpgrade);

            return tieredUpgrade;
        }

        internal override void UpgradesCleared()
        {
            if (cleared) // Because this might be called multiple times by the various members of the tier,
                return; // exit if we've already made this call.

            cleared = true;
            finished = false;

            this.HighestValue = DefaultValue;

            OnClearUpgrades?.Invoke();
        }

        internal void TierCounted(T countedValue, Equipment modules, string slot, InventoryItem inventoryItem)
        {
            int comparison = countedValue.CompareTo(this.HighestValue);

            if (comparison > 0)
                this.HighestValue = countedValue;

            OnUpgradeCounted?.Invoke();
            OnUpgradeCountedDetailed?.Invoke(modules, slot, inventoryItem);
        }

        internal override void UpgradesFinished()
        {
            if (finished) // Because this might be called multiple times by the various members of the tier,
                return; // exit if we've already made this call.

            finished = true;
            cleared = false;

            OnFinishedUpgrades?.Invoke();

            CheckIfMaxedOut();
        }

        internal override void RegisterSelf(IDictionary<TechType, UpgradeHandler> dictionary)
        {
            foreach (TieredUpgradeHandler<T> upgrade in collection)
                upgrade.RegisterSelf(dictionary);
        }
    }
}
