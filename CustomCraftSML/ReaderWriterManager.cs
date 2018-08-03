namespace CustomCraft2SML
{
    internal static partial class FileReaderWriter
    {
        internal static void Patch()
        {
            HandleReadMeFile();

            GenerateOriginalRecipes();

            HandleWorkingFiles();
        }

    }
}
