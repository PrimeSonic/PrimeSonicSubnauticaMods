namespace MoreScannerRoomUpgrades.Patchers
{
    using Common;
    using Craftables;
    using Harmony;
    using Monobehaviors;

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch("OnUpgradeModuleToggle")]
    internal class Vehicle_OnUpgradeModuleToggle_Patcher
    {
        [HarmonyPostfix]
        internal static void PostFix(Vehicle __instance, int slotID, bool active)
        {
            string slotName = Utilities.SlotIDs(__instance)[slotID];

            InventoryItem item = __instance.modules.GetItemInSlot(slotName);

            QuickLogger.Debug($"OnUpgradeModuleToggle slotID:{slotName} active:{active}", true);

            TechType techType = item.item.GetTechType();
            if (techType != VehicleMapScannerModule.ItemID)
            {
                QuickLogger.Debug($"{techType} is Not a VehicleMapScanner", true);
                return;
            }

            VehicleMapScanner scanner = item.item.GetComponent<VehicleMapScanner>();

            if (scanner == null)
            {
                QuickLogger.Debug("VehicleMapScanner NOT found in InventoryItem", true);
                return;
            }

            QuickLogger.Debug("VehicleMapScanner found", true);

#if DEBUG

            if (active) // Need a way to interact with this
            {
                scanner.StartScanning(TechType.AluminumOxide); // For testing only
                QuickLogger.Debug("Started scanning", true);
            }
            else if (scanner.IsScanActive())
            {
                scanner.StopScanning();
                QuickLogger.Debug("Scanning stopped", true);
            }
#endif
        }
    }

    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    internal class Vehicle_OnUpgradeModuleChange_Patcher
    {
        [HarmonyPostfix]
        internal static void PostFix(Vehicle __instance, int slotID, TechType techType, bool added)
        {
            if (techType != VehicleMapScannerModule.ItemID)
                return; // Not of interest - Skip

            if (added)
            {
                VehicleMapScanner scanner = Utilities.GetScannerInSlot(__instance, slotID);

                scanner.LinkVehicle(__instance, slotID);
                QuickLogger.Debug($"VehicleMapScanner linked on slot {slotID}", true);
            }
            else // Removed
            {
                VehicleMapScanner scanner = VehicleMapScanner.VehicleMapScanners.Find(s => s.LinkedVehicleSlotID == slotID && s.LinkedVehicle.GetInstanceID() == __instance.GetInstanceID());

                if (scanner != null)
                {
                    scanner.UnlinkVehicle();
                    QuickLogger.Debug($"VehicleMapScanner unlinked from slot {slotID}", true);
                }
            }
        }
    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch("OnUpgradeModuleUse")]
    internal class Vehicle_OnUpgradeModuleUse_Patcher
    {
        [HarmonyPostfix]
        internal static void PostFix(Vehicle __instance, TechType techType, int slotID)
        {
            if (techType != VehicleMapScannerModule.ItemID)
                return; // Not of interest - Skip

            VehicleMapScanner scanner = Utilities.GetScannerInSlot(__instance, slotID);

            if (scanner == null)
            {
                QuickLogger.Debug("VehicleMapScanner NOT found in InventoryItem", true);
                return;
            }

            QuickLogger.Debug("VehicleMapScanner ready to enable UI for resource selection", true);
            // TODO put logic for selecting a resource to scan here
        }

    }
}
