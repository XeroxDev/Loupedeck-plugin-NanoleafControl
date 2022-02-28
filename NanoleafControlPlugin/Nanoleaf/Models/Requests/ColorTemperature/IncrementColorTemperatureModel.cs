namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Requests.ColorTemperature
{
    using System;

    using Newtonsoft.Json;

    [JsonObject(Title = "ct")]
    internal class IncrementColorTemperatureModel
    {
        public IncrementColorTemperatureModel(Int32 increment) => this.Increment = increment;

        [JsonProperty("increment")] public Int32 Increment { get; set; }
    }
}