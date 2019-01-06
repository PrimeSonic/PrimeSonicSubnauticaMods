namespace MoreScannerRoomUpgrades
{
    using System;
    using Common;
    using Craftables;

    internal class QPatch
    {
        public static void Patch()
        {
            try
            {
                QuickLogger.Message("Started patching. Version:" + QuickLogger.GetAssemblyVersion());

                VehicleMapScannerModule.PatchAll();
                QuickHarmony.PatchAssembly();

                QuickLogger.Message("Patching finished");
            }
            catch (Exception ex)
            {
                QuickLogger.Error("Exception during paching" + Environment.NewLine + ex);
            }
        }
    }
}
