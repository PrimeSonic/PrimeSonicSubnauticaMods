namespace DataBoxScannerFix
{
    using Common;
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(ResourceTracker), "Start")]
    internal class ResourceTracker_Patcher
    {
        [HarmonyPostfix]
        internal static void PostFix(ref ResourceTracker __instance)
        {
            bool isDataBox = __instance.overrideTechType == TechType.Databox ||
                __instance.techType == TechType.Databox;

            if (!isDataBox)
                return; // Not a data box, early exit

            var blueprint = __instance.GetComponentInParent<BlueprintHandTarget>();

            if (blueprint == null)
                return; // safety check, but shouldn't happen

            if (!blueprint.used)
                return; // blueprint still unused

            __instance.OnBreakResource(); // call this to invoke the "Unregister" method
        }
    }

    [HarmonyPatch(typeof(BlueprintHandTarget), "UnlockBlueprint")]
    internal class BlueprintHandTarget_Patcher
    {
        [HarmonyPrefix]
        internal static bool PreFix(ref BlueprintHandTarget __instance)
        {
            __instance.SendMessage("OnBreakResource", null, SendMessageOptions.DontRequireReceiver);

            return true;
        }
    }

}
