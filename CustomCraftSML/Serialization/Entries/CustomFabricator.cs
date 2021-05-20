namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Common;
    using CustomCraft2SML.Fabricators;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Interfaces.InternalUse;
    using CustomCraft2SML.Serialization.Components;
    using CustomCraft2SML.Serialization.Lists;
    using EasyMarkup;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;
    using IOPath = System.IO.Path;
#if SUBNAUTICA
    using Sprite = Atlas.Sprite;
#endif

    public enum ModelTypes
    {
        Fabricator,
        Workbench,
        MoonPool,
    }

    internal class CustomFabricator : AliasRecipe, ICustomFabricator<CfCustomCraftingTab, CfMovedRecipe, CfAddedRecipe, CfAliasRecipe, CfCustomFood>, IFabricatorEntries
    {
        protected const string ModelKey = "Model";
        protected const string ColorTintKey = "ColorTint";
        protected const string AllowedInBaseKey = "AllowedInBase";
        protected const string AllowedInCyclopsKey = "AllowedInCyclops";
        protected const string CfCustomCraftingTabListKey = CustomCraftingTabList.ListKey;
        protected const string CfAliasRecipeListKey = AliasRecipeList.ListKey;
        protected const string CfAddedRecipeListKey = AddedRecipeList.ListKey;
        protected const string CfMovedRecipeListKey = MovedRecipeList.ListKey;
        protected const string CfCustomFoodListKey = CustomFoodList.ListKey;

        public override string[] TutorialText => CustomFabricatorTutorial;

        internal static readonly string[] CustomFabricatorTutorial = new[]
        {
            $"{CustomFabricatorList.ListKey}: Create your own fabricator with your own completely custom crafting tree!",
            $"    Custom fabricators have all the same properties as {AliasRecipeList.ListKey} with the following additions.",
            $"    {ModelKey}: Choose from one of three visual styles for your fabricator.",
            $"        Valid options are: {ModelTypes.Fabricator}|{ModelTypes.MoonPool}|{ModelTypes.Workbench}",
            $"        This property is optional. Defaults to {ModelTypes.Fabricator}.",
            $"    {ColorTintKey}: This optional property lets you apply a color tint over your fabricator.",
            $"        This value is a list of floating point numbers.",
            $"        Use three numbers to set the value as RGB. Example: 1.0, 0.64, 0.0 makes an orange color.",
            $"        Use four numbers to set the value as RGBA (RGB with Alpha).",
            $"    {AllowedInBaseKey}: Determines if your fabricator can or can't be built inside a stationary base. ",
            $"        This property is optional. Defaults to YES.",
            $"    {AllowedInCyclopsKey}: Determines if your fabricator can or can't be built inside a Cyclops. ",
            $"        This property is optional. Defaults to YES.",
            $"    Everything to be added to the custom fabricator's crafting tree must be specified as a list inside the custom fabricator entry.",
            $"    Every entry will still need to specify a full path that includes the new fabricator as the starting point for the path.",
            $"    The lists you can add are as follows.",
            $"        {CfCustomCraftingTabListKey}: List of crafting tabs to be added to the custom fabricator.",
            $"        {CfAddedRecipeListKey}: List of added recipes for the custom fabricator.",
            $"        {CfAliasRecipeListKey}: List of alias recipes for the custom fabricator.",
            $"        {CfMovedRecipeListKey}: List of moved recipes for the custom fabricator.",
            $"        {CfCustomFoodListKey}: List of custom foods for the custom fabricator.",
        };

        protected readonly EmProperty<ModelTypes> model;
        protected readonly EmColorRGB colortint;
        protected readonly EmYesNo allowedInBase;
        protected readonly EmYesNo allowedInCyclops;

        protected static List<EmProperty> CustomFabricatorProperties => new List<EmProperty>(AliasRecipeProperties)
        {
            new EmProperty<ModelTypes>(ModelKey, ModelTypes.Fabricator),
            new EmColorRGB(ColorTintKey) { Optional = true },
            new EmYesNo(AllowedInBaseKey, true) { Optional = true },
            new EmYesNo(AllowedInCyclopsKey, true) { Optional = true },
            new EmPropertyCollectionList<CfCustomCraftingTab>(CfCustomCraftingTabListKey) { Optional = true },
            new EmPropertyCollectionList<CfMovedRecipe>(CfMovedRecipeListKey) { Optional = true },
            new EmPropertyCollectionList<CfAddedRecipe>(CfAddedRecipeListKey) { Optional = true },
            new EmPropertyCollectionList<CfAliasRecipe>(CfAliasRecipeListKey) { Optional = true },
            new EmPropertyCollectionList<CfCustomFood>(CfCustomFoodListKey) { Optional = true },
        };

        public CustomFabricator() : this("CustomFabricator", CustomFabricatorProperties)
        {
        }

        protected CustomFabricator(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            model = (EmProperty<ModelTypes>)Properties[ModelKey];
            colortint = (EmColorRGB)Properties[ColorTintKey];
            allowedInBase = (EmYesNo)Properties[AllowedInBaseKey];
            allowedInCyclops = (EmYesNo)Properties[AllowedInCyclopsKey];
            this.CustomCraftingTabs = (EmPropertyCollectionList<CfCustomCraftingTab>)Properties[CfCustomCraftingTabListKey];
            this.MovedRecipes = (EmPropertyCollectionList<CfMovedRecipe>)Properties[CfMovedRecipeListKey];
            this.AddedRecipes = (EmPropertyCollectionList<CfAddedRecipe>)Properties[CfAddedRecipeListKey];
            this.AliasRecipes = (EmPropertyCollectionList<CfAliasRecipe>)Properties[CfAliasRecipeListKey];
            this.CustomFoods = (EmPropertyCollectionList<CfCustomFood>)Properties[CfCustomFoodListKey];

            path.Optional = true;
        }

        public ModelTypes Model
        {
            get => model.Value;
            set => model.Value = value;
        }

        public Color ColorTint => colortint.GetColor();

        internal bool HasColorValue => colortint.HasValue;

        public bool AllowedInBase
        {
            get => allowedInBase.Value;
            set => allowedInBase.Value = value;
        }

        public bool AllowedInCyclops
        {
            get => allowedInCyclops.Value;
            set => allowedInCyclops.Value = value;
        }

        // Set in constructor
        public EmPropertyCollectionList<CfCustomCraftingTab> CustomCraftingTabs { get; }
        public EmPropertyCollectionList<CfMovedRecipe> MovedRecipes { get; }
        public EmPropertyCollectionList<CfAddedRecipe> AddedRecipes { get; }
        public EmPropertyCollectionList<CfAliasRecipe> AliasRecipes { get; }
        public EmPropertyCollectionList<CfCustomFood> CustomFoods { get; }

        private IDictionary<string, CfCustomCraftingTab> UniqueCustomTabs { get; } = new Dictionary<string, CfCustomCraftingTab>();
        private IDictionary<string, CfMovedRecipe> UniqueMovedRecipes { get; } = new Dictionary<string, CfMovedRecipe>();
        private IDictionary<string, CfAddedRecipe> UniqueAddedRecipes { get; } = new Dictionary<string, CfAddedRecipe>();
        private IDictionary<string, CfAliasRecipe> UniqueAliasRecipes { get; } = new Dictionary<string, CfAliasRecipe>();
        private IDictionary<string, CfCustomFood> UniqueCustomFoods { get; } = new Dictionary<string, CfCustomFood>();

        public string ListKey { get; }

        public CraftTree.Type TreeTypeID { get; private set; }

        public ModCraftTreeRoot RootNode { get; private set; }

        public ICollection<string> CustomTabIDs => this.UniqueCustomTabs.Keys;
        public ICollection<string> MovedRecipeIDs => this.UniqueMovedRecipes.Keys;
        public ICollection<string> AddedRecipeIDs => this.UniqueAddedRecipes.Keys;
        public ICollection<string> AliasRecipesIDs => this.UniqueAliasRecipes.Keys;
        public ICollection<string> CustomFoodIDs => this.UniqueCustomFoods.Keys;

        public override bool PassesPreValidation(OriginFile originFile)
        {
            return ItemIDisUnique() & InnerItemsAreValid() & FunctionalItemIsValid() & ValidFabricatorValues() & ValidateInternalEntries();
        }

        private bool ValidFabricatorValues()
        {
            switch (this.Model)
            {
                case ModelTypes.Fabricator:
                case ModelTypes.Workbench:
                case ModelTypes.MoonPool:
                    break;
                default:
                    QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} contained an invalue {ModelKey} value. Entry will be removed. Accepted values are only: {ModelTypes.Fabricator}|{ModelTypes.Workbench}|{ModelTypes.MoonPool}");
                    return false;
            }

            if (!this.AllowedInBase && this.AllowedInCyclops)
            {
                QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} is denied from being built anywhere as both {AllowedInBaseKey} and {AllowedInCyclopsKey} are set to NO. Entry will be removed.");
                return false;
            }

            return true;
        }

        private bool ValidateInternalEntries()
        {
            ValidateUniqueEntries(this.CustomCraftingTabs, this.UniqueCustomTabs);
            ValidateUniqueEntries(this.AddedRecipes, this.UniqueAddedRecipes);
            ValidateUniqueEntries(this.AliasRecipes, this.UniqueAliasRecipes);
            ValidateUniqueEntries(this.MovedRecipes, this.UniqueMovedRecipes);
            ValidateUniqueEntries(this.CustomFoods, this.UniqueCustomFoods);

            return true;
        }

        protected override bool FunctionalItemIsValid()
        {
            if (!string.IsNullOrEmpty(this.FunctionalID))
            {
                this.FunctionalID = string.Empty;
                QuickLogger.Warning($"{FunctionalIdKey} is not valid for {this.Key} entries. Was detected on '{this.ItemID}' from {this.Origin}. Please remove and try again.");
                return false;
            }

            return true;
        }

        internal void StartCustomCraftingTree()
        {
            this.RootNode = CraftTreeHandler.CreateCustomCraftTreeAndType(this.ItemID, out CraftTree.Type craftType);
            this.TreeTypeID = craftType;
            CraftTreePath.CraftTreeLookup[this.ItemID] = this.TreeTypeID;
        }

        internal void FinishCustomCraftingTree()
        {
            SendToSMLHelper(this.UniqueCustomTabs);
            SendToSMLHelper(this.UniqueAddedRecipes);
            SendToSMLHelper(this.UniqueAliasRecipes);
            SendToSMLHelper(this.UniqueMovedRecipes);
            SendToSMLHelper(this.UniqueCustomFoods);
        }

        internal void HandleCraftTreeAddition<CraftingNode>(CraftingNode entry)
            where CraftingNode : ICustomFabricatorEntry, ITechTyped
        {
            try
            {
                CraftTreePath craftingNodePath = entry.GetCraftTreePath();
                if (craftingNodePath.HasError)
                {
                    QuickLogger.Error($"Encountered error in path for '{this.ItemID}' - Entry from {this.Origin} - Error Message: {craftingNodePath.Error}");
                    return;
                }

                QuickLogger.Debug($"Sending {entry.Key} '{entry.ItemID}' to be added to the crafting tree at '{craftingNodePath.RawPath}'");
                if (entry.IsAtRoot)
                {
                    this.RootNode.AddCraftingNode(entry.TechType);
                }
                else
                {
                    ModCraftTreeTab otherTab = this.RootNode.GetTabNode(craftingNodePath.StepsToParentTab);
                    otherTab.AddCraftingNode(entry.TechType);
                }
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {entry.Key} '{entry.ItemID}' from {this.Origin}", ex);

            }
        }

        internal void ValidateUniqueEntries<CustomCraftEntry>(EmPropertyCollectionList<CustomCraftEntry> collectionList, IDictionary<string, CustomCraftEntry> uniqueEntries)
            where CustomCraftEntry : EmPropertyCollection, ICustomCraft, ICustomFabricatorEntry, new()
        {
            foreach (CustomCraftEntry entry in collectionList)
            {
                entry.ParentFabricator = this;
                entry.Origin = this.Origin;
                if (!entry.PassesPreValidation(entry.Origin))
                    continue;

                if (uniqueEntries.ContainsKey(entry.ID))
                {
                    QuickLogger.Warning($"Duplicate entry for {entry.Key} '{entry.ID}' from {this.Origin} was already added by another working file. Kept first one. Discarded duplicate.");
                }
                else
                {
                    // All checks passed
                    uniqueEntries.Add(entry.ID, entry);
                }
            }

            if (collectionList.Count > 0)
                QuickLogger.Info($"{uniqueEntries.Count} of {collectionList.Count} {this.Key}:{typeof(CustomCraftEntry).Name} entries for {this.Key} staged for patching");
        }

        internal void SendToSMLHelper<CustomCraftEntry>(IDictionary<string, CustomCraftEntry> uniqueEntries)
            where CustomCraftEntry : ICustomCraft
        {
            int successCount = 0;
            foreach (CustomCraftEntry item in uniqueEntries.Values)
            {
                if (item.SendToSMLHelper())
                    successCount++;
            }

            if (uniqueEntries.Count > 0)
                QuickLogger.Info($"{successCount} of {uniqueEntries.Count} {typeof(CustomCraftEntry).Name} entries were patched");
        }

        protected override void HandleCustomPrefab()
        {
            if (this.TechType == TechType.None)
                throw new InvalidOperationException("TechTypeHandler.AddTechType must be called before PrefabHandler.RegisterPrefab.");

            StartCustomCraftingTree();

            PrefabHandler.RegisterPrefab(new CustomFabricatorBuildable(this));

            FinishCustomCraftingTree();
        }

        protected override void HandleCustomSprite()
        {
            Sprite sprite;

            string imagePath = IOPath.Combine(FileLocations.AssetsFolder, $"{this.ItemID}.png");
            if (File.Exists(imagePath))
            {
                QuickLogger.Debug($"Custom sprite found in Assets folder for {this.Key} '{this.ItemID}' from {this.Origin}");
                sprite = ImageUtils.LoadSpriteFromFile(imagePath);
            }
            else
            {
                QuickLogger.Debug($"Default sprite for {this.Key} '{this.ItemID}' from {this.Origin}");
                switch (this.Model)
                {
                    case ModelTypes.Fabricator:
                        sprite = SpriteManager.Get(TechType.Fabricator);
                        break;
                    case ModelTypes.Workbench:
                        sprite = SpriteManager.Get(TechType.Workbench);
                        break;
                    case ModelTypes.MoonPool:
                        imagePath = IOPath.Combine(FileLocations.AssetsFolder, $"MoonPool.png");
                        sprite = ImageUtils.LoadSpriteFromFile(imagePath);
                        break;
                    default:
                        throw new InvalidOperationException("Invalid ModelType encountered in HandleCustomSprite");
                }

            }

            SpriteHandler.RegisterSprite(this.TechType, sprite);
        }

        protected override void HandleCraftTreeAddition()
        {
            return; // Buildables aren't part of a crafting tree
        }

        public void DuplicateCustomTabDiscovered(string id)
        {
            QuickLogger.Warning($"Duplicate entry for {CustomCraftingTabList.ListKey} '{id}' from {this.Origin} was already added by another working file. Kept first one. Discarded duplicate.");
            if (this.UniqueCustomTabs.TryGetValue(id, out CfCustomCraftingTab tab))
            {
                tab.PassedSecondValidation = false;
                this.PassedSecondValidation = false;
                this.UniqueCustomTabs.Remove(id);
            }
        }

        public void DuplicateMovedRecipeDiscovered(string id)
        {
            QuickLogger.Warning($"Duplicate entry for {MovedRecipeList.ListKey} '{id}' from {this.Origin} was already added by another working file. Kept first one. Discarded duplicate.");
            if (this.UniqueMovedRecipes.TryGetValue(id, out CfMovedRecipe moved))
            {
                moved.PassedSecondValidation = false;
                this.PassedSecondValidation = false;
                this.UniqueMovedRecipes.Remove(id);
            }
        }

        public void DuplicateAddedRecipeDiscovered(string id)
        {
            QuickLogger.Warning($"Duplicate entry for {AddedRecipeList.ListKey} '{id}' from {this.Origin} was already added by another working file. Kept first one. Discarded duplicate.");
            if (this.UniqueAddedRecipes.TryGetValue(id, out CfAddedRecipe added))
            {
                added.PassedSecondValidation = false;
                this.PassedSecondValidation = false;
                this.UniqueAddedRecipes.Remove(id);
            }
        }

        public void DuplicateAliasRecipesDiscovered(string id)
        {
            QuickLogger.Warning($"Duplicate entry for {AliasRecipeList.ListKey} '{id}' from {this.Origin} was already added by another working file. Kept first one. Discarded duplicate.");
            if (this.UniqueAliasRecipes.TryGetValue(id, out CfAliasRecipe alias))
            {
                alias.PassedSecondValidation = false;
                this.PassedSecondValidation = false;
                this.UniqueAliasRecipes.Remove(id);
            }
        }

        public void DuplicateCustomFoodsDiscovered(string id)
        {
            QuickLogger.Warning($"Duplicate entry for {CustomFoodList.ListKey} '{id}' from {this.Origin} was already added by another working file. Kept first one. Discarded duplicate.");
            this.UniqueCustomFoods.Remove(id);
            if (this.UniqueCustomFoods.TryGetValue(id, out CfCustomFood food))
            {
                food.PassedSecondValidation = false;
                this.PassedSecondValidation = false;
                this.UniqueCustomFoods.Remove(id);
            }
        }

        internal override EmProperty Copy()
        {
            return new CustomFabricator(this.Key, this.CopyDefinitions);
        }
    }
}
