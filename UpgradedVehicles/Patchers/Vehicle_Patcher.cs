namespace UpgradedVehicles.Patchers
{
    using System;
    using Harmony;

    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    internal class Vehicle_OnUpgradeModuleChange_Patcher
    {
        public static void Postfix(Vehicle __instance)
        {
            VehicleUpgrader.UpgradeVehicle(__instance);
        }
    }

    //[HarmonyPatch(typeof(Vehicle))]
    //[HarmonyPatch("GetStorageInSlot")]
    //internal class Vehicle_GetStorageInSlot_Patcher
    //{
    //    public static bool Prefix(Vehicle __instance, int slotID, TechType techType, ref ItemsContainer __result)
    //    {
    //        if (techType != TechType.VehicleStorageModule)
    //        {
    //            Console.WriteLine($"[UpgradedVehicles] GetStorageInSlot : Skipped not storage");
    //            return true;
    //        }

    //        if (__instance.GetComponentInChildren<PrefabIdentifier>().ClassId != SeaMothMk2.NameID)
    //        {
    //            Console.WriteLine($"[UpgradedVehicles] GetStorageInSlot : Skipped not SeamothMk2");

    //            return true; // This is a normal Seamoth.
    //        }

    //        var storageDeluxe = __instance.GetComponentInChildren<SeaMothStorageDeluxe>();

    //        __result = storageDeluxe.GetStorageInSlot(slotID);

    //        return false;
    //    }
    //}

}
