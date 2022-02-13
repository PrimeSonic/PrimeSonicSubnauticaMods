namespace UpgradedVehicles.SaveData
{
    internal interface IUpgradeOptions
    {
        bool DebugLogsEnabled { get; }
        float ExosuitBonusSpeedMultiplier { get; }
#if SUBNAUTICA
        float SeamothBonusSpeedMultiplier { get; }
#elif BELOWZERO
        float SeatruckBonusSpeedMultiplier { get; }
#endif
    }
}