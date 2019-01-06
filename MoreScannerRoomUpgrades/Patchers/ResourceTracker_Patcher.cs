namespace MoreScannerRoomUpgrades.Patchers
{
    using System.Collections.Generic;
    using Harmony;
    using Monobehaviors;

    [HarmonyPatch(typeof(uGUI_ResourceTracker))]
    [HarmonyPatch("GatherScanned")]
    internal class UGUI_ResourceTracker_GatherScanned_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(uGUI_ResourceTracker __instance)
        {
            HashSet<ResourceTracker.ResourceInfo> nodes = Utilities.ResourceNodes(__instance);

            foreach (VehicleMapScanner mobileScanner in VehicleMapScanner.VehicleMapScanners)
            {
                if (Utilities.Distance(MainCamera.camera.transform.position, mobileScanner.LinkedVehiclePosition()) > 500f)
                    continue; // Too far away, don't render

                mobileScanner.GetDiscoveredNodes(nodes);
            }
        }
    }


}
