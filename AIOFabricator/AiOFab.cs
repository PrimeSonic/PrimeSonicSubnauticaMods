namespace AIOFabricator
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal class AiOFab : CustomFabricator
    {
        private const string ToolTipFormat = "{0}Menu_{1}";
        private const string TabSpriteFormat = "{0}_{1}";

        private const string AioFabScheme = "AiOFab";
        private const string FabricatorScheme = "Fabricator";
        private const string WorkBenchScheme = "Workbench";
        private const string SeamothUpgradesScheme = "SeamothUpgrades";
        private const string MapRoomScheme = "MapRoom";
        private const string CyclopsFabScheme = "CyclopsFabricator";

        private static CraftTree craftTree;
        private static Texture2D texture;
        private static Atlas.Sprite sprite;

        public AiOFab()
            : base(AioFabScheme,
                   "All-In-One Fabricator",
                   "Multi-fuction fabricator capable of synthesizing all types of blueprints.")
        {
            OnStartedPatching += LoadImageFiles;
            OnFinishedPatching += () =>
            {
                RegisterTopLevelTab(FabricatorScheme, "Fabricator", TechType.Fabricator);
                RegisterTopLevelTab(WorkBenchScheme, "Modification Station", TechType.Workbench);
                RegisterTopLevelTab(SeamothUpgradesScheme, "Vehicle Upgrades", TechType.BaseUpgradeConsole);
                RegisterTopLevelTab(MapRoomScheme, "Scanner Room", TechType.BaseMapRoom);
                RegisterTopLevelTab(CyclopsFabScheme, "Cyclops Upgrades", TechType.Cyclops);

                this.Root.CraftTreeCreation = CreateCraftingTree;
            };
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
                sprite = ImageUtils.LoadSpriteFromFile(fileLocation);
            }
        }

        private CraftTree CreateCraftingTree()
        {
            if (craftTree == null)
            {
                Dictionary<string, string> langLines = Language.main.strings;
                Dictionary<string, Atlas.Sprite> group = SpriteManager.groups[SpriteManager.Group.Category];
                Dictionary<string, Atlas.Sprite> atlas = Atlas.nameToAtlas["Categories"]._nameToSprite;

                CraftNode fab = CraftTree.FabricatorScheme();
                CloneTabDetails(FabricatorScheme, fab, ref langLines, ref group, ref atlas);

                CraftNode wb = CraftTree.WorkbenchScheme();
                CloneTabDetails(WorkBenchScheme, wb, ref langLines, ref group, ref atlas);

                CraftNode su = CraftTree.SeamothUpgradesScheme();
                CloneTabDetails(SeamothUpgradesScheme, su, ref langLines, ref group, ref atlas);

                CraftNode map = CraftTree.MapRoomSheme();
                CloneTabDetails(MapRoomScheme, map, ref langLines, ref group, ref atlas);

                CraftNode cy = CraftTree.CyclopsFabricatorScheme();
                CloneTabDetails(CyclopsFabScheme, cy, ref langLines, ref group, ref atlas);

                CraftNode aioRoot = new CraftNode("Root").AddNode(fab, wb, su, map, cy);

                craftTree = new CraftTree(AioFabScheme, aioRoot);
            }

            return craftTree;
        }

        public override bool UseCustomTint => true;
        public override Color ColorTint { get; } = new Color(0.7f, 0.7f, 0.9f);

        public override TechType RequiredForUnlock => TechType.Workbench;

        protected override Atlas.Sprite GetItemSprite()
        {
            return sprite;
        }

        public override GameObject GetGameObject()
        {
            GameObject gObj = base.GetGameObject();

            // Set the custom texture
            SkinnedMeshRenderer skinnedMeshRenderer = gObj.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderer.material.mainTexture = texture;

            // Change size
            Vector3 scale = gObj.transform.localScale;
            const float factor = 1.25f;
            gObj.transform.localScale = new Vector3(scale.x * factor, scale.y * factor, scale.z * factor);

            return gObj;
        }

        public override Models Model => Models.Fabricator;

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.Titanium, 3),
                    new Ingredient(TechType.ComputerChip, 2),
                    new Ingredient(TechType.Diamond, 2),
                    new Ingredient(TechType.Lead, 1),
                    new Ingredient(TechType.Magnetite, 1)
                }
            };
        }

        private void RegisterTopLevelTab(string scheme, string tabDisplayName, TechType tabIconId)
        {
            SpriteHandler.RegisterSprite(SpriteManager.Group.Category, string.Format(TabSpriteFormat, AioFabScheme, scheme), SpriteManager.Get(tabIconId));
            LanguageHandler.SetLanguageLine(string.Format(ToolTipFormat, AioFabScheme, scheme), tabDisplayName);
        }

        private void CloneTabDetails(string scheme, CraftNode node, ref Dictionary<string, string> languageLines, ref Dictionary<string, Atlas.Sprite> group, ref Dictionary<string, Atlas.Sprite> atlas)
        {
            switch (node.action)
            {
                case TreeAction.Craft:
                    return;
                case TreeAction.None:
                    node.id = scheme;
                    node.action = TreeAction.Expand;
                    break;
                case TreeAction.Expand:
                {
                    string clonedLangKey = string.Format(ToolTipFormat, AioFabScheme, node.id);

                    if (!languageLines.ContainsKey(clonedLangKey))
                    {
                        string origLangKey = string.Format(ToolTipFormat, scheme, node.id);
                        languageLines.Add(clonedLangKey, languageLines[origLangKey]);
                        //Console.WriteLine($"[AIOFabricator] Cloned language key '{origLangKey}' > '{clonedLangKey}'");
                    }

                    string origSpriteKey = string.Format(TabSpriteFormat, scheme, node.id);
                    string clonedSpriteKey = string.Format(TabSpriteFormat, AioFabScheme, node.id);

                    if (group.TryGetValue(origSpriteKey, out Atlas.Sprite groupSprite))
                    {
                        group[clonedSpriteKey] = groupSprite;
                        //Console.WriteLine($"[AIOFabricator] Cloned Group Sprite '{origSpriteKey}' > '{clonedSpriteKey}'");
                    }
                    else if (atlas.TryGetValue(origSpriteKey, out Atlas.Sprite resourceSprite))
                    {
                        atlas[clonedSpriteKey] = resourceSprite;
                        //Console.WriteLine($"[AIOFabricator] Cloned Resource Sprite '{origSpriteKey}' > '{clonedSpriteKey}'");
                    }

                    break;
                }
            }

            foreach (CraftNode innerNode in node)
                CloneTabDetails(scheme, innerNode, ref languageLines, ref group, ref atlas);
        }
    }
}
