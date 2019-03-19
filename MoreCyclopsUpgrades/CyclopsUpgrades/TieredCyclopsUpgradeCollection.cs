namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using System;
    using System.Collections.Generic;

    public class TieredCyclopsUpgradeCollection<T> where T : IComparable<T>
    {
        public ICollection<TieredCyclopsUpgrade<T>> Collection { get; } = new List<TieredCyclopsUpgrade<T>>();

        public T BestValue { get; set; }
        public readonly T DefaultValue;
        private bool finished = true;

        public TieredCyclopsUpgradeCollection(T defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public UpgradeEvent OnFinishedUpgrades;
        public UpgradeEvent OnClearUpgrades;

        public void CreateTier(TechType techType, T tieredValue)
        {
            var tieredUpgrade = new TieredCyclopsUpgrade<T>(techType, tieredValue, this);
            this.Collection.Add(tieredUpgrade);
        }

        internal void CreateTiers(IDictionary<TechType, T> collection)
        {
            foreach (KeyValuePair<TechType, T> upgrade in collection)            
                CreateTier(upgrade.Key, upgrade.Value);
        }

        internal void UpgradesCleared(SubRoot cyclops)
        {
            if (!finished)
                return;

            finished = false;
            this.BestValue = DefaultValue;
            OnClearUpgrades?.Invoke(cyclops);
        }

        internal void TierCounted(TieredCyclopsUpgrade<T> counted)
        {
            int comparison = counted.TieredValue.CompareTo(this.BestValue);

            if (comparison > 0)
                this.BestValue = counted.TieredValue;
        }

        internal void UpgradesFinished(SubRoot cyclops)
        {
            if (finished)
                return;

            OnFinishedUpgrades?.Invoke(cyclops);
            finished = true;
        }
    }
}
