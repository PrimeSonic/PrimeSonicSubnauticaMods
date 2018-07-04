namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using Common;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class SeaMothMk3
    {
        public static TechType TechTypeID { get; private set; } = TechType.Unobtanium; // Default for when not set but still used in comparisons
        public static TechType SeamothHullModule4 { get; private set; } = TechType.Unobtanium;
        public static TechType SeamothHullModule5 { get; private set; } = TechType.Unobtanium;

        public const string NameID = "SeaMothMk3";
        public const string FriendlyName = "Seamoth Mk3";
        public const string Description = "The highest end SeaMoth. Ready for adventures in the most hostile environments.";

        public static void Patch(TechType seamothHullModule4, TechType seamothHullModule5)
        {
            SeamothHullModule4 = seamothHullModule4;
            SeamothHullModule5 = seamothHullModule5;

            TechTypeID = TechTypeHandler.AddTechType(NameID, FriendlyName, Description);

            SpriteHandler.RegisterSprite(TechTypeID, @"./QMods/UpgradedVehicles/Assets/SeamothMk3.png");

            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Constructor, TechTypeID, "Vehicles");
            CraftDataHandler.SetCraftingTime(TechTypeID, 20f);
            CraftDataHandler.SetTechData(TechTypeID, GetRecipe(seamothHullModule5));

            PrefabHandler.RegisterPrefab(new SeaMothMk3Prefab(TechTypeID, NameID));
            KnownTechHandler.SetAnalysisTechEntry(seamothHullModule5, new List<TechType>(1) { TechTypeID }, $"{FriendlyName} blueprint discovered!");
        }

        private static TechData GetRecipe(TechType SeamothHullModule5)
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[5]
                             {
                                 new Ingredient(TechType.PlasteelIngot, 1), // Stronger than titanium ingot                                 
                                 new Ingredient(TechType.EnameledGlass, 2), // Stronger than glass
                                 new Ingredient(TechType.Lead, 1),

                                 new Ingredient(SeamothHullModule5, 1), // Minimum crush depth of 1700 without upgrades
                                 new Ingredient(VehiclePowerCore.TechTypeID, 1), // armor and speed without engine efficiency penalty
                             })
            };
        }

        internal class SeaMothMk3Prefab : ModPrefab
        {
            internal SeaMothMk3Prefab(TechType techtype, string nameID) : base(nameID, $"{nameID}Prefab", techtype)
            {
            }

            public override GameObject GetGameObject()
            {
                GameObject seamothPrefab = Resources.Load<GameObject>("WorldEntities/Tools/SeaMoth");
                GameObject obj = GameObject.Instantiate(seamothPrefab);

                var seamoth = obj.GetComponent<SeaMoth>();

                var life = seamoth.GetComponent<LiveMixin>();

                LiveMixinData lifeData = ScriptableObject.CreateInstance<LiveMixinData>();

                life.data.CloneFieldsInto(lifeData);
                lifeData.maxHealth = life.maxHealth * 2.15f; // 115% more HP

                life.data = lifeData;
                life.health = life.data.maxHealth;
                lifeData.weldable = true;

                var crush = obj.GetComponent<CrushDamage>();
                crush.vehicle = seamoth;
                crush.liveMixin = life;

                // Always on upgrades handled in OnUpgradeModuleChange patch

                return obj;
            }
        }
    }
}
