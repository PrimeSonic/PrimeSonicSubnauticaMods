namespace MidGameBatteries.Patchers
{
    using System.Collections.Generic;
    using System.Reflection;
    using Common;
    using CustomBatteries.API;
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

            MethodInfo energyMixStartMethod = AccessTools.Method(typeof(EnergyMixin), nameof(EnergyMixin.Awake));
            MethodInfo startPrefixMethod = AccessTools.Method(typeof(EnergyMixinPatcher), nameof(EnergyMixinPatcher.AwakePrefix));

            var harmonyStartPrefix = new HarmonyMethod(startPrefixMethod);
            harmony.Patch(energyMixStartMethod, prefix: harmonyStartPrefix); // Patches the EnergyMixin Start method
        }

        public static void NotifyHasBatteryPostfix(ref EnergyMixin __instance, InventoryItem item)
        {
            if (CbDatabase.PowerCellItems.Count == 0)
                return;

            // For vehicles that show a battery model when one is equipped,
            // this will replicate the model for the normal Power Cell so it doesn't look empty

            // Null checks added on every step of the way
            TechType? itemInSlot = item?.item?.GetTechType();

            if (!itemInSlot.HasValue || itemInSlot.Value == TechType.None)
                return; // Nothing here

            TechType powerCellTechType = itemInSlot.Value;
            bool isKnownModdedPowerCell = CbDatabase.PowerCellItems.Find(pc => pc.TechType == powerCellTechType) != null;

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

        public static void AwakePrefix(ref EnergyMixin __instance)
        {
            // This is necessary to allow the new batteries to be compatible with tools and vehicles

            if (!__instance.allowBatteryReplacement)
                return; // Battery replacement not allowed - No need to make changes

            if (CbDatabase.BatteryItems.Count == 0)
                return;

            List<TechType> compatibleBatteries = __instance.compatibleBatteries;

            List<BatteryModels> Models = new List<BatteryModels>(__instance.batteryModels);
            GameObject batteryModel = null;
            GameObject powerCellModel = null;
            GameObject ionBatteryModel = null;
            GameObject ionPowerCellModel = null;

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

            List<TechType> existingModels = new List<TechType>();

            for (int b = 0; b < Models.Count; b++)
            {
                if (Models[b].techType == TechType.Battery)
                {
                    batteryModel = Models[b].model;
                }

                if (Models[b].techType == TechType.PrecursorIonBattery)
                {
                    ionBatteryModel = Models[b].model;
                }

                if (Models[b].techType == TechType.PowerCell)
                {
                    powerCellModel = Models[b].model;
                }

                if (Models[b].techType == TechType.PrecursorIonPowerCell)
                {
                    ionPowerCellModel = Models[b].model;
                }
                existingModels.Add(Models[b].techType);
            }

            if (compatibleBatteries.Contains(TechType.Battery) || compatibleBatteries.Contains(TechType.PrecursorIonBattery))
            {
                // If the regular Battery is compatible with this item, then modded batteries should also be compatible
                AddMissingTechTypesToList(compatibleBatteries, CbDatabase.BatteryItems);
                if (batteryModel != null && ionBatteryModel != null)
                {
                    AddCustomModels(batteryModel, ionBatteryModel, ref Models, CbDatabase.BatteryModels, existingModels);
                }
            }

            if (compatibleBatteries.Contains(TechType.PowerCell) || compatibleBatteries.Contains(TechType.PrecursorIonPowerCell))
            {
                // If the regular Power Cell is compatible with this item, then modded power cells should also be compatible
                AddMissingTechTypesToList(compatibleBatteries, CbDatabase.PowerCellItems);
                if (powerCellModel != null && ionPowerCellModel != null)
                {
                    AddCustomModels(powerCellModel, ionPowerCellModel, ref Models, CbDatabase.PowerCellModels, existingModels);
                }
            }

            __instance.batteryModels = Models.ToArray();
        }

        private static void AddCustomModels(GameObject originalModel, GameObject ionModel, ref List<BatteryModels> Models, Dictionary<TechType, CBModelData> customModels, List<TechType> existingModels)
        {
            foreach (KeyValuePair<TechType, CBModelData> pair in customModels)
            {
                //dont add models that already exist.
                if (existingModels.Contains(pair.Key))
                    continue;

                GameObject modelBase = (pair.Value?.UseIonModelsAsBase ?? false) ? ionModel : originalModel;
                GameObject obj = GameObject.Instantiate(modelBase, modelBase.transform.parent);
                obj.name = pair.Key.AsString() + "_model";

                if (pair.Value != null)
                {
                    Renderer renderer = obj.GetComponentInChildren<Renderer>();
                    if (renderer != null)
                    {
                        if (pair.Value.CustomTexture != null)
                            renderer.material.SetTexture(ShaderPropertyID._MainTex, pair.Value.CustomTexture);

                        if (pair.Value.CustomNormalMap != null)
                            renderer.material.SetTexture(ShaderPropertyID._BumpMap, pair.Value.CustomNormalMap);

                        if (pair.Value.CustomSpecMap != null)
                            renderer.material.SetTexture(ShaderPropertyID._SpecTex, pair.Value.CustomSpecMap);

                        if (pair.Value.CustomIllumMap != null)
                        {
                            renderer.material.SetTexture(ShaderPropertyID._Illum, pair.Value.CustomIllumMap);
                            renderer.material.SetFloat(ShaderPropertyID._GlowStrength, pair.Value.CustomIllumStrength);
                            renderer.material.SetFloat(ShaderPropertyID._GlowStrengthNight, pair.Value.CustomIllumStrength);
                        }
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
