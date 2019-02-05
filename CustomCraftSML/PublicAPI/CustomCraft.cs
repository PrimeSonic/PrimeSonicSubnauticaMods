namespace CustomCraft2SML.PublicAPI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Common;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Serialization.Components;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;

    public static class CustomCraft
    {
        public static TechType GetTechType(string value)
        {
            // Look for a known TechType
            if (TechTypeExtensions.FromString(value, out TechType tType, true))
            {
                return tType;
            }

            //  Not one of the known TechTypes - is it registered with SMLHelper?
            if (TechTypeHandler.TryGetModdedTechType(value, out TechType custom))
            {
                return custom;
            }

            return TechType.None;
        }

        public static TechType PrePass<T>(T entry)
            where T : ITechTyped
        {
            switch (entry)
            {
                case IAliasRecipe aliasRecipe:
                    //  Register the alias TechType
                    if (!string.IsNullOrEmpty(aliasRecipe.ItemID))
                        return TechTypeHandler.AddTechType(aliasRecipe.ItemID, aliasRecipe.DisplayName, aliasRecipe.Tooltip);

                    return TechType.None;
                case IAddedRecipe addedRecipe:
                    return GetTechType(addedRecipe.ItemID);
                case IModifiedRecipe modifiedRecipe:
                    return GetTechType(modifiedRecipe.ItemID);
                case ICustomSize customSize:
                    return GetTechType(customSize.ItemID);
                case ICustomBioFuel customBioFuel:
                    return GetTechType(customBioFuel.ItemID);
                default:
                    QuickLogger.Error("Type check failure in CustomCraft.PrePass");
                    return TechType.None;
            }
        }

        public static bool AddEntry<T>(T entry)
            where T : ITechTyped
        {
            switch (entry)
            {
                case IAliasRecipe aliasRecipe:
                    return AliasRecipe(aliasRecipe);
                case IAddedRecipe addedRecipe:
                    return AddRecipe(addedRecipe);
                case IModifiedRecipe modifiedRecipe:
                    return ModifyRecipe(modifiedRecipe);
                case ICustomSize customSize:
                    return CustomizeItemSize(customSize);
                case ICustomBioFuel customBioFuel:
                    return CustomizeBioFuel(customBioFuel);
                case IMovedRecipe movedRecipe:
                    return MoveRecipe(movedRecipe);
                //case ICustomFragmentCount customFragment:
                    //return CustomizeFragments(customFragment);
                default:
                    QuickLogger.Error("Type check failure in CustomCraft.AddEntry");
                    return false;
            }
        }

        internal static bool AddRecipe(IAddedRecipe addedRecipe)
        {
            try
            {
                HandleAddedRecipe(addedRecipe);

                HandleCraftTreeAddition(addedRecipe);

                HandleUnlocks(addedRecipe);

                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling Added Recipe '{addedRecipe.ItemID}'{Environment.NewLine}{ex}");
                return false;
            }
        }

        internal static bool AliasRecipe(IAliasRecipe aliasRecipe)
        {
            try
            {
                //  See if there is an asset in the asset folder that has the same name
                HandleCustomSprite(aliasRecipe);

                HandleAddedRecipe(aliasRecipe, 0 /* alias recipes should default to not producing the custom item unless explicitly configured */);

                HandleCraftTreeAddition(aliasRecipe);

                HandleUnlocks(aliasRecipe);

                HandleFunctionalClone(aliasRecipe);

                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling Alias Recipe '{aliasRecipe.ItemID}'{Environment.NewLine}{ex}");
                return false;
            }
        }

        internal static bool ModifyRecipe(IModifiedRecipe modifiedRecipe)
        {
            try
            {
                HandleModifiedRecipe(modifiedRecipe);

                HandleUnlocks(modifiedRecipe);

                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling Modified Recipe '{modifiedRecipe.ItemID}'{Environment.NewLine}{ex}");
                return false;
            }
        }

        internal static bool CustomizeItemSize(ICustomSize customSize)
        {
            if (customSize.Width <= 0 || customSize.Height <= 0 || customSize.Width > 6 || customSize.Height > 6)
            {
                QuickLogger.Error($"Error in custom size for '{customSize.ItemID}'. Size values must be between 1 and 6.");
                return false;
            }

            try
            {
                CraftDataHandler.SetItemSize(GetTechType(customSize.ItemID), customSize.Width, customSize.Height);
                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling Custom Item Size '{customSize.ItemID}'{Environment.NewLine}{ex}");
                return false;
            }
        }

        internal static bool CustomizeBioFuel(ICustomBioFuel customBioFuel)
        {
            try
            {
                BioReactorHandler.SetBioReactorCharge(GetTechType(customBioFuel.ItemID), customBioFuel.Energy);
                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling Modified Recipe '{customBioFuel.ItemID}'{Environment.NewLine}{ex}");
                return false;
            }
        }

        internal static bool AddCustomCraftingTab(ICraftingTab craftingTab)
        {
            if (craftingTab.FabricatorType > CraftTree.Type.Rocket)
            {
                QuickLogger.Error($"Error on crafting tab '{craftingTab.TabID}'. This API in intended only for use with standard, non-modded CraftTree.Types.");
                return false;
            }

            if (craftingTab.FabricatorType == CraftTree.Type.None)
            {
                QuickLogger.Error($"Error on crafting tab '{craftingTab.TabID}'. ParentTabPath must identify a fabricator for the custom tab.");
                return false;
            }
            try
            {
                HandleCraftingTab(craftingTab);

                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling crafting tab '{craftingTab.TabID}'{Environment.NewLine}{ex}");
                return false;
            }
        }

        internal static bool MoveRecipe(IMovedRecipe movedRecipe)
        {
            if (!movedRecipe.IsComplete)
            {
                QuickLogger.Warning($"Error on moved recipe for '{movedRecipe.ItemID}'. Moved recipes require both the Old and New paths.");
                return false;
            }

            var oldPath = new CraftingPath(movedRecipe.OldPath);
            var newPath = new CraftingPath(movedRecipe.NewPath);

            string[] oldSteps = (oldPath.Path + CraftingNode.Splitter + movedRecipe.ItemID).Split(CraftingNode.Splitter);
            string[] newSteps = newPath.Path.Split(CraftingNode.Splitter);

            CraftTreeHandler.RemoveNode(oldPath.Scheme, oldSteps);

            TechType itemID = GetTechType(movedRecipe.ItemID);

            if (newSteps.Length <= 1)
                CraftTreeHandler.AddCraftingNode(newPath.Scheme, itemID);
            else
                CraftTreeHandler.AddCraftingNode(newPath.Scheme, itemID, newSteps);

            return true;
        }

        internal static bool CustomizeFragments(ICustomFragmentCount fragments)
        {
            TechType itemID = GetTechType(fragments.ItemID);

            if (itemID == TechType.None)
                return false;

            int fragCount = fragments.FragmentsToScan;
            if (fragCount < PDAScanner.EntryData.minFragments || 
                fragCount > PDAScanner.EntryData.maxFragments)
            {
                QuickLogger.Warning($"Invalid number of FragmentsToScan for entry '{fragments.ItemID}'. Must be between {PDAScanner.EntryData.minFragments} and {PDAScanner.EntryData.maxFragments}.");
                return false;
            }

            if (ScannerMappings.BlueprintToFragment.TryGetValue(itemID, out PDAScanner.EntryData entryData))
            {
                entryData.totalFragments = fragCount;
                return true;
            }

            QuickLogger.Warning($"Item '{fragments.ItemID}' for CustomFragmentCount does not have matchign fragments.");
            return false;
        }

        // ----------------------

        private static void HandleCraftingTab(ICraftingTab craftingTab)
        {
            Atlas.Sprite sprite = GetCraftingTabSprite(craftingTab);

            if (craftingTab.StepsToTab == null)
            {
                CraftTreeHandler.AddTabNode(craftingTab.FabricatorType, craftingTab.TabID, craftingTab.DisplayName, sprite);
            }
            else
            {
                CraftTreeHandler.AddTabNode(craftingTab.FabricatorType, craftingTab.TabID, craftingTab.DisplayName, sprite, craftingTab.StepsToTab);
            }
        }

        private static void HandleCustomSprite(IAliasRecipe aliasRecipe)
        {
            string imagePath = FileReaderWriter.AssetsFolder + aliasRecipe.ItemID + @".png";
            if (File.Exists(imagePath))
            {
                QuickLogger.Message($"Custom sprite found for AliasRecipe '{aliasRecipe.ItemID}'");
                Atlas.Sprite sprite = ImageUtils.LoadSpriteFromFile(imagePath);
                SpriteHandler.RegisterSprite(GetTechType(aliasRecipe.ItemID), sprite);
            }
            else if (aliasRecipe.LinkedItemsCount > 0)
            {
                QuickLogger.Message($"First LinkedItemID used for icon of AliasRecipe '{aliasRecipe.ItemID}'");
                Atlas.Sprite sprite = SpriteManager.Get(GetTechType(aliasRecipe.GetLinkedItem(0)));
                SpriteHandler.RegisterSprite(GetTechType(aliasRecipe.ItemID), sprite);
            }
            else
            {
                QuickLogger.Warning($"No sprite loaded for '{aliasRecipe.ItemID}'");
            }
        }

        private static Atlas.Sprite GetCraftingTabSprite(ICraftingTab craftingTab)
        {
            Atlas.Sprite sprite;
            string imagePath = FileReaderWriter.AssetsFolder + craftingTab.TabID + @".png";
            if (File.Exists(imagePath))
            {
                QuickLogger.Message($"Custom sprite found for CraftingTab '{craftingTab.TabID}'");
                sprite = ImageUtils.LoadSpriteFromFile(imagePath);
            }
            else if (craftingTab.SpriteItemID != TechType.None)
            {
                QuickLogger.Message($"SpriteItemID used for CraftingTab '{craftingTab.TabID}'");
                sprite = SpriteManager.Get(craftingTab.SpriteItemID);
            }
            else
            {
                QuickLogger.Warning($"No sprite loaded for CraftingTab '{craftingTab.TabID}'");
                sprite = SpriteManager.Get(TechType.None);
            }

            return sprite;
        }

        private static void HandleCraftTreeAddition(IAddedRecipe addedRecipe)
        {
            var craftPath = new CraftingPath(addedRecipe.Path);

            TechType itemID = GetTechType(addedRecipe.ItemID);

            if (craftPath.IsAtRoot)
            {
                CraftTreeHandler.AddCraftingNode(craftPath.Scheme, itemID);
            }
            else
            {
                string[] steps = craftPath.Path.Split(CraftingNode.Splitter);

                CraftTreeHandler.AddCraftingNode(craftPath.Scheme, itemID, steps);
            }
        }

        private static void HandleAddedRecipe(IAddedRecipe addedRecipe, short defaultCraftAmount = 1)
        {
            var replacement = new TechData
            {
                craftAmount = addedRecipe.AmountCrafted ?? defaultCraftAmount
            };

            foreach (EmIngredient ingredient in addedRecipe.Ingredients)
                replacement.Ingredients.Add(new Ingredient(GetTechType(ingredient.ItemID), ingredient.Required));

            foreach (string linkedItem in addedRecipe.LinkedItems)
                replacement.LinkedItems.Add(GetTechType(linkedItem));

            TechType itemID = GetTechType(addedRecipe.ItemID);

            CraftDataHandler.SetTechData(itemID, replacement);

            if (addedRecipe.PdaGroup != TechGroup.Uncategorized)
            {
                CraftDataHandler.AddToGroup(addedRecipe.PdaGroup, addedRecipe.PdaCategory, itemID);
            }
        }

        private static void HandleModifiedRecipe(IModifiedRecipe modifiedRecipe)
        {
            bool overrideRecipe = false;

            ITechData original = CraftData.Get(GetTechType(modifiedRecipe.ItemID));

            var replacement = new TechData();

            // Amount
            if (modifiedRecipe.AmountCrafted.HasValue)
            {
                overrideRecipe |= true;
                replacement.craftAmount = modifiedRecipe.AmountCrafted.Value;
            }
            else
                replacement.craftAmount = original.craftAmount;

            // Ingredients
            if (modifiedRecipe.IngredientsCount.HasValue)
            {
                overrideRecipe |= true;
                foreach (EmIngredient ingredient in modifiedRecipe.Ingredients)
                {
                    replacement.Ingredients.Add(
                        new Ingredient(
                            GetTechType(ingredient.ItemID),
                            ingredient.Required));
                }
            }
            else
            {
                for (int i = 0; i < original.ingredientCount; i++)
                    replacement.Ingredients.Add(
                        new Ingredient(
                        original.GetIngredient(i).techType,
                        original.GetIngredient(i).amount));
            }

            // Linked Items
            if (modifiedRecipe.LinkedItemsCount.HasValue)
            {
                overrideRecipe |= true;
                foreach (string linkedItem in modifiedRecipe.LinkedItems)
                    replacement.LinkedItems.Add(GetTechType(linkedItem));
            }
            else
            {
                for (int i = 0; i < original.linkedItemCount; i++)
                    replacement.LinkedItems.Add(original.GetLinkedItem(i));
            }

            if (overrideRecipe)
                CraftDataHandler.SetTechData(GetTechType(modifiedRecipe.ItemID), replacement);
        }

        private static void HandleUnlocks(IModifiedRecipe modifiedRecipe)
        {
            if (modifiedRecipe.ForceUnlockAtStart)
                KnownTechHandler.UnlockOnStart(GetTechType(modifiedRecipe.ItemID));

            if (modifiedRecipe.UnlocksCount.HasValue)
            {
                var unlocks = new List<TechType>();

                foreach (string value in modifiedRecipe.Unlocks)
                {
                    unlocks.Add(GetTechType(value));
                }

                KnownTechHandler.SetAnalysisTechEntry(GetTechType(modifiedRecipe.ItemID), unlocks);
            }
        }

        private static void HandleFunctionalClone(IAliasRecipe aliasRecipe)
        {
            if (string.IsNullOrEmpty(aliasRecipe.FunctionalID))
                return; // No value provided. This is fine.

            TechType functionalID = GetTechType(aliasRecipe.FunctionalID);

            if (functionalID != TechType.None)
            {
                var clone = new FunctionalClone(aliasRecipe, functionalID);
                PrefabHandler.RegisterPrefab(clone);
            }
        }
    }
}
