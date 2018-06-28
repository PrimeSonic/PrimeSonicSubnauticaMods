namespace MoreCyclopsUpgrades
{
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary>
    /// This class handles keeping track of the nuclear batteries.
    /// </summary>
    internal static class NuclearChargingManager
    {
        internal const float BatteryDrainRate = 0.15f;
        internal const float MaxCharge = 6000f; // Less than the normal 20k for balance

        /// <summary>
        /// Replaces a nuclear battery modules with Depleted Reactor Rods when they fully drained.
        /// </summary>
        public static void HandleBatteryDepletion(Equipment modules, string slotName, Battery nuclearBattery)
        {
            if (nuclearBattery.charge <= 0f) // Drained nuclear batteries are handled just like how the Nuclear Reactor handles depleated reactor rods
            {
                InventoryItem inventoryItem = modules.RemoveItem(slotName, true, false);
                Object.Destroy(inventoryItem.item.gameObject);
                modules.AddItem(slotName, SpawnDepletedModule(), true);
            }
        }

        private static InventoryItem SpawnDepletedRod()
        {
            GameObject prefabForTechType = CraftData.GetPrefabForTechType(TechType.DepletedReactorRod, true);
            GameObject gameObject = Object.Instantiate(prefabForTechType);
            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
            return new InventoryItem(pickupable);
        }

        private static InventoryItem SpawnDepletedModule()
        {
            GameObject prefab = Resources.Load<GameObject>("WorldEntities/Natural/DepletedCyclopsNuclearModule");
            GameObject obj = Object.Instantiate(prefab);
            Pickupable pickupable = obj.GetComponent<Pickupable>().Pickup(false);
            return new InventoryItem(pickupable);
        }
    }

}
