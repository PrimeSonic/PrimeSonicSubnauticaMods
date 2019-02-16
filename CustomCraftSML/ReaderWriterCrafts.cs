namespace CustomCraft2SML
{
    using System.Collections.Generic;
    using System.IO;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Serialization.Entries;
    using CustomCraft2SML.Serialization.Lists;

    internal static partial class FileReaderWriter
    {
        internal const string WorkingFolder = FolderRoot + "WorkingFiles/";
        internal const string AssetsFolder = FolderRoot + "Assets/";

        //  Initial storage for the serialization - key is string as we have not resolved the TechType at this point
        private static readonly IList<MovedRecipe> movedRecipes = new List<MovedRecipe>();
        private static readonly IList<AddedRecipe> addedRecipes = new List<AddedRecipe>();
        private static readonly IList<AliasRecipe> aliasRecipes = new List<AliasRecipe>();
        private static readonly IList<ModifiedRecipe> modifiedRecipes = new List<ModifiedRecipe>();
        private static readonly IList<CustomSize> customSizes = new List<CustomSize>();
        private static readonly IList<CustomBioFuel> customBioFuels = new List<CustomBioFuel>();
        private static readonly IList<CustomFragmentCount> customFragments = new List<CustomFragmentCount>();
        private static readonly IList<CustomCraftingTab> customTabs = new List<CustomCraftingTab>();

        private static readonly IDictionary<string, CustomCraftingTab> uniqueCustomTabs = new Dictionary<string, CustomCraftingTab>();
        private static readonly IDictionary<string, MovedRecipe> uniqueMovedRecipes = new Dictionary<string, MovedRecipe>();
        private static readonly IDictionary<string, AddedRecipe> uniqueAddedRecipes = new Dictionary<string, AddedRecipe>();
        private static readonly IDictionary<string, AliasRecipe> uniqueAliasRecipes = new Dictionary<string, AliasRecipe>();
        private static readonly IDictionary<string, ModifiedRecipe> uniqueModifiedRecipes = new Dictionary<string, ModifiedRecipe>();
        private static readonly IDictionary<string, CustomSize> uniqueCustomSizes = new Dictionary<string, CustomSize>();
        private static readonly IDictionary<string, CustomBioFuel> uniqueCustomBioFuels = new Dictionary<string, CustomBioFuel>();
        private static readonly IDictionary<string, CustomFragmentCount> uniqueCustomFragments = new Dictionary<string, CustomFragmentCount>();

        private static void HandleWorkingFiles()
        {
            if (!Directory.Exists(AssetsFolder))
                Directory.CreateDirectory(AssetsFolder);

            QuickLogger.Message("Reading contents of WorkingFiles folder");

            ICollection<string> workingFiles = new List<string>(Directory.GetFiles(WorkingFolder));

            QuickLogger.Message($"{workingFiles.Count} files found");

            foreach (string file in workingFiles)
                DeserializeFile(file);

            PrePassValidation(customTabs, uniqueCustomTabs);
            PrePassValidation(movedRecipes, uniqueMovedRecipes);
            PrePassValidation(addedRecipes, uniqueAddedRecipes);
            PrePassValidation(aliasRecipes, uniqueAliasRecipes);
            PrePassValidation(modifiedRecipes, uniqueModifiedRecipes);
            PrePassValidation(customSizes, uniqueCustomSizes);
            PrePassValidation(customBioFuels, uniqueCustomBioFuels);
            PrePassValidation(customFragments, uniqueCustomFragments);

            SendToSMLHelper(uniqueCustomTabs.Values);
            SendToSMLHelper(uniqueMovedRecipes.Values);
            SendToSMLHelper(uniqueAddedRecipes.Values);
            SendToSMLHelper(uniqueAliasRecipes.Values);
            SendToSMLHelper(uniqueModifiedRecipes.Values);
            SendToSMLHelper(uniqueCustomSizes.Values);
            SendToSMLHelper(uniqueCustomBioFuels.Values);
            SendToSMLHelper(uniqueCustomFragments.Values);
        }

        private static void DeserializeFile(string workingFilePath)
        {
            string fileName = Path.GetFileName(workingFilePath);

            string serializedData = File.ReadAllText(workingFilePath);

            if (string.IsNullOrEmpty(serializedData))
            {
                QuickLogger.Warning($"File '{fileName}' contained no text");
                return;
            }

            if (EmProperty.CheckKey(serializedData, out string key))
            {
                int check = -2;
                switch (key)
                {
                    case AddedRecipeList.ListKey:
                        check = ParseEntries<AddedRecipe, AddedRecipeList>(serializedData, addedRecipes);
                        break;

                    case AliasRecipeList.ListKey:
                        check = ParseEntries<AliasRecipe, AliasRecipeList>(serializedData, aliasRecipes);
                        break;

                    case ModifiedRecipeList.ListKey:
                        check = ParseEntries<ModifiedRecipe, ModifiedRecipeList>(serializedData, modifiedRecipes);
                        break;

                    case CustomSizeList.ListKey:
                        check = ParseEntries<CustomSize, CustomSizeList>(serializedData, customSizes);
                        break;

                    case CustomBioFuelList.ListKey:
                        check = ParseEntries<CustomBioFuel, CustomBioFuelList>(serializedData, customBioFuels);
                        break;

                    case CustomCraftingTabList.ListKey:
                        check = ParseEntries<CustomCraftingTab, CustomCraftingTabList>(serializedData, customTabs);
                        break;

                    case MovedRecipeList.ListKey:
                        check = ParseEntries<MovedRecipe, MovedRecipeList>(serializedData, movedRecipes);
                        break;

                    case CustomFragmentCountList.ListKey:
                        check = ParseEntries<CustomFragmentCount, CustomFragmentCountList>(serializedData, customFragments);
                        break;

                    default:
                        QuickLogger.Error($"Unknown primary key '{key}' detected in file '{fileName}'");
                        return;
                }

                switch (check)
                {
                    case -1:
                        QuickLogger.Error($"Unable to parse file '{fileName}'");
                        break;
                    case 0:
                        QuickLogger.Message($"File '{fileName}' was parsed but no entries were found");
                        break;
                    default:
                        QuickLogger.Message($"{check} entries parsed from file '{fileName}'");
                        break;
                }
            }
            else
            {
                QuickLogger.Warning($"Could not identify primary key in file '{fileName}'");
            }
        }

        private static int ParseEntries<T, T2>(string serializedData, ICollection<T> parsedItems)
            where T : EmPropertyCollection, ICustomCraft
            where T2 : EmPropertyCollectionList<T>, new()
        {
            var list = new T2();

            bool successfullyParsed = list.Deserialize(serializedData);

            if (!successfullyParsed)
                return -1; // Error case

            if (list.Count == 0)
                return 0; // No entries

            int count = 0;
            foreach (T item in list)
            {
                parsedItems.Add(item);
                count++;
            }

            return count; // Return the number of unique entries added in this list
        }

        private static void PrePassValidation<T>(ICollection<T> entries, IDictionary<string, T> uniqueEntries) where T : ICustomCraft
        {
            string typeName = typeof(T).Name;

            //  Use the ToSet function as a copy constructor - this way we can iterate across the
            //      temp structure, but change the permanent one in the case of duplicates
            foreach (T item in entries)
            {
                if (!item.PassesPreValidation())
                    continue;

                if (uniqueEntries.ContainsKey(item.ID))
                {
                    QuickLogger.Warning($"Duplicate entry for {typeName} '{item.ID}' was already added by another working file. Kept first one. Discarded duplicate.");
                    continue;
                }

                // All checks passed
                uniqueEntries.Add(item.ID, item);
            }

            if (entries.Count > 0)
            {
                QuickLogger.Message($"{uniqueEntries.Count} of {entries.Count} {typeName} entries staged for patching");
            }
        }

        private static void SendToSMLHelper<T>(ICollection<T> uniqueEntries) where T : ICustomCraft
        {
            int successCount = 0;
            foreach (T item in uniqueEntries)
            {
                if (item.SendToSMLHelper())
                    successCount++;
            }

            if (uniqueEntries.Count > 0)
                QuickLogger.Message($"{successCount} of {uniqueEntries.Count} {typeof(T).Name} entries were patched");
        }
    }
}
