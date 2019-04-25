using Common;
using Harmony;
using UnityEngine;
using MoreCyclopsUpgrades.Monobehaviors;

namespace MoreCyclopsUpgrades.Patchers
{
    /// <summary>
    /// this class is very early stage initialized in the Cyclops
    /// </summary>    
    [HarmonyPatch(typeof(CyclopsExternalCams))]
    [HarmonyPatch("Start")]
    public class CyclopsExternalCams_Start_Patch
    {
        /// <summary>
        /// Patch entry point
        /// </summary>   
        [HarmonyPostfix]
        public static void Postfix(CyclopsExternalCams __instance)
        {
            // get Cyclops root object
            GameObject CyclopsRoot = __instance.transform.parent.gameObject;

            // check if component is exists
            if (CyclopsRoot.GetComponent<UpgradeModuleEventHandler>() == null)
            {
                // if no, adding component to the Cyclops root object
                CyclopsRoot.AddComponent<UpgradeModuleEventHandler>();
               
                QuickLogger.Message($"UgradeModuleEventHandler added to instance: {CyclopsRoot.GetInstanceID()}");
            }            
        }
    }
    
}
