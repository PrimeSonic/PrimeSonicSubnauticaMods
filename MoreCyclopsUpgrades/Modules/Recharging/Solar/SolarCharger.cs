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
                  TechType.SeamothSolarCharge)
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
                                 new Ingredient(TechType.SeamothSolarCharge, 1), // This is to make sure the player has access to vehicle solar charging
                                 new Ingredient(TechType.Quartz, 3),
                                 new Ingredient(TechType.Titanium, 3),
                                 new Ingredient(TechType.CopperWire, 1),
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