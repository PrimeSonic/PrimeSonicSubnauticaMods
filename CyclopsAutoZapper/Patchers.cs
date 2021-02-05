namespace CyclopsAutoZapper
{
    using Common;
    using CyclopsAutoZapper.Managers;
    using HarmonyLib;
    using MoreCyclopsUpgrades.API;

    [HarmonyPatch(typeof(CyclopsSonarDisplay))]
    [HarmonyPatch(nameof(CyclopsSonarDisplay.NewEntityOnSonar))]
    internal static class CyclopsSonarDisplay_NewEntityOnSonar_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(CyclopsSonarCreatureDetector.EntityData entityData, CyclopsSonarDisplay __instance)
        {
            if (entityData.entityType != CyclopsSonarDisplay.EntityType.Creature)
                return;

            if (entityData.attackCyclops == null)
                return;

            SubRoot cyclops = __instance?.noiseManager?.subRoot;

            if (cyclops == null)
            {
                QuickLogger.Error("Unable to find Cyclops SubRoot in CyclopsSonarDisplay parent", true);
                return;
            }

            // Did we just detect an aggressive creature on sonar?
            if (!entityData.attackCyclops.IsAggressiveTowardsCyclops(cyclops.gameObject))
                return;

            // Yes, both auto-defense zappers can be activated at the same time
            var defenser = MCUServices.Find.AuxCyclopsManager<AutoDefenser>(cyclops);
            if (defenser != null)
                defenser.Zap(entityData);
            else
                QuickLogger.Warning("Unable to find AutoDefenser AuxCyclopsManager", true);

            var defenserMk2 = MCUServices.Find.AuxCyclopsManager<AutoDefenserMk2>(cyclops);
            if (defenserMk2 != null)
                defenserMk2.Zap(entityData);
            else
                QuickLogger.Warning("Unable to find AutoDefenserMk2 AuxCyclopsManager", true);
        }
    }

    [HarmonyPatch(typeof(CyclopsSonarDisplay))]
    [HarmonyPatch(nameof(CyclopsSonarDisplay.DistanceCheck))]
    internal static class CyclopsSonarDisplay_DistanceCheck_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(CyclopsSonarDisplay __instance)
        {
            SubRoot cyclops = __instance?.noiseManager?.subRoot;

            if (cyclops == null)
            {
                QuickLogger.Error("Unable to find Cyclops SubRoot in CyclopsSonarDisplay parent", true);
                return;
            }

            foreach (CyclopsSonarDisplay.EntityPing entity in __instance.entitysOnSonar)
            {
                CyclopsHUDSonarPing ping = entity?.ping?.GetComponent<CyclopsHUDSonarPing>();

                if (ping == null)
                    continue;

                // Are there any aggressive creatures on sonar?
                if (ping.currentColor != ping.aggressiveColor)
                    return;

                // Yes, both auto-defense zappers can be activated at the same time
                var defenser = MCUServices.Find.AuxCyclopsManager<AutoDefenser>(cyclops);
                if (defenser != null)
                    defenser.Zap();
                else
                    QuickLogger.Warning("Unable to find AutoDefenser AuxCyclopsManager", true);

                var defenserMk2 = MCUServices.Find.AuxCyclopsManager<AutoDefenserMk2>(cyclops);
                if (defenserMk2 != null)
                    defenserMk2.Zap();
                else
                    QuickLogger.Warning("Unable to find AutoDefenserMk2 AuxCyclopsManager", true);
            }
        }
    }

    [HarmonyPatch(typeof(CyclopsHolographicHUD))]
    [HarmonyPatch(nameof(CyclopsHolographicHUD.AttachedLavaLarva))]
    internal static class CyclopsHolographicHUD_AttachedLavaLarva_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(CyclopsHolographicHUD __instance)
        {
            SubRoot cyclops = __instance?.subFire?.subRoot;

            if (cyclops != null)
            {
                var shieldPulser = MCUServices.Find.AuxCyclopsManager<ShieldPulser>(cyclops);

                if (shieldPulser != null)
                    shieldPulser.PulseShield();
                else
                    QuickLogger.Error("Unable to find ShieldPulser AuxCyclopsManager", true);
            }
        }
    }
}
