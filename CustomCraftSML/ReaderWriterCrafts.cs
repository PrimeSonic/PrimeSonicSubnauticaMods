namespace CustomCraft2SML
{
    using System.Collections.Generic;
    using System.IO;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization;
    using CustomCraft2SML.Serialization.Entries;
    using CustomCraft2SML.Serialization.Lists;

    internal static partial class FileReaderWriter
    {
        internal const string WorkingFolder = FolderRoot + "WorkingFiles/";
        internal const string AssetsFolder = FolderRoot + "Assets/";

        private static readonly IParsingPackage CustomCraftingTabs = new ParsingPackage<CustomCraftingTab, CustomCraftingTabList>(CustomCraftingTabList.ListKey);
        private static readonly IParsingPackage MovedRecipes = new ParsingPackage<MovedRecipe, MovedRecipeList>(MovedRecipeList.ListKey);
        private static readonly IParsingPackage AddedRecipes = new ParsingPackage<AddedRecipe, AddedRecipeList>(AddedRecipeList.ListKey);
        private static readonly IParsingPackage AliasRecipes = new ParsingPackage<AliasRecipe, AliasRecipeList>(AliasRecipeList.ListKey);
        private static readonly IParsingPackage ModifiedRecipes = new ParsingPackage<ModifiedRecipe, ModifiedRecipeList>(ModifiedRecipeList.ListKey);
        private static readonly IParsingPackage CustomSizes = new ParsingPackage<CustomSize, CustomSizeList>(CustomSizeList.ListKey);
        private static readonly IParsingPackage CustomBioFuels = new ParsingPackage<CustomBioFuel, CustomBioFuelList>(CustomBioFuelList.ListKey);
        private static readonly IParsingPackage CustomFragmentCounts = new ParsingPackage<CustomFragmentCount, CustomFragmentCountList>(CustomFragmentCountList.ListKey);

        private static IList<IParsingPackage> OrderedPackages = new List<IParsingPackage>();
        private static IDictionary<string, IParsingPackage> LookupPackages = new Dictionary<string, IParsingPackage>();

        private static void HandleWorkingFiles()
        {
            OrderedPackages.Add(CustomCraftingTabs);
            OrderedPackages.Add(MovedRecipes);
            OrderedPackages.Add(AddedRecipes);
            OrderedPackages.Add(AliasRecipes);
            OrderedPackages.Add(ModifiedRecipes);
            OrderedPackages.Add(CustomSizes);
            OrderedPackages.Add(CustomBioFuels);
            OrderedPackages.Add(CustomFragmentCounts);

            foreach (IParsingPackage package in OrderedPackages)
                LookupPackages.Add(package.ListKey, package);

            if (!Directory.Exists(AssetsFolder))
                Directory.CreateDirectory(AssetsFolder);

            QuickLogger.Message("Reading contents of WorkingFiles folder");

            ICollection<string> workingFiles = new List<string>(Directory.GetFiles(WorkingFolder));

            QuickLogger.Message($"{workingFiles.Count} files found");

            foreach (string file in workingFiles)
                DeserializeFile(file);

            QuickLogger.Message($"Validating parsed entries");

            foreach (IParsingPackage package in OrderedPackages)
                package.PrePassValidation();

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
                if (LookupPackages.TryGetValue(key, out IParsingPackage package))
                {
                    check = package.ParseEntries(serializedData);
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
    }
}
