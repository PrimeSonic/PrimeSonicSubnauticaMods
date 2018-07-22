namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using Common;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal class SeaMothMk3 : ModPrefab
    {
        public static TechType TechTypeID { get; private set; } = TechType.Unobtanium; // Default for when not set but still used in comparisons
        public static TechType SeamothHullModule4 { get; private set; } = TechType.Unobtanium;
        public static TechType SeamothHullModule5 { get; private set; } = TechType.Unobtanium;

        public readonly TechType PowerCoreID;

        public const string NameID = "SeaMothMk3";
        public const string FriendlyName = "Seamoth Mk3";
        public const string Description = "The highest end SeaMoth. Ready for adventures in the most hostile environments.";

        internal SeaMothMk3(TechType vehiclePowerCore, TechType seamothHullModule4, TechType seamothHullModule5) : base(NameID, $"{NameID}Prefab")
        {
            PowerCoreID = vehiclePowerCore;
            SeamothHullModule4 = seamothHullModule4;
            SeamothHullModule5 = seamothHullModule5;
        }

        public void Patch()
        {
            this.TechType = TechTypeHandler.AddTechType(NameID,
                                         FriendlyName,
                                         Description,
                                         ImageUtils.LoadSpriteFromFile(@"./QMods/UpgradedVehicles/Assets/SeamothMk3.png"),
                                         false);

            TechTypeID = this.TechType;

            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Constructor, TechTypeID, "Vehicles");
            CraftDataHandler.SetCraftingTime(TechTypeID, 15f);
            CraftDataHandler.SetTechData(TechTypeID, GetRecipe());

            PrefabHandler.RegisterPrefab(this);
            KnownTechHandler.SetAnalysisTechEntry(SeamothHullModule5, new TechType[1] { TechTypeID }, $"{FriendlyName} blueprint discovered!");
            CraftDataHandler.AddToGroup(TechGroup.Constructor, TechCategory.Constructor, TechTypeID);
        }

        private TechData GetRecipe()
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
                                 new Ingredient(PowerCoreID, 1), // armor and speed without engine efficiency penalty
                             })
            };
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
