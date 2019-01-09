namespace CustomCraft2SML.Serialization
{
    using Common;
    using System;
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    internal class AliasRecipe : AddedRecipe, IAliasRecipe
    {
        private readonly EmProperty<string> displayName;
        private readonly EmProperty<string> tooltip;

        public string ItemName
        {
            get => emTechType.Value;
        }

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

        public override TechType ItemID
        {
            get
            {
                if (internalId == TechType.None)
                {
                    string tAlias = String.Copy(emTechType.Value);
                    if (tAlias.Length > 1)
                    {
                        internalId = SMLHelper.V2.Handlers.TechTypeHandler.AddTechType(tAlias, DisplayName, Tooltip);
                    }
                }
                return internalId;
            }
        }

        protected static List<EmProperty> AliasRecipeProperties => new List<EmProperty>(AddedRecipeProperties)
        {
            new EmProperty<string>("DisplayName"),
            new EmProperty<string>("Tooltip"),
        };

        public AliasRecipe() : this("AliasRecipe", AliasRecipeProperties)
        {
        }

        public AliasRecipe(string key) : this(key, AliasRecipeProperties)
        {
            internalId = TechType.None;
        }

        protected AliasRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            displayName = (EmProperty<string>)Properties["DisplayName"];
            tooltip = (EmProperty<string>)Properties["Tooltip"];
            internalId = TechType.None;
        }

        internal override EmProperty Copy() => new AliasRecipe(Key, CopyDefinitions);
    }
}
