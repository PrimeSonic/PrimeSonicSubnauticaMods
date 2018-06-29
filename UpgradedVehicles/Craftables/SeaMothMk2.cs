namespace UpgradedVehicles
{
    using System;
    using System.Collections.Generic;
    using Common;
    using SMLHelper;
    using SMLHelper.Patchers;
    using UnityEngine;

    internal class SeaMothMk2
    {
        public static TechType TechTypeID { get; private set; }
        public const string NameID = "SeaMothMk2";
        public const string FriendlyName = "Seamoth Mk2";
        public const string Description = "An upgraded SeaMoth ready to take you anywhere.";

        public static void Patch()
        {
            TechTypeID = TechTypePatcher.AddTechType(NameID, FriendlyName, Description, unlockOnGameStart: true);

            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(NameID, $"WorldEntities/Tools/{NameID}", TechTypeID, GetGameObject));

            // TODO Icon
            CustomSpriteHandler.customSprites.Add(new CustomSprite(TechTypeID, SpriteManager.Get(TechType.Seamoth)));            

            CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechTypeID, CraftTree.Type.Constructor, $"Vehicles/{NameID}"));

            CraftDataPatcher.customTechData[TechTypeID] = GetRecipe();
        }

        private static TechDataHelper GetRecipe()
        {
            return new TechDataHelper()
            {
                _craftAmount = 1,
                _ingredients = new List<IngredientHelper>(new IngredientHelper[5]
                             {
                                 new IngredientHelper(TechType.PlasteelIngot, 1), // Stronger than titanium ingot                                 
                                 new IngredientHelper(TechType.EnameledGlass, 2), // Stronger than glass
                                 new IngredientHelper(TechType.Lead, 1),

                                 new IngredientHelper(TechType.VehicleHullModule3, 1), // Minimum crush depth of 900 without upgrades
                                 new IngredientHelper(VehiclePowerCore.TechTypeID, 1), // +2 to armor + speed without engine efficiency penalty
                             }),
                _techType = TechTypeID
            };
        }

        private static GameObject GetGameObject()
        {
            GameObject seamothPrefab = Resources.Load<GameObject>("WorldEntities/Tools/SeaMoth");
            GameObject obj = GameObject.Instantiate(seamothPrefab);

            obj.name = NameID;

            obj.GetComponent<TechTag>().type = TechTypeID;

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
