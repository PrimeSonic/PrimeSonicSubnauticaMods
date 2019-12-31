namespace CustomBatteries
{
    using System;
    using Common;
    using CustomBatteries.Items;
    using CustomBatteries.PackReading;
    using Harmony;
    using MidGameBatteries.Patchers;
    using QModManager.API.ModLoading;

    [QModCore]
    public static class QPatch
    {
        [QModPatch]
        public static void Patch()
        {
            QuickLogger.Info("Start patching. Version: " + QuickLogger.GetAssemblyVersion());

            try
            {
                CbCore.PatchCraftingTabs();
                PackReader.PatchTextPacks();

                // Packs from external mods are patched as they arrive.
                // They can still be patched in even after the harmony patches have completed.

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
