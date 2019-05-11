namespace IonCubeGenerator.Display.Patching
{
    using SMLHelper.V2.Handlers;

    internal static class DisplayLanguagePatching
    {
        public const string OverClockKey = "OverClocked";
        public const string CompletedKey = "Completed";
        public const string StorageKey = "Storage";
        public const string PrevSpeedKey = "PrevSpeed";
        public const string NextSpeedKey = "NextSpeed";
        public const string ToggleIonPowerKey = "PowerButtonDesc";
        public const string OpenStorageKey = "OpenStorage";
        public const string PoweredOffKey = "PoweredOff";
        public const string ReadyKey = "Ready";
        public const string MaxKey = "Max";
        public const string HighKey = "High";
        public const string MinKey = "Min";
        public const string LowKey = "Low";
        public const string OffKey = "Off";

        internal static void AdditionPatching()
        {
            LanguageHandler.SetLanguageLine(ReadyKey, "Ready");
            LanguageHandler.SetLanguageLine(PoweredOffKey, "POWERED OFF");
            LanguageHandler.SetLanguageLine(OpenStorageKey, "Open Storage");
            LanguageHandler.SetLanguageLine(ToggleIonPowerKey, "Toggle Ion Cube Generator Power");
            LanguageHandler.SetLanguageLine(NextSpeedKey, "Next Speed Mode");
            LanguageHandler.SetLanguageLine(PrevSpeedKey, "Previous Speed Mode");
            LanguageHandler.SetLanguageLine(StorageKey, "STORAGE");
            LanguageHandler.SetLanguageLine(CompletedKey, "COMPLETED");
            LanguageHandler.SetLanguageLine(OverClockKey, "OVERCLOCK");
            LanguageHandler.SetLanguageLine(MaxKey, "Max");
            LanguageHandler.SetLanguageLine(HighKey, "High");
            LanguageHandler.SetLanguageLine(MinKey, "Min");
            LanguageHandler.SetLanguageLine(LowKey, "Low");
            LanguageHandler.SetLanguageLine(OffKey, "Off");
        }
    }
}
