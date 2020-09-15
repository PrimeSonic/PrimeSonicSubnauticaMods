namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
#if SUBNAUTICA
    using RecipeData = SMLHelper.V2.Crafting.TechData;
#endif

    internal class HullArmorMk2 : VehicleUpgradeModule
    {
        private const int ArmorCount = 2;
        public HullArmorMk2()
            : base(classId: "HullArmorMk2",
                friendlyName: "Hull Reinforcement Mk II",
                description: "An upgrade containing nanites improving and maintaining the inner structure of the hull.\nEquivalent to 2 regular Hull Reinforcements")
        {
            OnFinishedPatching += () =>
            {
                VehicleUpgrader.CommonUpgradeModules.Add(this.TechType);
                VehicleUpgrader.ArmorPlatingModules.Add(this.TechType, ArmorCount);
            };
        }

        public override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;
        public override string[] StepsToFabricatorTab => new[] { QPatch.WorkBenchTab };

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(TechType.VehicleArmorPlating, 1),
                    new Ingredient(TechType.Titanium, 2),
                    new Ingredient(TechType.Lead, 1),
                    new Ingredient(TechType.ComputerChip, 1)
                }
            };
        }
    }
}