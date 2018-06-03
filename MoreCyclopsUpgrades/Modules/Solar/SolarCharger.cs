namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using SMLHelper; // by ahk1221 https://github.com/ahk1221/SMLHelper/
    using SMLHelper.Patchers;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class SolarCharger
    {
        public static TechType CySolarChargerTechType { get; private set; }

        public const string NameID = "CyclopsSolarCharger";
        public const string FriendlyName = "Cyclops Solar Charger";
        public const string Description = "Recharge your Cyclops with the power of the sun itself. Stack multiple for even faster charging.";

        public static void Patch(AssetBundle assetBundle)
        {
            // Create a new TechType
            CySolarChargerTechType = TechTypePatcher.AddTechType(NameID, FriendlyName, Description, unlockOnGameStart: true);

            // Create the in-game item that will behave like any other Cyclops upgrade module
            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(NameID, $"WorldEntities/Tools/{NameID}", CySolarChargerTechType, GetSolarChargerObject));

            // Get the custom icon from the Unity assets bundle
            CustomSpriteHandler.customSprites.Add(new CustomSprite(CySolarChargerTechType, assetBundle.LoadAsset<Sprite>("CySolarIcon")));

            // Add the new recipe to the Modification Station crafting tree
            CraftTreePatcher.customNodes.Add(new CustomCraftNode(CySolarChargerTechType, CraftTree.Type.Workbench, $"CyclopsMenu/{NameID}"));

            // Create a new Recipie and pair the new recipie with the new TechType
            CraftDataPatcher.customTechData[CySolarChargerTechType] = GetRecipe();

            // Ensure that the new in-game item is classified as a Cyclops upgrade module. Otherwise you can't equip it.
            CraftDataPatcher.customEquipmentTypes[CySolarChargerTechType] = EquipmentType.CyclopsModule;
        }

        private static TechDataHelper GetRecipe()
        {
            return new TechDataHelper()
            {
                _craftAmount = 1,
                _ingredients = new List<IngredientHelper>(new IngredientHelper[1]
                             { new IngredientHelper(TechType.SeamothSolarCharge, 2) }),
                _techType = CySolarChargerTechType
            };
        }

        public static GameObject GetSolarChargerObject()
        {
            GameObject prefab = Resources.Load<GameObject>("WorldEntities/Tools/CyclopsThermalReactorModule");
            GameObject obj = Object.Instantiate(prefab);

            obj.GetComponent<PrefabIdentifier>().ClassId = NameID;
            obj.GetComponent<TechTag>().type = CySolarChargerTechType;

            return obj;
        }
    }
}