namespace CustomCraft2SML
{
    using System;
    using System.IO;
    using System.Text;
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization;
    using Common.EasyMarkup;
    using System.Collections.Generic;

    public class QPatch
    {
        private static readonly string FolderRoot = $"./QMods/{CustomCraft.RootModName}/";
        private static readonly string CustomSizesFile = FolderRoot + "CustomSizes.txt";
        private static readonly string ModifiedRecipesFile = FolderRoot + "ModifiedRecipes.txt";
        private static readonly string AddedRecipiesFile = FolderRoot + "AddedRecipes.txt";
        private static readonly string HowToFile = FolderRoot + "README_HowToUseThisMod.txt";
        private static readonly string ReadMeVersionLine = $"# How to use {CustomCraft.RootModName} (Revision 4) #";

        private static readonly string SamplesFolder = FolderRoot + "SampleFiles/";
        private static readonly string OriginalsFile = SamplesFolder + "OriginalRecipes.txt";

        private static CustomSizeList customSizeList;
        private static ModifiedRecipeList modifiedRecipeList;
        private static AddedRecipeList addedRecipeList;

        public static void Patch()
        {
            Logger.Log("Loading files begin");

            PatchCustomSizes();

            PatchModifiedRecipes();

            PatchAddedRecipes();

            HandleReadMeFile();

            try

            {
                GenerateOriginalRecipes();
            }
            catch (IndexOutOfRangeException outEx)
            {
                Logger.Log(outEx.ToString());
            }

            Logger.Log("Loading files complete");
        }

        private static void HandleReadMeFile()
        {
            if (!File.Exists(HowToFile))
            {
                File.WriteAllText(HowToFile, ReadMeFileText());
                Logger.Log($"{HowToFile} file not found. File created.");
            }
            else
            {
                string[] readmeLines = File.ReadAllLines(HowToFile);

                if (readmeLines.Length < 1 || readmeLines[0] != ReadMeVersionLine)
                {
                    File.WriteAllText(HowToFile, ReadMeFileText());
                    Logger.Log($"{HowToFile} out of date. Regenerated with new version.");
                }
            }
        }

