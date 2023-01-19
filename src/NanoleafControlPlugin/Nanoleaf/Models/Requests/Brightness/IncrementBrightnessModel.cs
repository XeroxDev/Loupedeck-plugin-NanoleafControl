namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Requests.Brightness
{
    using System;

    using Newtonsoft.Json;

    [JsonObject(Title = "brightness")]
    internal class IncrementBrightnessModel
    {
        public IncrementBrightnessModel(Int32 increment) => this.Increment = increment;

        [JsonProperty("increment")] public Int32 Increment { get; set; }
    }
}