namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;

    internal class HullArmorMk4 : VehicleUpgradeModule
    {
        private const int ArmorCount = 4;
        private readonly TechType HullArmorMk3;
        public HullArmorMk4(TechType hullArmorMk3)
            : base(classId: "HullArmorMk4",
                friendlyName: "Hull Reinforcement Mk IV",
                description: "The best hull upgrade. Equivalent to 4 regular Hull Reinforcements")
        {
            this.HullArmorMk3 = hullArmorMk3;
            OnFinishedPatching += () =>
            {
                VehicleUpgrader.CommonUpgradeModules.Add(this.TechType);
                VehicleUpgrader.ArmorPlatingModules.Add(this.TechType, ArmorCount);
            };
        }
        
        public override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;
        public override string[] StepsToFabricatorTab => new[] { "SeamothMenu" };

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(HullArmorMk3, 1),
                    new Ingredient(TechType.Titanium, ArmorCount),
                }
            };
        }
    }
}