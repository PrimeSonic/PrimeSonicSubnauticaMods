namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;

    internal class HullArmorMk4 : VehicleUpgradeModule
    {
        private const int ArmorCount = 4;
        public HullArmorMk4()
            : base(classId: "HullArmorMk4",
                friendlyName: "Hull Reinforcement Mk IV",
                description: "The best hull upgrade. Equivalent to 4 regular Hull Reinforcements")
        {
            OnFinishedPatching += () =>
            {
                VehicleUpgrader.CommonUpgradeModules.Add(this.TechType);
                VehicleUpgrader.ArmorPlatingModules.Add(this.TechType, ArmorCount);
            };
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(TechType.VehicleArmorPlating, 4)
                }
            };
        }
    }
}