namespace UpgradedVehicles
{    
    using System.Collections.Generic;
    using Common;    
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class SeaMothMk2
    {
        public static TechType TechTypeID { get; private set; }
        public const string NameID = "SeaMothMk2";
        public const string FriendlyName = "Seamoth Mk2";
        public const string Description = "An upgraded SeaMoth ready to take you anywhere.";

        public static void Patch()
        {
            TechTypeID = TechTypeHandler.AddTechType(NameID, FriendlyName, Description);

            // TODO Icon
            SpriteHandler.RegisterSprite(TechTypeID, SpriteManager.Get(TechType.Seamoth));                       

            CraftTreeHandler.AddCraftingNodeToTab(CraftTree.Type.Constructor, TechTypeID, "Vehicles");
            CraftDataHandler.AddCraftingTime(TechTypeID, 15f);

            CraftDataHandler.AddTechData(TechTypeID, GetRecipe());

            PrefabHandler.RegisterPrefab(new SeaMothMk2Prefab(TechTypeID, NameID));
        }

        private static TechData GetRecipe()
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
                                 new Ingredient(VehiclePowerCore.TechTypeID, 1), // +2 to armor + speed without engine efficiency penalty
                             })
            };
        }

        internal class SeaMothMk2Prefab : ModPrefab
        {
            public SeaMothMk2Prefab(TechType techtype, string nameID) : base(nameID, $"{nameID}Prefab", techtype)
            {
            }

            public override GameObject GetGameObject()
            {
                GameObject seamothPrefab = Resources.Load<GameObject>("WorldEntities/Tools/SeaMoth");
                GameObject obj = GameObject.Instantiate(seamothPrefab);

                obj.name = this.PrefabFileName;
                obj.GetComponent<TechTag>().type = this.TechType;
                obj.GetComponent<PrefabIdentifier>().ClassId = this.ClassID;

                var seamoth = obj.GetComponent<SeaMoth>();

                var life = seamoth.GetComponent<LiveMixin>();

                LiveMixinData lifeData = (LiveMixinData)ScriptableObject.CreateInstance(typeof(LiveMixinData));

                life.data.CloneFieldsInto(lifeData);
                lifeData.maxHealth = life.maxHealth * 2; // 100% more HP

                life.data = lifeData;
                life.health = life.data.maxHealth;

                // Always on upgrades handled in OnUpgradeModuleChange patch

                return obj;
            }
        }
    }
}
