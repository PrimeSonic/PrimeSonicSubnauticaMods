namespace CyclopsNuclearReactor.Helpers
{    
    using UnityEngine;
    using MoreCyclopsUpgrades.API;

    public static class CyNukeRodHelper
    {
        public static GameObject Find(GameObject gameObject, int index)
        {
            if (gameObject == null)
            {
                MCUServices.Logger.Error($"CyNukeRodHelper Cannot find gameObject");
                return null;
            }

            GameObject rod = gameObject
                                .FindChild("model")
                                .FindChild("Rod_Slots")
                                .FindChild($"PowerRod_Item_{index}")?.gameObject;

            if (rod != null)
                return rod;

            MCUServices.Logger.Error($"Cannot find gameObject PowerRod_Item_{index}");
            return null;
        }

        public static void EmptyRod(GameObject gameObject, int index)
        {
            GameObject uranium = Find(gameObject, index)?.FindChild("PowerRod_Uranium")?.gameObject;

            if (uranium != null)
            {
                uranium.transform.localPosition = new Vector3(uranium.transform.localPosition.x, 0, uranium.transform.localPosition.z);
            }
        }
    }
}
