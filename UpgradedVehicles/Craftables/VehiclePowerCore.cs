namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper;
    using SMLHelper.Patchers;
    using UnityEngine;

    internal class VehiclePowerCore
    {
        public static TechType TechTypeID { get; private set; }
        public const string NameID = "VehiclePowerCore";
        public const string FriendlyName = "Vehicle Power Core";
        public const string Description = "A replcement power core for upgraded vehicles. Enables permanent enhacements without use of external upgrade modules.";

        public static void Patch()
        {
            TechTypeID = TechTypePatcher.AddTechType(NameID, FriendlyName, Description, unlockOnGameStart: true);

            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(NameID, $"WorldEntities/Tools/{NameID}", TechTypeID, GetGameObject));

            // TODO Icon
            CustomSpriteHandler.customSprites.Add(new CustomSprite(TechTypeID, SpriteManager.Get(TechType.VehiclePowerUpgradeModule)));

            CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechTypeID, CraftTree.Type.SeamothUpgrades, $"CommonModules/{NameID}"));

            CraftDataPatcher.customTechData[TechTypeID] = GetRecipe();
                        
            CraftDataPatcher.customEquipmentTypes[TechTypeID] = EquipmentType.None;
        }

        private static TechDataHelper GetRecipe()
        {
            return new TechDataHelper()
            {
                _craftAmount = 1,
                _ingredients = new List<IngredientHelper>(new IngredientHelper[6]
                             {
                                 new IngredientHelper(TechType.Benzene, 1), 
                                 new IngredientHelper(TechType.Lead, 2), 
                                 new IngredientHelper(TechType.PowerCell, 1), 

                                 new IngredientHelper(TechType.VehiclePowerUpgradeModule, 2), // Permanent +2 to engine eficiency
                                 new IngredientHelper(TechType.VehicleArmorPlating, 2), // Permanent +2 to armor                                 
                                 new IngredientHelper(SpeedBooster.TechTypeID, 2), // Permanent speed boost
                             }),
                _techType = TechTypeID
            };
        }


        public static GameObject GetGameObject()
        {
            GameObject prefab = Resources.Load<GameObject>("WorldEntities/Tools/PrecursorIonPowerCell");
            GameObject obj = GameObject.Instantiate(prefab);
            GameObject.DestroyImmediate(obj.GetComponent<Battery>());

            obj.GetComponent<TechTag>().type = TechTypeID;

            return obj;
        }
    }
}
