namespace IonCubeGenerator.Mono
{
    using Common;

    internal partial class CubeGeneratorMono : IProtoTreeEventListener
    {
        private bool _isLoadingSaveData = false;
        private CubeGeneratorSaveData _saveData;

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            _isLoadingSaveData = true;

            InitializeContainer();

            QuickLogger.Debug("Loading save data");

            if (_saveData.LoadData())
            {
                QuickLogger.Debug("Save data found");

                _cubeContainer.Clear(false);

                timeToNextCube = _saveData.RemainingTimeToNextCube;

                int numberOfCubes = _saveData.NumberOfCubes;

                for (int i = 0; i < numberOfCubes; i++)
                    SpawnCube();
            }

            _isLoadingSaveData = false;
        }

        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
            QuickLogger.Debug("Saving state to file");
            _saveData.NumberOfCubes = this.CurrentCubeCount;
            _saveData.RemainingTimeToNextCube = isGenerating ? timeToNextCube : -1f;
            _saveData.SaveData();
        }
    }
}
