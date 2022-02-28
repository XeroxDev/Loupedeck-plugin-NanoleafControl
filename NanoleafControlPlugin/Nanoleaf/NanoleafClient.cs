namespace Loupedeck.NanoleafControlPlugin.Nanoleaf
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Colors;

    using Helpers;

    using Interfaces;

    using Models.Requests;
    using Models.Requests.Brightness;
    using Models.Requests.ColorTemperature;
    using Models.Requests.Effects;
    using Models.Requests.Hue;
    using Models.Requests.Saturation;
    using Models.Responses;

    using Newtonsoft.Json;

    /// <inheritdoc />
    public class NanoleafClient : INanoleafClient
    {
        private readonly NanoleafHttpClient _nanoleafHttpClient;
        private Boolean _isDisposed;

        /// <summary>
        ///     Create a new nanoleaf client
        /// </summary>
        /// <param name="host">Hostname or IP address of nanoleaf device</param>
        /// <param name="userToken">(Optional) User token to use if authorized</param>
        public NanoleafClient(String host, String userToken = "")
        {
            this._nanoleafHttpClient = new NanoleafHttpClient(host, userToken);
            this.HostName = host;
        }

        /// <summary>
        ///     Allow us to retrieve the device hostname for later
        /// </summary>
        public String HostName { get; }

        #region INanoleafClient Members

        /// <inheritdoc />
        public void Authorize(String token) => this._nanoleafHttpClient.AuthorizeRequests(token);

        /// <inheritdoc />
        public async Task<Info> GetInfoAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest();
            var info = JsonConvert.DeserializeObject<Info>(response);

            return info;
        }

        /// <inheritdoc />
        public async Task<String> GetColorModeAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/colorMode");
            var colorMode = JsonConvert.DeserializeObject<String>(response);

            return colorMode;
        }

        /// <inheritdoc />
        public async Task SetHsvAsync(Int32 h, Int32 s, Int32 v)
        {
            var request = new HsvRequest(h, s, v);
            var json = JsonConvert.SerializeObject(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "state/");
        }

        /// <inheritdoc />
        public async Task SetRgbAsync(Int32 r, Int32 g, Int32 b)
        {
            var hsv = ColorConverter.RgbToHsv(r, g, b);
            var request = new HsvRequest(hsv.H, hsv.S, hsv.V);
            var json = JsonConvert.SerializeObject(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "state/");
        }

        public void Dispose() => this.Dispose(true);

        #endregion

        /// <summary>
        ///     Retrieves the current panel layout.
        ///     Requires authorization.
        /// </summary>
        /// <returns></returns>
        public async Task<Layout> GetLayoutAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("panelLayout/layout");
            var layout = JsonConvert.DeserializeObject<Layout>(response);
            return layout;
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    this._nanoleafHttpClient.Dispose();
                }

                this._isDisposed = true;
            }
        }


        #region User

        /// <summary>
        ///     Attempt to create a new user token object.
        /// </summary>
        /// <returns>New user token</returns>
        public async Task<UserToken> CreateTokenAsync()
        {
            var response = await this._nanoleafHttpClient.AddUserRequestAsync();
            var token = JsonConvert.DeserializeObject<UserToken>(response);

            return token;
        }

        /// <summary>Deletes the token asynchronous.</summary>
        /// <param name="userToken">The user token.</param>
        public async Task DeleteTokenAsync(String userToken) => await this._nanoleafHttpClient.DeleteUserRequest(userToken + "/");

        #endregion

        #region Power

        /// <inheritdoc />
        public async Task<Boolean> GetPowerStatusAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/on");
            var powerStatus = JsonConvert.DeserializeObject<PowerStatus>(response);

            return powerStatus?.Value ?? false;
        }


        /// <inheritdoc />
        public async Task TurnOnAsync()
        {
            var request = new OnOffRequest(true);
            var json = Serializer.Serialize(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "state/");
        }

        /// <inheritdoc />
        public async Task TurnOffAsync()
        {
            var request = new OnOffRequest(false);
            var json = Serializer.Serialize(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "state/");
        }

        #endregion

        #region Brightness

        /// <inheritdoc />
        public async Task<Brightness> GetBrightnessInfoAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/brightness");
            var brightnessInfo = JsonConvert.DeserializeObject<Brightness>(response);

            return brightnessInfo;
        }

        /// <inheritdoc />
        public async Task<Int32> GetBrightnessAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/brightness/value");
            var brightnessInfo = JsonConvert.DeserializeObject<Int32>(response);

            return brightnessInfo;
        }

        /// <inheritdoc />
        public async Task<Int32> GetBrightnessMaxValueAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/brightness/max");
            var brightnessMaxValue = JsonConvert.DeserializeObject<Int32>(response);

            return brightnessMaxValue;
        }

        /// <inheritdoc />
        public async Task<Int32> GetBrightnessMinValueAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/brightness/min");
            var brightnessMinValue = JsonConvert.DeserializeObject<Int32>(response);

            return brightnessMinValue;
        }

        /// <inheritdoc />
        public async Task SetBrightnessAsync(Int32 targetBrightness, Int32 time = 0)
        {
            var request = new SetBrightnessModel(targetBrightness, time);
            var json = Serializer.Serialize(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "state/");
        }

        /// <inheritdoc />
        public async Task RaiseBrightnessAsync(Int32 value)
        {
            var request = new IncrementBrightnessModel(value);
            var json = Serializer.Serialize(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "state/");
        }

        /// <inheritdoc />
        public async Task LowerBrightnessAsync(Int32 value)
        {
            var request = new IncrementBrightnessModel(-value);
            var json = Serializer.Serialize(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "state/");
        }

        #endregion

        #region Hue

        /// <inheritdoc />
        public async Task<Hue> GetHueInfoAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/hue");
            var hueInfo = JsonConvert.DeserializeObject<Hue>(response);

            return hueInfo;
        }

        /// <inheritdoc />
        public async Task<Int32> GetHueAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/hue/value");
            var hue = JsonConvert.DeserializeObject<Int32>(response);

            return hue;
        }

        /// <inheritdoc />
        public async Task<Int32> GetHueMaxValueAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/hue/max");
            var hueMaxValue = JsonConvert.DeserializeObject<Int32>(response);

            return hueMaxValue;
        }

        /// <inheritdoc />
        public async Task<Int32> GetHueMinValueAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/hue/min");
            var hueMinValue = JsonConvert.DeserializeObject<Int32>(response);

            return hueMinValue;
        }

        /// <inheritdoc />
        public async Task SetHueAsync(Int32 targetHue)
        {
            var request = new SetHueModel(targetHue);
            var json = Serializer.Serialize(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "state/");
        }

        /// <inheritdoc />
        public async Task RaiseHueAsync(Int32 value)
        {
            var request = new IncrementHueModel(value);
            var json = Serializer.Serialize(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "state/");
        }

        /// <inheritdoc />
        public async Task LowerHueAsync(Int32 value)
        {
            var request = new IncrementHueModel(-value);
            var json = Serializer.Serialize(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "state/");
        }

        #endregion

        #region Saturation

        /// <inheritdoc />
        public async Task<Saturation> GetSaturationInfoAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/sat");
            var hueInfo = JsonConvert.DeserializeObject<Saturation>(response);

            return hueInfo;
        }

        /// <inheritdoc />
        public async Task<Int32> GetSaturationAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/sat/value");
            var saturation = JsonConvert.DeserializeObject<Int32>(response);

            return saturation;
        }

        /// <inheritdoc />
        public async Task<Int32> GetSaturationMaxValueAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/sat/max");
            var saturationMaxValue = JsonConvert.DeserializeObject<Int32>(response);

            return saturationMaxValue;
        }

        /// <inheritdoc />
        public async Task<Int32> GetSaturationMinValueAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/sat/min");
            var saturationMinValue = JsonConvert.DeserializeObject<Int32>(response);

            return saturationMinValue;
        }

        /// <inheritdoc />
        public async Task SetSaturationAsync(Int32 targetSat)
        {
            var request = new SetSaturationModel(targetSat);
            var json = Serializer.Serialize(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "state/");
        }

        /// <inheritdoc />
        public async Task RaiseSaturationAsync(Int32 value)
        {
            var request = new IncrementSaturationModel(value);
            var json = Serializer.Serialize(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "state/");
        }

        /// <inheritdoc />
        public async Task LowerSaturationAsync(Int32 value)
        {
            var request = new IncrementSaturationModel(-value);
            var json = Serializer.Serialize(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "state/");
        }

        #endregion

        #region Color Temperature

        /// <inheritdoc />
        public async Task<ColorTemperature> GetTemperatureInfoAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/ct");
            var colorTemperatureInfo = JsonConvert.DeserializeObject<ColorTemperature>(response);

            return colorTemperatureInfo;
        }

        /// <inheritdoc />
        public async Task<Int32> GetColorTemperatureAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/ct/value");
            var colorTemperature = JsonConvert.DeserializeObject<Int32>(response);

            return colorTemperature;
        }

        /// <inheritdoc />
        public async Task<Int32> GetColorTemperatureMaxValueAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/ct/max");
            var colorTemperatureMaxValue = JsonConvert.DeserializeObject<Int32>(response);

            return colorTemperatureMaxValue;
        }

        /// <inheritdoc />
        public async Task<Int32> GetColorTemperatureMinValueAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("state/ct/min");
            var colorTemperatureMinValue = JsonConvert.DeserializeObject<Int32>(response);

            return colorTemperatureMinValue;
        }

        /// <inheritdoc />
        public async Task SetColorTemperatureAsync(Int32 targetCt)
        {
            var request = new SetColorTemperatureModel(targetCt);
            var json = Serializer.Serialize(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "state/");
        }

        /// <inheritdoc />
        public async Task RaiseColorTemperatureAsync(Int32 value)
        {
            var request = new IncrementColorTemperatureModel(value);
            var json = Serializer.Serialize(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "state/");
        }

        /// <inheritdoc />
        public async Task LowerColorTemperatureAsync(Int32 value)
        {
            var request = new IncrementColorTemperatureModel(-value);
            var json = Serializer.Serialize(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "state/");
        }

        #endregion

        #region Effects

        /// <inheritdoc />
        public async Task<String> GetCurrentEffectAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("effects/select");
            var effect = JsonConvert.DeserializeObject<String>(response);

            return effect;
        }

        /// <inheritdoc />
        public async Task<List<String>> GetEffectsAsync()
        {
            var response = await this._nanoleafHttpClient.SendGetRequest("effects/effectsList");
            var effectsList = JsonConvert.DeserializeObject<List<String>>(response);

            return effectsList;
        }

        /// <inheritdoc />
        public async Task SetEffectAsync(String effectName)
        {
            var request = new SelectEffectModel(effectName);
            var json = JsonConvert.SerializeObject(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "effects/");
        }

        /// <inheritdoc />
        public async Task StartExternalAsync(String version = "v2")
        {
            var request = new SelectEternalModel(version);
            var json = JsonConvert.SerializeObject(request);

            await this._nanoleafHttpClient.SendPutRequest(json, "effects/");
        }

        #endregion
    }
}