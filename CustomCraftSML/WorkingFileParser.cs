namespace CustomCraft2SML
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces.InternalUse;
    using CustomCraft2SML.Serialization;
    using CustomCraft2SML.Serialization.Entries;
    using CustomCraft2SML.Serialization.Lists;

    internal static class WorkingFileParser
    {
        private static readonly ParsingPackage<CustomCraftingTab, CustomCraftingTabList> CustomTabs = new ParsingPackage<CustomCraftingTab, CustomCraftingTabList>(CustomCraftingTabList.ListKey);
        private static readonly ParsingPackage<MovedRecipe, MovedRecipeList> MovedRecipes = new ParsingPackage<MovedRecipe, MovedRecipeList>(MovedRecipeList.ListKey);
        private static readonly ParsingPackage<AddedRecipe, AddedRecipeList> AddedRecipes = new ParsingPackage<AddedRecipe, AddedRecipeList>(AddedRecipeList.ListKey);
        private static readonly ParsingPackage<AliasRecipe, AliasRecipeList> AliasRecipes = new ParsingPackage<AliasRecipe, AliasRecipeList>(AliasRecipeList.ListKey);
        private static readonly ParsingPackage<CustomFabricator, CustomFabricatorList> CustomFabricatorParser = new ParsingPackage<CustomFabricator, CustomFabricatorList>(CustomFabricatorList.ListKey);
        private static readonly ParsingPackage<ModifiedRecipe, ModifiedRecipeList> ModifiedRecipeParser = new ParsingPackage<ModifiedRecipe, ModifiedRecipeList>(ModifiedRecipeList.ListKey);
        private static readonly ParsingPackage<CustomSize, CustomSizeList> CustomSizeParser = new ParsingPackage<CustomSize, CustomSizeList>(CustomSizeList.ListKey);
        private static readonly ParsingPackage<CustomBioFuel, CustomBioFuelList> CustomBioFuelParser = new ParsingPackage<CustomBioFuel, CustomBioFuelList>(CustomBioFuelList.ListKey);
        private static readonly ParsingPackage<CustomFragmentCount, CustomFragmentCountList> CustomFragCountParser = new ParsingPackage<CustomFragmentCount, CustomFragmentCountList>(CustomFragmentCountList.ListKey);
        private static readonly ParsingPackage<CustomFood, CustomFoodList> CustomFoods = new ParsingPackage<CustomFood, CustomFoodList>(CustomFoodList.ListKey);

        internal static readonly IEnumerable<IParsingPackage> OrderedPackages = new List<IParsingPackage>(8)
        {
            CustomFabricatorParser,
            CustomTabs,            
            AddedRecipes,
            AliasRecipes,
            ModifiedRecipeParser,
            CustomFoods,
            MovedRecipes,
            CustomFragCountParser,
            CustomSizeParser,
            CustomBioFuelParser,
        };

        private static IDictionary<string, IParsingPackage> PackagesLookup = new Dictionary<string, IParsingPackage>(9);

        internal static void HandleWorkingFiles()
        {
            foreach (IParsingPackage package in OrderedPackages)
                PackagesLookup.Add(package.ListKey, package);

            if (!Directory.Exists(FileLocations.AssetsFolder))
                Directory.CreateDirectory(FileLocations.AssetsFolder);

            if (!Directory.Exists(FileLocations.WorkingFolder))
                Directory.CreateDirectory(FileLocations.WorkingFolder);

            string[] workingFiles = Directory.GetFiles(FileLocations.WorkingFolder);

            QuickLogger.Info($"{workingFiles.Length} files found in the WorkingFiles folder");

            int rollingCount = 0;
            foreach (string file in workingFiles)
                rollingCount+= DeserializeFile(file);

            QuickLogger.Info($"{rollingCount} total entries discovered across all files.");

            QuickLogger.Debug($"Validating entries - First Pass");
            foreach (IParsingPackage package in OrderedPackages)
                package.PrePassValidation();

            QuickLogger.Debug($"Validating entries - Second Pass");
            MasterUniquenessValidation();

            QuickLogger.Debug($"Sending requests to SMLHelper");
            foreach (IParsingPackage package in OrderedPackages)
                package.SendToSMLHelper();
        }

        private static int DeserializeFile(string workingFilePath)
        {
            string fileName = Path.GetFileName(workingFilePath);

            string serializedData = File.ReadAllText(workingFilePath, Encoding.UTF8);

            if (string.IsNullOrEmpty(serializedData))
            {
                QuickLogger.Warning($"File '{fileName}' contained no text");
                return 0;
            }

            if (EmProperty.CheckKey(serializedData, out string key))
            {
                int check = -2;
                if (PackagesLookup.TryGetValue(key, out IParsingPackage package))
                {
                    check = package.ParseEntries(serializedData, OriginFile.GetOriginFile(fileName));
                }
                else
                {
                    QuickLogger.Warning($"Unknown primary key '{key}' detected in file '{fileName}'");
                    return 0;
                }

                switch (check)
                {
                    case -2:
                        QuickLogger.Error($"Unexpected error when attempting to parse file '{fileName}'");
                        break;
                    case -1:
                        QuickLogger.Warning($"Unable to parse file '{fileName}'");
                        break;
                    case 0:
                        QuickLogger.Warning($"File '{fileName}' was parsed but no entries were found");
                        break;
                    default:
                        QuickLogger.Debug($"{check} entries parsed from file '{fileName}'");
                        return check;
                }
            }
            else
            {
                QuickLogger.Warning($"Could not identify primary key in file '{fileName}'");                
            }

            return 0;
        }

        private static void MasterUniquenessValidation()
        {
            var allTabs = new HashSet<string>(CustomTabs.UniqueEntries.Keys, StringComparer.InvariantCultureIgnoreCase);
            var allMoves = new HashSet<string>(MovedRecipes.UniqueEntries.Keys, StringComparer.InvariantCultureIgnoreCase);
            var allAdded = new HashSet<string>(AddedRecipes.UniqueEntries.Keys, StringComparer.InvariantCultureIgnoreCase);
            var allAlias = new HashSet<string>(AliasRecipes.UniqueEntries.Keys, StringComparer.InvariantCultureIgnoreCase);
            var allFoods = new HashSet<string>(CustomFoods.UniqueEntries.Keys, StringComparer.InvariantCultureIgnoreCase);

            foreach (IFabricatorEntries entries in CustomFabricatorParser.UniqueEntries.Values)
            {                
                ValidateSets(allTabs, entries.CustomTabIDs, entries.DuplicateCustomTabDiscovered);
                ValidateSets(allMoves, entries.MovedRecipeIDs, entries.DuplicateMovedRecipeDiscovered);
                ValidateSets(allAdded, entries.AddedRecipeIDs, entries.DuplicateAddedRecipeDiscovered);
                ValidateSets(allAlias, entries.AliasRecipesIDs, entries.DuplicateAliasRecipesDiscovered);                                
                ValidateSets(allFoods, entries.CustomFoodIDs, entries.DuplicateCustomFoodsDiscovered);                                
            }
        }

        private static void ValidateSets(HashSet<string> masterSet, ICollection<string> setToCheck, Action<string> informDuplicate)
        {
            foreach (string id in setToCheck)
            {
                if (masterSet.Contains(id))
                    informDuplicate.Invoke(id);
                else
                    masterSet.Add(id);
            }
        }
    }
}
