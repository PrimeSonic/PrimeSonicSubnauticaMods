namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.API.PDA;

    internal static class PdaOverlayManager
    {
        private static readonly IDictionary<TechType, CreateIconOverlay> OverlayCreators = new Dictionary<TechType, CreateIconOverlay>();
        private static readonly IconOverlayCollection ActiveOverlays = new IconOverlayCollection();

        private static Equipment upgradeModules = null;
        private static uGUI_Equipment equipmentUI = null;

        internal static void RegisterHandlerCreator(TechType techType, CreateIconOverlay createEvent, string assemblyName)
        {
            if (OverlayCreators.ContainsKey(techType))
            {
                QuickLogger.Warning($"PdaOverlayManager blocked duplicate IconOverlayCreator from {assemblyName}");
                return;
            }

            QuickLogger.Info($"PdaOverlayManager received IconOverlayCreator from {assemblyName}");
            OverlayCreators.Add(techType, createEvent);
        }

        internal static void StartConnectingToPda(Equipment modules)
        {
            QuickLogger.Debug("PdaOverlayManager started connecting upgrade console to PDA overlay");
            upgradeModules = modules;
        }

        internal static void FinishingConnectingToPda(uGUI_Equipment uGUI_Equipment)
        {
            equipmentUI = uGUI_Equipment;

            if (upgradeModules != null && equipmentUI != null && upgradeModules == uGUI_Equipment.equipment)
            {
                QuickLogger.Debug("PdaOverlayManager continued connecting upgrade console to PDA overlay");
                ConnectToInventory(uGUI_Equipment);
                ActiveOverlays.UpdateText();
            }
            else
            {
                QuickLogger.Debug("PdaOverlayManager failed to connect upgrade console to PDA overlay");

                if (upgradeModules == null)
                    QuickLogger.Debug("upgradeModules null");

                if (uGUI_Equipment == null)
                    QuickLogger.Debug("uGUI_Equipment null");

                DisconnectFromPda();
            }
        }

        internal static void DisconnectFromPda()
        {
            QuickLogger.Debug("PdaOverlayManager disconnecting from PDA overlay");
            ActiveOverlays.Deactivate();
            upgradeModules = null;
            equipmentUI = null;
        }

        internal static void UpdateIconOverlays()
        {
            if (upgradeModules == null || equipmentUI == null)
                return;

            ActiveOverlays.UpdateText();
        }

        internal static void RemapItems()
        {
            if (upgradeModules == null || equipmentUI == null)
                return;

            QuickLogger.Debug("PdaOverlayManager remapping PDA overlay");
            ActiveOverlays.Deactivate();
            ConnectToInventory(equipmentUI);
            ActiveOverlays.UpdateText();
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
