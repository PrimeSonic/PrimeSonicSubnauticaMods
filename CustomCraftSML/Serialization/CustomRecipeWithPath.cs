namespace CustomCraftSML.Serialization
{
    using System.Collections.Generic;
    using EasyMarkup;

    public class CustomRecipeWithPath : CustomRecipe
    {
        private readonly EmProperty<string> path;

        public string Path => path.Value;

        protected static List<EmProperty> RecipeWithPathProperties => new List<EmProperty>(RecipeProperties)
        {          
            new EmProperty<string>("Path")
        };

        public CustomRecipeWithPath() : base("AddedRecipe", RecipeWithPathProperties)
        {
            path = (EmProperty<string>)Properties["Path"];
        }
    }
}
