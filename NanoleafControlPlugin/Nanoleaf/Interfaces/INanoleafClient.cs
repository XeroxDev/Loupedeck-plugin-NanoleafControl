namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Models.Responses;

    /// <summary>
    ///     Nanoleaf API interface
    /// </summary>
    public interface INanoleafClient : IDisposable
    {
        /// <summary>
        ///     Gets nanoleaf information.
        /// </summary>
        /// <returns>Nanoleaf device information.</returns>
        Task<Info> GetInfoAsync();

        /// <summary>  Adds the user asynchronous.</summary>
        /// <returns>
        ///     The task object representing the asynchronous operation.
        ///     The task result contains a User Token.
        /// </returns>
        Task<UserToken> CreateTokenAsync();

        /// <summary>
        ///     Authorize all following request for this device.
        /// </summary>
        /// <param name="token">User token</param>
        void Authorize(String token);

        /// <summary>Deletes the user asynchronous.</summary>
        /// <param name="userToken">The user token.</param>
        Task DeleteTokenAsync(String userToken);

        /// <summary>
        ///     Get color mode
        /// </summary>
        /// <returns></returns>
        Task<String> GetColorModeAsync();

        /// <summary>
        ///     Set color using Hsv
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        Task SetHsvAsync(Int32 h, Int32 s, Int32 v);

        /// <summary>
        ///     Set color using Rgb
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        Task SetRgbAsync(Int32 r, Int32 g, Int32 b);

        #region Power

        /// <summary>
        ///     Get power status.
        /// </summary>
        /// <returns>True/False - On/Off</returns>
        Task<Boolean> GetPowerStatusAsync();

        /// <summary>
        ///     Turn on device
        /// </summary>
        /// <returns>Task that represents the asynchronous operation.</returns>
        Task TurnOnAsync();

        /// <summary>
        ///     Turn off device
        /// </summary>
        /// <returns>Task that represents the asynchronous operation.</returns>
        Task TurnOffAsync();

        #endregion

        #region Brightness

        /// <summary>
        ///     Get brightness information.
        /// </summary>
        /// <returns>Brightness information.</returns>
        Task<Brightness> GetBrightnessInfoAsync();

        /// <summary>
        ///     Get current brightness value
        /// </summary>
        /// <returns>Brightness value from 0 to 100.</returns>
        Task<Int32> GetBrightnessAsync();

        /// <summary>
        ///     Get maximum brightness value.
        /// </summary>
        /// <returns>Maximum brightness value.</returns>
        Task<Int32> GetBrightnessMaxValueAsync();

        /// <summary>
        ///     Get minimum brightness value.
        /// </summary>
        /// <returns>Minimum brightness value.</returns>
        Task<Int32> GetBrightnessMinValueAsync();

        /// <summary>
        ///     Set brightness.
        /// </summary>
        /// <param name="targetBrightness">brightness value</param>
        /// <param name="time">transition time in seconds</param>
        Task SetBrightnessAsync(Int32 targetBrightness, Int32 time = 0);

        /// <summary>
        ///     Raise brightness.
        /// </summary>
        /// <param name="value">Brightness increment.</param>
        Task RaiseBrightnessAsync(Int32 value);

        /// <summary>
        ///     Lower brightness
        /// </summary>
        /// <param name="value">Brightness decrement.</param>
        Task LowerBrightnessAsync(Int32 value);

        #endregion

        #region Hue

        /// <summary>
        ///     Get hue information
        /// </summary>
        /// <returns>Hue information</returns>
        Task<Hue> GetHueInfoAsync();

        /// <summary>
        ///     Get current hue value.
        /// </summary>
        /// <returns>Hue value from 0 to 360.</returns>
        Task<Int32> GetHueAsync();

        /// <summary>
        ///     Get max hue value.
        /// </summary>
        /// <returns>Max hue value.</returns>
        Task<Int32> GetHueMaxValueAsync();

        /// <summary>
        ///     Get min hue value.
        /// </summary>
        /// <returns>Min hue value.</returns>
        Task<Int32> GetHueMinValueAsync();

        /// <summary>
        ///     Set hue.
        /// </summary>
        /// <param name="targetHue">Target hue value.</param>
        Task SetHueAsync(Int32 targetHue);

        /// <summary>
        ///     Raise hue value.
        /// </summary>
        /// <param name="value">Hue increment value</param>
        Task RaiseHueAsync(Int32 value);

        /// <summary>
        ///     Lower hue value.
        /// </summary>
        /// <param name="value">Hue decrement value.</param>
        Task LowerHueAsync(Int32 value);

        #endregion

        #region Saturation

        /// <summary>
        ///     Get saturation information.
        /// </summary>
        /// <returns>Saturation information.</returns>
        Task<Saturation> GetSaturationInfoAsync();

        /// <summary>
        ///     Get current saturation value.
        /// </summary>
        /// <returns>Saturation value from 0 to 100.</returns>
        Task<Int32> GetSaturationAsync();

        /// <summary>
        ///     Get maximum saturation value.
        /// </summary>
        /// <returns>Max saturation value.</returns>
        Task<Int32> GetSaturationMaxValueAsync();

        /// <summary>
        ///     Get minimum saturation value.
        /// </summary>
        /// <returns>Min saturation value.</returns>
        Task<Int32> GetSaturationMinValueAsync();

        /// <summary>
        ///     Set saturation
        /// </summary>
        /// <param name="targetSat">Target saturation value.</param>
        Task SetSaturationAsync(Int32 targetSat);

        /// <summary>
        ///     Raise saturation by number.
        /// </summary>
        /// <param name="value">Saturation increment value.</param>
        Task RaiseSaturationAsync(Int32 value);

        /// <summary>
        ///     Lower saturation by number.
        /// </summary>
        /// <param name="value">Saturation decrement value.</param>
        Task LowerSaturationAsync(Int32 value);

        #endregion

        #region Color Temperature

        /// <summary>
        ///     Get color temperature information.
        /// </summary>
        /// <returns>Color temperature information.</returns>
        Task<ColorTemperature> GetTemperatureInfoAsync();

        /// <summary>
        ///     Get current color temperature value.
        /// </summary>
        /// <returns>Current color temperature value.</returns>
        Task<Int32> GetColorTemperatureAsync();

        /// <summary>
        ///     Get maximum color temperature.
        /// </summary>
        /// <returns>Max color temperature.</returns>
        Task<Int32> GetColorTemperatureMaxValueAsync();

        /// <summary>
        ///     Get minimum color temperature.
        /// </summary>
        /// <returns>Min color temperature.</returns>
        Task<Int32> GetColorTemperatureMinValueAsync();

        /// <summary>
        ///     Set color temperature.
        /// </summary>
        /// <param name="targetCt">Target color temperature.</param>
        Task SetColorTemperatureAsync(Int32 targetCt);

        /// <summary>
        ///     Raise color temperature by value.
        /// </summary>
        /// <param name="value">Color temperature increment value.</param>
        Task RaiseColorTemperatureAsync(Int32 value);

        /// <summary>
        ///     Lower color temperature by value.
        /// </summary>
        /// <param name="value">Color temperature decrement value.</param>
        Task LowerColorTemperatureAsync(Int32 value);

        #endregion

        #region Effects

        /// <summary>
        /// </summary>
        /// <returns></returns>
        Task<String> GetCurrentEffectAsync();

        /// <summary>
        /// </summary>
        /// <returns></returns>
        Task<List<String>> GetEffectsAsync();

        /// <summary>
        /// </summary>
        /// <param name="effectName"></param>
        /// <returns></returns>
        Task SetEffectAsync(String effectName);

        #endregion
    }
}