        private static string ReadMeFileText()
        {
            var builder = new StringBuilder();
            builder.AppendLine(ReadMeVersionLine);
            builder.AppendLine("# -------------------------------------------- #");
            builder.AppendLine();
            builder.AppendLine($"# {CustomCraft.RootModName} uses simple text files to send simple requests to SMLHelper, no additional DLLs needed #");
            builder.AppendLine("# This can be great for those who have a few simple and specific ideas in mind but aren't able to create a whole mod themselves #");
            builder.AppendLine("# Special thanks to Nexus modder Iw23J, creator of the original Custom Craft mod, who showed us how much we can empower players to take modding into their own hands #");
            builder.AppendLine("# -------------------------------------------- #");
            builder.AppendLine("# Currently, this mod includes the following features: #");
            builder.AppendLine("# 1 - Customize the space occupied by an inventory item #");
            builder.AppendLine("# 2 - Modify an existing crafting recipe: #");
            builder.AppendLine("#   a - You can change a recipe's required ingredients in any way #");
            builder.AppendLine("#   b - You can alter how many copies of the item are created when you craft the recipe #");
            builder.AppendLine("#   c - You can also modify the recipe's linked items, those being items also created along side the main one #");
            builder.AppendLine("#   d - NEW! You can now also modify what other items will be unlocked when you analyze or craft this one #");
            builder.AppendLine("#   e - NEW! You can now also set if this recipe should be unlocked at the start or not #");
            builder.AppendLine("# 3 - Adding new recipes and placing them into any of the existing fabricators #");
            builder.AppendLine("#   a - Added recipes work exactly like Modified recipes, with the addition of a Path to where that recipe should go #");
            builder.AppendLine($"# Remember that only the standard in-game items can be used with {CustomCraft.RootModName} #");
            builder.AppendLine("# Modded items can't be used at this time #");
            builder.AppendLine("# Additional features may be added in the future so keep an eye on the Nexus mod page #");
            builder.AppendLine("# -------------------------------------------- #");
            builder.AppendLine();
            builder.AppendLine($"# Writing the text files that {CustomCraft.RootModName} uses can be simple if you're paying attention #");
            builder.AppendLine("# You can even copy the text from the sample files directly and use them right away without any changes #");
            builder.AppendLine("# When you want to add a new item reference, remember that the 'ItemID' is the internal spawn ID of that item #");
            builder.AppendLine("# You can visit the Subnautica Wikia for the full list of item IDs at http://subnautica.wikia.com/wiki/Obtainable_Items #");
            builder.AppendLine("# NEW! As an added bonus, a file containing all the original game's recipes has been generated in the SampleFiles folder #");
            builder.AppendLine("# You can copy and paste individual recipes from that file into your ModifiedRecipes file and tweak them to your liking #");
            builder.AppendLine();
            builder.AppendLine("# -------------------------------------------- #");
            builder.AppendLine("# A quick overview of the less obvious keys for Added and Modified recipes #");
            builder.AppendLine("# LinkedItemIDs: This is the list of items that will be created along side the main item when you craft the recipe #");
            builder.AppendLine("#     - If you want multiple copies of an item here, you need to write it to this list multiple times #");
            builder.AppendLine("# AmountCrafted: This is how many copies of the item will be created when you craft the recipe #");
            builder.AppendLine("# ForceUnlockAtStart: When you set this to 'YES' this item will be forced to be unlocked from the very start of the game #");
            builder.AppendLine("#     - You can use it to have Modified recipes early or to set if have Added recipes should wait to be unlocked by something else #");
            builder.AppendLine("#     - If you don't include this ForceUnlockAtStart, then default behavior will be used #");
            builder.AppendLine("#         - By default, Modified recipes will be unlocked as they normally would be #");
            builder.AppendLine("#         - By default, Added recipes will be forced to be unlocked at the start of the game as a safety #");
            builder.AppendLine("# Unlocks: This is a list of other ItemIDs that will be unlocked when you analyze or craft this recipe #");
            builder.AppendLine("#     - You can use this along with 'ForceUnlockAtStart:NO' to have recipes be unlocked when you want them to be #");
            builder.AppendLine("# Ingredients: This key is now optional #");
            builder.AppendLine("#     - You can leave this out of your recipe block and the original recipe won't be altered in any way #");
            builder.AppendLine("# -------------------------------------------- #");
            builder.AppendLine();
            builder.AppendLine("# You'll notice that a lot of text is written between these hash signs (or pound sign or hashtag if you prefer) #");
            builder.AppendLine("# Any text in between these symbols is safely ignored by the mod's text reader #");
            builder.AppendLine("# Use these if you want to leave notes or comments for yourself in the txt files #");
            builder.AppendLine("# Whitespace is also ignored, so feel free to make your files as flat or as indented as you prefer #");
            builder.AppendLine("# -------------------------------------------- #");
            builder.AppendLine();
            builder.AppendLine("# Once you've created your txt files, go ahead and launch the game #");
            builder.AppendLine("# Assuming you've got everything configured correctly, your customizations will appear on your next game #");
            builder.AppendLine();
            builder.AppendLine("# -------------------------------------------- #");
            builder.AppendLine("# Extra tip: When adding recipes, you must use the correct path to the tab you wish to add it to #");
            builder.AppendLine("# Provided here is a list of all the valid paths to all the standard tabs for all available fabricators #");
            builder.AppendLine();
            builder.AppendLine(PathHelper.GeneratePaths());
            builder.AppendLine("# -------------------------------------------- #");
            builder.AppendLine("# Enjoy and happy modding #");

            return builder.ToString();
        }

        private static void PatchAddedRecipes()
        {
            addedRecipeList = new AddedRecipeList();
            if (File.Exists(AddedRecipiesFile))
            {
                string serializedData = File.ReadAllText(AddedRecipiesFile);
                if (!string.IsNullOrEmpty(serializedData) && addedRecipeList.Deserialize(serializedData) && addedRecipeList.Count > 0)
                {
                    foreach (IAddedRecipe item in addedRecipeList)
                    {
                        try
                        {
                            CustomCraft.AddRecipe(item);
                        }
                        catch
                        {
                            Logger.Log($"Error on AddRecipe{Environment.NewLine}" +
                                        $"Entry with error:{Environment.NewLine}" +
                                        $"{item}");
                        }
                    }

                    Logger.Log($"AddedRecipies loaded. File reformatted.");
                    File.WriteAllText(AddedRecipiesFile, addedRecipeList.PrintyPrint());
                }
                else
                {
                    Logger.Log($"No AddedRecipes were loaded. File was empty or malformed.");
                }
            }
            else
            {
                File.WriteAllText(AddedRecipiesFile, $"# Added Recipes #{Environment.NewLine}" +
                    $"# Check the AddedRecipes_Samples.txt file in the SampleFiles folder for details on how to add recipes for items normally not craftable #{Environment.NewLine}");
                Logger.Log($"{AddedRecipiesFile} file not found. Empty file created.");
            }
        }

