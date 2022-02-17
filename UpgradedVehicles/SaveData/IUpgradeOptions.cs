namespace UpgradedVehicles.SaveData
{
    internal interface IUpgradeOptions
    {
        bool DebugLogsEnabled { get; }
        float ExosuitBonusSpeedMultiplier { get; }
        float SeamothBonusSpeedMultiplier { get; }
#if BELOWZERO
        float SeatruckBonusSpeedMultiplier { get; }
#endif
    }
}