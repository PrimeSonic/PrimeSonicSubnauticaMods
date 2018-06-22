//namespace UpgradedVehicles.Patchers
//{
//    using System;
//    using Common;
//    using Harmony;
//    using UnityEngine;

//    [HarmonyPatch(typeof(SeamothStorageInput))]
//    [HarmonyPatch("OpenPDA")]
//    internal class SeamothStorageInput_OpenPDA_Patcher
//    {
//        internal static bool Prefix(SeamothStorageInput __instance)
//        {
//            Console.WriteLine($"[UpgradedVehicles] SeamothStorageInput.OpenPDA : Start");

//            SeaMoth seamoth = __instance.seamoth;

//            var storageDeluxe = seamoth.GetComponent<SeaMothStorageDeluxe>();

//            if (storageDeluxe == null)
//                return true;

//            ItemsContainer storageInSlot = storageDeluxe.GetStorageInSlot(__instance.slotID);

//            Console.WriteLine($"[UpgradedVehicles] SeamothStorageInput.OpenPDA : storageInSlot slotID:{__instance.slotID} IsNull:{storageInSlot == null}");

//            if (storageInSlot != null)
//            {
//                PDA pda = Player.main.GetPDA();
//                Inventory.main.SetUsedStorage(storageInSlot, false);

//                Transform tr = (Transform)__instance.GetPrivateField("tr");

//                PDA.OnClose onClosePDA = delegate (PDA p)
//                {
//                    OnClosePDA(__instance, p);
//                };

//                if (!pda.Open(PDATab.Inventory, tr, new PDA.OnClose(onClosePDA), -1f))
//                {
//                    OnClosePDA(__instance, pda);
//                }
//            }
//            else
//            {
//                OnClosePDA(__instance, null);
//            }

//            Console.WriteLine($"[UpgradedVehicles] SeamothStorageInput.OpenPDA : End");
//            return false;
//        }

//        internal static void OnClosePDA(SeamothStorageInput instance, PDA pda)
//        {
//            Sequence sequence = (Sequence)instance.GetPrivateField("sequence");

//            sequence.Set(instance.timeClose, false, null);
//            Utils.PlayFMODAsset(instance.closeSound, instance.transform, 1f);
//        }

//    }
//}
