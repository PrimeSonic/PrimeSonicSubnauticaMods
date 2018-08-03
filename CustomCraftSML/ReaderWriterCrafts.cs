namespace CustomCraft2SML
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization;

    internal static partial class FileReaderWriter
    {
        private const string WorkingFolder = FolderRoot + "WorkingFiles/";
        private const string CustomSizesFile = WorkingFolder + "CustomSizes.txt";
        private const string ModifiedRecipesFile = WorkingFolder + "ModifiedRecipes.txt";
        private const string AddedRecipiesFile = WorkingFolder + "AddedRecipes.txt";

        private const string OldCustomSizesFile = FolderRoot + "CustomSizes.txt";
        private const string OldModifiedRecipesFile = FolderRoot + "ModifiedRecipes.txt";
        private const string OldAddedRecipiesFile = FolderRoot + "AddedRecipes.txt";

        private static readonly IDictionary<TechType, AddedRecipe> addedRecipes = new Dictionary<TechType, AddedRecipe>();
        private static readonly IDictionary<TechType, ModifiedRecipe> modifiedRecipes = new Dictionary<TechType, ModifiedRecipe>();
        private static readonly IDictionary<TechType, CustomSize> customSizes = new Dictionary<TechType, CustomSize>();

        private static void HandleWorkingFiles()
        {
            // Old files and file location
            MoveToWorkingFiles(OldAddedRecipiesFile, AddedRecipiesFile);
            MoveToWorkingFiles(OldModifiedRecipesFile, ModifiedRecipesFile);
            MoveToWorkingFiles(OldCustomSizesFile, CustomSizesFile);

            string[] workingFiles = Directory.GetFiles(WorkingFolder);

            if (workingFiles.Length == 0)
            {
                CreateEmptyDefaultFiles();
                return;
            }

            DeserializedFiles(workingFiles);

            SendToSMLHelper();
        }

        private static void CreateEmptyDefaultFiles()
        {
            File.WriteAllText(AddedRecipiesFile, $"# Added Recipes #{Environment.NewLine}" +
                $"# Check the AddedRecipes_Samples.txt file in the SampleFiles folder for details on how to add recipes for items normally not craftable #{Environment.NewLine}");
            File.WriteAllText(ModifiedRecipesFile, $"# Modified Recipes #{Environment.NewLine}" +
                $"# Check the ModifiedRecipes_Samples.txt file in the SampleFiles folder for details on how to alter existing crafting recipes #{Environment.NewLine}");
            File.WriteAllText(CustomSizesFile, $"# Custom Sizes go in this file #{Environment.NewLine}" +
                $"# Check the CustomSizes_Samples.txt file in the SampleFiles folder for details on how to set your own custom sizes #{Environment.NewLine}");

            Logger.Log($"No files found in working folder. Empty starter files created.");
        }

        private static void DeserializedFiles(string[] workingFiles)
        {
            var keyChecker = new EmFileKeyChecker();

            foreach (string fileName in workingFiles)
            {
                QuickLogger.Message($"Reading file: {fileName}", false);

                string serializedData = File.ReadAllText(fileName);

                if (string.IsNullOrEmpty(serializedData))
                {
                    QuickLogger.Warning($"File contained no text", false);
                    continue;
                }

                if (keyChecker.CheckKey(serializedData, out string key))
                {
                    int check = -2;
                    switch (key)
                    {
                        case "AddedRecipes":
                            check = ParseAddedRecipes(serializedData);
                            break;

                        case "ModifiedRecipes":
                            check = ParseModifiedRecipes(serializedData);
                            break;

                        case "CustomSizes":
                            check = ParseCustomSizes(serializedData);
                            break;

                        default:
                            QuickLogger.Error($"Invalid primary key '{key}' detected in file", false);
                            continue;
                    }

                    switch (check)
                    {
                        case -1:
                            QuickLogger.Error($"Unable to parse file", false);
                            break;
                        case 0:
                            QuickLogger.Warning($"File was parsed but no entries were found", false);
                            break;
                        default:
                            QuickLogger.Message($"{check} entries parsed from file", false);
                            break;
                    }
                }
                else
                {
                    QuickLogger.Warning("Could not identify primary key in file");
                }
            }
        }

        private static int ParseAddedRecipes(string serializedData)
        {
            var addedRecipeList = new AddedRecipeList();

            bool successfullyParsed = addedRecipeList.Deserialize(serializedData);

            if (!successfullyParsed)
                return -1;

            if (addedRecipeList.Count == 0)
                return 0;

            int unique = 0;
            foreach (AddedRecipe recipe in addedRecipeList)
            {
                if (addedRecipes.ContainsKey(recipe.ItemID))
                {
                    QuickLogger.Warning($"Added recipe for '{recipe.ItemID}' was already added by another working file. First found kept.", false);
                }
                else
                {
                    addedRecipes.Add(recipe.ItemID, recipe);
                    unique++;
                }
            }

            return unique++;
        }

        private static int ParseModifiedRecipes(string serializedData)
        {
            var modifiedRecipeList = new ModifiedRecipeList();

            bool successfullyParsed = modifiedRecipeList.Deserialize(serializedData);

            if (!successfullyParsed)
                return -1;

            if (modifiedRecipeList.Count == 0)
                return 0;

            int unique = 0;
            foreach (ModifiedRecipe recipe in modifiedRecipeList)
            {
                if (modifiedRecipes.ContainsKey(recipe.ItemID))
                {
                    QuickLogger.Warning($"Modified recipe for '{recipe.ItemID}' was already added by another working file. First found kept.", false);
                }
                else
                {
                    modifiedRecipes.Add(recipe.ItemID, recipe);
                    unique++;
                }
            }

            return unique++;
        }

        private static int ParseCustomSizes(string serializedData)
        {
            var customSizesList = new CustomSizeList();

            bool successfullyParsed = customSizesList.Deserialize(serializedData);

            if (!successfullyParsed)
                return -1;

            if (customSizesList.Count == 0)
                return 0;

            int unique = 0;
            foreach (CustomSize size in customSizesList)
            {
                if (customSizes.ContainsKey(size.ItemID))
                {
                    QuickLogger.Warning($"Custom Size for '{size.ItemID}' was already added by another working file. First found kept.", false);
                }
                else
                {
                    customSizes.Add(size.ItemID, size);
                    unique++;
                }
            }

            return unique;
        }

        private static void MoveToWorkingFiles(string oldFile, string newFile)
        {
            if (!Directory.Exists(WorkingFolder))
                Directory.CreateDirectory(WorkingFolder);

            if (File.Exists(oldFile))
                File.Move(oldFile, newFile);
        }

        private static void SendToSMLHelper()
        {
            foreach (IAddedRecipe item in addedRecipes.Values)
                CustomCraft.AddRecipe(item);

            Logger.Log($"{addedRecipes.Count} Added Recipies patched.");

            foreach (IModifiedRecipe item in modifiedRecipes.Values)
                CustomCraft.ModifyRecipe(item);

            Logger.Log($"{modifiedRecipes.Count} Modified Recipies patched.");

            foreach (ICustomSize customSize in customSizes.Values)
                CustomCraft.CustomizeItemSize(customSize);

            Logger.Log($"{customSizes.Count} Custom Sizes patched.");
        }
    }
}
