namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using SMLHelper; // by ahk1221 https://github.com/ahk1221/SMLHelper/
    using SMLHelper.Patchers;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class PowerUpgradeMk2
    {
        public static TechType Power2TechType { get; private set; }

        public const string NameID = "PowerUpgradeModuleMk2";
        public const string FriendlyName = "Cyclops Engine Efficiency Module MK2";
        public const string Description = "Additional enhancement to engine efficiency. Silent running, Sonar, and Shield optimized. Does not stack.";

        public static void Patch(AssetBundle assetBundle)
        {
            // Create a new TechType
            Power2TechType = TechTypePatcher.AddTechType(NameID, FriendlyName, Description, unlockOnGameStart: true);

            // Create the in-game item that will behave like any other Cyclops upgrade module
            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(NameID, $"WorldEntities/Tools/{NameID}", Power2TechType, GetObject));

            // Get the custom icon from the Unity assets bundle
            CustomSpriteHandler.customSprites.Add(new CustomSprite(Power2TechType, assetBundle.LoadAsset<Sprite>("CyPowerMk2")));

            // Add the new recipe to the Modification Station crafting tree
            CraftTreePatcher.customNodes.Add(new CustomCraftNode(Power2TechType, CraftTree.Type.Workbench, $"CyclopsMenu/{NameID}"));

            // Create a new Recipie and pair the new recipie with the new TechType
            CraftDataPatcher.customTechData[Power2TechType] = GetRecipe();

            // Ensure that the new in-game item is classified as a Cyclops upgrade module. Otherwise you can't equip it.
            CraftDataPatcher.customEquipmentTypes[Power2TechType] = EquipmentType.CyclopsModule;
        }

        private static TechDataHelper GetRecipe()
        {
            return new TechDataHelper()
            {
                _craftAmount = 1,
                _ingredients = new List<IngredientHelper>(new IngredientHelper[3]
                             {
                                 new IngredientHelper(TechType.PowerUpgradeModule, 1),
                                 new IngredientHelper(TechType.Aerogel, 1), 
                                 new IngredientHelper(TechType.Sulphur, 2) // Did you make it to the Lost River yet?
                             }),
                _techType = Power2TechType
            };
        }

        public static GameObject GetObject()
        {
            GameObject prefab = Resources.Load<GameObject>("WorldEntities/Tools/PowerUpgradeModule");
            GameObject obj = Object.Instantiate(prefab);

            obj.GetComponent<PrefabIdentifier>().ClassId = NameID;
            obj.GetComponent<TechTag>().type = Power2TechType;

            return obj;
        }
    }
}
