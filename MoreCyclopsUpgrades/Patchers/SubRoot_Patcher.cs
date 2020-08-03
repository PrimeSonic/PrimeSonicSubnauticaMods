namespace MoreCyclopsUpgrades.Patchers
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using Common;
    using HarmonyLib;
    using Managers;
    using UnityEngine;

    [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.Awake))]
    internal class SubRoot_Awake_Patcher
    {
        [HarmonyPrefix]
        public static void Prefix(ref SubRoot __instance)
        {
            if (__instance.isCyclops)
            {
                // Set up a CyclopsManager early if possible
                var mgr = CyclopsManager.GetManager(ref __instance);

                // Big thanks to Waisie Milliams Hah for helping to fix the upgrade console lighting bug
                Transform consoleMesh = mgr.Cyclops.transform.Find("CyclopsMeshStatic/undamaged/cyclops_LOD0/cyclops_engine_room/cyclops_engine_console/Submarine_engine_GEO/submarine_engine_console_01_wide");

                foreach (Renderer mesh in consoleMesh.GetComponentsInChildren<Renderer>())
                {
                    SkyApplier skyApplier = mesh.gameObject.EnsureComponent<SkyApplier>();
                    skyApplier.renderers = mesh.GetComponentsInChildren<MeshRenderer>();
                    skyApplier.anchorSky = Skies.Auto;
                }
            }
        }
    }

    [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.UpdateThermalReactorCharge))]
    internal class SubRoot_UpdateThermalReactorCharge_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            var mgr = CyclopsManager.GetManager(ref __instance);

            if (mgr == null)
                return true; // Safety Check

            // All charging handled here.
            // Even vanill thermal charging was replicated to allow enable it's own Power Indicator Icon.
            mgr.Charge.RechargeCyclops();

            return false;
        }
    }

    [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.UpdatePowerRating))]
    internal class SubRoot_UpdatePowerRating_Patcher
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // This replaces the entire contents of the original method with a simple call to PowerRatingUpdate.
            QuickLogger.Debug("Transpiling SubRoot.UpdatePowerRating");
            MethodInfo powerRatingMethod = typeof(SubRoot_UpdatePowerRating_Patcher).GetMethod(nameof(SubRoot_UpdatePowerRating_Patcher.PowerRatingUpdate));

            // This is handled as a simple method call to avoid replicating the many null checks of the PowerRatingUpdate method.
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, powerRatingMethod);
            yield return new CodeInstruction(OpCodes.Ret);
        }

        public static void PowerRatingUpdate(SubRoot cyclops)
        {
            CyclopsManager.GetManager(ref cyclops)?.Engine?.UpdatePowerRating();
        }
    }

    [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.SetCyclopsUpgrades))]
    internal class SubRoot_SetCyclopsUpgrades_Patcher
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // This replaces the entire contents of the original method with a simple call to SetUpgrades.
            // No need to execute original method anymore as it wouldn't handle the AuxUpgradeConsole.

            MethodInfo setUpgradesMethod = typeof(SubRoot_SetCyclopsUpgrades_Patcher).GetMethod(nameof(SubRoot_SetCyclopsUpgrades_Patcher.SetUpgrades));

            // This is handled as a simple method call to avoid replicating the many checks of the SetUpgrades method.
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, setUpgradesMethod);
            yield return new CodeInstruction(OpCodes.Ret);
        }

        public static void SetUpgrades(SubRoot cyclops)
        {
            if (cyclops.live?.IsAlive() == true)
            {
                // If the Cyclops is alive, we can handle upgrades.
                CyclopsManager.GetManager(ref cyclops)?.Upgrade?.HandleUpgrades();
            }
        }
    }

    [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.SetExtraDepth))]
    internal class SubRoot_SetExtraDepth_Patcher
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Replaces the entire method with just a return statement, effectively making the method do nothing.
            // Replacing this with a custom handler is necessary as SetExtraDepth wouldn't work with the AuxUpgradeConsole.

            QuickLogger.Debug("Transpiling SubRoot.SetExtraDepth");
            yield return new CodeInstruction(OpCodes.Ret); // Now handled by the UpgradeManager
        }
    }

    [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.OnPlayerEntered))]
    internal class SubRoot_OnPlayerEntered_BeQuiet
    {
        // Prevent the first instance of a voice notification
        // This is to prevent the base or cyclops "no power" warning that occurs during the loading screen.
        // This is caused by this event being triggered before the power sources have been loaded.

        private static bool firstEventDone = false;
        private static VoiceNotificationManager voiceMgr = null;

        [HarmonyPrefix]
        public static void Prefix(ref SubRoot __instance)
        {
            if (firstEventDone)
                return;

            if (voiceMgr != null || __instance.voiceNotificationManager == null)
                return;

            voiceMgr = __instance.voiceNotificationManager;
            __instance.voiceNotificationManager = null;
        }

        [HarmonyPostfix]
        public static void Postfix(ref SubRoot __instance)
        {
            if (firstEventDone)
                return;

            if (voiceMgr != null && __instance.voiceNotificationManager == null)
            {
                __instance.voiceNotificationManager = voiceMgr;
            }

            firstEventDone = true;
        }
    }
}
