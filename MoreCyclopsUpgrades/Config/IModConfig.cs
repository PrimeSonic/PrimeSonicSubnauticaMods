namespace MoreCyclopsUpgrades.Config
{
    using MoreCyclopsUpgrades.Config.ChoiceEnums;

    internal interface IModConfig
    {
        bool AuxConsoleEnabled { get; }
        ChallengeLevel ChallengeMode { get; }
        ShowChargerIcons ChargerIcons { get; }
        bool DebugLogsEnabled { get; }
        HelmEnergyDisplay EnergyDisplay { get; }
        float DeficitThreshold { get; }
        float RechargePenalty { get; }
        bool ShowIconsOnHoloDisplay { get; }
        bool ShowIconsWhilePiloting { get; }
        float MinimumEnergyDeficit { get; }
        void UpdateCyclopsMaxPower(float maxPower);
    }
}