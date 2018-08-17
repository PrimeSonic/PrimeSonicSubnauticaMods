namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
    using UnityEngine;

    internal class PowerUpgradeMk2 : CyclopsModule
    {
        internal PowerUpgradeMk2()
            : base("PowerUpgradeModuleMk2",
                  "Cyclops Engine Efficiency Module MK2",
                  "Additional enhancement to engine efficiency. Silent running, Sonar, and Shield optimized. Does not stack.",
                  CraftTree.Type.Workbench,
                  new[] { "CyclopsMenu" },
                  TechType.Workbench)
        {

        }

        public override ModuleTypes ModuleID => ModuleTypes.PowerMk2;

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.CyclopsThermalReactorModule);
            GameObject obj = GameObject.Instantiate(prefab);

            return obj;
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[3]
                             {
                                 new Ingredient(TechType.PowerUpgradeModule, 1),
                                 new Ingredient(TechType.Aerogel, 1),
                                 new Ingredient(TechType.Sulphur, 2) // Did you make it to the Lost River yet?
                             })
            };
        }

        protected override void SetStaticTechTypeID(TechType techTypeID)
        {
            PowerUpgradeMk2ID = techTypeID;
        }
    }
}
