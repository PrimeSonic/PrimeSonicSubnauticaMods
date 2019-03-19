namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using System;

    public class CyclopsUpgrade
    {
        public readonly TechType techType;

        private int count = 0;

        internal bool IsPowerProducer = false;

        public int Count => Math.Min(this.MaxCount, count);

        public int MaxCount { get; set; } = 99;

        public bool MaxLimitReached => count == this.MaxCount;

        public delegate void UpgradeEvent(SubRoot cyclops);
        public delegate void UpgradeEventSlotBound(SubRoot cyclops, Equipment modules, string slot);

        public UpgradeEvent OnClearUpgrades;
        public UpgradeEventSlotBound OnUpgradeCounted;
        public UpgradeEvent OnFinishedUpgrades;

        public CyclopsUpgrade(TechType techType)
        {
            this.techType = techType;
        }

        internal void UpgradesCleared(SubRoot cyclops)
        {
            OnClearUpgrades?.Invoke(cyclops);
            count = 0;
        }

        internal void UpgradeCounted(SubRoot cyclops, Equipment modules, string slot)
        {
            count++;
            OnUpgradeCounted?.Invoke(cyclops, modules, slot);
        }

        internal void UpgradesFinished(SubRoot cyclops)
        {
            OnFinishedUpgrades?.Invoke(cyclops);
        }
    }
}
