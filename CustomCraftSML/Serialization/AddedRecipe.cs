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

        public AddedRecipe() : base("AddedRecipe", AddedRecipeProperties)
        {
            path = (EmProperty<string>)Properties["Path"];
        }
    }
}
