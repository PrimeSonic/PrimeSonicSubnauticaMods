namespace MoreCyclopsUpgrades.Config
{
    internal interface IModConfig
    {
        bool AuxConsoleEnabled { get; }
        ChallengeLevel ChallengeMode { get; }
        ShowChargerIcons ChargerIcons { get; }
        bool DebugLogsEnabled { get; }
        float DeficitThreshold { get; }
        bool ShowIconsAtHelm { get; }
        bool ShowIconsWhilePiloting { get; }
    }
}