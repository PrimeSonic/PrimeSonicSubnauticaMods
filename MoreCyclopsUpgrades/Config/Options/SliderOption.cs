namespace MoreCyclopsUpgrades.Config.Options
{
    using EasyMarkup;

    internal class SliderOption : ConfigOption
    {
        public delegate void SliderEvent(float value, ModConfig config);

        public SliderEvent ValueChanged;
        public float MinValue;
        public float MaxValue;
        public float Value;
        public EmProperty<float> SaveData;

        public SliderOption(string id, string label)
            : base(OptionTypes.Slider, id, label)
        {
        }

        public override void LoadFromSaveData(ModConfigSaveData saveData)
        {
            SaveData = saveData.GetFloatProperty(this);
        }

        public override void UpdateProperty(ModConfig config)
        {
            LinkedProperty.SetValue(config, this.SaveData.Value, null);
        }
    }
}
