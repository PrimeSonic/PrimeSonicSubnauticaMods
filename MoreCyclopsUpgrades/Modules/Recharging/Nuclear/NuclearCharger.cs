namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Assets;
    using UnityEngine;

    internal class NuclearCharger : CyclopsModule
    {
        internal NuclearCharger()
            : base("CyclopsNuclearModule",
                  "Cyclops Nuclear Reactor Module",
                  "Recharge your Cyclops using this portable nuclear reactor. Intelligently provides power only when you need it.",
                  CraftTree.Type.Workbench, // TODO Custom fabricator for all that is Cyclops and nuclear
                  new[] { "CyclopsMenu" },
                  TechType.BaseNuclearReactor)
        {
        }

        public override CyclopsModules ModuleID => CyclopsModules.Nuclear;

        protected override ModPrefab GetPrefab()
        {
            return new NuclearChargerPreFab(NameID, TechTypeID);
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[5]
                             {
                                 new Ingredient(TechType.ReactorRod, 1), // This is to validate that the player has access to nuclear power already
                                 new Ingredient(TechType.Benzene, 1), // And this is the validate that they've gone a little further down
                                 new Ingredient(TechType.Lead, 2), // Extra insulation
                                 new Ingredient(TechType.AdvancedWiringKit, 1), // All the smarts
                                 new Ingredient(TechType.PlasteelIngot, 1) // Housing
                             })
            };
        }

        protected override void SetStaticTechTypeID(TechType techTypeID)
        {
            NuclearChargerID = techTypeID;
        }

        internal class NuclearChargerPreFab : ModPrefab
        {
            internal NuclearChargerPreFab(string classId, TechType techType) : base(classId, $"{classId}PreFab", techType)
            {
            }

            public override GameObject GetGameObject()
            {
                GameObject prefab = CraftData.GetPrefabForTechType(TechType.CyclopsThermalReactorModule);
                GameObject obj = GameObject.Instantiate(prefab);

                // The battery component makes it easy to track the charge and saving the data is automatic.
                var pCell = obj.AddComponent<Battery>();
                pCell.name = "NuclearBattery";
                pCell._capacity = NuclearChargingManager.MaxCharge;

                return obj;
            }
        }
    }
}
