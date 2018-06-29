namespace UpgradedVehicles
{
    using System;
    using System.Collections.Generic;
    using Common;
    using SMLHelper;
    using SMLHelper.Patchers;
    using UnityEngine;

    internal class ExosuitMk2
    {
        public static TechType TechTypeID { get; private set; }
        public const string NameID = "ExosuitMk2";
        public const string FriendlyName = "Prawn Suit Mk2";
        public const string Description = "An upgraded Prawn Suit built even tougher to take on anything.";

        public static void Patch()
        {
            TechTypeID = TechTypePatcher.AddTechType(NameID, FriendlyName, Description, unlockOnGameStart: true);

            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(NameID, $"WorldEntities/Tools/{NameID}", TechTypeID, GetGameObject));

            // TODO Icon
            CustomSpriteHandler.customSprites.Add(new CustomSprite(TechTypeID, SpriteManager.Get(TechType.Exosuit)));

            CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechTypeID, CraftTree.Type.Constructor, $"Vehicles/{NameID}"));

            CraftDataPatcher.customTechData[TechTypeID] = GetRecipe();
        }

        private static TechDataHelper GetRecipe()
        {
            return new TechDataHelper()
            {
                _craftAmount = 1,
                _ingredients = new List<IngredientHelper>(new IngredientHelper[6]
                             {
                                 new IngredientHelper(TechType.PlasteelIngot, 2),                                 
                                 new IngredientHelper(TechType.Kyanite, 4), // Better than Aerogel
                                 new IngredientHelper(TechType.EnameledGlass, 1),
                                 new IngredientHelper(TechType.Diamond, 2),
                                 
                                 new IngredientHelper(TechType.ExoHullModule2, 1), // Minimum crush depth of 1700 without upgrades
                                 new IngredientHelper(VehiclePowerCore.TechTypeID, 1),  // +2 to armor + speed without engine efficiency penalty
                             }),
                _techType = TechTypeID
            };
        }

        private static GameObject GetGameObject()
        {
            GameObject seamothPrefab = Resources.Load<GameObject>("WorldEntities/Tools/Exosuit");
            GameObject obj = GameObject.Instantiate(seamothPrefab);

            obj.name = NameID;
            
            obj.GetComponent<TechTag>().type = TechTypeID;

            var exosuit = obj.GetComponent<Exosuit>();

            var life = exosuit.GetComponent<LiveMixin>();

            LiveMixinData lifeData = (LiveMixinData)ScriptableObject.CreateInstance(typeof(LiveMixinData));

            life.data.CloneFieldsInto(lifeData);
            lifeData.maxHealth = life.maxHealth * 1.5f; // 50% more HP

            life.data = lifeData;
            life.health = life.data.maxHealth;

            // Always on upgrades handled in OnUpgradeModuleChange patch

            return obj;
        }
    }
}
