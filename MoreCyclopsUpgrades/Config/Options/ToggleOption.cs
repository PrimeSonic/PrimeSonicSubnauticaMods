namespace MoreCyclopsUpgrades.Config.Options
{
    using EasyMarkup;

    internal class ToggleOption : ConfigOption
    {
        public delegate void ToggledEvent(bool state, ModConfig config);

        public ToggledEvent OptionToggled;
        public bool State;
        public EmProperty<bool> SaveData;

        public ToggleOption(string id, string label)
            : base(OptionTypes.Toggle, id, label)
        {
        }

        public override void LoadFromSaveData(ModConfigSaveData saveData)
        {
            SaveData = saveData.GetBoolProperty(this);
        }

        public override void UpdateProperty(ModConfig config)
        {
            LinkedProperty.SetValue(config, this.SaveData.Value, null);
        }
    }
}
