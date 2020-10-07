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
    using SMLHelper.V2.Handlers;

    [QModCore]
    public static class QPatch
    {
        [QModPatch]
        public static void Patch()
        {
            QuickLogger.Info("Start patching. Version: " + QuickLogger.GetAssemblyVersion());

            try
            {
                PatchCraftingTabs();
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

        internal static void PatchCraftingTabs()
        {
            QuickLogger.Info("Separating batteries and power cells into their own fabricator crafting tabs");

            // Remove original crafting nodes
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, CbCore.ResCraftTab, CbCore.ElecCraftTab, TechType.Battery.ToString());
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, CbCore.ResCraftTab, CbCore.ElecCraftTab, TechType.PrecursorIonBattery.ToString());
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, CbCore.ResCraftTab, CbCore.ElecCraftTab, TechType.PowerCell.ToString());
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, CbCore.ResCraftTab, CbCore.ElecCraftTab, TechType.PrecursorIonPowerCell.ToString());

            // Add a new set of tab nodes for batteries and power cells
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, CbCore.BatteryCraftTab, "Batteries", SpriteManager.Get(TechType.Battery), CbCore.ResCraftTab);
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, CbCore.PowCellCraftTab, "Power Cells", SpriteManager.Get(TechType.PowerCell), CbCore.ResCraftTab);

            // Move the original batteries and power cells into these new tabs
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.Battery, CbCore.BatteryCraftPath);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.PrecursorIonBattery, CbCore.BatteryCraftPath);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.PowerCell, CbCore.PowCellCraftPath);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.PrecursorIonPowerCell, CbCore.PowCellCraftPath);
        }

        [QModPostPatch]
        public static void UpdateStaticCollections()
        {
            UpdateCollection(BatteryCharger.compatibleTech, CbCore.BatteryItems);
            UpdateCollection(PowerCellCharger.compatibleTech, CbCore.PowerCellItems);
        }

        private static void UpdateCollection(HashSet<TechType> compatibleTech, List<CbCore> toBeAdded)
        {
            if (toBeAdded.Count == 0)
                return;

            // Make sure all custom batteries are allowed in the battery charger
            for (int i = toBeAdded.Count - 1; i >= 0; i--)
            {
                CbCore cbCoreItem = toBeAdded[i];

                if (cbCoreItem.ExcludeFromChargers)
                    continue;

                TechType entry = cbCoreItem.TechType;
                
                if (compatibleTech.Contains(entry))
                    continue;

                compatibleTech.Add(entry);
            }
        }
    }
}
