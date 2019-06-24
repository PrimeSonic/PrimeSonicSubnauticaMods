namespace CommonCyclopsUpgrades.Options
{
    internal abstract class ConfigOption
    {
        protected ConfigOption(OptionTypes optionType, string id, string label)
        {
            this.OptionType = optionType;
            this.Id = id;
            this.Label = label;
        }

        public OptionTypes OptionType { get; }
        public string Id { get; }
        public string Label { get; }
    }
}
