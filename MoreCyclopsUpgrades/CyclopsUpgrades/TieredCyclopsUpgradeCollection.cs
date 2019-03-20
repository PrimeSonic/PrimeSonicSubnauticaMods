namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using Common;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the complete tier of <see cref="TieredCyclopsUpgrade{T}"/> instances.
    /// The events for this collection will be invoked only as few times as needed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="CyclopsUpgrade" />
    public class TieredCyclopsUpgradeCollection<T> : CyclopsUpgrade where T : IComparable<T>
    {
        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <value>
        /// The collection.
        /// </value>
        private readonly ICollection<TieredCyclopsUpgrade<T>> collection = new List<TieredCyclopsUpgrade<T>>();

        /// <summary>
        /// Gets the highest value.
        /// </summary>
        /// <value>
        /// The highest value.
        /// </value>
        public T HighestValue { get; private set; }
        public readonly T DefaultValue;
        private bool finished = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="TieredCyclopsUpgradeCollection{T}"/> class with the default tier value.
        /// </summary>
        /// <param name="defaultValue">The default value to use when upgrades are cleared.</param>
        public TieredCyclopsUpgradeCollection(T defaultValue) : base(TechType.None)
        {
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Adds a new <see cref="TieredCyclopsUpgrade{T}" /> to the collection, with all default events created.
        /// </summary>
        /// <param name="techType">The TechType of the upgrade module.</param>
        /// <param name="tieredValue">The tiered value this upgrade module represents.</param>
        /// <returns>THe newly created <see cref="TieredCyclopsUpgrade{T}"/> instance.</returns>
        public TieredCyclopsUpgrade<T> CreateTier(TechType techType, T tieredValue)
        {
            var tieredUpgrade = new TieredCyclopsUpgrade<T>(techType, tieredValue, this);
            collection.Add(tieredUpgrade);

            return tieredUpgrade;
        }

        internal override void UpgradesCleared(SubRoot cyclops)
        {
            if (!finished)
                return;

            finished = false;
            this.HighestValue = DefaultValue;
            OnClearUpgrades?.Invoke(cyclops);
        }

        internal void TierCounted(T countedValue)
        {
            int comparison = countedValue.CompareTo(this.HighestValue);

            if (comparison > 0)
                this.HighestValue = countedValue;
        }

        internal override void UpgradeCounted(SubRoot cyclops, Equipment modules, string slot)
        {
        }

        internal override void UpgradesFinished(SubRoot cyclops)
        {
            if (finished)
                return;

            OnFinishedUpgrades?.Invoke(cyclops);
            finished = true;
        }

        internal override void RegisterSelf(IDictionary<TechType, CyclopsUpgrade> dictionary)
        {
            foreach (TieredCyclopsUpgrade<T> upgrade in collection)
                upgrade.RegisterSelf(dictionary);
        }
    }
}
