namespace VModFabricator
{
    using System.Collections.Generic;
    using Common;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    public class VModFabricatorModule : ModPrefab
    {
        public CraftTree.Type TreeTypeID { get; private set; }

        // This name will be used as both the new TechType of the buildable fabricator and the CraftTree Type for the custom crafting tree.
        public const string NameID = "VModFabricator";

        // The text you'll see in-game when you mouseover over it.
        public const string FriendlyName = "Vehicle Module Fabricator";

        public const string HandOverText = "UseVModFabricator";

        private readonly ModdedItemsConfig ModdedItems = new ModdedItemsConfig();

        internal VModFabricatorModule() : base(NameID, $"{NameID}PreFab")
        {
        }

        public void Patch()
        {
            ModdedItems.Initialize();

            // Create new Craft Tree Type
            CreateCustomTree(out CraftTree.Type craftType);
            this.TreeTypeID = craftType;

            // Create a new TechType for new fabricator
            this.TechType = TechTypeHandler.AddTechType(
                                    internalName: NameID,
                                    displayName: FriendlyName,
                                    tooltip: "Construct vehicle upgrade modules from the comfort of your favorite habitat or cyclops.",
                                    sprite: ImageUtils.LoadSpriteFromFile(@"./QMods/VModFabricator/Assets/VModFabIcon.png"),
                                    unlockAtStart: false);

            // Create a Recipie for the new TechType
            var customFabRecipe = new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[4]
                             {
                                 new Ingredient(TechType.Titanium, 2),
                                 new Ingredient(TechType.ComputerChip, 1),
                                 new Ingredient(TechType.Diamond, 1),
                                 new Ingredient(TechType.Lead, 1),
                             })
            };

            // Add the new TechType to the buildables
            CraftDataHandler.AddBuildable(this.TechType);

            // Add the new TechType to the group of Interior Module buildables
            CraftDataHandler.AddToGroup(TechGroup.InteriorModules, TechCategory.InteriorModule, this.TechType);

            LanguageHandler.SetLanguageLine(HandOverText, "Use Vehicle Module Fabricator");

            // Set the buildable prefab
            PrefabHandler.RegisterPrefab(this);

            // Associate the recipie to the new TechType
            CraftDataHandler.SetTechData(this.TechType, customFabRecipe);

