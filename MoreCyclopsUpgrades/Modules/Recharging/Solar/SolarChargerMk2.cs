namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Assets;
    using UnityEngine;

    internal class SolarChargerMk2 : CyclopsModule
    {
        internal SolarChargerMk2()
            : base("CyclopsSolarChargerMk2",
                  "Cyclops Solar Charger Mk2",
                  "Improved solar charging and with integrated batteries to store a little extra power for when you can't see the sun.",
                  CraftTree.Type.Workbench,
                  new[] { "CyclopsMenu" },
                  TechType.Workbench)
        {            
        }

        public override CyclopsModules ModuleID => CyclopsModules.SolarMk2;

        protected override ModPrefab GetPrefab()
        {
            return new SolarChargerMk2PreFab(NameID, TechTypeID);
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[6]
                             {
                                 new Ingredient(SolarChargerID, 1),
                                 new Ingredient(TechType.Battery, 2),
                                 new Ingredient(TechType.Sulphur, 1),
                                 new Ingredient(TechType.Kyanite, 1),
                                 new Ingredient(TechType.WiringKit, 1),
                                 new Ingredient(TechType.CopperWire, 1),
                             })
            };
        }

        protected override void SetStaticTechTypeID(TechType techTypeID)
        {
            SolarChargerMk2ID = techTypeID;
        }

        internal class SolarChargerMk2PreFab : ModPrefab
        {
            internal SolarChargerMk2PreFab(string classId, TechType techType) : base(classId, $"{classId}PreFab", techType)
            {
            }

            public override GameObject GetGameObject()
            {
                GameObject prefab = CraftData.GetPrefabForTechType(TechType.CyclopsThermalReactorModule);
                GameObject obj = Object.Instantiate(prefab);

                var pCell = obj.AddComponent<Battery>();
                pCell.name = "SolarBackupBattery";
                pCell._capacity = PowerManager.MaxMk2Charge;

                return obj;
            }
        }
    }
}
