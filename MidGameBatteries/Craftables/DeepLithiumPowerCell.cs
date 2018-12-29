namespace MidGameBatteries.Craftables
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;

    internal class DeepLithiumPowerCell : DeepLithiumBase
    {
        internal const int BatteriesPerPowerCell = 2;

        public DeepLithiumPowerCell(DeepLithiumBattery lithiumBattery)
            : base(classId: "DeepLithiumPowerCell",
                   friendlyName: "Deep Lithium Power Cell",
                   description: "A stronger power cell created from rare materials.")
        {
            if (!lithiumBattery.IsPatched)
                lithiumBattery.Patch();

            OnFinishedPatching += SetStaticTechType;
        }

        protected override TechType BaseType { get; } = TechType.PowerCell;
        protected override float PowerCapacity { get; } = DeepLithiumBattery.BatteryCapacity * BatteriesPerPowerCell;
        protected override EquipmentType ChargerType { get; } = EquipmentType.PowerCellCharger;

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(2)
                {
                    new Ingredient(BatteryID, BatteriesPerPowerCell),
                    new Ingredient(TechType.Silicone, 1),
                }
            };
        }

        private void SetStaticTechType() => PowerCellID = this.TechType;
    }
}