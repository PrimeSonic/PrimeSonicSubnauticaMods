namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;
    using MoreCyclopsUpgrades.Managers;
    using UnityEngine;
    using UnityEngine.UI;

    [HarmonyPatch(typeof(UpgradeConsole))]
    [HarmonyPatch("InitializeModules")]
    internal static class UpgradeConsole_InitializeModules_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(UpgradeConsole __instance)
        {
            const float topRowY = 1.15f;//-0.109f;
            const float botRowY = 1.075f;//-0.239f;

            const float leftColX = 0.15f;//0.159f;
            const float middColX = 0f;//0f;
            const float rightColX = -0.15f;//-0.152f;

            const float topRowZ = 0.12f;// 1.146f;
            const float botRowZ = 0.270f;//1.06f;

            var rotation = Quaternion.Euler(60f, 180, 0);

            Canvas moduleDisplay1 = CreateModuleDisplay(__instance, new Vector3(rightColX, botRowY, botRowZ), rotation);
            Canvas moduleDisplay2 = CreateModuleDisplay(__instance, new Vector3(middColX, botRowY, botRowZ), rotation);
            Canvas moduleDisplay3 = CreateModuleDisplay(__instance, new Vector3(leftColX, botRowY, botRowZ), rotation);
            Canvas moduleDisplay4 = CreateModuleDisplay(__instance, new Vector3(rightColX, topRowY, topRowZ), rotation);
            Canvas moduleDisplay5 = CreateModuleDisplay(__instance, new Vector3(middColX, topRowY, topRowZ), rotation);
            Canvas moduleDisplay6 = CreateModuleDisplay(__instance, new Vector3(leftColX, topRowY, topRowZ), rotation);

            moduleDisplay1.transform.SetParent(__instance.module1.transform, false);
            moduleDisplay2.transform.SetParent(__instance.module2.transform, false);
            moduleDisplay3.transform.SetParent(__instance.module3.transform, false);
            moduleDisplay4.transform.SetParent(__instance.module4.transform, false);
            moduleDisplay5.transform.SetParent(__instance.module5.transform, false);
            moduleDisplay6.transform.SetParent(__instance.module6.transform, false);
        }

        private static Canvas CreateModuleDisplay(UpgradeConsole parent, Vector3 position, Quaternion rotation)
        {
            const float scale = 0.215f;

            Canvas canvas = new GameObject("Canvas", typeof(RectTransform)).AddComponent<Canvas>();
            Transform t = canvas.transform;
            t.SetParent(parent.transform, false);
            canvas.sortingLayerID = 1;

            uGUI_GraphicRaycaster raycaster = canvas.gameObject.AddComponent<uGUI_GraphicRaycaster>();

            var rt = t as RectTransform;
            RectTransformExtensions.SetParams(rt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            RectTransformExtensions.SetSize(rt, 0.5f, 0.5f);

            t.localPosition = position;
            t.localRotation = rotation;
            t.localScale = new Vector3(scale, scale, scale);

            canvas.scaleFactor = 0.01f;
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.referencePixelsPerUnit = 100;

            CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 20;

            uGUI_Icon icon = canvas.gameObject.AddComponent<uGUI_Icon>();
            icon.enabled = false;
            return canvas;
        }
    }

    [HarmonyPatch(typeof(UpgradeConsole))]
    [HarmonyPatch("UpdateVisuals")]
    internal static class UpgradeConsole_UpdateVisuals_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(UpgradeConsole __instance)
        {
            SetModuleVisibility(__instance.modules, "Module1", __instance.module1);
            SetModuleVisibility(__instance.modules, "Module2", __instance.module2);
            SetModuleVisibility(__instance.modules, "Module3", __instance.module3);
            SetModuleVisibility(__instance.modules, "Module4", __instance.module4);
            SetModuleVisibility(__instance.modules, "Module5", __instance.module5);
            SetModuleVisibility(__instance.modules, "Module6", __instance.module6);
        }

        private static void SetModuleVisibility(Equipment modules, string slot, GameObject module)
        {
            if (modules == null || module == null)
                return;

            TechType techType = modules.GetTechTypeInSlot(slot);

            bool hasItem = techType != TechType.None;

            uGUI_Icon icon = module.GetComponentInChildren<uGUI_Icon>();

            if (icon == null)
                return;

            if (hasItem)
            {
                Atlas.Sprite atlasSprite = SpriteManager.Get(techType);

                if (atlasSprite != null)
                    icon.sprite = atlasSprite;
            }
            else
            {
                icon.sprite = null; // Clear the sprite when empty                
            }

            icon.enabled = hasItem;
        }
    }

    [HarmonyPatch(typeof(UpgradeConsole))]
    [HarmonyPatch("OnHandClick")]
    internal static class UpgradeConsole_OnHandClick_Patcher
    {
        [HarmonyPrefix]
        public static void Prefix(UpgradeConsole __instance)
        {
            PdaOverlayManager.StartConnectingToPda(__instance.modules);
        }

        [HarmonyPostfix]
        public static void Postfix(UpgradeConsole __instance)
        {
            PDA pda = Player.main.GetPDA();
            pda.onClose = new PDA.OnClose((PDA closingPda) => PdaOverlayManager.DisconnectFromPda());
        }        
    }
}
