namespace CustomCraft2SML
{
    using System;

    public static class QPatch
    {
        public static void Patch()
        {
            Logger.Log("START");

            try
            {
                FileReaderWriter.Patch();
            }
            catch (IndexOutOfRangeException outEx)
            {
                Logger.Log(outEx.ToString());
            }

            Logger.Log("FINISH");
        }
    }
}
