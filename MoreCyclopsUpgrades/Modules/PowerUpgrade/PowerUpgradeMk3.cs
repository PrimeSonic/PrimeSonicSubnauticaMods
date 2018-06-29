namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using SMLHelper; // by ahk1221 https://github.com/ahk1221/SMLHelper/
    using SMLHelper.Patchers;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class PowerUpgradeMk3
    {
        public static TechType Power3TechType { get; private set; }

        public const string NameID = "PowerUpgradeModuleMk3";
        public const string FriendlyName = "Cyclops Engine Efficiency Module MK3";
        public const string Description = "Maximum engine efficiency. Silent running, Sonar, and Shield greatly optimized.  Does not stack.";

        public static void Patch(AssetBundle assetBundle)
        {
            // Create a new TechType
            Power3TechType = TechTypePatcher.AddTechType(NameID, FriendlyName, Description, unlockOnGameStart: true);

            // Create the in-game item that will behave like any other Cyclops upgrade module
            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(NameID, $"WorldEntities/Tools/{NameID}", Power3TechType, GetObject));

            // Get the custom icon from the Unity assets bundle
            CustomSpriteHandler.customSprites.Add(new CustomSprite(Power3TechType, assetBundle.LoadAsset<Sprite>("CyPowerMk3")));

            // Add the new recipe to the Modification Station crafting tree
            CraftTreePatcher.customNodes.Add(new CustomCraftNode(Power3TechType, CraftTree.Type.Workbench, $"CyclopsMenu/{NameID}"));

            // Create a new Recipie and pair the new recipie with the new TechType
            CraftDataPatcher.customTechData[Power3TechType] = GetRecipe();

            // Ensure that the new in-game item is classified as a Cyclops upgrade module. Otherwise you can't equip it.
            CraftDataPatcher.customEquipmentTypes[Power3TechType] = EquipmentType.CyclopsModule;
        }

        private static TechDataHelper GetRecipe()
        {
            return new TechDataHelper()
            {
                _craftAmount = 1,
                _ingredients = new List<IngredientHelper>(new IngredientHelper[3]
                             {
                                 new IngredientHelper(PowerUpgradeMk2.Power2TechType, 1),
                                 new IngredientHelper(TechType.Kyanite, 1), // More uses for Kyanite!
                                 new IngredientHelper(TechType.Diamond, 1),
                             }),
                _techType = Power3TechType
            };
        }

        public static GameObject GetObject()
        {
            GameObject prefab = Resources.Load<GameObject>("WorldEntities/Tools/PowerUpgradeModule");
            GameObject obj = Object.Instantiate(prefab);

            obj.GetComponent<PrefabIdentifier>().ClassId = NameID;
            obj.GetComponent<TechTag>().type = Power3TechType;

            return obj;
        }
    }
}
