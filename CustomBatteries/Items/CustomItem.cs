namespace CustomBatteries.Items
{
    using CustomBatteries.API;

    internal class CustomItem : CbCore
    {
        public CustomItem(CbItem packItem, ItemTypes itemType) : base(packItem)
        {
            this.PackItem = packItem;
            this.ItemType = itemType;
        }

        public ItemTypes ItemType { get; }

        public CbItem PackItem { get; }

        protected override TechType PrefabType => ItemType == ItemTypes.Battery ? this.UsingIonCellSkins ? TechType.PrecursorIonBattery : TechType.Battery : this.UsingIonCellSkins ? TechType.PrecursorIonPowerCell : TechType.PowerCell;

        protected override EquipmentType ChargerType => ItemType == ItemTypes.Battery ? EquipmentType.BatteryCharger : EquipmentType.PowerCellCharger;

        protected override string[] StepsToFabricatorTab => ItemType == ItemTypes.Battery ? BatteryCraftPath : PowCellCraftPath;

        protected override void AddToList()
        {
            if (ItemType == ItemTypes.Battery)
                BatteryItems.Add(this);
            else
                PowerCellItems.Add(this);

            TrackItems.Add(this.TechType);
        }
    }
}
