namespace MoreScannerRoomUpgrades
{
    using System;
    using Common;

    internal class QPatch
    {
        public static void Patch()
        {
            try
            {




                QuickHarmony.PatchAssembly();
            }
            catch (Exception ex)
            {
                QuickLogger.Error("Exception during paching" + Environment.NewLine + ex);
            }
        }
    }
}
