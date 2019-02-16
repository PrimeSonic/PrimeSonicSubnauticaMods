namespace CustomCraft2SML.Serialization.Entries
{
    using System.Collections.Generic;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization.Components;
    using SMLHelper.V2.Handlers;

    internal class MovedRecipe : EmTechTyped, IMovedRecipe
    {
        internal static readonly string[] TutorialText = new[]
        {
            "MovedRecipe: Further customize the crafting tree to your liking.",
            "    OldPath: First locate the crafting node you want to change.",
            "        NewPath: Set this property to move the recipe to a new location. It could even be a different crafting tree.",
            "        Hidden: Or you can set this property to 'YES' to simply remove the crafting node instead.",
            "    Reorganize, rebalance, or both!",
        };

        private readonly EmProperty<string> oldPath;
        private readonly EmProperty<string> newPath;
        private readonly EmYesNo hidden;

        protected static List<EmProperty> MovedRecipeProperties => new List<EmProperty>(TechTypedProperties)
        {
            new EmProperty<string>("ItemID"),
            new EmProperty<string>("OldPath"),
            new EmProperty<string>("NewPath"),
            new EmYesNo("Hidden"){ Optional = true }
        };

        public MovedRecipe() : this("MovedRecipe", MovedRecipeProperties)
        {
        }

        protected MovedRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            oldPath = (EmProperty<string>)Properties["OldPath"];
            newPath = (EmProperty<string>)Properties["NewPath"];
            hidden = (EmYesNo)Properties["Hidden"];
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
                QuickLogger.Warning($"OldPath missing in MovedRecipe for '{this.ItemID}'");
                return false;
            }

            if (!this.Hidden && string.IsNullOrEmpty(this.NewPath))
            {
                QuickLogger.Warning($"NewPath missing in MovedRecipe for '{this.ItemID}' while not set as 'Hidden'");
                return false;
            }

            return true;
        }

        public bool SendToSMLHelper()
        {
            var oldPath = new CraftingPath(this.OldPath);
            string[] oldSteps = (oldPath.Path + CraftingNode.Splitter + this.ItemID).Split(CraftingNode.Splitter);

            CraftTreeHandler.RemoveNode(oldPath.Scheme, oldSteps);
            QuickLogger.Message($"Recipe for '{this.ItemID}' was removed from the {oldPath.Scheme} crafting tree");

            if (this.Hidden)
            {
                return true;
            }

            var newPath = new CraftingPath(this.NewPath);

            AddCraftNode(newPath, this.TechType);

            return true;
        }
    }
}
