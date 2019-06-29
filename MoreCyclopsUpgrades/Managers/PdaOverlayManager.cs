namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.API.PDA;

    internal static class PdaOverlayManager
    {
        private static readonly IDictionary<TechType, CreateIconOverlay> OverlayCreators = new Dictionary<TechType, CreateIconOverlay>();
        private static readonly IconOverlayCollection ActiveOverlays = new IconOverlayCollection();

        internal static bool HasUpgradeConsoleInPda => upgradeModules != null;
        private static Equipment upgradeModules = null;
        private static uGUI_Equipment equipmentUI = null;

        internal static void RegisterHandlerCreator(TechType techType, CreateIconOverlay createEvent, string assemblyName)
        {
            if (OverlayCreators.ContainsKey(techType))
            {
                QuickLogger.Warning($"Duplicate OverlayCreator blocked from {assemblyName}");
                return;
            }

            QuickLogger.Info($"Received OverlayCreator from {assemblyName}");
            OverlayCreators.Add(techType, createEvent);
        }

        internal static void StartConnectingToPda(Equipment modules)
        {
            upgradeModules = modules;
        }
        internal static void FinishingConnectingToPda(uGUI_Equipment equipmentUI)
        {
            if (upgradeModules == equipmentUI.equipment)
                ConnectToInventory(equipmentUI);
            else
                DisconnectFromPda();
        }

        internal static void DisconnectFromPda()
        {
            ActiveOverlays.Deactivate();
            upgradeModules = null;
            equipmentUI = null;
        }

        internal static void UpdateIconOverlays()
        {
            if (!HasUpgradeConsoleInPda)
                return;

            ActiveOverlays.UpdateText();
        }

        internal static void RemapItems(Equipment modules)
        {
            if (equipmentUI.equipment == modules)
            {
                ActiveOverlays.Deactivate();
                ConnectToInventory(equipmentUI);
            }
        }

        private static void ConnectToInventory(uGUI_Equipment equipmentUI)
        {
            foreach (KeyValuePair<InventoryItem, uGUI_EquipmentSlot> pair in equipmentUI.items)
            {
                InventoryItem item = pair.Key;

                if (OverlayCreators.TryGetValue(item.item.GetTechType(), out CreateIconOverlay creator))
                {
                    uGUI_ItemIcon icon = pair.Value.icon;
                    ActiveOverlays.Add(creator.Invoke(icon, item));
                }
            }
        }
    }
}
