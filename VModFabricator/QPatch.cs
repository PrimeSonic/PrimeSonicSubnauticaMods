namespace VModFabricator
{
    using System;
    using Common;

    public class QPatch
    {
        public static void Patch()
        {
            try
            {
                QuickLogger.Message("Start");
                var vmodFabricator = new VModFabricatorModule();

                vmodFabricator.Patch();

                QuickLogger.Message("Finish");
            }
            catch (Exception ex)
            {
                QuickLogger.Error("EXCEPTION on Patch: " + ex.ToString());
            }
        }
    }
}
