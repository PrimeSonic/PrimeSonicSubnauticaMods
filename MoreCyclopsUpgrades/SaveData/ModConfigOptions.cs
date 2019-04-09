namespace MoreCyclopsUpgrades.SaveData
{
    using SMLHelper.V2.Options;
    using System;


    internal class ModConfigOptions : ModOptions
    {
        private const string ChoiceID = "MCUMode";

        public ModConfigOptions() : base("MoreCyclopsUpgrades Difficulty")
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
            base.AddChoiceOption(ChoiceID, "Cyclops Power (Requires restart!)",
                                 new string[]
                                 {
                                    $"{CyclopsPowerLevels.Leviathan} (Easy difficulty)",
                                    $"{CyclopsPowerLevels.Ampeel} (Modest difficulty)",
                                    $"{CyclopsPowerLevels.Crabsnake} (Moderate difficulty)",
                                    $"{CyclopsPowerLevels.Peeper} (Hard difficulty)",
                                 }, (int)ModConfig.Settings.PowerLevel);
        }
    }
}
