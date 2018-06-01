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
        public const string CustomFabAndTreeID = "VModFabricator";

        // The text you'll see in-game when you mouseover over it.
        public const string FriendlyName = "Vehicle Module Fabricator";

        // AssetBundles must only be loaded once
        private static AssetBundle Assets = AssetBundle.LoadFromFile(@"./QMods/VModFabricator/Assets/vmodfabricator.assets");

        public static void Patch()
        {
            // Create new Craft Tree Type
            CustomCraftTreeRoot customTreeRootNode = CreateCustomTree(out CraftTree.Type craftType);
            VModTreeType = craftType;

            // Create a new TechType for new fabricator
            VModFabTechType = TechTypePatcher.AddTechType(CustomFabAndTreeID, FriendlyName, "Construct vehicle upgrade modules from the comfort of your own habitat or cyclops.", true);

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
            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(CustomFabAndTreeID, $"Submarine/Build/{CustomFabAndTreeID}", VModFabTechType, GetPrefab));

            // Set the custom sprite for the Habitat Builder Tool menu
            CustomSpriteHandler.customSprites.Add(new CustomSprite(VModFabTechType, Assets.LoadAsset<Sprite>("fabricator_icon_blue")));

            // Associate the recipie to the new TechType
            CraftDataPatcher.customTechData[VModFabTechType] = customFabRecipe;
        }

        private static CustomCraftTreeRoot CreateCustomTree(out CraftTree.Type craftType)
        {
            var rootNode = CraftTreeTypePatcher.CreateCustomCraftTreeAndType(CustomFabAndTreeID, out craftType);

            var cyclopsTab = rootNode.AddTabNode("CyclopsModules", "Cyclops Modules", SpriteManager.Get(SpriteManager.Group.Category, "Workbench_CyclopsMenu"));
            var cyclopsDepthTab = cyclopsTab.AddTabNode("CyclopsDepthModules", "Depth Modules", SpriteManager.Get(TechType.CyclopsHullModule1));
            cyclopsDepthTab.AddCraftingNode(TechType.CyclopsHullModule1,
                                            TechType.CyclopsHullModule2,
                                            TechType.CyclopsHullModule3);
            cyclopsTab.AddCraftingNode(TechType.PowerUpgradeModule,
                                       TechType.CyclopsShieldModule,
                                       TechType.CyclopsSonarModule,
                                       TechType.CyclopsSeamothRepairModule,
                                       TechType.CyclopsFireSuppressionModule,
                                       TechType.CyclopsDecoyModule,
                                       TechType.CyclopsThermalReactorModule);
            cyclopsTab.AddModdedCraftingNode("CyclopsSolarCharger"); // Compatible with the CyclopsSolarPower mod whether you have it or not!
            cyclopsTab.AddModdedCraftingNode("CyclopsNuclearModule"); // Compatible with the CyclopsNuclearPower mod whether you have it or not!
            var exosuitTab = rootNode.AddTabNode("ExosuitModules", "Prawn Suit Modules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_ExosuitModules"));
            var exosuitDepthTab = exosuitTab.AddTabNode("ExosuitDepthModules", "Depth Modules", SpriteManager.Get(TechType.ExoHullModule1));
            exosuitDepthTab.AddCraftingNode(TechType.ExoHullModule1,
                                            TechType.ExoHullModule2);
            exosuitTab.AddCraftingNode(TechType.ExosuitThermalReactorModule,
                                       TechType.ExosuitJetUpgradeModule,
                                       TechType.ExosuitGrapplingArmModule);
            var seamothTab = rootNode.AddTabNode("SeamothModules", "Seamoth Modules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_SeamothModules"));
            var seamothDepthTab = seamothTab.AddTabNode("SeamothDepthModules", "Depth Modules", SpriteManager.Get(TechType.VehicleHullModule1));
            seamothDepthTab.AddCraftingNode(TechType.VehicleHullModule1,
                                            TechType.VehicleHullModule2,
                                            TechType.VehicleHullModule3);
            seamothTab.AddCraftingNode(TechType.SeamothSolarCharge,
                                       TechType.SeamothElectricalDefense,
                                       TechType.SeamothSonarModule);
            var commonTab = rootNode.AddTabNode("CommonModules", "Common Modules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_CommonModules"));
            commonTab.AddCraftingNode(TechType.VehicleArmorPlating,
                                      TechType.VehiclePowerUpgradeModule,
                                      TechType.VehicleStorageModule);
            return rootNode;
        }

        public static GameObject GetPrefab()
        {
            // The standard Fabricator is the base to this new item
            GameObject originalPrefab = Resources.Load<GameObject>("Submarine/Build/Fabricator");
            GameObject prefab = GameObject.Instantiate(originalPrefab);

            prefab.name = CustomFabAndTreeID;

            var prefabId = prefab.GetComponent<PrefabIdentifier>();
            prefabId.ClassId = CustomFabAndTreeID;
            prefabId.name = FriendlyName;

            var techTag = prefab.GetComponent<TechTag>();
            techTag.type = VModFabTechType;

            var fabricator = prefab.GetComponent<Fabricator>();
            fabricator.craftTree = VModTreeType; // This is how the custom craft tree is associated to the fabricator

            // All this was necessary because the PowerRelay wasn't being instantiated
            var ghost = fabricator.GetComponent<GhostCrafter>();
            var powerRelay = new PowerRelay();
            // Ignore any errors you see about this fabricator not having a power relay in its parent. It does and it works.


            FieldInfo fieldInfo = typeof(GhostCrafter).GetField("powerRelay", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            fieldInfo.SetValue(fabricator, powerRelay);

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
            skinnedMeshRenderer.material.color = Color.white;

            return prefab;
        }
    }
}
