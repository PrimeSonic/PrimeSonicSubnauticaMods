namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Fabricators;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization.Lists;
    using SMLHelper.V2.Crafting;

    internal enum ModelTypes
    {
        Fabricator,
        Workbench,
        MoonPool,
    }

    internal class CustomFabricator : AliasRecipe, ICustomFabricator<CfCustomCraftingTab, CfAliasRecipe, CfAddedRecipe, CfMovedRecipe>
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
            CustomCraftingTabs = (EmPropertyCollectionList<CfCustomCraftingTab>)Properties[CfCustomCraftingTabListKey];
            AliasRecipes = (EmPropertyCollectionList<CfAliasRecipe>)Properties[CfAliasRecipeListKey];
            AddedRecipes = (EmPropertyCollectionList<CfAddedRecipe>)Properties[CfAddedRecipeListKey];
            MovedRecipes = (EmPropertyCollectionList<CfMovedRecipe>)Properties[CfMovedRecipeListKey];

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
        public EmPropertyCollectionList<CfAliasRecipe> AliasRecipes { get; private set; }
        public EmPropertyCollectionList<CfAddedRecipe> AddedRecipes { get; private set; }
        public EmPropertyCollectionList<CfMovedRecipe> MovedRecipes { get; private set; }

        public override bool PassesPreValidation() => base.PassesPreValidation() & ValidFabricatorValues() & AllInternalItemsValid();

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

        private bool AllInternalItemsValid()
        {
            bool internalItemsValid = true;

            foreach (CfCustomCraftingTab tab in this.CustomCraftingTabs)
            {
                tab.ParentFabricator = this;
                internalItemsValid &= tab.PassesPreValidation();
            }

            foreach (CfMovedRecipe move in this.MovedRecipes)
            {
                move.ParentFabricator = this;
                internalItemsValid &= move.PassesPreValidation();
            }

            foreach (CfAddedRecipe added in this.AddedRecipes)
            {
                added.ParentFabricator = this;
                internalItemsValid &= added.PassesPreValidation();
            }

            foreach (CfAliasRecipe alias in this.AliasRecipes)
            {
                alias.ParentFabricator = this;
                internalItemsValid &= alias.PassesPreValidation();
            }

            return internalItemsValid;
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

        internal void HandleCraftTreeAddition(ICustomFabCraftingNode entry)
        {
            try
            {
                if (entry.IsAtRoot)
                {
                    entry.RootNode.AddCraftingNode(entry.TechType);
                }
                else
                {
                    ModCraftTreeTab otherTab = entry.RootNode.GetTabNode(entry.CraftingNodePath.Steps);
                    otherTab.AddCraftingNode(entry.TechType);
                }
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {entry.Key} '{entry.ItemID}'", ex);

            }
        }
    }
}
