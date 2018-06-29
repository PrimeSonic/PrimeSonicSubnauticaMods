namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using SMLHelper; // by ahk1221 https://github.com/ahk1221/SMLHelper/
    using SMLHelper.Patchers;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class SolarChargerMk2
    {
        public static TechType TechTypeID { get; private set; }

        public const string NameID = "CyclopsSolarChargerMk2";
        public const string FriendlyName = "Cyclops Solar Charger Mk2";
        public const string Description = "Improved solar charging and with integrated batteries to store a little extra power for when you can't see the sun.";

        public static void Patch(AssetBundle assetBundle)
        {
            // Create a new TechType
            TechTypeID = TechTypePatcher.AddTechType(NameID, FriendlyName, Description, unlockOnGameStart: true);

            // Create the in-game item that will behave like any other Cyclops upgrade module
            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(NameID, $"WorldEntities/Tools/{NameID}", TechTypeID, GetSolarChargerObject));

            // Get the custom icon from the Unity assets bundle
            CustomSpriteHandler.customSprites.Add(new CustomSprite(TechTypeID, assetBundle.LoadAsset<Sprite>("CySolarIconMk2")));

            // Add the new recipe to the Modification Station crafting tree
            CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechTypeID, CraftTree.Type.Workbench, $"CyclopsMenu/{NameID}"));

            // Create a new Recipie and pair the new recipie with the new TechType
            CraftDataPatcher.customTechData[TechTypeID] = GetRecipe();

            // Ensure that the new in-game item is classified as a Cyclops upgrade module. Otherwise you can't equip it.
            CraftDataPatcher.customEquipmentTypes[TechTypeID] = EquipmentType.CyclopsModule;
        }

        private static TechDataHelper GetRecipe()
        {
            return new TechDataHelper()
            {
                _craftAmount = 1,
                _ingredients = new List<IngredientHelper>(new IngredientHelper[6]
                             {
                                 new IngredientHelper(SolarCharger.TechTypeID, 1),
                                 new IngredientHelper(TechType.Battery, 2),
                                 new IngredientHelper(TechType.Sulphur, 1),
                                 new IngredientHelper(TechType.Kyanite, 1),
                                 new IngredientHelper(TechType.WiringKit, 1),
                                 new IngredientHelper(TechType.CopperWire, 1),
                             }),
                _techType = TechTypeID
            };
        }

        public static GameObject GetSolarChargerObject()
        {
            GameObject prefab = Resources.Load<GameObject>("WorldEntities/Tools/CyclopsThermalReactorModule");
            GameObject obj = Object.Instantiate(prefab);

            obj.GetComponent<PrefabIdentifier>().ClassId = NameID;
            obj.GetComponent<TechTag>().type = TechTypeID;

            var pCell = obj.AddComponent<Battery>();
            pCell.name = FriendlyName;
            pCell._capacity = PowerCharging.MaxMk2Charge;

            return obj;
        }
    }
}
