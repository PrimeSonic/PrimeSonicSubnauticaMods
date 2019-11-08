namespace CyclopsAutoZapper
{
    using Common;
    using Harmony;
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

            SubRoot cyclops = __instance.GetComponentInParent<SubRoot>();

            if (cyclops == null)
            {
                QuickLogger.Error("Unable to find Cyclops SubRoot in CyclopsSonarDisplay parent");
                return;
            }

            // Did we just detect an aggressive creature on sonar?
            if (!entityData.attackCyclops.IsAggressiveTowardsCyclops(cyclops.gameObject))
                return;

            MCUServices.Find.AuxCyclopsManager<Zapper>(cyclops)?.Zap(entityData);
        }
    }

    [HarmonyPatch(typeof(CyclopsSonarDisplay))]
    [HarmonyPatch(nameof(CyclopsSonarDisplay.DistanceCheck))]
    internal static class CyclopsSonarDisplay_DistanceCheck_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(CyclopsSonarDisplay __instance)
        {
            foreach (CyclopsSonarDisplay.EntityPing entity in __instance.entitysOnSonar)
            {
                if (entity == null || entity.ping == null)
                    continue;

                CyclopsHUDSonarPing ping = entity.ping.GetComponent<CyclopsHUDSonarPing>();

                // Are there any aggressive creatures on sonar?
                if (ping.currentColor == ping.aggressiveColor)
                {
                    SubRoot cyclops = __instance.GetComponentInParent<SubRoot>();

                    if (cyclops == null)
                    {
                        QuickLogger.Error("Unable to find Cyclops SubRoot in CyclopsSonarDisplay parent");
                        return;
                    }

                    MCUServices.Find.AuxCyclopsManager<Zapper>(cyclops)?.Zap();
                }
            }
        }
    }
}
