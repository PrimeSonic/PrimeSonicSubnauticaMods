namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using Common;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal class SeaMothMk2 : ModPrefab
    {
        public static TechType TechTypeID { get; private set; }
        public const string NameID = "SeaMothMk2";
        public const string FriendlyName = "Seamoth Mk2";
        public const string Description = "An upgraded SeaMoth, built harder and faster to take you anywhere.";

        internal readonly TechType PowerCoreID;

        internal SeaMothMk2(TechType vehiclePowerCore) : base(NameID, $"{NameID}Prefab")
        {
            PowerCoreID = vehiclePowerCore;
        }        

        public void Patch()
        {
            this.TechType = TechTypeHandler.AddTechType(NameID,
                                         FriendlyName,
                                         Description,
                                         ImageUtils.LoadSpriteFromFile(@"./QMods/UpgradedVehicles/Assets/SeamothMk2.png"),
                                         false);

            TechTypeID = this.TechType;

            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Constructor, this.TechType, "Vehicles");
            CraftDataHandler.SetCraftingTime(this.TechType, 15f);
            CraftDataHandler.SetTechData(this.TechType, GetRecipe());

            PrefabHandler.RegisterPrefab(this);
            KnownTechHandler.SetAnalysisTechEntry(TechType.VehicleHullModule3, new TechType[1] { this.TechType }, $"{FriendlyName} blueprint discovered!");
            CraftDataHandler.AddToGroup(TechGroup.Constructor, TechCategory.Constructor, this.TechType);
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

                                 new Ingredient(TechType.VehicleHullModule3, 1), // Minimum crush depth of 900 without upgrades
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
            lifeData.maxHealth = life.maxHealth * 2f; // 100% more HP

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
