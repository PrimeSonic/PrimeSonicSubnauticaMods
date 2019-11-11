namespace BetterBioReactor.Patchers
{
    using Common;
    using Harmony;

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch(nameof(BaseBioReactor.Start))]
    internal class Post_Start_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(BaseBioReactor __instance)
        {
            CyBioReactorMini.GetMiniReactor(__instance).UpdateInternals();
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch(nameof(BaseBioReactor.OnHover))]
    internal class OnHover_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(BaseBioReactor __instance)
        {
            CyBioReactorMini.GetMiniReactor(__instance).OnHover();

            return false; // Full override
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch(nameof(BaseBioReactor.OnUse))]
    internal class OnUse_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(BaseBioReactor __instance, ref BaseBioReactorGeometry model)
        {
            CyBioReactorMini.GetMiniReactor(__instance).OnPdaOpen(model);

            return false; // Full override            
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch(nameof(BaseBioReactor.Update))]
    internal class Update_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(BaseBioReactor __instance)
        {
            CyBioReactorMini.GetMiniReactor(__instance).UpdateDisplayText();
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch(nameof(BaseBioReactor.ProducePower))]
    internal class ProducePower_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(BaseBioReactor __instance, float requested, ref float __result)
        {
            __result = CyBioReactorMini.GetMiniReactor(__instance).ProducePower(requested);

            return false; // Full override
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch(nameof(BaseBioReactor.OnProtoDeserializeObjectTree))]
    internal class OnProtoDeserializeObjectTree_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(BaseBioReactor __instance, ProtobufSerializer serializer)
        {
            QuickLogger.Debug("OnProtoDeserializeObjectTree");
            CyBioReactorMini.GetMiniReactor(__instance).OnProtoDeserializeObjectTree(serializer);
            return false;
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch(nameof(BaseBioReactor.OnProtoDeserialize))]
    internal class OnProtoDeserialize_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(BaseBioReactor __instance, ProtobufSerializer serializer)
        {
            QuickLogger.Debug("OnProtoDeserialize");
            CyBioReactorMini.GetMiniReactor(__instance).OnProtoDeserialize(serializer);
            return false;
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch(nameof(BaseBioReactor.OnProtoSerializeObjectTree))]
    internal class OnProtoSerializeObjectTree_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(BaseBioReactor __instance, ProtobufSerializer serializer)
        {
            QuickLogger.Debug("OnProtoSerializeObjectTree");
            CyBioReactorMini.GetMiniReactor(__instance).OnProtoSerializeObjectTree(serializer);
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch(nameof(BaseBioReactor.OnProtoSerialize))]
    internal class OnProtoSerialize_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(BaseBioReactor __instance, ProtobufSerializer serializer)
        {
            QuickLogger.Debug("OnProtoSerialize");
            CyBioReactorMini.GetMiniReactor(__instance).OnProtoSerialize(serializer);
        }
    }
}
