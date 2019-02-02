namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    internal class AliasRecipe : AddedRecipe, IAliasRecipe
    {
        private readonly EmProperty<string> displayName;
        private readonly EmProperty<string> tooltip;
        private readonly EmProperty<string> functionalID;

        public string DisplayName
        {
            get => displayName.Value;
            set => displayName.Value = value;
        }

        public string Tooltip
        {
            get => tooltip.Value;
            set => tooltip.Value = value;
        }

        public string FunctionalID
        {
            get => functionalID.Value;
            set => functionalID.Value = value;
        }

        protected static List<EmProperty> AliasRecipeProperties => new List<EmProperty>(AddedRecipeProperties)
        {
            new EmProperty<string>("DisplayName"),
            new EmProperty<string>("Tooltip"),
            new EmProperty<string>("FunctionalID"),
        };

        public AliasRecipe() : this("AliasRecipe", AliasRecipeProperties)
        {
        }

        public AliasRecipe(string key) : this(key, AliasRecipeProperties)
        {
        }

        protected AliasRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            displayName = (EmProperty<string>)Properties["DisplayName"];
            tooltip = (EmProperty<string>)Properties["Tooltip"];
            functionalID = (EmProperty<string>)Properties["FunctionalID"];
        }

        internal override EmProperty Copy() => new AliasRecipe(this.Key, this.CopyDefinitions);
    }
}
