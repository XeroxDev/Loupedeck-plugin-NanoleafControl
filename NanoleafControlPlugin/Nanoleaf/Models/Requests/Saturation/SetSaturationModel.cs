namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Requests.Saturation
{
    using System;

    using Newtonsoft.Json;

    [JsonObject(Title = "sat")]
    internal class SetSaturationModel
    {
        public SetSaturationModel(Int32 value) => this.Value = value;

        [JsonProperty("value")] public Int32 Value { get; set; }
    }
}