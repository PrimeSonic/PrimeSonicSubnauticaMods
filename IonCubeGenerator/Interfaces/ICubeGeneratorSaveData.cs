namespace IonCubeGenerator.Interfaces
{
    using IonCubeGenerator.Enums;
    using System.Collections.Generic;

    internal interface ICubeGeneratorSaveData
    {
        bool IsLoadingSaveData { get; set; }
        SpeedModes CurrentSpeedMode { get; set; }
        int NumberOfCubes { get; set; }
        IList<float> Progress { get; set; }
    }
}