using IonCubeGenerator.Enums;

namespace IonCubeGenerator
{
    internal interface ICubeGeneratorSaveData
    {
        SpeedModes CurrentSpeedMode { get; set; }
        int NumberOfCubes { get; set; }
        float RemainingTimeToNextCube { get; set; }
    }
}