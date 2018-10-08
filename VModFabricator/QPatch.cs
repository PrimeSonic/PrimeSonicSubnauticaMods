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
                QuickLogger.Message("Start patching. Version: " + QuickLogger.GetAssemblyVersion());
                var vmodFabricator = new VModFabricatorModule();

                vmodFabricator.Patch();
                QuickLogger.Message("Finished patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error("EXCEPTION on Patch: " + ex.ToString());
            }
        }
    }
}
