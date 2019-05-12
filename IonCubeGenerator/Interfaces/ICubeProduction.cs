namespace IonCubeGenerator.Interfaces
{
    using IonCubeGenerator.Enums;

    internal interface ICubeProduction
    {
        bool NotAllowToGenerate { get; }
        float CubeProgress { get; }
        SpeedModes CurrentSpeedMode { get; set; }
        float RemainingTimeToNextCube { get; set; }
    }
}