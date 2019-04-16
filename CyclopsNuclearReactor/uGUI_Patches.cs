namespace CyclopsNuclearReactor
{
    using Harmony;
    using System.Collections.Generic;

    [HarmonyPatch(typeof(uGUI_InventoryTab))]
    [HarmonyPatch("OnOpenPDA")]
    internal class UGUI_InventoryTab_OnOpenPDA_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(uGUI_InventoryTab __instance)
        {
            //// This event happens whenever the player opens their PDA.
            //// We will make a series of checks to see if what they have opened is the Cyclops Bioreactor item container.

            //if (__instance == null)
            //    return; // Safety check

            //if (!Player.main.IsInSub() || !Player.main.currentSub.isCyclops)
            //    return; // If not in Cyclops then all is irrelevant

            //uGUI_Equipment equipmentUI = __instance.equipment;

            //if (equipmentUI == null)
            //    return; // Not an equipment container

            //var equipment = (Equipment)AccessTools.Field(typeof(Equipment), "equipment").GetValue(equipmentUI);

            //if (equipment == null)
            //    return; // Safety check

            //string label = (equipment as IItemsContainer).label;

            //if (label != CyNukReactorSMLHelper.EquipmentLabel())
            //    return; // Not a CyNukReactor

            //List<CyNukeReactorMono> reactors = CyNukeChargeManager.GetReactors(Player.main.currentSub);

            //if (reactors == null || reactors.Count == 0)
            //    return; // Cyclops has no bioreactors

            //// Look for the reactor that matches the container we just opened.
            //CyNukeReactorMono reactor = reactors.Find(r => r.RodSlots == equipment);

            //if (reactor == null)
            //    return; // Didn't find the reactor we were looking for. Could it be on another cyclops?

            //var lookup = (Dictionary<InventoryItem, uGUI_EquipmentSlot>)AccessTools.Field(typeof(uGUI_InventoryTab), "items").GetValue(equipmentUI);
            //reactor.ConnectToEquipment(lookup); // Found!
        }
    }
}
