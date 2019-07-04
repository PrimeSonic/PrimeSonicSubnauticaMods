namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;
    using Managers;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("Awake")]
    internal class SubRoot_Awake_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(ref SubRoot __instance)
        {
            // Set up a CyclopsManager early if possible
            CyclopsManager.GetManager(__instance);
        }
    }

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdateThermalReactorCharge")]
    internal class SubRoot_UpdateThermalReactorCharge_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            bool requiresVanillaCharging = CyclopsManager.GetManager(__instance).Charge.RechargeCyclops();

            // If there is no mod taking over how thermal charging is done on the Cyclops,
            // then we will allow the original method to run so it provides the vanilla thermal charging.            
            return requiresVanillaCharging;
        }
    }

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdatePowerRating")]
    internal class SubRoot_UpdatePowerRating_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            // Performing this custom handling was necessary as UpdatePowerRating wouldn't work with the AuxUpgradeConsole
            CyclopsManager.GetManager(__instance).Engine.UpdatePowerRating();

            return false; // Completely override the method and do not continue with original execution
        }
    }

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("SetCyclopsUpgrades")]
    internal class SubRoot_SetCyclopsUpgrades_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            LiveMixin cyclopsLife = __instance.live;

            if (cyclopsLife == null || !cyclopsLife.IsAlive())
                return true; // safety check

            CyclopsManager.GetManager(__instance).Upgrade.HandleUpgrades();

            // No need to execute original method anymore
            return false; // Completely override the method and do not continue with original execution
        }
    }

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("SetExtraDepth")]
    internal class SubRoot_SetExtraDepth_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            // Providing this custom handler is necessary as SetExtraDepth wouldn't work with the AuxUpgradeConsole
            return false; // Now handled by UpgradeManager HandleUpgrades
        }
    }

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("OnPlayerEntered")]
    internal class SubRoot_OnPlayerEntered_BeQuiet
    {
        private static bool firstEventDone = false;
        private static VoiceNotificationManager voiceMgr = null;

        [HarmonyPrefix]
        public static void Prefix(ref SubRoot __instance)
        {
            if (firstEventDone)
                return;

            voiceMgr = __instance.voiceNotificationManager;
            __instance.voiceNotificationManager = null;
        }

        [HarmonyPostfix]
        public static void Postfix(ref SubRoot __instance)
        {
            if (firstEventDone)
                return;

            __instance.voiceNotificationManager = voiceMgr;
            firstEventDone = true;
        }
    }
}
