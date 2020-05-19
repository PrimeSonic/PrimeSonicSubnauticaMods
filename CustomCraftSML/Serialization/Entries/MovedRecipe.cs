namespace CustomCraft2SML.Serialization.Entries
{
    using System.Collections.Generic;
    using Common;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Interfaces.InternalUse;
    using CustomCraft2SML.Serialization;
    using CustomCraft2SML.Serialization.Components;
    using CustomCraft2SML.Serialization.Lists;
    using EasyMarkup;
    using SMLHelper.V2.Handlers;

    internal class MovedRecipe : EmTechTyped, IMovedRecipe, ICustomCraft
    {
        private const string OldPathKey = "OldPath";
        private const string NewPathKey = "NewPath";
        private const string HiddenKey = "Hidden";
        private const string CopyKey = "Copied";

        public const string TypeName = "MovedRecipe";

        public string[] TutorialText => MovedRecipeTutorial;

        internal static readonly string[] MovedRecipeTutorial = new[]
        {
           $"{MovedRecipeList.ListKey}: Further customize the crafting tree to your liking.",
           $"    All moved recipes work with existing crafting nodes. So all moved recipe entries should be for items that can already be crafted.",
           $"    {OldPathKey}: If you want to move a craft node from its original location, this must be set.",
           $"    {OldPathKey}: If you want to move a craft node from its original location, this must be set.",
           $"        This node is optional if {CopyKey} is set to 'YES'.",
           $"        This node must be present if {HiddenKey} is set to 'YES'.",
           $"        This cannot be used to access paths in modded or custom fabricators.",
           $"    {NewPathKey}: If you want to move or copy the recipe to a new location, set the path here. It could even be a different (non-custom) crafting tree.",
           $"        This node is optional if {HiddenKey} is set to 'YES'.",
           $"        This node must be present if {CopyKey} is set to 'YES'.",
           $"        if neither {CopyKey} or {HiddenKey} are present, then the recipe will be removed from the {OldPathKey} and added to the {NewPathKey}.",
           $"    {CopyKey}: If you want, you can copy the recipe to the new path without removing the original by setting this to 'YES'.",
           $"        This node is optional and will default to 'NO' when not present.",
           $"        {CopyKey} cannot be set to 'YES' if {HiddenKey} is also set to 'YES'.",
           $"        Moved recipes will be handled after all other crafts, so if you added a new recipe, you can copy it to more than one fabricator.",
           $"    {HiddenKey}: Or you can set this property to 'YES' to simply remove the crafting node instead.",
           $"        This node is optional and will default to 'NO' when not present.",
           $"        {HiddenKey} cannot be set to 'YES' if {CopyKey} is also set to 'YES'.",
            "    Remember, all paths must be valid and point to existing tab or to a custom tab you've created.",
            "    You can find a full list of all original crafting paths for all the standard fabricators in the OriginalRecipes folder.",
        };

        private readonly EmProperty<string> oldPath;
        private readonly EmProperty<string> newPath;
        private readonly EmYesNo hidden;
        private readonly EmYesNo copied;

        protected static List<EmProperty> MovedRecipeProperties => new List<EmProperty>(TechTypedProperties)
        {
            new EmProperty<string>(OldPathKey){ Optional = true },
            new EmProperty<string>(NewPathKey){ Optional = true },
            new EmYesNo(HiddenKey, false){ Optional = true },
            new EmYesNo(CopyKey, false){ Optional = true }
        };

        public OriginFile Origin { get; set; }

        public bool PassedSecondValidation { get; set; } = true;

        public MovedRecipe() : this(TypeName, MovedRecipeProperties)
        {
        }

        protected MovedRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            oldPath = (EmProperty<string>)Properties[OldPathKey];
            newPath = (EmProperty<string>)Properties[NewPathKey];
            hidden = (EmYesNo)Properties[HiddenKey];
            copied = (EmYesNo)Properties[CopyKey];
        }

        public string OldPath
        {
            get => oldPath.Value;
            set => oldPath.Value = value;
        }

        public string NewPath
        {
            get => newPath.Value;
            set => newPath.Value = value;
        }

        public bool Hidden
        {
            get => hidden.Value;
            set => hidden.Value = value;
        }

        public bool Copied
        {
            get => copied.Value;
            set => copied.Value = value;
        }

        public string ID => this.ItemID;

        internal override EmProperty Copy()
        {
            return new MovedRecipe(this.Key, this.CopyDefinitions);
        }

        public override bool PassesPreValidation(OriginFile originFile)
        {
            return base.PassesPreValidation(originFile) & IsValidState();
        }

        private bool IsValidState()
        {
            if (!this.Copied && string.IsNullOrEmpty(this.OldPath))
            {
                QuickLogger.Warning($"{OldPathKey} missing while {CopyKey} was not set to 'YES' in {this.Key} for '{this.ItemID}' from {this.Origin}");
                return false;
            }

            if (this.Copied && this.Hidden)
            {
                QuickLogger.Warning($"Invalid request in {this.Key} for '{this.ItemID}' from {this.Origin}. {CopyKey} and {HiddenKey} cannot both be set to 'YES'");
                return false;
            }

            if (string.IsNullOrEmpty(this.NewPath) && (this.Copied || !this.Hidden))
            {
                QuickLogger.Warning($"{NewPathKey} value missing in {this.Key} for '{this.ItemID}' from {this.Origin}");
                return false;
            }

            return true;
        }

        public bool SendToSMLHelper()
        {
            if (this.Hidden || !this.Copied)
            {
                var oldPath = new CraftTreePath(this.OldPath, this.ItemID);

                CraftTreeHandler.RemoveNode(oldPath.Scheme, oldPath.StepsToNode);
                QuickLogger.Debug($"Removed crafting node at '{this.ItemID}' - Entry from {this.Origin}");
            }

            if (this.Hidden)
            {
                return true;
            }

            HandleCraftTreeAddition();

            return true;
        }

        protected virtual void HandleCraftTreeAddition()
        {
            var newPath = new CraftTreePath(this.NewPath, this.ItemID);

            if (newPath.HasError)
                QuickLogger.Error($"Encountered error in path for '{this.ItemID}' - Entry from {this.Origin} - Error Message: {newPath.Error}");
            else
                AddCraftNode(newPath, this.TechType);
        }
    }
}
