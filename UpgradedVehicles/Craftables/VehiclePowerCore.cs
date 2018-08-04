namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class VehiclePowerCore : Craftable
    {
        protected readonly SpeedBooster SpeedBoosterModule;

        internal VehiclePowerCore(SpeedBooster speedBoostModule)
             : base(nameID: "VehiclePowerCore",
                  friendlyName: "Vehicle Power Core",
                  description: "A modified power core for constructing upgraded vehicles. Enables permanent enhancements without use of external upgrade modules.",
                  template: TechType.PrecursorIonPowerCell,
                  fabricatorType: CraftTree.Type.SeamothUpgrades,
                  fabricatorTab: "CommonModules",
                  requiredAnalysis: TechType.BaseUpgradeConsole,
                  groupForPDA: TechGroup.Resources,
                  categoryForPDA: TechCategory.Electronics,
                  prerequisite: speedBoostModule)
        {
            SpeedBoosterModule = speedBoostModule;
        }

        protected override void PostPatch()
        {
            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.None);
            MTechType.VehiclePowerCore = this.TechType;
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[6]
                             {
                                 new Ingredient(TechType.Benzene, 1),
                                 new Ingredient(TechType.Lead, 2),
                                 new Ingredient(TechType.PowerCell, 1),

                                 new Ingredient(TechType.VehiclePowerUpgradeModule, 2), // Engine eficiency
                                 new Ingredient(TechType.VehicleArmorPlating, 2), // Armor
                                 new Ingredient(SpeedBoosterModule.TechType, 2), // Speed boost
                             })
            };
        }

        public override GameObject GetGameObject()
        {
            var obj = base.GetGameObject();

            GameObject.DestroyImmediate(obj.GetComponent<Battery>());

            return obj;
        }
    }
}
