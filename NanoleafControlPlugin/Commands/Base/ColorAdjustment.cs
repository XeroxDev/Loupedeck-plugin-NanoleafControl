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

namespace Loupedeck.NanoleafControlPlugin.Commands.Base
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    using Helper;

    using Nanoleaf.Types;

    public abstract class ColorAdjustment : NanoleafPluginDynamicAdjustment
    {
        /// <summary>
        /// Cached values to not flood the API with requests.
        /// </summary>
        private readonly List<ValueInfo> _cachedValues = new();

        protected ColorAdjustment(String displayName) : base(true)
        {
            this.DisplayName = displayName;
            this.SuperGroupName = displayName;
        }

        /// <inheritdoc/>
        protected override async Task DeviceFound(Object sender, Device device) => await this.AuthenticationChanged(sender, device);

        /// <inheritdoc/>
        protected override async Task AuthenticationChanged(Object sender, Device device)
        {
            if (!device.Authorized)
            {
                this._cachedValues.RemoveAll(x => x.Id == device.Id);
                return;
            }

            var brightness = await this.GetValue(device);
            this._cachedValues.Add(new ValueInfo(device.Id, brightness, new Debouncer(TimeSpan.FromMilliseconds(300), () => this.DebouncedSet(device).Wait())));
        }

        /// <inheritdoc/>
        protected override Task DeviceLost(Object _, Device device)
        {
            this._cachedValues.RemoveAll(x => x.Id == device.Id);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            if (!this.TryGetDevice(actionParameter, out var device, true))
            {
                return;
            }

            var info = this._cachedValues.Find(x => x.Id == device.Id);

            if (info == null)
            {
                return;
            }

            var (min, max) = this.GetMinMax(device);

            if (diff + info.Value < min)
            {
                diff = min - info.Value;
            }
            else if (diff + info.Value > max)
            {
                diff = max - info.Value;
            }

            info.Diff += diff;
            info.Debouncer.Invoke();
            this.AdjustmentValueChanged(info.Id);
        }

        /// <inheritdoc/>
        protected override async Task TimerTick()
        {
            foreach (var info in this._cachedValues)
            {
                var device = this.GetDevice(info.Id);
                if (!device.Authorized)
                {
                    continue;
                }

                var current = await this.GetValue(device);
                info.Value = current;
                this.AdjustmentValueChanged(info.Id);
            }
        }

        private async Task DebouncedSet(Device device)
        {
            if (!device.Authorized)
            {
                return;
            }

            var info = this._cachedValues.Find(x => x.Id == device.Id);
            var current = await this.GetValue(device);
            var (min, max) = this.GetMinMax(device);
            var newValue = Math.Max(min, Math.Min(max, current + info.Diff));

            if (newValue == current)
            {
                info.Diff = 0;
                info.Value = newValue;
                return;
            }

            await this.SetValue(device, newValue);
            info.Diff = 0;
            info.Value = newValue;
            this.AdjustmentValueChanged(info.Id);
        }

        /// <inheritdoc/>
        protected override BitmapImage GetAdjustmentImage(String actionParameter, PluginImageSize imageSize)
        {
            if (String.IsNullOrEmpty(actionParameter) || !this.TryGetDevice(actionParameter, out var device, true))
            {
                return base.GetAdjustmentImage(actionParameter, imageSize);
            }

            var info = this._cachedValues.Find(x => x.Id == device.Id);
            if (info == null)
            {
                return base.GetAdjustmentImage(actionParameter, imageSize);
            }

            var (min, max) = this.GetMinMax(device);
            var currentValue = Math.Max(min, Math.Min(max, info.Value + info.Diff));

            return DrawingHelper.DrawVolumeBar(imageSize, new BitmapColor(156, 156, 156), BitmapColor.White, currentValue, min, max);
        }

        /// <summary>
        /// Gets the color value.
        /// </summary>
        /// <param name="client">Client of the device</param>
        /// <returns>The color value.</returns>
        protected abstract Task<Int32> GetValue(Device client);

        /// <summary>
        /// Sets the value of the color adjustment.
        /// </summary>
        /// <param name="client">Client of the device</param>
        /// <param name="value">The value to set.</param>
        /// <returns>A awaitable task.</returns>
        protected abstract Task SetValue(Device client, Int32 value);

        /// <summary>
        /// Gets the min and max values of the color adjustment.
        /// </summary>
        /// <param name="device">The device</param>
        /// <returns>The min and max values.</returns>
        protected abstract MinMaxInfo.MinMaxValue GetMinMax(Device device);

        #region Nested type: ValueInfo

        /// <summary>
        /// The value info.
        /// </summary>
        private class ValueInfo
        {
            public ValueInfo(String id, Int32 value, Debouncer debouncer, Int32 diff = 0)
            {
                this.Id = id;
                this.Value = value;
                this.Debouncer = debouncer;
                this.Diff = diff;
            }

            public String Id { get; }
            public Int32 Value { get; set; }
            public Int32 Diff { get; set; }

            public Debouncer Debouncer { get; }
        }

        #endregion
    }
}