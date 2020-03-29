namespace UnSlowSeaTruck
{
    using Oculus.Newtonsoft.Json;

    [JsonObject]
    public class SeaTruckConfig
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public float SteeringMultiplier { get; set; } = 1.17f;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public float AccelerationMultiplier { get; set; } = 1.28f;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public float WeightOverride { get; set; } = 0.0001f;
    }
}
