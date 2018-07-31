namespace CustomCraft2SML
{
    using System;

    public static class QPatch
    {
        public static void Patch()
        {
            Logger.Log("Loading files begin");

            try
            {
                TutorialFiles.HandleReadMeFile();

                TutorialFiles.GenerateOriginalRecipes();

                ParsingFiles.PatchCustomSizes();

                ParsingFiles.PatchModifiedRecipes();

                ParsingFiles.PatchAddedRecipes();

            }
            catch (IndexOutOfRangeException outEx)
            {
                Logger.Log(outEx.ToString());
            }

            Logger.Log("Loading files complete");
        }
    }
}
