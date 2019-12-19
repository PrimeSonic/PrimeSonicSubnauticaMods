namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;

    internal class HullArmorMk3 : VehicleUpgradeModule
    {
        private const int ArmorCount = 3;
        public HullArmorMk3()
            : base(classId: "HullArmorMk3",
                friendlyName: "Hull Reinforcement Mk III",
                description: "An even better hull upgrade. Equivalent to 3 regular Hull Reinforcements")
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
                    new Ingredient(TechType.VehicleArmorPlating, ArmorCount)
                }
            };
        }
    }
}