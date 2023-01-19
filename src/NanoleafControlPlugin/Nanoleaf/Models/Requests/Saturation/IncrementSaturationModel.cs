namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Requests.Saturation
{
    using System;

    using Newtonsoft.Json;

    [JsonObject(Title = "sat")]
    internal class IncrementSaturationModel
    {
        public IncrementSaturationModel(Int32 increment) => this.Increment = increment;

        [JsonProperty("increment")] public Int32 Increment { get; set; }
    }
}