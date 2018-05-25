namespace VModFabricator
{
    using System.Collections.Generic;
    using SMLHelper;
    using SMLHelper.Patchers;
    using UnityEngine;
    using System.Reflection;

    public class VModFabricatorModule
    {
        public static CraftTree.Type VModTreeType { get; private set; }
        public static TechType VModFabTechType { get; private set; }

        // This name will be used as both the new TechType of the buildable fabricator and the CraftTree Type for the custom crafting tree.
        public const string CustomFabID = "VModFabricator";

        public const string FriendlyName = "Vehicle Module Fabricator";

        // AssetBundles must only be loaded once
        private static AssetBundle Assets = AssetBundle.LoadFromFile(@"./QMods/VModFabricator/Assets/vmodfabricator.assets");

        public static void Patch()
        {
            // Create new Craft Tree Type
            VModTreeType = CraftTreeTypePatcher.AddCraftTreeType(CustomFabID);

            // Create a new TechType for new fabricator
            VModFabTechType = TechTypePatcher.AddTechType(CustomFabID, FriendlyName, "Construct vehicle upgrade modules from the comfort of your own habitat or cyclops.", true);

            var customCraftTree = GetCraftingTree();

            // Add the new Craft Tree and link it to the new CraftTree Type
            CraftTreePatcher.CustomTrees[VModTreeType] = customCraftTree;

            // Create a Recipie for the new TechType
            var customFabRecipe = new TechDataHelper()
            {
                _craftAmount = 1,
                _ingredients = new List<IngredientHelper>(new IngredientHelper[4]
                             {
                                 new IngredientHelper(TechType.Titanium, 2),
                                 new IngredientHelper(TechType.ComputerChip, 1),
                                 new IngredientHelper(TechType.Diamond, 1),
                                 new IngredientHelper(TechType.Lead, 1),
                             }),
                _techType = VModFabTechType
            };

            // Add the new TechType to the buildables
            CraftDataPatcher.customBuildables.Add(VModFabTechType);

            // Add the new TechType to the group of Interior Module buildables
            CraftDataPatcher.AddToCustomGroup(TechGroup.InteriorModules, TechCategory.InteriorModule, VModFabTechType);

            // Set the buildable prefab
            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(CustomFabID, $"Submarine/Build/{CustomFabID}", VModFabTechType, GetPrefab));

            // Set the custom sprite for the Habitat Builder Tool menu
            CustomSpriteHandler.customSprites.Add(new CustomSprite(VModFabTechType, Assets.LoadAsset<Sprite>("fabricator_icon_blue")));

            // Associate the recipie to the new TechType
            CraftDataPatcher.customTechData[VModFabTechType] = customFabRecipe;
        }

        private static CustomCraftTreeRoot GetCraftingTree()
        {
            return new CustomCraftTreeRoot(VModTreeType, new CustomCraftTreeNode[]
                {
                    new CustomCraftTreeTab(VModTreeType, "CyclopsModules", "Cyclops Modules", SpriteManager.Get(SpriteManager.Group.Category, "Workbench_CyclopsMenu"), new CustomCraftTreeNode[]
                    {
                        new CustomCraftTreeTab(VModTreeType, "CyclopsDepthModules", "Depth Modules", SpriteManager.Get(TechType.CyclopsHullModule1), new CustomCraftTreeNode[]
                        {
                            new CustomCraftTreeCraft(VModTreeType, TechType.CyclopsHullModule1),
                            new CustomCraftTreeCraft(VModTreeType, TechType.CyclopsHullModule2),
                            new CustomCraftTreeCraft(VModTreeType, TechType.CyclopsHullModule3)
                        }),
                        new CustomCraftTreeCraft(VModTreeType, TechType.PowerUpgradeModule),
                        new CustomCraftTreeCraft(VModTreeType, TechType.CyclopsShieldModule),
                        new CustomCraftTreeCraft(VModTreeType, TechType.CyclopsSonarModule),
                        new CustomCraftTreeCraft(VModTreeType, TechType.CyclopsSeamothRepairModule),
                        new CustomCraftTreeCraft(VModTreeType, TechType.CyclopsFireSuppressionModule),
                        new CustomCraftTreeCraft(VModTreeType, TechType.CyclopsDecoyModule),
                        new CustomCraftTreeCraft(VModTreeType, TechType.CyclopsThermalReactorModule)
                    }),
                    new CustomCraftTreeTab(VModTreeType, "ExosuitModules", "Prawn Suit Modules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_ExosuitModules"), new CustomCraftTreeNode[]
                    {
                        new CustomCraftTreeTab(VModTreeType, "ExosuitDepthModules", "Depth Modules", SpriteManager.Get(TechType.ExoHullModule1), new CustomCraftTreeNode[]
                        {
                            new CustomCraftTreeCraft(VModTreeType, TechType.ExoHullModule1),
                            new CustomCraftTreeCraft(VModTreeType, TechType.ExoHullModule2)
                        }),
                        new CustomCraftTreeCraft(VModTreeType, TechType.ExosuitThermalReactorModule),
                        new CustomCraftTreeCraft(VModTreeType, TechType.ExosuitJetUpgradeModule),
                        new CustomCraftTreeCraft(VModTreeType, TechType.ExosuitGrapplingArmModule),
                    }),
                    new CustomCraftTreeTab(VModTreeType, "SeamothModules", "Seamoth Modules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_SeamothModules"), new CustomCraftTreeNode[]
                    {
                        new CustomCraftTreeTab(VModTreeType, "SeamothDepthModules", "Depth Modules", SpriteManager.Get(TechType.VehicleHullModule1), new CustomCraftTreeNode[]
                        {
                            new CustomCraftTreeCraft(VModTreeType, TechType.VehicleHullModule1),
                            new CustomCraftTreeCraft(VModTreeType, TechType.VehicleHullModule2),
                            new CustomCraftTreeCraft(VModTreeType, TechType.VehicleHullModule3)
                        }),
                        new CustomCraftTreeCraft(VModTreeType, TechType.SeamothSolarCharge),
                        new CustomCraftTreeCraft(VModTreeType, TechType.SeamothElectricalDefense),
                        new CustomCraftTreeCraft(VModTreeType, TechType.SeamothSonarModule)
                    }),
                    new CustomCraftTreeTab(VModTreeType, "CommonModules", "Common Modules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_CommonModules"), new CustomCraftTreeNode[]
                    {
                        new CustomCraftTreeCraft(VModTreeType, TechType.VehicleArmorPlating),
                        new CustomCraftTreeCraft(VModTreeType, TechType.VehiclePowerUpgradeModule),
                        new CustomCraftTreeCraft(VModTreeType, TechType.VehicleStorageModule)
                    }),
                });
        }

        public static GameObject GetPrefab()
        {
            // The standard Fabricator is the base to this new item
            GameObject originalPrefab = Resources.Load<GameObject>("Submarine/Build/Fabricator");
            GameObject prefab = GameObject.Instantiate(originalPrefab);

            prefab.name = CustomFabID;

            var prefabId = prefab.GetComponent<PrefabIdentifier>();
            prefabId.ClassId = CustomFabID;
            prefabId.name = FriendlyName;

            var techTag = prefab.GetComponent<TechTag>();
            techTag.type = VModFabTechType;

            var fabricator = prefab.GetComponent<Fabricator>();
            fabricator.craftTree = VModTreeType; // This is how the custom craft tree is associated to the fabricator

            // All this was necessary because the PowerRelay wasn't being instantiated
            var ghost = fabricator.GetComponent<GhostCrafter>();
            var crafter = ghost.GetComponent<Crafter>();
            var powerRelay = new PowerRelay();

            FieldInfo fieldInfo = typeof(GhostCrafter).GetField("powerRelay", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(ghost, powerRelay);

            // Set where this can be built
            var constructible = prefab.GetComponent<Constructable>();
            constructible.allowedInBase = true;
            constructible.allowedInSub = true; // This is the important one
            constructible.allowedOutside = false;
            constructible.allowedOnCeiling = false;
            constructible.allowedOnGround = false;
            constructible.allowedOnConstructables = false;
            constructible.controlModelState = true;
            constructible.techType = VModFabTechType; // This was necessary to correctly associate the recipe at building time

            // Set the custom texture
            var blueTexture = Assets.LoadAsset<Texture2D>("submarine_fabricator_blue");

            var skinnedMeshRenderer = prefab.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderer.material.mainTexture = blueTexture;

            // Add a slight blue tint to the material for added effect
            skinnedMeshRenderer.material.color = new Color(0.8f, 0.8f, 0.95f);

            return prefab;
        }
    }
}
