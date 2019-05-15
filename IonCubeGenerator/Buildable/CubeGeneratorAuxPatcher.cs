namespace IonCubeGenerator.Buildable
{
    using SMLHelper.V2.Handlers;

    internal partial class CubeGeneratorBuildable
    {
        private const string StorageLabelKey = "CubeGenStorage";
        public static string StorageLabel()
        {
            return Language.main.Get(StorageLabelKey);
        }

        private const string OnHoverProgressKey = "CubeGenProgress";
        public static string OnHoverTextProgress(int percentage)
        {
            return Language.main.GetFormat(OnHoverProgressKey, percentage);
        }

        private const string OnHoverUnpoweredKey = "CubeGenNoPower";
        public static string OnHoverTextNoPower()
        {
            return Language.main.Get(OnHoverUnpoweredKey);
        }

        private const string OnHoverFullKey = "CubeGenFull";
        public static string OnHoverTextFull()
        {
            return Language.main.Get(OnHoverFullKey);
        }

        private const string BlueprintUnlockedKey = "CubeGenUnlock";
        public static string BlueprintUnlockedMsg()
        {
            return Language.main.Get(BlueprintUnlockedKey);
        }

        public static string BuildableName { get; private set; }
        public static TechType TechTypeID { get; private set; }

        private void AdditionalPatching()
        {
            BuildableName = this.FriendlyName;
            TechTypeID = this.TechType;
            LanguageHandler.SetLanguageLine(StorageLabelKey, "Ion Cube Generator Receptical");
            LanguageHandler.SetLanguageLine(OnHoverProgressKey, "Ion Cube Generator ({0}%)");
            LanguageHandler.SetLanguageLine(OnHoverUnpoweredKey, "Insufficient power");
            LanguageHandler.SetLanguageLine(OnHoverFullKey, "Ion cubes ready!");
            LanguageHandler.SetLanguageLine(BlueprintUnlockedKey, "Ion cube generator blueprint discovered!");
        }
    }
}
