namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Responses
{
    using System;

    using Newtonsoft.Json;

    public class UserToken
    {
        [JsonProperty("auth_token")] public String Token { get; set; }
    }
}