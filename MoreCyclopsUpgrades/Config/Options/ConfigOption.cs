namespace MoreCyclopsUpgrades.Config.Options
{
    using System.Reflection;

    internal abstract class ConfigOption
    {
        protected ConfigOption(OptionTypes optionType, string id, string label)
        {
            this.OptionType = optionType;
            this.Id = id;
            this.Label = label;
            LinkedProperty = typeof(ModConfig).GetProperty(id);
        }

        public OptionTypes OptionType { get; }
        public string Id { get; }
        public string Label { get; }

        public readonly PropertyInfo LinkedProperty;

        public abstract void LoadFromSaveData(ModConfigSaveData saveData);

        public abstract void UpdateProperty(ModConfig config);
    }
}
