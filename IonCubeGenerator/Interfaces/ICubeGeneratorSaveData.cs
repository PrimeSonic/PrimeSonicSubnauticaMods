namespace IonCubeGenerator.Interfaces
{
    using IonCubeGenerator.Enums;

    internal interface ICubeGeneratorSaveData
    {
        bool IsLoadingSaveData { get; set; }
        SpeedModes CurrentSpeedMode { get; set; }
        int NumberOfCubes { get; set; }
        float RemainingTimeToNextCube { get; set; }
    }
}