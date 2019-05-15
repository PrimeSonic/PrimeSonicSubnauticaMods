namespace IonCubeGenerator.Interfaces
{
    internal interface ICubeGeneratorSaveHandler
    {
        void LoadData(ICubeGeneratorSaveData cubeGenerator);
        void SaveData(ICubeGeneratorSaveData cubeGenerator);
    }
}