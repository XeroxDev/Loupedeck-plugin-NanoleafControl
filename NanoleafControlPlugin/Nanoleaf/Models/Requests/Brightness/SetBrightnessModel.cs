namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Requests.Brightness
{
    using System;

    using Newtonsoft.Json;

    [JsonObject(Title = "brightness")]
    internal class SetBrightnessModel
    {
        public SetBrightnessModel(Int32 value, Int32 duration)
        {
            this.Value = value;
            this.Duration = duration;
        }

        [JsonProperty("value")] public Int32 Value { get; set; }

        [JsonProperty("duration")] public Int32 Duration { get; set; }
    }
}