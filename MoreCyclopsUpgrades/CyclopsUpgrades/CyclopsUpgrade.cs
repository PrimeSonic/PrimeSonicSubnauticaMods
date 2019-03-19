namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using MoreCyclopsUpgrades.Modules;
    using System;

    public delegate void UpgradeEvent(SubRoot cyclops);
    public delegate void UpgradeEventSlotBound(SubRoot cyclops, Equipment modules, string slot);

    public class CyclopsUpgrade
    {
        public readonly TechType techType;

        private int count = 0;

        internal bool IsPowerProducer = false;

        public int Count => Math.Min(this.MaxCount, count);

        public int MaxCount { get; set; } = 99;

        public bool MaxLimitReached => count == this.MaxCount;

        public bool HasUpgrade => this.Count > 0;

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

        internal virtual void UpgradesFinished(SubRoot cyclops)
        {
            if (count > this.MaxCount)
            {
                ErrorMessage.AddMessage($"Cannot exceed more than {this.MaxCount} {CyclopsModule.CyclopsModulesByTechType[techType].NameID}");
                return;
            }

            OnFinishedUpgrades?.Invoke(cyclops);

            if (count == this.MaxCount)
            {
                ErrorMessage.AddMessage($"Maximum number of {CyclopsModule.CyclopsModulesByTechType[techType].NameID} reached");
                return;
            }
        }
    }
}
