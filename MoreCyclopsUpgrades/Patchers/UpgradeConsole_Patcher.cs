namespace MoreCyclopsUpgrades.Patchers
{
    using System.Collections.Generic;
    using Common;
    using Harmony;
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

    [HarmonyPatch(typeof(UpgradeConsole), nameof(UpgradeConsole.Awake))]
    internal static class UpgradeConsole_Awake_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(UpgradeConsole __instance)
        {
            if (UpgradeConsoleIcons.RegisteredSets.ContainsKey(__instance))
                return;

            const float topRowX = -0.12f;
            const float botRowX = -0.27f;

            const float leftColY = 0.01f + 0.155f;
            const float middColY = 0.01f + 0f;
            const float rigtColY = 0.01f + -0.155f;

            const float botRowZ = 1.1f;
            const float topRowZ = 1.18f;

            var rotation = Quaternion.Euler(0f, 150f, 90f);

            Canvas moduleDisplay4 = IconCreator.CreateModuleDisplay(__instance.gameObject, new Vector3(botRowX, rigtColY, botRowZ), rotation);
            Canvas moduleDisplay5 = IconCreator.CreateModuleDisplay(__instance.gameObject, new Vector3(botRowX, middColY, botRowZ), rotation);
            Canvas moduleDisplay6 = IconCreator.CreateModuleDisplay(__instance.gameObject, new Vector3(botRowX, leftColY, botRowZ), rotation);
            Canvas moduleDisplay1 = IconCreator.CreateModuleDisplay(__instance.gameObject, new Vector3(topRowX, rigtColY, topRowZ), rotation);
            Canvas moduleDisplay2 = IconCreator.CreateModuleDisplay(__instance.gameObject, new Vector3(topRowX, middColY, topRowZ), rotation);
            Canvas moduleDisplay3 = IconCreator.CreateModuleDisplay(__instance.gameObject, new Vector3(topRowX, leftColY, topRowZ), rotation);

            UpgradeConsoleIcons.RegisteredSets.Add(__instance,
                                    new ModuleIconDisplay(
                                        moduleDisplay1,
                                        moduleDisplay2,
                                        moduleDisplay3,
                                        moduleDisplay4,
                                        moduleDisplay5,
                                        moduleDisplay6));

            QuickLogger.Debug("Added module display icons to Cyclops engine room");
        }
    }

    internal class UpgradeConsoleIcons
    {
        public static readonly Dictionary<UpgradeConsole, ModuleIconDisplay> RegisteredSets = new Dictionary<UpgradeConsole, ModuleIconDisplay>();
    }

    [HarmonyPatch(typeof(UpgradeConsole), nameof(UpgradeConsole.SetModuleVisibility))]
    internal static class UpgradeConsole_SetModuleVisibility_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(UpgradeConsole __instance, string slot, GameObject module)
        {
            if (UpgradeConsoleIcons.RegisteredSets.TryGetValue(__instance, out ModuleIconDisplay consoleIcons))
            {
#pragma warning disable CS0618 // Type or member is obsolete
                if (module.active)
#pragma warning restore CS0618 // Type or member is obsolete
                {
                    TechType techType = __instance.modules.GetTechTypeInSlot(slot);
                    consoleIcons.EnableIcon(slot, techType);
                }
                else
                {
                    consoleIcons.DisableIcon(slot);
                }
            }
        }
    }

#if DEBUG
    [HarmonyPatch(typeof(UpgradeConsole), nameof(UpgradeConsole.OnHandHover))]
    internal static class UpgradeConsole_OnHandHover_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(UpgradeConsole __instance)
        {
            //ModuleIconDisplay displays = UpgradeConsoleIcons.RegisteredSets[__instance];

            //Canvas thing = displays.IconDisplays["Module1"];
            //PositionStuff(thing);
            //PositionStuff(displays.IconDisplays["Module2"]);
            //PositionStuff(displays.IconDisplays["Module3"]);
            //PositionStuff(displays.IconDisplays["Module4"]);
            //PositionStuff(displays.IconDisplays["Module5"]);
            //PositionStuff(displays.IconDisplays["Module6"]);
            //QuickLogger.Debug("Current position=" + thing.transform.localPosition.ToString("G5"), true);
        }

        private static bool SlowDown = false;

        // Also shamelessly copied from RandyKnapp
        // https://github.com/RandyKnapp/SubnauticaModSystem/blob/master/SubnauticaModSystem/HabitatControlPanel/HabitatControlPanel.cs#L711
        public static void PositionStuff(Canvas thing)
        {
            float amount = 0.5f;

            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                var currentDelta = new Vector3(0, amount, 0);
                Change(thing, currentDelta);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                var currentDelta = new Vector3(0, -amount, 0);
                Change(thing, currentDelta);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                var currentDelta = new Vector3(amount, 0, 0);
                Change(thing, currentDelta);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                var currentDelta = new Vector3(-amount, 0, 0);
                Change(thing, currentDelta);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                var currentDelta = new Vector3(0, 0, amount);
                Change(thing, currentDelta);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                var currentDelta = new Vector3(0, 0, -amount);
                Change(thing, currentDelta);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                thing.transform.localRotation = Quaternion.Euler(60f, 180f, 0f);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                thing.transform.localRotation = Quaternion.Euler(60f, 0f, 0f);
            }
            else
            {
                SlowDown = false;
            }
        }

        private static void Change(Canvas thing, Vector3 currentDelta)
        {
            Transform t = thing.transform;
            thing.transform.localPosition += currentDelta;
        }
    }
#endif
}
