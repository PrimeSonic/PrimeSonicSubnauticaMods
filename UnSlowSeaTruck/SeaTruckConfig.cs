namespace UnSlowSeaTruck
{
    using System.ComponentModel;
    using Oculus.Newtonsoft.Json;

    [JsonObject]
    public class SeaTruckConfig
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(1.15f)]
        public float SteeringMultiplier { get; set; } = 1.17f;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(1.25f)]
        public float AccelerationMultiplier { get; set; } = 1.28f;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0.0001f)]
        public float WeightOverride { get; set; } = 0.0001f;
    }
}
