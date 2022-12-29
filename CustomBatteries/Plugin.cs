namespace CustomBatteries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using CustomBatteries.Items;
    using CustomBatteries.PackReading;
    using CustomBatteries.Patches;
    using HarmonyLib;
    using MidGameBatteries.Patchers;
    using SMLHelper.V2.Handlers;
    using BepInEx;
    using BepInEx.Logging;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        private const string
            MODNAME = "Custom Batteries",
            AUTHOR = "PrimeSonic",
            GUID = "com.custombatteries.psmod",
            VERSION = "1.0.0.0";
        internal static ManualLogSource logSource;
        #endregion

        public void Awake()
        {
            logSource = Logger;
            QuickLogger.Info("Start patching. Version: " + QuickLogger.GetAssemblyVersion());

            try
            {
                PatchCraftingTabs();
                PackReader.PatchTextPacks();

                // Packs from external mods are patched as they arrive.
                // They can still be patched in even after the harmony patches have completed.

                var harmony = new Harmony("com.custombatteries.mod");
                EnergyMixinPatcher.Patch(harmony);
                ChargerPatcher.Patch(harmony);

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
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, CbDatabase.ResCraftTab, CbDatabase.ElecCraftTab, TechType.Battery.ToString());
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, CbDatabase.ResCraftTab, CbDatabase.ElecCraftTab, TechType.PrecursorIonBattery.ToString());
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, CbDatabase.ResCraftTab, CbDatabase.ElecCraftTab, TechType.PowerCell.ToString());
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, CbDatabase.ResCraftTab, CbDatabase.ElecCraftTab, TechType.PrecursorIonPowerCell.ToString());

            // Add a new set of tab nodes for batteries and power cells
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, CbDatabase.BatteryCraftTab, "Batteries", SpriteManager.Get(TechType.Battery), CbDatabase.ResCraftTab);
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, CbDatabase.PowCellCraftTab, "Power Cells", SpriteManager.Get(TechType.PowerCell), CbDatabase.ResCraftTab);

            // Move the original batteries and power cells into these new tabs
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.Battery, CbDatabase.BatteryCraftPath);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.PrecursorIonBattery, CbDatabase.BatteryCraftPath);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.PowerCell, CbDatabase.PowCellCraftPath);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.PrecursorIonPowerCell, CbDatabase.PowCellCraftPath);
        }

        public void Start()
        {
            UpdateCollection(BatteryCharger.compatibleTech, CbDatabase.BatteryItems);
            UpdateCollection(PowerCellCharger.compatibleTech, CbDatabase.PowerCellItems);
        }

        private static void UpdateCollection(HashSet<TechType> compatibleTech, List<CbCore> toBeAdded)
        {
            if (toBeAdded.Count == 0)
                return;

            // Make sure all custom batteries are allowed in the battery charger
            for (int i = toBeAdded.Count - 1; i >= 0; i--)
            {
                CbCore cbCoreItem = toBeAdded[i];

                TechType entry = cbCoreItem.TechType;
                
                if (compatibleTech.Contains(entry))
                    continue;

                compatibleTech.Add(entry);
            }
        }
    }
}
