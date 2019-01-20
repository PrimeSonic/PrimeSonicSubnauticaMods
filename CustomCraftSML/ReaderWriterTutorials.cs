namespace CustomCraft2SML
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization;

    internal static partial class FileReaderWriter
    {
        internal const string RootModName = "CustomCraft2SML";
        internal const string FolderRoot = "./QMods/" + RootModName + "/";
        internal const string SamplesFolder = FolderRoot + "SampleFiles/";
        internal const string OriginalsFolder = FolderRoot + "OriginalRecipes/";
        internal const string HowToFile = FolderRoot + "README_HowToUseThisMod.txt";
        private static readonly string ReadMeVersionLine = $"# How to use {RootModName} (Revision {QuickLogger.GetAssemblyVersion()}) #";

        private static void GenerateOriginalRecipes()
        {
            if (!Directory.Exists(SamplesFolder))
                Directory.CreateDirectory(SamplesFolder);

            if (!Directory.Exists(OriginalsFolder))
                Directory.CreateDirectory(OriginalsFolder);

            var allGroups = ValidTechTypes.groups;

            foreach (TechGroup group in allGroups.Keys)
            {
                Dictionary<TechCategory, List<TechType>> groupCategories = allGroups[group];

                foreach (TechCategory category in groupCategories.Keys)
                {
                    string buildablesFile = $"{group}_{category}.txt";

                    if (File.Exists(OriginalsFolder + buildablesFile))
                        continue;

                    List<TechType> buildablesList = groupCategories[category];

                    GenerateOriginalsFile(category.ToString(), buildablesList, buildablesFile);
                }
            }

            GenerateOriginalBioFuels();
        }

        private static void GenerateOriginalBioFuels()
        {
            const string fileName = "BioReactor_Values.txt";

            if (File.Exists(OriginalsFolder + fileName))
                return;

            var allBioFuels = ValidBioFuels.charge;

            var bioFuelList = new CustomBioFuelList();
            foreach (TechType bioEnergyKey in allBioFuels.Keys)
                bioFuelList.Add(new CustomBioFuel { ItemID = bioEnergyKey.ToString(), Energy = allBioFuels[bioEnergyKey] });

            var printyPrints = new List<string>();
            printyPrints.AddRange(EmUtils.CommentTextLinesCentered(new string[]
            {
                "This file was generated with original BIoFuel energy values in the game",
                "You can copy individual entries from this file to use in your personal overrides",
                "--------------------------------------------------------------------------------",
            }));

            printyPrints.Add(bioFuelList.PrettyPrint());

            File.WriteAllLines(OriginalsFolder + fileName, printyPrints.ToArray());

            Logger.Log($"{fileName} file not found. File generated.");
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
            builder.AppendLine($"# {RootModName} uses simple text files to send simple requests to SMLHelper, no additional DLLs needed #");
            builder.AppendLine("# This can be great for those who have a few simple and specific ideas in mind but aren't able to create a whole mod themselves #");
            builder.AppendLine("# Special thanks to Nexus modder Iw23J, creator of the original Custom Craft mod, who showed us how much we can empower players to take modding into their own hands #");
            builder.AppendLine("# -------------------------------------------- #");
            builder.AppendLine("# Currently, this mod includes the following features: #");
            builder.AppendLine("# 1 - Customize the space occupied by an inventory item #");
            builder.AppendLine("# 2 - Modify an existing crafting recipe: #");
            builder.AppendLine("#   a - You can change a recipe's required ingredients in any way #");
            builder.AppendLine("#   b - You can alter how many copies of the item are created when you craft the recipe #");
            builder.AppendLine("#   c - You can also modify the recipe's linked items, those being items also created along side the main one #");
            builder.AppendLine("#   d - You can now also modify what other items will be unlocked when you analyze or craft this one #");
            builder.AppendLine("#   e - You can now also set if this recipe should be unlocked at the start or not #");
            builder.AppendLine("# 3 - Adding new recipes and placing them into any of the existing fabricators #");
            builder.AppendLine("#   a - Added recipes work exactly like Modified recipes, with the addition of a Path to where that recipe should go #");
            builder.AppendLine("# 4 - Customize the energy values of items in the BioReactor #");
            builder.AppendLine("#   a - This can also be used to make items compatible with the BioReactor that originally weren't. #");
            builder.AppendLine("# 5 - NEW! Add your own custom tabs into the fabricator crafting trees. #");
            builder.AppendLine($"# Remember that only the standard in-game items can be used with {RootModName} #");
            builder.AppendLine("# Modded items can't be used at this time #");
            builder.AppendLine("# Additional features may be added in the future so keep an eye on the Nexus mod page #");
            builder.AppendLine("# -------------------------------------------- #");
            builder.AppendLine();
            builder.AppendLine("# New feature: You can now have multiple files for added recipes, modified recipes, and custom sizes, all living together in the WorkingFiles folder. #");
            builder.AppendLine("# As of v1.2, the file name no longer matters. All files in the WorkingFiles folder will be read and parsed into the game. So name them however you want. #");
            builder.AppendLine("# As long as the file contains one, and only one, primary key of 'AddedRecipes:', 'ModifiedRecipes', or 'CustomSizes' with its entries, will be handled correctly. #");
            builder.AppendLine("# So if you've created your own customized crafts for and want to share them, you can now do so easily. #");
            builder.AppendLine("# Installation of your custom crafts on another player's system is as simple as copying the txt files into their WorkingFiles folder. #");
            builder.AppendLine("# -------------------------------------------- #");
            builder.AppendLine();
            builder.AppendLine($"# Writing the text files that {RootModName} uses can be simple if you're paying attention #");
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
            builder.AppendLine("#     - You can leave this out of your modified recipes. If you do, the original crafted amount will be used #");
            builder.AppendLine("# ForceUnlockAtStart: When you set this to 'YES' this item will be forced to be unlocked from the very start of the game #");
            builder.AppendLine("#     - You can use it to have Modified recipes early or to set if have Added recipes should wait to be unlocked by something else #");
            builder.AppendLine("#     - If you don't include ForceUnlockAtStart, then default behavior will be used: #");
            builder.AppendLine("#         - By default, Modified recipes will be unlocked as they normally would be #");
            builder.AppendLine("#         - By default, Added recipes will be forced to be unlocked at the start of the game as a safety #");
            builder.AppendLine("# Unlocks: This is a list of other ItemIDs that will be unlocked when you analyze or craft this recipe #");
            builder.AppendLine("#     - You can use this along with 'ForceUnlockAtStart:NO' to have recipes be unlocked when you want them to be #");
            builder.AppendLine("# Ingredients: This key is now optional #");
            builder.AppendLine("#     - You can leave this out of your modified recipes. If you do, the original recipe won't be altered in any way #");
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

        private static void GenerateOriginalsFile(string key, List<TechType> list, string fileName)
        {
            var printyPrints = new List<string>();
            printyPrints.AddRange(EmUtils.CommentTextLinesCentered(new string[]
            {
                "This file was generated with original recipes in the game",
                "You can copy individual entries from this file to use in your personal overrides",
                "--------------------------------------------------------------------------------",
            }));

            var originals = new ModifiedRecipeList();

            foreach (TechType craftable in list)
                originals.Add(new ModifiedRecipe(craftable));

            printyPrints.Add(originals.PrettyPrint());

            File.WriteAllLines(OriginalsFolder + fileName, printyPrints.ToArray());

            Logger.Log($"{fileName} file not found. File generated.");
        }

    }
}
