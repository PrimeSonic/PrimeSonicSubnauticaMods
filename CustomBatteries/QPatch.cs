namespace CustomBatteries
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Common;
    using CustomBatteries.Items;
    using CustomBatteries.PackReading;
    using Harmony;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Info("Start patching. Version: " + QuickLogger.GetAssemblyVersion());

            try
            {
                QuickLogger.Info("Applying Harmony Patches");
                var harmony = HarmonyInstance.Create("com.custombatteries.mod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                CbCore.PatchCraftingTabs();
                PatchPacks();

                QuickLogger.Info("Finished patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }

        private static void PatchPacks()
        {
            QuickLogger.Info("Reading pluging packs");
            string pluginPacksFolder = Path.Combine(CbCore.ExecutingFolder, "Packs");
            
            var customPacks = new List<CustomPack>();

            QuickLogger.Info("Building pluging packs");
            foreach (IPluginPack pluginPack in PackReader.GetAllPacks(pluginPacksFolder))
                customPacks.Add(new CustomPack(pluginPack));

            QuickLogger.Info("Patching pluging packs with SMLHelper");
            foreach (CustomPack customPack in customPacks)
                customPack.Patch();
        }
    }
}
