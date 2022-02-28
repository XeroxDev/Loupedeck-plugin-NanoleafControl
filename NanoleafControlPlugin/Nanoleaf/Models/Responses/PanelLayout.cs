namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Responses
{
    using System;
    using System.ComponentModel;

    using Newtonsoft.Json;

    /// <summary>
    ///     Layout info on a specific panel
    /// </summary>
    [Serializable]
    public class PanelLayout
    {
        /// <summary>
        ///     Unique panel ID
        /// </summary>
        [JsonProperty]
        public Int32 PanelId { get; set; }

        /// <summary>
        ///     X coordinate
        /// </summary>
        [JsonProperty]
        public Int32 X { get; set; }

        /// <summary>
        ///     Y coordinate
        /// </summary>
        [DefaultValue(10)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public Int32 Y { get; set; } = 10;

        /// <summary>
        ///     Orientation
        /// </summary>
        [JsonProperty]
        public Int32 O { get; set; }

        /// <summary>
        ///     Shape type, should probably be an enum
        /// </summary>
        [JsonProperty]
        public Int32 ShapeType { get; set; }
    }
}