        private static void PatchModifiedRecipes()
        {
            modifiedRecipeList = new ModifiedRecipeList();
            if (File.Exists(ModifiedRecipesFile))
            {
                string serializedData = File.ReadAllText(ModifiedRecipesFile);
                if (!string.IsNullOrEmpty(serializedData) && modifiedRecipeList.Deserialize(serializedData) && modifiedRecipeList.Count > 0)
                {
                    foreach (IModifiedRecipe item in modifiedRecipeList)
                    {
                        try
                        {
                            CustomCraft.ModifyRecipe(item);
                        }
                        catch
                        {
                            Logger.Log($"Error on ModifyRecipe{Environment.NewLine}" +
                                        $"Entry with error:{Environment.NewLine}" +
                                        $"{item}");
                        }
                    }

                    Logger.Log($"ModifiedRecipes loaded. File reformatted.");
                    File.WriteAllText(ModifiedRecipesFile, modifiedRecipeList.PrintyPrint());
                }
                else
                {
                    Logger.Log($"No ModifiedRecipes were loaded. File was empty or malformed.");
                }
            }
            else
            {
                File.WriteAllText(ModifiedRecipesFile, $"# Modified Recipes #{Environment.NewLine}" +
                    $"# Check the ModifiedRecipes_Samples.txt file in the SampleFiles folder for details on how to alter existing crafting recipes #{Environment.NewLine}");
                Logger.Log($"{ModifiedRecipesFile} file not found. Empty file created.");
            }
        }

        private static void PatchCustomSizes()
        {
            customSizeList = new CustomSizeList();
            if (File.Exists(CustomSizesFile))
            {
                string serializedData = File.ReadAllText(CustomSizesFile);
                if (!string.IsNullOrEmpty(serializedData) && customSizeList.Deserialize(serializedData) && customSizeList.Count > 0)
                {
                    foreach (ICustomSize customSize in customSizeList)
                    {
                        try
                        {
                            CustomCraft.CustomizeItemSize(customSize);
                        }
                        catch
                        {
                            Logger.Log($"Error on CustomizeItemSize{Environment.NewLine}" +
                                        $"Entry with error:{Environment.NewLine}" +
                                        $"{customSize}");
                        }
                    }

                    Logger.Log($"CustomSizes loaded. File reformatted.");
                    File.WriteAllText(CustomSizesFile, customSizeList.PrintyPrint());
                }
                else
                {
                    Logger.Log($"No CustomSizes were loaded. File was empty or malformed.");
                }
            }
            else
            {
                File.WriteAllText(CustomSizesFile,
                    $"# Custom Sizes go in this file #{Environment.NewLine}" +
                    $"# Check the CustomSizes_Samples.txt file in the SampleFiles folder for details on how to set your own custom sizes #{Environment.NewLine}");
                Logger.Log($"{CustomSizesFile} file not found. Empty file created.");
            }
        }

        private static void GenerateOriginalRecipes()
        {
            if (!Directory.Exists(SamplesFolder))
                Directory.CreateDirectory(SamplesFolder);

            if (File.Exists(OriginalsFile))
                return;

            List<string> printyPrints = GenerateOriginalsText();

            File.WriteAllLines(OriginalsFile, printyPrints.ToArray());

            Logger.Log($"{OriginalsFile} file not found. File created.");
        }

        public static List<string> GenerateOriginalsText()
        {
            var treeTypes = new CraftTree.Type[6]
                        {
                CraftTree.Type.Fabricator, CraftTree.Type.Constructor, CraftTree.Type.SeamothUpgrades,
                CraftTree.Type.Workbench,
                CraftTree.Type.MapRoom, CraftTree.Type.CyclopsFabricator
                        };

            var printyPrints = new List<string>(treeTypes.Length * 4 + 3)
            {
                "# This file was generated with all the existing recipes from all non-modded fabricators #",
                "#         You can copy samples from this file to use in your personal overrides         #",
                "# ------------------------------------------------------------------------------------- #",
            };

            foreach (CraftTree.Type tree in treeTypes)
            {
                ModifiedRecipeList list = GetOriginals(tree);

                printyPrints.Add(list.PrintyPrint());
                printyPrints.Add("");
                printyPrints.Add("# ------------------------------------------------------------------------------------- #");
                printyPrints.Add("");
            }

            return printyPrints;
        }

        public static ModifiedRecipeList GetOriginals(CraftTree.Type treeType)
        {
            CraftTree tree = CraftTree.GetTree(treeType);

            IEnumerator<CraftNode> mover = tree.nodes.Traverse(true);

            var originals = new ModifiedRecipeList($"{treeType}Originals");

            while (mover.MoveNext())
            {
                if (mover.Current.action == TreeAction.Craft && mover.Current.techType0 < TechType.Databox)
                    originals.Collections.Add(new ModifiedRecipe(mover.Current.techType0));
            };

            return originals;
        }

    }



}
