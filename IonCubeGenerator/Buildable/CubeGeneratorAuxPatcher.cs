namespace IonCubeGenerator.Buildable
{
    using SMLHelper.V2.Handlers;

    internal partial class CubeGeneratorBuildable
    {
        // TODO OnHover text

        private const string StorageLabelKey = "CubeGenStorage";
        public static string StorageLabel()
        {
            return Language.main.Get(StorageLabelKey);
        }

        private const string OnHoverKey = "CubeGenOnHover";
        public static string OnHoverText()
        {
            return Language.main.Get(OnHoverKey);
        }

        private void AdditionalPatching()
        {
            LanguageHandler.SetLanguageLine(StorageLabelKey, "Ion Cube Generator Receptical");
            LanguageHandler.SetLanguageLine(OnHoverKey, "Ion Cube Generator");
        }
    }
}
