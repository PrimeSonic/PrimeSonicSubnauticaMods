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
                CbCore.PatchCraftingTabs();
                PatchPacks();

                QuickLogger.Info("Applying Harmony Patches");
                var harmony = HarmonyInstance.Create("com.custombatteries.mod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

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

            if (!Directory.Exists(pluginPacksFolder))
            {
                QuickLogger.Warning("'Packs' folder was not found. Folder will be created. No text plugins were patched.");
                Directory.CreateDirectory(pluginPacksFolder);
                return;
            }

            var customPacks = new List<CustomPack>();

            QuickLogger.Info("Building pluging packs");
            foreach (IPluginDetails pluginPack in PackReader.GetAllPacks(pluginPacksFolder))
            {
                QuickLogger.Info($"Found CustomBatteriesPack '{pluginPack.PluginPackName}'");
                customPacks.Add(new CustomPack(pluginPack));
            }

            if (customPacks.Count == 0)
            {
                QuickLogger.Warning("No plugin files were found in the 'Packs' folder. No text plugins were patched.");
                return;
            }

            QuickLogger.Info($"Patching '{customPacks.Count}' pluging pack(s) with SMLHelper");
            foreach (CustomPack customPack in customPacks)
            {
                QuickLogger.Info($"Patching plugin pack '{customPack.PluginPackName}'");
                customPack.Patch();
            }
        }
    }
}
