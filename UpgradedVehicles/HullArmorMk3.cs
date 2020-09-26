namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
#if SUBNAUTICA
    using RecipeData = SMLHelper.V2.Crafting.TechData;
#endif

    internal class HullArmorMk3 : VehicleUpgradeModule
    {
        private const int ArmorCount = 3;
        private readonly TechType HullArmorMk2;
        public HullArmorMk3(TechType hullArmorMk2)
            : base(classId: "HullArmorMk3",
                friendlyName: "Hull Reinforcement Mk III",
                description: "An upgrade containing nanites improving and maintaining the inner structure of the hull.\nEquivalent to 3 regular Hull Reinforcements")
        {
            HullArmorMk2 = hullArmorMk2;
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
                    new Ingredient(HullArmorMk2, 1),
                    new Ingredient(TechType.Titanium, 3),
                    new Ingredient(TechType.AluminumOxide, 1),
                    new Ingredient(TechType.ComputerChip, 1)
                }
            };
        }
    }
}