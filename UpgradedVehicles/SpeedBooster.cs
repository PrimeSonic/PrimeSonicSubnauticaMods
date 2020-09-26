namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
#if SUBNAUTICA
    using RecipeData = SMLHelper.V2.Crafting.TechData;
    using Sprite = Atlas.Sprite;
#endif

    internal class SpeedBooster : VehicleUpgradeModule
    {
        public SpeedBooster()
            : base(classId: "SpeedModule",
                friendlyName: "Speed Boost Module",
                description: "Allows small vehicle engines to go into overdrive, adding greater speeds but at the cost of higher energy consumption rates.")
        {
            OnFinishedPatching += () =>
            {
                VehicleUpgrader.CommonUpgradeModules.Add(this.TechType);
                VehicleUpgrader.SpeedBoostingModule = this.TechType;
            };
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(TechType.Aerogel, 1),
                    new Ingredient(TechType.Magnetite, 1),
                    new Ingredient(TechType.Titanium, 2),
                }
            };
        }
    }
}