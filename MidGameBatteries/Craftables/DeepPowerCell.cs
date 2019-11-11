namespace MidGameBatteries.Craftables
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;

    internal class DeepPowerCell : DeepBatteryCellBase
    {
        // Just like all the other power cells, this is a combination of 2 batteries
        internal const int BatteriesPerPowerCell = 2;

        public DeepPowerCell(DeepBattery lithiumBattery)
            : base(classId: "DeepPowerCell",
                   friendlyName: "Deep Power Cell",
                   description: "A longer lasting power cell created from rare materials and stronger chemicals.")
        {
            // Because the DeepLithiumPowerCell is dependent on the DeepLithiumBattery regarding its blueprint,
            // we'll go ahead and add this little safety check here
            if (!lithiumBattery.IsPatched)
                lithiumBattery.Patch();

            // This event will be invoked after all patching done by the Craftable class is complete
            OnFinishedPatching += SetStaticTechType;
        }

        protected override TechType BaseType { get; } = TechType.PowerCell;
        protected override float PowerCapacity { get; } = DeepBattery.BatteryCapacity * BatteriesPerPowerCell;
        protected override EquipmentType ChargerType { get; } = EquipmentType.PowerCellCharger;

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                // This is just like all the other Power Cell blueprints
                craftAmount = 1,
                Ingredients = new List<Ingredient>(2)
                {
                    new Ingredient(BatteryID, BatteriesPerPowerCell),
                    new Ingredient(TechType.Silicone, 1),
                }
            };
        }

        private void SetStaticTechType()
        {
            PowerCellID = this.TechType;
        }
    }
}