namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
    using UnityEngine;

    internal class PowerUpgradeMk3 : CyclopsModule
    {
        internal PowerUpgradeMk3()
            : base("PowerUpgradeModuleMk3",
                  "Cyclops Engine Efficiency Module MK3",
                  "Maximum engine efficiency. Silent running, Sonar, and Shield greatly optimized. Does not stack.",
                  CraftTree.Type.Workbench,
                  new[] { "CyclopsMenu" },
                  TechType.Workbench)
        {
        }

        public override ModuleTypes ModuleID => ModuleTypes.PowerMk3;

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
                                 new Ingredient(PowerUpgradeMk2ID, 1),
                                 new Ingredient(TechType.Kyanite, 1), // More uses for Kyanite!
                                 new Ingredient(TechType.Diamond, 1),
                             })
            };
        }

        protected override void SetStaticTechTypeID(TechType techTypeID)
        {
            PowerUpgradeMk3ID = techTypeID;
        }        
    }
}
