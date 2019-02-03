namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    internal class AddedRecipe : ModifiedRecipe, IAddedRecipe
    {
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
