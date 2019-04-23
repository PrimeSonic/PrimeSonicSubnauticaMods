using Common;
using UnityEngine;

namespace CyclopsNuclearReactor.Helpers
{
    public static class CyNukeRodHelper
    {
        public static GameObject Find(GameObject gameObject, int index)
        {
            if (gameObject == null)
            {
                QuickLogger.Error($"CyNukeRodHelper Cannot find gameObject");
                return null;
            }

            var rod = gameObject.FindChild("model").FindChild("Rod_Slots").FindChild($"PowerRod_Item_{index}")
                ?.gameObject;

            if (rod != null) return rod;

            QuickLogger.Error($"Cannot find gameObject PowerRod_Item_{index}");
            return null;
        }

        public static void EmptyRod(GameObject gameObject, int index)
        {
            var rod = Find(gameObject, index);
            rod.transform.localPosition = Vector3.zero;
        }
    }
}
