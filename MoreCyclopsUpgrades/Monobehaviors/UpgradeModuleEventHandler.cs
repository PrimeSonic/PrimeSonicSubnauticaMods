using UnityEngine;
using UWE;

namespace MoreCyclopsUpgrades.Monobehaviors
{       
    public struct UpgradeInfo
    {        
        public UpgradeInfo(SubRoot Cyclops, TechType TechType) : this()
        {
            cyclops = Cyclops;
            techType = TechType;
        }

        public SubRoot cyclops { get; private set; }
        public TechType techType { get; private set; }
    }

    /// <summary>
    /// This component will add every Cyclops root object <see cref="UpgradeModuleEventHandler"/>.
    /// For get handler component simply call a <see cref="GameObject.GetComponent{T}"/> extension and ready to handle upgrade events.
    /// </summary>
    public class UpgradeModuleEventHandler : MonoBehaviour
    {
        // modders can add a handlers to these events simply.

        // only need to put these lines the mod MonoBehaviour Awake() or Start() method

        // Example:
        // getting the event handler component in this Cyclops:
        // upgradeModuleEventHandler = SubRoot.gameObject.GetComponent<UpgradeModuleEventHandler>();

        // adding a clearing handler with parameter this Cyclops
        // upgradeModuleEventHandler.onUpgradeModuleRemove.AddHandler(this, new Event<SubRoot>.HandleFunction(OnUpgradeModuleRemove));

        // adding an adding handler with parameter UpgradeInfo struct to this Cyclops
        // upgradeModuleEventHandler.onUpgradeModuleAdd.AddHandler(this, new Event<UpgradeInfo>.HandleFunction(onUpgradeModuleAdd));

        // removing method example
        // public void OnUpgradeModuleRemove(SubRoot cyclops)
        // {
        //   any operation to handling a removing event
        // }

        // adding method eaxample
        // public void OnUpgradeModuleAdd(UpgradeInfo upgradeInfo)
        // {
        //      check the subroot and the techtype
        //  if (upgradeInfo.cyclops == 'This Cyclops' && upgradeInfo.techType == 'the added techtype')
        //  {
        //     any function to handling the adding event
        //  }
        // }

        /// <summary>
        /// This event is trigger when any module add for this Cyclops upgradeconsoles. <see cref="UpgradeInfo"/>       
        /// </summary>
        public Event<UpgradeInfo> onUpgradeModuleAdd = new Event<UpgradeInfo>();

        /// <summary>
        /// This event is trigger when any module added or removed from this Cyclops upgradeconsoles.        
        /// </summary>       
        public Event<SubRoot> onUpgradeModuleRemove = new Event<SubRoot>();
    }
}
