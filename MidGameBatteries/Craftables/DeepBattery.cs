namespace MidGameBatteries.Craftables
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;

    internal class DeepBattery : DeepBatteryCellBase
    {
        // This battery provides 2.5x the power of a normal battery
        internal static float BatteryCapacity { get; private set; } = 250f;

        public DeepBattery(float capacity = 250f)
            : base(classId: "DeepBattery",
                   friendlyName: "Deep Battery",
                   description: "A longer lasting battery created from rare materials and stronger chemicals.")
        {
            BatteryCapacity = capacity;
            // This event will be invoked after all patching done by the Craftable class is complete
            OnFinishedPatching += SetStaticTechType;
        }

        protected override TechType BaseType { get; } = TechType.LithiumIonBattery;
        protected override float PowerCapacity => BatteryCapacity;
        protected override EquipmentType ChargerType { get; } = EquipmentType.BatteryCharger;

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(4)
                {
                    new Ingredient(TechType.WhiteMushroom, 3), // Have you gone deep enough?
                    new Ingredient(TechType.Lithium, 1), // For flavor with the old LithiumIonBattery
                    new Ingredient(TechType.AluminumOxide, 1), // Mid-game resource
                    new Ingredient(TechType.Magnetite, 1), // Mid-game resource
                }
            };
        }

        private void SetStaticTechType()
        {
            BatteryID = this.TechType;
        }
    }
}
