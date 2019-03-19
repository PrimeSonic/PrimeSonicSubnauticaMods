namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using System;
    using System.Collections.Generic;

    internal class TieredCyclopsUpgradeCollection<T> where T : IComparable<T>
    {
        internal ICollection<TieredCyclopsUpgrade<T>> Collection { get; } = new List<TieredCyclopsUpgrade<T>>();

        internal T BestValue { get; set; }
        internal readonly T DefaultValue;

        public TieredCyclopsUpgradeCollection(T defaultValue)
        {
            DefaultValue = defaultValue;
        }

        internal TieredCyclopsUpgrade<T> Create(TechType techType, T tieredValue)
        {
            var tieredUpgrade = new TieredCyclopsUpgrade<T>(techType, tieredValue, this);
            this.Collection.Add(tieredUpgrade);
            return tieredUpgrade;
        }

        internal void ResetValue()
        {
            this.BestValue = DefaultValue;
        }

        internal void TierCounted(TieredCyclopsUpgrade<T> counted)
        {
            int comparison = counted.TieredValue.CompareTo(this.BestValue);

            if (comparison > 0)
                this.BestValue = counted.TieredValue;
        }
    }
}
