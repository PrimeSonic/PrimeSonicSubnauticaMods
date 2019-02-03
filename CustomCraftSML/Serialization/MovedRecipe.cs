namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    internal class MovedRecipe : EmPropertyCollection, IMovedRecipe
    {
        internal const string TutorialText = "MovedRecipe: Move an existing recipe in the crafting tree from one location to another. Great for reorganizing.";

        protected readonly EmProperty<string> emTechType;
        private readonly EmProperty<string> oldPath;
        private readonly EmProperty<string> newPath;

        protected static List<EmProperty> MovedRecipeProperties => new List<EmProperty>()
        {
            new EmProperty<string>("ItemID"),
            new EmProperty<string>("OldPath"),
            new EmProperty<string>("NewPath")
        };

        public MovedRecipe() : this(MovedRecipeProperties)
        {
        }

        protected MovedRecipe(ICollection<EmProperty> definitions) : base("MovedRecipe", definitions)
        {
            oldPath = (EmProperty<string>)Properties["OldPath"];
            newPath = (EmProperty<string>)Properties["NewPath"];
        }

        public string ItemID
        {
            get => emTechType.Value;
            set => emTechType.Value = value;
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

        public bool IsComplete => !string.IsNullOrEmpty(OldPath) && !string.IsNullOrEmpty(NewPath);

        internal override EmProperty Copy() => new MovedRecipe(this.CopyDefinitions);
    }
}