            // Set which blueprints unlock the VMod Fabricator
            string unlockMessage = $"{FriendlyName} blueprint discovered!";
            var unlockThis = new TechType[1] { this.TechType };
            KnownTechHandler.SetAnalysisTechEntry(TechType.Workbench, unlockThis, unlockMessage);
            KnownTechHandler.SetAnalysisTechEntry(TechType.BaseUpgradeConsole, unlockThis, unlockMessage);
            KnownTechHandler.SetAnalysisTechEntry(TechType.Cyclops, unlockThis, unlockMessage);
        }

        private void CreateCustomTree(out CraftTree.Type craftType)
        {
            if (!ModdedItems.IsInitialized)
                ModdedItems.Initialize();

            ModCraftTreeRoot rootNode = CraftTreeHandler.CreateCustomCraftTreeAndType(NameID, out craftType);

            ModCraftTreeTab cyclopsTab = rootNode.AddTabNode("CyclopsModules", "Cyclops Modules", SpriteManager.Get(SpriteManager.Group.Category, "Workbench_CyclopsMenu"));
            ModCraftTreeTab cyclopsAbilityTab = cyclopsTab.AddTabNode("CyclopsAbilityModules", "Ability Modules", SpriteManager.Get(TechType.CyclopsShieldModule));
            cyclopsAbilityTab.AddCraftingNode(TechType.CyclopsShieldModule,
                           TechType.CyclopsSonarModule,
                           TechType.CyclopsSeamothRepairModule,
                           TechType.CyclopsFireSuppressionModule,
                           TechType.CyclopsDecoyModule);
            ModdedItems.AddModdedModules(cyclopsAbilityTab);

            cyclopsAbilityTab.AddModdedCraftingNode("CyclopsSpeedModule");
            ModCraftTreeTab cyclopsDepthTab = cyclopsTab.AddTabNode("CyclopsDepthModules", "Depth Modules", SpriteManager.Get(TechType.CyclopsHullModule1));
            cyclopsDepthTab.AddCraftingNode(TechType.CyclopsHullModule1,
                                            TechType.CyclopsHullModule2,
                                            TechType.CyclopsHullModule3);

            ModCraftTreeTab cyclopsPowerTab = cyclopsTab.AddTabNode("CyclopsPowerModules", "Power Modules", SpriteManager.Get(TechType.PowerUpgradeModule));
            cyclopsPowerTab.AddCraftingNode(TechType.PowerUpgradeModule);
            ModdedItems.AddModdedModules(cyclopsPowerTab);

            ModCraftTreeTab cyclopsRechargTab = cyclopsTab.AddTabNode("CyclopsRechargeTab", "Recharge Modules", SpriteManager.Get(TechType.SeamothSolarCharge));
            cyclopsRechargTab.AddCraftingNode(TechType.CyclopsThermalReactorModule);
            ModdedItems.AddModdedModules(cyclopsRechargTab);

            ModCraftTreeTab exosuitTab = rootNode.AddTabNode("ExosuitModules", "Prawn Suit Modules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_ExosuitModules"));
            ModCraftTreeTab exosuitDepthTab = exosuitTab.AddTabNode("ExosuitDepthModules", "Depth Modules", SpriteManager.Get(TechType.ExoHullModule1));
            exosuitDepthTab.AddCraftingNode(TechType.ExoHullModule1,
                                            TechType.ExoHullModule2);
            exosuitTab.AddCraftingNode(TechType.ExosuitJetUpgradeModule,
                                       TechType.ExosuitPropulsionArmModule,
                                       TechType.ExosuitGrapplingArmModule,
                                       TechType.ExosuitDrillArmModule,
                                       TechType.ExosuitTorpedoArmModule);
            ModdedItems.AddModdedModules(exosuitTab);

            ModCraftTreeTab seamothTab = rootNode.AddTabNode("SeamothModules", "Seamoth Modules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_SeamothModules"));
            ModCraftTreeTab seamothDepthTab = seamothTab.AddTabNode("SeamothDepthModules", "Depth Modules", SpriteManager.Get(TechType.VehicleHullModule1));
            seamothDepthTab.AddCraftingNode(TechType.VehicleHullModule1,
                                            TechType.VehicleHullModule2,
                                            TechType.VehicleHullModule3);
            ModdedItems.AddModdedModules(seamothDepthTab);

            ModCraftTreeTab seamothAbilityTab = seamothTab.AddTabNode("SeamothAbilityModules", "Ability Modules", SpriteManager.Get(TechType.SeamothElectricalDefense));
            seamothAbilityTab.AddCraftingNode(TechType.SeamothElectricalDefense,
                                              TechType.SeamothSonarModule,
                                              TechType.SeamothTorpedoModule);
            seamothTab.AddCraftingNode(TechType.SeamothSolarCharge);
            ModdedItems.AddModdedModules(seamothAbilityTab);
            ModdedItems.AddModdedModules(seamothTab);

            ModCraftTreeTab commonTab = rootNode.AddTabNode("CommonModules", "Common Modules", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_CommonModules"));
            commonTab.AddCraftingNode(TechType.VehicleArmorPlating,
                                      TechType.VehiclePowerUpgradeModule,
                                      TechType.VehicleStorageModule);
            ModdedItems.AddModdedModules(commonTab);

            ModCraftTreeTab torpedoesTab = rootNode.AddTabNode("TorpedoesModules", "Torpedoes", SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_Torpedoes"));
            torpedoesTab.AddCraftingNode(TechType.WhirlpoolTorpedo,
                                         TechType.GasTorpedo);
            ModdedItems.AddModdedModules(torpedoesTab);
        }

        public override GameObject GetGameObject()
        {
            // Instantiate CyclopsFabricator object
            var cyclopsFabPrefab = GameObject.Instantiate(Resources.Load<GameObject>("Submarine/Build/CyclopsFabricator"));

            // Retrieve sub game objects
            GameObject cyclopsFabLight = cyclopsFabPrefab.FindChild("fabricatorLight");
            GameObject cyclopsFabModel = cyclopsFabPrefab.FindChild("submarine_fabricator_03");

            // Add prefab ID because CyclopsFabricator normaly doesn't have one
            PrefabIdentifier prefabId = cyclopsFabPrefab.AddComponent<PrefabIdentifier>();
            prefabId.ClassId = NameID;
            prefabId.name = FriendlyName;

            // Add tech tag because CyclopsFabricator normaly doesn't have one
            TechTag techTag = cyclopsFabPrefab.AddComponent<TechTag>();
            techTag.type = this.TechType;

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
            SkyApplier skyApplier = cyclopsFabPrefab.GetComponent<SkyApplier>();
            skyApplier.renderers = cyclopsFabPrefab.GetComponentsInChildren<Renderer>();
            skyApplier.anchorSky = Skies.Auto;

            // Associate custom craft tree to the fabricator
            Fabricator fabricator = cyclopsFabPrefab.GetComponent<Fabricator>();
            fabricator.craftTree = this.TreeTypeID;
            fabricator.handOverText = HandOverText;

            // Associate power relay
            GhostCrafter ghost = fabricator.GetComponent<GhostCrafter>();
            var powerRelay = new PowerRelay(); // This isn't correct, but nothing else seems to work.

            ghost.powerRelay = powerRelay;

            // Add constructable - This prefab normally isn't constructed.
            Constructable constructible = cyclopsFabPrefab.AddComponent<Constructable>();

            constructible.allowedInBase = true;
            constructible.allowedInSub = true;
            constructible.allowedOutside = false;
            constructible.allowedOnCeiling = false;
            constructible.allowedOnGround = false;
            constructible.allowedOnWall = true;
            constructible.allowedOnConstructables = false;
            constructible.controlModelState = true;
            constructible.rotationEnabled = false;
            constructible.techType = this.TechType; // This was necessary to correctly associate the recipe at building time
            constructible.model = cyclopsFabModel;

            return cyclopsFabPrefab;
        }
    }
}
