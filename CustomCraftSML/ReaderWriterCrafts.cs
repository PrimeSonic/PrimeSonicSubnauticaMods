namespace CustomCraft2SML
{
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


        private static IEnumerable<IParsingPackage> OrderedPackages = new List<IParsingPackage>(8)
        {
            new ParsingPackage<CustomCraftingTab, CustomCraftingTabList>(CustomCraftingTabList.ListKey),
            new ParsingPackage<MovedRecipe, MovedRecipeList>(MovedRecipeList.ListKey),
            new ParsingPackage<AddedRecipe, AddedRecipeList>(AddedRecipeList.ListKey),
            new ParsingPackage<AliasRecipe, AliasRecipeList>(AliasRecipeList.ListKey),
            new ParsingPackage<ModifiedRecipe, ModifiedRecipeList>(ModifiedRecipeList.ListKey),
            new ParsingPackage<CustomSize, CustomSizeList>(CustomSizeList.ListKey),
            new ParsingPackage<CustomBioFuel, CustomBioFuelList>(CustomBioFuelList.ListKey),
            new ParsingPackage<CustomFragmentCount, CustomFragmentCountList>(CustomFragmentCountList.ListKey)
        };

        private static IDictionary<string, IParsingPackage> PackagesLookup = new Dictionary<string, IParsingPackage>(8);

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
                if (PackagesLookup.TryGetValue(key, out IParsingPackage package))
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
