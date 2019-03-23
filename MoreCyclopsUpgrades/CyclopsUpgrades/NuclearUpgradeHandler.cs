namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using Modules;
    using Modules.Recharging.Nuclear;
    using UnityEngine;

    internal class NuclearUpgradeHandler : BatteryUpgradeHandler
    {
        public NuclearUpgradeHandler() : base(CyclopsModule.NuclearChargerID, canRecharge: false)
        {
            this.LoggingName = "NuclearCharger";

            OnBatteryDrained += (BatteryDetails details) =>
            {
                this.TotalBatteryCapacity -= details.BatteryRef._capacity;

                Equipment modules = details.ParentEquipment;
                string slotName = details.SlotName;
                // Drained nuclear batteries are handled just like how the Nuclear Reactor handles depleated reactor rods
                InventoryItem inventoryItem = modules.RemoveItem(slotName, true, false);
                GameObject.Destroy(inventoryItem.item.gameObject);
                modules.AddItem(slotName, CyclopsModule.SpawnCyclopsModule(CyclopsModule.DepletedNuclearModuleID), true);
                ErrorMessage.AddMessage(DepletedNuclearModule.DepletedEvent);
            };
        }
    }
}
