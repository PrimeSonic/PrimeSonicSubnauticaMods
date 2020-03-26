namespace UpgradedVehicles.SaveData
{
    internal interface IUpgradeOptions
    {
        bool DebugLogsEnabled { get; }
        float ExosuitBonusSpeedMultiplier { get; }
        float SeamothBonusSpeedMultiplier { get; }
    }
}