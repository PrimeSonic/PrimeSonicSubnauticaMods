namespace CustomCraft2SML
{
    using System;
    using System.IO;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization;

    internal static partial class FileReaderWriter
    {
        private const string CustomSizesFile = FolderRoot + "CustomSizes.txt";
        private const string ModifiedRecipesFile = FolderRoot + "ModifiedRecipes.txt";
        private const string AddedRecipiesFile = FolderRoot + "AddedRecipes.txt";

        private static CustomSizeList customSizeList;
        private static ModifiedRecipeList modifiedRecipeList;
        private static AddedRecipeList addedRecipeList;

        internal static void PatchAddedRecipes()
        {
            addedRecipeList = new AddedRecipeList();
            if (File.Exists(AddedRecipiesFile))
            {
                string serializedData = File.ReadAllText(AddedRecipiesFile);
                if (!string.IsNullOrEmpty(serializedData) && // Not a blank file
                    addedRecipeList.Deserialize(serializedData) && // Correctly parsed
                    addedRecipeList.Count > 0) // Has entries
                {
                    int successful = 0;
                    foreach (IAddedRecipe item in addedRecipeList)
                    {
                        try
                        {
                            CustomCraft.AddRecipe(item);
#if DEBUG
                            Logger.Log($"Added recipe for {item.ItemID}");
#endif
                            successful++;
                        }
                        catch
                        {
                            QuickLogger.Error($"Error on AddRecipe{Environment.NewLine}" +
                                              $"Entry with error:{Environment.NewLine}" +
                                              $"{item}", false);
                        }
                    }

                    Logger.Log($"{successful}/{addedRecipeList.Count} AddedRecipies loaded.");
                }
                else
                {
                    Logger.Log($"No AddedRecipes were loaded. {AddedRecipiesFile} was empty or could not be read.");
                }
            }
            else
            {
                File.WriteAllText(AddedRecipiesFile, $"# Added Recipes #{Environment.NewLine}" +
                    $"# Check the AddedRecipes_Samples.txt file in the SampleFiles folder for details on how to add recipes for items normally not craftable #{Environment.NewLine}");
                Logger.Log($"{AddedRecipiesFile} file not found. Empty file created.");
            }
        }

        internal static void PatchModifiedRecipes()
        {
            modifiedRecipeList = new ModifiedRecipeList();
            if (File.Exists(ModifiedRecipesFile))
            {
                string serializedData = File.ReadAllText(ModifiedRecipesFile);                
                if (!string.IsNullOrEmpty(serializedData) && // Not a blank file
                    modifiedRecipeList.Deserialize(serializedData) && // Parsed correctly
                    modifiedRecipeList.Count > 0) // Has entries
                {
                    int successful = 0;
                    foreach (IModifiedRecipe item in modifiedRecipeList)
                    {
                        try
                        {
                            CustomCraft.ModifyRecipe(item);
#if DEBUG
                            Logger.Log($"Modified recipe for {item.ItemID}");
#endif
                            successful++;
                        }
                        catch
                        {
                            QuickLogger.Error($"Error on ModifyRecipe{Environment.NewLine}" +
                                              $"Entry with error:{Environment.NewLine}" +
                                              $"{item}", false);
                        }
                    }

                    Logger.Log($"{successful}/{modifiedRecipeList.Count} ModifiedRecipes loaded.");
                }
                else
                {
                    Logger.Log($"No ModifiedRecipes were loaded. {ModifiedRecipesFile} was empty or could not be read.");
                }
            }
            else
            {
                File.WriteAllText(ModifiedRecipesFile, $"# Modified Recipes #{Environment.NewLine}" +
                    $"# Check the ModifiedRecipes_Samples.txt file in the SampleFiles folder for details on how to alter existing crafting recipes #{Environment.NewLine}");
                Logger.Log($"{ModifiedRecipesFile} file not found. Empty file created.");
            }
        }

        internal static void PatchCustomSizes()
        {
            customSizeList = new CustomSizeList();
            if (File.Exists(CustomSizesFile))
            {
                string serializedData = File.ReadAllText(CustomSizesFile);
                if (!string.IsNullOrEmpty(serializedData) && // Not a blank file
                    customSizeList.Deserialize(serializedData) && // Parsed correctly
                    customSizeList.Count > 0) // Has entires
                {
                    int successful = 0;
                    foreach (ICustomSize customSize in customSizeList)
                    {
                        try
                        {
                            CustomCraft.CustomizeItemSize(customSize);
#if DEBUG
                            Logger.Log($"Custom size for {customSize.ItemID}");
#endif
                            successful++;
                        }
                        catch
                        {
                            QuickLogger.Error($"Error on CustomizeItemSize{Environment.NewLine}" +
                                              $"Entry with error:{Environment.NewLine}" +
                                              $"{customSize}", false);
                        }
                    }

                    Logger.Log($"{successful}/{customSizeList.Count} CustomSizes succesffully loaded.");
                }
                else
                {
                    Logger.Log($"No CustomSizes were loaded. {CustomSizesFile} was empty or could not be read.");
                }
            }
            else
            {
                File.WriteAllText(CustomSizesFile,
                    $"# Custom Sizes go in this file #{Environment.NewLine}" +
                    $"# Check the CustomSizes_Samples.txt file in the SampleFiles folder for details on how to set your own custom sizes #{Environment.NewLine}");
                Logger.Log($"{CustomSizesFile} file not found. Empty file created.");
            }
        }
    }
}
