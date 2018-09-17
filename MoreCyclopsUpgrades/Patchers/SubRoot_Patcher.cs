namespace MoreCyclopsUpgrades.Patchers
{
    using Caching;
    using Common;
    using Harmony;
    using SMLHelper.V2.Utility;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdateThermalReactorCharge")]
    internal class SubRoot_UpdateThermalReactorCharge_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            PowerManager powerMgr = CyclopsManager.GetPowerManager(__instance);

            if (powerMgr == null)
            {
                return true; // safety check
            }

            powerMgr.RechargeCyclops();

            // No need to execute original method anymore
            return false; // Completely override the method and do not continue with original execution
        }
    }

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdatePowerRating")]
    internal class SubRoot_UpdatePowerRating_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            PowerManager powerMgr = CyclopsManager.GetPowerManager(__instance);

            if (powerMgr == null)
            {
                return true; // safety check
            }

            powerMgr.UpdatePowerSpeedRating();

            // No need to execute original method anymore
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
            var cyclopsLife = (LiveMixin)__instance.GetInstanceField("live");

            if (cyclopsLife == null || !cyclopsLife.IsAlive())
                return true; // safety check

            UpgradeManager upgradeMgr = CyclopsManager.GetUpgradeManager(__instance);

            if (upgradeMgr == null)
            {
                return true; // safety check
            }

            upgradeMgr.HandleUpgrades();

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
            UpgradeManager upgradeMgr = CyclopsManager.GetUpgradeManager(__instance);

            if (upgradeMgr == null)
            {
                return true;
            }

            CrushDamage crushDmg = __instance.gameObject.GetComponent<CrushDamage>();

            if (crushDmg == null)
            {
                return true;
            }

            float orignialCrushDepth = crushDmg.crushDepth;

            crushDmg.SetExtraCrushDepth(upgradeMgr.BonusCrushDepth);

            if (orignialCrushDepth != crushDmg.crushDepth)
                ErrorMessage.AddMessage(Language.main.GetFormat("CrushDepthNow", crushDmg.crushDepth));

            return false; // Completely override the method and do not continue with original execution
            // The original method execution sucked anyways :P
        }
    }
}
