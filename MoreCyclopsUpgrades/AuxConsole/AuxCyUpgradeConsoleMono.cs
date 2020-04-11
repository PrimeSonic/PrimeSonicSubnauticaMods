namespace MoreCyclopsUpgrades.AuxConsole
{
    using Common;
    using MoreCyclopsUpgrades.API.Buildables;
    using ProtoBuf;
    using UnityEngine;
    using UnityEngine.UI;

    [ProtoContract]
    internal class AuxCyUpgradeConsoleMono : AuxiliaryUpgradeConsole
    {
        protected override void UpdateVisuals()
        {
            if (ModuleDisplay1 == null)
                AddModuleSpriteHandlers();

            SetModuleVisibility("Module1", ModuleDisplay1);
            SetModuleVisibility("Module2", ModuleDisplay2);
            SetModuleVisibility("Module3", ModuleDisplay3);
            SetModuleVisibility("Module4", ModuleDisplay4);
            SetModuleVisibility("Module5", ModuleDisplay5);
            SetModuleVisibility("Module6", ModuleDisplay6);
        }

        private void AddModuleSpriteHandlers()
        {
            const float topRowY = 1.15f;//-0.109f;
            const float botRowY = 1.075f;//-0.239f;

            const float leftColX = 0.15f;//0.159f;
            const float middColX = 0f;//0f;
            const float rightColX = -0.15f;//-0.152f;

            const float topRowZ = 0.12f;// 1.146f;
            const float botRowZ = 0.270f;//1.06f;

            var rotation = Quaternion.Euler(60f, 180, 0);

            ModuleDisplay1 = CreateModuleDisplay(new Vector3(rightColX, botRowY, botRowZ), rotation);
            ModuleDisplay2 = CreateModuleDisplay(new Vector3(middColX, botRowY, botRowZ), rotation);
            ModuleDisplay3 = CreateModuleDisplay(new Vector3(leftColX, botRowY, botRowZ), rotation);
            ModuleDisplay4 = CreateModuleDisplay(new Vector3(rightColX, topRowY, topRowZ), rotation);
            ModuleDisplay5 = CreateModuleDisplay(new Vector3(middColX, topRowY, topRowZ), rotation);
            ModuleDisplay6 = CreateModuleDisplay(new Vector3(leftColX, topRowY, topRowZ), rotation);
        }

        private GameObject CreateModuleDisplay(Vector3 position, Quaternion rotation)
        {
            const float scale = 0.215f;

            Canvas canvas = new GameObject("Canvas", typeof(RectTransform)).AddComponent<Canvas>();
            Transform t = canvas.transform;
            t.SetParent(this.transform, false);
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
            return canvas.gameObject;
        }

        private void SetModuleVisibility(string slot, GameObject canvasObject)
        {
            if (canvasObject == null)
                return;

            uGUI_Icon icon = canvasObject.GetComponent<uGUI_Icon>();

            if (icon == null)
                return;

            TechType techType = this.Modules.GetTechTypeInSlot(slot);
            bool hasItem = techType != TechType.None;

            if (hasItem)
            {
                Atlas.Sprite atlasSprite = SpriteManager.Get(techType);

                if (atlasSprite == null)
                    QuickLogger.Debug($"sprite for {canvasObject.name} was null when it should not have been", true);

                icon.sprite = atlasSprite;
            }
            else
            {
                icon.sprite = null; // Clear the sprite when empty
            }

            canvasObject.SetActive(hasItem);
            icon.enabled = hasItem;
        }

        public GameObject ModuleDisplay1;

        public GameObject ModuleDisplay2;

        public GameObject ModuleDisplay3;

        public GameObject ModuleDisplay4;

        public GameObject ModuleDisplay5;

        public GameObject ModuleDisplay6;
    }
}

