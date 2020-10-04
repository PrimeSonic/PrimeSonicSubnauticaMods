namespace CustomBatteries.Items
{
    using CustomBatteries.API;

    internal class CustomItem : CbCore
    {
        public CustomItem(ICbItem packItem, ItemTypes itemType) : base(packItem)
        {
            this.PackItem = packItem;
            this.ItemType = itemType;
        }

        public ItemTypes ItemType { get; }

        public ICbItem PackItem { get; }

        protected override TechType PrefabType => ItemType == ItemTypes.Battery ? TechType.Battery : TechType.PowerCell;

        protected override EquipmentType ChargerType => ItemType == ItemTypes.Battery ? EquipmentType.BatteryCharger : EquipmentType.PowerCellCharger;

        protected override string[] StepsToFabricatorTab => ItemType == ItemTypes.Battery ? BatteryCraftPath : PowCellCraftPath;

        protected override void AddToList()
        {
            if (ItemType == ItemTypes.Battery)
                BatteryTechTypes.Add(this.TechType);
            else
                PowerCellTechTypes.Add(this.TechType);
        }
    }
}
