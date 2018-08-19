namespace MoreCyclopsUpgrades.Modules.Enhancement
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
    using UnityEngine;

    internal class CyclopsSpeedBooster : CyclopsModule
    {
        internal CyclopsSpeedBooster(bool fabModPresent) : this(fabModPresent ? null : new[] { "CyclopsMenu" })
        {
        }

        private CyclopsSpeedBooster(string[] tabs)
            : base("CyclopsSpeedModule",
                  "Cyclops Speed Boost Module",
                  "Allows the cyclops engines to go into overdrive, adding greater speeds but at the cost of higher energy consumption rates.",
                  CraftTree.Type.CyclopsFabricator,
                  tabs,
                  TechType.CyclopsHullModule1)
        {
        }

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.CyclopsThermalReactorModule);
            var obj = GameObject.Instantiate(prefab);

            return obj;
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[3]
                             {
                                 new Ingredient(TechType.Aerogel, 1),
                                 new Ingredient(TechType.Magnetite, 2),
                                 new Ingredient(TechType.ComputerChip, 1),
                             })
            };
        }

        protected override void SetStaticTechTypeID(TechType techTypeID) => SpeedBoosterModuleID = techTypeID;
    }
}
