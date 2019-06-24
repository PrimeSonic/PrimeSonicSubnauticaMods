namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.API.PDA;
    using MoreCyclopsUpgrades.Items.AuxConsole;

    internal static class PdaOverlayManager
    {
        private static readonly List<IconOverlay> iconOverlays = new List<IconOverlay>(6);
        private static readonly IDictionary<TechType, CreateOverlayText> OverlayCreators = new Dictionary<TechType, CreateOverlayText>();

        internal static bool HasUpgradeConsoleInPda => upgradeModules != null;
        private static Equipment upgradeModules = null;        

        internal static void RegisterHandlerCreator(TechType techType, CreateOverlayText createEvent, string assemblyName)
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
        internal static void FinishingConnectingToPda(uGUI_Equipment items)
        {
            if (upgradeModules == items.equipment)
                ConnectToInventory(items.items);            
            else
                DisconnectFromPda();
        }

        internal static void DisconnectFromPda()
        {
            iconOverlays.Clear();
            upgradeModules = null;
        }

        internal static void UpdateIconOverlays()
        {
            if (!HasUpgradeConsoleInPda)
                return;

            foreach (IconOverlay overlay in iconOverlays)
                overlay.UpdateText();
        }

        private static void ConnectToInventory(Dictionary<InventoryItem, uGUI_EquipmentSlot> items)
        {            
            foreach (KeyValuePair<InventoryItem, uGUI_EquipmentSlot> pair in items)
            {
                InventoryItem item = pair.Key;

                if (OverlayCreators.TryGetValue(item.item.GetTechType(), out CreateOverlayText creator))
                {
                    iconOverlays.Add(creator.Invoke(pair.Value.icon, item));
                }
            }
        }
    }
}
