namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Assets;
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

        public override CyclopsModules ModuleID => CyclopsModules.PowerMk2;

        protected override ModPrefab GetPrefab()
        {
            return new PowerUpgradeMk2PreFab(NameID, TechTypeID);
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

        internal class PowerUpgradeMk2PreFab : ModPrefab
        {
            internal PowerUpgradeMk2PreFab(string classId, TechType techType) : base(classId, $"{classId}PreFab", techType)
            {
            }

            public override GameObject GetGameObject()
            {
                GameObject prefab = CraftData.GetPrefabForTechType(TechType.CyclopsThermalReactorModule);
                GameObject obj = GameObject.Instantiate(prefab);

                return obj;
            }
        }
    }
}
