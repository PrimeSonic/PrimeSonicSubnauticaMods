namespace MoreCyclopsUpgrades
{
    using UnityEngine;

    internal static class ThermalChargingManager
    {
        public const float MaxMk2Charge = 100f;
        private const float NoCharge = 0f;
        private const float BaseChargingFactor = 1.25f;
        private const float BaseDrainRate = 0.01f;

        public static float GetThermalChargeAmount(ref SubRoot cyclops)
        {
            WaterTemperatureSimulation main = WaterTemperatureSimulation.main;
            float temperature = (!(main != null)) ? 0f : main.GetTemperature(cyclops.transform.position);

            float thermalCharge = cyclops.thermalReactorCharge.Evaluate(temperature) * BaseChargingFactor;
            float thermalChargeOverTime = thermalCharge * Time.deltaTime;

            UWE.Utils.Assert(thermalChargeOverTime >= 0f, "ThermalReactorModule must produce positive amounts", cyclops);

            return thermalChargeOverTime;
        }

        public static void ChargeFromThermalMk2(ref SubRoot cyclops, Equipment modules, float thermalChargeAmount, string slotName, ref float powerDeficit)
        {
            if (thermalChargeAmount <= 0)
            {
                BatteryChargeManager.DrainBattery(ref cyclops, modules, slotName, BaseDrainRate, ref powerDeficit);
            }
            else
            {
                cyclops.powerRelay.AddEnergy(thermalChargeAmount, out float amtStored);
                powerDeficit = Mathf.Max(0f, powerDeficit - thermalChargeAmount);

                BatteryChargeManager.ChargeBattery(modules, slotName, thermalChargeAmount);
            }
        }
    }
}
