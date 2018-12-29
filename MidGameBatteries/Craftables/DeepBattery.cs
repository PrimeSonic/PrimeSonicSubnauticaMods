namespace MidGameBatteries.Craftables
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;

    internal class DeepBattery : DeepBatteryCellBase
    {
        // This battery provides 2.5x the power of a normal battery
        internal const float BatteryCapacity = 250f;

        public DeepBattery()
            : base(classId: "DeepBattery",
                   friendlyName: "Deep Battery",
                   description: "A longer lasting battery created from rare materials and stronger chemicals.")
        {
            // This event will be invoked after all patching done by the Craftable class is complete
            OnFinishedPatching += SetStaticTechType;
        }

        protected override TechType BaseType { get; } = TechType.LithiumIonBattery;
        protected override float PowerCapacity { get; } = BatteryCapacity;
        protected override EquipmentType ChargerType { get; } = EquipmentType.BatteryCharger;

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(3)
                {
                    new Ingredient(TechType.WhiteMushroom, 2), // Have you gone deep enough?
                    new Ingredient(TechType.Lithium, 1), // For flavor with the old LithiumIonBattery
                    new Ingredient(TechType.Nickel, 1), // Have you reached the Lost River?
                }
            };
        }

        private void SetStaticTechType() => BatteryID = this.TechType;
    }
}
