namespace MoreScannerRoomUpgrades
{
    using System.Collections.Generic;
    using System.Reflection;
    using Monobehaviors;
    using UnityEngine;

    internal static class Utilities
    {
        private static readonly FieldInfo nodesInfo = typeof(uGUI_ResourceTracker).GetField("nodes", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly PropertyInfo SlotIDsInfo = typeof(Vehicle).GetProperty("slotIDs", BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance);

        internal static string[] SlotIDs(Vehicle vehicle) => (string[])SlotIDsInfo.GetValue(vehicle, null);

        internal static HashSet<ResourceTracker.ResourceInfo> ResourceNodes(uGUI_ResourceTracker tracker) => (HashSet<ResourceTracker.ResourceInfo>)nodesInfo.GetValue(tracker);

        internal static float Distance(Vector3 a, Vector3 b) => (a - b).sqrMagnitude;

        internal static VehicleMapScanner GetScannerInSlot(Vehicle vehicle, int slotId)
        {
            string[] slots = SlotIDs(vehicle);

            string slotName = slots[slotId];

            InventoryItem item = vehicle.modules.GetItemInSlot(slotName);

            return item.item.GetComponent<VehicleMapScanner>();
        }
    }
}
