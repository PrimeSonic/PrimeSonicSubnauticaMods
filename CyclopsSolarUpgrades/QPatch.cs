namespace CyclopsSolarUpgrades
{
    using System;
    using Common;
    using CyclopsSolarUpgrades.Craftables;

    public static class QPatch
    {
        public static void Patch()
        {
            try
            {
                QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

                var solar1 = new CyclopsSolarCharger();
                var solar2 = new CyclopsSolarChargerMk2(solar1);

                solar1.Patch();
                solar2.Patch();

                QuickLogger.Info($"Finished patching.");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }
    }
}
