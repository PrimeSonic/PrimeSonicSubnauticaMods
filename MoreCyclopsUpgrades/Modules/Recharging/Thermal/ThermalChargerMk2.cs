namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using SMLHelper;
    using SMLHelper.Patchers;
    using UnityEngine;

    internal class ThermalChargerMk2
    {
        public static TechType ThermalMk2TechType { get; private set; }

        public const string NameID = "CyclopsThermalChargerMk2";
        public const string FriendlyName = "Cyclops Thermal Reactor Mk2";
        public const string Description = "Improved thermal charging and with integrated batteries to store a little extra power for when it get cold.";

        public static void Patch(AssetBundle assetBundle)
        {
            // Create a new TechType
            ThermalMk2TechType = TechTypePatcher.AddTechType(NameID, FriendlyName, Description, unlockOnGameStart: true);

            // Create the in-game item that will behave like any other Cyclops upgrade module
            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(NameID, $"WorldEntities/Tools/{NameID}", ThermalMk2TechType, GetThermalChargerObject));

            // Get the custom icon from the Unity assets bundle
            CustomSpriteHandler.customSprites.Add(new CustomSprite(ThermalMk2TechType, assetBundle.LoadAsset<Sprite>("ThermalMk2")));

            // Add the new recipe to the Modification Station crafting tree
            CraftTreePatcher.customNodes.Add(new CustomCraftNode(ThermalMk2TechType, CraftTree.Type.Workbench, $"CyclopsMenu/{NameID}"));

            // Create a new Recipie and pair the new recipie with the new TechType
            CraftDataPatcher.customTechData[ThermalMk2TechType] = GetRecipe();

            // Ensure that the new in-game item is classified as a Cyclops upgrade module. Otherwise you can't equip it.
            CraftDataPatcher.customEquipmentTypes[ThermalMk2TechType] = EquipmentType.CyclopsModule;
        }

        private static TechDataHelper GetRecipe()
        {
            return new TechDataHelper()
            {
                _craftAmount = 1,
                _ingredients = new List<IngredientHelper>(new IngredientHelper[6]
                             {
                                 new IngredientHelper(TechType.CyclopsThermalReactorModule, 1),
                                 new IngredientHelper(TechType.Battery, 2),
                                 new IngredientHelper(TechType.Sulphur, 1),
                                 new IngredientHelper(TechType.Kyanite, 1),
                                 new IngredientHelper(TechType.WiringKit, 1),
                                 new IngredientHelper(TechType.CopperWire, 1),
                             }),
                _techType = ThermalMk2TechType
            };
        }

        public static GameObject GetThermalChargerObject()
        {
            GameObject prefab = Resources.Load<GameObject>("WorldEntities/Tools/CyclopsThermalReactorModule");
            GameObject obj = Object.Instantiate(prefab);

            obj.GetComponent<PrefabIdentifier>().ClassId = NameID;
            obj.GetComponent<TechTag>().type = ThermalMk2TechType;

            var pCell = obj.AddComponent<Battery>();
            pCell.name = FriendlyName;
            pCell._capacity = PowerCharging.MaxMk2Charge;

            return obj;
        }
    }
}
