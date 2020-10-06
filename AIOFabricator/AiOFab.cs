namespace AIOFabricator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;
#if SUBNAUTICA
    using Sprite = Atlas.Sprite;
    using RecipeData = SMLHelper.V2.Crafting.TechData;
#endif

    internal class AiOFab : CustomFabricator
    {
        private const string DisplayNameFormat = "{0}Menu_{1}";
        private const string TabSpriteFormat = "{0}_{1}";

        private const string AioFabScheme = "AiOFab";
        private const string FabricatorScheme = "Fabricator";
        private const string WorkBenchScheme = "Workbench";
        private const string SeamothUpgradesScheme = "SeamothUpgrades";
        private const string MapRoomScheme = "MapRoom";
#if SUBNAUTICA
        private const string CyclopsFabScheme = "CyclopsFabricator";
#elif BELOWZERO
        private const string SeaTruckFabScheme = "SeaTruckFabricator";
#endif

        private static CraftTree craftTree;
        private static Texture2D texture;
        private static Sprite sprite;

        public AiOFab()
            : base(AioFabScheme,
                   "All-In-One Fabricator",
                   "Multi-fuction fabricator capable of synthesizing most blueprints.")
        {
            OnStartedPatching += LoadImageFiles;
            OnFinishedPatching += RegisterCraftTreeBasics;
        }

        private void LoadImageFiles()
        {
            string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string folderPath = Path.Combine(executingLocation, "Assets");

            if (texture == null)
            {
                string fileLocation = Path.Combine(folderPath, "AiOFabTex.png");
                texture = ImageUtils.LoadTextureFromFile(fileLocation);
            }

            if (sprite == null)
            {
                string fileLocation = Path.Combine(folderPath, "AiOFab.png");
                sprite = ImageUtils.LoadSpriteFromFile(fileLocation) ?? SpriteManager.Get(TechType.Fabricator);
            }
        }

        private void RegisterCraftTreeBasics()
        {
            RegisterTopLevelVanillaTab(FabricatorScheme, "Fabricator", TechType.Fabricator);
            RegisterTopLevelVanillaTab(WorkBenchScheme, "Modification Station", TechType.Workbench);
            RegisterTopLevelVanillaTab(SeamothUpgradesScheme, "Vehicle Upgrades", TechType.BaseUpgradeConsole);
            RegisterTopLevelVanillaTab(MapRoomScheme, "Scanner Room", TechType.BaseMapRoom);
#if SUBNAUTICA
            RegisterTopLevelVanillaTab(CyclopsFabScheme, "Cyclops Upgrades", TechType.Cyclops);
#elif BELOWZERO
            RegisterTopLevelVanillaTab(SeaTruckFabScheme, "SeaTruck Fabricator", TechType.SeaTruck);
#endif

            this.Root.CraftTreeCreation = CreateCraftingTree;
        }

        private CraftTree CreateCraftingTree()
        {
            if (craftTree == null)
            {
                Dictionary<string, string> langLines = Language.main.strings;

                var atlasName = SpriteManager.mapping[SpriteManager.Group.Category];
#if SUBNAUTICA
                var group = Atlas.GetAtlas(atlasName).nameToSprite;
#elif BELOWZERO
                var group = SpriteManager.atlases[atlasName];
#endif
                List<CraftNode> craftNodes = new List<CraftNode>();

                CraftNode fab = CraftTree.FabricatorScheme();
                CloneTabDetails(FabricatorScheme, fab, ref langLines, ref group);
                craftNodes.Add(fab);

                CraftNode wb = CraftTree.WorkbenchScheme();
                CloneTabDetails(WorkBenchScheme, wb, ref langLines, ref group);
                craftNodes.Add(wb);

                CraftNode su = CraftTree.SeamothUpgradesScheme();
                CloneTabDetails(SeamothUpgradesScheme, su, ref langLines, ref group);
                craftNodes.Add(su);

                CraftNode map = CraftTree.MapRoomSheme();
                CloneTabDetails(MapRoomScheme, map, ref langLines, ref group);
                craftNodes.Add(map);
#if SUBNAUTICA
                CraftNode cy = CraftTree.CyclopsFabricatorScheme();
                CloneTabDetails(CyclopsFabScheme, cy, ref langLines, ref group);
                craftNodes.Add(cy);
#elif BELOWZERO
                CraftNode st = CraftTree.SeaTruckFabricatorScheme();
                CloneTabDetails(SeaTruckFabScheme, st, ref langLines, ref group);
                craftNodes.Add(st);
#endif

                CraftNode aioRoot = new CraftNode("Root").AddNode(craftNodes.ToArray());

                Type smlCTPatcher = typeof(CraftTreeHandler).Assembly.GetType("SMLHelper.V2.Patchers.CraftTreePatcher");
                var customTrees = (Dictionary<CraftTree.Type, ModCraftTreeRoot>)smlCTPatcher.GetField("CustomTrees", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                foreach (KeyValuePair<CraftTree.Type, ModCraftTreeRoot> entry in customTrees)
                {
                    if (entry.Key == this.TreeTypeID)
                        continue;

                    CraftTree tree = entry.Value.CraftTreeCreation.Invoke();
                    CraftNode root = tree.nodes;
                    string scheme = entry.Key.ToString();

                    CloneTabDetails(scheme, root, ref langLines, ref group);
                    CloneTopLevelModTab(scheme, ref langLines, ref group);
                    aioRoot.AddNode(root);
                }

                craftTree = new CraftTree(AioFabScheme, aioRoot);
            }

            return craftTree;
        }

        protected override Sprite GetItemSprite()
        {
            return sprite;
        }

        public override GameObject GetGameObject()
        {
            GameObject gObj = base.GetGameObject();

            if (texture != null)
            {
                // Set the custom texture
                SkinnedMeshRenderer skinnedMeshRenderer = gObj.GetComponentInChildren<SkinnedMeshRenderer>();
                skinnedMeshRenderer.material.mainTexture = texture;
            }

            // Change size
            Vector3 scale = gObj.transform.localScale;
            const float factor = 1.25f;
            gObj.transform.localScale = new Vector3(scale.x * factor, scale.y * factor, scale.z * factor);

            return gObj;
        }

        public override Models Model => Models.Fabricator;

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.Titanium, 3),
                    new Ingredient(TechType.ComputerChip, 2),
                    new Ingredient(TechType.WiringKit, 1),
                    new Ingredient(TechType.Diamond, 1),
                    new Ingredient(TechType.AluminumOxide, 1),
                    new Ingredient(TechType.Magnetite, 1)
                }
            };
        }

        private void RegisterTopLevelVanillaTab(string scheme, string tabDisplayName, TechType tabIconId)
        {
            SpriteHandler.RegisterSprite(SpriteManager.Group.Category, string.Format(TabSpriteFormat, AioFabScheme, scheme), SpriteManager.Get(tabIconId));
            LanguageHandler.SetLanguageLine(string.Format(DisplayNameFormat, AioFabScheme, scheme), tabDisplayName);
        }

        private void CloneTopLevelModTab(string scheme, ref Dictionary<string, string> languageLines, ref Dictionary<string, Sprite> group)
        {
            string clonedLangKey = string.Format(DisplayNameFormat, AioFabScheme, scheme);

            if (!languageLines.ContainsKey(clonedLangKey) && languageLines.TryGetValue(scheme, out string origString))
            {
                LanguageHandler.SetLanguageLine(clonedLangKey, origString);
                languageLines[clonedLangKey] = origString;
            }
            else
            {
                Console.WriteLine($"[AIOFabricator][WARN] Problem cloning language line for '{scheme}:root'{Environment.NewLine}Language resource not found");
            }

            string clonedSpriteKey = string.Format(TabSpriteFormat, AioFabScheme, scheme);

            if (TechTypeExtensions.FromString(scheme, out TechType techType, true))
            {
                var rootSprite = SpriteManager.Get(techType);
                SpriteHandler.RegisterSprite(SpriteManager.Group.Category, clonedSpriteKey, rootSprite);
                group[clonedSpriteKey] = rootSprite;
            }
            else
            {
                Console.WriteLine($"[AIOFabricator][WARN] Problem cloning sprite for '{scheme}:root'{Environment.NewLine}Sprite resource not found");
            }
        }

        private void CloneTabDetails(string scheme, CraftNode node, ref Dictionary<string, string> languageLines, ref Dictionary<string, Sprite> group)
        {
            if (node == null)
                return;

            switch (node.action)
            {
                case TreeAction.Craft:
                    return;
                case TreeAction.None:
                    node.id = scheme;
                    node.action = TreeAction.Expand;
                    Console.WriteLine($"[AIOFabricator][INFO] Cloning tab nodes for '{scheme}:{node.id}'");
                    break;
                case TreeAction.Expand:
                {
                    string clonedLangKey = string.Format(DisplayNameFormat, AioFabScheme, node.id);
                    string origLangKey = string.Format(DisplayNameFormat, scheme, node.id);

                    try
                    {
                        if (languageLines != null && !languageLines.ContainsKey(clonedLangKey) && languageLines.TryGetValue(origLangKey, out string origString))
                        {
                            LanguageHandler.SetLanguageLine(clonedLangKey, origString);
                            languageLines[clonedLangKey] = origString;
                        }
                        else
                        {
                            Console.WriteLine($"[AIOFabricator][WARN] Problem cloning language line for '{scheme}:{node.id}'{Environment.NewLine}Language resource not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[AIOFabricator][ERROR] Error cloning language line for '{scheme}:{node.id}'{Environment.NewLine}{ex.Message}");
                    }

                    string origSpriteKey = string.Format(TabSpriteFormat, scheme, node.id);
                    string clonedSpriteKey = string.Format(TabSpriteFormat, AioFabScheme, node.id);
                    try
                    {
                        if (group != null && group.TryGetValue(origSpriteKey, out Sprite groupSprite))
                        {
                            SpriteHandler.RegisterSprite(SpriteManager.Group.Category, clonedSpriteKey, groupSprite);
                            group[clonedSpriteKey] = groupSprite;
                        }
                        else
                        {
                            Console.WriteLine($"[AIOFabricator][WARN] Problem cloning sprite for '{scheme}:{node.id}'{Environment.NewLine}Sprite resource not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[AIOFabricator][ERROR] Error cloning sprite for '{scheme}:{node.id}'{Environment.NewLine}{ex.Message}");
                    }
                    break;
                }
            }

            foreach (CraftNode innerNode in node)
                CloneTabDetails(scheme, innerNode, ref languageLines, ref group);
        }
    }
}
