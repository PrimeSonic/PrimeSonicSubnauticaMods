namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;

    internal class HullArmorMk3 : VehicleUpgradeModule
    {
        public HullArmorMk3()
            : base(classId: "HullArmorMk3",
                friendlyName: "Hull Reinforcement Mk III",
                description: "An even better hull upgrade. Equivalent to 3 regular Hull Reinforcements")
        {
            OnFinishedPatching += () => { VehicleUpgrader.SetNewModule(this, true); };
        }

        protected override TechData GetBlueprintRecipe() => new TechData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>(new Ingredient[1]
            {
                new Ingredient(TechType.VehicleArmorPlating, 3)
            })
        };
    }
}