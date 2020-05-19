namespace MoreCyclopsUpgrades
{
    using MoreCyclopsUpgrades.Managers;
    using UnityEngine;
    using UnityEngine.UI;

    internal class IconCreator
    {
        internal static Canvas CreateModuleDisplay(GameObject anchor, Vector3 position, Quaternion rotation, TechType startingState)
        {
            const float scale = 0.215f;

            Canvas canvas = new GameObject("Canvas", typeof(RectTransform)).AddComponent<Canvas>();
            Transform t = canvas.transform;
            t.SetParent(anchor.transform, false);
            canvas.sortingLayerID = 1;

            canvas.gameObject.AddComponent<uGUI_GraphicRaycaster>();

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

            if (startingState > TechType.None)
            {
                icon.sprite = SpriteManager.Get(startingState);
                icon.enabled = true;
                canvas.gameObject.SetActive(true);
            }
            else
            {
                canvas.gameObject.SetActive(false);
                icon.enabled = false;
            }

            return canvas;
        }

        internal static PowerIndicatorIcon CreatePowerIndicatorIcon(Canvas canvas, float xoffset, float yoffset, float zoffset, float scale)
        {
            var iconGo = new GameObject("IconGo", typeof(RectTransform));
            iconGo.transform.SetParent(canvas.transform, false);
            iconGo.transform.localPosition = new Vector3(xoffset, yoffset, zoffset);
            iconGo.transform.localScale = new Vector3(scale, scale, scale);

            uGUI_Icon icon = iconGo.AddComponent<uGUI_Icon>();

            var arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            var textGO = new GameObject("TextGo");

            textGO.transform.SetParent(iconGo.transform, false);

            Text text = textGO.AddComponent<Text>();
            text.font = arial;
            text.material = arial.material;
            text.text = "??";
            text.fontSize = 22;
            text.alignment = TextAnchor.LowerCenter;
            text.color = Color.white;

            Outline outline = textGO.AddComponent<Outline>();
            outline.effectColor = Color.black;

            RectTransform rectTransform = text.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.anchoredPosition += new Vector2(0f, -15f);
            return new PowerIndicatorIcon(icon, text);
        }
    }
}
