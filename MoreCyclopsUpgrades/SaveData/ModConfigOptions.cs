namespace MoreCyclopsUpgrades.SaveData
{
    using SMLHelper.V2.Options;

    internal class ModConfigOptions : ModOptions
    {
        private const string ChoiceID = "MCUMode";

        public ModConfigOptions() : base("MoreCyclopsUpgrades Difficulty (Requires restart!)")
        {
            base.ChoiceChanged += ModConfigOptions_ChoiceChanged;
        }

        private void ModConfigOptions_ChoiceChanged(object sender, ChoiceChangedEventArgs e)
        {
            if (e.Id != ChoiceID)
                return;

            ModConfig.Settings.PowerLevel = (CyclopsPowerLevels)e.Index;
        }

        public override void BuildModOptions()
        {
            base.AddChoiceOption(ChoiceID, "Cyclops Power",
                                 new string[]
                                 {
                                    $"{CyclopsPowerLevels.Leviathan} (Easy)",
                                    $"{CyclopsPowerLevels.Ampeel} (Modest)",
                                    $"{CyclopsPowerLevels.Crabsnake} (Moderate)",
                                    $"{CyclopsPowerLevels.Peeper} (Hard)",
                                 }, (int)ModConfig.Settings.PowerLevel);
        }
    }
}
