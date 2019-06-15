namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the complete tier of <see cref="TieredUpgradeHandler{T}"/> instances.
    /// The events for this collection will be invoked only as few times as needed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="UpgradeHandler" />
    public class TieredUpgradesHandlerCollection<T> : UpgradeHandler where T : IComparable<T>
    {
        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <value>
        /// The collection.
        /// </value>
        private readonly ICollection<TieredUpgradeHandler<T>> collection = new List<TieredUpgradeHandler<T>>();

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

        private bool cleared = false;
        private bool finished = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TieredUpgradesHandlerCollection{T}"/> class with the default tier value.
        /// </summary>
        /// <param name="defaultValue">The default value to use when upgrades are cleared.</param>
        public TieredUpgradesHandlerCollection(T defaultValue, SubRoot cyclops) : base(TechType.None, cyclops)
        {
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Adds a new <see cref="TieredUpgradeHandler{T}" /> to the collection, with all default events created.
        /// </summary>
        /// <param name="techType">The TechType of the upgrade module.</param>
        /// <param name="tieredValue">The tiered value this upgrade module represents.</param>
        /// <returns>THe newly created <see cref="TieredUpgradeHandler{T}"/> instance.</returns>
        public TieredUpgradeHandler<T> CreateTier(TechType techType, T tieredValue)
        {
            var tieredUpgrade = new TieredUpgradeHandler<T>(techType, tieredValue, this);
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

        internal void TierCounted(T countedValue, Equipment modules, string slot)
        {
            int comparison = countedValue.CompareTo(this.HighestValue);

            if (comparison > 0)
                this.HighestValue = countedValue;

            OnUpgradeCounted?.Invoke(modules, slot);
        }

        internal override void UpgradesFinished()
        {
            if (finished) // Because this might be called multiple times by the various members of the tier,
                return; // exit if we've already made this call.

            finished = true;
            cleared = false;

            OnFinishedUpgrades?.Invoke();
        }

        internal override void RegisterSelf(IDictionary<TechType, UpgradeHandler> dictionary)
        {
            foreach (TieredUpgradeHandler<T> upgrade in collection)
                upgrade.RegisterSelf(dictionary);
        }
    }
}
