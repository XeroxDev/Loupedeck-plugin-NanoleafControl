namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Requests.Effects
{
    using System;

    using Newtonsoft.Json;

    internal class SelectEffectModel
    {
        public SelectEffectModel(String effectName) => this.EffectName = effectName;

        [JsonProperty("select")] public String EffectName { get; set; }
    }
}