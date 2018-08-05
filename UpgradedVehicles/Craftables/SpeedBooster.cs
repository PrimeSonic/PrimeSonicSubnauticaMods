namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;

    internal class SpeedBooster : Craftable
    {
        internal SpeedBooster()
            : base(nameID: "SpeedModule",
                  friendlyName: "Speed Boost Module",
                  description: "Allows small vehicle engines to go into overdrive, adding greater speeds but at the cost of higher energy consumption rates.",
                  template: TechType.PowerUpgradeModule,
                  fabricatorType: CraftTree.Type.SeamothUpgrades,
                  fabricatorTab: "CommonModules",
                  requiredAnalysis: TechType.BaseUpgradeConsole,
                  groupForPDA: TechGroup.VehicleUpgrades,
                  categoryForPDA: TechCategory.VehicleUpgrades)
        {
        }

        protected override void PrePatch() { }

        protected override void PostPatch()
        {
            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.VehicleModule);
            MTechType.SpeedBooster = this.TechType;
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[3]
                             {
                                 new Ingredient(TechType.Aerogel, 1),
                                 new Ingredient(TechType.Magnetite, 1),
                                 new Ingredient(TechType.ComputerChip, 1),
                             })
            };
        }
    }
}
