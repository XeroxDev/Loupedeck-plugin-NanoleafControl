namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Requests.Hue
{
    using System;

    using Newtonsoft.Json;

    [JsonObject(Title = "hue")]
    internal class SetHueModel
    {
        public SetHueModel(Int32 value) => this.Value = value;

        [JsonProperty("value")] public Int32 Value { get; set; }
    }
}