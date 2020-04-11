namespace MoreCyclopsUpgrades.AuxConsole
{
    internal struct UpgradeSlot
    {
        internal readonly Equipment Modules;
        internal readonly string Slot;

        public UpgradeSlot(Equipment modules, string slot)
        {
            Modules = modules;
            Slot = slot;
        }

        public TechType GetTechTypeInSlot()
        {
            return Modules.GetTechTypeInSlot(Slot);
        }

        public InventoryItem GetItemInSlot()
        {
            return Modules.GetItemInSlot(Slot);
        }
    }
}
