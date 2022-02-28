namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Requests.Effects
{
    using System;

    using Newtonsoft.Json;

    [JsonObject(Title = "write")]
    public class SelectEternalModel
    {
        public Write write;
        public SelectEternalModel(String controlVersion = "v2") => this.write = new Write { ControlVersion = controlVersion, Command = "display", AnimationType = "extControl" };

        #region Nested type: Write

        [Serializable]
        public class Write
        {
            [JsonProperty("animType")] public String AnimationType;
            [JsonProperty("command")] public String Command;

            [JsonProperty("extControlVersion")] public String ControlVersion { get; set; }
        }

        #endregion
    }
}