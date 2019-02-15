namespace CustomCraft2SML.Serialization.Entries
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    internal class MovedRecipe : EmPropertyCollection, IMovedRecipe
    {

        internal static readonly string[] TutorialText = new[]
        {
            "MovedRecipe: Further customize the crafting tree to your liking.",
            "    OldPath: First locate the crafting node you want to change.",
            "        NewPath: Set this property to move the recipe to a new location. It could even be a different crafting tree.",
            "        Hidden: Or you can set this property to 'YES' to simply remove the crafting node instead.",
            "    Reorganize, rebalance, or both!",
        };

        protected readonly EmProperty<string> emTechType;
        private readonly EmProperty<string> oldPath;
        private readonly EmProperty<string> newPath;
        private readonly EmYesNo hidden;

        protected static List<EmProperty> MovedRecipeProperties => new List<EmProperty>()
        {
            new EmProperty<string>("ItemID"),
            new EmProperty<string>("OldPath"),
            new EmProperty<string>("NewPath"),
            new EmYesNo("Hidden"){ Optional = true }
        };

        public MovedRecipe() : this(MovedRecipeProperties)
        {
        }

        protected MovedRecipe(ICollection<EmProperty> definitions) : base("MovedRecipe", definitions)
        {
            emTechType = (EmProperty<string>)Properties["ItemID"];
            oldPath = (EmProperty<string>)Properties["OldPath"];
            newPath = (EmProperty<string>)Properties["NewPath"];
            hidden = (EmYesNo)Properties["Hidden"];
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

        public bool Hidden
        {
            get => hidden.Value;
            set => hidden.Value = value;
        }

        internal override EmProperty Copy() => new MovedRecipe(this.CopyDefinitions);
    }
}
