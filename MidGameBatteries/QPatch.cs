namespace MidGameBatteries
{
    using System;
    using Common;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Message("Start patching. Version: " + QuickLogger.GetAssemblyVersion());

            try
            {
                var lithiumBattery = new DeepLithiumBattery();
                lithiumBattery.Patch();

                var lithiumPowerCell = new DeepLithiumPowerCell(lithiumBattery);
                lithiumPowerCell.Patch();

                QuickLogger.Message("Finished patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }
    }
}
