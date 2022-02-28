namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Requests.ColorTemperature
{
    using System;

    using Newtonsoft.Json;

    [JsonObject(Title = "ct")]
    internal class SetColorTemperatureModel
    {
        public SetColorTemperatureModel(Int32 value) => this.Value = value;

        [JsonProperty("value")] public Int32 Value { get; set; }
    }
}