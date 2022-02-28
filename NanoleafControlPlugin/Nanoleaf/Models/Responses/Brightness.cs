namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Responses
{
    using System;

    using Newtonsoft.Json;

    public class Brightness
    {
        [JsonProperty("value")] public Int32 Value { get; set; }

        [JsonProperty("max")] public Int32 Maximum { get; set; }

        [JsonProperty("min")] public Int32 Minimum { get; set; }
    }
}