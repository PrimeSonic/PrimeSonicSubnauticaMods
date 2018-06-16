namespace CustomCraftSML.Serialization
{
    using System.Collections.Generic;
    using EasyMarkup;

    public class AddedRecipe : ModifiedRecipe
    {
        private readonly EmProperty<string> path;

        public string Path => path.Value;

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
        }

        internal override EmProperty Copy() => new AddedRecipe(Key, CopyDefinitions);
    }
}
