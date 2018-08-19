namespace MoreCyclopsUpgrades.Modules.Recharging.Solar
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
    using UnityEngine;

    internal class SolarCharger : CyclopsModule
    {
        internal SolarCharger(bool fabModPresent) : this(fabModPresent ? null : new[] { "CyclopsMenu" })
        {
        }

        private SolarCharger(string[] tabs)
            : base("CyclopsSolarCharger",
                  "Cyclops Solar Charger",
                  "Recharge your Cyclops with the plentiful power of the sun itself.",
                  CraftTree.Type.CyclopsFabricator,
                  tabs,
                  TechType.Cyclops)
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
                Ingredients = new List<Ingredient>(new Ingredient[4]
                             {
                                 new Ingredient(TechType.AdvancedWiringKit, 1),
                                 new Ingredient(TechType.EnameledGlass, 1),
                                 new Ingredient(TechType.Quartz, 2),
                                 new Ingredient(TechType.Titanium, 2)
                             })
            };
        }

        protected override void SetStaticTechTypeID(TechType techTypeID) => SolarChargerID = techTypeID;
    }

}
