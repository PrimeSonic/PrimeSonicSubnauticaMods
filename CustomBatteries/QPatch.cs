namespace CustomBatteries
{
    using System;
    using Common;
    using CustomBatteries.Items;
    using CustomBatteries.PackReading;
    using Harmony;
    using MidGameBatteries.Patchers;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Info("Start patching. Version: " + QuickLogger.GetAssemblyVersion());

            try
            {
                CbCore.PatchCraftingTabs();
                PackReader.PatchTextPacks();

                var harmony = HarmonyInstance.Create("com.custombatteries.mod");
                ChargerPatcher.Patch(harmony);
                EnergyMixinPatcher.Patch(harmony);

                QuickLogger.Info("Finished patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }
    }
}
