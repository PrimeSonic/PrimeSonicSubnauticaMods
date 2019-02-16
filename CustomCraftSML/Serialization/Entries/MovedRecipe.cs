namespace CustomCraft2SML.Serialization.Entries
{
    using System.Collections.Generic;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization.Components;
    using CustomCraft2SML.Serialization.Lists;
    using SMLHelper.V2.Handlers;

    internal class MovedRecipe : EmTechTyped, IMovedRecipe
    {
        private const string OldPathKey = "OldPath";
        private const string NewPathKey = "NewPath";
        private const string HiddenKey = "Hidden";
        internal static readonly string[] TutorialText = new[]
        {
           $"{MovedRecipeList.ListKey}: Further customize the crafting tree to your liking. Move a crafting node or get rid of it.",
           $"    {OldPathKey}: First locate the crafting node you want to change.",
           $"        {NewPathKey}: Set this property to move the recipe to a new location. It could even be a different crafting tree.",
           $"        {HiddenKey}: Or you can set this property to 'YES' to simply remove the crafting node instead.",
           $"    You should use either {NewPathKey} or {HiddenKey} but not both.",
        };

        private readonly EmProperty<string> oldPath;
        private readonly EmProperty<string> newPath;
        private readonly EmYesNo hidden;

        protected static List<EmProperty> MovedRecipeProperties => new List<EmProperty>(TechTypedProperties)
        {
            new EmProperty<string>(OldPathKey),
            new EmProperty<string>(NewPathKey),
            new EmYesNo(HiddenKey){ Optional = true }
        };

        public MovedRecipe() : this("MovedRecipe", MovedRecipeProperties)
        {
        }

        protected MovedRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            oldPath = (EmProperty<string>)Properties[OldPathKey];
            newPath = (EmProperty<string>)Properties[NewPathKey];
            hidden = (EmYesNo)Properties[HiddenKey];
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
        public string ID => this.ItemID;

        internal override EmProperty Copy() => new MovedRecipe(this.Key, this.CopyDefinitions);

        public override bool PassesPreValidation() => base.PassesPreValidation() && IsValidState();

        private bool IsValidState()
        {
            if (string.IsNullOrEmpty(this.OldPath))
            {
                QuickLogger.Warning($"{OldPathKey} missing in {this.Key} for '{this.ItemID}'");
                return false;
            }

            if (!this.Hidden && string.IsNullOrEmpty(this.NewPath))
            {
                QuickLogger.Warning($"{NewPathKey} or {HiddenKey} value missing or invalid in {this.Key} for '{this.ItemID}'");
                return false;
            }

            return true;
        }

        public bool SendToSMLHelper()
        {
            var oldPath = new CraftingPath(this.OldPath, this.ItemID);

            CraftTreeHandler.RemoveNode(oldPath.Scheme, oldPath.CraftNodeSteps);
            QuickLogger.Message($"Removed crafting node at '{this.ItemID}'");
            if (this.Hidden)
            {
                return true;
            }

            var newPath = new CraftingPath(this.NewPath, this.ItemID);

            AddCraftNode(newPath, this.TechType);

            return true;
        }
    }
}
