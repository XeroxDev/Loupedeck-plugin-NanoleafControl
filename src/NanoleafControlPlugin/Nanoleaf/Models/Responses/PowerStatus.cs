namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Responses
{
    using System;

    using Newtonsoft.Json;

    public class PowerStatus
    {
        [JsonProperty("value")] public Boolean Value { get; set; }
    }
}