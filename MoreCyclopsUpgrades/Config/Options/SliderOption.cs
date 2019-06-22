namespace MoreCyclopsUpgrades.Config.Options
{
    using Common.EasyMarkup;

    internal class SliderOption : ConfigOption
    {
        public delegate void SliderEvent(float value);

        public SliderEvent ValueChanged;
        public float MinValue;
        public float MaxValue;
        public float Value;
        public EmProperty<float> SaveData;

        public SliderOption(string id, string label)
            : base(OptionTypes.Slider, id, label)
        {
        }
    }
}
