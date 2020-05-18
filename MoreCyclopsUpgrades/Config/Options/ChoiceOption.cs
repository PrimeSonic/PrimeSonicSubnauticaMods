namespace MoreCyclopsUpgrades.Config.Options
{
    using EasyMarkup;

    internal class ChoiceOption : ConfigOption
    {
        public delegate void ChoiceEvent(int index, ModConfig config);

        public ChoiceEvent ChoiceChanged;
        public string[] Choices;
        public int Index;
        public EmProperty<int> SaveData;

        public ChoiceOption(string id, string label)
            : base(OptionTypes.Choice, id, label)
        {
        }

        public override void LoadFromSaveData(ModConfigSaveData saveData)
        {
            SaveData = saveData.GetIntProperty(this);
        }

        public override void UpdateProperty(ModConfig config)
        {
            LinkedProperty.SetValue(config, this.SaveData.Value, null);
        }
    }
}
