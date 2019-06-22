namespace MoreCyclopsUpgrades.API.Upgrades
{
    public class BatteryDetails
    {
        public readonly Equipment ParentEquipment;
        public readonly string SlotName;
        public readonly Battery BatteryRef;

        public bool IsFull => BatteryRef._charge == BatteryRef._capacity;
        public bool HasCharge => BatteryRef._charge == 0f;

        public BatteryDetails(Equipment parentEquipment, string slotName, Battery batteryRef)
        {
            ParentEquipment = parentEquipment;
            SlotName = slotName;
            BatteryRef = batteryRef;
        }
    }
}
