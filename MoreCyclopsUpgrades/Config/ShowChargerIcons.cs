namespace MoreCyclopsUpgrades.Config
{
    using System;

    [Flags]
    internal enum ShowChargerIcons
    {
        None,
        WhenPiloting = 0b01,
        HelmDisplay = 0b10,
        Both = WhenPiloting | HelmDisplay
    }
}
