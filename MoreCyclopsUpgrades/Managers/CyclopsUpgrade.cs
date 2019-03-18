namespace MoreCyclopsUpgrades.Managers
{
    public class CyclopsUpgrade
    {
        public readonly TechType techType;

        internal bool IsPowerProducer = false;
        public int Count { get; private set; } = 0;

        public delegate void OnClearUpgradesEvent();
        public OnClearUpgradesEvent OnClearUpgrades;

        public delegate void OnClearUpgradesCyclopsEvent(SubRoot cyclops);
        public OnClearUpgradesCyclopsEvent OnClearUpgradesCyclops;

        public delegate void OnUpgradeCountedSimpleEvent();
        public OnUpgradeCountedSimpleEvent OnUpgradeCounted;

        public delegate void OnUpgradeCountedSlotEvent(Equipment modules, string slot);
        public OnUpgradeCountedSlotEvent OnUpgradeCountedBySlot;

        public delegate void OnUpgradeCountedCyclopsEvent(SubRoot cyclops);
        public OnUpgradeCountedCyclopsEvent OnUpgradeCountedByCyclops;

        public delegate void OnFinishedUpgradesEvent();
        public OnFinishedUpgradesEvent OnFinishedUpgrades;

        public delegate void OnFinishedUpgradesCyclopsEvent(SubRoot cyclops);
        public OnFinishedUpgradesCyclopsEvent OnFinishedUpgradesByCyclops;

        public CyclopsUpgrade(TechType techType)
        {
            this.techType = techType;
        }

        internal void UpgradesCleared(SubRoot cyclops)
        {
            OnClearUpgrades?.Invoke();
            OnClearUpgradesCyclops?.Invoke(cyclops);
            this.Count = 0;
        }

        internal void UpgradeCounted(SubRoot cyclops, Equipment modules, string slot)
        {
            OnUpgradeCounted?.Invoke();
            OnUpgradeCountedBySlot?.Invoke(modules, slot);
            OnUpgradeCountedByCyclops?.Invoke(cyclops);
            this.Count++;
        }

        internal void UpgradesFinished(SubRoot cyclops)
        {
            OnFinishedUpgrades?.Invoke();
            OnFinishedUpgradesByCyclops?.Invoke(cyclops);
        }
    }
}
