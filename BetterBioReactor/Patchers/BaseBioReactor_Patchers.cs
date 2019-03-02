namespace BetterBioReactor.Patchers
{
    using Common;
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch("Start")]
    internal class Pre_Start_Patcher
    {
        [HarmonyPrefix]
        internal static void Prefix(BaseBioReactor __instance)
        {
            CyBioReactorMini bioMini = __instance.gameObject.AddComponent<CyBioReactorMini>();
            
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch("Start")]
    internal class Post_Start_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(BaseBioReactor __instance)
        {
            if (CyBioReactorMini.LookupMiniReactor.TryGetValue(__instance, out CyBioReactorMini bioMini))
            {
                bioMini.MaxPower = Mathf.RoundToInt(__instance._powerSource.GetMaxPower());
                bioMini.ConnectToBioRector(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch("OnHover")]
    internal class OnHover_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(BaseBioReactor __instance)
        {
            if (CyBioReactorMini.LookupMiniReactor.TryGetValue(__instance, out CyBioReactorMini bioMini))
            {
                bioMini.OnHover();

                return false; // Full override
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch("OnUse")]
    internal class OnUse_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(BaseBioReactor __instance, ref BaseBioReactorGeometry model)
        {
            if (CyBioReactorMini.LookupMiniReactor.TryGetValue(__instance, out CyBioReactorMini bioMini))
            {
                bioMini.OnPdaOpen(model);

                return false; // Full override
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch("Update")]
    internal class Update_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(BaseBioReactor __instance)
        {
            if (CyBioReactorMini.LookupMiniReactor.TryGetValue(__instance, out CyBioReactorMini bioMini))
            {
                bioMini.UpdateDisplayText();
            }
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch("ProducePower")]
    internal class ProducePower_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(BaseBioReactor __instance, float requested, ref float __result)
        {
            if (CyBioReactorMini.LookupMiniReactor.TryGetValue(__instance, out CyBioReactorMini bioMini))
            {
                __result = bioMini.ProducePower(requested);

                return false; // Full override
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch("OnProtoDeserializeObjectTree")]
    internal class OnProtoDeserializeObjectTree_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(BaseBioReactor __instance, ProtobufSerializer serializer)
        {
            if (CyBioReactorMini.LookupMiniReactor.TryGetValue(__instance, out CyBioReactorMini bioMini))
            {
                QuickLogger.Debug("OnProtoDeserializeObjectTree");
                bioMini.OnProtoDeserializeObjectTree(serializer);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch("OnProtoDeserialize")]
    internal class OnProtoDeserialize_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(BaseBioReactor __instance, ProtobufSerializer serializer)
        {
            if (CyBioReactorMini.LookupMiniReactor.TryGetValue(__instance, out CyBioReactorMini bioMini))
            {
                QuickLogger.Debug("OnProtoDeserialize");
                bioMini.OnProtoDeserialize(serializer);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch("OnProtoSerializeObjectTree")]
    internal class OnProtoSerializeObjectTree_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(BaseBioReactor __instance, ProtobufSerializer serializer)
        {
            if (CyBioReactorMini.LookupMiniReactor.TryGetValue(__instance, out CyBioReactorMini bioMini))
            {
                QuickLogger.Debug("OnProtoSerializeObjectTree");
                bioMini.OnProtoSerializeObjectTree(serializer);                
            }
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch("OnProtoSerialize")]
    internal class OnProtoSerialize_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(BaseBioReactor __instance, ProtobufSerializer serializer)
        {
            if (CyBioReactorMini.LookupMiniReactor.TryGetValue(__instance, out CyBioReactorMini bioMini))
            {
                QuickLogger.Debug("OnProtoSerialize");
                bioMini.OnProtoSerialize(serializer);
            }
        }
    }
}
