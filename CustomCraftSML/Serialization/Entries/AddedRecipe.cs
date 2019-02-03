namespace CustomCraft2SML.Serialization.Entries
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    internal class AddedRecipe : ModifiedRecipe, IAddedRecipe
    {
        internal new static readonly string[] TutorialText = new[]
        {
            "AddedRecipe: Adding your own recipes into any of the existing fabricators.",
            "    Added recipes have all the same properties as Modified recipes, with the following additions:",
            "    Path: Sets the fabricator and crafting tab where the new recipe will be added.",
            "    PdaGroup: Sets the main group for blueprint shown in the PDA.",
            "    PdaCategory: Sets the category under the group for blueprint shown in the PDA."
        };

        private readonly EmProperty<string> path;
        private readonly EmProperty<TechGroup> techGroup;
        private readonly EmProperty<TechCategory> techCategory;

        public string Path
        {
            get => path.Value;
            set => path.Value = value;
        }

        protected static List<EmProperty> AddedRecipeProperties => new List<EmProperty>(ModifiedRecipeProperties)
        {
            new EmProperty<string>("Path"),
            new EmProperty<TechGroup>("PdaGroup"),
            new EmProperty<TechCategory>("PdaCategory")
        };

        public TechGroup PdaGroup
        {
            get => techGroup.Value;
            set => techGroup.Value = value;
        }

        public TechCategory PdaCategory
        {
            get => techCategory.Value;
            set => techCategory.Value = value;
        }

        public AddedRecipe() : this("AddedRecipe", AddedRecipeProperties)
        {
        }

        protected AddedRecipe(string key) : this(key, AddedRecipeProperties)
        {
        }

        protected AddedRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            path = (EmProperty<string>)Properties["Path"];
            techGroup = (EmProperty<TechGroup>)Properties["PdaGroup"];
            techCategory = (EmProperty<TechCategory>)Properties["PdaCategory"];
            DefaultForceUnlock = true;
            PdaGroup = TechGroup.Uncategorized;
            PdaCategory = TechCategory.Misc;

        }

        internal override EmProperty Copy() => new AddedRecipe(this.Key, this.CopyDefinitions);

    }
}
