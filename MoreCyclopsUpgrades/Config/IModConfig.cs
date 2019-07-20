namespace MoreCyclopsUpgrades.Config
{
    using MoreCyclopsUpgrades.Config.ChoiceEnums;

    internal interface IModConfig
    {
        bool AuxConsoleEnabled { get; }
        ChallengeMode ChallengeMode { get; }
        ShowChargerIcons ChargerIcons { get; }
        bool DebugLogsEnabled { get; }
        HelmEnergyDisplay EnergyDisplay { get; }
        float DeficitThreshold { get; }
        float RechargePenalty { get; }
        bool HidePowerIcons { get; }
        bool ShowIconsOnHoloDisplay { get; }
        bool ShowIconsWhilePiloting { get; }
        float MinimumEnergyDeficit { get; }
        float EmergencyEnergyDeficit { get; }
        void UpdateCyclopsMaxPower(float maxPower);
    }
}