namespace MidGameBatteries.Craftables
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;

    internal class DeepLithiumBattery : DeepLithiumBase
    {
        internal const float BatteryCapacity = 250f;

        public DeepLithiumBattery()
            : base(classId: "DeepLithiumBattery",
                   friendlyName: "Deep Lithium Battery",
                   description: "A stronger battery created from rare materials.")
        {
            OnFinishedPatching += SetStaticTechType;
        }

        protected override TechType BaseType { get; } = TechType.Battery;
        protected override float PowerCapacity { get; } = BatteryCapacity;
        protected override EquipmentType ChargerType { get; } = EquipmentType.BatteryCharger;

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(3)
                {
                    new Ingredient(TechType.WhiteMushroom, 3),
                    new Ingredient(TechType.Lithium, 1),
                    new Ingredient(TechType.Magnetite, 1),
                }
            };
        }

        private void SetStaticTechType() => BatteryID = this.TechType;
    }
}
