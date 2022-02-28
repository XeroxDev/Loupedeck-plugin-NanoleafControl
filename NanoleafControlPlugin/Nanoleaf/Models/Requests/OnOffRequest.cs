namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Requests
{
    using System;

    using Newtonsoft.Json;

    [JsonObject(Title = "on")]
    internal class OnOffRequest
    {
        public OnOffRequest(Boolean value) => this.Value = value;

        [JsonProperty("value")] public Boolean Value { get; }
    }
}