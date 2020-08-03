namespace MoreCyclopsUpgrades.Patchers
{
    using Common;
    using HarmonyLib;
    using MoreCyclopsUpgrades.AuxConsole;
    using MoreCyclopsUpgrades.Managers;
    using UnityEngine;

    [HarmonyPatch(typeof(UpgradeConsole), nameof(UpgradeConsole.OnHandClick))]
    internal static class UpgradeConsole_OnHandClick_Patcher
    {
        [HarmonyPrefix]
        public static void Prefix(UpgradeConsole __instance)
        {
            PdaOverlayManager.StartConnectingToPda(__instance.modules);
        }

        [HarmonyPostfix]
        public static void Postfix(UpgradeConsole __instance)
        {
            PDA pda = Player.main.GetPDA();
            pda.onClose = new PDA.OnClose((PDA closingPda) => PdaOverlayManager.DisconnectFromPda());
        }
    }

    [HarmonyPatch(typeof(UpgradeConsole), nameof(UpgradeConsole.UnlockDefaultModuleSlots))]
    internal static class UpgradeConsole_UnlockDefaultModuleSlots_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(UpgradeConsole __instance)
        {
            if (ModuleDisplayIconCollection.IsRegistered(__instance))
                return;

            var rotation = Quaternion.Euler(0f, 155f, 90f);

            Equipment modules = __instance.modules;

            const float topRowX = -0.124f;
            const float botRowX = -0.271f;

            const float rigtColY = 0.151f;
            const float middColY = 0f;
            const float leftColY = -rigtColY;

            const float topRowZ = 1.18f;
            const float botRowZ = 1.1f;

            Canvas moduleDisplay1 = IconCreator.CreateModuleDisplay(__instance.gameObject, new Vector3(topRowX, leftColY, topRowZ), rotation, modules.GetTechTypeInSlot("Module1"));
            Canvas moduleDisplay2 = IconCreator.CreateModuleDisplay(__instance.gameObject, new Vector3(topRowX, middColY, topRowZ), rotation, modules.GetTechTypeInSlot("Module2"));
            Canvas moduleDisplay3 = IconCreator.CreateModuleDisplay(__instance.gameObject, new Vector3(topRowX, rigtColY, topRowZ), rotation, modules.GetTechTypeInSlot("Module3"));

            Canvas moduleDisplay4 = IconCreator.CreateModuleDisplay(__instance.gameObject, new Vector3(botRowX, leftColY, botRowZ), rotation, modules.GetTechTypeInSlot("Module4"));
            Canvas moduleDisplay5 = IconCreator.CreateModuleDisplay(__instance.gameObject, new Vector3(botRowX, middColY, botRowZ), rotation, modules.GetTechTypeInSlot("Module5"));
            Canvas moduleDisplay6 = IconCreator.CreateModuleDisplay(__instance.gameObject, new Vector3(botRowX, rigtColY, botRowZ), rotation, modules.GetTechTypeInSlot("Module6"));

            ModuleDisplayIconCollection.Register(__instance, moduleDisplay1, moduleDisplay2, moduleDisplay3, moduleDisplay4, moduleDisplay5, moduleDisplay6);

            QuickLogger.Debug("Added module display icons to Cyclops engine room");
        }
    }

    [HarmonyPatch(typeof(UpgradeConsole), nameof(UpgradeConsole.OnEquip))]
    internal static class UpgradeConsole_OnEquip_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(UpgradeConsole __instance, string slot, InventoryItem item)
        {
            if (ModuleDisplayIconCollection.TryGetRegistered(__instance, out ModuleIconDisplay consoleIcons))
            {
                TechType techType = __instance.modules.GetTechTypeInSlot(slot);
                consoleIcons.EnableIcon(slot, techType);

                GameObject modulePlug = ModuleDisplayIconCollection.GetModulePlug(__instance, slot);
                modulePlug.SetActive(true);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(UpgradeConsole), nameof(UpgradeConsole.OnUnequip))]
    internal static class UpgradeConsole_OnUnequip_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(UpgradeConsole __instance, string slot, InventoryItem item)
        {
            if (ModuleDisplayIconCollection.TryGetRegistered(__instance, out ModuleIconDisplay consoleIcons))
            {
                consoleIcons.DisableIcon(slot);

                GameObject modulePlug = ModuleDisplayIconCollection.GetModulePlug(__instance, slot);
                modulePlug.SetActive(false);

                return false;
            }

            return true;
        }
    }
}
