namespace UpgradedVehicles
{
    using System;
    using System.Collections.Generic;
    using Common;
    using SMLHelper;
    using SMLHelper.Patchers;
    using UnityEngine;
    using UpgradedVehicles.Modules;

    internal class ExosuitMk2
    {
        public static TechType TechTypeID { get; private set; }
        public const string NameID = "ExosuitMk2";
        public const string FriendlyName = "Prawn Suit Mk2";
        public const string Description = "An upgraded Prawn Suit built to take on anything.";

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
                _ingredients = new List<IngredientHelper>(new IngredientHelper[]
                             {
                                 new IngredientHelper(TechType.PlasteelIngot, 2), // Same
                                 new IngredientHelper(TechType.PowerCell, 2), // Should have been here
                                 new IngredientHelper(TechType.Kyanite, 4), // Better than Aerogel
                                 new IngredientHelper(TechType.EnameledGlass, 1), // Same
                                 new IngredientHelper(TechType.Lead, 2), // Same
                                 new IngredientHelper(TechType.Diamond, 2), // Same
                                 new IngredientHelper(TechType.Nickel, 1), // In addition to diamond
                                 
                                 new IngredientHelper(TechType.ExoHullModule2, 1), // Minimum crush depth of 1700 without upgrades

                                 // This will be replaced with a single component
                                 new IngredientHelper(TechType.VehiclePowerUpgradeModule, 2), // Permanent +2 to engine eficiency
                                 new IngredientHelper(TechType.VehicleArmorPlating, 2), // Permanent +2 to armor                                 
                                 new IngredientHelper(SpeedBooster.TechTypeID, 2), // Permanent speed boost
                             }),
                _techType = TechTypeID
            };
        }

        private static GameObject GetGameObject()
        {
            GameObject seamothPrefab = Resources.Load<GameObject>("WorldEntities/Tools/Exosuit");
            GameObject obj = GameObject.Instantiate(seamothPrefab);

            obj.name = NameID;
            
            obj.GetComponent<PrefabIdentifier>().ClassId = NameID;
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
