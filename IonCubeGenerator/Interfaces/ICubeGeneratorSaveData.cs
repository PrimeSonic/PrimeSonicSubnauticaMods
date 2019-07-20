namespace IonCubeGenerator.Interfaces
{
    using IonCubeGenerator.Enums;

    internal interface ICubeGeneratorSaveData
    {
        int NumberOfCubes { get; set; }

        float StartUpProgress { get; set; }
        float GenerationProgress { get; set; }
        float CoolDownProgress { get; set; }

        SpeedModes CurrentSpeedMode { get; set; }
    }
}