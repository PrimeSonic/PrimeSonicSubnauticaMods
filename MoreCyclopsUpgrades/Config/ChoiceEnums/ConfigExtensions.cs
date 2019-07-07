namespace MoreCyclopsUpgrades.Config.ChoiceEnums
{
    using UnityEngine;

    internal static class ConfigExtensions
    {
        internal static float ChallengePenalty(this ChallengeMode value)
        {
            return ((int)value) * 0.20f;
        }

        internal static string AsDisplay(this ChallengeMode value)
        {
            return $"{value} (-{Mathf.RoundToInt(ChallengePenalty(value) * 100f)}%)";
        }

        internal static string AsDisplay(this ShowChargerIcons value)
        {
            switch (value)
            {
                case ShowChargerIcons.OnPilotingHUD:
                    return "On Piloting HUD";
                case ShowChargerIcons.OnHoloDisplay:
                    return "On Holo Display";
                default:
                    return value.ToString();
            }
        }

        internal static string AsDisplay(this HelmEnergyDisplay value)
        {
            switch (value)
            {
                case HelmEnergyDisplay.PowerCellPercentage:
                    return "PowerCell %";
                case HelmEnergyDisplay.PowerCellAmount:
                    return "PowerCell Energy";
                case HelmEnergyDisplay.PercentageOverPowerCells:
                    return "PowerCell + Reserve %";
                case HelmEnergyDisplay.CombinedAmount:
                    return "PowerCell + Reserve Energy";
                default:
                    return value.ToString();
            }
        }
    }
}
