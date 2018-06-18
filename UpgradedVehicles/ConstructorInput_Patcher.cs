//namespace UpgradedVehicles
//{
//    using Harmony;
//    using UnityEngine;

//    [HarmonyPatch(typeof(ConstructorInput))]
//    [HarmonyPatch("Craft")]
//    internal static class ConstructorInput_Patcher
//    {

//        public static bool PreFix(ref ConstructorInput __instance, ref TechType techType, float duration)
//        {
//            if (techType != SeaMothMk2.TechTypeID)
//                return true;

//            Vector3 zero = Vector3.zero;
//            Quaternion identity = Quaternion.identity;
//            __instance.GetCraftTransform(techType, ref zero, ref identity);
//            if (!__instance.ReturnValidCraftingPosition(zero))
//            {
//                __instance.invalidNotification.Play();
//                return false;
//            }
//            if (!CrafterLogic.ConsumeResources(techType))
//            {
//                return false;
//            }
//            duration = 3f;
//            switch (techType)
//            {
//                case TechType.Seamoth:
//                case TechType.Exosuit:
//                    duration = 10f;
//                    break;
//                default:
//                    if (techType == TechType.RocketBase)
//                    {
//                        duration = 25f;
//                    }
//                    break;
//                case TechType.Cyclops:
//                    duration = 20f;
//                    break;
//            }
//            ((Crafter)__instance).Craft(techType, duration);
//        }

//        private static void GetCraftTransform(this ConstructorInput constructorInput, TechType techType, ref Vector3 position, ref Quaternion rotation)
//        {
//            Transform itemSpawnPoint = constructorInput.constructor.GetItemSpawnPoint(techType);
//            position = itemSpawnPoint.position;
//            rotation = itemSpawnPoint.rotation;
//        }

//        private static bool ReturnValidCraftingPosition(this ConstructorInput constructorInput, Vector3 pollPosition)
//        {
//            float num = Mathf.Clamp01((pollPosition.x + 2048f) / 4096f);
//            float num2 = Mathf.Clamp01((pollPosition.z + 2048f) / 4096f);
//            int x = (int)(num * (float)constructorInput.validCraftPositionMap.width);
//            int y = (int)(num2 * (float)constructorInput.validCraftPositionMap.height);
//            return constructorInput.validCraftPositionMap.GetPixel(x, y).g > 0.5f;
//        }

//    }
//}
