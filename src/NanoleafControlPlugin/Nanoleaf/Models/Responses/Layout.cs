namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Models.Responses
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    ///     Layout response returned from a layout request
    /// </summary>
    [Serializable]
    public class Layout
    {
        /// <summary>
        ///     Number of panels
        /// </summary>
        [JsonProperty]
        public Int32 NumPanels { get; set; }

        /// <summary>
        ///     Side of each length
        /// </summary>
        [JsonProperty]
        public Int32 SideLength { get; set; } = 1;

        /// <summary>
        ///     Array of position data
        /// </summary>
        [JsonProperty]
        public PanelLayout[] PositionData { get; set; } = Array.Empty<PanelLayout>();
    }
}