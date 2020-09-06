namespace CustomBatteries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using CustomBatteries.Items;
    using CustomBatteries.PackReading;
    using HarmonyLib;
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

                var harmony = new Harmony("com.custombatteries.mod");
                EnergyMixinPatcher.Patch(harmony);

                QuickLogger.Info("Finished patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }

        [QModPostPatch]
        public static void UpdateStaticCollections()
        {
            UpdateCollection(BatteryCharger.compatibleTech, CbCore.BatteryTechTypes);
            UpdateCollection(PowerCellCharger.compatibleTech, CbCore.PowerCellTechTypes);
        }

        private static void UpdateCollection(HashSet<TechType> compatibleTech, List<TechType> toBeAdded)
        {
            if (toBeAdded.Count == 0)
                return;

            // Make sure all custom batteries are allowed in the battery charger
            if (!compatibleTech.Contains(toBeAdded[toBeAdded.Count - 1]))
            {
                // Checks in reverse order to account for the (unlikely) event that an external mod patches later than expected
                for (int i = toBeAdded.Count - 1; i >= 0; i--)
                {
                    TechType entry = toBeAdded[i];
                    if (compatibleTech.Contains(entry))
                        return;

                    compatibleTech.Add(entry);
                }
            }
        }
    }
}
