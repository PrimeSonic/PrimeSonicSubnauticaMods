namespace VModFabricator
{
    using System.Collections.Generic;
    using System.Reflection;
    using Common;
    using SMLHelper;
    using SMLHelper.Patchers;
    using UnityEngine;

    public class VModFabricatorModule
    {
        // This will contain the original prefab of CyclopsFabricator.
        public static GameObject originalCyclopsFabricatorPrefab = null;

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
            // Retrieve original CyclopsFabricator prefab. This must be done once (not everytime we call GetPrefab).
            originalCyclopsFabricatorPrefab = Resources.Load<GameObject>("Submarine/Build/CyclopsFabricator");

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
            CustomSpriteHandler.customSprites.Add(new CustomSprite(VModFabTechType, Assets.LoadAsset<Sprite>("CyFabIcon")));

            // Associate the recipie to the new TechType
            CraftDataPatcher.customTechData[VModFabTechType] = customFabRecipe;
        }

        private static CustomCraftTreeRoot CreateCustomTree(out CraftTree.Type craftType)
        {
            var rootNode = CraftTreeTypePatcher.CreateCustomCraftTreeAndType(CustomFabAndTreeID, out craftType);

            var cyclopsTab = rootNode.AddTabNode("CyclopsModules", "Cyclops Modules", SpriteManager.Get(SpriteManager.Group.Category, "Workbench_CyclopsMenu"));
            cyclopsTab.AddCraftingNode(TechType.CyclopsShieldModule,
                           TechType.CyclopsSonarModule,
                           TechType.CyclopsSeamothRepairModule,
                           TechType.CyclopsFireSuppressionModule,
                           TechType.CyclopsDecoyModule);
            var cyclopsDepthTab = cyclopsTab.AddTabNode("CyclopsDepthModules", "Depth Modules", SpriteManager.Get(TechType.CyclopsHullModule1));
            cyclopsDepthTab.AddCraftingNode(TechType.CyclopsHullModule1,
                                            TechType.CyclopsHullModule2,
                                            TechType.CyclopsHullModule3);
            var cyclopsPowerTab = cyclopsTab.AddTabNode("CyclopsPowerModules", "Power Modules", SpriteManager.Get(TechType.PowerUpgradeModule));
            cyclopsPowerTab.AddCraftingNode(TechType.PowerUpgradeModule);
            // Compatible with the MoreCyclopsUpgrades mod whether you have it or not!
            cyclopsPowerTab.AddModdedCraftingNode("PowerUpgradeModuleMk2");
            cyclopsPowerTab.AddModdedCraftingNode("PowerUpgradeModuleMk3");
            cyclopsPowerTab.AddModdedCraftingNode("CyclopsSolarCharger");
            cyclopsPowerTab.AddModdedCraftingNode("CyclopsSolarChargerMk2");
            cyclopsPowerTab.AddModdedCraftingNode("CyclopsNuclearModule");
            cyclopsPowerTab.AddModdedCraftingNode("CyclopsThermalChargerMk2");
            cyclopsPowerTab.AddCraftingNode(TechType.CyclopsThermalReactorModule);

            var exosuitTab = rootNode.AddTabNode("ExosuitModules", "Prawn Suit Modules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_ExosuitModules"));
            var exosuitDepthTab = exosuitTab.AddTabNode("ExosuitDepthModules", "Depth Modules", SpriteManager.Get(TechType.ExoHullModule1));
            exosuitDepthTab.AddCraftingNode(TechType.ExoHullModule1,
                                            TechType.ExoHullModule2);
            exosuitTab.AddCraftingNode(TechType.ExosuitThermalReactorModule,
                                       TechType.ExosuitJetUpgradeModule,
                                       TechType.ExosuitPropulsionArmModule,
                                       TechType.ExosuitGrapplingArmModule,
                                       TechType.ExosuitDrillArmModule,
                                       TechType.ExosuitTorpedoArmModule);

            var seamothTab = rootNode.AddTabNode("SeamothModules", "Seamoth Modules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_SeamothModules"));
            var seamothDepthTab = seamothTab.AddTabNode("SeamothDepthModules", "Depth Modules", SpriteManager.Get(TechType.VehicleHullModule1));
            seamothDepthTab.AddCraftingNode(TechType.VehicleHullModule1,
                                            TechType.VehicleHullModule2,
                                            TechType.VehicleHullModule3);
            seamothDepthTab.AddModdedCraftingNode("SeamothHullModule4"); // Compatible with MoreSeamothUpgrades mod whether you have it or not!
            seamothDepthTab.AddModdedCraftingNode("SeamothHullModule5"); // Compatible with MoreSeamothUpgrades mod whether you have it or not!           
            seamothTab.AddCraftingNode(TechType.SeamothSolarCharge,
                                       TechType.SeamothElectricalDefense,
                                       TechType.SeamothSonarModule);
            seamothTab.AddModdedCraftingNode("SeamothThermalModule"); // Compatible with MoreSeamothUpgrades mod whether you have it or not!
            seamothTab.AddModdedCraftingNode("SeamothDrillModule"); // Compatible with MoreSeamothUpgrades mod whether you have it or not!
            var commonTab = rootNode.AddTabNode("CommonModules", "Common Modules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_CommonModules"));
            commonTab.AddCraftingNode(TechType.VehicleArmorPlating,
                                      TechType.VehiclePowerUpgradeModule,
                                      TechType.VehicleStorageModule);

            var torpedoesTab = rootNode.AddTabNode("TorpedoesModules", "Torpedoes", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_Torpedoes"));
            torpedoesTab.AddCraftingNode(TechType.WhirlpoolTorpedo,
                                         TechType.GasTorpedo);
            return rootNode;
        }

        public static GameObject GetPrefab()
        {
            // Instantiate CyclopsFabricator object
            GameObject cyclopsFabPrefab = GameObject.Instantiate(originalCyclopsFabricatorPrefab);

            // Retrieve sub game objects
            GameObject cyclopsFabLight = cyclopsFabPrefab.FindChild("fabricatorLight");
            GameObject cyclopsFabModel = cyclopsFabPrefab.FindChild("submarine_fabricator_03");

            // Update prefab name
            cyclopsFabPrefab.name = CustomFabAndTreeID;

            // Add prefab ID
            var prefabId = cyclopsFabPrefab.AddComponent<PrefabIdentifier>();
            prefabId.ClassId = CustomFabAndTreeID;
            prefabId.name = FriendlyName;

            // Add tech tag
            var techTag = cyclopsFabPrefab.AddComponent<TechTag>();
            techTag.type = VModFabTechType;

            // Translate CyclopsFabricator model and light
            cyclopsFabModel.transform.localPosition = new Vector3(
                                                        cyclopsFabModel.transform.localPosition.x, // Same X position
                                                        cyclopsFabModel.transform.localPosition.y - 0.8f, // Push towards the wall slightly
                                                        cyclopsFabModel.transform.localPosition.z); // Same Z position
            cyclopsFabLight.transform.localPosition = new Vector3(
                                                        cyclopsFabLight.transform.localPosition.x, // Same X position
                                                        cyclopsFabLight.transform.localPosition.y - 0.8f, // Push towards the wall slightly
                                                        cyclopsFabLight.transform.localPosition.z); // Same Z position

            // Update sky applier
            var skyApplier = cyclopsFabPrefab.GetComponent<SkyApplier>();
            skyApplier.renderers = cyclopsFabPrefab.GetComponentsInChildren<Renderer>();
            skyApplier.anchorSky = Skies.Auto;

            // Associate custom craft tree to the fabricator
            var fabricator = cyclopsFabPrefab.GetComponent<Fabricator>();
            fabricator.craftTree = VModTreeType;

            // Associate power relay
            var ghost = fabricator.GetComponent<GhostCrafter>();
            var powerRelay = new PowerRelay();            

            fabricator.SetPrivateField("powerRelay", powerRelay, BindingFlags.FlattenHierarchy);

            // Add constructable
            var constructible = cyclopsFabPrefab.AddComponent<Constructable>();
            constructible.allowedInBase = true;
            constructible.allowedInSub = true;
            constructible.allowedOutside = false;
            constructible.allowedOnCeiling = false;
            constructible.allowedOnGround = false;
            constructible.allowedOnWall = true;
            constructible.allowedOnConstructables = false;
            constructible.controlModelState = true;
            constructible.rotationEnabled = false;
            constructible.techType = VModFabTechType; // This was necessary to correctly associate the recipe at building time
            constructible.model = cyclopsFabModel;

            return cyclopsFabPrefab;
        }
    }
}
