#if DEBUG
using Harmony;
using UnityEngine;

namespace IonCubeGenerator.Debug_IONCube.Patches
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch(nameof(Player.Update))]
    public class OpenDebugMenu
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.F))
            {
                DebugMenu.main.Toggle();
            }
        }
    }
}
#endif