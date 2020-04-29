namespace MoreCyclopsUpgrades.API.Buildables
{
    /// <summary>
    /// A struct for quick access to the details of an upgrade console slot.
    /// </summary>
    public struct UpgradeSlot
    {
        /// <summary>
        /// The reference to the parent <see cref="Equipment"/> that houses this upgrade slot.
        /// </summary>
        public readonly Equipment equipment;

        /// <summary>
        /// The upgrade slot name.
        /// </summary>
        public readonly string slotName;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradeSlot"/> struct.
        /// </summary>
        /// <param name="parent">The parent equipment.</param>
        /// <param name="name">The slot name.</param>
        public UpgradeSlot(Equipment parent, string name)
        {
            equipment = parent;
            slotName = name;
        }

        /// <summary>
        /// Gets the TechType value in this slot.
        /// </summary>
        /// <returns>
        ///     The <see cref="TechType"/> value corresponding to the item in the slot if one is present;
        ///     Otherwise returns <see cref="TechType.None"/> if the slot is empty.
        /// </returns>
        public TechType GetTechTypeInSlot()
        {
            return equipment.GetTechTypeInSlot(slotName);
        }

        /// <summary>
        /// Gets the inventory item in slot.
        /// </summary>
        /// <returns>
        ///     The <see cref="InventoryItem"/> reference of the item in the slot if one is present;
        ///     Otherwise returns <c>null</c>.
        /// </returns>
        public InventoryItem GetItemInSlot()
        {
            return equipment.GetItemInSlot(slotName);
        }

        /// <summary>
        /// Determines whether the slot is empty.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this slot is empty; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSlotEmpty()
        {
            return equipment.GetTechTypeInSlot(slotName) == TechType.None;
        }

        /// <summary>
        /// Determines whether there the slot has as item.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if there is an item in this slot; otherwise, <c>false</c>.
        /// </returns>
        public bool HasItemInSlot()
        {
            return equipment.GetTechTypeInSlot(slotName) != TechType.None;
        }
    }
}
