namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    internal class BatteryDetails
    {
        internal readonly Equipment ParentEquipment;
        internal readonly string SlotName;
        internal readonly Battery BatteryRef;

        internal bool IsFull => BatteryRef._charge == BatteryRef._capacity;
        internal bool HasCharge => BatteryRef._charge == 0f;

        public BatteryDetails(Equipment parentEquipment, string slotName, Battery batteryRef)
        {
            ParentEquipment = parentEquipment;
            SlotName = slotName;
            BatteryRef = batteryRef;
        }
    }
}
