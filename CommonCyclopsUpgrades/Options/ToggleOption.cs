namespace CommonCyclopsUpgrades.Options
{
    using Common.EasyMarkup;

    internal class ToggleOption : ConfigOption
    {
        public delegate void ToggledEvent(bool state);

        public ToggledEvent OptionToggled;
        public bool State;
        public EmProperty<bool> SaveData;

        public ToggleOption(string id, string label)
            : base(OptionTypes.Toggle, id, label)
        {
        }
    }
}
