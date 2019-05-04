namespace IonCubeGenerator.Mono
{
    internal partial class CubeGeneratorMono : IProtoEventListener, IProtoTreeEventListener
    {
        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            // TODO - Load custom save data
        }

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            InitializeContainer();
            // TODO - Load custom save data
        }

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            // TODO - Create custom save data
        }

        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
            // TODO - Create custom save data
        }
    }
}
