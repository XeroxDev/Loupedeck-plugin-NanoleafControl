namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Responses
{
    using System;

    using Newtonsoft.Json;

    public class Switch
    {
        [JsonProperty("value")] public Boolean Power { get; set; }
    }
}