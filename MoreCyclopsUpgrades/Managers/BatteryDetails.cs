namespace MoreCyclopsUpgrades.Managers
{
    internal class BatteryDetails
    {
        internal readonly Equipment ParentEquipment;
        internal readonly string SlotName;
        internal readonly Battery BatteryRef;

        public BatteryDetails(Equipment parentEquipment, string slotName, Battery batteryRef)
        {
            ParentEquipment = parentEquipment;
            SlotName = slotName;
            BatteryRef = batteryRef;
        }
    }
}
