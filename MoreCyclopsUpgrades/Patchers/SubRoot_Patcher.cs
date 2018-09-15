namespace MoreCyclopsUpgrades.Patchers
{
    using Caching;
    using Harmony;
    using SMLHelper.V2.Utility;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("Awake")]
    internal class SubRoot_Awake_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(ref SubRoot __instance)
        {
            UpgradeManager upgradeMgr = __instance.GetComponent<UpgradeManager>();

            if (upgradeMgr is null)
            {
                upgradeMgr = __instance.gameObject.AddComponent<UpgradeManager>();
                upgradeMgr.Initialize(__instance);
                upgradeMgr.SyncUpgradeConsoles(__instance);
            }

            PowerManager powerMgr = __instance.GetComponent<PowerManager>();

            if (powerMgr is null)
            {
                powerMgr = __instance.gameObject.AddComponent<PowerManager>();
                powerMgr.Initialize(__instance, upgradeMgr);
            }

            CrushDamage crushDmg = __instance.gameObject.GetComponent<CrushDamage>();

            ComponentCache.CacheComponents(__instance, upgradeMgr, powerMgr, crushDmg);
        }
    }

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdateThermalReactorCharge")]
    internal class SubRoot_UpdateThermalReactorCharge_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            var components = ComponentCache.Find(__instance);

            components?.PowerManager?.RechargeCyclops();

            // No need to execute original method anymore
            return components == null; // Completely override the method and do not continue with original execution
        }
    }

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdatePowerRating")]
    internal class SubRoot_UpdatePowerRating_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            var components = ComponentCache.Find(__instance);

            components?.PowerManager?.UpdatePowerSpeedRating();

            // No need to execute original method anymore
            return components is null; // Completely override the method and do not continue with original execution
        }
    }

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("SetCyclopsUpgrades")]
    internal class SubRoot_SetCyclopsUpgrades_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            var cyclopsLife = (LiveMixin)__instance.GetInstanceField("live");

            if (cyclopsLife is null || !cyclopsLife.IsAlive())
                return true; // safety check

            var components = ComponentCache.Find(__instance);

            components?.UpgradeManager?.HandleUpgrades();

            // No need to execute original method anymore
            return components is null; // Completely override the method and do not continue with original execution
        }
    }

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("SetExtraDepth")]
    internal class SubRoot_SetExtraDepth_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            var components = ComponentCache.Find(__instance);

            if (components is null)
                return true; // safety check

            float orignialCrushDepth = components.CrushDamage.crushDepth;

            components.CrushDamage.SetExtraCrushDepth(components.UpgradeManager.BonusCrushDepth);

            ErrorMessage.AddMessage(Language.main.GetFormat("CrushDepthNow", components.CrushDamage.crushDepth));

            return false; // Completely override the method and do not continue with original execution
            // The original method execution sucked anyways :P
        }
    }

}
