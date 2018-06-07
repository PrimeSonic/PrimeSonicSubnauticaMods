namespace MoreCyclopsUpgrades
{
    using UnityEngine;

    internal static class ThermalChargingManager
    {
        private const float ThermalChargingFactor = 1.5f;
        internal const float BatteryDrainRate = 0.01f;

        public static float GetThermalChargeAmount(ref SubRoot cyclops)
        {
            WaterTemperatureSimulation main = WaterTemperatureSimulation.main;
            float temperature = (!(main != null)) ? 0f : main.GetTemperature(cyclops.transform.position);

            float thermalCharge = cyclops.thermalReactorCharge.Evaluate(temperature) * ThermalChargingFactor;
            float thermalChargeOverTime = thermalCharge * Time.deltaTime;

            UWE.Utils.Assert(thermalChargeOverTime >= 0f, "ThermalReactorModule must produce positive amounts", cyclops);

            return thermalChargeOverTime;
        }
    }
}
