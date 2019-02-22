namespace CustomCraft2SML
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Serialization;
    using CustomCraft2SML.Serialization.Entries;
    using CustomCraft2SML.Serialization.Lists;

    internal static partial class FileReaderWriter
    {
        internal const string WorkingFolder = FolderRoot + "WorkingFiles/";
        internal const string AssetsFolder = FolderRoot + "Assets/";

        internal static readonly ParsingPackage<CustomCraftingTab, CustomCraftingTabList> CustomTabs = new ParsingPackage<CustomCraftingTab, CustomCraftingTabList>(CustomCraftingTabList.ListKey);
        internal static readonly ParsingPackage<MovedRecipe, MovedRecipeList> MovedRecipes = new ParsingPackage<MovedRecipe, MovedRecipeList>(MovedRecipeList.ListKey);
        internal static readonly ParsingPackage<AddedRecipe, AddedRecipeList> AddedRecipes = new ParsingPackage<AddedRecipe, AddedRecipeList>(AddedRecipeList.ListKey);
        internal static readonly ParsingPackage<AliasRecipe, AliasRecipeList> AliasRecipes = new ParsingPackage<AliasRecipe, AliasRecipeList>(AliasRecipeList.ListKey);
        internal static readonly ParsingPackage<CustomFabricator, CustomFabricatorList> CustomFabricatorParser = new ParsingPackage<CustomFabricator, CustomFabricatorList>(CustomFabricatorList.ListKey);
        internal static readonly ParsingPackage<ModifiedRecipe, ModifiedRecipeList> ModifiedRecipeParser = new ParsingPackage<ModifiedRecipe, ModifiedRecipeList>(ModifiedRecipeList.ListKey);
        internal static readonly ParsingPackage<CustomSize, CustomSizeList> CustomSizeParser = new ParsingPackage<CustomSize, CustomSizeList>(CustomSizeList.ListKey);
        internal static readonly ParsingPackage<CustomBioFuel, CustomBioFuelList> CustomBioFuelParser = new ParsingPackage<CustomBioFuel, CustomBioFuelList>(CustomBioFuelList.ListKey);
        internal static readonly ParsingPackage<CustomFragmentCount, CustomFragmentCountList> CustomFragCountParser = new ParsingPackage<CustomFragmentCount, CustomFragmentCountList>(CustomFragmentCountList.ListKey);

        private static IEnumerable<IParsingPackage> OrderedPackages = new List<IParsingPackage>(8)
        {
            CustomFabricatorParser,
            CustomTabs,
            MovedRecipes,
            AddedRecipes,
            AliasRecipes,
            ModifiedRecipeParser,
            CustomSizeParser,
            CustomBioFuelParser,
            CustomFragCountParser
        };

        private static IDictionary<string, IParsingPackage> PackagesLookup = new Dictionary<string, IParsingPackage>(9);

        internal static void HandleWorkingFiles()
        {
            foreach (IParsingPackage package in OrderedPackages)
                PackagesLookup.Add(package.ListKey, package);

            if (!Directory.Exists(AssetsFolder))
                Directory.CreateDirectory(AssetsFolder);

            string[] workingFiles = Directory.GetFiles(WorkingFolder);

            QuickLogger.Message($"{workingFiles.Length} files found in the WorkingFiles folder");
            foreach (string file in workingFiles)
                DeserializeFile(file);

            QuickLogger.Message($"Validating entries - First Pass");
            foreach (IParsingPackage package in OrderedPackages)
                package.PrePassValidation();

            QuickLogger.Message($"Validating entries - Second Pass");
            MasterUniquenessValidation();


            QuickLogger.Message($"Sending requests to SMLHelper");
            foreach (IParsingPackage package in OrderedPackages)
                package.SendToSMLHelper();
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
                if (PackagesLookup.TryGetValue(key, out IParsingPackage package))
                {
                    check = package.ParseEntries(serializedData, OriginFile.GetOriginFile(fileName));
                }
                else
                {
                    QuickLogger.Error($"Unknown primary key '{key}' detected in file '{fileName}'");
                    return;
                }

                switch (check)
                {
                    case -2:
                        QuickLogger.Error($"Unexpected error when attempting to parse file '{fileName}'");
                        break;
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

        private static void MasterUniquenessValidation()
        {
            var allTabs = new HashSet<string>(CustomTabs.UniqueEntries.Keys, StringComparer.InvariantCultureIgnoreCase);
            var allMoves = new HashSet<string>(MovedRecipes.UniqueEntries.Keys, StringComparer.InvariantCultureIgnoreCase);
            var allAdded = new HashSet<string>(AddedRecipes.UniqueEntries.Keys, StringComparer.InvariantCultureIgnoreCase);
            var allAlias = new HashSet<string>(AliasRecipes.UniqueEntries.Keys, StringComparer.InvariantCultureIgnoreCase);

            foreach (IFabricatorEntries entries in CustomFabricatorParser.UniqueEntries.Values)
            {
                foreach (string tabID in entries.CustomTabIDs)
                {
                    if (allTabs.Contains(tabID))
                        entries.DuplicateCustomTabDiscovered(tabID);
                    else
                        allTabs.Add(tabID);
                }

                foreach (string moveID in entries.MovedRecipeIDs)
                {
                    if (allMoves.Contains(moveID))
                        entries.DuplicateMovedRecipeDiscovered(moveID);
                    else
                        allMoves.Add(moveID);
                }

                foreach (string addID in entries.AddedRecipeIDs)
                {
                    if (allAdded.Contains(addID))
                        entries.DuplicateAddedRecipeDiscovered(addID);
                    else
                        allAdded.Add(addID);
                }

                foreach (string aliasID in entries.AliasRecipesIDs)
                {
                    if (allAlias.Contains(aliasID))
                        entries.DuplicateAliasRecipesDiscovered(aliasID);
                    else
                        allAlias.Add(aliasID);
                }
            }
        }


    }
}
