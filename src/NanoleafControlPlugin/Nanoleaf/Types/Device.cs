// This file is part of the NanoleafControlPlugin project.
// 
// Copyright (c) 2022 Dominic Ris
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Types
{
    #region

    using System;
    using System.Threading;
    using System.Threading.Tasks;

    #endregion

    public sealed class Device
    {
        private Boolean _authorized;

        private CancellationTokenSource _disconnectedCancellationTokenSource;
        private Boolean _isDisconnecting;

        public Device(String id, NanoleafClient client, String displayName)
        {
            this.Id = id;
            this.DisplayName = displayName;
            this.Client = client;
            this._disconnectedCancellationTokenSource = new CancellationTokenSource();
        }

        public String Id { get; }
        public String DisplayName { get; }

        public Boolean Authorized
        {
            get => this._authorized;
            private set
            {
                if (this._authorized == value)
                {
                    return;
                }

                this._authorized = value;
                this.AuthenticationChanged.Invoke(this, this);
            }
        }

        public NanoleafClient Client { get; }
        private CancellationToken DisconnectedToken => this._disconnectedCancellationTokenSource.Token;

        public MinMaxInfo MinMaxInfo { get; } = new MinMaxInfo();

        public void Authorize(String authToken)
        {
            this.Client.Authorize(authToken);
            this.SetMinMaxValues();
            this.Authorized = true;
        }

        public async Task StartDisconnect()
        {
            this._isDisconnecting = true;
            try
            {
                this.DisconnectedToken.ThrowIfCancellationRequested();
                await Task.Delay(TimeSpan.FromSeconds(60), this.DisconnectedToken);
                this.Client.Dispose();
                this.Disconnected.Invoke(this, this);
            }
            catch (OperationCanceledException)
            {
                this._disconnectedCancellationTokenSource = new CancellationTokenSource();
                this.Reconnected.Invoke(this, this);
            }
        }

        public void StopDisconnect()
        {
            if (this._isDisconnecting)
            {
                this._disconnectedCancellationTokenSource.Cancel();
            }
        }

        public event EventHandler<Device> Disconnected = delegate { };
        public event EventHandler<Device> Reconnected = delegate { };
        public event EventHandler<Device> AuthenticationChanged = delegate { };

        private void SetMinMaxValues()
        {
            var brightness = this.Client.GetBrightnessInfoAsync().Result;
            var colorTemperature = this.Client.GetTemperatureInfoAsync().Result;
            var hue = this.Client.GetHueInfoAsync().Result;
            var saturation = this.Client.GetSaturationInfoAsync().Result;

            this.MinMaxInfo.Brightness = new MinMaxInfo.MinMaxValue(brightness.Minimum, brightness.Maximum);
            this.MinMaxInfo.ColorTemperature = new MinMaxInfo.MinMaxValue(colorTemperature.Minimum, colorTemperature.Maximum);
            this.MinMaxInfo.Hue = new MinMaxInfo.MinMaxValue(hue.Minimum, hue.Maximum);
            this.MinMaxInfo.Saturation = new MinMaxInfo.MinMaxValue(saturation.Minimum, saturation.Maximum);
        }
    }

    public class MinMaxInfo
    {
        public MinMaxValue Brightness { get; set; }
        public MinMaxValue ColorTemperature { get; set; }
        public MinMaxValue Hue { get; set; }
        public MinMaxValue Saturation { get; set; }

        #region Nested type: MinMaxValue

        public class MinMaxValue
        {
            public MinMaxValue(Int32 min, Int32 max)
            {
                this.Min = min;
                this.Max = max;
            }

            public Int32 Min { get; }
            public Int32 Max { get; }

            // Deconstructor
            public void Deconstruct(out Int32 min, out Int32 max)
            {
                min = this.Min;
                max = this.Max;
            }
        }

        #endregion
    }
}