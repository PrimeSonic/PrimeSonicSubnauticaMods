namespace CustomCraft2SML.PublicAPI
{
    using Common;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Serialization;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine.Assertions;

    public static class CustomCraft
    {
        public static bool AddEntry<T>(T entry)
            where T : ITechTyped
        {
            switch (entry)
            {
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
            Assert.IsTrue(addedRecipe.ItemID <= TechType.Databox, "This API in intended only for use with standard, non-modded TechTypes.");

            HandleAddedRecipe(addedRecipe);

            HandleCraftTreeAddition(addedRecipe);

            HandleUnlocks(addedRecipe);
        }

        internal static void ModifyRecipe(IModifiedRecipe modifiedRecipe)
        {
            Assert.IsTrue(modifiedRecipe.ItemID <= TechType.Databox, "This API in intended only for use with standard, non-modded TechTypes.");

            HandleModifiedRecipe(modifiedRecipe);

            HandleUnlocks(modifiedRecipe);
        }

        internal static void CustomizeItemSize(ICustomSize customSize)
        {
            Assert.IsTrue(customSize.ItemID <= TechType.Databox, "This API in intended only for use with standard, non-modded TechTypes.");

            Assert.IsTrue(customSize.Width > 0 && customSize.Height > 0, "Values must be positive and non-zero");
            Assert.IsTrue(customSize.Width < 6 && customSize.Height < 6, "Values must be smaller than six to fit");
            // Value chosen for what should be the standard inventory size

            CraftDataHandler.SetItemSize(customSize.ItemID, customSize.Width, customSize.Height);
        }

        internal static void CustomizeBioFuel(ICustomBioFuel customBioFuel)
        {
            Assert.IsTrue(customBioFuel.ItemID <= TechType.Databox, "This API in intended only for use with standard, non-modded TechTypes.");

            BioReactorHandler.SetBioReactorCharge(customBioFuel.ItemID, customBioFuel.Energy);
        }

        // ----------------------

        private static void HandleCraftingTab(ICraftingTab craftingTab)
        {
            Assert.IsTrue(craftingTab.ItemForSprite <= TechType.Databox, "This API in intended only for use with standard, non-modded TechTypes.");
            Assert.IsTrue(craftingTab.FabricatorType <= CraftTree.Type.Rocket, "This API in intended only for use with standard, non-modded CraftTree.Types.");

            CraftTreeHandler.AddTabNode(craftingTab.FabricatorType, craftingTab.TabID, craftingTab.DisplayName, SpriteManager.Get(craftingTab.ItemForSprite));
        }

        private static void HandleCraftTreeAddition(IAddedRecipe addedRecipe)
        {
            var craftPath = new CraftingPath(addedRecipe.Path);

            string[] steps = craftPath.Path.Split(CraftingNode.Splitter);

            if (steps.Length <= 1)
                CraftTreeHandler.AddCraftingNode(craftPath.Scheme, addedRecipe.ItemID);
            else
                CraftTreeHandler.AddCraftingNode(craftPath.Scheme, addedRecipe.ItemID, steps);
        }

        private static void HandleAddedRecipe(IAddedRecipe modifiedRecipe)
        {
            var replacement = new TechData
            {
                craftAmount = modifiedRecipe.AmountCrafted ?? 1
            };

            foreach (EmIngredient ingredient in modifiedRecipe.Ingredients)
                replacement.Ingredients.Add(new Ingredient(ingredient.ItemID, ingredient.Required));

            foreach (TechType linkedItem in modifiedRecipe.LinkedItems)
                replacement.LinkedItems.Add(linkedItem);

            CraftDataHandler.SetTechData(modifiedRecipe.ItemID, replacement);
        }

        private static void HandleModifiedRecipe(IModifiedRecipe modifiedRecipe)
        {
            bool overrideRecipe = false;

            ITechData original = CraftData.Get(modifiedRecipe.ItemID);

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
                            ingredient.ItemID,
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
                foreach (TechType linkedItem in modifiedRecipe.LinkedItems)
                    replacement.LinkedItems.Add(linkedItem);
            }
            else
            {
                for (int i = 0; i < original.linkedItemCount; i++)
                    replacement.LinkedItems.Add(original.GetLinkedItem(i));
            }

            if (overrideRecipe)
                CraftDataHandler.SetTechData(modifiedRecipe.ItemID, replacement);
        }

        private static void HandleUnlocks(IModifiedRecipe modifiedRecipe)
        {
            if (modifiedRecipe.ForceUnlockAtStart)
                KnownTechHandler.UnlockOnStart(modifiedRecipe.ItemID);

            if (modifiedRecipe.UnlocksCount.HasValue)
                KnownTechHandler.SetAnalysisTechEntry(modifiedRecipe.ItemID, modifiedRecipe.Unlocks);
        }
    }
}
