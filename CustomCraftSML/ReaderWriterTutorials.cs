namespace CustomCraft2SML
{
    using System;
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
        internal const string ModFriendlyName = "Custom Craft 2";
        internal const string FolderRoot = "./QMods/" + RootModName + "/";
        internal const string SamplesFolder = FolderRoot + "SampleFiles/";
        internal const string OriginalsFolder = FolderRoot + "OriginalRecipes/";
        internal const string HowToFile = FolderRoot + "README_HowToUseThisMod.txt";

        private const string HorizontalLine = " -------------------------------------------- ";
        private static readonly string ReadMeVersionLine = $"# How to use {RootModName} (Revision {QuickLogger.GetAssemblyVersion()}) #";

        private static void GenerateOriginalRecipes()
        {
            if (!Directory.Exists(SamplesFolder))
                Directory.CreateDirectory(SamplesFolder);

            if (!Directory.Exists(OriginalsFolder))
                Directory.CreateDirectory(OriginalsFolder);

            Dictionary<TechGroup, Dictionary<TechCategory, List<TechType>>> allGroups = ValidTechTypes.groups;

            foreach (TechGroup group in allGroups.Keys)
            {
                Dictionary<TechCategory, List<TechType>> groupCategories = allGroups[group];

                foreach (TechCategory category in groupCategories.Keys)
                {
                    string buildablesFile = $"{group}_{category}.txt";

                    if (File.Exists(OriginalsFolder + buildablesFile))
                        continue;

                    List<TechType> buildablesList = groupCategories[category];

                    GenerateOriginalsFile(group, category, buildablesList, buildablesFile);
                }
            }

            GenerateOriginalBioFuels();
        }

        private static void GenerateOriginalBioFuels()
        {
            const string fileName = "BioReactor_Values.txt";

            if (File.Exists(OriginalsFolder + fileName))
                return;

            Dictionary<TechType, float> allBioFuels = ValidBioFuels.charge;

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
                File.WriteAllLines(HowToFile, ReadMeFileLines());
                Logger.Log($"{HowToFile} file not found. File created.");
            }
            else
            {
                string[] readmeLines = File.ReadAllLines(HowToFile);

                if (readmeLines.Length < 1 || readmeLines[0] != ReadMeVersionLine)
                {
                    File.WriteAllLines(HowToFile, ReadMeFileLines());
                    Logger.Log($"{HowToFile} out of date. Regenerated with new version.");
                }
            }
        }

        private static string[] ReadMeFileLines()
        {
            var tutorialLines = new List<string>
            {
                ReadMeVersionLine,
                HorizontalLine,
                Environment.NewLine,
                $"{ModFriendlyName} uses simple text files to send requests to SMLHelper, no coding required",
                "This can be great for those who have a specific ideas in mind for custom crafts but don't have the means to code",
                $"{ModFriendlyName} is dedicated to Nexus modder Iw23J, creator of the original Custom Craft mod, who showed us how much we can empower players to take modding into their own hands",
                HorizontalLine,
                $"As of version {QuickLogger.GetAssemblyVersion()}, the following features are supported:",
            };
            tutorialLines.Add(MovedRecipe.TutorialText);
            tutorialLines.Add(Environment.NewLine);
            tutorialLines.Add(CustomSize.TutorialText);
            tutorialLines.Add(Environment.NewLine);
            tutorialLines.AddRange(CustomBioFuel.TutorialText);
            tutorialLines.Add(Environment.NewLine);
            tutorialLines.AddRange(CustomCraftingTab.TutorialText);
            tutorialLines.Add(Environment.NewLine);
            tutorialLines.AddRange(ModifiedRecipe.TutorialText);
            tutorialLines.Add(Environment.NewLine);
            tutorialLines.AddRange(AddedRecipe.TutorialText);
            tutorialLines.Add(Environment.NewLine);
            tutorialLines.AddRange(AliasRecipe.TutorialText);
            tutorialLines.Add(Environment.NewLine);
            tutorialLines.Add(HorizontalLine);
            tutorialLines.Add("As of version 1.5, you can include modded items in your custom crafts.");
            tutorialLines.Add("Most modded items should work, but we can only guarantee compatibility with items created using SMLHelper.");
            tutorialLines.Add("To get the ItemID for modded items, consult with the original mod author.");
            tutorialLines.Add(Environment.NewLine);
            tutorialLines.Add(HorizontalLine);
            tutorialLines.Add("Additional features may be added in the future so keep an eye on the Nexus mod page.");
            tutorialLines.Add("After an update, this file will be updated with new info the next time you load the game.");
            tutorialLines.Add(Environment.NewLine);
            tutorialLines.Add(HorizontalLine);
            tutorialLines.Add("As of version 1.2, file names no longer matter. All files in the WorkingFiles folder will be read and parsed into the game. So name them however you want.");
            tutorialLines.Add("If you want to be able to easily sahre your custom crafts with others, make sure to chose unique names for your files.");
            tutorialLines.Add("Remember: For now, each file can only contain one type of entry. The valid entry types are: MovedRecipes, CustomSizes, CustomBioFuels, CustomCraftingTabs, ModifiedRecipes, AddedRecipes, AliasRecipe.");
            tutorialLines.Add(Environment.NewLine);
            tutorialLines.Add(HorizontalLine);
            tutorialLines.Add($"Creating the text files that {RootModName} uses can be simple if you're paying attention.");
            tutorialLines.Add("As easy way to get started is to copy the text from the SampleFiles or OriginalRecipes and then modify the parts you want.");
            tutorialLines.Add("When you want to add a new item reference, remember that the 'ItemID' is the internal spawn ID of that item.");
            tutorialLines.Add("    You can visit the Subnautica Wikia for the full list of item IDs at http://subnautica.wikia.com/wiki/Obtainable_Items");
            tutorialLines.Add("The files in OriginalRecipes are generated automatically when the game first loads if the mod detected that they are missing.");
            tutorialLines.Add("    If you can see this file, then the OriginalRecipes will be there.");
            tutorialLines.Add(Environment.NewLine);
            tutorialLines.Add(HorizontalLine);
            tutorialLines.Add("You'll notice that a lot of text is written between these hash signs (or pound sign or hashtag if you prefer).");
            tutorialLines.Add("Any text in between these symbols is treated as a comment and safely ignored by the mod's text reader.");
            tutorialLines.Add("Use these hash signs if you want to leave notes or comments in the txt files.");
            tutorialLines.Add("Extra whitespace that isn't between quotes (\") is also ignored, so feel free to make your files as flat or as indented as you prefer.");
            tutorialLines.Add("It's just recommended that you make them as readable as possible.");
            tutorialLines.Add(Environment.NewLine);
            tutorialLines.Add(HorizontalLine);
            tutorialLines.Add("Once you've created your txt files, go ahead and launch the game.");
            tutorialLines.Add("Assuming you've got everything configured correctly, your customizations will appear on your next game.");
            tutorialLines.Add(Environment.NewLine);
            tutorialLines.Add(HorizontalLine);
            tutorialLines.Add("Remember: When adding new recipes, you must use the correct path for the crafting tab that the new recipe will be added to.");
            tutorialLines.Add("Provided here is a list of all the valid paths to all the standard crafting tabs for all available fabricators.");
            tutorialLines.Add("And don't forget that you can always add your own crafting tabs too.");
            tutorialLines.Add(Environment.NewLine);
            tutorialLines.Add(PathHelper.GeneratePaths());
            tutorialLines.Add(Environment.NewLine);
            tutorialLines.Add(HorizontalLine);
            tutorialLines.Add("Enjoy and happy modding");

            return EmUtils.CommentTextLinesCentered(tutorialLines.ToArray());
        }

        private static void GenerateOriginalsFile(TechGroup group, TechCategory category, List<TechType> list, string fileName)
        {
            var printyPrints = new List<string>();
            printyPrints.AddRange(EmUtils.CommentTextLinesCentered(new string[]
            {
                "This file was generated with original recipes in the game",
                "You can copy individual entries from this file to use in your personal overrides",
                "--------------------------------------------------------------------------------",
                $"PdaGroup: {group} - PdaCategory:{category}",
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
