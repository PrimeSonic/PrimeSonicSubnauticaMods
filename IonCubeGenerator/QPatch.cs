namespace IonCubeGenerator
{
    using Common;
    using IonCubeGenerator.Buildable;
    using System;

    public static class QPatch
    {
        public static void Patch()
        {

#if DEBUG
            QuickLogger.DebugLogsEnabled = true;
            QuickLogger.Debug("Debug logs enabled");
#endif

            try
            {
                CubeGeneratorBuildable.PatchSMLHelper();
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }
    }
}
