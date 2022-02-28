namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Requests
{
    using System;

    using Newtonsoft.Json;

    internal class HsvRequest
    {
        public HsvRequest(Int32 h, Int32 s, Int32 v)
        {
            this.H = h;
            this.S = s;
            this.V = v;
        }

        [JsonProperty("hue")] public Int32 H { get; set; }

        [JsonProperty("sat")] public Int32 S { get; set; }

        [JsonProperty("brightness")] public Int32 V { get; set; }
    }
}