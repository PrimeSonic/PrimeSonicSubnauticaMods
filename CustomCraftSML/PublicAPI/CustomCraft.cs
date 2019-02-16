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
                        return TechTypeHandler.AddTechType(aliasRecipe.ItemID, aliasRecipe.DisplayName, aliasRecipe.Tooltip, false);

                    return TechType.None;
                case IAddedRecipe addedRecipe:
                    return GetTechType(addedRecipe.ItemID);

                case IModifiedRecipe modifiedRecipe:
                    return GetTechType(modifiedRecipe.ItemID);

                case ICustomSize customSize:
                    return GetTechType(customSize.ItemID);

                case ICustomBioFuel customBioFuel:
                    return GetTechType(customBioFuel.ItemID);

                case IMovedRecipe movedRecipe:
                    return GetTechType(movedRecipe.ItemID);

                case ICustomFragmentCount customFragment:
                    return GetTechType(customFragment.ItemID);

                default:
                    QuickLogger.Error($"Type check failure in CustomCraft.PrePass on entry for '{entry.ItemID}'");
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

                case ICustomFragmentCount customFragment:
                    return CustomizeFragments(customFragment);

                default:
                    QuickLogger.Error($"Type check failure in CustomCraft.AddEntry on entry for '{entry.ItemID}'");
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
                return
                    HandleModifiedRecipe(modifiedRecipe) &&
                    HandleUnlocks(modifiedRecipe);
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
                CraftDataHandler.SetItemSize(customSize.TechID, customSize.Width, customSize.Height);
                QuickLogger.Message($"'{customSize.ItemID}' was resized to {customSize.Width}x{customSize.Height}");
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
                BioReactorHandler.SetBioReactorCharge(customBioFuel.TechID, customBioFuel.Energy);
                QuickLogger.Message($"'{customBioFuel.ItemID}' now provides {customBioFuel.Energy} energy in the BioReactor");
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
            if (string.IsNullOrEmpty(movedRecipe.OldPath))
            {
                QuickLogger.Warning($"OldPath missing in MovedRecipe for '{movedRecipe.ItemID}'");
                return false;
            }

            if (!movedRecipe.Hidden && string.IsNullOrEmpty(movedRecipe.NewPath))
            {
                QuickLogger.Warning($"NewPath missing in MovedRecipe for '{movedRecipe.ItemID}' while not set as 'Hidden'");
                return false;
            }

            var oldPath = new CraftingPath(movedRecipe.OldPath);
            string[] oldSteps = (oldPath.Path + CraftingNode.Splitter + movedRecipe.ItemID).Split(CraftingNode.Splitter);

            CraftTreeHandler.RemoveNode(oldPath.Scheme, oldSteps);
            QuickLogger.Message($"Recipe for '{movedRecipe.ItemID}' was removed from the {oldPath.Scheme} crafting tree");

            if (movedRecipe.Hidden)
            {
                return true;
            }

            var newPath = new CraftingPath(movedRecipe.NewPath);

            AddCraftNode(newPath, movedRecipe.TechID);

            return true;
        }

        internal static bool CustomizeFragments(ICustomFragmentCount fragments)
        {
            int fragCount = fragments.FragmentsToScan;
            if (fragCount < PDAScanner.EntryData.minFragments ||
                fragCount > PDAScanner.EntryData.maxFragments)
            {
                QuickLogger.Warning($"Invalid number of FragmentsToScan for entry '{fragments.ItemID}'. Must be between {PDAScanner.EntryData.minFragments} and {PDAScanner.EntryData.maxFragments}.");
                return false;
            }

            if (fragments.TechID > TechType.Databox)
            {
                QuickLogger.Warning($"Item '{fragments.ItemID}' appears to be a modded item. CustomFragmentCount can only be applied to existing game items.");
                return false;
            }

            PDAHandler.EditFragmentsToScan(fragments.TechID, fragCount);
            QuickLogger.Message($"'{fragments.ItemID}' now requires {fragCount} fragments scanned to unlock.");
            return true;
        }

        // ----------------------

        private static void AddCraftNode(CraftingPath newPath, TechType itemID)
        {
            if (newPath.IsAtRoot)
            {
                CraftTreeHandler.AddCraftingNode(newPath.Scheme, itemID);
                QuickLogger.Message($"New crafting node for '{itemID}' added to the root of the {newPath.Scheme} crafting tree");
            }
            else
            {
                CraftTreeHandler.AddCraftingNode(newPath.Scheme, itemID, newPath.Steps);
                QuickLogger.Message($"New crafting node for '{itemID}' added to the {newPath.Scheme} crafting tree at {newPath.Path}");
            }
        }

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
            if (aliasRecipe.SpriteItemID > TechType.None)
            {
                QuickLogger.Message($"SpriteItemID {aliasRecipe.SpriteItemID} used for AliasRecipe '{aliasRecipe.ItemID}'");
                Atlas.Sprite sprite = SpriteManager.Get(aliasRecipe.SpriteItemID);
                SpriteHandler.RegisterSprite(aliasRecipe.TechID, sprite);
                return;
            }

            string imagePath = FileReaderWriter.AssetsFolder + aliasRecipe.ItemID + @".png";
            if (File.Exists(imagePath))
            {
                QuickLogger.Message($"Custom sprite found for AliasRecipe '{aliasRecipe.ItemID}'");
                Atlas.Sprite sprite = ImageUtils.LoadSpriteFromFile(imagePath);
                SpriteHandler.RegisterSprite(aliasRecipe.TechID, sprite);
                return;
            }

            if (aliasRecipe.LinkedItemsCount > 0)
            {
                QuickLogger.Message($"First LinkedItemID used for icon of AliasRecipe '{aliasRecipe.ItemID}'");
                Atlas.Sprite sprite = SpriteManager.Get(GetTechType(aliasRecipe.GetLinkedItem(0)));
                SpriteHandler.RegisterSprite(aliasRecipe.TechID, sprite);
                return;
            }

            QuickLogger.Warning($"No sprite loaded for '{aliasRecipe.ItemID}'");
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

            AddCraftNode(craftPath, addedRecipe.TechID);
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
            QuickLogger.Message($"Adding new recipe for '{addedRecipe.ItemID}'");

            if (addedRecipe.PdaGroup != TechGroup.Uncategorized)
            {
                CraftDataHandler.AddToGroup(addedRecipe.PdaGroup, addedRecipe.PdaCategory, itemID);
                QuickLogger.Message($"'{addedRecipe.ItemID}' categorized in the PDA under {addedRecipe.PdaGroup}/{addedRecipe.PdaCategory}");
            }
        }

        private static bool HandleModifiedRecipe(IModifiedRecipe modifiedRecipe)
        {
            bool overrideRecipe = false;

            TechType itemID = GetTechType(modifiedRecipe.ItemID);

            if (itemID == TechType.None)
                return false; // Unknown item

            ITechData original = CraftData.Get(itemID, skipWarnings: true);

            if (original == null) // Possibly a mod recipe
                original = CraftDataHandler.GetModdedTechData(itemID);

            if (original == null)
                return false;  // Unknown recipe

            var replacement = new TechData();

            // Amount
            if (modifiedRecipe.AmountCrafted.HasValue)
            {
                overrideRecipe |= true;
                replacement.craftAmount = modifiedRecipe.AmountCrafted.Value;
            }
            else
            {
                replacement.craftAmount = original.craftAmount;
            }

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
            {
                CraftDataHandler.SetTechData(itemID, replacement);
                QuickLogger.Message($"Modifying recipe for '{modifiedRecipe.ItemID}'");
            }

            return true;
        }

        private static bool HandleUnlocks(IModifiedRecipe modifiedRecipe)
        {
            if (modifiedRecipe.ForceUnlockAtStart)
            {
                KnownTechHandler.UnlockOnStart(modifiedRecipe.TechID);
                QuickLogger.Message($"Recipe for '{modifiedRecipe.ItemID}' will be a unlocked at the start of the game");
            }

            if (modifiedRecipe.UnlocksCount.HasValue && modifiedRecipe.UnlocksCount > 0)
            {
                var unlocks = new List<TechType>();

                foreach (string value in modifiedRecipe.Unlocks)
                {
                    unlocks.Add(GetTechType(value));
                    QuickLogger.Message($"Recipe for '{value}' will be a unlocked when '{modifiedRecipe.ItemID}' is scanned or picked up");
                }

                KnownTechHandler.SetAnalysisTechEntry(modifiedRecipe.TechID, unlocks);
            }

            return true;
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
                QuickLogger.Message($"Custom item '{aliasRecipe.ItemID}' will be a functional clone of '{aliasRecipe.FunctionalID}'");
            }
        }
    }
}
