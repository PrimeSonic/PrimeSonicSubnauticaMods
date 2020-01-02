namespace IonCubeGenerator.Display.Patching
{
    using SMLHelper.V2.Handlers;

    internal static class DisplayLanguagePatching
    {
        public const string OverClockKey = "ICG_OverClocked";
        public const string CompletedKey = "ICG_Completed";
        public const string StorageKey = "ICG_Storage";
        public const string PrevSpeedKey = "ICG_PrevSpeed";
        public const string NextSpeedKey = "ICG_NextSpeed";
        public const string ToggleIonPowerKey = "ICG_PowerButtonDesc";
        public const string OpenStorageKey = "ICG_OpenStorage";
        public const string PoweredOffKey = "ICG_PoweredOff";
        public const string ReadyKey = "ICG_Ready";
        public const string MaxKey = "ICG_Max";
        public const string HighKey = "ICG_High";
        public const string MinKey = "ICG_Min";
        public const string LowKey = "ICG_Low";
        public const string OffKey = "ICG_Off";

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
