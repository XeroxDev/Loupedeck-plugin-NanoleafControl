namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Responses
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class Effects
    {
        [JsonProperty("select")] public String SelectedEffect { get; set; }

        [JsonProperty("effectsList")] public List<String> EffectList { get; set; }
    }
}