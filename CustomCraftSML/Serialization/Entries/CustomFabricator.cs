namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Fabricators;
    using CustomCraft2SML.Interfaces;

    internal enum ModelTypes : byte
    {
        Fabricator = (byte)'F',
        Workbench = (byte)'W',
        MoonPool = (byte)'M',
    }

    internal class CustomFabricator : AliasRecipe, ICustomFabricator
    {
        protected const string ModelKey = "Model";
        protected const string HueOffsetKey = "Color";

        protected readonly EmProperty<ModelTypes> model;
        protected readonly EmProperty<int> hueOffset;

        protected static List<EmProperty> CustomFabricatorProperties => new List<EmProperty>(AliasRecipeProperties)
        {
            new EmProperty<ModelTypes>(ModelKey, ModelTypes.Fabricator),
            new EmProperty<int>(HueOffsetKey, 0),
        };

        public CustomFabricator() : this("CustomFabricator", CustomFabricatorProperties)
        {
        }

        protected CustomFabricator(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            model = (EmProperty<ModelTypes>)Properties[ModelKey];
            hueOffset = (EmProperty<int>)Properties[HueOffsetKey];

            (Properties[PathKey] as EmProperty<string>).Optional = true;
        }

        public ModelTypes Model
        {
            get => model.Value;
            set => model.Value = value;
        }

        public int HueOffset
        {
            get => hueOffset.Value;
            set => hueOffset.Value = value;
        }

        protected CustomFabricatorBuildable BuildableFabricator { get; set; }

        public override bool PassesPreValidation() => base.PassesPreValidation() & ValidFabricatorValues();

        private bool ValidFabricatorValues()
        {
            switch (this.Model)
            {
                case ModelTypes.Fabricator:
                case ModelTypes.Workbench:
                case ModelTypes.MoonPool:
                    this.BuildableFabricator = new CustomFabricatorBuildable(this);
                    break;
                default:
                    QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' contained an invalue {ModelKey} value. Entry will be removed. Accepted values are only: {ModelTypes.Fabricator}|{ModelTypes.Workbench}|{ModelTypes.MoonPool}");
                    return false;
            }

            return true;
        }

        public override bool SendToSMLHelper()
        {
            if (this.BuildableFabricator != null)
            {
                try
                {
                    this.BuildableFabricator.Patch();
                    return true;
                }
                catch (Exception ex)
                {
                    QuickLogger.Error($"Exception thrown while handling {this.Key} entry '{this.ItemID}'", ex);
                    return false;
                }

            }

            return false;
        }
    }
}
