namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;

    internal class HullArmorMk2 : VehicleUpgradeModule
    {
        public HullArmorMk2()
            : base(classId: "HullArmorMk2",
                friendlyName: "Hull Reinforcement Mk II",
                description: "A better hull upgrade. Equivalent to 2 regular Hull Reinforcements")
        {
            OnFinishedPatching += () => { VehicleUpgrader.SetNewModule(this, true); };
        }

        protected override TechData GetBlueprintRecipe() => new TechData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>(new Ingredient[1]
            {
                new Ingredient(TechType.VehicleArmorPlating, 2)
            })
        };
    }
}