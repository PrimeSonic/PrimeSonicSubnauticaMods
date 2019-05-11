namespace IonCubeGenerator.Interfaces
{
    using IonCubeGenerator.Enums;

    internal interface ICubeProduction
    {
        bool IsProductionStopped { get; }
        float CubeProgress { get; }
        SpeedModes CurrentSpeedMode { get; set; }
        float RemainingTimeToNextCube { get; set; }
    }
}