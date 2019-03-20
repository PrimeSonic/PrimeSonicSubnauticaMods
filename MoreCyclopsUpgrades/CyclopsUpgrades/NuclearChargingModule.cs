namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using MoreCyclopsUpgrades.Modules;
    using UnityEngine;

    internal class NuclearChargingModule : BatteryCyclopsUpgrade
    {
        public NuclearChargingModule() : base(CyclopsModule.NuclearChargerID, canRecharge: false)
        {
            OnBatteryDrained += (BatteryDetails details) =>
            {
                this.TotalBatteryCapacity -= details.BatteryRef._capacity;

                Equipment modules = details.ParentEquipment;
                string slotName = details.SlotName;
                // Drained nuclear batteries are handled just like how the Nuclear Reactor handles depleated reactor rods
                InventoryItem inventoryItem = modules.RemoveItem(slotName, true, false);
                GameObject.Destroy(inventoryItem.item.gameObject);
                modules.AddItem(slotName, CyclopsModule.SpawnCyclopsModule(CyclopsModule.DepletedNuclearModuleID), true);
                ErrorMessage.AddMessage("Nuclear Reactor Module depleted");
            };
        }
    }
}
