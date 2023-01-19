namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Responses
{
    using System;

    using Newtonsoft.Json;

    public class State
    {
        [JsonProperty("on")] public Switch Switch { get; set; }

        [JsonProperty("brightness")] public Brightness Brightness { get; set; }

        [JsonProperty("hue")] public Hue Hue { get; set; }

        [JsonProperty("sat")] public Saturation Saturation { get; set; }

        [JsonProperty("ct")] public ColorTemperature ColorTemperature { get; set; }

        [JsonProperty("effect")] public String ColorMode { get; set; }
    }
}