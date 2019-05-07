namespace IonCubeGenerator.Mono
{
    internal partial class CubeGeneratorMono : IProtoTreeEventListener
    {
        private bool _isLoadingSaveData = false;
        private CubeGeneratorSaveData _saveData;

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            _isLoadingSaveData = true;

            _saveData.LoadData(this);

            _isLoadingSaveData = false;
        }

        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
            _saveData.SaveData(this);
        }
    }
}
