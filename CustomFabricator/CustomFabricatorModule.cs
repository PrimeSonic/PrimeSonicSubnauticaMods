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

            //var old = new CraftNode("Root", TreeAction.None, TechType.None).AddNode(new CraftNode[]
            //{
            //    new CraftNode("CommonModules", TreeAction.Expand, TechType.None).AddNode(
            //        new CraftNode("VehiclePowerUpgradeModule", TreeAction.Craft, TechType.VehiclePowerUpgradeModule),
            //        new CraftNode("VehicleStorageModule", TreeAction.Craft, TechType.VehicleStorageModule)
            //    ),
            //    new CraftNode("SeamothModules", TreeAction.Expand, TechType.None).AddNode(
            //        new CraftNode("VehicleHullModule1", TreeAction.Craft, TechType.VehicleHullModule1),
            //        new CraftNode("SeamothSolarCharge", TreeAction.Craft, TechType.SeamothSolarCharge),
            //        new CraftNode("SeamothElectricalDefense", TreeAction.Craft, TechType.SeamothElectricalDefense),
            //        new CraftNode("SeamothSonarModule", TreeAction.Craft, TechType.SeamothSonarModule)
            //    ),
            //    new CraftNode("ExosuitModules", TreeAction.Expand, TechType.None).AddNode(
            //        new CraftNode("ExoHullModule1", TreeAction.Craft, TechType.ExoHullModule1),
            //        new CraftNode("ExosuitThermalReactorModule", TreeAction.Craft, TechType.ExosuitThermalReactorModule),
            //        new CraftNode("ExosuitJetUpgradeModule", TreeAction.Craft, TechType.ExosuitJetUpgradeModule),
            //        new CraftNode("ExosuitGrapplingArmModule", TreeAction.Craft, TechType.ExosuitGrapplingArmModule),
            //        new CraftNode("ExosuitDrillArmModule", TreeAction.Craft, TechType.ExosuitDrillArmModule)
            //    ),
            //    new CraftNode("CyclopsModules", TreeAction.Expand, TechType.None).AddNode(
            //        new CraftNode("CyclopsHullModule1", TreeAction.Craft, TechType.CyclopsHullModule1),
            //        new CraftNode("PowerUpgradeModule", TreeAction.Craft, TechType.PowerUpgradeModule),
            //        new CraftNode("CyclopsShieldModule", TreeAction.Craft, TechType.CyclopsShieldModule),
            //        new CraftNode("CyclopsSonarModule", TreeAction.Craft, TechType.CyclopsSonarModule),
            //        new CraftNode("CyclopsSeamothRepairModule", TreeAction.Craft, TechType.CyclopsSeamothRepairModule),
            //        new CraftNode("CyclopsFireSuppressionModule", TreeAction.Craft, TechType.CyclopsFireSuppressionModule),
            //        new CraftNode("CyclopsDecoyModule", TreeAction.Craft, TechType.CyclopsDecoyModule),
            //        new CraftNode("CyclopsThermalReactorModule", TreeAction.Craft, TechType.CyclopsThermalReactorModule)
            //    ),
            //});

            var customCraftTree = new CustomCraftTreeRoot(CustomTreeType, new CustomCraftTreeNode[]
                {
                    new CustomCraftTreeTab(CustomTreeType, "CommonModules", "Common Modules", 
                    new CustomSprite(SpriteManager.Group.Category, "CommonModules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_CommonModules")), new CustomCraftTreeNode[]
                    {
                        new CustomCraftTreeCraft(CustomTreeType, TechType.VehiclePowerUpgradeModule),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.VehicleStorageModule)
                    }),
                    new CustomCraftTreeTab(CustomTreeType, "SeamothModules", "Seamoth Modules",
                    new CustomSprite(SpriteManager.Group.Category, "SeamothModules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_SeamothModules")), new CustomCraftTreeNode[]
                    {
                        new CustomCraftTreeCraft(CustomTreeType, TechType.VehicleHullModule1),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.VehicleHullModule2),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.VehicleHullModule3),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.SeamothSolarCharge),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.SeamothElectricalDefense),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.SeamothSonarModule)
                    }),
                    new CustomCraftTreeTab(CustomTreeType, "ExosuitModules", "Prawn Suit Modules",
                    new CustomSprite(SpriteManager.Group.Category, "ExosuitModules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_ExosuitModules")), new CustomCraftTreeNode[]
                    {
                        new CustomCraftTreeCraft(CustomTreeType, TechType.ExoHullModule1),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.ExoHullModule2),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.ExosuitThermalReactorModule),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.ExosuitJetUpgradeModule),
                        new CustomCraftTreeCraft(CustomTreeType, TechType.ExosuitGrapplingArmModule),                       
                    }),
                    new CustomCraftTreeTab(CustomTreeType, "CyclopsModules", "Cyclops Modules", 
                    new CustomSprite(SpriteManager.Group.Category, "CyclopsModules", SpriteManager.Get(SpriteManager.Group.Category, "Constructor_Vehicles")), new CustomCraftTreeNode[]
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

            //CustomSpriteHandler.customSprites.Add(new CustomSprite(SpriteManager.Group.Category, GetTabSpriteID("CommonModules"), SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_CommonModules")));
            //LanguagePatcher.customLines[GetTabLanguageID("CommonModules")] = "Common Modules";

            //CustomSpriteHandler.customSprites.Add(new CustomSprite(SpriteManager.Group.Category, GetTabSpriteID("SeamothModules"), SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_SeamothModules")));
            //LanguagePatcher.customLines[GetTabLanguageID("SeamothModules")] = "Seamoth Modules";

            //CustomSpriteHandler.customSprites.Add(new CustomSprite(SpriteManager.Group.Category, GetTabSpriteID("ExosuitModules"), SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_ExosuitModules")));
            //LanguagePatcher.customLines[GetTabLanguageID("ExosuitModules")] = "Prawn Suit Modules";

            //CustomSpriteHandler.customSprites.Add(new CustomSprite(SpriteManager.Group.Category, GetTabSpriteID("CyclopsModules"), SpriteManager.Get(SpriteManager.Group.Category, "Constructor_Vehicles")));
            //LanguagePatcher.customLines[GetTabLanguageID("CyclopsModules")] = "Cyclops Modules";

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
            

            var constructible = prefab.GetComponent<Constructable>();
            constructible.allowedInBase = true;
            constructible.allowedInSub = true;

            // This is just to make the model look different.
            // There has to be a better way.
            var skyApp = prefab.GetComponent<SkyApplier>();
            skyApp.dynamic = true;
            skyApp.anchorSky = Skies.ExplorableWreck;
            skyApp.renderers = skyApp.GetAllComponentsInChildren<MeshRenderer>();

            // Why doesn't this code do anything?
            var meshRenderers = prefab.GetComponentsInChildren<MeshRenderer>();
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.material.color = new Color(0, 0, 1);
            }

            // TODO Figure out what this error in the logs is about "Error: CustomFabricator.GhostCrafter() should have a power relay in parent."

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
