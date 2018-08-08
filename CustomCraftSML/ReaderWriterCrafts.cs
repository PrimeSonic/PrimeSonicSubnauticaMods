namespace CustomCraft2SML
{
    using System.Collections.Generic;
    using System.IO;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization;
    using UnityEngine.Assertions;

    internal static partial class FileReaderWriter
    {
        private const string WorkingFolder = FolderRoot + "WorkingFiles/";
        private const string CustomSizesFile = WorkingFolder + "CustomSizes.txt";
        private const string ModifiedRecipesFile = WorkingFolder + "ModifiedRecipes.txt";
        private const string AddedRecipiesFile = WorkingFolder + "AddedRecipes.txt";
        private const string CustomBioFuelsFile = WorkingFolder + "CustomBioFuels.txt";

        private static readonly IDictionary<TechType, AddedRecipe> addedRecipes = new Dictionary<TechType, AddedRecipe>();
        private static readonly IDictionary<TechType, ModifiedRecipe> modifiedRecipes = new Dictionary<TechType, ModifiedRecipe>();
        private static readonly IDictionary<TechType, CustomSize> customSizes = new Dictionary<TechType, CustomSize>();
        private static readonly IDictionary<TechType, CustomBioFuel> customBioFuels = new Dictionary<TechType, CustomBioFuel>();

        private static void HandleWorkingFiles()
        {
            ICollection<string> workingFiles = new List<string>(Directory.GetFiles(WorkingFolder));

            foreach (string file in workingFiles)
                DeserializeFile(file);

            if (addedRecipes.Count == 0 && !workingFiles.Contains(AddedRecipiesFile))
                CreateEmptyFile<AddedRecipeList>(AddedRecipiesFile);

            if (modifiedRecipes.Count == 0 && !workingFiles.Contains(ModifiedRecipesFile))
                CreateEmptyFile<ModifiedRecipeList>(ModifiedRecipesFile);

            if (customSizes.Count == 0 && !workingFiles.Contains(CustomSizesFile))
                CreateEmptyFile<CustomSizeList>(CustomSizesFile);

            if (customBioFuels.Count == 0 && !workingFiles.Contains(CustomBioFuelsFile))
                CreateEmptyFile<CustomBioFuelList>(CustomBioFuelsFile);

            SendToSMLHelper(addedRecipes);
            SendToSMLHelper(modifiedRecipes);
            SendToSMLHelper(customSizes);
            SendToSMLHelper(customBioFuels);
        }

        private static void CreateEmptyFile<T>(string filePath) where T : EmProperty, ITutorialText, new()
        {
            T emptyList = new T();

            List<string> tutorialText = emptyList.TutorialText;

            tutorialText.Add(emptyList.PrettyPrint());

            File.WriteAllLines(filePath, tutorialText.ToArray());
        }

        private static void DeserializeFile(string workingFilePath)
        {
            QuickLogger.Message($"Reading file: {workingFilePath}");

            string serializedData = File.ReadAllText(workingFilePath);

            if (string.IsNullOrEmpty(serializedData))
            {
                QuickLogger.Warning($"File contained no text");
                return;
            }

            if (EmProperty.CheckKey(serializedData, out string key))
            {
                int check = -2;
                switch (key)
                {
                    case "AddedRecipes":
                        check = ParseEntries<AddedRecipe, AddedRecipeList>(serializedData, addedRecipes);
                        break;

                    case "ModifiedRecipes":
                        check = ParseEntries<ModifiedRecipe, ModifiedRecipeList>(serializedData, modifiedRecipes);
                        break;

                    case "CustomSizes":
                        check = ParseEntries<CustomSize, CustomSizeList>(serializedData, customSizes);
                        break;

                    case "CustomBioFuels":
                        check = ParseEntries<CustomBioFuel, CustomBioFuelList>(serializedData, customBioFuels);
                        break;

                    default:
                        QuickLogger.Error($"Invalid primary key '{key}' detected in file");
                        return;
                }

                switch (check)
                {
                    case -1:
                        QuickLogger.Error($"Unable to parse file");
                        break;
                    case 0:
                        QuickLogger.Message($"File was parsed but no entries were found");
                        break;
                    default:
                        QuickLogger.Message($"{check} entries parsed from file");
                        break;
                }
            }
            else
            {
                QuickLogger.Warning("Could not identify primary key in file");
            }
        }

        private static int ParseEntries<T, T2>(string serializedData, IDictionary<TechType, T> parsedItems)
            where T : EmPropertyCollection, ITechTyped
            where T2 : EmPropertyCollectionList<T>, new()
        {
            T2 list = new T2();

            Assert.AreEqual(typeof(T), list.ItemType);

            bool successfullyParsed = list.Deserialize(serializedData);

            if (!successfullyParsed)
                return -1; // Error case

            if (list.Count == 0)
                return 0; // No entries

            int unique = 0;
            foreach (T item in list)
            {
                if (parsedItems.ContainsKey(item.ItemID))
                {
                    QuickLogger.Warning($"Duplicate entry for '{item.ItemID}' in '{list.Key}' was already added by another working file. Kept first one. Discarded duplicate.");
                }
                else
                {
                    parsedItems.Add(item.ItemID, item);
                    unique++;
                }
            }

            return unique++; // Return the number of unique entries added in this list
        }

        private static void SendToSMLHelper<T>(IDictionary<TechType, T> uniqueEntries)
            where T : ITechTyped
        {
            int successCount = 0;
            foreach (T item in uniqueEntries.Values)
            {
                bool result = CustomCraft.AddEntry(item);

                if (result) successCount++;
            }

            Logger.Log($"{successCount} of {uniqueEntries.Count} {typeof(T).Name} entries were patched.");
        }
    }
}
