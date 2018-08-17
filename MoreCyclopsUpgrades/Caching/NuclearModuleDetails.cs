namespace MoreCyclopsUpgrades.Caching
{
    internal class NuclearModuleDetails
    {
        internal readonly Equipment ParentEquipment;
        internal readonly string SlotName;
        internal readonly Battery NuclearBattery;

        public NuclearModuleDetails(Equipment parentModule, string slotName, Battery nuclearBattery)
        {
            ParentEquipment = parentModule;
            SlotName = slotName;
            NuclearBattery = nuclearBattery;
        }
    }
}
