namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;

    internal class BatteryBackedCyclopsUpgrade : CyclopsUpgrade
    {
        internal IList<BatteryDetails> Batteries { get; } = new List<BatteryDetails>();

        public BatteryBackedCyclopsUpgrade(TechType techType) : base(techType)
        {
            OnUpgradeCountedBySlot = (Equipment modules, string slot) =>
            {
                this.Batteries.Add(new BatteryDetails(modules, slot, modules.GetItemInSlot(slot).item.GetComponent<Battery>()));
            };
        }
    }
}
