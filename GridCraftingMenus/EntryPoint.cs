namespace GridCraftingMenus
{
    using System;
    using System.Reflection;
    using Harmony;
    using QModManager.API.ModLoading;
    using UnityEngine;

    [QModCore]
    public static class EntryPoint
    {
        [QModPatch]
        public static void Initialize()
        {
            var harmony = HarmonyInstance.Create("com.gridcraftingmenus.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Console.WriteLine($"[GridCraftingMenus:INFO]: Patched");
        }
    }

    [HarmonyPatch(typeof(uGUI_CraftNode))]
    [HarmonyPatch(nameof(uGUI_CraftNode.CreateIcon))]
    internal static class UguiCraftNodePatcher
    {
        [HarmonyPostfix]
        public static void PostFix(ref uGUI_CraftNode __instance)
        {
            PositionIconInGrid(ref __instance);
        }

        private static void PositionIconInGrid(ref uGUI_CraftNode node)
        {
            // Adapted from Below Zero disassembly
            var parent = (uGUI_CraftNode)node.parent;
            RectTransform rectTransform = parent.GetRectTransform();

            bool asGrid = UseGrid(ref parent);

            int depth = node.depth;
            float width = rectTransform.rect.width;
            float sizeModifier = 1f / Mathf.Pow(1.28f, depth - 1);
            float size = Mathf.Max(40f, 92f * sizeModifier);
            int nodesPerLine = asGrid
                ? Math.Max(1, Mathf.FloorToInt(Mathf.Sqrt(node.siblingCount)))
                : 1;
            int num3 = (node.siblingCount - 1) / nodesPerLine + 1;
            int num4 = node.index / nodesPerLine;
            int num5 = node.index - num4 * nodesPerLine;
            float x = (num5 + 0.5f) * size;
            float y = (0.5f * (num3 - 1) - num4) * size;

            x += 0.5f * width;

            node.icon.SetPosition(x, y);            
        }

        private static bool UseGrid(ref uGUI_CraftNode parent)
        {
            foreach (uGUI_CraftNode child in parent)
            {
                if (child.action == TreeAction.Expand)
                    return false;
            }
            return true;
        }
    }
}
