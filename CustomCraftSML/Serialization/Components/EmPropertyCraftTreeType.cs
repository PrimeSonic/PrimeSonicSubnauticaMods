namespace CustomCraft2SML.Serialization.Components
{
    using Common.EasyMarkup;

    internal class EmPropertyCraftTreeType : EmProperty<CraftTree.Type>
    {
        public EmPropertyCraftTreeType(string key, CraftTree.Type defaultValue = CraftTree.Type.Fabricator) : base(key, defaultValue)
        {
        }

        public override CraftTree.Type ConvertFromSerial(string value)
        {
            switch (value)
            {
                case "Fabricator":
                    return CraftTree.Type.Fabricator;
                case "Constructor":
                case "MobileVehicleBay":
                    return CraftTree.Type.Constructor;
                case "Workbench":
                case "ModificationStation":
                    return CraftTree.Type.Workbench;
                case "SeamothUpgrades":
                case "VehicleUpgradeConsole":
                    return CraftTree.Type.SeamothUpgrades;
                case "MapRoom":
                case "ScannerRoom":
                    return CraftTree.Type.MapRoom;
                case "CyclopsFabricator":
                    return CraftTree.Type.CyclopsFabricator;
                default:
                    return CraftTree.Type.None;
            }
        }

        internal override EmProperty Copy()
        {
            if (this.HasValue)
                return new EmPropertyCraftTreeType(this.Key, this.Value);

            return new EmPropertyCraftTreeType(this.Key);
        }
    }
}
