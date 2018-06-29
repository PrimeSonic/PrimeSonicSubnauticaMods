namespace UpgradedVehicles.Modules
{    
    using System.Collections.Generic;
    using SMLHelper;
    using SMLHelper.Patchers;
    using UnityEngine;

    internal class SpeedBooster
    {
        public static TechType TechTypeID { get; private set; }
        public const string NameID = "SpeedModule";
        public const string FriendlyName = "Speed Boost Module";
        public const string Description = "Allows small vehicle engines to go into overdrive, adding a 25% speed boost per module. Warning: expect higher energy consumption rates.";

        public static void Patch()
        {
            TechTypeID = TechTypePatcher.AddTechType(NameID, FriendlyName, Description, unlockOnGameStart: true);

            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(NameID, $"WorldEntities/Tools/{NameID}", TechTypeID, GetGameObject));

            // TODO Icon
            CustomSpriteHandler.customSprites.Add(new CustomSprite(TechTypeID, SpriteManager.Get(TechType.VehiclePowerUpgradeModule)));            

            CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechTypeID, CraftTree.Type.SeamothUpgrades, $"CommonModules/{NameID}"));

            CraftDataPatcher.customTechData[TechTypeID] = GetRecipe();
        }

        private static TechDataHelper GetRecipe()
        {
            return new TechDataHelper()
            {
                _craftAmount = 1,
                _ingredients = new List<IngredientHelper>(new IngredientHelper[3]
                             {
                                 new IngredientHelper(TechType.AluminumOxide, 1),
                                 new IngredientHelper(TechType.Polyaniline, 1),
                                 new IngredientHelper(TechType.ComputerChip, 1)
                             }),
                _techType = TechTypeID
            };
        }

        public static GameObject GetGameObject()
        {
            GameObject prefab = Resources.Load<GameObject>("WorldEntities/Tools/VehiclePowerUpgradeModule");
            GameObject obj = GameObject.Instantiate(prefab);

            obj.GetComponent<PrefabIdentifier>().ClassId = NameID;
            obj.GetComponent<TechTag>().type = TechTypeID;

            return obj;
        }

    }
}
