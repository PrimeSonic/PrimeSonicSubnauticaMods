namespace CustomCraft2SML
{
    using System;
    using System.IO;
    using Common.EasyMarkup;
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization;

    internal static class ParsingFiles
    {
        private static readonly string FolderRoot = $"./QMods/{CustomCraft.RootModName}/";
        private static readonly string CustomSizesFile = FolderRoot + "CustomSizes.txt";
        private static readonly string ModifiedRecipesFile = FolderRoot + "ModifiedRecipes.txt";
        private static readonly string AddedRecipiesFile = FolderRoot + "AddedRecipes.txt";

        private static CustomSizeList customSizeList;
        private static ModifiedRecipeList modifiedRecipeList;
        private static AddedRecipeList addedRecipeList;

        internal static void PatchAddedRecipes()
        {
            addedRecipeList = new AddedRecipeList();
            if (File.Exists(AddedRecipiesFile))
            {
                string serializedData = File.ReadAllText(AddedRecipiesFile);
                if (!string.IsNullOrEmpty(serializedData) && addedRecipeList.Deserialize(serializedData) && addedRecipeList.Count > 0)
                {
                    foreach (IAddedRecipe item in addedRecipeList)
                    {
                        try
                        {
                            CustomCraft.AddRecipe(item);
#if DEBUG
                            Logger.Log($"Added recipe for {item.ItemID}");
#endif
                        }
                        catch
                        {
                            Logger.Log($"Error on AddRecipe{Environment.NewLine}" +
                                        $"Entry with error:{Environment.NewLine}" +
                                        $"{item}");
                        }
                    }

                    Logger.Log($"AddedRecipies loaded. File reformatted.");
                    File.WriteAllText(AddedRecipiesFile, addedRecipeList.PrettyPrint());
                }
                else
                {
                    Logger.Log($"No AddedRecipes were loaded. File was empty or malformed.");
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
                if (!string.IsNullOrEmpty(serializedData) && modifiedRecipeList.Deserialize(serializedData) && modifiedRecipeList.Count > 0)
                {
                    foreach (IModifiedRecipe item in modifiedRecipeList)
                    {
                        try
                        {
                            CustomCraft.ModifyRecipe(item);
#if DEBUG
                            Logger.Log($"Modified recipe for {item.ItemID}");
#endif
                        }
                        catch
                        {
                            Logger.Log($"Error on ModifyRecipe{Environment.NewLine}" +
                                        $"Entry with error:{Environment.NewLine}" +
                                        $"{item}");
                        }
                    }

                    Logger.Log($"ModifiedRecipes loaded. File reformatted.");
                    File.WriteAllText(ModifiedRecipesFile, modifiedRecipeList.PrettyPrint());
                }
                else
                {
                    Logger.Log($"No ModifiedRecipes were loaded. File was empty or malformed.");
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
                if (!string.IsNullOrEmpty(serializedData) && customSizeList.Deserialize(serializedData) && customSizeList.Count > 0)
                {
                    foreach (ICustomSize customSize in customSizeList)
                    {
                        try
                        {
                            CustomCraft.CustomizeItemSize(customSize);
#if DEBUG
                            Logger.Log($"Custom size for {customSize.ItemID}");
#endif
                        }
                        catch
                        {
                            Logger.Log($"Error on CustomizeItemSize{Environment.NewLine}" +
                                        $"Entry with error:{Environment.NewLine}" +
                                        $"{customSize}");
                        }
                    }

                    Logger.Log($"CustomSizes loaded. File reformatted.");
                    File.WriteAllText(CustomSizesFile, customSizeList.PrettyPrint());
                }
                else
                {
                    Logger.Log($"No CustomSizes were loaded. File was empty or malformed.");
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
