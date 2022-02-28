namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Requests.Hue
{
    using System;

    using Newtonsoft.Json;

    [JsonObject(Title = "hue")]
    internal class IncrementHueModel
    {
        public IncrementHueModel(Int32 increment) => this.Increment = increment;

        [JsonProperty("increment")] public Int32 Increment { get; set; }
    }
}