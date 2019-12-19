namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;

    internal class HullArmorMk4 : VehicleUpgradeModule
    {
        public HullArmorMk4()
            : base(classId: "HullArmorMk4",
                friendlyName: "Hull Reinforcement Mk IV",
                description: "The best hull upgrade. Equivalent to 4 regular Hull Reinforcements")
        {
            OnFinishedPatching += () => { VehicleUpgrader.SetNewModule(this, true); };
        }

        protected override TechData GetBlueprintRecipe() => new TechData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>(new Ingredient[1]
            {
                new Ingredient(TechType.VehicleArmorPlating, 4)
            })
        };
    }
}