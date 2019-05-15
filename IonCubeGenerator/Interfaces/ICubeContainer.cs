namespace IonCubeGenerator.Interfaces
{
    internal interface ICubeContainer
    {
        bool IsFull { get; }
        int NumberOfCubes { get; set; }
        void OpenStorage();
    }
}