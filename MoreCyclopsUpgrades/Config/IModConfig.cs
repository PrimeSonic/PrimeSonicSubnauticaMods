namespace MoreCyclopsUpgrades.Config
{
    using MoreCyclopsUpgrades.Config.ChoiceEnums;

    internal interface IModConfig
    {
        bool AuxConsoleEnabled { get; }
        ChallengeMode ChallengeMode { get; }
        ShowChargerIcons ChargerIcons { get; }
        bool EnableDebugLogs { get; }
        HelmEnergyDisplay EnergyDisplay { get; }
        float RechargePenalty { get; }
        bool HidePowerIcons { get; }
        bool ShowIconsOnHoloDisplay { get; }
        bool ShowIconsWhilePiloting { get; }
        bool ShowThermometer { get; }
    }
}