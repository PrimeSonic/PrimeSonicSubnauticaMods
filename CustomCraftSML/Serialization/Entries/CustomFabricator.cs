﻿namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Fabricators;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Serialization.Lists;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;

    internal enum ModelTypes
    {
        Fabricator,
        Workbench,
        MoonPool,
    }

    internal class CustomFabricator : AliasRecipe, ICustomFabricator<CfCustomCraftingTab, CfMovedRecipe, CfAddedRecipe, CfAliasRecipe>
    {
        protected const string ModelKey = "Model";
        protected const string HueOffsetKey = "Color";
        protected const string AllowedInBaseKey = "AllowedInBase";
        protected const string AllowedInCyclopsKey = "AllowedInCyclops";
        protected const string CfCustomCraftingTabListKey = CustomCraftingTabList.ListKey;
        protected const string CfAliasRecipeListKey = AliasRecipeList.ListKey;
        protected const string CfAddedRecipeListKey = AddedRecipeList.ListKey;
        protected const string CfMovedRecipeListKey = MovedRecipeList.ListKey;

        internal static new readonly string[] TutorialText = new[]
        {
            "TODO"
        };

        protected readonly EmProperty<ModelTypes> model;
        protected readonly EmProperty<int> hueOffset;

        protected readonly EmYesNo allowedInBase;
        protected readonly EmYesNo allowedInCyclops;

        protected static List<EmProperty> CustomFabricatorProperties => new List<EmProperty>(AliasRecipeProperties)
        {
            new EmProperty<ModelTypes>(ModelKey, ModelTypes.Fabricator),
            new EmProperty<int>(HueOffsetKey, 0) { Optional = true },
            new EmYesNo(AllowedInBaseKey, true) { Optional = true },
            new EmYesNo(AllowedInCyclopsKey, true) { Optional = true },
            new EmPropertyCollectionList<CfCustomCraftingTab>(CfCustomCraftingTabListKey) { Optional = true },
            new EmPropertyCollectionList<CfAddedRecipe>(CfAddedRecipeListKey) { Optional = true },
            new EmPropertyCollectionList<CfAddedRecipe>(CfAddedRecipeListKey) { Optional = true },
            new EmPropertyCollectionList<CfMovedRecipe>(CfMovedRecipeListKey) { Optional = true },
        };

        public CustomFabricator() : this("CustomFabricator", CustomFabricatorProperties)
        {
        }

        protected CustomFabricator(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            model = (EmProperty<ModelTypes>)Properties[ModelKey];
            hueOffset = (EmProperty<int>)Properties[HueOffsetKey];
            allowedInBase = (EmYesNo)Properties[AllowedInBaseKey];
            allowedInCyclops = (EmYesNo)Properties[AllowedInCyclopsKey];
            this.CustomCraftingTabs = (EmPropertyCollectionList<CfCustomCraftingTab>)Properties[CfCustomCraftingTabListKey];
            this.AliasRecipes = (EmPropertyCollectionList<CfAliasRecipe>)Properties[CfAliasRecipeListKey];
            this.AddedRecipes = (EmPropertyCollectionList<CfAddedRecipe>)Properties[CfAddedRecipeListKey];
            this.MovedRecipes = (EmPropertyCollectionList<CfMovedRecipe>)Properties[CfMovedRecipeListKey];

            (Properties[PathKey] as EmProperty<string>).Optional = true;
        }

        public ModelTypes Model
        {
            get => model.Value;
            set => model.Value = value;
        }

        public int HueOffset
        {
            get => hueOffset.Value;
            set => hueOffset.Value = value;
        }

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

        internal CustomFabricatorBuildable BuildableFabricator { get; set; }

        public EmPropertyCollectionList<CfCustomCraftingTab> CustomCraftingTabs { get; private set; }
        public EmPropertyCollectionList<CfMovedRecipe> MovedRecipes { get; private set; }
        public EmPropertyCollectionList<CfAddedRecipe> AddedRecipes { get; private set; }
        public EmPropertyCollectionList<CfAliasRecipe> AliasRecipes { get; private set; }

        internal IDictionary<string, CfCustomCraftingTab> UniqueCustomTabs { get; } = new Dictionary<string, CfCustomCraftingTab>();
        internal IDictionary<string, CfMovedRecipe> UniqueMovedRecipes { get; } = new Dictionary<string, CfMovedRecipe>();
        internal IDictionary<string, CfAddedRecipe> UniqueAddedRecipes { get; } = new Dictionary<string, CfAddedRecipe>();
        internal IDictionary<string, CfAliasRecipe> UniqueAliasRecipes { get; } = new Dictionary<string, CfAliasRecipe>();

        public string ListKey { get; }

        public CraftTree.Type TreeTypeID { get; set; }

        public ModCraftTreeRoot RootNode { get; set; }

        public override bool PassesPreValidation() => base.PassesPreValidation() & ValidFabricatorValues() & ValidateInternalEntries();

        private bool ValidFabricatorValues()
        {
            switch (this.Model)
            {
                case ModelTypes.Fabricator:
                case ModelTypes.Workbench:
                case ModelTypes.MoonPool:
                    this.BuildableFabricator = new CustomFabricatorBuildable(this);
                    break;
                default:
                    QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' contained an invalue {ModelKey} value. Entry will be removed. Accepted values are only: {ModelTypes.Fabricator}|{ModelTypes.Workbench}|{ModelTypes.MoonPool}");
                    return false;
            }

            return true;
        }

        private bool ValidateInternalEntries()
        {
            ValidateUniqueEntries(this.CustomCraftingTabs, this.UniqueCustomTabs);
            ValidateUniqueEntries(this.MovedRecipes, this.UniqueMovedRecipes);
            ValidateUniqueEntries(this.AddedRecipes, this.UniqueAddedRecipes);
            ValidateUniqueEntries(this.AliasRecipes, this.UniqueAliasRecipes);

            return true;
        }

        protected override bool FunctionalItemIsValid()
        {
            if (!string.IsNullOrEmpty(this.FunctionalID))
            {
                this.FunctionalID = string.Empty;
                QuickLogger.Warning($"{FunctionalIdKey} is not valid for {this.Key} entries. Was detected on '{this.ItemID}'. Please remove and try again.");
                return false;
            }

            return true;
        }

        public override bool SendToSMLHelper()
        {
            if (this.BuildableFabricator != null)
            {
                try
                {
                    this.BuildableFabricator.Patch();

                    return true;
                }
                catch (Exception ex)
                {
                    QuickLogger.Error($"Exception thrown while handling {this.Key} entry '{this.ItemID}'", ex);
                    return false;
                }

            }

            return false;
        }

        internal void StartCustomCraftingTree()
        {
            this.RootNode = CraftTreeHandler.CreateCustomCraftTreeAndType(this.ItemID, out CraftTree.Type craftType);
            this.TreeTypeID = craftType;
        }

        internal void FinishCustomCraftingTree()
        {
            SendToSMLHelper(this.UniqueCustomTabs);
            SendToSMLHelper(this.UniqueMovedRecipes);
            SendToSMLHelper(this.UniqueAddedRecipes);
            SendToSMLHelper(this.UniqueAliasRecipes);
        }

        internal void HandleCraftTreeAddition<CraftingNode>(CraftingNode entry)
            where CraftingNode : ICustomFabricatorEntry, ITechTyped
        {
            try
            {
                if (entry.IsAtRoot)
                {
                    this.RootNode.AddCraftingNode(entry.TechType);
                }
                else
                {
                    ModCraftTreeTab otherTab = this.RootNode.GetTabNode(entry.CraftingNodePath.Steps);
                    otherTab.AddCraftingNode(entry.TechType);
                }
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {entry.Key} '{entry.ItemID}'", ex);

            }
        }

        internal void ValidateUniqueEntries<CustomCraftEntry>(EmPropertyCollectionList<CustomCraftEntry> collectionList, IDictionary<string, CustomCraftEntry> uniqueEntries)
            where CustomCraftEntry : EmPropertyCollection, ICustomCraft, ICustomFabricatorEntry, new()
        {
            foreach (CustomCraftEntry entry in collectionList)
            {
                entry.ParentFabricator = this;
                if (!entry.PassesPreValidation())
                    continue;

                if (uniqueEntries.ContainsKey(entry.ID))
                {
                    QuickLogger.Warning($"Duplicate entry for {entry.Key} '{entry.ID}' was already added by another working file. Kept first one. Discarded duplicate.");
                }
                else
                {
                    // All checks passed
                    uniqueEntries.Add(entry.ID, entry);
                }
            }

            if (collectionList.Count > 0)
                QuickLogger.Message($"{uniqueEntries.Count} of {collectionList.Count} {this.Key}:{typeof(CustomCraftEntry).Name} entries for {this.Key} staged for patching");
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
                QuickLogger.Message($"{successCount} of {uniqueEntries.Count} {typeof(CustomCraftEntry).Name} entries were patched");
        }
    }
}
