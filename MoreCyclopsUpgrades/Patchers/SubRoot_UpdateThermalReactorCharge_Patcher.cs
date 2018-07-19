namespace MoreCyclopsUpgrades
{
    using Harmony;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdateThermalReactorCharge")]
    internal class SubRoot_UpdateThermalReactorCharge_Patcher
    {
        public static bool Prefix(ref SubRoot __instance)
        {
            if (__instance.upgradeConsole == null)
            {
                return true;
            }

            Equipment modules = __instance.upgradeConsole.modules;            

            AuxUpgradeConsole[] secondaryUpgradeConsoles = __instance.GetAllComponentsInChildren<AuxUpgradeConsole>();

            PowerCharging.RechargeCyclops(ref __instance, modules, secondaryUpgradeConsoles);

            // No need to execute original method anymore.
            // Original thermal charging is handled in here now.
            __instance.thermalReactorUpgrade = false;
            
            return true;
        }

        
    }
}
