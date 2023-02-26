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

namespace Loupedeck.NanoleafControlPlugin.Actions
{
    using System;

    using Base;

    using Helper;

    using SkiaSharp;

    public class ToggleStateCommand : NanoleafPluginDynamicCommand
    {
        public ToggleStateCommand()
        {
            this.DisplayName = "Toggle on/off";
            this.SuperGroupName = "ToggleState";
        }

        /// <summary>
        ///     Toggles device on / off state.
        /// </summary>
        /// <param name="actionParameter">The id of the device</param>
        protected override void RunCommand(String actionParameter)
        {
            if (!this.TryGetDevice(actionParameter, out var device, true))
            {
                return;
            }

            var client = device.Client;
            var isPowered = client.GetPowerStatusAsync().GetAwaiter().GetResult();
            (isPowered ? client.TurnOffAsync() : client.TurnOnAsync()).GetAwaiter().GetResult();
            this.ActionImageChanged(actionParameter);
        }

        /// <summary>
        ///     Shows the state of the device.
        /// </summary>
        /// <param name="actionParameter">The id of the device</param>
        /// <param name="imageSize"></param>
        /// <returns></returns>
        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (!this.TryGetDevice(actionParameter, out var device, true))
            {
                return DrawingHelper.DrawDefaultImage("Not available", device?.DisplayName ?? "Unknown", SKColors.Red);
            }

            var client = device.Client;
            var isPowered = client.GetPowerStatusAsync().GetAwaiter().GetResult();
            return DrawingHelper.DrawDefaultImage(isPowered ? "On" : "Off", device.DisplayName, isPowered ? SKColors.Green : SKColors.Gray);
        }
    }
}