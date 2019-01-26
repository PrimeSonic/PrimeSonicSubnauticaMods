namespace CustomCraft2SML.PublicAPI
{
    using System.Collections.Generic;
    using System.IO;
    using Common;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Serialization;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine.Assertions;

    public static class CustomCraft
    {
        public static TechType GetTechType(string value)
        {
            if (TechTypeExtensions.FromString(value, out TechType tType, true))
            {
                return tType;
            }
            else
            {
                //  Not one of the known tech types - is it registered with SMLHelper?
                if (TechTypeHandler.TryGetModdedTechType(value, out TechType custom))
                {
                    return custom;
                }
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
                    AliasRecipe(aliasRecipe);
                    return true;
                case IAddedRecipe addedRecipe:
                    AddRecipe(addedRecipe);
                    return true;
                case IModifiedRecipe modifiedRecipe:
                    ModifyRecipe(modifiedRecipe);
                    return true;
                case ICustomSize customSize:
                    CustomizeItemSize(customSize);
                    return true;
                case ICustomBioFuel customBioFuel:
                    CustomizeBioFuel(customBioFuel);
                    return true;
                default:
                    QuickLogger.Error("Type check failure in CustomCraft.AddEntry");
                    return false;
            }
        }

        internal static void AddRecipe(IAddedRecipe addedRecipe)
        {
            HandleAddedRecipe(addedRecipe);

            HandleCraftTreeAddition(addedRecipe);

            HandleUnlocks(addedRecipe);
        }

        internal static void AliasRecipe(IAliasRecipe aliasRecipe)
        {
            //  See if there is an asset in the asset folder that has the same name
            HandleCustomSprite(aliasRecipe);

            HandleAddedRecipe(aliasRecipe, 0 /* alias recipes should default to not producing the custom item unless explicitly configured */);

            HandleCraftTreeAddition(aliasRecipe);

            HandleUnlocks(aliasRecipe);
        }

        private static void HandleCustomSprite(IAliasRecipe aliasRecipe)
        {
            string imagePath = FileReaderWriter.AssetsFolder + aliasRecipe.ItemID + @".png";
            if (File.Exists(imagePath))
            {
                Atlas.Sprite sprite = ImageUtils.LoadSpriteFromFile(imagePath);
                SpriteHandler.RegisterSprite(GetTechType(aliasRecipe.ItemID), sprite);
            }
            else if (aliasRecipe.LinkedItemsCount > 0)
            {
                Atlas.Sprite sprite = SpriteManager.Get(GetTechType(aliasRecipe.GetLinkedItem(0)));
                SpriteHandler.RegisterSprite(GetTechType(aliasRecipe.ItemID), sprite);
            }
            else
            {
                QuickLogger.Warning($"No sprite loaded for '{aliasRecipe.ItemID}'");
            }
        }

        internal static void ModifyRecipe(IModifiedRecipe modifiedRecipe)
        {
            Assert.IsTrue(GetTechType(modifiedRecipe.ItemID) <= TechType.Databox, "This API in intended only for use with standard, non-modded TechTypes.");

            HandleModifiedRecipe(modifiedRecipe);

            HandleUnlocks(modifiedRecipe);
        }

        internal static void CustomizeItemSize(ICustomSize customSize)
        {
            Assert.IsTrue(GetTechType(customSize.ItemID) <= TechType.Databox, "This API in intended only for use with standard, non-modded TechTypes.");

            Assert.IsTrue(customSize.Width > 0 && customSize.Height > 0, "Values must be positive and non-zero");
            Assert.IsTrue(customSize.Width < 6 && customSize.Height < 6, "Values must be smaller than six to fit");
            // Value chosen for what should be the standard inventory size

            CraftDataHandler.SetItemSize(GetTechType(customSize.ItemID), customSize.Width, customSize.Height);
        }

        internal static void CustomizeBioFuel(ICustomBioFuel customBioFuel)
        {
            Assert.IsTrue(GetTechType(customBioFuel.ItemID) <= TechType.Databox, "This API in intended only for use with standard, non-modded TechTypes.");

            BioReactorHandler.SetBioReactorCharge(GetTechType(customBioFuel.ItemID), customBioFuel.Energy);
        }

        internal static void CustomCraftingTab(ICraftingTab craftingTab)
        {
            Assert.IsTrue(craftingTab.SpriteItemID <= TechType.Databox, "This API in intended only for use with standard, non-modded TechTypes.");
            Assert.IsTrue(craftingTab.FabricatorType <= CraftTree.Type.Rocket, "This API in intended only for use with standard, non-modded CraftTree.Types.");
            Assert.IsTrue(craftingTab.FabricatorType > CraftTree.Type.None, "ParentTabPath must identify a fabricator for the custom tab.");

            HandleCraftingTab(craftingTab);
        }

        // ----------------------

        private static void HandleCraftingTab(ICraftingTab craftingTab)
        {
            if (craftingTab.StepsToTab == null)
            {
                CraftTreeHandler.AddTabNode(craftingTab.FabricatorType, craftingTab.TabID, craftingTab.DisplayName, SpriteManager.Get(craftingTab.SpriteItemID));
            }
            else
            {
                CraftTreeHandler.AddTabNode(craftingTab.FabricatorType, craftingTab.TabID, craftingTab.DisplayName, SpriteManager.Get(craftingTab.SpriteItemID), craftingTab.StepsToTab);
            }
        }
        private static void HandleCraftTreeAddition(IAddedRecipe addedRecipe)
        {
            var craftPath = new CraftingPath(addedRecipe.Path);

            string[] steps = craftPath.Path.Split(CraftingNode.Splitter);

            if (steps.Length <= 1)
                CraftTreeHandler.AddCraftingNode(craftPath.Scheme, GetTechType(addedRecipe.ItemID));
            else
                CraftTreeHandler.AddCraftingNode(craftPath.Scheme, GetTechType(addedRecipe.ItemID), steps);
        }

        private static void HandleAddedRecipe(IAddedRecipe modifiedRecipe, short defaultCraftAmount = 1)
        {
            var replacement = new TechData
            {
                craftAmount = modifiedRecipe.AmountCrafted ?? defaultCraftAmount
            };

            foreach (EmIngredient ingredient in modifiedRecipe.Ingredients)
                replacement.Ingredients.Add(new Ingredient(GetTechType(ingredient.ItemID), ingredient.Required));

            foreach (string linkedItem in modifiedRecipe.LinkedItems)
                replacement.LinkedItems.Add(GetTechType(linkedItem));

            CraftDataHandler.SetTechData(GetTechType(modifiedRecipe.ItemID), replacement);
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
    }
}
