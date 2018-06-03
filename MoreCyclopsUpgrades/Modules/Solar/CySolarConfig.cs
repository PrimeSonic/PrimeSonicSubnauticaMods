namespace MoreCyclopsUpgrades
{
    using System;

    [Serializable]
    internal class CySolarConfig
    {
        /// <summary>
        /// The custom user chargerate for the Cyclops Solar Charger.
        /// </summary>
        /// <remarks>Defaults to 1.</remarks>
        public float SolarChargeRate = 1;

        public CySolarConfig()
        {
            SolarChargeRate = 1;
        }

        public CySolarConfig(float chargeRate)
        {
            SolarChargeRate = chargeRate;
        }

        public override string ToString() => $"{{ \"SolarChargeRate\":\"{0}\" }}";
    }
}
