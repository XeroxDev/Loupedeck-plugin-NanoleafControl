namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Responses
{
    using System;

    using Newtonsoft.Json;

    public class Info
    {
        [JsonProperty("name")] public String Name { get; set; }

        [JsonProperty("serialNo")] public String SerialNumber { get; set; }

        [JsonProperty("manufacturer")] public String Manufacturer { get; set; }

        [JsonProperty("firmwareVersion")] public String FirmwareVersion { get; set; }

        [JsonProperty("model")] public String Model { get; set; }

        [JsonProperty("state")] public State State { get; set; }

        [JsonProperty("effects")] public Effects Effects { get; set; }
    }
}