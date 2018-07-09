namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Assets;
    using UnityEngine;

    internal class SolarCharger : CyclopsModule
    {
        internal SolarCharger()
            : base("CyclopsSolarCharger",
                  "Cyclops Solar Charger",
                  "Recharge your Cyclops with the plentiful power of the sun itself.",
                  CraftTree.Type.Workbench,
                  new[] { "CyclopsMenu" },
                  TechType.BaseUpgradeConsole) // This is to make sure the player has access to vehicle solar charging
        {
        }

        public override CyclopsModules ModuleID => CyclopsModules.Solar;

        protected override ModPrefab GetPrefab()
        {
            return new SolarChargerPreFab(NameID, TechTypeID);
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[4]
                             {
                                 new Ingredient(TechType.AdvancedWiringKit, 1),
                                 new Ingredient(TechType.EnameledGlass, 1),
                                 new Ingredient(TechType.Quartz, 2),
                                 new Ingredient(TechType.Titanium, 2)
                             })
            };
        }


        protected override void SetStaticTechTypeID(TechType techTypeID)
        {
            SolarChargerID = techTypeID;
        }

        internal class SolarChargerPreFab : ModPrefab
        {
            internal SolarChargerPreFab(string classId, TechType techType) : base(classId, $"{classId}PreFab", techType)
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
