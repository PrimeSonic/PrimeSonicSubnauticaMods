namespace CustomCraft2SML.Serialization.Entries
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Serialization.Components;

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

        internal override EmProperty Copy() => new MovedRecipe(this.Key, this.CopyDefinitions);
    }
}
