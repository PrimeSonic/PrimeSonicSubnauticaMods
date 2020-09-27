namespace CustomCraft2SML
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Common;
    using CustomCraft2SML.Interfaces.InternalUse;
    using CustomCraft2SML.Serialization.Entries;
    using CustomCraft2SML.Serialization.Lists;
    using EasyMarkup;

    internal static partial class HelpFilesWriter
    {
        private const string HorizontalLine = " -------------------------------------------- ";
        private static readonly string ReadMeVersionLine = $"How to use {FileLocations.RootModName} (Version {QuickLogger.GetAssemblyVersion()})";

        internal static void HandleHelpFiles()
        {
            HandleReadMeFile();
            GenerateOriginalRecipes();
        }

        private static void GenerateOriginalRecipes()
        {
            if (!Directory.Exists(FileLocations.SamplesFolder))
                Directory.CreateDirectory(FileLocations.SamplesFolder);

            if (!Directory.Exists(FileLocations.OriginalsFolder))
                Directory.CreateDirectory(FileLocations.OriginalsFolder);

            Dictionary<TechGroup, Dictionary<TechCategory, List<TechType>>> allGroups = ValidTechTypes.groups;

            foreach (TechGroup group in allGroups.Keys)
            {
                Dictionary<TechCategory, List<TechType>> groupCategories = allGroups[group];

                foreach (TechCategory category in groupCategories.Keys)
                {
                    string buildablesFile = $"{group}_{category}.txt";

                    if (File.Exists(FileLocations.OriginalsFolder + buildablesFile))
                        continue;

                    List<TechType> buildablesList = groupCategories[category];

                    GenerateOriginalsFile(group, category, buildablesList, buildablesFile);
                }
            }

            GenerateOriginalBioFuels();

            GenerateOriginalCraftingPaths();

            GenerateValidFoodModels();
        }

        private static void GenerateOriginalBioFuels()
        {
            const string fileName = "BioReactor_Values.txt";
            string path = Path.Combine(FileLocations.OriginalsFolder, fileName);

            if (File.Exists(path))
                return;

            Dictionary<TechType, float> allBioFuels = ValidBioFuels.charge;

            var bioFuelList = new CustomBioFuelList();
            foreach (TechType bioEnergyKey in allBioFuels.Keys)
                bioFuelList.Add(new CustomBioFuel { ItemID = bioEnergyKey.ToString(), Energy = allBioFuels[bioEnergyKey] });

            var printyPrints = new List<string>();
            printyPrints.AddRange(EmUtils.CommentTextLinesCentered(new string[]
            {
                "This file was generated with original BioFuel energy values in the game",
                "You can copy individual entries from this file to use in your personal overrides",
                "--------------------------------------------------------------------------------",
            }));

            printyPrints.Add(bioFuelList.PrettyPrint());

            File.WriteAllLines(path, printyPrints.ToArray(), Encoding.UTF8);

            QuickLogger.Debug($"{fileName} file not found. File generated.");
        }

        private static void GenerateOriginalCraftingPaths()
        {
            const string fileName = "CraftingPaths.txt";
            string path = Path.Combine(FileLocations.OriginalsFolder, fileName);

            if (File.Exists(path))
                return;

            string[] originalPaths =
            {
                "# Mobile Vehicle Bay #",
                "Constructor",
                "Constructor/Vehicles",
                "Constructor/Rocket",
                "",
                "# Cyclops Fabricator #",
                "CyclopsFabricator",
                "",
                "# Fabricator #",
                "Fabricator",
                "Fabricator/Resources",
                "Fabricator/Resources/BasicMaterials",
                "Fabricator/Resources/AdvancedMaterials",
                "Fabricator/Resources/Electronics",
                "Fabricator/Survival",
                "Fabricator/Survival/Water",
                "Fabricator/Survival/CookedFood",
                "Fabricator/Survival/CuredFood",
                "Fabricator/Personal",
                "Fabricator/Personal/Equipment",
                "Fabricator/Personal/Tools",
                "Fabricator/Machines",
                "",
                "# Scanner Room #",
                "MapRoom",
                "",
                "# Vehicle Upgrade Console #",
                "SeamothUpgrades",
                "SeamothUpgrades/CommonModules",
                "SeamothUpgrades/SeamothModules",
                "SeamothUpgrades/ExosuitModules",
                "SeamothUpgrades/CommonModules",
                "",
                "# Modification Station #",
                "Workbench",
                "Workbench/KnifeMenu",
                "Workbench/TankMenu",
                "Workbench/FinsMenu",
                "Workbench/PropulsionCannonMenu",
                "Workbench/CyclopsMenu",
                "Workbench/SeamothMenu",
                "Workbench/ExosuitMenu"
            };

            File.WriteAllLines(path, originalPaths, Encoding.UTF8);

            QuickLogger.Debug($"{fileName} file not found. File generated.");
        }

        private static void GenerateValidFoodModels()
        {
            const string fileName = "FoodModels.txt";
            string path = Path.Combine(FileLocations.OriginalsFolder, fileName);

            if (File.Exists(path))
                return;

            var models = (FoodModel[])Enum.GetValues(typeof(FoodModel));

            string[] strings = new string[models.Length];

            for (int i = 0; i < models.Length; i++)
            {
                strings[i] = models[i].ToString();
            }

            File.WriteAllLines(path, strings, Encoding.UTF8);

            QuickLogger.Debug($"{fileName} file not found. File generated.");
        }

        private static void HandleReadMeFile()
        {
            if (!File.Exists(FileLocations.HowToFile))
            {
                File.WriteAllLines(FileLocations.HowToFile, ReadMeFileLines(), Encoding.UTF8);
                QuickLogger.Debug($"{FileLocations.HowToFile} file not found. File created.");
            }
            else
            {
                string[] readmeLines = File.ReadAllLines(FileLocations.HowToFile, Encoding.UTF8);

                if (readmeLines.Length < 1 || readmeLines[0] != ReadMeVersionLine)
                {
                    File.WriteAllLines(FileLocations.HowToFile, ReadMeFileLines(), Encoding.UTF8);
                    QuickLogger.Info($"{FileLocations.HowToFile} out of date. Regenerated with new version.");
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
                $"{FileLocations.ModFriendlyName} uses simple text files to send requests to SMLHelper, no coding required",
                "This can be great for those who have a specific ideas in mind for custom crafts but don't have the means to code",
                $"{FileLocations.ModFriendlyName} is dedicated to Nexus modder Iw23J, creator of the original Custom Craft mod, ",
                $"who showed us how much we can empower players to take modding into their own hands",
                HorizontalLine,
                Environment.NewLine
            };

            tutorialLines.Add(HorizontalLine);
            AddGettingStarted(tutorialLines);
            tutorialLines.Add(Environment.NewLine);

            tutorialLines.Add(HorizontalLine);
            AddWorkingFileTypes(tutorialLines);
            tutorialLines.Add(Environment.NewLine);

            tutorialLines.Add(HorizontalLine);
            AddChangeLog(tutorialLines);
            tutorialLines.Add(Environment.NewLine);

            return tutorialLines.ToArray();
        }

        private static void AddWorkingFileTypes(List<string> tutorialLines)
        {
            tutorialLines.Add(HorizontalLine);
            tutorialLines.Add("--- Working File Types ---");
            tutorialLines.Add(Environment.NewLine);

            foreach (IParsingPackage package in WorkingFileParser.OrderedPackages)
            {
                tutorialLines.Add(HorizontalLine);
                tutorialLines.AddRange(package.TutorialText);
                tutorialLines.Add(Environment.NewLine);
            }
        }

        private static void AddGettingStarted(List<string> tutorialLines)
        {
            tutorialLines.Add(HorizontalLine);
            tutorialLines.Add("--- Getting Starts ---");
            tutorialLines.Add(Environment.NewLine);

            tutorialLines.Add(HorizontalLine);
            tutorialLines.Add($"Creating the text files that {FileLocations.RootModName} uses can be simple if you're paying attention.");
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
            tutorialLines.Add("Good luck and happy modding!");
            tutorialLines.Add(Environment.NewLine);
        }

        private static void AddChangeLog(List<string> tutorialLines)
        {
            tutorialLines.Add(HorizontalLine);
            tutorialLines.Add("--- Change Log ---");
            tutorialLines.Add(Environment.NewLine);

            tutorialLines.Add(HorizontalLine);
            tutorialLines.Add(Environment.NewLine);
            tutorialLines.Add("Version 1.9 makes changes to CustomFoods");
            tutorialLines.Add("AllowOverfill has been removed from CustomFoods as it is no longer enabled by the game");
            tutorialLines.Add("Added UseDrinkSound as a YES/NO entry so that the drinking sound effect is used over the eat sound effect");
            tutorialLines.Add("Corrections made to the documentation of Custom tabs and BioFuel");

            tutorialLines.Add(HorizontalLine);
            tutorialLines.Add("Version 1.8 adds the long awaited custom foods");
            tutorialLines.Add("You can now create foods with custom food and water values and change the speed of decomposition for your foods.");
            tutorialLines.Add("Additionally you can change the model your item uses to already existing foods, for example a Cooked Peeper, otherwise it will auto-select using the food and water values.");
            tutorialLines.Add("CustomFoods are available in the CustomFabricators too.");
            tutorialLines.Add(Environment.NewLine);

            tutorialLines.Add(HorizontalLine);
            tutorialLines.Add("Version 1.7 adds some very big additions to really help make your crafting.");
            tutorialLines.Add("You can now make your own Custom Fabricators and set up your own crafting tree in them from scratch.");
            tutorialLines.Add("MovedRecipes can now copy an existing crafting node into another fabricator, even a custom one, without removing the original.");
            tutorialLines.Add("Everything that derives from ModifiedRecipes (which includes AddedRecipes, AliasRecipes, and now CustomFabricators) now has the 'UnlockedBy' property.");
            tutorialLines.Add("UnlockedBy lets you specify a list of TechTypes that will be updated to unlock your main item whenever you discover any of those.");
            tutorialLines.Add(Environment.NewLine);

            tutorialLines.Add(HorizontalLine);
            tutorialLines.Add("As of version 1.6, introduces powerful tools that let you further create your own crafting experience.");
            tutorialLines.Add("AliasRecipes receive the new FunctionalityI, letting these items use in-game model while crafting and even mimic their in-game functions.");
            tutorialLines.Add("MovedRecipes now let you move things around the crafting tree, or start removing parts of it entirely.");
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

            File.WriteAllLines(Path.Combine(FileLocations.OriginalsFolder, fileName), printyPrints.ToArray(), Encoding.UTF8);

            QuickLogger.Debug($"{fileName} file not found. File generated.");
        }
    }
}
