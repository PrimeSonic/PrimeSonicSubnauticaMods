namespace CommonCyclopsUpgrades.Options
{
    using Common.EasyMarkup;

    internal class ChoiceOption : ConfigOption
    {
        public delegate void ChoiceEvent(int index);

        public ChoiceEvent ChoiceChanged;
        public string[] Choices;
        public int Index;
        public EmProperty<int> SaveData;

        public ChoiceOption(string id, string label)
            : base(OptionTypes.Choice, id, label)
        {
        }
    }
}
