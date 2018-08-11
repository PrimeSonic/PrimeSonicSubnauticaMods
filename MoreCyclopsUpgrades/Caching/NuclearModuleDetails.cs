namespace MoreCyclopsUpgrades.Caching
{
    internal class NuclearModuleDetails
    {
        internal readonly Equipment ParentModule;
        internal readonly string SlotName;
        internal readonly Battery NuclearBattery;

        public NuclearModuleDetails(Equipment parentModule, string slotName, Battery nuclearBattery)
        {
            ParentModule = parentModule;
            SlotName = slotName;
            NuclearBattery = nuclearBattery;
        }
    }
}
