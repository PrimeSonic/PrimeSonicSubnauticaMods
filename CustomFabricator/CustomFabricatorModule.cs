namespace CustomFabricator
{
    using System.Collections.Generic;
    using SMLHelper;
    using SMLHelper.Patchers;
    using UnityEngine;
    using Object = UnityEngine.Object;
    using Harmony;
    using System.Reflection;
    using Logger = Utilites.Logger.Logger;

    public class CustomFabricatorModule
    {
        public static CraftTree.Type CustomTreeType { get; private set; }
        public static TechType CustomFabTechType { get; private set; }

        public const string CustomFabID = "CustomFabricator";

        public static void Patch()
        {
            // Create new Craft Tree Type
            CustomTreeType = CraftTreeTypePatcher.AddCraftTreeType(CustomFabID);

            // Create a new TechType for new fabricator
            CustomFabTechType = TechTypePatcher.AddTechType(CustomFabID, "Vehicle Module Fabricator", "A Vehicle Upgrade Console for your Cyclops.", true);            

            var customCraftTree = new CraftNode("Root", TreeAction.None, TechType.None).AddNode(new CraftNode[]
            {
                new CraftNode("CommonModules", TreeAction.Expand, TechType.None).AddNode(new CraftNode[]
                {
                    new CraftNode("VehiclePowerUpgradeModule", TreeAction.Craft, TechType.VehiclePowerUpgradeModule),
                    new CraftNode("VehicleStorageModule", TreeAction.Craft, TechType.VehicleStorageModule)
                }),
                new CraftNode("SeamothModules", TreeAction.Expand, TechType.None).AddNode(new CraftNode[]
                {
                    new CraftNode("VehicleHullModule1", TreeAction.Craft, TechType.VehicleHullModule1),
                    new CraftNode("SeamothSolarCharge", TreeAction.Craft, TechType.SeamothSolarCharge),
                    new CraftNode("SeamothElectricalDefense", TreeAction.Craft, TechType.SeamothElectricalDefense),
                    new CraftNode("SeamothSonarModule", TreeAction.Craft, TechType.SeamothSonarModule)
                }),
                new CraftNode("ExosuitModules", TreeAction.Expand, TechType.None).AddNode(new CraftNode[]
                {
                    new CraftNode("ExoHullModule1", TreeAction.Craft, TechType.ExoHullModule1),
                    new CraftNode("ExosuitThermalReactorModule", TreeAction.Craft, TechType.ExosuitThermalReactorModule),
                    new CraftNode("ExosuitJetUpgradeModule", TreeAction.Craft, TechType.ExosuitJetUpgradeModule),
                    new CraftNode("ExosuitGrapplingArmModule", TreeAction.Craft, TechType.ExosuitGrapplingArmModule),
                    new CraftNode("ExosuitDrillArmModule", TreeAction.Craft, TechType.ExosuitDrillArmModule),
                }),
            });

            // Add the new Craft Tree
            CraftTreePatcher.customTrees.Add(new CustomCraftTree(CustomTreeType, customCraftTree));

            // Create a new Recipie
            var customFabRecipe = new TechDataHelper()
            {
                _craftAmount = 1,
                _ingredients = new List<IngredientHelper>(new IngredientHelper[3]
                             {
                                 new IngredientHelper(TechType.Titanium, 1),
                                 new IngredientHelper(TechType.ComputerChip, 1),
                                 new IngredientHelper(TechType.CopperWire, 1),
                             }),
                _techType = CustomFabTechType
            };

            CraftDataPatcher.customBuildables.Add(CustomFabTechType);            
            
            CraftDataPatcher.AddToCustomGroup(TechGroup.InteriorPieces, TechCategory.InteriorPiece, CustomFabTechType);
            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(CustomFabID, $"Submarine/Build/{CustomFabID}", CustomFabTechType, GetPrefab));

            CustomSpriteHandler.customSprites.Add(new CustomSprite(CustomFabTechType, SpriteManager.Get(TechType.VehiclePowerUpgradeModule)));

            CraftDataPatcher.customTechData[CustomFabTechType] = customFabRecipe;
        }

        public static GameObject GetPrefab()
        {
            GameObject originalPrefab = Resources.Load<GameObject>("Submarine/Build/Fabricator");
            GameObject prefab = GameObject.Instantiate(originalPrefab);

            prefab.name = CustomFabID;
            prefab.GetComponent<PrefabIdentifier>().ClassId = CustomFabID;
            prefab.GetComponent<TechTag>().type = CustomFabTechType;

            var fabricator = prefab.GetComponent<Fabricator>();
            fabricator.craftTree = CustomTreeType;

            var constructible = prefab.GetComponent<Constructable>();
            constructible.allowedInBase = false;
            constructible.allowedInSub = true;

            return prefab;
        }
    }
}
