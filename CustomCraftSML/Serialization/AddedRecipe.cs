namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;

    public class AddedRecipe : ModifiedRecipe, IAddedRecipe
    {
        private readonly EmProperty<string> path;

        public string Path
        {
            get => path.Value;
            set => path.Value = value;
        }

        protected static List<EmProperty> AddedRecipeProperties => new List<EmProperty>(ModifiedRecipeProperties)
        {          
            new EmProperty<string>("Path")
        };

        public AddedRecipe() : this("AddedRecipe", AddedRecipeProperties)
        {            
        }

        public AddedRecipe(string key) : this(key, AddedRecipeProperties)
        {
        }

        protected AddedRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            path = (EmProperty<string>)Properties["Path"];
            ForceUnlockAtStart = true;
        }

        protected override bool DefaultForceUnlock => true;

        internal override EmProperty Copy() => new AddedRecipe(Key, CopyDefinitions);

    }
}
