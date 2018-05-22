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
        public const string FriendlyName = "Vehicle Module Fabricator";

        public static void Patch()
        {
            // Create new Craft Tree Type
            CustomTreeType = CraftTreeTypePatcher.AddCraftTreeType(CustomFabID);

            // Create a new TechType for new fabricator
            CustomFabTechType = TechTypePatcher.AddTechType(CustomFabID, FriendlyName, "A Vehicle Upgrade Console for your Cyclops.", true);

            var customCraftTree = new CustomCraftTreeRoot(CustomTreeType, new CustomCraftTreeNode[]
                {
                    new CustomCraftTreeTab(CustomTreeType, "CommonModules", "Common Modules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_CommonModules"), new CustomCraftTreeNode[]
                    {
                        new CustomCraftTreeCraft(CustomTreeType, TechType.VehiclePowerUpgradeModule),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.VehicleStorageModule)
                    }),
                    new CustomCraftTreeTab(CustomTreeType, "SeamothModules", "Seamoth Modules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_SeamothModules"), new CustomCraftTreeNode[]
                    {
                        new CustomCraftTreeCraft(CustomTreeType, TechType.VehicleHullModule1),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.VehicleHullModule2),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.VehicleHullModule3),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.SeamothSolarCharge),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.SeamothElectricalDefense),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.SeamothSonarModule)
                    }),
                    new CustomCraftTreeTab(CustomTreeType, "ExosuitModules", "Prawn Suit Modules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_ExosuitModules"), new CustomCraftTreeNode[]
                    {
                        new CustomCraftTreeCraft(CustomTreeType, TechType.ExoHullModule1),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.ExoHullModule2),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.ExosuitThermalReactorModule),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.ExosuitJetUpgradeModule),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.ExosuitGrapplingArmModule),
                    }),
                    new CustomCraftTreeTab(CustomTreeType, "CyclopsModules", "Cyclops Modules", SpriteManager.Get(SpriteManager.Group.Category, "Constructor_Vehicles"), new CustomCraftTreeNode[]
                    {
                        new CustomCraftTreeCraft(CustomTreeType, TechType.CyclopsHullModule1),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.CyclopsHullModule2),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.CyclopsHullModule3),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.PowerUpgradeModule),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.CyclopsShieldModule),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.CyclopsSonarModule),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.CyclopsSeamothRepairModule),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.CyclopsFireSuppressionModule),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.CyclopsDecoyModule),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.CyclopsThermalReactorModule)
                    })
                });

            // Add the new Craft Tree
            CraftTreePatcher.CustomTrees[CustomTreeType] = customCraftTree;

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

            CraftDataPatcher.AddToCustomGroup(TechGroup.InteriorModules, TechCategory.InteriorModule, CustomFabTechType);
            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(CustomFabID, $"Submarine/Build/{CustomFabID}", CustomFabTechType, GetPrefab));

            CustomSpriteHandler.customSprites.Add(new CustomSprite(CustomFabTechType, SpriteManager.Get(TechType.BaseUpgradeConsole)));

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
            fabricator.handOverText = $"Use {FriendlyName}";

            var constructible = prefab.AddComponent<Constructable>();
            constructible.allowedInBase = true;
            constructible.allowedInSub = true;
            constructible.allowedOutside = false;
            constructible.allowedOnCeiling = false;
            constructible.allowedOnGround = false;
            constructible.allowedOnConstructables = false;            

            var skinnedMeshRenderer = prefab.GetComponentInChildren<SkinnedMeshRenderer>();

            skinnedMeshRenderer.material.color = Color.blue;

            var powerRelay = prefab.AddComponent<PowerRelay>();

            return prefab;
        }

        public static string GetTabLanguageID(string tabName)
        {
            return $"{CustomTreeType}Menu_{System.IO.Path.GetFileName(tabName)}";
        }

        public static string GetTabSpriteID(string tabName)
        {
            return $"{CustomTreeType}_{System.IO.Path.GetFileName(tabName)}";
        }
    }
}
