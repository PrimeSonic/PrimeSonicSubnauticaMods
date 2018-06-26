namespace UpgradedVehicles.Patchers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using Common;
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(ConstructorInput))]
    [HarmonyPatch("Craft")]
    internal static class ConstructorInput_Patcher
    {
        // WHY WON'T THIS PATCH?!?!?!

        internal static bool Prefix(ref ConstructorInput __instance, ref TechType techType, ref float duration)
        {
            Console.WriteLine($" [UpgradedVehicles] Craft, Start with TechType:{techType}");

            Vector3 zero = Vector3.zero;
            Quaternion identity = Quaternion.identity;
            __instance.GetCraftTransform(techType, ref zero, ref identity);
            if (techType == TechType.Cyclops && !__instance.ReturnValidCraftingPosition(zero))
            {
                Console.WriteLine($" [UpgradedVehicles] Craft, Invalid with TechType:{techType}");
                __instance.invalidNotification.Play();
                return false;
            }
            if (!CrafterLogic.ConsumeResources(techType))
            {
                Console.WriteLine($" [UpgradedVehicles] Craft, Invalid resources with TechType:{techType}");
                return false;
            }
            
            switch (techType)
            {
                case TechType.RocketBase:
                    duration = 25f;
                    break;
                case TechType.Cyclops:
                    duration = 20f;
                    break;
                default:
                    duration = 10f;
                    break;
            }
            Console.WriteLine($" [UpgradedVehicles] Craft, started crafting TechType:{techType}");
            __instance.BaseCraft(techType, duration);

            return false;
        }

        internal static void BaseCraft(this Crafter cft, TechType techType, float duration)
        {
            CrafterLogic _logic = (CrafterLogic)cft.GetPrivateField("_logic");

            if (_logic != null && _logic.Craft(techType, duration))
            {
                cft.SetPrivateField("state", true);
                cft.OnCraftingBegin(techType, duration);
            }

        }

        internal static void OnCraftingBegin(this Crafter cft, TechType techType, float duration)
        {
            if (!GameInput.GetButtonHeld(GameInput.Button.Sprint))
            {
                if (duration > 5.9f)
                {
                    uGUI.main.craftingMenu.Close(cft);
                }
                else
                {
                    uGUI.main.craftingMenu.Lock(cft);
                }
            }
        }

        internal static bool ReturnValidCraftingPosition(this ConstructorInput cft, Vector3 pollPosition)
        {
            float num = Mathf.Clamp01((pollPosition.x + 2048f) / 4096f);
            float num2 = Mathf.Clamp01((pollPosition.z + 2048f) / 4096f);
            int x = (int)(num * (float)cft.validCraftPositionMap.width);
            int y = (int)(num2 * (float)cft.validCraftPositionMap.height);
            return cft.validCraftPositionMap.GetPixel(x, y).g > 0.5f;
        }

        internal static void GetCraftTransform(this ConstructorInput cft, TechType techType, ref Vector3 position, ref Quaternion rotation)
        {
            Transform itemSpawnPoint = cft.constructor.GetItemSpawnPoint(techType);
            position = itemSpawnPoint.position;
            rotation = itemSpawnPoint.rotation;
        }
    }
}

/*ORIGINAL*/
//IL_0017: ldarg.1	// Loads the argument at index 1 onto the evaluation stack.
//IL_0018: ldc.i4    2000	// Pushes a supplied value of type int32 onto the evaluation stack as an int32.
//IL_001D: beq IL_002C	// Transfers control to a target instruction if two values are equal.

//IL_0022: ldarg.1	// Loads the argument at index 1 onto the evaluation stack.
//IL_0023: ldc.i4    2001	// Pushes a supplied value of type int32 onto the evaluation stack as an int32.
//IL_0028: ceq	// 
//IL_002A: br.s IL_002D	// Unconditionally transfers control to a target instruction (short form).

//IL_002C: ldc.i4.1	// Pushes the integer value of 1 onto the evaluation stack as an int32.

//IL_002D: stloc.2	// Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 2.
//IL_002E: ldloc.2	// Loads the local variable at index 2 onto the evaluation stack.
//IL_002F: brtrue IL_004E	// Transfers control to a target instruction if value is true, not null, or non-zero.

//IL_0034: ldarg.0	// Loads the argument at index 0 onto the evaluation stack.
//IL_0035: ldloc.0	// Loads the local variable at index 0 onto the evaluation stack.
//IL_0036: call instance bool ConstructorInput::ReturnValidCraftingPosition(valuetype[UnityEngine] UnityEngine.Vector3)
//IL_003B: stloc.3	// Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 3.
//IL_003C: ldloc.3	// Loads the local variable at index 3 onto the evaluation stack.
//IL_003D: brtrue IL_004E	// Transfers control to a target instruction if value is true, not null, or non-zero.

//IL_0042: ldarg.0	// Loads the argument at index 0 onto the evaluation stack.
//IL_0043: ldfld class PDANotification ConstructorInput::invalidNotification    // Finds the value of a field in the object whose reference is currently on the evaluation stack.
//IL_0048: callvirt instance void PDANotification::Play()	// Calls a late-bound method on an object, pushing the return value onto the evaluation stack.
//IL_004D: ret  // Returns from the current method, pushing a return value (if present) from the callee's evaluation stack onto the caller's evaluation stack.
//IL_004E: ldarg.1	// Loads the argument at index 1 onto the evaluation stack.

/*REPLACED*/
//IL_0017: ldarg.1	// Loads the argument at index 1 onto the evaluation stack.
//IL_0018: ldc.i4    2000	// Pushes a supplied value of type int32 onto the evaluation stack as an int32.
//IL_001D: beq IL_002C	// Transfers control to a target instruction if two values are equal.

//IL_0022: ldarg.1	// Loads the argument at index 1 onto the evaluation stack.
//IL_0023: ldc.i4    2001	// Pushes a supplied value of type int32 onto the evaluation stack as an int32.
//IL_0028: ceq	// 
//IL_002A: br.s IL_002D	// Unconditionally transfers control to a target instruction (short form).

//IL_002C: ldc.i4.1	// Pushes the integer value of 1 onto the evaluation stack as an int32.

//IL_002D: stloc.2	// Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 2.
//IL_002E: ldloc.2	// Loads the local variable at index 2 onto the evaluation stack.
//IL_002F: brtrue IL_004E	// Transfers control to a target instruction if value is true, not null, or non-zero.

//IL_0034: ldarg.0	// Loads the argument at index 0 onto the evaluation stack.
//IL_0035: ldloc.0	// Loads the local variable at index 0 onto the evaluation stack.
//IL_0036: call instance bool ConstructorInput::ReturnValidCraftingPosition(valuetype[UnityEngine] UnityEngine.Vector3)
//IL_003B: stloc.3	// Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 3.
//IL_003C: ldloc.3	// Loads the local variable at index 3 onto the evaluation stack.
//IL_003D: brtrue IL_004E	// Transfers control to a target instruction if value is true, not null, or non-zero.

//IL_0042: ldarg.0	// Loads the argument at index 0 onto the evaluation stack.
//IL_0043: ldfld class PDANotification ConstructorInput::invalidNotification    // Finds the value of a field in the object whose reference is currently on the evaluation stack.
//IL_0048: callvirt instance void PDANotification::Play()	// Calls a late-bound method on an object, pushing the return value onto the evaluation stack.
//IL_004D: ret  // Returns from the current method, pushing a return value (if present) from the callee's evaluation stack onto the caller's evaluation stack.
//IL_004E: ldarg.1	// Loads the argument at index 1 onto the evaluation stack.
