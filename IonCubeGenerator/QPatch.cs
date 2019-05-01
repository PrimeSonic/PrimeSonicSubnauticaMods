namespace IonCubeGenerator
{
    using Common;
    using IonCubeGenerator.Buildable;
    using System;

    public static class QPatch
    {

        public static void Patch()
        {
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
