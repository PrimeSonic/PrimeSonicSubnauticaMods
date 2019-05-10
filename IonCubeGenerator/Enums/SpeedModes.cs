namespace IonCubeGenerator.Enums
{
    internal enum SpeedModes : int
    {
        Off = 0,  // Not generating
        Max = 250,  // 250 seconds ~ 4.1 minutes
        High = 500, // 500 seconds ~ 8.3 minutes
        Low = 750,  // 750 seconds ~ 12.5 minutes
        Min = 1000, // 1000 seconds ~ 16.6 minutes
    }
}
