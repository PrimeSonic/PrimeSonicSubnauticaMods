namespace CustomCraft2SML
{
    using System;
    using Common;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Message($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            try
            {
                FileReaderWriter.HandleReadMeFile();

                FileReaderWriter.GenerateOriginalRecipes();

                FileReaderWriter.HandleWorkingFiles();

                QuickLogger.Message("Finished patching.");
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Critical error during patching{Environment.NewLine}{ex}");
            }
        }
    }
}
