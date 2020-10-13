namespace MidGameBatteries.Patchers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Common;
    using CustomBatteries.Items;
    using HarmonyLib;
    using UnityEngine;
    using static EnergyMixin;

    internal static class EnergyMixinPatcher
    {
        internal static void Patch(Harmony harmony)
        {
            QuickLogger.Debug($"{nameof(EnergyMixinPatcher)} Applying Harmony Patches");

            MethodInfo energyMixinNotifyHasBattery = AccessTools.Method(typeof(EnergyMixin), nameof(EnergyMixin.NotifyHasBattery));
            MethodInfo notifyHasBatteryPostfixMethod = AccessTools.Method(typeof(EnergyMixinPatcher), nameof(EnergyMixinPatcher.NotifyHasBatteryPostfix));

            var harmonyNotifyPostfix = new HarmonyMethod(notifyHasBatteryPostfixMethod);
            harmony.Patch(energyMixinNotifyHasBattery, postfix: harmonyNotifyPostfix); // Patches the EnergyMixin NotifyHasBattery method

            MethodInfo energyMixStartMethod = AccessTools.Method(typeof(EnergyMixin), nameof(EnergyMixin.Start));
            MethodInfo startPrefixMethod = AccessTools.Method(typeof(EnergyMixinPatcher), nameof(EnergyMixinPatcher.StartPrefix));

            var harmonyStartPrefix = new HarmonyMethod(startPrefixMethod);
            harmony.Patch(energyMixStartMethod, prefix: harmonyStartPrefix); // Patches the EnergyMixin Start method
        }

        public static void NotifyHasBatteryPostfix(ref EnergyMixin __instance, InventoryItem item)
        {
            if (CbCore.PowerCellItems.Count == 0)
                return;

            // For vehicles that show a battery model when one is equipped,
            // this will replicate the model for the normal Power Cell so it doesn't look empty

            // Null checks added on every step of the way
            TechType? itemInSlot = item?.item?.GetTechType();

            if (!itemInSlot.HasValue || itemInSlot.Value == TechType.None)
                return; // Nothing here

            TechType powerCellTechType = itemInSlot.Value;
            bool isKnownModdedPowerCell = CbCore.PowerCellItems.Find(pc => pc.TechType == powerCellTechType) != null;

            if (isKnownModdedPowerCell)
            {
                int modelToDisplay = 0; // If a matching model cannot be found, the standard PowerCell model will be used instead.
                for (int b = 0; b < __instance.batteryModels.Length; b++)
                {
                    if (__instance.batteryModels[b].techType == powerCellTechType)
                    {
                        modelToDisplay = b;
                        break;
                    }
                }

                __instance.batteryModels[modelToDisplay].model.SetActive(true);
            }
        }

        public static void StartPrefix(ref EnergyMixin __instance)
        {
            // This is necessary to allow the new batteries to be compatible with tools and vehicles

            if (!__instance.allowBatteryReplacement)
                return; // Battery replacement not allowed - No need to make changes

            if (CbCore.BatteryItems.Count == 0)
                return;

            List<TechType> compatibleBatteries = __instance.compatibleBatteries;

            List<BatteryModels> Models = new List<BatteryModels>(__instance.batteryModels);
            GameObject batteryModel = null;
            GameObject powerCellModel = null;

            TechType techType = CraftData.GetTechType(__instance.gameObject);

            bool isStasisRifle = techType == TechType.StasisRifle;
            bool isPropCannon = techType == TechType.PropulsionCannon;
            bool isRepCannon = techType == TechType.RepulsionCannon;
#if SUBNAUTICA
            bool isBlacklisted = techType == TechType.Builder;
#elif BELOWZERO
            bool isBlacklisted = techType == TechType.MetalDetector || techType == TechType.Builder;
#endif

            //if no models found but has controlled object that is a battery move object to models
            if ((Models.Count == 0 && __instance.controlledObjects.Length == 1 && !isBlacklisted) || isStasisRifle || isPropCannon || isRepCannon)
            {
                if (isStasisRifle)
                {
                    batteryModel = __instance.gameObject.transform.Find("stasis_rifle/battery_01").gameObject;
                }
                else if (isPropCannon || isRepCannon)
                {
                    GameObject batteryGeo = __instance.gameObject.transform.Find("1st person model/Propulsion_Cannon_anim/battery_geo").gameObject;
                    batteryModel = GameObject.Instantiate(batteryGeo, batteryGeo.transform.parent);

                    batteryGeo.SetActive(false);
                }
                else
                {
                    batteryModel = __instance.controlledObjects[0];
                }

                if (compatibleBatteries.Contains(TechType.Battery))
                {
                    Models.Add(new BatteryModels() { model = batteryModel, techType = TechType.Battery });

                    GameObject ionbattery = Resources.Load<GameObject>("worldentities/tools/precursorionbattery");
                    GameObject model2 = GameObject.Instantiate(batteryModel, batteryModel.transform.parent);
                    model2.name = "precursorIonBatteryModel";

                    Material ionBatteryMaterial = ionbattery?.GetComponentInChildren<MeshRenderer>()?.material;
                    if (ionBatteryMaterial != null)
                    {
                        model2.GetComponentInChildren<Renderer>().material = new Material(ionBatteryMaterial);
                    }

                    Models.Add(new BatteryModels() { model = model2, techType = TechType.PrecursorIonBattery });

                }

                if (compatibleBatteries.Contains(TechType.PowerCell))
                {
                    powerCellModel = Resources.Load<GameObject>("worldentities/tools/powercell");
                    GameObject model3 = GameObject.Instantiate(batteryModel, batteryModel.transform.parent);
                    model3.name = "PowerCellModel";

                    Material powercellMaterial = powerCellModel?.GetComponentInChildren<MeshRenderer>()?.material;
                    if (powercellMaterial != null)
                    {
                        model3.GetComponentInChildren<Renderer>().material = new Material(powercellMaterial);
                    }
                    Models.Add(new BatteryModels() { model = GameObject.Instantiate(model3, batteryModel.transform.parent), techType = TechType.PowerCell });

                    GameObject precursorIonPowerCell = Resources.Load<GameObject>("worldentities/tools/precursorionpowercell");
                    GameObject model4 = GameObject.Instantiate(batteryModel, batteryModel.transform.parent);
                    model4.name = "PrecursorIonPowerCellModel";

                    Material precursorIonPowerCellMaterial = precursorIonPowerCell?.GetComponentInChildren<MeshRenderer>()?.material;
                    if (precursorIonPowerCellMaterial != null)
                    {
                        model4.GetComponentInChildren<Renderer>().material = new Material(precursorIonPowerCellMaterial);
                    }
                    Models.Add(new BatteryModels() { model = GameObject.Instantiate(model4, batteryModel.transform.parent), techType = TechType.PrecursorIonPowerCell });
                }

                __instance.controlledObjects = new GameObject[0];

            }

            if (batteryModel is null && powerCellModel is null)
            {
                for (int b = 0; b < Models.Count; b++)
                {
                    if (Models[b].techType == TechType.Battery)
                    {
                        batteryModel = Models[b].model;
                    }

                    if (Models[b].techType == TechType.PowerCell)
                    {
                        powerCellModel = Models[b].model;
                    }
                }
            }


            if (compatibleBatteries.Contains(TechType.Battery))
            {
                // If the regular Battery is compatible with this item, then modded batteries should also be compatible
                AddMissingTechTypesToList(compatibleBatteries, CbCore.BatteryItems);
                if (batteryModel != null)
                {
                    AddCustomModels(batteryModel, ref Models, CbCore.BatteryModels);
                }
            }

            if (compatibleBatteries.Contains(TechType.PowerCell))
            {
                // If the regular Power Cell is compatible with this item, then modded power cells should also be compatible
                AddMissingTechTypesToList(compatibleBatteries, CbCore.PowerCellItems);
                if (powerCellModel != null)
                {
                    AddCustomModels(powerCellModel, ref Models, CbCore.PowerCellModels);
                }
            }

            __instance.batteryModels = Models.ToArray();
        }

        private static void AddCustomModels(GameObject originalModel, ref List<BatteryModels> Models, Dictionary<TechType, Tuple<Texture2D, Texture2D, Texture2D, Texture2D, float>> customModels)
        {
            foreach (KeyValuePair<TechType, Tuple<Texture2D, Texture2D, Texture2D, Texture2D, float>> pair in customModels)
            {
                GameObject obj = GameObject.Instantiate(originalModel, originalModel.transform.parent);
                obj.name = pair.Key.AsString() + "_model";

                Renderer renderer = obj.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    renderer.material.SetTexture(ShaderPropertyID._MainTex, pair.Value.Item1);

                    if (pair.Value.Item2 != null)
                        renderer.material.SetTexture(ShaderPropertyID._BumpMap, pair.Value.Item2);

                    if (pair.Value.Item3 != null)
                        renderer.material.SetTexture(ShaderPropertyID._SpecTex, pair.Value.Item3);

                    if (pair.Value.Item4 != null)
                    {
                        renderer.material.EnableKeyword("_EnableGlow");
                        renderer.material.SetColor("_GlowColor", Color.white);
                        renderer.material.SetTexture(ShaderPropertyID._Illum, pair.Value.Item4);
                        renderer.material.SetFloat(ShaderPropertyID._GlowStrength, pair.Value.Item5);
                        renderer.material.SetFloat(ShaderPropertyID._GlowStrengthNight, pair.Value.Item5);
                    }
                }

                Models.Add(new BatteryModels() { model = obj, techType = pair.Key });
            }
        }

        private static void AddMissingTechTypesToList(List<TechType> compatibleTechTypes, List<CbCore> toBeAdded)
        {
            for (int i = toBeAdded.Count - 1; i >= 0; i--)
            {
                TechType entry = toBeAdded[i].TechType;

                if (compatibleTechTypes.Contains(entry))
                    return;

                compatibleTechTypes.Add(entry);
            }
        }
    }
}
